using UnityEngine;

namespace BubbleShooter.Session.Ball
{
    public class BallInfo : MonoBehaviour
    {
        private const int _addPointsCount = 100;

        public GameSession GameSession { get; private set; }
        public int BallColor { get; private set; }

        private int _row;
        private int _column;

        public void SetData(int row, int column, int color, GameSession session)
        {
            _row = row;
            _column = column;
            BallColor = color;

            GameSession = session;
        }

        public int GetRow() => _row;

        public int GetColumn() => _column;

        public void SetRowAndColumn(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public void PrepareToDestroy()
        {
            GameSession.ResetRowAndColumn(_row, _column);
            GameSession.CheckOnWin();
            GameSession.CheckOnLose();
            GameSession.AddScore(_addPointsCount);
        }
    }
}
