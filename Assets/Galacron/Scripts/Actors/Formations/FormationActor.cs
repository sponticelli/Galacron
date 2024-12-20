using System.Collections.Generic;
using Galacron.Actors.States;
using Galacron.Paths;
using Galacron.Player;
using Galacron.Score;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using Nexus.Registries;
using UnityEngine;


namespace Galacron.Actors
{
    public class FormationActor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PathFollower pathFollower;
        [SerializeField] private MoveToTarget moveToTarget;
        [SerializeField] private Weapon weapon;
        [SerializeField] private ActorVisual visual;

        [Header("Shooting Settings")] 
        [SerializeField] private FireSettings onPathFireSettings;
        [SerializeField] private FireSettings onFlyInFireSettings;
        [SerializeField] private FireSettings onIdleFireSettings;
        [SerializeField] private FireSettings onDiveFireSettings;
        
        [Header("Score Settings")]
        [SerializeField] private int pointValue = 50;
        [SerializeField] private int divePointValue = 100;
        
        private PathBase pathToFollow;
        private Formation formation;
        private IActorState currentState;
        private readonly Dictionary<StateType, IActorState> states;

        public enum StateType
        {
            OnPath,
            FlyIn,
            Idle,
            Dive
        }

        public int EnemyID { get; private set; } = -1;


        // Properties for state access
        public MoveToTarget MoveToTarget => moveToTarget;
        public Weapon Weapon => weapon;
        public Formation Formation => formation;
        public Transform Target => target;
        public FireSettings OnPathFireSettings => onPathFireSettings;
        public FireSettings OnFlyInFireSettings => onFlyInFireSettings;
        public FireSettings OnIdleFireSettings => onIdleFireSettings;
        public FireSettings OnDiveFireSettings => onDiveFireSettings;

        private IPoolingService poolingService;
        private IComponentRegistry componentRegistry;
        private Transform target;

        public FormationActor()
        {
            states = new Dictionary<StateType, IActorState>
            {
                { StateType.OnPath, new OnPathState(this) },
                { StateType.FlyIn, new FlyInState(this) },
                { StateType.Idle, new IdleState(this) },
                { StateType.Dive, new DiveState(this) }
            };
        }

        private void Start()
        {
            pathFollower.OnPathCompleted += OnPathEnd;
            poolingService = ServiceLocator.Instance.GetService<IPoolingService>();
        }

        private void OnEnable()
        {
            if (!ServiceLocator.Instance.CanResolve(typeof(IComponentRegistry)))
                return;

            componentRegistry = ServiceLocator.Instance.GetService<IComponentRegistry>();
            if (componentRegistry == null) return;

            target = componentRegistry.Get<PlayerController>().transform;
            componentRegistry.SubscribeToRegister<PlayerController>(OnPlayerRegistered);
            componentRegistry.SubscribeToDeRegister<PlayerController>(OnPlayerUnregistered);
            
            moveToTarget.Stop();
            
        }

        private void OnDisable()
        {
            if (componentRegistry == null) return;
            componentRegistry.UnsubscribeFromRegister<PlayerController>(OnPlayerRegistered);
            componentRegistry.UnsubscribeFromDeRegister<PlayerController>(OnPlayerUnregistered);
        }

        private void Update()
        {
            currentState?.Update();
        }

        public void ChangeState(StateType newState)
        {
            currentState?.Exit();
            currentState = states[newState];
            currentState.Enter();
        }

        public void SpawnSetup(PathBase path, int id, Formation formation)
        {
            pathToFollow = path;
            EnemyID = id;
            this.formation = formation;
            pathFollower.SetPath(pathToFollow);
            ChangeState(StateType.OnPath);
        }

        public void DiveSetup(PathBase path)
        {
            pathToFollow = path;
            transform.SetParent(null);
            pathFollower.SetPath(pathToFollow);
            ChangeState(StateType.Dive);
        }

        public void OnDeath()
        {
            if (currentState is DiveState)
            {
                pathToFollow = null;
            }
            
            var points = currentState is DiveState ? divePointValue : this.pointValue;
            
            GameEvents.OnEnemyKilled.OnNext(
                new EnemyKilledEvent(points, transform.position)
            );

            formation.OnMemberDestroyed(this);
        }

        public void OnPathEnd()
        {
            pathToFollow = null;
            pathFollower.SetPath(null);
            
            if (currentState is DiveState)
            {
                formation.OnDiveComplete(EnemyID);
            }

            ChangeState(StateType.FlyIn);
        }

        public void OnFormationPositionReached()
        {
            transform.SetParent(formation.gameObject.transform);
            transform.eulerAngles = Vector3.zero;
            formation.RegisterMember(this, EnemyID);
            ChangeState(StateType.Idle);
            moveToTarget.Stop();
        }

        public void ActivateTrails(bool on)
        {
            visual.ActivateTrails(on);
        }

        private void OnPlayerUnregistered(PlayerController obj)
        {
            target = null;
        }

        private void OnPlayerRegistered(PlayerController obj)
        {
            target = obj.transform;
        }
    }
}