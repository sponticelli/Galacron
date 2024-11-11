using System.Collections;
using System.Collections.Generic;
using Galacron.Paths;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Galacron.Actors
{
    public class Formation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BaseMovement baseMovement;
        
        [Header("Grid Settings")]
        [SerializeField] private int gridSizeX = 10;
        [SerializeField] private int gridSizeY = 2;
        [SerializeField] private float gridOffsetX = 1f;
        [SerializeField] private float gridOffsetY = 1f;

        
        [Header("Diving Settings")]
        [SerializeField] private float minTimeAfterSpawn = 6f;
        [SerializeField] private float minTimeBetweenDives = 3f;
        [SerializeField] private float maxTimeBetweenDives = 10f;
        
        
        [Header("Events")]
        [SerializeField] private UnityEvent onFormationDead;
        

        private List<Vector3> gridList = new List<Vector3>();
        private float _timeToDive;
        private bool _canDive;
        

        //DIVING
        private bool canDive;
        public List<Pools> divePathList = new List<Pools>();
        
        
        

        public List<EnemyFormation> enemyList = new List<EnemyFormation>();
        private int _totalEnemies;
        
        private Dictionary<int, GameObject> _divePaths = new Dictionary<int, GameObject>();
        
        private IPoolingService _poolingService;

        [System.Serializable]
        public class EnemyFormation
        {
            public int index;
            public FormationActor enemy;
            
            public EnemyFormation(int _index, FormationActor _enemy)
            {
                index = _index;
                enemy = _enemy;
            }
        }


        private async void Start()
        {
            _poolingService = ServiceLocator.Instance.GetService<IPoolingService>();
            await _poolingService.WaitForInitialization();
            CreateGrid();
        }

        private void Update()
        {
            if (_canDive)
            {
                _timeToDive -= Time.deltaTime;
                if (_timeToDive <= 0)
                {
                    SetDiving();
                    _timeToDive = Random.Range(minTimeBetweenDives, maxTimeBetweenDives);
                }
            }
        }

        private void OnDrawGizmos()
        {
            int num = 0;

            CreateGrid();
            foreach (Vector3 pos in gridList)
            {
                Gizmos.DrawWireSphere(GetVector(num), 0.1f);
                num++;
            }
        }

        private void CreateGrid()
        {
            gridList.Clear();

            int num = 0;

            var width = (gridSizeX - 1) * gridOffsetX;
            var height = (gridSizeY - 1) * gridOffsetY;
            var startX = transform.position.x  - width/2 ;
            var startY = transform.position.y -  height ;
            
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    float x = startX + i * gridOffsetX;
                    float y = startY +  j * gridOffsetY;

                    Vector3 vec = new Vector3(x, y, 0);

                    num++;

                    gridList.Add(vec);
                }
            }
        }

        public Vector3 GetVector(int ID)
        {
            return transform.position + gridList[ID];
        }

        private void SetDiving()
        {
            if (enemyList.Count > 0)
            {
                var choosenPath = divePathList[Random.Range(0, divePathList.Count)];
                int choosenEnemy = Random.Range(0, enemyList.Count);
                
                var enemy = enemyList[choosenEnemy].enemy;
                if (enemy == null)
                {
                    enemyList.RemoveAt(choosenEnemy);
                    return;
                }

                GameObject pathGO = _poolingService.GetFromPool(PoolIdConverter.GetId(choosenPath), 
                    enemy.transform.position, Quaternion.identity);
                

                var path = pathGO.GetComponent<PathBase>();
                _divePaths.Add(enemy.EnemyID, pathGO);
                path.RecalculatePath(); 
                enemy.DiveSetup(path);
                enemyList.RemoveAt(choosenEnemy);
            }
        }

        public void OnSpawnComplete()
        {
           _canDive = true;
           _timeToDive = minTimeAfterSpawn;    
        }

        public void ReportDeath(int enemyID)
        {
            RemoveDivePath(enemyID);
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].index == enemyID)
                {
                    enemyList.RemoveAt(i);
                    break;
                }
            }
            
            _totalEnemies--;
            if (_totalEnemies <= 0)
            {
                onFormationDead.Invoke();
            }
        }

        public void RegisterEnemy(FormationActor enemyBehavior)
        {
            // If it's not already in the list, add it
            if (enemyList.Find(x => x.enemy == enemyBehavior) == null)
            {
                var id = enemyBehavior.EnemyID == -1 ?  enemyList.Count : enemyBehavior.EnemyID;
                enemyList.Add(new EnemyFormation(id, enemyBehavior));
            }
        }

        public void SetTotalEnemies(int totalEnemies)
        {
            _totalEnemies = totalEnemies;
        }

        public void OnDiveEnd(int enemyID)
        {
            RemoveDivePath(enemyID);
        }

        private void RemoveDivePath(int enemyID)
        {
            if (_divePaths.ContainsKey(enemyID))
            {
                var path = _divePaths[enemyID];
                _divePaths.Remove(enemyID);
                path.gameObject.ReturnToPool();
            }
        }
    }
}