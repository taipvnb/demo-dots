using Unity.Entities;
using Unity.Mathematics;

namespace Demo.Scripts.Components
{
    public struct PathfindingParams : IComponentData
    {
        public int2 startPos;
        public int2 endPos;
    }
}