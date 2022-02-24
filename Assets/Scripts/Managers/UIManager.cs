using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
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
    [SerializeField] private PlayerEventChannel _playerEventChannel;

    private int _currentLevel;

    private void Awake()
    {
        _inGameEventChannel.GameStartedEvent += OnGameStarted;
        _inGameEventChannel.LevelStartedEvent += OnLevelStarted;
        _inGameEventChannel.LevelFailedEvent += OnLevelFailed;
        _inGameEventChannel.GemCollectedEvent += OnGemCollected;
        _inGameEventChannel.LevelSucceedEvent += OnLevelSucceed;
    }

    private void OnDestroy()
    {
        _inGameEventChannel.GameStartedEvent -= OnGameStarted;
        _inGameEventChannel.LevelStartedEvent -= OnLevelStarted;
        _inGameEventChannel.LevelFailedEvent -= OnLevelFailed;
        _inGameEventChannel.GemCollectedEvent -= OnGemCollected;
        _inGameEventChannel.LevelSucceedEvent -= OnLevelSucceed;
    }

    private void OnGameStarted(int level)
    {
        _currentLevel = level;
        _startScreen_LevelText.text = "LEVEL " + level.ToString();
        // TODO: will be changed
        _startScreen_CollectibleText.text = "0";
    }


    private void OnLevelStarted(int totalGem)
    {
        _inGameScreen_CollectibleText.text = totalGem.ToString();
        _inGameScreen_LevelText.text = "LEVEL " + _currentLevel.ToString();

        _startScreen.SetActive(false);
        _inGameScreen.SetActive(true);
    }

    private void OnLevelSucceed()
    {
        _inGameScreen.SetActive(false);
        _winScreen.SetActive(true);
    }

    private void OnLevelFailed()
    {
        _inGameScreen.SetActive(false);
        _loseScreen.SetActive(true);
    }

    private void OnGemCollected(int totalGem)
    {
        _inGameScreen_CollectibleText.text = totalGem.ToString();
    }
}
