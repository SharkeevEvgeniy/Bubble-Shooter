using UnityEngine;

namespace BubbleShooter.Session.DrawTrajectory
{
    public class TrajectoryDrawer : MonoBehaviour
    {
        private const int _rendererPositionCount = 2;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LineRenderer _lineRenderer2;

        [SerializeField] private Color _scatterColor;
        [SerializeField] private Color _standartColor;

        private int _countPoints;

        public void Disable()
        {
            _lineRenderer.enabled = false;
            _lineRenderer2.enabled = false;
        }

        public void Draw(TrajectoryInfo trajectoryInfo, Vector2 ballPosition)
        {
            _countPoints = trajectoryInfo.GetPositions().Count;
            _lineRenderer2.positionCount = _rendererPositionCount;
            _lineRenderer.enabled = true;

            switch (trajectoryInfo.TrajectoryType)
            {
                case TrajectoryType.Standart:

                    _lineRenderer.startColor = _standartColor;
                    _lineRenderer.endColor = _standartColor;

                    _lineRenderer2.enabled = false;

                    _lineRenderer.positionCount = _countPoints + 1;
                    _lineRenderer.SetPosition(0, ballPosition);

                    for (int i = 1; i <= _countPoints; i++)
                    {
                        _lineRenderer.SetPosition(i, trajectoryInfo.GetPositions()[i - 1]);
                    }

                    break;

                case TrajectoryType.Scatter:

                    _lineRenderer.startColor = _scatterColor;
                    _lineRenderer.endColor = _scatterColor;

                    if (_countPoints == 2)
                    {
                        _lineRenderer2.enabled = false;
                        _lineRenderer.positionCount = _countPoints + 1;

                        _lineRenderer.SetPosition(0, trajectoryInfo.GetPositions()[0]);
                        _lineRenderer.SetPosition(1, ballPosition);
                        _lineRenderer.SetPosition(2, trajectoryInfo.GetPositions()[1]);
                    }
                    else if (trajectoryInfo.DirectionSide == 1 && _countPoints == 5)
                    {
                        _lineRenderer2.enabled = true;
                        _lineRenderer.positionCount = _countPoints - 1;

                        _lineRenderer.SetPosition(0, trajectoryInfo.GetPositions()[0]);
                        _lineRenderer.SetPosition(1, ballPosition);
                        _lineRenderer.SetPosition(2, trajectoryInfo.GetPositions()[2]);
                        _lineRenderer.SetPosition(3, trajectoryInfo.GetPositions()[4]);

                        _lineRenderer2.SetPosition(0, trajectoryInfo.GetPositions()[1]);
                        _lineRenderer2.SetPosition(1, trajectoryInfo.GetPositions()[3]);
                    }
                    if (trajectoryInfo.DirectionSide == -1 && _countPoints == 5)
                    {
                        _lineRenderer2.enabled = true;
                        _lineRenderer.positionCount = _countPoints - 1;

                        _lineRenderer.SetPosition(0, trajectoryInfo.GetPositions()[1]);
                        _lineRenderer.SetPosition(1, ballPosition);
                        _lineRenderer.SetPosition(2, trajectoryInfo.GetPositions()[2]);
                        _lineRenderer.SetPosition(3, trajectoryInfo.GetPositions()[4]);

                        _lineRenderer2.SetPosition(0, trajectoryInfo.GetPositions()[0]);
                        _lineRenderer2.SetPosition(1, trajectoryInfo.GetPositions()[3]);
                    }

                    break;
            }
        }
    }
}
