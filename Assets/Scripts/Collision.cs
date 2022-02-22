using System;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [SerializeField] private string _stackableHeelsTag = "heels";
    [SerializeField] private string _obstacleTag = "obstacle";

    public static event EventHandler<HeelsCollideEventArgs> HeelsCollide;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_stackableHeelsTag))
        {
            HeelsCollide?.Invoke(
                this,
                new HeelsCollideEventArgs(HeelsCollideType.HEELS, other)
            );
        }
        else if (other.gameObject.CompareTag(_obstacleTag))
        {
            HeelsCollide?.Invoke(
                this,
                new HeelsCollideEventArgs(HeelsCollideType.OBSTACLE, other)
            );
        }
    }
}
