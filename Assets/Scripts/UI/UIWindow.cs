using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    private GameObject _window;
    protected virtual void Awake() => _window = transform.GetChild(0).gameObject;

    public void Enable() => _window.SetActive(true);
    public void Disable() => _window.SetActive(false);
}