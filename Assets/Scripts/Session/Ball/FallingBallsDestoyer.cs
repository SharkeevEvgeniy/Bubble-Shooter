using UnityEngine;

namespace BubbleShooter.Session.Ball
{
    public class FallingBallsDestoyer : MonoBehaviour
    {
        [SerializeField] private BallDestroyer _ballDestroyer;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                _ballDestroyer.Destroy(collision.gameObject.GetComponent<BallInfo>());
            }
        }
    }
}
