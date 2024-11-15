using Nexus.Audio;
using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actions
{
    public class PlaySound : MonoBehaviour
    {
       [SerializeField] private Sounds _sound;
        
        private ISoundService _soundService;
        private bool _isInitialized;
        
        private string _soundId;
        
        public void Execute()
        {
            if (!_isInitialized)
            {
                _soundService = ServiceLocator.Instance.GetService<ISoundService>();
                _isInitialized = true;
                _soundId = SoundIdConverter.GetId(_sound);
            }
            
            _soundService.PlayOneShot(_soundId, transform.position);
        }
    }
    
    
}