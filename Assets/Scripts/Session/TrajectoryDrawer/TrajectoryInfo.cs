using System.Collections.Generic;
using UnityEngine;

public class TrajectoryInfo
{
    public int DirectionSide { get; set; }

    public TrajectoryType TrajectoryType { get; set; }

    private List<Vector2> _positions;

    public List<Vector2> GetPositions()
    {
        if (_positions == null)
            throw new System.NullReferenceException();

        return _positions;
    }

    public void SetPositions(Vector2[] positions)
    {
        _positions = new List<Vector2>();

        foreach (Vector2 position in positions)
        {
            if (position != Vector2.zero)
                _positions.Add(position);
        }
    }
}
