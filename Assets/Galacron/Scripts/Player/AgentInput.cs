using Galacron.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Galacron.Player
{
    public class AgentInput : MonoBehaviour, IAgentInputHandler
    {
        [Header("References")]
        [SerializeField] private PlayerInput _input;
        

        [field: SerializeField] public float HorizontalMovement { get; private set; }
        
        public event InputEvent<float> OnHorizontalMovement;
        public event InputEvent OnFirePressed;
        public event InputEvent OnFireReleased;
        
        

        private void OnEnable()
        {
            _input.actions["Player/Horizontal"].performed += HorizontalMoving;
            _input.actions["Player/Horizontal"].canceled += HorizontalMoving;
            
            _input.actions["Player/Shoot"].performed += Shooting;
            _input.actions["Player/Shoot"].canceled += Shooting;
        }
        
        private void OnDisable()
        {
            _input.actions["Player/Horizontal"].performed -= HorizontalMoving;
            _input.actions["Player/Horizontal"].canceled -= HorizontalMoving;
            
            _input.actions["Player/Shoot"].performed -= Shooting;
            _input.actions["Player/Shoot"].canceled -= Shooting;
        }

        private void HorizontalMoving(InputAction.CallbackContext obj)
        {
            HorizontalMovement = obj.ReadValue<float>();
            OnHorizontalMovement?.Invoke(HorizontalMovement);
        }

        private void Shooting(InputAction.CallbackContext obj)
        {
            var isPressed = obj.ReadValue<float>() > 0.5f;
            if (isPressed)
            {
                OnFirePressed?.Invoke();
            }
            else
            {
                OnFireReleased?.Invoke();
            }
        }
    }

}