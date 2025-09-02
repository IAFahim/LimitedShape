using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Authoring
{
    public class AtomicSimulationAuthoring : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [Tooltip("Time in seconds between creating new elements")]
        public float elementProgressionInterval = 3f;
        
        [Tooltip("Base orbital speed for electrons")]
        public float baseOrbitSpeed = 1f;
        
        [Header("Visual Settings")]
        [Tooltip("Scale of nucleus particles (protons/neutrons)")]
        public float nucleusScale = 0.1f;
        
        [Tooltip("Scale of electrons")]
        public float electronScale = 0.05f;
        
        [Header("Layout Settings")]
        [Tooltip("Number of atoms per row in the display grid")]
        public int elementsPerRow = 10;
        
        [Tooltip("Spacing between atoms in the grid")]
        public float atomSpacing = 3f;
        
        [Header("Materials")]
        public Material protonMaterial;
        public Material neutronMaterial;  
        public Material electronMaterial;

        [Header("Mesh")] public GameObject neutron;
        
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
                    AtomSpacing = authoring.atomSpacing,
                    Neutron = GetEntity(authoring.neutron, TransformUsageFlags.None)
                });
            }
        }
    }
}