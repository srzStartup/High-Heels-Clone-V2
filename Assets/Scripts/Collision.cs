using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [SerializeField] private HeelsEventChannel _heelsEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CollectibleItem item))
        {
            other.isTrigger = false;
            _heelsEventChannel.RaiseCollideCollectibleEvent(other, item.ItemType);
        }

        if (other.transform.parent && other.transform.parent.TryGetComponent(out ObstacleGroupManager obstacleGroupManager))
        {
            _heelsEventChannel.RaiseCollideObstacleEvent(other, obstacleGroupManager);
        }
    }
}
