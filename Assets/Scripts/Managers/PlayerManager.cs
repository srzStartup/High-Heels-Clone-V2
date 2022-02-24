using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _platform;
    [SerializeField] private Transform _ragdollRoot;
    [SerializeField] private Transform[] _ignoreRagdollColliders;

    [Header("Leg Openers")]
    [SerializeField] private Rig[] _heelRigs;
    [SerializeField] private Rig[] _legOpenerRigs;

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    private List<Collider> _ragdollParts;

    private PlayerState playerState;
    private int _gemCollected;
    private bool _colliderOn = false;

    public override void Awake()
    {
        SetRagdollParts();

        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;

        _heelsEventChannel.CollideWithNoHeels += OnCollideWithNoHeels;
        _heelsEventChannel.HeelsReadyEvent += OnHeelsReady;
        _heelsEventChannel.HeelsPushedEvent += OnHeelsPushed;
        _heelsEventChannel.HeelsPoppedEvent += OnHeelsPopped;
        _heelsEventChannel.CollideCollectibleEvent += OnCollideCollectible;
        _heelsEventChannel.HeelsGroundedEvent += OnHeelsGrounded;
        _heelsEventChannel.NoHeelsEvent += OnNoHeels;
        _heelsEventChannel.ExistHeelsEvent += OnExistHeels;
        _heelsEventChannel.BalkTriggerEnterEvent += OnBalkTrigger;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;

        _heelsEventChannel.CollideWithNoHeels -= OnCollideWithNoHeels;
        _heelsEventChannel.HeelsReadyEvent -= OnHeelsReady;
        _heelsEventChannel.HeelsPushedEvent -= OnHeelsPushed;
        _heelsEventChannel.HeelsPoppedEvent -= OnHeelsPopped;
        _heelsEventChannel.CollideCollectibleEvent -= OnCollideCollectible;
        _heelsEventChannel.HeelsGroundedEvent -= OnHeelsGrounded;
        _heelsEventChannel.NoHeelsEvent -= OnNoHeels;
        _heelsEventChannel.ExistHeelsEvent -= OnExistHeels;
        _heelsEventChannel.BalkTriggerEnterEvent -= OnBalkTrigger;
    }

    private void Start()
    {
        _player = _player != null ? _player : transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.ToLower().Contains("platform") && playerState == PlayerState.Falling)
        {
            GetComponent<Animator>().SetTrigger("_walk");
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            playerState = PlayerState.Grounded;
            _playerEventChannel.RaisePlayerStateChangedEvent(playerState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_colliderOn) return;

        if (other.gameObject.TryGetComponent(out CollectibleItem item))
        {
            other.isTrigger = false;
            _heelsEventChannel.RaiseCollideCollectibleEvent(other, item.ItemType);
        }

        if (other.transform.parent && other.transform.parent.TryGetComponent(out ObstacleGroupManager obstacleGroupManager))
        {
            _heelsEventChannel.RaiseCollideObstacleEvent(other, obstacleGroupManager);
        }

        if (other.gameObject.TryGetComponent(out BalkTrigger balkTrigger))
        {
            _heelsEventChannel.RaiseBalkTriggerEnterEvent(other, balkTrigger);
        }
    }

    private void OnLevelStarted(int totalGem)
    {
        _gemCollected = totalGem;
        GetComponent<Animator>().SetTrigger("_walk");
        playerState = PlayerState.StartMoving;
        _playerEventChannel.RaisePlayerStateChangedEvent(playerState);
    }

    private void OnHeelsReady(List<Stackable> initialHeels, float totalHeelSize)
    {
        DisableRagdoll();

        transform.position = new Vector3(
            transform.position.x,
            _platform.position.y + totalHeelSize,
            transform.position.z
        );
    }

    private void OnHeelsPushed(float totalHeelSize)
    {
        transform.position = new Vector3(
            transform.position.x,
            _platform.position.y + totalHeelSize,
            transform.position.z
        );
    }

    private void OnHeelsPopped(float totalHeelSize)
    {
        playerState = PlayerState.Falling;
        GetComponent<Animator>().SetTrigger("_walk");
        _playerEventChannel.RaisePlayerStateChangedEvent(playerState);

        transform.DOMoveZ(transform.position.z + .5f, .1f)
            .OnComplete(() => 
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            });
    }

    private void OnCollideWithNoHeels()
    {
        transform.GetComponent<Animator>()
            .enabled = false;

        Collider collider = transform.GetComponent<Collider>();
        collider.isTrigger = true;
        collider.attachedRigidbody.velocity = Vector3.zero;
        collider.attachedRigidbody.constraints = RigidbodyConstraints.None;

        EnableRagdoll();

        playerState = PlayerState.Dead;
        _playerEventChannel.RaisePlayerStateChangedEvent(playerState);
    }

    private void OnCollideCollectible(Collider other, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Heels:

                other.transform.DOScale(.001f, .5f)
                    .OnComplete(() => other.gameObject.SetActive(false));

                _heelsEventChannel.RaiseHeelsCollectedEvent();

                break;
            case ItemType.Gem:

                _inGameEventChannel.RaiseGemCollectedEvent(++_gemCollected);

                break;
        }
    }

    public void OnHeelsGrounded()
    {
        GetComponent<Animator>().SetTrigger("_walk");
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        playerState = PlayerState.Grounded;
        _playerEventChannel.RaisePlayerStateChangedEvent(playerState);
    }

    private void OnExistHeels()
    {
        _colliderOn = false;
    }

    private void OnNoHeels()
    {
        _colliderOn = true;
    }

    private void OnBalkTrigger(Collider other, BalkTrigger balkTrigger)
    {
        other.isTrigger = false;

        foreach (Rig heelRig in _heelRigs)
        {
            heelRig.weight = 0;
        }

        foreach (Rig legOpenerRig in _legOpenerRigs)
        {
            legOpenerRig.weight = 1;
        }

        GetComponent<Animator>().SetTrigger("_clap");

        playerState = PlayerState.Stretching;
        _playerEventChannel.RaisePlayerStateChangedEvent(playerState);
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
                collider.attachedRigidbody.isKinematic = true;
            });
    }

    private void EnableRagdoll()
    {
        _ragdollParts.ForEach(collider =>
            {
                collider.isTrigger = false;
                collider.attachedRigidbody.isKinematic = false;
            });
    }
}

public enum PlayerState
{
    StartMoving,
    Moving,
    Falling,
    Grounded,
    Dead,
    Stretching
}
