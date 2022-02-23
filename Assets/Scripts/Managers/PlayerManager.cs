using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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

    public Transform leftHeelRig;
    public Transform rightHeelRig;

    private List<Collider> _ragdollParts;

    private PlayerState playerState;
    private int _gemCollected;
    private bool _colliderOn = false;

    private Bounds _initialColliderBounds;

    public override void Awake()
    {
        SetRagdollParts();

        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;

        _heelsEventChannel.CollideWithNoHeels += OnCollideWithNoHeels;
        _heelsEventChannel.HeelsReadyEvent += OnHeelsReady;
        _heelsEventChannel.HeelsPushedEvent += OnHeelsPushed;
        _heelsEventChannel.HeelsPoppedEvent += OnHeelsPopped;
        _heelsEventChannel.CollideCollectibleEvent += OnCollideCollectible;
        _heelsEventChannel.NoHeelsEvent += OnNoHeels;
        _heelsEventChannel.ExistHeelsEvent += OnExistHeels;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;

        _heelsEventChannel.CollideWithNoHeels -= OnCollideWithNoHeels;
        _heelsEventChannel.HeelsReadyEvent -= OnHeelsReady;
        _heelsEventChannel.HeelsPushedEvent -= OnHeelsPushed;
        _heelsEventChannel.HeelsPoppedEvent -= OnHeelsPopped;
        _heelsEventChannel.CollideCollectibleEvent -= OnCollideCollectible;
        _heelsEventChannel.NoHeelsEvent -= OnNoHeels;
        _heelsEventChannel.ExistHeelsEvent -= OnExistHeels;
    }

    private void Start()
    {
        _player = _player != null ? _player : transform;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
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
            totalHeelSize,
            transform.position.z
        );

        _initialColliderBounds = transform.GetComponent<BoxCollider>().bounds;
        //HandleCollider(totalHeelSize);

        //_isGrounded = true;
    }

    private void OnHeelsPushed(float totalHeelSize)
    {
        transform.position = new Vector3(
            transform.position.x,
            totalHeelSize,
            transform.position.z
        );

        //HandleCollider(totalHeelSize);
    }

    private void OnHeelsPopped(float totalHeelSize)
    {
        playerState = PlayerState.Falling;
        _playerEventChannel.RaisePlayerStateChangedEvent(playerState);
        GetComponent<Animator>().SetTrigger("_idle");
        _playerEventChannel.RaisePlayerIdleEvent();

        transform.DOMoveZ(transform.position.z + .5f, .1f)
            .OnComplete(() => GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation);

        //HandleCollider(totalHeelSize);
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

    //private void HandleCollider(float totalHeelSize)
    //{
    //    BoxCollider collider = GetComponent<BoxCollider>();

    //    float updatedSize = totalHeelSize / collider.transform.localScale.y;
    //    float initialColliderSizeLocalY = _initialColliderBounds.size.y / collider.transform.localScale.y;

    //    collider.size = new Vector3(
    //        collider.size.x,
    //        initialColliderSizeLocalY + updatedSize,
    //        collider.size.z
    //    );

    //    collider.center = new Vector3(
    //        collider.center.x,
    //        // WHY???????
    //        collider.center.y - (updatedSize / 4),
    //        collider.center.z
    //    );
    //}

    private void OnExistHeels()
    {
        _colliderOn = false;
    }

    private void OnNoHeels()
    {
        _colliderOn = true;
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
    Dead
}
