using System;
using Data;
using Grid;
using Managers.GameStates;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Responsible for loading and generating levels, including spawning swipeable objects within the grid.
    /// </summary>
    public class LevelLoader : MonoBehaviour
    {
        public static event Action<LevelData> LevelLoaded;

        [SerializeField]
        private SwipeableObject _swipeableObjectPrefab;

        [Min(2)]
        [SerializeField]
        private int _gridWidth = 2;

        [Min(2)]
        [SerializeField]
        private int _gridHeight = 2;

        private LevelData _levelToLoad;
        private Grid<SwipeableObjectData> _grid;

        private void OnEnable() => WinState.LoadNextLevel += LoadLevel;
        private void OnDisable() => WinState.LoadNextLevel -= LoadLevel;

        private void Start() => LoadLevel();

        /// <summary>
        /// Loads a new level by generating level data, creating the grid, and spawning objects.
        /// </summary>
        private void LoadLevel()
        {
            GenerateLevelData();
            InitializeGrid();
            UpdateLevelIndex();
            SpawnSwipeableObjects();
            LevelLoaded?.Invoke(_levelToLoad); // Trigger the LevelLoaded event
        }

        /// <summary>
        /// Generates the level data with random parameters.
        /// </summary>
        private void GenerateLevelData()
        {
            int slicesNumber = UnityEngine.Random.Range(3, _gridWidth * _gridHeight);
            _levelToLoad = LevelGenerator.GenerateLevel(slicesNumber, _gridWidth, _gridHeight);
        }

        /// <summary>
        /// Initializes the grid based on the generated level data.
        /// </summary>
        private void InitializeGrid()
        {
            _grid = GridHandler.CreateGrid(_levelToLoad.Width, _levelToLoad.Height, 1);
        }

        /// <summary>
        /// Updates the level index stored in player preferences.
        /// </summary>
        private void UpdateLevelIndex()
        {
            _levelToLoad.Index = PlayerPrefs.GetInt(Constants.LAST_LEVEL_INDEX);
            PlayerPrefs.SetInt(Constants.LAST_LEVEL_INDEX, _levelToLoad.Index + 1);
        }

        /// <summary>
        /// Spawns swipeable objects in the grid based on the generated level data.
        /// </summary>
        private void SpawnSwipeableObjects()
        {
            foreach (Vector2Int coordinates in _levelToLoad.SpawnPoints)
            {
                CreateSwipeableObjectAt(coordinates);
            }
        }

        /// <summary>
        /// Creates a swipeable object at the specified coordinates.
        /// </summary>
        private void CreateSwipeableObjectAt(Vector2Int coordinates)
        {
            SwipeableObjectData swipeableData = _grid.GetGridObject(coordinates.x, coordinates.y);
            swipeableData.Edge = IsEdgeCoordinate(swipeableData.Row, swipeableData.Column);
            
            SwipeableObject newSwipeable = Instantiate(
                _swipeableObjectPrefab,
                _grid.GetWorldPosition(swipeableData.Column, swipeableData.Row),
                Quaternion.identity
            );

            newSwipeable.Data = swipeableData;
            newSwipeable.InitializeObject();
        }

        /// <summary>
        /// Checks if the given coordinates are at the edge (start or end point) of the level.
        /// </summary>
        private bool IsEdgeCoordinate(int row, int column)
        {
            return (row == _levelToLoad.StartPoint.x && column == _levelToLoad.StartPoint.y) || 
                   (row == _levelToLoad.EndPoint.x && column == _levelToLoad.EndPoint.y);
        }

#if UNITY_EDITOR
        // Draws the grid in the editor for visualization purposes
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            GridHandler.VisualizeGrid();
        }
#endif
    }
}