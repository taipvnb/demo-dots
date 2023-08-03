using Demo.Scripts.Components;
using Unity.Entities;
using UnityEngine;

namespace Demo.Scripts.Authorings
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public bool isSpawn;
        public float Delay;
        public float SpawnRate;
        public int SpawnCount;
        public int MaxSpawnedCount;
    }

    class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnerData
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                isSpawn = authoring.isSpawn,
                NextSpawnTime = authoring.Delay,
                SpawnRate = authoring.SpawnRate,
                SpawnCount = authoring.SpawnCount,
                MaxSpawnedCount = authoring.MaxSpawnedCount
            });
        }
    }
}