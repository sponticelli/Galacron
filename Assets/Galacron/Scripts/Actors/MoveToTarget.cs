using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Actors
{
    public class MoveToTarget : MonoBehaviour
    {
        [SerializeField] private Transform objectToMove;
        [SerializeField] private float speed = 2;
        [SerializeField] private Vector3 target;
        [SerializeField] private UnityEvent OnTargetReached;


        private bool _move;
        
        public void SetTarget(Vector3 newTarget)
        {
            target = newTarget;
            _move = true;
        }
        
        public void Stop()
        {
            _move = false;
        }
        
        
        private void Start()
        {
            if (objectToMove == null)
            {
                objectToMove = transform;
            }
        }
        
        private void Update()
        {
            if (!_move) return;
            
            objectToMove.position =
                Vector3.MoveTowards(objectToMove.position, target, speed * Time.deltaTime);
            //ROTATION OF ENEMY
            var direction = target - objectToMove.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            objectToMove.rotation = Quaternion.Euler(0, 0, angle);

            if (Vector3.Distance(objectToMove.position, target) <= 0.001f)
            {
                OnTargetReached?.Invoke();
            }
        }
    }
}