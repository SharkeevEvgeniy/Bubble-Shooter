using UnityEngine;

namespace BubbleShooter.Session.Info
{
    public class UserPrefs : MonoBehaviour
    {
        private int _bestResult;

        private void Awake()
        {
            if (PlayerPrefs.HasKey("Best"))
                _bestResult = PlayerPrefs.GetInt("Best");
        }

        public int GetBestResult() => _bestResult;

        public void SetBestResult(int value) => PlayerPrefs.SetInt("Best", value);
    }
}