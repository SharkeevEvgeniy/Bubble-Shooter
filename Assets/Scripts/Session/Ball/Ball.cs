using UnityEngine;

namespace BubbleShooter.Session.Ball
{
    public class Ball : MonoBehaviour
    {
        private const float _forceHit = 100f;
        private const float _colliderSize = 1.3f;

        [SerializeField] private float _startVelocity;
        private float _velocityMultiple;

        private Vector2 _direction;
        private BallActionType _actionType;

        private bool _isAttached;

        private CircleCollider2D _collider;
        private SpringJoint2D _springJoint;
        private Rigidbody2D _rigidbody;

        private GameSession _gameSession;
        private BallInfo _ballInfo;

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _springJoint = GetComponent<SpringJoint2D>();
            _ballInfo = GetComponent<BallInfo>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (_isAttached == true)
                return;

            Move();
        }

        private void Move()
        {
            transform.Translate(_direction * Time.deltaTime * (_startVelocity + (_startVelocity * _velocityMultiple)));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Border") == true ||
                collision.gameObject.CompareTag("BorderUp") == true)
            {
                _direction = Vector2.Reflect(_direction, collision.contacts[0].normal);
            }

            if (_isAttached == true)
                return;

            if (collision.gameObject.CompareTag("Ball") == true)
            {
                _isAttached = true;

                if (_actionType == BallActionType.Attach)
                {
                    _gameSession.Check(_ballInfo);
                    _springJoint.connectedBody = collision.rigidbody;
                }
                else
                {
                    BallInfo info = collision.gameObject.GetComponent<BallInfo>();
                    SpringJoint2D joint = collision.gameObject.GetComponent<SpringJoint2D>();

                    _gameSession.ReplaceBall(info, _ballInfo, _springJoint, joint);
                    _gameSession.Check(_ballInfo);
                }

                _gameSession.AddBallInfo(_ballInfo);

                _collider.radius = _colliderSize;
                collision.rigidbody.AddForce(_direction * _forceHit);
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                _springJoint.enabled = true;

                this.enabled = false;
            }
        }

        public void SetGameSession(GameSession session) => _gameSession = session;

        public void SetAttachedInfo(BallInfo info) => _ballInfo = info;

        public void SetActionType(BallActionType actionType) => _actionType = actionType;

        public void SetVelocityMultiple(float velocityMuiltiple) => _velocityMultiple = velocityMuiltiple;
        
        public void SetDirection(Vector2 direction)
        {
            _collider.enabled = true;
            _direction = direction;
        }
    }
}
