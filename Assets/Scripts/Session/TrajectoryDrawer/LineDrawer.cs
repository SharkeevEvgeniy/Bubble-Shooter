using UnityEngine;

namespace BubbleShooter.Session.DrawTrajectory
{
    public class LineDrawer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        [SerializeField] private Transform[] _points;

        private void Start()
        {
            _lineRenderer.positionCount = _points.Length;
        }

        private void Update()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _lineRenderer.SetPosition(i, _points[i].position);
            }
        }
    }
}
