using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Player Event Channel")]
public class PlayerEventChannel : ScriptableObject
{
    public UnityAction<PlayerState> PlayerStateChangedEvent;

    public UnityAction<float, Vector3> PlayerMoveEvent;

    // speed + direction = velocity?
    public void RaisePlayerMoveEvent(float speed, Vector3 direction)
    {
        PlayerMoveEvent?.Invoke(speed, direction);
    }

    public void RaisePlayerStateChangedEvent(PlayerState newPlayerState)
    {
        PlayerStateChangedEvent?.Invoke(newPlayerState);
    }
}
