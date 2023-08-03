using System.Runtime.InteropServices;
using Demo.Scripts.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Demo.Scripts.Systems
{
    public partial struct SpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
            var seed = (uint) UnityEngine.Random.Range(0, 1000000);
            new ProcessSpawnerJob
            {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                BaseSeed = seed,
                Ecb = ecb
            }.ScheduleParallel();
        }

        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }

    [BurstCompile]
    public partial struct ProcessSpawnerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public uint BaseSeed;
        public double ElapsedTime;

        private void Execute([ChunkIndexInQuery] int chunkIndex, ref SpawnerData spawner)
        {
            if (spawner.isSpawn)
            {
                if (spawner.NextSpawnTime < ElapsedTime && spawner.SpawnedCount < spawner.MaxSpawnedCount)
                {
                    var random = new Unity.Mathematics.Random(BaseSeed);
                    for (int i = 0; i < spawner.SpawnCount; i++)
                    {
                        spawner.SpawnedCount++;
                        var position = new float3(random.NextFloat(-5, -9), random.NextFloat(-5, 5), 0);
                        Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
                        Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(position));
                    }
                    
                    spawner.NextSpawnTime = (float) ElapsedTime + spawner.SpawnRate;
                    
                    
                }
                else  if(spawner.SpawnedCount > spawner.MaxSpawnedCount)
                {
                    spawner.isSpawn = false;
                    spawner.SpawnedCount = 0;
                    spawner.MaxSpawnedCount = 0;
                }
            }
           
        }
    }
}