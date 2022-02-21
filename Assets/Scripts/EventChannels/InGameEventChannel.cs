using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/In Game Event Channel")]
public class InGameEventChannel : ScriptableObject
{
    public UnityAction RestartLevelRequestedEvent;
    public UnityAction NextLevelRequestedEvent;

    public UnityAction<int> GameStartedEvent;
    public UnityAction<int> LevelStartedEvent;
    public UnityAction LevelFailedEvent;
    public UnityAction LevelSucceedEvent;

    public UnityAction<int> GemCollectedEvent;

    public void RaiseRestartLevelRequestedEvent()
    {
        RestartLevelRequestedEvent?.Invoke();
    }

    public void RaiseNextLevelRequestedEvent()
    {
        NextLevelRequestedEvent?.Invoke();
    }

    public void RaiseGameStartedEvent(int level)
    {
        GameStartedEvent?.Invoke(level);
    }

    public void RaiseLevelStartedEvent(int currentGem)
    {
        LevelStartedEvent?.Invoke(currentGem);
    }

    public void RaiseLevelFailedEvent()
    {
        LevelFailedEvent?.Invoke();
    }

    public void RaiseLevelSucceedEvent()
    {
        LevelSucceedEvent?.Invoke();
    }

    public void RaiseGemCollectedEvent(int total)
    {
        GemCollectedEvent?.Invoke(total);
    }
}
