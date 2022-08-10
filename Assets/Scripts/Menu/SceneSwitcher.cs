using UnityEngine;
using BubbleShooter.Tools;

namespace BubbleShooter.Menu
{
    public class SceneSwitcher : MonoBehaviour, IExitTransitionHandler
    {
        private SceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = new SceneLoader();
        }

        public void LoadScene(int index)
        {
            Transition.Instance.Make(1f, this);
            _sceneLoader.LoadScene(index);
        }

        public void TransitionHandle()
        {
            _sceneLoader.AllowLoadScene();
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
