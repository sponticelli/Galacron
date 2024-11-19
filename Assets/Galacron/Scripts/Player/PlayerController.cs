using Galacron.Actors;
using UnityEngine;

namespace Galacron.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AgentInput _input;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private Weapon _weapon;
        
        
        private void OnEnable()
        {
            _input.OnHorizontalMovement += _movement.Move;
            _input.OnFirePressed += _weapon.Shoot;
            _input.OnFireReleased += _weapon.StopShooting;
        }
        
        private void OnDisable()
        {
            _input.OnHorizontalMovement -= _movement.Move;
            _input.OnFirePressed -= _weapon.Shoot;
            _input.OnFireReleased -= _weapon.StopShooting;
        }
    }
}