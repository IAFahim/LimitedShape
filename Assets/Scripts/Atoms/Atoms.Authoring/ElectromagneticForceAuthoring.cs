using AtomicSimulation.Core;
using Unity.Entities;
using UnityEngine;

namespace Atoms.Atoms.Authoring
{
    public class ElectromagneticForceAuthoring : MonoBehaviour
    {
        [Tooltip("Charge strength for electromagnetic interactions")]
        public float chargeStrength = 0.5f;

        public class ElectromagneticForceBaker : Baker<ElectromagneticForceAuthoring>
        {
            public override void Bake(ElectromagneticForceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ElectromagneticForce
                {
                    ChargeStrength = authoring.chargeStrength
                });
            }
        }
    }
}