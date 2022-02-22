using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event EventHandler<HeelsCollideEventArgs> HeelsCollision;
    public static event EventHandler<HeelsHeightChangedEventArgs> HeelsHeightChanged;

    private void Start()
    {
        Collision.HeelsCollide += HeelsCollision;
        HeelsManager.HeelsHeightChanged += HeelsHeightChanged;
    }
}