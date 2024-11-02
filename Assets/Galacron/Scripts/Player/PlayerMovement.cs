using UnityEngine;

namespace Galacron.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _speed;
     
        
        [Header("References")]
        [SerializeField] private Transform _targetTransform;
        
        
        private float _horizontalMovement;
        

        private void Update()
        {
            _targetTransform.position += Vector3.right * (_horizontalMovement * _speed * Time.deltaTime);
        }

        

        public void Move(float horizontalMovement)
        {
            _horizontalMovement = horizontalMovement;
        }
        
        
        
    }
    
    
    
}