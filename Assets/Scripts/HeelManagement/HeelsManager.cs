using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class HeelsManager : MonoBehaviour
{
    #region Events
    public static event EventHandler<HeelsHeightChangedEventArgs> HeelsHeightChanged;
    #endregion

    public List<ConstraintedHeel> heels { get; set; }
    public Transform ground { get; set; }

    public float lengthPerSizing { get; set; }
    private const float DefaultLengthPerSizing = .1f;

    public Bounds bounds => heels.First().transform.GetComponent<Renderer>().bounds;

    #region MonoBehaviour Methods
    private void Awake()
    {
        EventManager.HeelsCollision += OnHeelsCollide;
    }

    private void Start()
    {
        heels.ForEach((heel) => heel.positionStrategy.SetPivot(y: -bounds.extents.y));
    }

    #endregion

    #region Event Listeners

    private void OnHeelsCollide(object sender, HeelsCollideEventArgs heelsCollision)
    {
        switch (heelsCollision.collideType)
        {
            case HeelsCollideType.HEELS:
                SetHeelsHeight(lengthPerSizing);
                HeelsHeightChanged?.Invoke(this,
                    new HeelsHeightChangedEventArgs(HeelsHeightChangeType.Increase,
                        bounds.size.y));
                break;
            case HeelsCollideType.OBSTACLE:
                SetHeelsHeight(-lengthPerSizing);
                HeelsHeightChanged?.Invoke(this,
                    new HeelsHeightChangedEventArgs(HeelsHeightChangeType.Decrease,
                        bounds.size.y));
                break;
        }
    }

    #endregion

    private void SetHeelsHeight(float length, int times = 1)
    {
        heels.ForEach((heel) =>
        {
            float stableLength = length.Equals(0) ? DefaultLengthPerSizing : length;
            stableLength *= times;
            float endValue = heel.transform.localScale.y + stableLength;

            float pivot = endValue > heel.transform.localScale.y ? -bounds.extents.y : bounds.extents.y;
            heel.transform.localScale = new Vector3(heel.transform.localScale.x, endValue, heel.transform.localScale.z);
            heel.positionStrategy.SetPivot(y: pivot);
        });
    }
}
