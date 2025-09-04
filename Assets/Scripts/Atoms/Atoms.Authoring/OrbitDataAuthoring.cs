using AtomicSimulation.Core;
using Unity.Entities;
using UnityEngine;

namespace Atoms.Atoms.Authoring
{
    public class OrbitDataAuthoring : MonoBehaviour
    {
        public float targetRadius;
        public float targetSpeed;
        public float currentAngle;
        public int shellNumber;
        public float orbitForceMultiplier;

        public class OrbitDataBaker : Baker<OrbitDataAuthoring>
        {
            public override void Bake(OrbitDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity,
                    new OrbitData
                    {
                        TargetRadius = authoring.targetRadius,
                        TargetSpeed = authoring.targetSpeed,
                        CurrentAngle = authoring.currentAngle,
                        ShellNumber = authoring.shellNumber,
                        OrbitForceMultiplier = authoring.orbitForceMultiplier
                    });
            }
        }
    }
}