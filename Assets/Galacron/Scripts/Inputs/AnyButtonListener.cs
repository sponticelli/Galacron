using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;

namespace Galacron.Inputs
{
    public class AnyButtonListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent onAnyButtonPressed;
        
        private IDisposable _eventListener;
        
        void OnEnable()
        {
            _eventListener =
                InputSystem.onAnyButtonPress
                    .Call(OnAnyButtonPress);
        }

        void OnDisable()
        {
            _eventListener.Dispose();
        }

        private void OnAnyButtonPress(InputControl control)
        {
            onAnyButtonPressed?.Invoke();
        }
    }
}