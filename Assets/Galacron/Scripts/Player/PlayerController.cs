using UnityEngine;

namespace Galacron.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AgentInput _input;
        [SerializeField] private PlayerMovement _movement;
        
        private void OnEnable()
        {
            _input.OnHorizontalMovement += _movement.Move;
        }
        
        private void OnDisable()
        {
            _input.OnHorizontalMovement -= _movement.Move;
        }
    }
}