using System;
using System.Collections;
using System.Collections.Generic;
using Galacron.Paths;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actors
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private float spawnWaveInterval = 1f;
        [SerializeField] private Formation formation;
        [SerializeField] private Wave[] waves;


        private int currentWaveIndex;
        private IPoolingService _poolingService;
        private Dictionary<int, PathBase> _paths = new Dictionary<int, PathBase>();
        private List<GameObject> _enemies = new List<GameObject>();

        [Serializable]
        public class Wave
        {
            public PoolReference<FormationActor>[] _enemies;
            public float spawnInterval = 0.1f;
            public PoolReference<PathBase>[] paths;
            public int amount;
        }

        private void OnEnable()
        {
            formation.gameObject.SetActive(true);
        }


        private async void Start()
        {
            _poolingService = ServiceLocator.Instance.GetService<IPoolingService>();
            await _poolingService.WaitForInitialization();
            PreparePaths();

            // Calculate total amount of enemies
            int totalEnemies = 0;
            foreach (var wave in waves)
            {
                totalEnemies += wave.amount;
            }

            formation.SetTotalMembers(totalEnemies);
        }

        private void PreparePaths()
        {
            _paths.Clear();
            foreach (var wave in waves)
            {
                foreach (var pathRef in wave.paths)
                {
                   var hash = pathRef.GetHashCode();
                    
                    if (_paths.ContainsKey(hash)) continue;
                    Debug.Log($"Spawner: PreparePaths: {hash}");
                    
                    var path = pathRef.Get(transform.position, Quaternion.identity);
                    path.RecalculatePath();
                    _paths.Add(hash, path);
                }
            }
        }

        public void StartSpawning()
        {
            StartCoroutine(SpawnWaves());
        }

        public void OnWaveDestroyed()
        {
            var keys = _paths.Keys;
            foreach (var key in keys)
            {
                var path = _paths[key];
                path.gameObject.ReturnToPool();
            }
            _paths.Clear();
        }

        private IEnumerator SpawnWaves()
        {
            int totalEnemies = 0;
            currentWaveIndex = 0;
            yield return new WaitForSeconds(1f);
            while (currentWaveIndex < waves.Length)
            {
                var wave = waves[currentWaveIndex];
                for (int i = 0; i < wave.amount; i++)
                {
                    var enemy = wave._enemies[i % wave._enemies.Length];
                    var pathId = wave.paths[i % wave.paths.Length].GetHashCode();
                    var path = _paths[pathId];
                    var enemyBehavior = enemy.Get(transform.position, Quaternion.identity);
                    enemyBehavior.SpawnSetup(path, totalEnemies, formation);
                    formation.RegisterMember(enemyBehavior, enemyBehavior.EnemyID);
                    _enemies.Add(enemyBehavior.gameObject);
                    totalEnemies++;

                    yield return new WaitForSeconds(wave.spawnInterval);
                }

                yield return new WaitForSeconds(spawnWaveInterval);
                currentWaveIndex++;
            }

            formation.OnSpawnComplete();
        }
    }
}