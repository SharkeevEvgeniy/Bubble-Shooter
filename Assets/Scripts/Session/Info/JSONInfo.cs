using System;

namespace BubbleShooter.Session.Info
{
    [Serializable]
    public struct JSONInfo
    {
        public float X;
        public float Y;
        public int ConnectTo;
        public bool IsRoot;
        public int Color;
        public int Row;
        public int Column;
    }
}
