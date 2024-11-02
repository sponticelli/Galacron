using UnityEngine;

namespace Galacron
{
    public class BlinkTMPText : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _text;
        [SerializeField] private float _blinkSpeed = 1f;
        
        private float _blinkTimer;
        
        private void Update()
        {
            _blinkTimer += Time.deltaTime;
            if (_blinkTimer >= _blinkSpeed)
            {
                _blinkTimer = 0;
                _text.enabled = !_text.enabled;
            }
        }
    }
}