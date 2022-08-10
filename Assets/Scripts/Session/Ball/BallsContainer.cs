using System;
using System.Collections.Generic;
using BubbleShooter.Session.Info;

namespace BubbleShooter.Session.Ball
{
    [Serializable]
    public class BallsContainer
    {
        public int RowCount;
        public int ColumnCount;
        public List<JSONInfo> JSONInfo;
    }
}