using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : UIWindow
{
    public static event Action<bool> ToggleAudio;
    [SerializeField]
    private Toggle _audioToggle;

    private void OnEnable() => _audioToggle.onValueChanged.AddListener((bool value) => ToggleAudio?.Invoke(!value));

    private void OnDisable() => _audioToggle.onValueChanged.RemoveAllListeners();

}
