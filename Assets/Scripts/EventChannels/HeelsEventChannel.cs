using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Heels Event Channel")]
public class HeelsEventChannel : ScriptableObject
{
    public UnityAction<List<Stackable>, float> HeelsReadyEvent;

    public UnityAction HeelsCollectedEvent;
    public UnityAction<float> HeelsPushedEvent;
    public UnityAction<float> HeelsPoppedEvent;
    public UnityAction OutOfHeelsEvent;
    public UnityAction<Collider> CollideObstacleEvent;

    public void RaiseHeelsReadyEvent(List<Stackable> heels, float totalHeelSize)
    {
        HeelsReadyEvent?.Invoke(heels, totalHeelSize);
    }

    public void RaiseHeelsCollectedEvent()
    {
        HeelsCollectedEvent?.Invoke();
    }

    public void RaiseHeelsPushedEvent(float heelSize)
    {
        HeelsPushedEvent?.Invoke(heelSize);
    }

    public void RaiseHeelsPoppedEvent(float heelSize)
    {
        HeelsPoppedEvent?.Invoke(heelSize);
    }

    public void RaiseOutOfHeelsEvent()
    {
        OutOfHeelsEvent?.Invoke();
    }

    public void RaiseCollideObstacleEvent(Collider other)
    {
        CollideObstacleEvent?.Invoke(other);
    }
}
