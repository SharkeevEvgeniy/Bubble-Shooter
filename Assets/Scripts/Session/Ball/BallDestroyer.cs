using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Session.Ball
{
    public class BallDestroyer : MonoBehaviour
    {
        private List<GameObject> _container;

        [SerializeField] private float _destroyDistance;

        public event Action<int, Vector2> OnDestroyObjectEvent;

        private void Awake()
        {
            _container = new List<GameObject>();
        }

        public bool Check(BallInfo current, List<BallInfo> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].BallColor == current.BallColor)
                {
                    if (Vector2.Distance(current.transform.position, balls[i].transform.position) < _destroyDistance)
                    {
                        _container.Add(balls[i].gameObject);
                        balls[i].PrepareToDestroy();
                    }
                }
            }

            if (_container.Count > 0)
            {
                _container.Add(current.gameObject);

                current.PrepareToDestroy();

                return true;
            }
            else return false;
        }

        public void DestroyAllFinded()
        {
            if (_container.Count == 0)
                return;

            foreach (GameObject obj in _container)
            {
                obj.SetActive(false);

                OnDestroyObjectEvent.Invoke(0, obj.transform.position);
            }

            _container.Clear();
        }

        public void Destroy(BallInfo ballInfo)
        {
            ballInfo.PrepareToDestroy();
            ballInfo.gameObject.SetActive(false);

            OnDestroyObjectEvent.Invoke(0, ballInfo.transform.position);
        }

        public void Destroy(GameObject obj)
        {
            obj.gameObject.SetActive(false);

            OnDestroyObjectEvent.Invoke(0, obj.transform.position);
        }
    }
}
