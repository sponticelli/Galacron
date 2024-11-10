using System;
using System.Collections;
using System.Collections.Generic;
using Galacron.Paths;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Galacron.Actors
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private float spawnWaveInterval = 1f;
        [SerializeField] private Formation formation;
        [SerializeField] private Wave[] waves;


        private int currentWaveIndex;
        private IPoolingService _poolingService;
        private Dictionary<Pools, Path> _paths = new Dictionary<Pools, Path>();
        private List<GameObject> _enemies = new List<GameObject>();

        [Serializable]
        public class Wave
        {
            public Pools[] _enemies;
            public float spawnInterval = 0.1f;
            public Pools[] paths;
            public int amount;
        }


        private async void Start()
        {
            _poolingService = ServiceLocator.Instance.GetService<IPoolingService>();
            await _poolingService.WaitForInitialization();
            PreparePaths();

            // CAlculate total amount of enemies
            int totalEnemies = 0;
            foreach (var wave in waves)
            {
                totalEnemies += wave.amount;
            }

            formation.SetTotalEnemies(totalEnemies);
        }

        private void PreparePaths()
        {
            _paths.Clear();
            foreach (var wave in waves)
            {
                foreach (var prefab in wave.paths)
                {
                    if (_paths.ContainsKey(prefab)) continue;
                    var pathGO = _poolingService.GetFromPool(PoolIdConverter.GetId(prefab), transform.position,
                        Quaternion.identity);
                    var path = pathGO.GetComponent<Path>();
                    _paths.Add(prefab, path);
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
            while (currentWaveIndex < waves.Length)
            {
                var wave = waves[currentWaveIndex];
                for (int i = 0; i < wave.amount; i++)
                {
                    var enemy = wave._enemies[i % wave._enemies.Length];
                    var pathId = wave.paths[i % wave.paths.Length];
                    var path = _paths[pathId];
                    var enemyGO = _poolingService.GetFromPool(PoolIdConverter.GetId(enemy), transform.position,
                        Quaternion.identity);
                    var enemyBehavior = enemyGO.GetComponent<FormationActor>();
                    enemyBehavior.SpawnSetup(path, formation.enemyList.Count, formation);
                    formation.RegisterEnemy(enemyBehavior);
                    _enemies.Add(enemyGO);

                    yield return new WaitForSeconds(wave.spawnInterval);
                }

                yield return new WaitForSeconds(spawnWaveInterval);
                currentWaveIndex++;
            }

            formation.OnSpawnComplete();
        }
    }
}