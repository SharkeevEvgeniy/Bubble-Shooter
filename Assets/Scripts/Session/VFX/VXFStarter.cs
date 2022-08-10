using BubbleShooter.Session.Ball;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Session.VFX
{
    public class VXFStarter : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _startupEffectPrefabs;
        [SerializeField] private List<GameObject> _recycledEffectPrefabs;

        [SerializeField] private BallDestroyer _ballDestroyer;

        [SerializeField] private int _poolingCapacity;

        private GameObject[,] _container;

        private void Awake()
        {
            foreach (GameObject effect in _startupEffectPrefabs)
            {
                Instantiate(effect, transform);
            }

            _container = new GameObject[_poolingCapacity, _recycledEffectPrefabs.Count];

            for (int i = 0; i < _recycledEffectPrefabs.Count; i++)
            {
                for (int j = 0; j < _poolingCapacity; j++)
                {
                    _container[j, i] = (Instantiate(_recycledEffectPrefabs[i], transform));
                    _container[j, i].SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            _ballDestroyer.OnDestroyObjectEvent += Play;
        }

        private void OnDisable()
        {
            _ballDestroyer.OnDestroyObjectEvent -= Play;
        }

        public void Play(int effectIndex, Vector2 position)
        {
            GameObject poolObject = PoolObject(effectIndex);
            poolObject.SetActive(true);
            poolObject.transform.position = position;
        }

        private GameObject PoolObject(int effectIndex)
        {
            for (int i = 0; i < _poolingCapacity; i++)
            {
                if (_container[i, effectIndex].activeSelf == false)
                    return _container[i, effectIndex];
            }

            return null;
        }
    }
}
