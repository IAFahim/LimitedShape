using AtomicSimulation.Core;
using Unity.Entities;
using UnityEngine;

namespace Atoms.Atoms.Authoring
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        [Header("Spawner Settings")]
        public GameObject enemyPrefab;
        public float spawnRadius = 20f;
        public float spawnInterval = 2f;
        public int maxEnemies = 10;

        class Baker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new EnemySpawner
                {
                    EnemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                    SpawnRadius = authoring.spawnRadius,
                    SpawnInterval = authoring.spawnInterval,
                    NextSpawnTime = authoring.spawnInterval,
                    MaxEnemies = authoring.maxEnemies,
                    CurrentEnemyCount = 0
                });
            }
        }
    }
}