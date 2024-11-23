using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actors
{
    public class PixelBlock
    {
        private PixelShieldBrick _brick;
        private Health _health;
        private int x, y;
        private PixelShield _shield;
        private Vector2 _worldPosition;
        private bool _isDestroyed = false;

        public int X => x;
        public int Y => y;
        public Vector2 WorldPosition => _worldPosition;

        public PixelShieldBrick Brick
        {
            get => _brick;
            set => _brick = value;
        }

        public PixelBlock(PixelShield shield, PixelShieldBrick brick, int xPos, int yPos)
        {
            _brick = brick;
            _health = _brick.Health;
            _shield = shield;
            x = xPos;
            y = yPos;
            _worldPosition = _brick.transform.position;

            _health.onDeath.AddListener(OnPixelDestroyed);
            _isDestroyed = false;
        }

        public bool IsValid() => _brick != null && _brick.gameObject != null;
        public bool IsAlive() => !_isDestroyed && IsValid() && _health != null && _health.CurrentHealth > 0;
        public Vector2 Position => new Vector2(x, y);

        public void SetWorldPosition(Vector2 newPosition)
        {
            _worldPosition = newPosition;
            if (_brick != null)
            {
                _brick.transform.position = _worldPosition;
            }
        }

        public void UpdateGridPosition(int newX, int newY)
        {
            x = newX;
            y = newY;
            if (_brick != null)
            {
                _brick.name = $"Pixel_{x}_{y}";
            }
        }
        
        private void OnPixelDestroyed()
        {
            if (_isDestroyed) return; // Prevent multiple destruction calls
            
            _isDestroyed = true;
            Debug.Log($"Pixel destroyed at {x}, {y}");
            
            if (_brick != null && _brick.gameObject != null)
            {
                _brick.gameObject.ReturnToPool();
            }
            
            if (_shield != null)
            {
                _shield.OnPixelDestroyed(this);
            }
            
            Cleanup();
        }

        public void Cleanup()
        {
            if (_health != null)
            {
                _health.onDeath.RemoveListener(OnPixelDestroyed);
            }
            
            if (_brick != null && _brick.gameObject != null)
            {
                _brick.gameObject.ReturnToPool();
            }
            
            _brick = null;
            _health = null;
            _shield = null;
        }

        public void SetColliderEnabled(bool b)
        {
            if (_brick != null)
            {
                _brick.SetColliderEnabled(b);
            }
        }

        public void Revive()
        {
            if (_health != null)
            {
                _isDestroyed = false;
                _health.Revive();
            }
        }
    }
}