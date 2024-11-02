using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actors
{
    /// <summary>
    /// Destry the object when it goes out of bounds of the screen (any direction)
    /// </summary>
    public class DestroyOutOfBounds : MonoBehaviour
    {
        [SerializeField] private float _offsetBounds = 1f;
        [SerializeField] private Transform _targetTransform;

        private Camera _camera;
        private PooledObject _pooledObject;

        private void Start()
        {
            _camera = Camera.main;
            _pooledObject = GetComponent<PooledObject>();
        }

        private void Update()
        {
            var min = _camera.ViewportToWorldPoint(new Vector3(0, 0,
                _targetTransform.position.z - _camera.transform.position.z));
            var max = _camera.ViewportToWorldPoint(new Vector3(1, 1,
                _targetTransform.position.z - _camera.transform.position.z));

            if (_targetTransform.position.x < min.x - _offsetBounds ||
                _targetTransform.position.x > max.x + _offsetBounds ||
                _targetTransform.position.y < min.y - _offsetBounds ||
                _targetTransform.position.y > max.y + _offsetBounds)
            {
                if (_pooledObject != null)
                {
                    _pooledObject.ReturnToPool();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}