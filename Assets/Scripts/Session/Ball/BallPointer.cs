using BubbleShooter.Session.DrawTrajectory;
using UnityEngine;

namespace BubbleShooter.Session.Ball
{
    public class BallPointer : MonoBehaviour
    {
        [SerializeField] private TrajectoryDrawer _trajectoryDrawer;
        [SerializeField] private GameSession _gameSession;

        [SerializeField] private Transform _ballStartPosition;
        [SerializeField] private Transform _center;
        [SerializeField] private Transform[] _points;

        [SerializeField] private float _radius;
        [SerializeField] private float _offset;
        [SerializeField] private float _distance;
        [SerializeField] private float _angleMultiple;

        private Transform _ballTransform;
        private Ball _ball;

        private bool _isDragging;
        private bool _lockDragging;

        private ITrajectoryPointer _trajectoryPointer;
        private TrajectoryInfo _trajectoryInfo;
        private TrajectoryType _trajectoryType;

        private void Awake()
        {
            _trajectoryInfo = new TrajectoryInfo();
        }

        private void Start()
        {
            _lockDragging = true;
            _trajectoryType = TrajectoryType.Standart;
            _trajectoryPointer = new StandartTrajectoryStrategy();
        }

        private void OnMouseUp()
        {
            SetLock(true);
            Disable();

            _isDragging = false;

            _ball.gameObject.transform.SetParent(null);
            _ball.SetDirection((_center.position - _ballTransform.position).normalized);

            float distance = Vector2.Distance(_center.position, _ballTransform.position);
            _ball.SetVelocityMultiple(distance);

            transform.position = _ballStartPosition.position;

            _gameSession.MoveNext();
        }

        private void OnMouseDrag()
        {
            if (_lockDragging == true)
                return;

            _isDragging = true;

            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
            Vector3 offset = position - _center.position;
            position = _center.position + Vector3.ClampMagnitude(offset, _radius);
            position.y = Mathf.Clamp(position.y, _center.position.y - _radius, _center.position.y - _offset);

            transform.position = position;
        }

        private void SetTrajectoryType(ITrajectoryPointer trajectoryPointer)
        {
            if (_trajectoryType != (trajectoryPointer is StandartTrajectoryStrategy ? TrajectoryType.Standart : TrajectoryType.Scatter))
            {
                _trajectoryPointer = trajectoryPointer;
                _trajectoryType = trajectoryPointer is StandartTrajectoryStrategy ? TrajectoryType.Standart : TrajectoryType.Scatter;
            }
        }

        public void SetBall(Transform ballTransform, Ball ball)
        {
            _ballTransform = ballTransform;
            _ball = ball;
        }

        public void SetLock(bool value) => _lockDragging = value;


        private void Disable()
        {
            _trajectoryDrawer.Disable();

            foreach (Transform point in _points)
            {
                point.gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (_isDragging == false)
                return;

            float distance = Vector2.Distance(_center.position, _ballTransform.position);

            if (distance >= _distance)
            {
                SetTrajectoryType(new ScatterTrajectoryStrategy());

                _trajectoryInfo = _trajectoryPointer.GetTrajectoryInfo(_center, _ballTransform, distance / _angleMultiple);

                _ball.SetActionType(BallActionType.Replace);
            }
            else
            {
                SetTrajectoryType(new StandartTrajectoryStrategy());
                _trajectoryInfo = _trajectoryPointer.GetTrajectoryInfo(_center, _ballTransform, 0f);

                _ball.SetActionType(BallActionType.Attach);
            }

            for (int i = 0; i < _points.Length; i++)
            {
                if (_trajectoryInfo.GetPositions().Count != 0)
                {
                    if (i < _trajectoryInfo.GetPositions().Count)
                    {
                        _points[i].gameObject.SetActive(true);
                        _points[i].position = _trajectoryInfo.GetPositions()[i];
                    }
                    else _points[i].gameObject.SetActive(false);
                }
            }

            _trajectoryDrawer.Draw(_trajectoryInfo, _ballTransform.position);
        }
    }
}

