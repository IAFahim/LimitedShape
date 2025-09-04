using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class ProtonAuthoring : MonoBehaviour
    {
        public class ProtonBaker : Baker<ProtonAuthoring>
        {
            public override void Bake(ProtonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<Proton>(entity);
            }
        }
    }
}