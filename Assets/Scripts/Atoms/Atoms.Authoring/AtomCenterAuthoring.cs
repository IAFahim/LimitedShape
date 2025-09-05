using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class AtomCenterAuthoring : MonoBehaviour
    {
        public float3 position;

        public class AtomCenterBaker : Baker<AtomCenterAuthoring>
        {
            public override void Bake(AtomCenterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new AtomCenter { Position = authoring.position });
            }
        }
    }
}