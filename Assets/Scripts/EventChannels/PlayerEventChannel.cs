using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Player Event Channel")]
public class PlayerEventChannel : ScriptableObject
{
    public UnityAction PlayerMovedEvent;

    public void RaisePlayerMovedEvent()
    {
        PlayerMovedEvent?.Invoke();
    }
}
