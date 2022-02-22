using System;

public class HeelsHeightChangedEventArgs : EventArgs
{
    public HeelsHeightChangeType changeType { get; }
    public float diff { get; }

    public HeelsHeightChangedEventArgs(HeelsHeightChangeType changeType, float diff)
    {
        this.changeType = changeType;
        this.diff = diff;
    }
}