using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Heels Event Channel")]
public class HeelsEventChannel : ScriptableObject
{
    public UnityAction<List<Stackable>, float> HeelsReadyEvent;
    public UnityAction NoHeelsEvent;
    public UnityAction ExistHeelsEvent;

    public UnityAction HeelsCollectedEvent;
    public UnityAction<float> HeelsPushedEvent;
    public UnityAction<float> HeelsPoppedEvent;

    public UnityAction CollideWithNoHeels;
    public UnityAction<Collider, ObstacleGroupManager> CollideObstacleEvent;
    public UnityAction<Collider, ItemType> CollideCollectibleEvent;

    public void RaiseHeelsReadyEvent(List<Stackable> heels, float totalHeelSize)
    {
        HeelsReadyEvent?.Invoke(heels, totalHeelSize);
    }

    public void RaiseNoHeelsEvent()
    {
        NoHeelsEvent?.Invoke();
    }

    public void RaiseExistHeelsEvent()
    {
        ExistHeelsEvent?.Invoke();
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

    public void RaiseCollideWithNoHeels()
    {
        CollideWithNoHeels?.Invoke();
    }

    public void RaiseCollideObstacleEvent(Collider other, ObstacleGroupManager obstacleGroupManager)
    {
        CollideObstacleEvent?.Invoke(other, obstacleGroupManager);
    }

    public void RaiseCollideCollectibleEvent(Collider other, ItemType itemType)
    {
        CollideCollectibleEvent?.Invoke(other, itemType);
    }
}
