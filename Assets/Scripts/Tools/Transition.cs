using System.Collections;
using UnityEngine;

namespace BubbleShooter.Tools
{
    public class Transition : MonoBehaviour
    {
        public static Transition Instance { get; private set; }

        [SerializeField] private GameObject _transitionPanel;

        private Animator _animator;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            _animator = _transitionPanel.GetComponent<Animator>();
        }

        public void Make(float duration, IExitTransitionHandler handler)
        {
            StartCoroutine(MakeTransitionRoutine(duration, handler));
        }

        private IEnumerator MakeTransitionRoutine(float duration, IExitTransitionHandler handler)
        {
            _transitionPanel.SetActive(true);
            _animator.speed = 1f / duration;
            yield return new WaitForSeconds(duration / 2f);
            handler.TransitionHandle();
            yield return new WaitForSeconds(duration / 2f);
            _transitionPanel.SetActive(false);
        }
    }
}
