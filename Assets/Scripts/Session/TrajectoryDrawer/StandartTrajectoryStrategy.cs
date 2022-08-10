using UnityEngine;

namespace BubbleShooter.Session.DrawTrajectory
{
    public class StandartTrajectoryStrategy : ITrajectoryPointer
    {
        private const int _countPoints = 2;
        private const float _optimizeParameter = 1.001f;
        private const float _rayLenght = 20f;

        private Vector2[] _directions;
        private Vector2[] _startPoints;
        private Vector2[] _positions;

        private RaycastHit2D _hit;

        private TrajectoryInfo _trajectoryInfo;

        public StandartTrajectoryStrategy()
        {
            _trajectoryInfo = new TrajectoryInfo();

            _directions = new Vector2[_countPoints];
            _startPoints = new Vector2[_countPoints];
            _positions = new Vector2[_countPoints];
        }

        private void RefreshTrajectoryInfo()
        {
            _trajectoryInfo.SetPositions(_positions);
            _trajectoryInfo.TrajectoryType = TrajectoryType.Standart;
        }

        private void ClearPositions()
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                _positions[i] = Vector2.zero;
            }
        }

        public TrajectoryInfo GetTrajectoryInfo(Transform center, Transform ball, float angle)
        {
            ClearPositions();

            _directions[0] = (center.position - ball.position).normalized;
            _startPoints[0] = ball.position;

            _directions[0].x = _directions[0].x * Mathf.Cos(angle) - _directions[0].y * Mathf.Sin(angle);
            _directions[0].y = _directions[0].y * Mathf.Cos(angle) + _directions[0].x * Mathf.Sin(angle);

            for (int i = 0; i < _countPoints;)
            {
                _hit = Physics2D.Raycast(_startPoints[i] / _optimizeParameter, _directions[i], _rayLenght, 1 << LayerMask.NameToLayer("Reflection"));

                _positions[i] = _hit.point;

                if (_hit.transform.CompareTag("Ball") == true || _hit.transform.CompareTag("BorderUp") == true)
                {
                    RefreshTrajectoryInfo();
                    return _trajectoryInfo;
                }

                i++;
                if (i >= _countPoints)
                    break;

                _startPoints[i] = _positions[i - 1];
                _directions[i] = _directions[i - 1];
                _directions[i] = Vector2.Reflect(_directions[i], _hit.normal);
            }

            RefreshTrajectoryInfo();

            return _trajectoryInfo;
        }
    }
}
