using UnityEngine;

namespace Galacron.Actions
{
    public class LoadSceneAction :  MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        
        public void Execute()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
        }
        
    }
}