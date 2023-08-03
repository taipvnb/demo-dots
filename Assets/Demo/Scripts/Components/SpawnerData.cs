using Unity.Entities;

namespace Demo.Scripts.Components
{
    public struct SpawnerData : IComponentData
    {
        public Entity Prefab;
        public bool isSpawn;
        public float NextSpawnTime;
        public float SpawnRate;
        public int SpawnCount;
        public int SpawnedCount;
        public int MaxSpawnedCount;
    }
}