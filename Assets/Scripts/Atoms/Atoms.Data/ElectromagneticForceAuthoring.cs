using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class ElectromagneticForceAuthoring : MonoBehaviour
    {
        [Header("Electromagnetic Force Settings")]
        [Tooltip("Charge strength for electromagnetic interactions")]
        public float chargeStrength = 0.5f;
        
        [Tooltip("Repulsion multiplier between like charges")]
        public float repulsionMultiplier = 0.3f;

        public class ElectromagneticForceBaker : Baker<ElectromagneticForceAuthoring>
        {
            public override void Bake(ElectromagneticForceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ElectromagneticForce
                {
                    ChargeStrength = authoring.chargeStrength,
                    RepulsionMultiplier = authoring.repulsionMultiplier
                });
            }
        }
    }
}