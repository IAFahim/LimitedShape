using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class ElectronAuthoring : MonoBehaviour
    {
        public class ElectronBaker : Baker<ElectronAuthoring>
        {
            public override void Bake(ElectronAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<Electron>(entity);
            }
        }
    }
}