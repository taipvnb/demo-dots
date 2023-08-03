using Unity.Entities;
using Unity.Mathematics;

namespace Demo.Scripts.Components
{
    public struct TargetData : IComponentData
    {
        public int3 target;
    }
}