using Galacron.Paths;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;


namespace Galacron.Actors
{
    public class FormationActor : MonoBehaviour
    {
        [SerializeField] private PathFollower pathFollower;
        [SerializeField] private MoveToTarget moveToTarget;
        
        public float speed = 2;
        private Path _pathToFollow;
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

        // Use this for initialization
        private void Start()
        {
            poolingService = ServiceLocator.Instance.GetService<IPoolingService>();
        }

        // Update is called once per frame
        private void Update()
        {
            switch (CurrentState)
            {
                case State.OnPath:
                {
                    TrailActivate(true);
                }
                    break;
                case State.FlyIn:
                {
                    //TrailActivate(true);
                    moveToTarget.SetTarget(_formation.GetVector(EnemyID));
                }
                    break;
                case State.Idle:
                    TrailActivate(false);
                    break;
                case State.Dive:
                    TrailActivate(true);
                    //SHOOTING
                    Shoot();    
                    break;
            }
        }

        private void Shoot()
        {
            // TODO Implement shooting
        }
        

        public void SpawnSetup(Path path, int ID, Formation _formation)
        {
            _pathToFollow = path;
            EnemyID = ID;
            this._formation = _formation;
            pathFollower.SetPath(_pathToFollow);
            CurrentState = State.OnPath;
        }

        public void DiveSetup(Path path)
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