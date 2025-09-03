using System.Runtime.CompilerServices;
using AtomicSimulation.Authoring;
using AtomicSimulation.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AtomicSimulation.Core
{
    public struct AtomicNumber : IComponentData
    {
        public byte Value;
    }

    public struct AtomReady : IComponentData, IEnableableComponent
    {
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

    public struct ElectronParticle : IComponentData, IEnableableComponent
    {
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
        public float M;
        public float C;
        public Entity ProtonPrefab;
        public Entity NeutronPrefab;
        public Entity ElectronPrefab;
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

            ECB.AddComponent(chunkIndex, protonEntity,
                LocalTransform.FromPositionRotationScale(
                    centerPos + nucleusOffset,
                    quaternion.identity,
                    Config.NucleusScale
                )
            );
            ECB.AddComponent(chunkIndex, protonEntity, new NucleusParticle { LocalOffset = nucleusOffset });
            ECB.AddComponent(chunkIndex, protonEntity, new AtomCenter { Position = centerPos });
        }

        // Create neutrons  
        for (int i = 0; i < neutrons; i++)
        {
            var neutronEntity = ECB.Instantiate(chunkIndex, Config.NeutronPrefab);
            AtomicData.GetNucleusParticleOffset(atomicNumber + i, nucleus, m, c, out var nucleusOffset);

            ECB.AddComponent(chunkIndex, neutronEntity,
                LocalTransform.FromPositionRotationScale(
                    centerPos + nucleusOffset,
                    quaternion.identity,
                    Config.NucleusScale
                )
            );
            ECB.AddComponent(chunkIndex, neutronEntity, new NucleusParticle { LocalOffset = nucleusOffset });
            ECB.AddComponent(chunkIndex, neutronEntity, new AtomCenter { Position = centerPos });
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
                ECB.AddComponent(chunkIndex, electronEntity, LocalTransform.FromPositionRotationScale(
                    orbitPos, quaternion.identity, Config.ElectronScale));
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
}


namespace AtomicSimulation.Core
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WithPresent(typeof(ElectronParticle))]
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

            private void Execute(
                ref LocalTransform transform, ref OrbitData orbitData,
                in AtomCenter atomCenter
            )
            {
                AtomicData.ElectronOrbitOffset(ref orbitData, DeltaTime, out var orbitOffset);
                transform.Position = atomCenter.Position + orbitOffset;
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