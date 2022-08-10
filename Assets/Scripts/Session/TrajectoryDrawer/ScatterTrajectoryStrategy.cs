using UnityEngine;

namespace BubbleShooter.Session.DrawTrajectory
{
    public class ScatterTrajectoryStrategy : ITrajectoryPointer
    {
        private const int _countPoints = 5;
        private const float _optimizeParameter = 1.001f;
        private const float _rayLenght = 20f;
        private const float _angleMultiple = 3f;
        private const float _offset = 2.2f;

        private Vector2[] _positions;
        private Vector2[] _directions;
        private Vector2[] _normals;

        private RaycastHit2D[] _hits;

        private int _directionSide;

        private TrajectoryInfo _trajectoryInfo;

        public ScatterTrajectoryStrategy()
        {
            _trajectoryInfo = new TrajectoryInfo();

            _positions = new Vector2[_countPoints];
            _directions = new Vector2[2];
            _normals = new Vector2[2];

            _hits = new RaycastHit2D[_countPoints - 1];
        }

        private void RefreshTrajectoryInfo()
        {
            _trajectoryInfo.SetPositions(_positions);
            _trajectoryInfo.TrajectoryType = TrajectoryType.Scatter;
            _trajectoryInfo.DirectionSide = _directionSide;
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

            _directionSide = center.position.x < ball.position.x ? 1 : -1;

            for (int i = 0; i < 2; i++)
            {
                _directions[i] = (center.position - ball.position).normalized;

                _directions[i].x = _directions[i].x * Mathf.Cos(angle + (angle * 2f * -i)) - _directions[i].y * Mathf.Sin(angle + (angle * 2f * -i));
                _directions[i].y = _directions[i].y * Mathf.Cos(angle + (angle * 2f * -i)) + _directions[i].x * Mathf.Sin(angle + (angle * 2f * -i));

                _hits[i] = Physics2D.Raycast(ball.position / _optimizeParameter, _directions[i], _rayLenght, 1 << LayerMask.NameToLayer("Reflection"));

                _normals[i] = _hits[i].normal;
                _positions[i] = _hits[i].point;
            }

            if (_hits[0].transform.CompareTag("Ball") == true ||
                _hits[0].transform.CompareTag("BorderUp") == true ||
                _hits[1].transform.CompareTag("BorderUp") == true)
            {
                RefreshTrajectoryInfo();
                return _trajectoryInfo;
            }

            _positions[2] = (_positions[0] + _positions[1]) / 2f;
            _positions[2] += _hits[0].normal * _offset;

            for (int i = 3; i < _countPoints; i++)
            {
                if (_directionSide == 1)
                    _positions[i] = Vector2.Reflect(_positions[1 - (i - 3)], _hits[1 - (i - 3)].normal);
                if (_directionSide == -1)
                    _positions[i] = Vector2.Reflect(_positions[0 + (i - 3)], _hits[0 + (i - 3)].normal);

                _positions[i].x = _positions[i].x * Mathf.Cos(angle * _angleMultiple * _directionSide) - _positions[i].y * Mathf.Sin(angle * _angleMultiple * _directionSide);
                _positions[i].y = _positions[i].y * Mathf.Cos(angle * _angleMultiple * _directionSide) + _positions[i].x * Mathf.Sin(angle * _angleMultiple * _directionSide);

                if (_directionSide == 1)
                    _hits[i - 1] = Physics2D.Raycast(_positions[1 - (i - 3)] / _optimizeParameter, _positions[i], _rayLenght, 1 << LayerMask.NameToLayer("Reflection"));
                if (_directionSide == -1)
                    _hits[i - 1] = Physics2D.Raycast(_positions[0 + (i - 3)] / _optimizeParameter, _positions[i], _rayLenght, 1 << LayerMask.NameToLayer("Reflection"));

                if (_hits[i - 1])
                {
                    _positions[i] = _hits[i - 1].point;
                }
            }

            RefreshTrajectoryInfo();

            return _trajectoryInfo;
        }
    }
}
