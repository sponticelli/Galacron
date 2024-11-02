using UnityEngine;

namespace Galacron.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed;
        
        [Header("References")]
        [SerializeField] private Transform _targetTransform;
        
        float _cumulativeMovement;
        
        private float _horizontalMovement;
        
        private void Update()
        {
            _targetTransform.position += new Vector3(_cumulativeMovement, 0, 0);
            _cumulativeMovement = 0;
        }
        
        private void FixedUpdate()
        {
            var movement = _horizontalMovement * _speed * Time.deltaTime;
            _cumulativeMovement += movement;
            
            
        }
        
        public void Move(float horizontalMovement)
        {
            _horizontalMovement = horizontalMovement;
        }
        
    }
}