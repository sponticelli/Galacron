using System;
using UnityEngine;

namespace Galacron.Actors
{
    public class PixelShieldBrick : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D collider2D;
        [SerializeField] private Health health;
       
        
        [Header("Health Sprites")]
        [SerializeField] HealthSprite[] healthSprites;
        
        [Serializable]
        public class HealthSprite
        {
            public Sprite[] sprites;
            public float healthPercentage;
            
            public Sprite sprite => sprites[UnityEngine.Random.Range(0, sprites.Length)];
        }
        
        public Health Health => health;
            
        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (collider2D == null)
            {
                collider2D = GetComponent<Collider2D>();
            }
            
            if (health == null)
            {
                health = GetComponent<Health>();
            }
            
            // Sort health sprites by health percentage
            Array.Sort(healthSprites, (a, b) => a.healthPercentage.CompareTo(b.healthPercentage));
            
           
        }
        
        private void OnEnable()
        {
            OnDamage(1f);
        }
            
        public void SetColliderEnabled(bool b)
        {
            collider2D.enabled = b;
        }
        
        public void OnDamage(float healthPercentage)
        {
            var sprite = GetSpriteForHealth(healthPercentage);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }
        
        
        private Sprite GetSpriteForHealth(float healthPercentage)
        {
            foreach (var healthSprite in healthSprites)
            {
                if (healthPercentage <= healthSprite.healthPercentage)
                {
                    return healthSprite.sprite;
                }
            }

            return null;
        }
    }
}