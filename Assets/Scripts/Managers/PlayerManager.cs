using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("It's okey to leave as none if the manager is player's compononet.")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _ragdollRoot;
    [SerializeField] private Transform[] _ignoreRagdollColliders;

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    private List<Collider> _ragdollParts;

    private int _gemCollected;

    private Bounds _initialColliderBounds;

    public override void Awake()
    {
        SetRagdollParts();

        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;

        _playerEventChannel.PlayerMoveEvent += OnPlayerMove;

        _heelsEventChannel.OutOfHeelsEvent += OnOutOfHeels;
        _heelsEventChannel.HeelsReadyEvent += OnHeelsReady;
        _heelsEventChannel.HeelsPushedEvent += OnHeelsPushed;
        _heelsEventChannel.HeelsPoppedEvent += OnHeelsPopped;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;

        _playerEventChannel.PlayerMoveEvent -= OnPlayerMove;

        _heelsEventChannel.OutOfHeelsEvent -= OnOutOfHeels;
        _heelsEventChannel.HeelsReadyEvent -= OnHeelsReady;
        _heelsEventChannel.HeelsPushedEvent -= OnHeelsPushed;
        _heelsEventChannel.HeelsPoppedEvent -= OnHeelsPopped;
    }

    private void Start()
    {
        _player = _player != null ? _player : transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out CollectibleItem item))
        {
            switch (item.ItemType)
            {
                case ItemType.Heels:
                    _heelsEventChannel.RaiseHeelsCollectedEvent();
                    break;
                case ItemType.Gem:
                    _inGameEventChannel.RaiseGemCollectedEvent(_gemCollected++);
                    break;
            }

            other.gameObject.SetActive(false);
        }
        else if (other.transform.parent.TryGetComponent(out ObstacleGroupManager obstacleGroupManager))
        {
            _heelsEventChannel.RaiseCollideObstacleEvent(other);
        }
    }

    private void OnLevelStarted(int totalGem)
    {
        _gemCollected = totalGem;
    }

    private void OnHeelsReady(List<Stackable> initialHeels, float totalHeelSize)
    {
        DisableRagdoll();

        transform.position = new Vector3(
            transform.position.x,
            totalHeelSize,
            transform.position.z
        );
        _initialColliderBounds = transform.GetComponent<BoxCollider>().bounds;
        HandleCollider(totalHeelSize);
    }

    private void OnPlayerMove(float speed, Vector3 direction)
    {
        GetComponent<Animator>()
            .SetTrigger("_walk");
    }

    private void OnHeelsPushed(float totalHeelSize)
    {
        transform.position = new Vector3(
            transform.position.x,
            totalHeelSize,
            transform.position.z
        );

        HandleCollider(totalHeelSize);
    }

    private void OnHeelsPopped(float totalHeelSize)
    {
        transform.DOMove(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z + 2), .1f)
            .OnComplete(() => transform.DOJump(new Vector3(transform.position.x, totalHeelSize, transform.position.z + 2), 1, 1, .5f));

        HandleCollider(totalHeelSize);
    }

    private void OnOutOfHeels()
    {
        transform.GetComponent<Rigidbody>()
            .constraints = RigidbodyConstraints.None;

        transform.GetComponent<Animator>()
            .enabled = false;

        EnableRagdoll();

        _playerEventChannel.RaisePlayerDeadEvent();
    }

    private void HandleCollider(float totalHeelSize)
    {
        BoxCollider collider = GetComponent<BoxCollider>();

        float updatedSize = totalHeelSize / collider.transform.localScale.y;
        float initialColliderSizeLocalY = _initialColliderBounds.size.y / collider.transform.localScale.y;

        collider.size = new Vector3(
            collider.size.x,
            initialColliderSizeLocalY + updatedSize,
            collider.size.z
        );

        collider.center = new Vector3(
            collider.center.x,
            // WHY???????
            collider.center.y - (updatedSize / 4),
            collider.center.z
        );
    }

    private void SetRagdollParts()
    {
        _ragdollParts = _ragdollRoot.GetComponentsInChildren<Transform>()
            .ToList()
            .FindAll(ragdollPart => ragdollPart.GetComponent<Collider>() && !_ignoreRagdollColliders.Contains(ragdollPart))
            .ConvertAll(ragdollPart => ragdollPart.GetComponent<Collider>());
    }

    private void DisableRagdoll()
    {
        _ragdollParts.ForEach(collider =>
            {
                collider.isTrigger = true;
            });
    }

    private void EnableRagdoll()
    {
        _ragdollParts.ForEach(collider =>
            {
                collider.isTrigger = false;
            });
    }
}
