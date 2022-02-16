using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;

public class HeelsManager : Singleton<HeelsManager>
{
    [SerializeField] private Transform heelLeft;
    [SerializeField] private Transform heelRight;
    [SerializeField] private float _lengthPerSizing;

    [Header("Event Channels")]
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    public Bounds heelBounds => heelLeft.GetComponent<Renderer>().bounds;

    public override void Awake()
    {
        base.Awake();

        _heelsEventChannel.HeelsReadyEvent += OnHeelsReady;
        _heelsEventChannel.HeelsCollectedEvent += OnHeelsCollected;
        _heelsEventChannel.CollideObstacleEvent += OnCollideObstacle;
    }

    private void OnDestroy()
    {
        _heelsEventChannel.HeelsReadyEvent -= OnHeelsReady;
        _heelsEventChannel.HeelsCollectedEvent -= OnHeelsCollected;
        _heelsEventChannel.CollideObstacleEvent -= OnCollideObstacle;
    }

    private void OnCollideObstacle(int obstacleCount)
    {
        Sequence sequence = DOTween.Sequence();
        ForEachHeel(heel => sequence.Append(heel.parent.DOScaleY(heelBounds.size.y - _lengthPerSizing * obstacleCount, 0)));
        sequence.OnComplete(() =>
        {
            _heelsEventChannel.RaiseHeelsLengthChangedEvent(-_lengthPerSizing * obstacleCount);
        });
    }

    private void OnHeelsCollected()
    {
        Sequence sequence = DOTween.Sequence();
        ForEachHeel(heel => sequence.Append(heel.parent.DOScaleY(heelBounds.size.y + _lengthPerSizing, 0)));
        sequence.OnComplete(() =>
        {
            _heelsEventChannel.RaiseHeelsLengthChangedEvent(_lengthPerSizing);
        });
    }

    private void OnHeelsReady(List<Transform> arg0)
    {
        throw new NotImplementedException();
    }

    private void ForEachHeel(System.Action<Transform> action)
    {
        action(heelLeft);
        action(heelRight);
    }
}
