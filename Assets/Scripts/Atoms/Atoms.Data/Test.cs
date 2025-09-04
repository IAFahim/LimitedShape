using System.Runtime.CompilerServices;
using AtomicSimulation.Authoring;
using AtomicSimulation.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Extensions;

namespace AtomicSimulation.Core
{
    public struct OrbitData : IComponentData
    {
        public float TargetRadius;
        public float TargetSpeed;
        public float CurrentAngle;
        public int ShellNumber; // K=1, L=2, M=3, etc.
        public float OrbitForceMultiplier;
    }

    public struct TargetPosition : IComponentData
    {
        public float3 Position;
    }

    public struct ElectronShellData
    {
        public BlobArray<int> MaxPerShells;
        public BlobArray<float> ShellRadii;
    }

    public struct ElectronShellBlob : IComponentData
    {
        public BlobAssetReference<ElectronShellData> BlobAssetRef;
    }
    
    public struct AtomicNumber : IComponentData { public byte Value; }
    public struct AtomCenter : IComponentData { public float3 Position; }

    public struct AtomReady : IComponentData, IEnableableComponent { }

    // --- PARTICLE TAGS ---
    public struct Proton : IComponentData { }

    public struct Neutron : IComponentData { }

    public struct Electron : IComponentData { }

    // Enhanced PID Controller for 3D physics
    public struct PID : IComponentData
    {
        // PID parameters
        public float Kp; // Proportional gain
        public float Ki; // Integral gain
        public float Kd; // Derivative gain
        
        // PID state
        public float3 Integral;
        public float3 PreviousError;
        
        public float MaxForce;
        public float IntegralClamp;
    }

    // Nuclear force component for protons/neutrons
    public struct NuclearForce : IComponentData
    {
        public float StrengthMultiplier;
        public float OptimalDistance;
        public float MaxDistance;
    }

    // Electromagnetic force component for electrons
    public struct ElectromagneticForce : IComponentData
    {
        public float ChargeStrength;
    }

    public struct GameState : IComponentData
    {
        public bool IsPlaying;
    }

    // Singleton component for simulation control
    public struct SimulationConfig : IComponentData
    {
        public float NucleusScale;
        public float ElectronScale;
        public float M;
        public float C;
        public Entity ProtonPrefab;
        public Entity NeutronPrefab;
        public Entity ElectronPrefab;
        public float BaseOrbitSpeed;
        
        public float ElectronOrbitForce;
        public float NucleusDamping;
        public float ElectronDamping;
    }

    public struct SimulationTimer : IComponentData
    {
        public float Timer;
        public int CurrentMaxAtomicNumber;
    }
}

[BurstCompile]
[WithPresent(typeof(AtomReady))]
public partial struct CreateAtomJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public SimulationConfig Config;
    [ReadOnly] public BlobAssetReference<ElectronShellData> BlobAssetRef;

    [BurstCompile]
    private void Execute(
        [ChunkIndexInQuery] int chunkIndex,
        in AtomicNumber atomicNumber,
        in AtomCenter center,
        EnabledRefRW<AtomReady> atomReady
    )
    {
        if (atomReady.ValueRO)
        {
            return;
        }

        CreateNucleus(chunkIndex, atomicNumber.Value, center.Position, Config.M, Config.C);
        CreateElectronShells(chunkIndex, atomicNumber.Value, center.Position);
        atomReady.ValueRW = true;
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateNucleus(int chunkIndex, int atomicNumber, float3 centerPos, float m, float c)
    {
        AtomicData.SimplifiedNeutronNucleusCount(atomicNumber, out var neutrons, out var nucleus);

        // Create protons
        for (int i = 0; i < atomicNumber; i++)
        {
            var protonEntity = ECB.Instantiate(chunkIndex, Config.ProtonPrefab);
            AtomicData.GetNucleusParticleOffset(i, nucleus, m, c, out var nucleusOffset);

            ECB.SetComponent(chunkIndex, protonEntity,
                LocalTransform.FromPositionRotationScale(
                    centerPos + nucleusOffset * 0.1f, // Start closer to center
                    quaternion.identity,
                    Config.NucleusScale
                )
            );
            
            ECB.SetComponent(chunkIndex, protonEntity, new AtomCenter { Position = centerPos });
        }

        // Create neutrons  
        for (int i = 0; i < neutrons; i++)
        {
            var neutronEntity = ECB.Instantiate(chunkIndex, Config.NeutronPrefab);
            AtomicData.GetNucleusParticleOffset(atomicNumber + i, nucleus, m, c, out var nucleusOffset);

            ECB.SetComponent(chunkIndex, neutronEntity,
                LocalTransform.FromPositionRotationScale(
                    centerPos + nucleusOffset * 0.1f, // Start closer to center
                    quaternion.identity,
                    Config.NucleusScale
                )
            );
            
            ECB.SetComponent(chunkIndex, neutronEntity, new AtomCenter { Position = centerPos });
        }
    }

    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateElectronShells(int chunkIndex, int atomicNumber, float3 centerPos)
    {
        int remainingElectrons = atomicNumber;
        int shellNumber = 1;

        ref var maxPerShells = ref BlobAssetRef.Value.MaxPerShells;
        var maxShellArrayLength = maxPerShells.Length;
        ref var shellRadii = ref BlobAssetRef.Value.ShellRadii;
        
        while (remainingElectrons > 0 && shellNumber <= maxShellArrayLength)
        {
            var shellIndex = shellNumber - 1;
            int electronsInShell = math.min(remainingElectrons, maxPerShells[shellIndex]);
            float shellRadius = shellNumber * shellRadii[shellIndex];
            float shellSpeed = Config.BaseOrbitSpeed / shellRadius; // Inner shells orbit faster

            for (int electronIndex = 0; electronIndex < electronsInShell; electronIndex++)
            {
                AtomicData.ElectronInitialAngle(electronIndex, electronsInShell, centerPos, shellRadius,
                    out var initialAngle, out var orbitPos);

                var electronEntity = ECB.Instantiate(chunkIndex, Config.ElectronPrefab);
                ECB.SetComponent(chunkIndex, electronEntity, LocalTransform.FromPositionRotationScale(orbitPos, quaternion.identity, Config.ElectronScale));
                
                // Add electron components
                ECB.SetComponent(chunkIndex, electronEntity, new OrbitData
                {
                    TargetRadius = shellRadius,
                    TargetSpeed = shellSpeed,
                    CurrentAngle = initialAngle,
                    ShellNumber = shellNumber,
                    OrbitForceMultiplier = Config.ElectronOrbitForce
                });
                ECB.SetComponent(chunkIndex, electronEntity, new AtomCenter { Position = centerPos });
            }

            remainingElectrons -= electronsInShell;
            shellNumber++;
        }
    }
}

namespace AtomicSimulation.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct NuclearForceSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var config = SystemAPI.GetSingleton<SimulationConfig>();

            var nuclearForceJob = new NuclearForceJob
            {
                DeltaTime = deltaTime,
                NucleusDamping = config.NucleusDamping
            };
            
            state.Dependency = nuclearForceJob.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
        }

        [BurstCompile]
        private partial struct NuclearForceJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public float NucleusDamping;

            private void Execute(
                ref PhysicsVelocity velocity,
                ref PID pid,
                in LocalTransform transform,
                in AtomCenter atomCenter,
                in NuclearForce nuclearForce,
                in PhysicsMass mass
            )
            {
                // Calculate distance and direction to atom center
                float3 currentPos = transform.Position;
                float3 targetPos = atomCenter.Position;
                float3 displacement = targetPos - currentPos;
                float distance = math.length(displacement);
                
                if (distance < 0.001f) return; // Avoid division by zero
                
                float3 direction = displacement / distance;
                
                // Calculate desired force based on nuclear force model
                float forceStrength = CalculateNuclearForce(distance, nuclearForce);
                float3 desiredForce = direction * forceStrength;
                
                // PID Controller
                float3 error = displacement;
                pid.Integral += error * DeltaTime;
                pid.Integral = math.clamp(pid.Integral, -pid.IntegralClamp, pid.IntegralClamp);
                
                float3 derivative = (error - pid.PreviousError) / DeltaTime;
                pid.PreviousError = error;
                
                float3 pidForce = error * pid.Kp + pid.Integral * pid.Ki + derivative * pid.Kd;
                pidForce = math.clamp(pidForce, -pid.MaxForce, pid.MaxForce);
                
                // Apply forces
                float3 totalForce = desiredForce + pidForce;
                velocity.Linear += totalForce * mass.InverseMass * DeltaTime;
                
                // Apply damping
                velocity.Linear *= (1.0f - NucleusDamping * DeltaTime);
            }
            
            private float CalculateNuclearForce(float distance, NuclearForce nuclearForce)
            {
                // Gentle attraction that doesn't cause oscillations
                if (distance > nuclearForce.MaxDistance)
                    return 0f;
                    
                // Linear falloff instead of exponential to reduce oscillations
                float normalizedDistance = distance / nuclearForce.OptimalDistance;
                return nuclearForce.StrengthMultiplier * math.max(0f, 1f - normalizedDistance);
            }
        }
    }

    // System for electrons - orbital motion with PID control
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct ElectronOrbitSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var config = SystemAPI.GetSingleton<SimulationConfig>();

            var orbitJob = new ElectronOrbitJob
            {
                DeltaTime = deltaTime,
                ElectronDamping = config.ElectronDamping
            };

            state.Dependency = orbitJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct ElectronOrbitJob : IJobEntity
        {
            public float DeltaTime;
            public float ElectronDamping;

            private void Execute(
                ref PhysicsVelocity velocity,
                ref OrbitData orbitData,
                ref TargetPosition targetPosition,
                ref PID pid,
                in LocalTransform transform,
                in AtomCenter atomCenter,
                in ElectromagneticForce emForce,
                in PhysicsMass mass
            )
            {
                // Update orbital angle
                orbitData.CurrentAngle += orbitData.TargetSpeed * DeltaTime;
                if (orbitData.CurrentAngle > 2 * math.PI)
                    orbitData.CurrentAngle -= 2 * math.PI;

                // Calculate target orbital position
                float3 centerPos = atomCenter.Position;
                float3 orbitDirection = new float3(
                    math.cos(orbitData.CurrentAngle),
                    0f, // Keep electrons in XZ plane, or add Y variation for 3D orbits
                    math.sin(orbitData.CurrentAngle)
                );
                
                targetPosition.Position = centerPos + orbitDirection * orbitData.TargetRadius;
                
                // Current position
                float3 currentPos = transform.Position;
                float3 displacement = targetPosition.Position - currentPos;
                
                // PID Controller for orbital position
                float3 error = displacement;
                pid.Integral += error * DeltaTime;
                pid.Integral = math.clamp(pid.Integral, -pid.IntegralClamp, pid.IntegralClamp);
                
                float3 derivative = (error - pid.PreviousError) / DeltaTime;
                pid.PreviousError = error;
                
                float3 pidForce = error * pid.Kp + pid.Integral * pid.Ki + derivative * pid.Kd;
                pidForce = math.clamp(pidForce, -pid.MaxForce, pid.MaxForce);
                
                // Add tangential velocity for smooth orbital motion
                float3 tangentDirection = new float3(-math.sin(orbitData.CurrentAngle), 0f, math.cos(orbitData.CurrentAngle));
                float3 tangentialForce = tangentDirection * orbitData.OrbitForceMultiplier;
                
                // Electromagnetic attraction to nucleus (simplified)
                float distanceToNucleus = math.length(currentPos - centerPos);
                float3 nucleusDirection = math.normalize(centerPos - currentPos);
                float3 electromagneticAttraction = nucleusDirection * emForce.ChargeStrength / (distanceToNucleus * distanceToNucleus + 0.1f);
                
                // Apply all forces
                float3 totalForce = pidForce + tangentialForce + electromagneticAttraction;
                velocity.Linear += totalForce * mass.InverseMass * DeltaTime;
                
                // Apply damping
                velocity.Linear *= (1.0f - ElectronDamping * DeltaTime);
            }
        }
    }
}

[BurstCompile]
public partial struct SimulationBootstrapSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<SimulationConfig>();
        var electronShellBlob = SystemAPI.GetSingleton<ElectronShellBlob>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        var createAtomJob = new CreateAtomJob
        {
            ECB = ecb.AsParallelWriter(),
            Config = config,
            BlobAssetRef = electronShellBlob.BlobAssetRef
        };
        
        state.Dependency = createAtomJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        state.Enabled = false;
    }
}