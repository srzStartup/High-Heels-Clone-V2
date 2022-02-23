using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HeelsCollisionController : MonoBehaviour
{
    [SerializeField] private HeelsEventChannel _heelsEventChannel;
    [SerializeField] private PlayerEventChannel _playerEventChannel;

    private PlayerState playerState;

    private void Awake()
    {
        _playerEventChannel.PlayerStateChangedEvent += OnPlayerStateChanged;
    }

    private void OnDestroy()
    {
        _playerEventChannel.PlayerStateChangedEvent -= OnPlayerStateChanged;
    }

    private void OnPlayerStateChanged(PlayerState newState)
    {
        playerState = newState;
    }

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

        if (other.gameObject.TryGetComponent(out BalkTrigger balkTrigger))
        {
            _heelsEventChannel.RaiseBalkTriggerEnterEvent(other, balkTrigger);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.ToLower().Contains("platform") &&
            playerState == PlayerState.Falling &&
            !transform.parent.GetComponent<Stackable>().isReleased)
        {
            _heelsEventChannel.RaiseHeelsGroundedEvent();
        }
    }
}
