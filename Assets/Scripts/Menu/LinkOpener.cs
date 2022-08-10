using UnityEngine;

namespace BubbleShooter.Menu
{
    public class LinkOpener : MonoBehaviour
    {
        private const string _url = "https://github.com/SharkeevEvgeniy";

        public void Open()
        {
            Application.OpenURL(_url);
        }
    }
}
