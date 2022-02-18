using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("It's okey to leave as none if the manager is player's compononet.")]
    [SerializeField] private Transform _player;

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    private int _gemCollected;

    // Collider handling (temporary)
    private float _initalColliderSizeY;

    public override void Awake()
    {
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

        _initalColliderSizeY = GetComponent<BoxCollider>().size.y;
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
        transform.position = new Vector3(
            transform.position.x,
            totalHeelSize,
            transform.position.z
        );

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
        GetComponent<Animator>()
            .SetTrigger("_failed");
    }

    private void HandleCollider(float totalHeelSize)
    {
        BoxCollider playerCollider = GetComponent<BoxCollider>();
        playerCollider.size = new Vector3(
            playerCollider.size.x,
            _initalColliderSizeY + totalHeelSize / transform.localScale.y,
            playerCollider.size.z
        );

        playerCollider.center = new Vector3(
            playerCollider.center.x,
            playerCollider.center.y - totalHeelSize / transform.localScale.y / 2,
            playerCollider.center.z
        );
    }
}
