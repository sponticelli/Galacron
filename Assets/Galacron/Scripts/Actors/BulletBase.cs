using UnityEngine;

namespace Galacron.Actors
{
    public class BulletBase : MonoBehaviour
    {
        [SerializeField]
        private Vector3 velocity;

        public Vector3 Velocity
        {
            get => velocity;
            set => velocity = value;
        }
        
        void Update()
        {
            transform.position += velocity * Time.deltaTime;
        }
    }
}