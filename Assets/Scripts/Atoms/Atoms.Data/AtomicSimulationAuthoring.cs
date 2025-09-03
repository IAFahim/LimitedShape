using AtomicSimulation.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AtomicSimulation.Authoring
{
    public class AtomicSimulationAuthoring : MonoBehaviour
    {
        [Header("Simulation Settings")] [Tooltip("Time in seconds between creating new elements")]
        public float elementProgressionInterval = 3f;

        [Tooltip("Base orbital speed for electrons")]
        public float baseOrbitSpeed = 1f;

        [Header("Visual Settings")] [Tooltip("Scale of nucleus particles (protons/neutrons)")]
        public float nucleusScale = 0.1f;

        [Tooltip("Scale of electrons")] public float electronScale = 0.05f;

        [Header("Layout Settings")] [Tooltip("Number of atoms per row in the display grid")]
        public int elementsPerRow = 10;

        [Tooltip("Spacing between atoms in the grid")]
        public float atomSpacing = 3f;

        public float m = 0.001f;
        public float c = 0.02f;

        public GameObject neutron;
        public GameObject proton;
        public GameObject electron;


        [Header("Simulation")] public float timer = 0;
        public int currentMaxAtomicNumber = 1;

        [Header("Atom")] public byte atomicNumber=1;


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
                    NeutronPrefab = GetEntity(authoring.neutron, TransformUsageFlags.None),
                    ProtonPrefab = GetEntity(authoring.proton, TransformUsageFlags.None),
                    ElectronPrefab = GetEntity(authoring.electron, TransformUsageFlags.None),
                    M = authoring.m,
                    C = authoring.c,
                });

                AddComponent(entity, new SimulationTimer
                    {
                        Timer = authoring.timer, CurrentMaxAtomicNumber = authoring.currentMaxAtomicNumber
                    }
                );

                AddComponent(entity, new AtomicNumber { Value = authoring.atomicNumber });
                AddComponent(entity, new AtomCenter { Position = authoring.transform.position });
                
                AddComponent<AtomReady>(entity);
                SetComponentEnabled<AtomReady>(entity, false);
            }
        }
    }
}