using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SargeUniverse.Scripts
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Animator _transition;
        [SerializeField] private float _transitionTime = 1f;

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneWithCrossfade(sceneName));
        }

        private IEnumerator LoadSceneWithCrossfade(string sceneName)
        {
            _transition.SetTrigger("ShowCrossfade");
            yield return new WaitForSeconds(_transitionTime);
            
            SceneManager.LoadScene(sceneName);
        }
    }
}
