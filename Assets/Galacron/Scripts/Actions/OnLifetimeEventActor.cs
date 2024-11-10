using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Actions
{
    public class OnLifetimeEventAction : MonoBehaviour
    {
        public enum LifetimeEvent
        {
            OnAwake,
            OnEnable,
            OnStart,
        }
        
        [SerializeField] private LifetimeEvent lifetimeEvent;
        [SerializeField] private UnityEvent onEvent;
        
        private void Awake()
        {
            if (lifetimeEvent == LifetimeEvent.OnAwake)
            {
                onEvent.Invoke();
            }
        }
        
        private void OnEnable()
        {
            if (lifetimeEvent == LifetimeEvent.OnEnable)
            {
                onEvent.Invoke();
            }
        }
        
        private void Start()
        {
            if (lifetimeEvent == LifetimeEvent.OnStart)
            {
                onEvent.Invoke();
            }
        }
    }
}