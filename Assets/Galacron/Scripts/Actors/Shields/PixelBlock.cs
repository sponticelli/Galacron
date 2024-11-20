using UnityEngine;

namespace Galacron.Actors
{
    public class PixelBlock
    {
        private PixelShieldBrick _brick;
        private Health _health;
        private int x, y;
        private PixelShield _shield;

        public int X => x;
        public int Y => y;
        
        public PixelBlock(PixelShield shield, PixelShieldBrick brick, int xPos, int yPos)
        {
            _brick = brick;
            _health = _brick.Health;
            _shield = shield;
            x = xPos;
            y = yPos;

            _health.onDeath.AddListener(OnPixelDestroyed);
        }

        public bool IsValid() =>  _brick.gameObject != null;
        public bool IsAlive() => IsValid() && _health != null && _health.CurrentHealth > 0;
        public Vector2 Position => new Vector2(x, y);
        
        private void OnPixelDestroyed()
        {
            Debug.Log($"Pixel destroyed at {x}, {y}");
            if (_shield != null)
            {
                _shield.OnPixelDestroyed(this);
            }
            _health.onDeath.RemoveListener(OnPixelDestroyed);
        }

        public void SetColliderEnabled(bool b)
        {
            _brick.SetColliderEnabled(b );
        }
    }
}