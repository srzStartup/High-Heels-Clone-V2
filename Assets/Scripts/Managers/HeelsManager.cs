using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    private void Update()
    {
        Debug.Log(_heelsCount);
    }

    private void Start()
    {
        StackInitialHeels();
    }

    private void OnCollideObstacle(Collider other, ObstacleGroupManager obstacleGroupManager)
    {
        obstacleGroupManager.AllTriggersOff();
        int obstacleLevel = obstacleGroupManager.GetObstacleLevel(other.transform);
        if (obstacleLevel == 0) return;

        if (_heelsCount == 0 || _heelsCount < obstacleLevel)
        {
            _heelsCount = -1;
            _heelsEventChannel.RaiseCollideWithNoHeels();
            return;
        }

        for (int i = 1; i <= obstacleLevel; i++)
        {
            List<Stackable> lastHeels = _heels.Pop();

            lastHeels.ForEach(heel => heel.stackedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None);
            _heels.Peek().ForEach(ancestorHeel => ancestorHeel.ReleaseConstrainedObject());
        }

        _heelsCount -= obstacleLevel;
        _heelsEventChannel.RaiseHeelsPoppedEvent(totalHeelSize);

        if (_heelsCount == 0)
        {
            _heelsEventChannel.RaiseNoHeelsEvent();
        }
    }

    private void OnHeelsCollected()
    {
        List<Stackable> lastHeels = new List<Stackable>();

        _heels.Peek().ForEach(stackableHeelParent =>
        {
            Transform heelParent = stackableHeelParent.transform;
            Transform stackedHeelTransform = Instantiate(_heelPrefab.transform, heelParent.position, heelParent.rotation);

            stackableHeelParent.SetConstrainedObject(stackedHeelTransform.GetComponent<Stackable>());

            lastHeels.Add(stackedHeelTransform.GetComponent<Stackable>());
        });

        _heels.Push(lastHeels);

        _heelsCount++;

        _heelsEventChannel.RaiseHeelsPushedEvent(totalHeelSize);

        if (_heelsCount == 1)
        {
            _heelsEventChannel.RaiseExistHeelsEvent();
        }
    }

    private void StackInitialHeels()
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
        _heelsEventChannel.RaiseNoHeelsEvent();
    }
}
