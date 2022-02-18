using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;
using UnityEngine;

public class HeelsManager : Singleton<HeelsManager>
{
    [SerializeField] private Stackable _heelPrefab;
    [SerializeField] private Transform heelLeft;
    [SerializeField] private Transform heelRight;

    [Header("Event Channels")]
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    private int _heelsCount = 0;
    private float _baseHeelSize = 0;
    private Stack<List<Stackable>> _heels;

    public float stackableHeelSize => _heelPrefab.stackedObject.GetComponent<Renderer>().bounds.size.y;
    public float totalHeelSize => _baseHeelSize + stackableHeelSize * _heelsCount;

    public override void Awake()
    {
        base.Awake();

        _heelsEventChannel.HeelsCollectedEvent += OnHeelsCollected;
        _heelsEventChannel.CollideObstacleEvent += OnCollideObstacle;
    }

    private void OnDestroy()
    {
        _heelsEventChannel.HeelsCollectedEvent -= OnHeelsCollected;
        _heelsEventChannel.CollideObstacleEvent -= OnCollideObstacle;
    }

    private void Start()
    {
        List<Stackable> initialHeels = new List<Stackable>()
        {
            heelLeft.parent.GetComponent<Stackable>(),
            heelRight.parent.GetComponent<Stackable>()
        };
        _heels = new Stack<List<Stackable>>();
        _heels.Push(initialHeels);

        _baseHeelSize = initialHeels.First().stackedObject.GetComponent<Renderer>().bounds.size.y;
        _heelsEventChannel.RaiseHeelsReadyEvent(initialHeels, totalHeelSize);
    }

    private void OnCollideObstacle(Collider other)
    {
        ObstacleGroupManager groupHandler = other.transform.parent.GetComponent<ObstacleGroupManager>();

        groupHandler.AllTriggersOff();
        int obstacleLevel = groupHandler.GetObstacleLevel(other.transform);
        if (obstacleLevel == 0) return;

        List<Stackable> lastHeels = _heels.Pop();

        lastHeels.ForEach(heel =>
        {
            Destroy(heel.gameObject);

            GameObject fallingHeels = Instantiate(_heelPrefab.gameObject, transform.position, Quaternion.identity);
            Rigidbody heelsRb = fallingHeels.GetComponent<Stackable>().stackedObject.GetComponent<Rigidbody>();
            heelsRb.constraints = RigidbodyConstraints.None;
        });

        if (_heelsCount < obstacleLevel)
        {
            _heelsCount = -1;
            _heelsEventChannel.RaiseOutOfHeelsEvent();
            return;
        }

        _heelsCount -= obstacleLevel;
        _heelsEventChannel.RaiseHeelsPoppedEvent(totalHeelSize);
    }

    private void OnHeelsCollected()
    {
        List<Stackable> lastHeels = new List<Stackable>();

        _heels.Peek().ForEach(stackableHeelParent =>
        {
            Transform heelParent = stackableHeelParent.transform;
            Transform stackedHeelTransform = Instantiate(_heelPrefab.transform, heelParent.position, heelParent.rotation);

            stackableHeelParent.SetConstrainedObject(stackedHeelTransform);

            lastHeels.Add(stackedHeelTransform.GetComponent<Stackable>());
        });

        _heels.Push(lastHeels);

        _heelsCount++;

        _heelsEventChannel.RaiseHeelsPushedEvent(totalHeelSize);
    }
}
