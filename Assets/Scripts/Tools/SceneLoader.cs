using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooter.Tools
{ 
    public class SceneLoader
    {
        private AsyncOperation _asyncOperation;

        public async void LoadScene(int indexScene)
        {
            try
            {
                await AsyncLoadScene(indexScene);
            }
            catch (OperationCanceledException exception)
            {
                Debug.LogError(exception);
            }
        }

        private async Task AsyncLoadScene(int indexScene)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(indexScene);
            _asyncOperation.allowSceneActivation = false;
            return;
        }

        public void AllowLoadScene()
        {
            _asyncOperation.allowSceneActivation = true;
        }
    }
}
