using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class TargetPositionAuthoring : MonoBehaviour
    {
        public float3 value;

        public class TargetPositionBaker : Baker<TargetPositionAuthoring>
        {
            public override void Bake(TargetPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TargetPosition { Value = authoring.value });
            }
        }
    }
}