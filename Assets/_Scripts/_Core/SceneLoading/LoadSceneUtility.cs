using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneLoading
{
    public class LoadSceneUtility : MonoBehaviour
    {
        [SerializeField] private LoadingScreenVisual Visual;

        public static LoadSceneUtility Instance;

        private AsyncOperation asyncScene;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            bool loadingScreenHasbeenUsed = PlayerPrefs.GetInt("LoadingSceen_used") == 1 ? false : true;

            Visual.SetEnabled(loadingScreenHasbeenUsed);

            if(loadingScreenHasbeenUsed)
            {
                Visual.UpdateProgress(1f);
            }
        }

        public static void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
        public static void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        #region Async
        public static void EnableLoadedAsyncScene()
        {
            Instance.asyncScene.allowSceneActivation = true;
        }

        public static void LoadSceneAsync(int index)
        {
            if (Instance.asyncScene != null)
            {
                if (!Instance.asyncScene.isDone)
                    return;
            }

            Instance.asyncScene = SceneManager.LoadSceneAsync(index);
            Instance.asyncScene.allowSceneActivation = false;
        }  
        public static void LoadSceneAsync(int index, int waitFrames)
        {
            Instance.StartCoroutine(LoadSceneAsyncCoroutine(index, waitFrames));
        }
        private static IEnumerator LoadSceneAsyncCoroutine(int index, int waitFrames = 1)
        {
            int currentFrame = 0;
            while (currentFrame < waitFrames)
            {
                currentFrame++;
                yield return new WaitForEndOfFrame();
            }

            LoadSceneAsync(index);
        }

        public static void LoadSceneAsyncVisualize(int index)
        {
            Instance.StartCoroutine(LoadSceneAsyncVisualizeCoroutine(index));
        }
        private static IEnumerator LoadSceneAsyncVisualizeCoroutine(int index)
        {
            Instance.Visual.SetEnabled(true);

            yield return new WaitForSecondsRealtime(0.3f);

            PlayerPrefs.SetInt("LoadingSceen_used", 1);

            Instance.asyncScene = SceneManager.LoadSceneAsync(index);

            while (!Instance.asyncScene.isDone)
            {
                Instance.Visual.UpdateProgress(Instance.asyncScene.progress);
                yield return null;
            }
        }

        public void UnloadSceneAsync(int index)
        {
            SceneManager.UnloadSceneAsync(index);
        }
        #endregion
    }
}
