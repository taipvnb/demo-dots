using Demo.Scripts.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Demo.Scripts.Systems
{
    [BurstCompile]
    public partial struct UnitMovingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ProcessMoveJob
            {
                time = SystemAPI.Time.DeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }

    [BurstCompile]
    public partial struct ProcessMoveJob : IJobEntity
    {
        public double time;

        private void Execute(ref TargetData targetData,
            RefRW<LocalTransform> localTransform)
        {
            float3 direction = targetData.target - localTransform.ValueRW.Position;

            if (math.distance(localTransform.ValueRW.Position, targetData.target) > 0.1f)
            {
                localTransform.ValueRW.Position += math.normalize(direction) * (float)time * 10;
            }
        }
    }
}