using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private InGameEventChannel _inGameEventChannel;
    [SerializeField] private int _level;

    public bool IsStarted { get; private set; }
    public bool IsFailed { get; private set; }
    public bool IsLevelStarted { get; private set; }
    public int CurrentLevel => _level;

    private int _collectibleCount;

    private float _swipeDeadZone = .1f;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        IsFailed = false;
        IsStarted = true;
        IsLevelStarted = false;

        _inGameEventChannel.RaiseGameStartedEvent(_level);
    }

    private void Update()
    {
        if (!IsLevelStarted && Mathf.Abs(ScreenInputManager.Instance.swipeDelta) >= _swipeDeadZone)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        IsLevelStarted = true;
        _inGameEventChannel.RaiseLevelStartedEvent(_collectibleCount);
    }

    public void EndLevel()
    {
        IsFailed = true;
        _inGameEventChannel.RaiseLevelFailedEvent();
    }
}