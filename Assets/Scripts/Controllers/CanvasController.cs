using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject _inGameScreen;
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _winScreen;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _startScreen_LevelText;
    [SerializeField] private TextMeshProUGUI _startScreen_CollectibleText;
    [SerializeField] private TextMeshProUGUI _inGameScreen_LevelText;
    [SerializeField] private TextMeshProUGUI _inGameScreen_CollectibleText;

    [Header("Event Channels")]
    [SerializeField] private InGameEventChannel _inGameEventChannel;

    private int _currentLevel;

    private void Awake()
    {
        _inGameEventChannel.GameStartedEvent += OnGameStarted;
        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;
        _inGameEventChannel.LevelFailedEvent += OnLevelFailed;
        _inGameEventChannel.LevelSucceedEvent += OnLevelSucceed;

        _inGameEventChannel.GemCollectedEvent += OnGemCollected;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.GameStartedEvent -= OnGameStarted;
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;
        _inGameEventChannel.LevelFailedEvent -= OnLevelFailed;
        _inGameEventChannel.LevelSucceedEvent -= OnLevelSucceed;

        _inGameEventChannel.GemCollectedEvent -= OnGemCollected;
    }

    private void Start()
    {

    }

    private void OnRetryButtonClicked()
    {
        _inGameEventChannel.RaiseRestartLevelRequestedEvent();
    }

    private void OnNextButtonClicked()
    {

    }

    private void OnGameStarted(int level)
    {
        _currentLevel = level;
        _startScreen_LevelText.text = "LEVEL " + level.ToString();
        _startScreen_CollectibleText.text = "0";
    }

    private void OnLevelStarted(int totalGem)
    {
        _inGameScreen_CollectibleText.text = totalGem.ToString();
        _inGameScreen_LevelText.text = "LEVEL " + _currentLevel.ToString();

        _startScreen.SetActive(false);
        _inGameScreen.SetActive(true);
    }

    private void OnLevelFailed()
    {
        _inGameScreen.SetActive(false);
        _loseScreen.SetActive(true);
    }

    private void OnLevelSucceed()
    {
        _inGameScreen.SetActive(false);
        _winScreen.SetActive(true);
    }

    private void OnGemCollected(int totalGem)
    {
        _inGameScreen_CollectibleText.text = totalGem.ToString();
    }
}
