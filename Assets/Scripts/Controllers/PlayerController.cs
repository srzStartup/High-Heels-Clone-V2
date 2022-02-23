using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float forwardSpeedRunning;
    [SerializeField] private float forwardSpeedFalling;
    [SerializeField] private float moveRange;

    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InGameEventChannel _inGameEventChannel;

    private PlayerState playerState;
    private float _currentSpeed;
    private bool enabledOnce = false;

    private void Awake()
    {
        _playerEventChannel.PlayerStateChangedEvent += OnPlayerStateChanged;
    }

    private void OnDestroy()
    {
        _playerEventChannel.PlayerStateChangedEvent -= OnPlayerStateChanged;
    }

    private void Update()
    {
        Movement();
        MoveLateral();
    }

    private void Movement()
    {
        transform.Translate(0, 0, _currentSpeed * Time.deltaTime);
    }

    private void MoveLateral()
    {
        float positionX = transform.position.x + ScreenInputManager.Instance.swipeDelta;

        if (positionX < -moveRange || positionX > moveRange) positionX = transform.position.x;

        transform.position = new Vector3(
            positionX,
            transform.position.y,
            transform.position.z
        );
    }

    public void OnPlayerStateChanged(PlayerState newState)
    {
        Debug.Log(newState);
        switch (newState)
        {
            case PlayerState.StartMoving:

                _currentSpeed = forwardSpeedRunning;
                enabled = true;

                break;

            case PlayerState.Moving:

                _currentSpeed = forwardSpeedRunning;

                break;

            case PlayerState.Grounded:

                _currentSpeed = forwardSpeedRunning;

                break;

            case PlayerState.Falling:

                _currentSpeed = forwardSpeedFalling;

                break;

            case PlayerState.Dead:

                enabled = false;

                break;
        }

        playerState = newState;
    }
}