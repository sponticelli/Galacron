using System.Collections;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.FloatingTexts
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshPro text;
        [SerializeField] private Vector3 direction = Vector3.up * 0.5f;
        [SerializeField] private float duration = 1f;
        
        public void SetText(string message)
        {
            text.text = message;
        }
        
        public void SetColor(Color color)
        {
            text.color = color;
        }

        public void Animate()
        {
            StartCoroutine(AnimateCoroutine());
        }

        private IEnumerator AnimateCoroutine()
        {
            var time = 0f;
            var start = transform.position;
            var end = start + direction;
            var color = text.color;
            var startColor = color;
            var endColor = new Color(color.r, color.g, color.b, 0);
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, time / duration);
                text.color = Color.Lerp(startColor, endColor, time / duration);
                yield return null;
            }
            gameObject.ReturnToPool();
        }
    }
}