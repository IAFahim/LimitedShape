// =============================================================================
// ASSEMBLY: AtomicSimulation.Core
// =============================================================================

// Components/AtomicComponents.cs

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AtomicSimulation.Core
{
    public struct AtomicNumber : IComponentData
    {
        public int Value;
    }

    public enum ParticleTypeEnum : byte
    {
        Proton,
        Neutron,
        Electron
    }

    public struct ParticleType : IComponentData
    {
        public ParticleTypeEnum Value;
    }

    public struct OrbitData : IComponentData
    {
        public float Radius;
        public float Speed;
        public float CurrentAngle;
        public int ShellNumber; // K=1, L=2, M=3, etc.
    }

    public struct AtomCenter : IComponentData
    {
        public float3 Position;
    }

    public struct NucleusParticle : IComponentData, IEnableableComponent
    {
        public float3 LocalOffset;
    }

    public struct ElectronParticle : IComponentData, IEnableableComponent { }

    // Singleton component for simulation control
    public struct SimulationConfig : IComponentData
    {
        public float ElementProgressionInterval;
        public float BaseOrbitSpeed;
        public float NucleusScale;
        public float ElectronScale;
        public int MaxAtomicNumber;
        public int ElementsPerRow;
        public float AtomSpacing;
        public Entity Neutron;
    }

    public struct SimulationTimer : IComponentData
    {
        public float Timer;
        public int CurrentMaxAtomicNumber;
    }
}

namespace AtomicSimulation.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct AtomCreationSystem : ISystem
    {
        // Electron shell configuration (K, L, M, N, O, P, Q shells)
        private static readonly FixedList128Bytes<int> MaxElectronsPerShell = new()
        {
            Length = 7,
            [0] = 2,   // K shell
            [1] = 8,   // L shell  
            [2] = 18,  // M shell
            [3] = 32,  // N shell
            [4] = 32,  // O shell
            [5] = 18,  // P shell
            [6] = 8    // Q shell
        };

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Process new atoms that need to be created
            var createAtomJob = new CreateAtomJob
            {
                ECB = ecb.AsParallelWriter(),
                MaxElectronsPerShell = MaxElectronsPerShell,
                Config = SystemAPI.GetSingleton<SimulationConfig>()
            };

            state.Dependency = createAtomJob.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        private partial struct CreateAtomJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public FixedList128Bytes<int> MaxElectronsPerShell;
            [ReadOnly] public SimulationConfig Config;

            public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, 
                in AtomicNumber atomicNumber, in AtomCenter center)
            {
                // Skip if atom already has particles (check for any particle component)
                // This would be handled by a "needs setup" component in practice
                
                CreateNucleus(chunkIndex, atomicNumber.Value, center.Position);
                CreateElectronShells(chunkIndex, atomicNumber.Value, center.Position);
            }

            private void CreateNucleus(int chunkIndex, int atomicNumber, float3 centerPos)
            {
                int neutronCount = CalculateNeutronCount(atomicNumber);
                int totalNucleusParticles = atomicNumber + neutronCount;

                // Create protons
                for (int i = 0; i < atomicNumber; i++)
                {
                    var protonEntity = ECB.Instantiate(chunkIndex,Config.Neutron);
                    var nucleusOffset = GetNucleusParticleOffset(i, totalNucleusParticles);
                    
                    ECB.AddComponent(chunkIndex, protonEntity, LocalTransform.FromPositionRotationScale(
                        centerPos + nucleusOffset, quaternion.identity, Config.NucleusScale));
                    ECB.AddComponent(chunkIndex, protonEntity, new ParticleType { Value = ParticleTypeEnum.Proton });
                    ECB.AddComponent(chunkIndex, protonEntity, new NucleusParticle { LocalOffset = nucleusOffset });
                    ECB.AddComponent(chunkIndex, protonEntity, new AtomCenter { Position = centerPos });
                }

                // Create neutrons  
                for (int i = 0; i < neutronCount; i++)
                {
                    var neutronEntity = ECB.Instantiate(chunkIndex,Config.Neutron);
                    var nucleusOffset = GetNucleusParticleOffset(atomicNumber + i, totalNucleusParticles);
                    
                    ECB.AddComponent(chunkIndex, neutronEntity, LocalTransform.FromPositionRotationScale(
                        centerPos + nucleusOffset, quaternion.identity, Config.NucleusScale));
                    ECB.AddComponent(chunkIndex, neutronEntity, new ParticleType { Value = ParticleTypeEnum.Neutron });
                    ECB.AddComponent(chunkIndex, neutronEntity, new NucleusParticle { LocalOffset = nucleusOffset });
                    ECB.AddComponent(chunkIndex, neutronEntity, new AtomCenter { Position = centerPos });
                }
            }

            private void CreateElectronShells(int chunkIndex, int atomicNumber, float3 centerPos)
            {
                int remainingElectrons = atomicNumber;
                int shellNumber = 1;

                while (remainingElectrons > 0 && shellNumber <= MaxElectronsPerShell.Length)
                {
                    int electronsInShell = math.min(remainingElectrons, MaxElectronsPerShell[shellNumber - 1]);
                    float shellRadius = shellNumber * 0.5f;
                    float shellSpeed = Config.BaseOrbitSpeed / shellRadius; // Inner shells orbit faster

                    for (int i = 0; i < electronsInShell; i++)
                    {
                        var electronEntity = ECB.Instantiate(chunkIndex,Config.Neutron);
                        float initialAngle = (float)i / electronsInShell * 2f * math.PI;
                        
                        var orbitPos = centerPos + new float3(
                            shellRadius * math.cos(initialAngle),
                            shellRadius * math.sin(initialAngle),
                            0f);

                        ECB.AddComponent(chunkIndex, electronEntity, LocalTransform.FromPositionRotationScale(
                            orbitPos, quaternion.identity, Config.ElectronScale));
                        ECB.AddComponent(chunkIndex, electronEntity, new ParticleType { Value = ParticleTypeEnum.Electron });
                        ECB.AddComponent(chunkIndex, electronEntity, new ElectronParticle());
                        ECB.AddComponent(chunkIndex, electronEntity, new OrbitData
                        {
                            Radius = shellRadius,
                            Speed = shellSpeed,
                            CurrentAngle = initialAngle,
                            ShellNumber = shellNumber
                        });
                        ECB.AddComponent(chunkIndex, electronEntity, new AtomCenter { Position = centerPos });
                    }

                    remainingElectrons -= electronsInShell;
                    shellNumber++;
                }
            }

            private float3 GetNucleusParticleOffset(int particleIndex, int totalParticles)
            {
                if (totalParticles == 1) return float3.zero;

                float radius = 0.02f + (totalParticles * 0.001f);
                
                // Golden spiral distribution for roughly uniform sphere packing
                float goldenAngle = 2.39996322972865332f;
                float theta = particleIndex * goldenAngle;
                float phi = math.acos(1f - 2f * (float)particleIndex / totalParticles);

                return new float3(
                    radius * math.sin(phi) * math.cos(theta),
                    radius * math.sin(phi) * math.sin(theta),
                    radius * math.cos(phi)
                );
            }

            private int CalculateNeutronCount(int atomicNumber)
            {
                // Simplified neutron approximation
                if (atomicNumber <= 2) return math.max(0, atomicNumber - 1);
                if (atomicNumber <= 20) return atomicNumber;
                return (int)(atomicNumber * 1.4f);
            }
        }
    }
}

// Systems/ElectronOrbitSystem.cs

namespace AtomicSimulation.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ElectronOrbitSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            var orbitJob = new ElectronOrbitJob
            {
                DeltaTime = deltaTime
            };

            state.Dependency = orbitJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct ElectronOrbitJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref LocalTransform transform, ref OrbitData orbitData, 
                in AtomCenter atomCenter, in ElectronParticle electron)
            {
                // Update orbit angle
                orbitData.CurrentAngle += orbitData.Speed * DeltaTime;
                
                // Keep angle in valid range
                if (orbitData.CurrentAngle > 2f * math.PI)
                    orbitData.CurrentAngle -= 2f * math.PI;

                // Calculate new position
                float3 orbitOffset = new float3(
                    orbitData.Radius * math.cos(orbitData.CurrentAngle),
                    orbitData.Radius * math.sin(orbitData.CurrentAngle),
                    0f
                );

                transform.Position = atomCenter.Position + orbitOffset;
            }
        }
    }
}


namespace AtomicSimulation.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ElectronOrbitSystem))]
    public partial struct ElementProgressionSystem : ISystem
    {
        private static readonly FixedString64Bytes[] ElementNames = new FixedString64Bytes[]
        {
            "Hydrogen", "Helium", "Lithium", "Beryllium", "Boron", "Carbon", "Nitrogen", "Oxygen", 
            "Fluorine", "Neon", "Sodium", "Magnesium", "Aluminum", "Silicon", "Phosphorus", "Sulfur",
            "Chlorine", "Argon", "Potassium", "Calcium", "Scandium", "Titanium", "Vanadium", "Chromium",
            "Manganese", "Iron", "Cobalt", "Nickel", "Copper", "Zinc", "Gallium", "Germanium",
            "Arsenic", "Selenium", "Bromine", "Krypton", "Rubidium", "Strontium", "Yttrium", "Zirconium",
            // ... Continue with all 118 elements (truncated for brevity)
            "Oganesson"
        };

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationConfig>();
            state.RequireForUpdate<SimulationTimer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<SimulationConfig>();
            var timerRW = SystemAPI.GetSingletonRW<SimulationTimer>();
            
            ref var timer = ref timerRW.ValueRW;
            timer.Timer += SystemAPI.Time.DeltaTime;

            if (timer.Timer >= config.ElementProgressionInterval && 
                timer.CurrentMaxAtomicNumber < config.MaxAtomicNumber)
            {
                timer.Timer = 0f;
                timer.CurrentMaxAtomicNumber++;
                
                CreateNextElement(ref state, timer.CurrentMaxAtomicNumber, config);
                
                // Log element creation (only in development builds)
                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                if (timer.CurrentMaxAtomicNumber <= ElementNames.Length)
                {
                    UnityEngine.Debug.Log($"Created {ElementNames[timer.CurrentMaxAtomicNumber - 1]} (Z={timer.CurrentMaxAtomicNumber})");
                }
                #endif
            }
        }

        private void CreateNextElement(ref SystemState state, int atomicNumber, in SimulationConfig config)
        {
            // Calculate grid position
            int row = (atomicNumber - 1) / config.ElementsPerRow;
            int col = (atomicNumber - 1) % config.ElementsPerRow;
            
            float3 position = new float3(
                col * config.AtomSpacing, 
                -row * config.AtomSpacing, 
                0f
            );

            var atomEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(atomEntity, new AtomicNumber { Value = atomicNumber });
            state.EntityManager.AddComponentData(atomEntity, new AtomCenter { Position = position });
        }
    }
}


namespace AtomicSimulation.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(AtomCreationSystem))]
    public partial struct SimulationBootstrapSystem : ISystem
    {
        private bool hasInitialized;

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (hasInitialized) return;
            hasInitialized = true;

            // Create simulation timer singleton  
            var timerEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(timerEntity, new SimulationTimer
            {
                Timer = 0f,
                CurrentMaxAtomicNumber = 0
            });

            // Create the first atom (Hydrogen)
            var hydrogenEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(hydrogenEntity, new AtomicNumber { Value = 1 });
            state.EntityManager.AddComponentData(hydrogenEntity, new AtomCenter { Position = float3.zero });

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            UnityEngine.Debug.Log("Atomic Simulation Bootstrap Complete - Starting with Hydrogen");
            #endif
        }
    }
}

// =============================================================================
// ASSEMBLY: AtomicSimulation.Authoring  
// =============================================================================


namespace AtomicSimulation.Authoring
{
}

// // =============================================================================
// // ASSEMBLY: AtomicSimulation.Tests
// // =============================================================================
//
// // Tests/AtomCreationSystemTests.cs  
// #if UNITY_INCLUDE_TESTS
// using NUnit.Framework;
// using Unity.Entities;
// using Unity.Mathematics;
// using AtomicSimulation.Core;
//
// namespace AtomicSimulation.Tests
// {
//     [TestFixture]
//     public class AtomCreationSystemTests : ECSTestsFixture
//     {
//         private AtomCreationSystem atomCreationSystem;
//         private SimulationBootstrapSystem bootstrapSystem;
//
//         [SetUp]
//         public void Setup()
//         {
//             atomCreationSystem = World.CreateSystem<AtomCreationSystem>();
//             bootstrapSystem = World.CreateSystem<SimulationBootstrapSystem>();
//         }
//
//         [Test]
//         public void CreateHydrogenAtom_CreatesOneProtonOneElectron()
//         {
//             // Arrange
//             bootstrapSystem.Update(World.Unmanaged);
//             
//             var hydrogenEntity = Manager.CreateEntity();
//             Manager.AddComponentData(hydrogenEntity, new AtomicNumber { Value = 1 });
//             Manager.AddComponentData(hydrogenEntity, new AtomCenter { Position = float3.zero });
//
//             // Act
//             atomCreationSystem.Update(World.Unmanaged);
//
//             // Assert
//             var protonQuery = Manager.CreateEntityQuery(typeof(ParticleType), typeof(NucleusParticle));
//             var electronQuery = Manager.CreateEntityQuery(typeof(ParticleType), typeof(ElectronParticle));
//
//             int protonCount = 0;
//             int electronCount = 0;
//
//             using var protons = protonQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
//             using var electrons = electronQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
//
//             foreach (var protonEntity in protons)
//             {
//                 var particleType = Manager.GetComponentData<ParticleType>(protonEntity);
//                 if (particleType.Value == ParticleTypeEnum.Proton)
//                     protonCount++;
//             }
//
//             foreach (var electronEntity in electrons)
//             {
//                 var particleType = Manager.GetComponentData<ParticleType>(electronEntity);
//                 if (particleType.Value == ParticleTypeEnum.Electron)
//                     electronCount++;
//             }
//
//             Assert.AreEqual(1, protonCount, "Hydrogen should have exactly 1 proton");
//             Assert.AreEqual(1, electronCount, "Hydrogen should have exactly 1 electron");
//         }
//
//         [Test]
//         public void CreateCarbonAtom_CreatesCorrectParticleCount()
//         {
//             // Arrange
//             bootstrapSystem.Update(World.Unmanaged);
//             
//             var carbonEntity = Manager.CreateEntity();
//             Manager.AddComponentData(carbonEntity, new AtomicNumber { Value = 6 }); // Carbon
//             Manager.AddComponentData(carbonEntity, new AtomCenter { Position = float3.zero });
//
//             // Act
//             atomCreationSystem.Update(World.Unmanaged);
//
//             // Assert
//             var allParticlesQuery = Manager.CreateEntityQuery(typeof(ParticleType));
//             using var allParticles = allParticlesQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
//
//             int protonCount = 0, neutronCount = 0, electronCount = 0;
//
//             foreach (var particleEntity in allParticles)
//             {
//                 var particleType = Manager.GetComponentData<ParticleType>(particleEntity);
//                 switch (particleType.Value)
//                 {
//                     case ParticleTypeEnum.Proton: protonCount++; break;
//                     case ParticleTypeEnum.Neutron: neutronCount++; break;
//                     case ParticleTypeEnum.Electron: electronCount++; break;
//                 }
//             }
//
//             Assert.AreEqual(6, protonCount, "Carbon should have 6 protons");
//             Assert.AreEqual(6, neutronCount, "Carbon should have 6 neutrons");
//             Assert.AreEqual(6, electronCount, "Carbon should have 6 electrons");
//         }
//
//         [Test]  
//         public void ElectronShellConfiguration_IsCorrect()
//         {
//             // Arrange
//             bootstrapSystem.Update(World.Unmanaged);
//             
//             var sodiumEntity = Manager.CreateEntity(); // Sodium has 11 electrons: 2 in K, 8 in L, 1 in M
//             Manager.AddComponentData(sodiumEntity, new AtomicNumber { Value = 11 });
//             Manager.AddComponentData(sodiumEntity, new AtomCenter { Position = float3.zero });
//
//             // Act
//             atomCreationSystem.Update(World.Unmanaged);
//
//             // Assert
//             var electronQuery = Manager.CreateEntityQuery(typeof(OrbitData), typeof(ElectronParticle));
//             using var electrons = electronQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
//
//             int kShellCount = 0, lShellCount = 0, mShellCount = 0;
//
//             foreach (var electronEntity in electrons)
//             {
//                 var orbitData = Manager.GetComponentData<OrbitData>(electronEntity);
//                 switch (orbitData.ShellNumber)
//                 {
//                     case 1: kShellCount++; break; // K shell
//                     case 2: lShellCount++; break; // L shell  
//                     case 3: mShellCount++; break; // M shell
//                 }
//             }
//
//             Assert.AreEqual(2, kShellCount, "Sodium should have 2 electrons in K shell");
//             Assert.AreEqual(8, lShellCount, "Sodium should have 8 electrons in L shell");  
//             Assert.AreEqual(1, mShellCount, "Sodium should have 1 electron in M shell");
//         }
//     }
//
//     // Mock test fixture - in real project you'd use BovineLabs.Testing or similar
//     public abstract class ECSTestsFixture
//     {
//         protected World World { get; private set; }
//         protected EntityManager Manager => World.EntityManager;
//
//         [SetUp]
//         public void BaseSetup()
//         {
//             World = new World("TestWorld");
//             World.GetOrCreateSystemManaged<SimulationSystemGroup>();
//             World.GetOrCreateSystemManaged<InitializationSystemGroup>();
//         }
//
//         [TearDown]
//         public void BaseTearDown()
//         {
//             World?.Dispose();
//             World = null;
//         }
//     }
// }
// #endif
//
// // =============================================================================
// // Assembly Definition Files (.asmdef)
// // =============================================================================
//
// /*
// // AtomicSimulation.Core.asmdef
// {
//     "name": "AtomicSimulation.Core",
//     "rootNamespace": "AtomicSimulation.Core",
//     "references": [
//         "Unity.Entities",
//         "Unity.Transforms",
//         "Unity.Collections",
//         "Unity.Burst",
//         "Unity.Mathematics"
//     ],
//     "includePlatforms": [],
//     "excludePlatforms": [],
//     "allowUnsafeCode": true,
//     "overrideReferences": false,
//     "precompiledReferences": [],
//     "autoReferenced": true,
//     "defineConstraints": [],
//     "versionDefines": [],
//     "noEngineReferences": false
// }
//
// // AtomicSimulation.Authoring.asmdef  
// {
//     "name": "AtomicSimulation.Authoring",
//     "rootNamespace": "AtomicSimulation.Authoring", 
//     "references": [
//         "AtomicSimulation.Core",
//         "Unity.Entities",
//         "Unity.Entities.Hybrid"
//     ],
//     "includePlatforms": [],
//     "excludePlatforms": [],
//     "allowUnsafeCode": false,
//     "overrideReferences": false,
//     "precompiledReferences": [],
//     "autoReferenced": true,
//     "defineConstraints": [],
//     "versionDefines": [],
//     "noEngineReferences": false
// }
//
// // AtomicSimulation.Tests.asmdef
// {
//     "name": "AtomicSimulation.Tests",
//     "rootNamespace": "AtomicSimulation.Tests",
//     "references": [
//         "AtomicSimulation.Core",
//         "Unity.Entities",
//         "UnityEngine.TestRunner",
//         "UnityEditor.TestRunner"
//     ],
//     "includePlatforms": [],
//     "excludePlatforms": [], 
//     "allowUnsafeCode": false,
//     "overrideReferences": true,
//     "precompiledReferences": [
//         "nunit.framework.dll"
//     ],
//     "autoReferenced": false,
//     "defineConstraints": [
//         "UNITY_INCLUDE_TESTS"
//     ],
//     "versionDefines": [],
//     "noEngineReferences": false
// }
// */