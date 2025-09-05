using AtomicSimulation.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Atoms.Atoms.Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float speed = 5f;

        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Enemy
                {
                    Speed = authoring.speed,
                    HasSetVelocity = false
                });
            }
        }
    }
}
