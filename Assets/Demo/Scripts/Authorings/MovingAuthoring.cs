using System;
using Demo.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Demo.Scripts.Authorings
{
    public class MovingAuthoring : MonoBehaviour
    {
        public Vector2Int target;

        public class MovingBaker : Baker<MovingAuthoring>
        {
            public override void Bake(MovingAuthoring authoring)
            {
                AddComponent(new TargetData()
                {
                    target = new int3(authoring.target.x,authoring.target.y,0)
                });
            }
        }
    }
}