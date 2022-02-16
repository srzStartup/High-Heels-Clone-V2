using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Heels Event Channel")]
public class HeelsEventChannel : ScriptableObject
{
    public UnityAction<List<Transform>> HeelsReadyEvent;

    public UnityAction HeelsCollectedEvent;
    public UnityAction<float> HeelsLengthChangedEvent;
    public UnityAction<int> CollideObstacleEvent;

    public void RaiseHeelsReadyEvent(List<Transform> heels)
    {
        HeelsReadyEvent?.Invoke(heels);
    }

    public void RaiseHeelsCollectedEvent()
    {
        HeelsCollectedEvent?.Invoke();
    }

    public void RaiseHeelsLengthChangedEvent(float total)
    {
        HeelsLengthChangedEvent?.Invoke(total);
    }

    public void RaiseCollideObstacleEvent(int number)
    {
        CollideObstacleEvent?.Invoke(number);
    }
}
