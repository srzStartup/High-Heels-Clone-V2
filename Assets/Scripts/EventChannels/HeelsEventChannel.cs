using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Heels Event Channel")]
public class HeelsEventChannel : ScriptableObject
{
    public UnityAction<List<Transform>> HeelsReadyEvent;

    public UnityAction HeelsCollectedEvent;
    public UnityAction<int, float> HeelsCountChangedEvent;
    public UnityAction<float> HeelsPushedEvent;
    public UnityAction<int> HeelsPoppedEvent;
    public UnityAction<Collider, int> CollideObstacleEvent;

    public void RaiseHeelsReadyEvent(List<Transform> heels)
    {
        HeelsReadyEvent?.Invoke(heels);
    }

    public void RaiseHeelsCollectedEvent()
    {
        HeelsCollectedEvent?.Invoke();
    }

    public void RaiseHeelsPushedEvent(float heelSize)
    {
        HeelsPushedEvent?.Invoke(heelSize);
    }

    public void RaiseHeelsCountChangedEvent(int currentCount, float length)
    {
        HeelsCountChangedEvent?.Invoke(currentCount, length);
    }

    public void RaiseHeelsPoppedEvent(int count)
    {
        HeelsPoppedEvent?.Invoke(count);
    }

    public void RaiseCollideObstacleEvent(Collider other, int count)
    {
        CollideObstacleEvent?.Invoke(other, count);
    }
}
