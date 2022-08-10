using UnityEngine;

public interface ITrajectoryPointer
{
    TrajectoryInfo GetTrajectoryInfo(Transform center, Transform ball, float angle);
}
