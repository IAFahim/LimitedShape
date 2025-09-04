using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class NeutronAuthoring : MonoBehaviour
    {
        public class NeutronBaker : Baker<NeutronAuthoring>
        {
            public override void Bake(NeutronAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<Neutron>(entity);
            }
        }
    }
}