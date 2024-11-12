using System;
using Galacron.Paths;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;


namespace Galacron.Actors
{
    [Serializable]
    public class FireSettings
    {
        public bool canFire = true;
        public float minFireRate = 0.5f;
        public float maxFireRate = 1f;
        public float minPrecision = 0.1f;
        public float maxPrecision = 0.5f;
        
        public float GetFireRate()
        {
            return UnityEngine.Random.Range(minFireRate, maxFireRate);
        }
        
        public float GetPrecision()
        {
            return UnityEngine.Random.Range(minPrecision, maxPrecision);
        }
    }
    
    
    public class FormationActor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PathFollower pathFollower;
        [SerializeField] private MoveToTarget moveToTarget;
        [SerializeField] private Weapon weapon;
        
        [Header("Shooting Settings")]
        [SerializeField] private FireSettings OnPathFireSettings;
        [SerializeField] private FireSettings OnFlyInFireSettings;
        [SerializeField] private FireSettings OnIdleFireSettings;
        [SerializeField] private FireSettings OnDiveFireSettings;
        
        public float speed = 2;
        private PathBase _pathToFollow;
        private float _distance; //current distance to next waypoint
        private Formation _formation;
        
        public enum State
        {
            OnPath, //is on a path
            FlyIn, //fly into formation
            Idle,
            Dive
        }

        public State CurrentState { get; private set; }
        public int EnemyID { get; private set; } = -1;
        

        //SCORE
        public int inFormationScore;
        public int notInFormationScore;

        //TRAILS
        public TrailRenderer[] trails;

        //EFFECTS
        public GameObject fx_Explosion;


        private IPoolingService poolingService;
        
        private float _nextFire;

        // Use this for initialization
        private void Start()
        {
            pathFollower.OnPathCompleted += OnPathEnd;
            poolingService = ServiceLocator.Instance.GetService<IPoolingService>();
        }

        // Update is called once per frame
        private void Update()
        {
            _nextFire -= Time.deltaTime;
            switch (CurrentState)
            {
                case State.OnPath:
                    if (OnPathFireSettings.canFire && _nextFire <= 0)
                    {
                        Shoot();
                        _nextFire = OnPathFireSettings.GetFireRate();
                    }
                    TrailActivate(true);
                    break;
                case State.FlyIn:
                    if (OnFlyInFireSettings.canFire && _nextFire <= 0)
                    {
                        Shoot();
                        _nextFire = OnFlyInFireSettings.GetFireRate();
                    }
                    moveToTarget.SetTarget(_formation.GetVector(EnemyID));
                
                    break;
                case State.Idle:
                    if (OnIdleFireSettings.canFire && _nextFire <= 0)
                    {
                        Shoot();
                        _nextFire = OnIdleFireSettings.GetFireRate();
                    }
                    TrailActivate(false);
                    break;
                case State.Dive:
                    TrailActivate(true);
                    if (OnDiveFireSettings.canFire && _nextFire <= 0)
                    {
                        Shoot();
                        _nextFire = OnDiveFireSettings.GetFireRate();
                    }
                    break;
            }
        }

        private void Shoot()
        {
            // ROTATE WEAPON  z axis is 180 degrees off
            if (weapon == null) return;
            
            weapon.transform.rotation = Quaternion.Euler(0, 0, 180);
            if (weapon.IsShooting)
            {
                weapon.StopShooting();
                return;
            }
            weapon.Shoot();
            
        }
        

        public void SpawnSetup(PathBase path, int ID, Formation _formation)
        {
            _pathToFollow = path;
            EnemyID = ID;
            this._formation = _formation;
            pathFollower.SetPath(_pathToFollow);
            CurrentState = State.OnPath;
        }

        public void DiveSetup(PathBase path)
        {
            _pathToFollow = path;
            transform.SetParent(null);
            pathFollower.SetPath(_pathToFollow);
            CurrentState = State.Dive;
        }


        public void OnDeath()
        {

                //PLAY SOUND

                //INSTATIATE PARTICLE
                if (fx_Explosion != null)
                {
                    Instantiate(fx_Explosion, transform.position, Quaternion.identity);
                }

                if (CurrentState == State.Dive)
                {
                    _pathToFollow.gameObject.ReturnToPool();
                    _pathToFollow = null;
                }


                //REPORT TO FORMATION
                _formation.ReportDeath(EnemyID);
        }
        
        public void OnPathEnd()
        {
            _pathToFollow = null;
            pathFollower.SetPath(null);
            if (CurrentState == State.Dive)
            {
                _formation.OnDiveEnd(EnemyID);
            }
            CurrentState = State.FlyIn;
            
        }
        
        public void OnFormationPositionReached()
        {
            transform.SetParent(_formation.gameObject.transform);
            transform.eulerAngles = Vector3.zero;
            _formation.RegisterEnemy(this);
            CurrentState = State.Idle;
            moveToTarget.Stop();
        }

        private void TrailActivate(bool on)
        {
            foreach (TrailRenderer trail in trails)
            {
                trail.enabled = on;
            }
        }
    }
}