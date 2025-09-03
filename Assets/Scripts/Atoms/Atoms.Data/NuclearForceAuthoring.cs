using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class NuclearForceAuthoring : MonoBehaviour
    {
        [Header("Nuclear Force Settings")]
        [Tooltip("Strength multiplier for nuclear attraction")]
        public float strengthMultiplier = 2f;
        
        [Tooltip("Optimal distance from atom center")]
        public float optimalDistance = 0.3f;
        
        [Tooltip("Maximum distance where nuclear force applies")]
        public float maxDistance = 1f;

        public class NuclearForceBaker : Baker<NuclearForceAuthoring>
        {
            public override void Bake(NuclearForceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new NuclearForce
                {
                    StrengthMultiplier = authoring.strengthMultiplier,
                    OptimalDistance = authoring.optimalDistance,
                    MaxDistance = authoring.maxDistance
                });
            }
        }
    }
}