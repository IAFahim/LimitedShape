using Unity.Entities;

namespace AtomicSimulation.Authoring
{
    public class AtomicSimulationBaker : Baker<AtomicSimulationAuthoring>
    {
        public override void Bake(AtomicSimulationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new Core.SimulationConfig
            {
                ElementProgressionInterval = authoring.elementProgressionInterval,
                BaseOrbitSpeed = authoring.baseOrbitSpeed,
                NucleusScale = authoring.nucleusScale,
                ElectronScale = authoring.electronScale,
                MaxAtomicNumber = 118,
                ElementsPerRow = authoring.elementsPerRow,
                AtomSpacing = authoring.atomSpacing
            });
        }
    }
}