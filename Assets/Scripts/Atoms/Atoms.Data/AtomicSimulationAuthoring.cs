using AtomicSimulation.Core;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace AtomicSimulation.Authoring
{
    [System.Serializable]
    public class AtomicSimulationAuthoring : MonoBehaviour
    {
        [Header("Visual Settings")] [Tooltip("Scale of nucleus particles (protons/neutrons)")]
        public float nucleusScale = 0.1f;

        [Tooltip("Scale of electrons")] public float electronScale = 0.05f;

        public float m = 0.001f;
        public float c = 0.02f;

        public GameObject neutron;
        public GameObject proton;
        public GameObject electron;
        
        [Tooltip("Base orbital speed for electrons")]
        public float baseOrbitSpeed = 1f;

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

        public float forceFieldStrength = 10;
        
        [Header("Physics Parameters")]
        [Tooltip("Nuclear force strength for protons/neutrons")]
        public float nuclearForceStrength = 2f;
        
        [Tooltip("Electromagnetic force strength for electrons")]
        public float electromagneticForceStrength = 0.5f;
        
        [Tooltip("Orbital force multiplier for electrons")]
        public float electronOrbitForce = 1f;
        
        [Tooltip("Damping for nucleus particles (0-1)")]
        [Range(0f, 1f)]
        public float nucleusDamping = 0.8f;
        
        [Tooltip("Damping for electrons (0-1)")]
        [Range(0f, 1f)]
        public float electronDamping = 0.3f;


        public class AtomicSimulationBaker : Baker<AtomicSimulationAuthoring>
        {
            public override void Bake(AtomicSimulationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new SimulationConfig
                {
                    NucleusScale = authoring.nucleusScale,
                    ElectronScale = authoring.electronScale,
                    NeutronPrefab = GetEntity(authoring.neutron, TransformUsageFlags.None),
                    ProtonPrefab = GetEntity(authoring.proton, TransformUsageFlags.None),
                    ElectronPrefab = GetEntity(authoring.electron, TransformUsageFlags.None),
                    BaseOrbitSpeed = authoring.baseOrbitSpeed,
                    M = authoring.m,
                    C = authoring.c,
                    NuclearForceStrength = authoring.nuclearForceStrength,
                    ElectromagneticForceStrength = authoring.electromagneticForceStrength,
                    ElectronOrbitForce = authoring.electronOrbitForce,
                    NucleusDamping = authoring.nucleusDamping,
                    ElectronDamping = authoring.electronDamping
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


                AddComponent(entity, new GameState { IsPlaying = true });
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

#if UNITY_EDITOR
        /// <summary>
        /// Draws gizmos in the editor to visualize the atom's structure.
        /// This provides a static preview of the shells, nucleus, and initial electron positions.
        /// The animation itself can only be seen in Play Mode.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Ensure we have valid data to work with
            if (shellRadius == null || maxPerShells == null || shellRadius.Length == 0 || maxPerShells.Length == 0)
            {
                return;
            }

            var center = (float3)transform.position;

            // --- 1. Draw Electron Shells and Electrons ---
            int remainingElectrons = atomicNumber;
            int shellNumber = 1;

            while (remainingElectrons > 0 && shellNumber <= maxPerShells.Length)
            {
                int shellIndex = shellNumber - 1;
                if (shellIndex >= shellRadius.Length) break; // Safety check

                // The job multiplies shellNumber * shellRadius. We replicate that behavior here for consistency.
                float radius = shellNumber * shellRadius[shellIndex];

                // Draw the shell orbit path (the "animation" path)
                Gizmos.color = new Color(0f, 1f, 1f, 0.25f); // Cyan for shells
                Gizmos.DrawWireSphere(center, radius);

                // Calculate and draw electrons for this shell
                int electronsInThisShell = Mathf.Min(remainingElectrons, maxPerShells[shellIndex]);

                for (int i = 0; i < electronsInThisShell; i++)
                {
                    // Use the static method from AtomicData to get the initial position
                    AtomicData.ElectronInitialAngle(i, electronsInThisShell, center, radius, out _, out var orbitPos);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(orbitPos, electronScale);
                }

                remainingElectrons -= electronsInThisShell;
                shellNumber++;
            }

            // --- 2. Draw Nucleus Particles ---
            // Use the static methods from AtomicData to get particle counts
            AtomicData.SimplifiedNeutronNucleusCount(atomicNumber, out var neutrons, out var totalParticles);
            int protons = atomicNumber;

            // Draw protons
            Gizmos.color = new Color(1f, 0f, 0f, 0.75f); // Red for protons
            for (int i = 0; i < protons; i++)
            {
                // Use the same logic to get the offset
                AtomicData.GetNucleusParticleOffset(i, totalParticles, m, c, out var nucleusOffset);
                Gizmos.DrawSphere((Vector3)center + (Vector3)nucleusOffset, nucleusScale);
            }

            // Draw neutrons
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.75f); // Gray for neutrons
            for (int i = 0; i < neutrons; i++)
            {
                // The index for neutrons starts after the protons
                AtomicData.GetNucleusParticleOffset(protons + i, totalParticles, m, c, out var nucleusOffset);
                Gizmos.DrawSphere((Vector3)center + (Vector3)nucleusOffset, nucleusScale);
            }
        }
#endif
    }
}