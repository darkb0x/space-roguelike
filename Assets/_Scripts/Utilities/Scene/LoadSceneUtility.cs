using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utilities.LoadScene
{
    using Visual;

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

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        #region Async
        public void EnableLoadedAsyncScene()
        {
            asyncScene.allowSceneActivation = true;
        }

        public void LoadSceneAsync(int index)
        {
            if(asyncScene != null)
            {
                if (!asyncScene.isDone)
                    return;
            }

            asyncScene = SceneManager.LoadSceneAsync(index);
            asyncScene.allowSceneActivation = false;
        }  
        public IEnumerator LoadSceneAsync(int index, int waitFrames = 0)
        {
            int currentFrame = 0;
            while (currentFrame < waitFrames)
            {
                currentFrame++;
                yield return new WaitForEndOfFrame();
            }

            LoadSceneAsync(index);
        }
        public IEnumerator LoadSceneAsyncVisualize(int index)
        {
            Visual.SetEnabled(true);

            yield return new WaitForSecondsRealtime(0.3f);

            PlayerPrefs.SetInt("LoadingSceen_used", 1);

            asyncScene = SceneManager.LoadSceneAsync(index);

            while (!asyncScene.isDone)
            {
                Visual.UpdateProgress(asyncScene.progress);
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
