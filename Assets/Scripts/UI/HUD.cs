using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Level;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : UIWindow
{
    public static event Action PerformUndo;
    public static event Action PerformPause;
    public static event Action PerformRestart;
    public static event Action PerformSkip;

    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _undoButton;
    [SerializeField]
    private Button _pauseButton;
    [SerializeField]
    private Button _skipButton;
    [SerializeField]
    private TextMeshProUGUI _levelIndexText;

    private void OnEnable()
    {
        _undoButton.onClick.AddListener(() => PerformUndo?.Invoke());
        _undoButton.onClick.AddListener(() => SoundManager.Play(Constants.BUTTON));
        _restartButton.onClick.AddListener(() => PerformRestart?.Invoke());
        _restartButton.onClick.AddListener(() => SoundManager.Play(Constants.BUTTON));
        _pauseButton.onClick.AddListener(() => PerformPause?.Invoke());
        _pauseButton.onClick.AddListener(() => SoundManager.Play(Constants.BUTTON));
        _skipButton.onClick.AddListener(() => PerformSkip?.Invoke());
        _skipButton.onClick.AddListener(() => SoundManager.Play(Constants.BUTTON));
        LevelLoader.LevelLoaded += UpdateLevelIndex;
    }

    private void OnDisable()
    {
        _undoButton.onClick.RemoveAllListeners();
        _restartButton.onClick.RemoveAllListeners();
        _pauseButton.onClick.RemoveAllListeners();
        _skipButton.onClick.RemoveAllListeners();
        LevelLoader.LevelLoaded -= UpdateLevelIndex;
    }

    private void UpdateLevelIndex(LevelData levelLoaded)
    {
        Debug.Log(levelLoaded.Index);
        _levelIndexText.text = $"Level {levelLoaded.Index + 1}";
    }
}