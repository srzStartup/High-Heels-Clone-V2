using System;

using UnityEngine;

public class HeelsCollideEventArgs : EventArgs
{
    public HeelsCollideType collideType { get; private set; }
    public Collider other { get; private set; }

    public HeelsCollideEventArgs(HeelsCollideType collideType, Collider other)
    {
        this.collideType = collideType;
        this.other = other;
    }
}
