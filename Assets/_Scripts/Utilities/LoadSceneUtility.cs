using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utilities
{
    public class LoadSceneUtility : MonoBehaviour
    {
        public static LoadSceneUtility Instance;

        private AsyncOperation asyncScene;

        private void Awake()
        {
            Instance = this;
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
        public IEnumerator LoadSceneAsync(int index, int waitFrames)
        {
            int currentFrame = 0;
            while (currentFrame < waitFrames)
            {
                currentFrame++;
                yield return new WaitForEndOfFrame();
            }

            LoadSceneAsync(index);
        }

        public void UnloadSceneAsync(int index)
        {
            SceneManager.UnloadSceneAsync(index);
        }
        #endregion
    }
}
