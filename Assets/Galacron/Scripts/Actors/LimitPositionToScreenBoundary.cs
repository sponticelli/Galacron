using UnityEngine;

namespace Galacron.Actors
{
    public class LimitPositionToScreenBoundary : MonoBehaviour
    {
        [SerializeField] private float _boundOffset;
        [SerializeField] private Transform _targetTransform;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }
        
        private void LateUpdate()
        {
            BoundPosition();
        }
        
        private void BoundPosition()
        {
            var min = _camera.ViewportToWorldPoint(new Vector3(0, 1, _targetTransform.position.z - _camera.transform.position.z));
            var max = _camera.ViewportToWorldPoint(new Vector3(1, 0, _targetTransform.position.z - _camera.transform.position.z));
            
            var pos = _targetTransform.position;
            pos.x = Mathf.Clamp(pos.x, min.x + _boundOffset, max.x - _boundOffset);
            _targetTransform.position = pos;
        }
        
        private void OnDrawGizmos()
        {
            if (_targetTransform == null)
            {
                return;
            }
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            var min = _camera.ViewportToWorldPoint(new Vector3(0, 1, _targetTransform.position.z - _camera.transform.position.z));
            var max = _camera.ViewportToWorldPoint(new Vector3(1, 0, _targetTransform.position.z - _camera.transform.position.z));
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(min.x + _boundOffset, min.y), new Vector3(max.x - _boundOffset, min.y));
            Gizmos.DrawLine(new Vector3(min.x + _boundOffset, max.y), new Vector3(max.x - _boundOffset, max.y));
        }
    }
}