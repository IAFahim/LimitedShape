using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace AtomicSimulation.Core
{
    // Enemy Components
    public struct Enemy : IComponentData
    {
        public float Speed;
        public bool HasSetVelocity; // Flag to ensure we only set velocity once
    }
    // Enemy spawning data
    public struct EnemySpawner : IComponentData
    {
        public Entity EnemyPrefab;
        public float SpawnRadius;
        public float SpawnInterval;
        public float NextSpawnTime;
        public int MaxEnemies;
        public int CurrentEnemyCount;
    }

    // System to initialize enemy movement (sets velocity once)
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct EnemyMovementInitSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var initJob = new EnemyInitJob();
            state.Dependency = initJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct EnemyInitJob : IJobEntity
        {
            private void Execute(
                ref PhysicsVelocity velocity,
                ref Enemy enemy,
                in LocalTransform transform,
                in TargetPosition targetPosition
            )
            {
                // Only set velocity once when enemy is first created
                if (!enemy.HasSetVelocity)
                {
                    float3 direction = math.normalize(targetPosition.Value - transform.Position);
                    velocity.Linear = direction * enemy.Speed;
                    enemy.HasSetVelocity = true;
                }
            }
        }
    }

    // System to spawn enemies
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float currentTime = (float)SystemAPI.Time.ElapsedTime;
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (spawner, entity) in SystemAPI.Query<RefRW<EnemySpawner>>().WithEntityAccess())
            {
                ref var spawnerData = ref spawner.ValueRW;
                
                // Check if it's time to spawn and we haven't reached max enemies
                if (currentTime >= spawnerData.NextSpawnTime && 
                    spawnerData.CurrentEnemyCount < spawnerData.MaxEnemies)
                {
                    SpawnEnemy(ref ecb, ref spawnerData, currentTime);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private void SpawnEnemy(ref EntityCommandBuffer ecb, ref EnemySpawner spawner, float currentTime)
        {
            // Generate random spawn position around the perimeter
            var random = Unity.Mathematics.Random.CreateFromIndex((uint)(currentTime * 1000));
            float angle = random.NextFloat(0f, 2f * math.PI);
            
            float3 spawnPos = new float3(
                math.cos(angle) * spawner.SpawnRadius,
                0f, // Keep at same Y level, adjust as needed
                math.sin(angle) * spawner.SpawnRadius
            );

            // Create enemy entity
            var enemyEntity = ecb.Instantiate(spawner.EnemyPrefab);
            
            // Set position
            ecb.SetComponent(enemyEntity, LocalTransform.FromPosition(spawnPos));
            
            // Update spawner data
            spawner.NextSpawnTime = currentTime + spawner.SpawnInterval;
            spawner.CurrentEnemyCount++;
        }
    }

    // Helper system to clean up enemies that are too far from center (failsafe)
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemyCleanupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            float maxDistance = 100f; // Adjust based on your game scale
            float maxDistanceSq = maxDistance * maxDistance;

            foreach (var (transform, entity) in 
                SystemAPI.Query<RefRO<LocalTransform>>()
                .WithAll<Enemy>()
                .WithEntityAccess())
            {
                float distanceSq = math.lengthsq(transform.ValueRO.Position);
                if (distanceSq > maxDistanceSq)
                {
                    ecb.DestroyEntity(entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}

