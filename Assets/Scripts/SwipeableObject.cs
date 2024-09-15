using System;
using Data;
using Level;
using Managers;
using Managers.GameStates;
using UnityEngine;

/// <summary>
/// Represents a stackable object in the game, managing interactions and state changes triggered by swipes.
/// </summary>
public class SwipeableObject : MonoBehaviour
{
    public static event Action<string, SwipeableObject, SwipeableObject> RunAnimation;
    public static event Action GameWon;

    public SwipeableObjectData Data;

    private static int _lastSkin = 0;
    private int _slicesNumber;

    private void OnEnable()
    {
        // Subscribe to events
        SwipesManager.TriggerStackMovement += TryMoveStack;
        LevelLoader.LevelLoaded += SetSlicesNumber;
        WinState.LoadNextLevel += DestroyItself;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        SwipesManager.TriggerStackMovement -= TryMoveStack;
        LevelLoader.LevelLoaded -= SetSlicesNumber;
        WinState.LoadNextLevel -= DestroyItself;
    }

    private void OnDestroy() => Resources.UnloadUnusedAssets();

    /// <summary>
    /// Initializes the swipeable object, setting its data and visual appearance.
    /// </summary>
    public void InitializeObject()
    {
        Data.This = this;
        Data.Stack = transform.GetChild(0).gameObject;
        Data.StackCount = 1;
        SetSkin();
    }

    /// <summary>
    /// Sets a random visual skin for the object, avoiding repetition of the last skin used.
    /// </summary>
    private void SetSkin()
    {
        if (Data.Edge) return;
        Data.Stack.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Mat_{GetRandomSkin()}");
    }

    /// <summary>
    /// Gets a random skin index, ensuring it's different from the last one.
    /// </summary>
    private int GetRandomSkin()
    {
        int currentSkin = UnityEngine.Random.Range(1, 6);
        if (_lastSkin == currentSkin)
            return GetRandomSkin();
        else
        {
            _lastSkin = currentSkin;
            return currentSkin;
        }
    }

    /// <summary>
    /// Attempts to move the stack from this object to another based on swipe input.
    /// </summary>
    private void TryMoveStack(SwipeableObject from, SwipeableObject to)
    {
        if (from != Data.This) return;

        if (from.Data.Edge && to.Data.Edge)
        {
            if (from.Data.StackCount + to.Data.StackCount != _slicesNumber)
            {
                RunAnimation?.Invoke(Constants.INVALID_MOVE, from, to);
            }
            else
            {
                HandleWinState(from, to);
            }
        }
        else if (from.Data.Edge)
        {
            RunAnimation?.Invoke(Constants.INVALID_MOVE, from, to);
        }
        else
        {
            RunAnimation?.Invoke(Constants.STACK_MOVE, from, to);
            TransferData(from, to);
        }
    }

    /// <summary>
    /// Handles the actions when a winning state is achieved.
    /// </summary>
    private void HandleWinState(SwipeableObject from, SwipeableObject to)
    {
        Debug.Log("WON");
        SoundManager.Play(Constants.WON);
        RunAnimation?.Invoke(Constants.STACK_MOVE, from, to);
        GameWon?.Invoke();
    }

    /// <summary>
    /// Sets the number of slices in the level.
    /// </summary>
    private void SetSlicesNumber(LevelData levelData) => _slicesNumber = levelData.SpawnPoints.Count;

    /// <summary>
    /// Transfers stack data from one object to another.
    /// </summary>
    private void TransferData(SwipeableObject from, SwipeableObject to)
    {
        to.Data.StackCount += from.Data.StackCount;
        from.Data.Stack = null;
        from.Data.StackCount = 0;
        from.Data.This = null;
    }

    /// <summary>
    /// Destroys the game object, cleaning up when the level is completed.
    /// </summary>
    private void DestroyItself() => Destroy(gameObject);
}