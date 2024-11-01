using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Galacron.UI
{
    public class TMPTypingEffect : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private float punctuationPause = 0.2f;
        [SerializeField] private float pauseAfterTyping = 0.25f;
        
    
        public UnityEvent onTypingComplete;
    
        private string targetText;
        private Coroutine typingCoroutine;

        private void Awake()
        {
            if (textComponent == null)
                textComponent = GetComponent<TextMeshProUGUI>();
        }

        public void StartTyping(string text)
        {
            if  (targetText == text)
                return;
            
            targetText = text;
        
            // Stop any existing typing coroutine
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            typingCoroutine = StartCoroutine(TypeText());
        }

        private IEnumerator TypeText()
        {
            textComponent.text = "";
            yield return new WaitForSeconds(pauseAfterTyping);
        
            foreach (char c in targetText)
            {
                textComponent.text += c;
            
                // Add extra pause for punctuation
                if (char.IsPunctuation(c))
                    yield return new WaitForSeconds(punctuationPause);
                else
                    yield return new WaitForSeconds(typingSpeed);
            }
        
            onTypingComplete?.Invoke();
        }

        // Stop typing immediately and show full text
        public void SkipTyping()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                textComponent.text = targetText;
                onTypingComplete?.Invoke();
            }
        }
    }
}