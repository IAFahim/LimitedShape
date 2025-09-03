using AtomicSimulation.Core;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AtomicSimulation.Authoring
{
    [System.Serializable]
    public class ShellConfiguration
    {
        [Tooltip("Radius of this electron shell")]
        public float radius = 0.5f;

        [Tooltip("Maximum electrons this shell can hold")]
        public int maxElectrons = 2;

        [Tooltip("Color for visualizing this shell")]
        public Color shellColor = Color.white;

        [Tooltip("Whether to show this shell in gizmos")]
        public bool showInGizmo = true;
    }

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
        [Range(1, 118)] public byte atomicNumber = 1;

        public byte maxAtomicNumber = 118;

        public int[] maxPerShells =
        {
            2, // K shell
            8, // L shell  
            18, // M shell
            32, // N shell
            32, // O shell
            18, // P shell
            8 // Q shell
        };

        // don't change we need this scale to gamify
        public float[] shellRadius =
        {
            0.5f, // K shell (n=1, Bohr radius for hydrogen)
            .65f, // L shell (n=2)
            0.8f, // M shell (n=3)
            1.0f, // N shell (n=4)
            1.2f, // O shell (n=5)
            1.35f, // P shell (n=6)
            1.5f // Q shell (n=7)
        };


        public class AtomicSimulationBaker : Baker<AtomicSimulationAuthoring>
        {
            public override void Bake(AtomicSimulationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new SimulationConfig
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
                    Timer = authoring.timer,
                    CurrentMaxAtomicNumber = authoring.maxAtomicNumber
                });

                AddComponent(entity, new AtomicNumber { Value = authoring.atomicNumber });
                AddComponent(entity, new AtomCenter { Position = authoring.transform.position });

                AddComponent<AtomReady>(entity);
                SetComponentEnabled<AtomReady>(entity, false);
                
                BakeBlobs(authoring, entity);
            }

            private void BakeBlobs(AtomicSimulationAuthoring authoring, Entity entity)
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var electronShellData = ref builder.ConstructRoot<ElectronShellData>();
                var maxShells = builder.Allocate(ref electronShellData.MaxPerShells, authoring.maxPerShells.Length);
                var shellRadius = builder.Allocate(ref electronShellData.ShellRadii, authoring.shellRadius.Length);
                for (var i = 0; i < authoring.maxPerShells.Length; i++) maxShells[i] = authoring.maxPerShells[i];
                for (var i = 0; i < authoring.shellRadius.Length; i++) shellRadius[i] = authoring.shellRadius[i];
                var blobReference = builder.CreateBlobAssetReference<ElectronShellData>(Allocator.Persistent);
                AddComponent(entity, new ElectronShellBlob { BlobAssetRef = blobReference });
                builder.Dispose();
                
            }
        }
    }
}