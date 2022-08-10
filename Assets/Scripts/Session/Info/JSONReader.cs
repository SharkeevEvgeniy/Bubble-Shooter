using BubbleShooter.Session.Ball;
using System.IO;
using UnityEngine;

namespace BubbleShooter.Session.Info
{
    public class JSONReader
    {
        public BallsContainer GetContainer(string path)
        {
            BallsContainer ballsContainer = JsonUtility.FromJson<BallsContainer>(File.ReadAllText(path));
            return ballsContainer;
        }
    }
}
