using Nexus.Audio;
using Nexus.Core.ServiceLocation;
using UnityEngine;

namespace Galacron.Actions
{
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] private Sounds _sound;
        
        private ISoundService _soundService;
        private bool _isInitialized;
        
        private string _soundId;
        
        private async void Start()
        {
            _soundService = ServiceLocator.Instance.GetService<ISoundService>();
            await _soundService.WaitForInitialization();
            _isInitialized = true;
            _soundId = SoundIdConverter.GetId(_sound);
        }
        
        public void Execute()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            _soundService.PlayOneShot(_soundId, transform.position);
        }
    }
}