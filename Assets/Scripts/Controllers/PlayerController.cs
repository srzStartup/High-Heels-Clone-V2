using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _moveRange;

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;
    [SerializeField] private HeelsEventChannel _heelsEventChannel;

    private int _gemCollected = 0;
    private int _currentHeelCount;
    private float _heelLength;

    private Animator _animator;

    private void Awake()
    {
        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;

        _heelsEventChannel.HeelsReadyEvent += OnHeelsReady;
        _heelsEventChannel.HeelsCountChangedEvent += OnHeelsCountChanged;
        _heelsEventChannel.HeelsPoppedEvent += OnHeelsPopped;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;

        _heelsEventChannel.HeelsReadyEvent -= OnHeelsReady;
        _heelsEventChannel.HeelsCountChangedEvent -= OnHeelsCountChanged;
        _heelsEventChannel.HeelsPoppedEvent -= OnHeelsPopped;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Run();
        MoveLateral();
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
        else if (other.gameObject.name.Contains("Obstacle"))
        {
            _heelsEventChannel.RaiseCollideObstacleEvent(other, 1);
        }
    }

    private void Run()
    {
        _animator.SetFloat("_speed", 1.0f);

        transform.Translate(0, 0, _forwardSpeed * Time.deltaTime);
    }

    private void MoveLateral()
    {
        float positionX = transform.position.x + ScreenInputManager.Instance.swipeDelta;

        if (positionX < -_moveRange || positionX > _moveRange) positionX = transform.position.x;

        transform.position = new Vector3(
            positionX,
            transform.position.y,
            transform.position.z
        );
    }

    private void OnLevelStarted(int totalGem)
    {
        enabled = true;
        _gemCollected = totalGem;
    }

    private void OnHeelsReady(List<Transform> readyHeels)
    {
        _heelLength = readyHeels[0].Find("BaseHeel").GetComponent<Renderer>().bounds.size.y;
    }

    private void OnHeelsCountChanged(int heelCount, float heelLength)
    {
        _currentHeelCount = heelCount;
        transform.position = new Vector3(
            transform.position.x,
            heelCount * heelLength,
            transform.position.z
        );
    }

    private void OnHeelsPopped(int count)
    {
        _currentHeelCount -= count;

        transform.DOMove(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z + 2), .1f)
            .OnComplete(() => transform.DOJump(new Vector3(transform.position.x, _currentHeelCount * _heelLength, transform.position.z + 2), 1, 1, .5f));
    }
}
