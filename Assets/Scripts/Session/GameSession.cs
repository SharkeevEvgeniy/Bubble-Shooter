using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using BubbleShooter.Session.Ball;
using BubbleShooter.Session.Info;

namespace BubbleShooter.Session 
{ 
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private float _tweenDuration;
        [SerializeField] private float _percentBallsToWin;
        [SerializeField] private int _movesCount;

        private bool _isTweening;
        private float _startTweeningTime;

        [SerializeField] private GameObject[] _ballPrefabs;
        [SerializeField] private GameObject[] _ballCreatablePrefabs;
        [SerializeField] private Transform _ballsParent;
        [SerializeField] private Transform _optionalBallsPosition;

        private List<Ball.Ball> _balls;
        private int[,] _ballsPositions;
        private List<GameObject> _ballsContainer;
        private List<GameObject> _ballsCreatableContainer;
        private int _currentBall;

        private int _score;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _scoreTextInSession;
        [SerializeField] private TextMeshPro _movesCountText;
        [SerializeField] private TextMeshProUGUI _bestText;
        [SerializeField] private TextMeshProUGUI _endGameText;
        [SerializeField] private GameObject _endGamePanel;

        [SerializeField] BallPointer _ballPointer;

        [SerializeField] private BallDestroyer _ballDestroyer;

        [SerializeField] private UserPrefs _userPrefs;

        private List<BallInfo> _ballInfos;
        private BallsContainer _JSONContainer;

        private bool _isLastBall;
        private bool _isEndGame;

        private void Awake()
        {
            _ballsContainer = new List<GameObject>();
            _ballsCreatableContainer = new List<GameObject>();
            _ballInfos = new List<BallInfo>();
            _balls = new List<Ball.Ball>();

            JSONReader reader = new JSONReader();
            _JSONContainer = reader.GetContainer($"{Application.streamingAssetsPath}/level.json");
        }

        private void Start()
        {
            Initialize();

            MoveNext();
        }

        private void Initialize()
        {
            _ballsPositions = new int[_JSONContainer.RowCount, _JSONContainer.ColumnCount];

            for (int i = 0; i < _JSONContainer.RowCount; i++)
            {
                for (int j = 0; j < _JSONContainer.ColumnCount; j++)
                {
                    _ballsPositions[i, j] = -1;
                }
            }

            for (int i = 0; i < _JSONContainer.JSONInfo.Count; i++)
            {
                GameObject ball = Instantiate(_ballCreatablePrefabs[_JSONContainer.JSONInfo[i].Color], _ballsParent);
                _ballsCreatableContainer.Add(ball);
                _ballsCreatableContainer[i].transform.localPosition = new Vector2(_JSONContainer.JSONInfo[i].X, _JSONContainer.JSONInfo[i].Y);

                BallInfo info = _ballsCreatableContainer[i].AddComponent<BallInfo>();
                info.SetData(_JSONContainer.JSONInfo[i].Row, _JSONContainer.JSONInfo[i].Column, _JSONContainer.JSONInfo[i].Color, this);
                _ballInfos.Add(info);

                _ballsPositions[_JSONContainer.JSONInfo[i].Row, _JSONContainer.JSONInfo[i].Column] = 1;
            }

            for (int i = 0; i < _ballsCreatableContainer.Count; i++)
            {
                if (_JSONContainer.JSONInfo[i].IsRoot)
                {
                    _ballsCreatableContainer[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                }
                else
                {
                    _ballsCreatableContainer[i].GetComponent<SpringJoint2D>().connectedBody =
                     _ballsCreatableContainer[_JSONContainer.JSONInfo[i].ConnectTo].GetComponent<Rigidbody2D>();
                }
            }

            for (int i = 0; i < _movesCount; i++)
            {
                int color = i % _ballPrefabs.Length;

                GameObject ballObj = Instantiate(_ballPrefabs[color]);
                
                BallInfo info = ballObj.AddComponent<BallInfo>();
                info.SetData(0, 0, color, this);

                ballObj.transform.position = _optionalBallsPosition.position;
                ballObj.SetActive(i == 0 ? true : false);

                Ball.Ball ball = ballObj.GetComponent<Ball.Ball>();
                ball.SetGameSession(this);
                ball.SetAttachedInfo(info);

                _balls.Add(ball);
                _ballsContainer.Add(ballObj);
            }
        }

        public void MoveNext()
        {
            if (_currentBall < _movesCount - 1)
            {
                _ballsContainer[_currentBall + 1].SetActive(true);
            }

            if (_currentBall < _movesCount)
            {
                _ballPointer.SetBall(_ballsContainer[_currentBall].transform, _balls[_currentBall]);
                _ballsContainer[_currentBall].transform.SetParent(_ballPointer.transform);

                _isTweening = true;
                _startTweeningTime = Time.time;

                ShowMovesCount(_movesCount - (_currentBall + 1));
            }

            _currentBall++;

            if (_currentBall > _movesCount)
            {
                _isLastBall = true;
            }
        }

        public void CheckOnWin()
        {
            if (_isEndGame)
                return;

            float percent = 100f / _JSONContainer.RowCount;
            float currentPercent = 0f;

            for (int i = _JSONContainer.ColumnCount - 1; i >= 0; i--)
            {
                for (int j = 0; j < _JSONContainer.RowCount; j++)
                {
                    if (i == 0)
                    {
                        if (_ballsPositions[j, i] == -1)
                        {
                            currentPercent += percent;

                            if (currentPercent >= _percentBallsToWin)
                            {
                                EndGameSession(EndSessionType.Win);
                                return;
                            }
                        }
                        continue;
                    }

                    if (_ballsPositions[j, i] == 1)
                    {
                        return;
                    }
                }
            }
        }

        public void CheckOnLose()
        {
            if (_isEndGame)
                return;

            if (_isLastBall)
                EndGameSession(EndSessionType.Lose);
        }

        private void EndGameSession(EndSessionType endSessionType)
        {
            _isEndGame = true;
            _endGamePanel.SetActive(true);
            _endGameText.text = endSessionType == EndSessionType.Win ? "Win!" : "Lose";

            _scoreText.text = $"Score: {_score}";

            int best = _userPrefs.GetBestResult();

            if (_score > best && endSessionType == EndSessionType.Win)
            {
                best = _score;
                _userPrefs.SetBestResult(best);
            }

            _bestText.text = $"Best: {best}";
        }

        public void AddScore(int value)
        {
            _score += value;
            _scoreTextInSession.text = $"Score: {_score}";
        }

        public void Check(BallInfo current)
        {
            if (_ballDestroyer.Check(current, _ballInfos) == true)
            {
                _ballDestroyer.DestroyAllFinded();
            }
        }

        public void ReplaceBall(BallInfo fromInfo, BallInfo toInfo, SpringJoint2D from, SpringJoint2D to)
        {
            toInfo.SetRowAndColumn(fromInfo.GetRow(), fromInfo.GetColumn());

            from.transform.position = to.transform.position;
            from.connectedBody = to.connectedBody;

            _ballDestroyer.Destroy(to.gameObject);
        }

        public void ResetRowAndColumn(int row, int column)
        {
            _ballsPositions[row, column] = -1;
        }

        public void AddBallInfo(BallInfo ballInfo)
        {
            _ballInfos.Add(ballInfo);
        }

        private void ShowMovesCount(int count)
        {
            if (count <= 0)
            {
                _movesCountText.enabled = false;
            }

            _movesCountText.text = count.ToString();
        }

        private void Update()
        {
            if (_isTweening == false)
                return;

            if (Time.time - _startTweeningTime <= _tweenDuration)
            {
                _ballsContainer[_currentBall - 1].transform.DOMove(_ballPointer.transform.position, _tweenDuration, false);
            }
            else
            {
                _isTweening = false;
                _ballPointer.SetLock(false);
            }
        }
    }
}
