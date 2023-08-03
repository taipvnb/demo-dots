using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using static Unity.Entities.SystemAPI;
using static Unity.Mathematics.math;
using ProjectDawn.LocalAvoidance;

namespace ProjectDawn.Navigation
{
    /// <summary>
    /// System that calculates avoidance direction from nearby agents.
    /// </summary>
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AgentForceSystemGroup))]
    public partial struct AgentSonarAvoidSystem : ISystem
    {
        BufferLookup<NavMeshWall> m_NavMeshWallLookup;
        BufferTypeHandle<NavMeshWall> m_NavMeshWallHandle;

        public void OnCreate(ref SystemState state)
        {
            m_NavMeshWallLookup = state.GetBufferLookup<NavMeshWall>(isReadOnly:true);
            m_NavMeshWallHandle = state.GetBufferTypeHandle<NavMeshWall>(isReadOnly:true);
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spatial = GetSingleton<AgentSpatialPartitioningSystem.Singleton>();

            m_NavMeshWallLookup.Update(ref state);
            m_NavMeshWallHandle.Update(ref state);

            new AgentAvoidJob
            {
                Spatial = spatial,
                NavMeshWallHandle = m_NavMeshWallHandle,
                NavMeshWallLookup = m_NavMeshWallLookup,
            }.ScheduleParallel();
        }

        [BurstCompile]
        unsafe partial struct AgentAvoidJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            [ReadOnly]
            public AgentSpatialPartitioningSystem.Singleton Spatial;

            [NativeDisableContainerSafetyRestriction]
            SonarAvoidance Sonar;

            [ReadOnly]
            public BufferTypeHandle<NavMeshWall> NavMeshWallHandle;
            [ReadOnly]
            public BufferLookup<NavMeshWall> NavMeshWallLookup;
            bool HasNavMeshWall;

            public void Execute(Entity entity, ref AgentBody body, in AgentShape shape, in AgentSonarAvoid avoid, in LocalTransform transform)
            {
                if (body.IsStopped)
                    return;

                float3 desiredDirection = body.Force;

                if (!SonarAvoidance.TryDirectionToRotation(desiredDirection, shape.GetUp(), out var rotation))
                    return;

                // Sonar should not extend pass the destination
                var sonarRadius = min(avoid.Radius, distance(body.Destination, transform.Position));

                var velocity = body.Velocity;
                if (math.lengthsq(velocity) <= 0.01f)
                {
                    velocity = desiredDirection * 0.01f;
                }

                // Recreate avoidance structure
                Sonar.Set(transform.Position, velocity, rotation, shape.Radius, sonarRadius, radians(180));

                // Add nearby agents as obstacles
                var action = new Action
                {
                    Sonar = Sonar,
                    Entity = entity,
                    Body = body,
                    Shape = shape,
                    Avoid = avoid,
                    Transform = transform,
                    DesiredDirection = desiredDirection,
                };
                if (shape.Type == ShapeType.Cylinder)
                {
                    Spatial.QueryCylinder(transform.Position, avoid.Radius, shape.Height, ref action);

                    if (HasNavMeshWall)
                    {
                        var walls = NavMeshWallLookup[entity];
                        // Add navmesh walls
                        for (int i = 0; i < walls.Length; ++i)
                        {
                            NavMeshWall wall = walls[i];

                            float extent = shape.Height * 0.5f;
                            float wallExtent = extent; // TODO: Use from baked navmesh agent id
                            if (abs((transform.Position.y + extent) - (wall.Start.y + wallExtent)) > extent + wallExtent)
                                continue;
                            if (abs((transform.Position.y + extent) - (wall.End.y + wallExtent)) > extent + wallExtent)
                                continue;

                            Sonar.InsertObstacleLineIgnoreRadius(wall.Start, wall.End);
                        }
                    }
                }
                else
                {
                    Spatial.QuerySphere(transform.Position, avoid.Radius, ref action);
                }

                // Add blocker behind the velocity
                // This will prevent situations where agent has on right and left equally good paths
                if (length(body.Velocity) > 1e-3f)
                    Sonar.InsertObstacle(normalizesafe(-body.Velocity), avoid.Angle);

                bool success = Sonar.FindClosestDirection(out float3 newDirection);

                // If blocked stop enabled, reset to previous direction
                if (!avoid.BlockedStop && !success)
                    newDirection = desiredDirection;

                body.Force = newDirection;
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                Sonar = new SonarAvoidance(Allocator.Temp);
                if (chunk.Has<NavMeshWall>())
                {
                    HasNavMeshWall = true;
                }
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
            {
                HasNavMeshWall = false;
                Sonar.Dispose();
            }

            struct Action : ISpatialQueryEntity
            {
                public SonarAvoidance Sonar;
                public Entity Entity;
                public AgentBody Body;
                public AgentShape Shape;
                public AgentSonarAvoid Avoid;
                public LocalTransform Transform;
                public float3 DesiredDirection;

                public void Execute(Entity otherEntity, AgentBody otherBody, AgentShape otherShape, LocalTransform otherTransform)
                {
                    // Skip itself
                    if (Entity == otherEntity)
                        return;

                    if (Avoid.Mode == SonarAvoidMode.IgnoreBehindAgents)
                    {
                        if (dot(DesiredDirection, normalizesafe(otherTransform.Position - Transform.Position)) < 0 && length(otherBody.Velocity) > 0)
                            return;
                    }

                    if (Shape.Type == ShapeType.Cylinder && otherShape.Type == ShapeType.Cylinder)
                    {
                        float extent = Shape.Height * 0.5f;
                        float otherExtent = otherShape.Height * 0.5f;
                        if (abs((Transform.Position.y + extent) - (otherTransform.Position.y + otherExtent)) > extent + otherExtent)
                            return;
                    }

                    Sonar.InsertObstacleCircle(otherTransform.Position, otherBody.Velocity, otherShape.Radius);
                }
            }
        }
    }
}
