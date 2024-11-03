using UnityEngine;

namespace Galacron.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float smoothTime = 0.1f;
     
        
        [Header("References")]
        [SerializeField] private Transform _targetTransform;
        
        
        private float _horizontalMovement;
        private Vector3 velocity = Vector3.zero;
        private Vector3 targetPosition;

        private void Start()
        {
            targetPosition = _targetTransform.position;
        }


        private void Update()
        {
            targetPosition = _targetTransform.position;
            targetPosition.x += _horizontalMovement * _speed * Time.deltaTime;
            
            _targetTransform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );
        }
        
        public void Move(float horizontalMovement)
        {
            _horizontalMovement = horizontalMovement;
        }
        
    }
    
}