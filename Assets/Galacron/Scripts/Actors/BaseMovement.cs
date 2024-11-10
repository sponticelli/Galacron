using UnityEngine;

namespace Galacron.Actors
{
    public class BaseMovement : MonoBehaviour
    {
        [SerializeField] private Transform _objectToMove;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _maxMoveOffsetX = 5f;
        
        
        private float _curPosX; //moving Position
        private Vector3 _startPosition;
        
        private int _direction = -1;
        
        private void Start()
        {
            if (_objectToMove == null)
            {
                _objectToMove = transform;
            }
            _startPosition = _objectToMove.position;
            _curPosX = _objectToMove.position.x;
        }
        
        private void Update()
        {
            _curPosX += Time.deltaTime * _speed * _direction;
            if (_curPosX >= _maxMoveOffsetX)
            {
                _direction *= -1;
                _curPosX = _maxMoveOffsetX;
            }
            else if (_curPosX <= -_maxMoveOffsetX)
            {
                _direction *= -1;
                _curPosX = -_maxMoveOffsetX;
            }

            _objectToMove.position = new Vector3(_curPosX, _startPosition.y, _startPosition.z);
        }
        
        
    }
}