using UnityEngine;

namespace Galacron.Actors
{
    public class ActorVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Header("Trails")]
        [SerializeField] private TrailRenderer[] trails;
        [SerializeField] private ParticleSystem[]  trailsParticles;
        
        public void ActivateTrails(bool on)
        {
            foreach (TrailRenderer trail in trails)
            {
                trail.enabled = on;
            }

            foreach (var ps in trailsParticles)
            {
                ps.gameObject.SetActive(on);
            }
        }
    }
}