using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    private float _gravity = -9.8f;

    private Animator _animator;
    private CharacterController _charController;

    private void Awake()
    {
        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;

        _heelsEventChannel.HeelsLengthChangedEvent += OnHeelsLengthChanged;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;

        _heelsEventChannel.HeelsLengthChangedEvent -= OnHeelsLengthChanged;
    }

    private void OnHeelsLengthChanged(float diff)
    {
        Debug.Log(_charController.isGrounded);

        _charController.skinWidth += diff;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleGravity();
        Run();
        MoveLateral();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.transform.TryGetComponent(out CollectibleItem item))
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

            hit.collider.gameObject.SetActive(false);
        }
        else if (hit.collider.gameObject.name.StartsWith("Obstacle"))
        {
            _heelsEventChannel.RaiseCollideObstacleEvent(1);
        }
    }

    private void Run()
    {
        _animator.SetFloat("_speed", 1.0f);

        _charController.Move(new Vector3(0, _gravity, _forwardSpeed * Time.deltaTime));
    }

    private void MoveLateral()
    {
        _charController.Move(new Vector3(ScreenInputManager.Instance.swipeDelta, _gravity, 0));
    }

    private void HandleGravity()
    {
        if (!_charController.isGrounded)
        {
            _gravity = -9.8f;
            return;
        }

        _gravity = -.5f;
    }

    private void OnLevelStarted(int totalGem)
    {
        enabled = true;
        _gemCollected = totalGem;
    }

    private Collider FindBelow(Collider other)
    {
        Collider below = Physics.OverlapSphere(other.transform.position, .5f)
                            .ToList()
                            .Find(collider => collider.transform.position.y < other.transform.position.y);

        if (below == null)
        {
            // throwing custom not found exception would be better
            throw new SystemException();
        }

        return below;
    }
}
