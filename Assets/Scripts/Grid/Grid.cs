using System;
using UnityEngine;

namespace Grid
{
    /// <summary>
    /// Represents a 2D grid structure that can hold objects of a specified type, allowing for easy placement and retrieval of objects based on grid coordinates.
    /// </summary>
    public class Grid<TGridObject> 
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private Vector3 _originPosition;
        private TGridObject[,] _gridArray;

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<int, int, TGridObject> createGridObject) 
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new TGridObject[height, width];

            for (int x = 0; x < _gridArray.GetLength(0); x++) 
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++) 
                {
                    _gridArray[x, y] = createGridObject(x, y);
                }
            }
        }

        public int GetWidth() => _width;

        public int GetHeight() => _height;

        public float GetCellSize() => _cellSize;

        /// <summary>
        /// Converts grid coordinates to the corresponding world position.
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0f, y) * _cellSize + new Vector3(1f, 0f, 1f) * _cellSize * 0.5f + _originPosition;
        }

        /// <summary>
        /// Outputs the grid coordinates corresponding to the given world position.
        /// </summary>
        public void GetXY(Vector3 worldPosition, out int x, out int y) 
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize); 
        }

        /// <summary>
        /// Retrieves the object located at the specified grid coordinates.
        /// </summary>
        public TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _height && y < _width) 
                return _gridArray[x, y];
            return default;
        }

        /// <summary>
        /// Retrieves the object located at the grid coordinates corresponding to the specified world position.
        /// </summary>
        public TGridObject GetGridObject(Vector3 worldPosition) 
        {
            GetXY(worldPosition, out int x, out int y);
            return GetGridObject(x, y);
        }
    }
}