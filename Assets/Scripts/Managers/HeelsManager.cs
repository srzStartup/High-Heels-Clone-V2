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
    [SerializeField] private float _lengthPerSizing;

    [Header("Event Channels")]
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    private int _heelsCollected = 1;
    private float _heelSize;
    private Stack<List<Stackable>> _heels;

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

        _heelSize = heelLeft.GetComponent<Renderer>().bounds.size.y;
        _heelsEventChannel.RaiseHeelsReadyEvent(_heels.Peek().ConvertAll(heel => heel.transform));
    }

    private void OnCollideObstacle(Collider other, int count)
    {
        other.transform.parent
            .GetComponentsInChildren<Transform>()
            .ToList()
            .FindAll(child => !child.Equals(other.transform.parent))
            .ForEach(child => child.GetComponent<Collider>().isTrigger = false);

        List<Stackable> lastHeels = _heels.Pop();

        lastHeels.ForEach(heel =>
        {
            Destroy(heel.gameObject);

            GameObject fallingHeels = Instantiate(_heelPrefab.gameObject, transform.position, Quaternion.identity);
            Rigidbody heelsRb = fallingHeels.transform.Find("BaseHeel").GetComponent<Rigidbody>();
            heelsRb.constraints = RigidbodyConstraints.None;
        });

        FindBelow(other);

        _heelsEventChannel.RaiseHeelsPoppedEvent(1);
    }

    private void OnHeelsCollected()
    {
        List<Stackable> lastHeels = new List<Stackable>();

        _heels.Peek().ForEach(stackableHeel =>
        {
            Transform heel = stackableHeel.transform;
            Transform stackedHeelTransform = Instantiate(_heelPrefab.transform, heel.position, heel.rotation);

            stackableHeel.SetConstrainedObject(stackedHeelTransform);

            lastHeels.Add(stackedHeelTransform.GetComponent<Stackable>());
        });

        _heels.Push(lastHeels);

        _heelsEventChannel.RaiseHeelsCountChangedEvent(++_heelsCollected, _heelSize);
    }

    private List<Transform> FindBelow(Collider other)
    {
        Debug.Log(other.name);
        Vector3 center = other.transform.position;
        float radius = transform.position.y;

        Collider[] colliders = Physics.OverlapSphere(center, radius);

        return colliders.ToList()
            .FindAll(collider =>
            {
                return collider.transform.position.x == other.transform.position.x &&
                       collider.transform.position.z == other.transform.position.z &&
                       collider.transform.position.y < other.transform.position.y;
            })
            .ConvertAll(collider => collider.transform);
    }
}
