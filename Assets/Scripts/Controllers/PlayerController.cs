using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _moveRange;

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;

    private void Awake()
    {
        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;

        _playerEventChannel.PlayerDeadEvent += OnPlayerDead;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;

        _playerEventChannel.PlayerDeadEvent -= OnPlayerDead;
    }

    private void Update()
    {
        Run();
        MoveLateral();
    }

    private void Run()
    {
        transform.Translate(0, 0, _forwardSpeed * Time.deltaTime);
        _playerEventChannel.RaisePlayerMoveEvent(_forwardSpeed, Vector3.forward);
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

    private void OnLevelStarted(int initialGem)
    {
        enabled = true;
    }

    private void OnPlayerDead()
    {
        enabled = false;
    }
}