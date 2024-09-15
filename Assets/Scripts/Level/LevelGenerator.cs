using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Abstract class for generating random levels, including the placement of spawn points within a grid.
    /// </summary>
    public abstract class LevelGenerator
    {
        private static readonly System.Random _random = new System.Random();

        private static readonly Vector2Int _directionUp = new Vector2Int(1, 0);
        private static readonly Vector2Int _directionDown = new Vector2Int(-1, 0);
        private static readonly Vector2Int _directionLeft = new Vector2Int(0, -1);
        private static readonly Vector2Int _directionRight = new Vector2Int(0, 1);

        private static List<Vector2Int> _generatedCoordinates;

        /// <summary>
        /// Generates a new level with specified parameters, ensuring valid grid and slice numbers.
        /// </summary>
        public static LevelData GenerateLevel(int slicesNumber, int gridWidth, int gridHeight)
        {
            // Ensure grid dimensions and slice number are valid
            gridWidth = gridWidth < 2 ? 2 : gridWidth;
            gridHeight = gridHeight < 2 ? 2 : gridHeight;
            slicesNumber = slicesNumber > gridWidth * gridHeight ? (gridWidth * gridHeight) - 1 : slicesNumber;

            ComputeRandomLevelData(slicesNumber, gridWidth, gridHeight);

            LevelData generatedLevel = ScriptableObject.CreateInstance<LevelData>();
            generatedLevel.SpawnPoints = _generatedCoordinates;
            generatedLevel.StartPoint = _generatedCoordinates[0];
            generatedLevel.EndPoint = _generatedCoordinates[1];
            generatedLevel.Width = gridWidth;
            generatedLevel.Height = gridHeight;
            return generatedLevel;
        }

        /// <summary>
        /// Computes random level data by generating coordinates for slices within the grid.
        /// </summary>
        private static void ComputeRandomLevelData(int slicesNumber, int gridWidth, int gridHeight)
        {
            _generatedCoordinates = new List<Vector2Int>();
            _generatedCoordinates.Add(GetRandomCoordinates(gridWidth, gridHeight));
            _generatedCoordinates.Add(GetRandomNeighbour(gridWidth, gridHeight, _generatedCoordinates[0]));
            Vector2Int even = _generatedCoordinates[0];
            Vector2Int odd = _generatedCoordinates[1];
            for (int i = 0; i < slicesNumber - 2; i++)
            {
                if (i % 2 == 0)
                {
                    _generatedCoordinates.Add(GetRandomNeighbour(gridWidth, gridHeight, even));
                    even = _generatedCoordinates[^1];
                }
                else
                {
                    _generatedCoordinates.Add(GetRandomNeighbour(gridWidth, gridHeight, odd));
                    odd = _generatedCoordinates[^1];
                }
            }
        }

        /// <summary>
        /// Generates random coordinates within the grid.
        /// </summary>
        private static Vector2Int GetRandomCoordinates(int gridWidth, int gridHeight) 
            => new Vector2Int(Random.Range(0, gridHeight), Random.Range(0, gridWidth));

        /// <summary>
        /// Finds a random neighboring coordinate that is not already in the list of generated coordinates.
        /// </summary>
        private static Vector2Int GetRandomNeighbour(int gridWidth, int gridHeight, Vector2Int center)
        {
            List<Vector2Int> directions = GetAllowedDirections(gridWidth, gridHeight, center);
            directions = directions.Count > 1 ? ShuffleList(directions) : directions;

            Vector2Int randomNeighbour;
            foreach (Vector2Int direction in directions)
            {
                randomNeighbour = center + direction;
                if (_generatedCoordinates.Contains(randomNeighbour)) continue;
                else return randomNeighbour;
            }

            Debug.LogWarning("Neighbour not found, reiterating method");
            return GetRandomNeighbour(gridWidth, gridHeight, _generatedCoordinates[_random.Next(_generatedCoordinates.Count)]);
        }

        /// <summary>
        /// Gets the allowed movement directions within the grid based on the current position.
        /// </summary>
        private static List<Vector2Int> GetAllowedDirections(int gridWidth, int gridHeight, Vector2Int center)
        {
            List<Vector2Int> allowedDirections = new List<Vector2Int>();
            if (center.x != 0) allowedDirections.Add(_directionDown);
            if (center.x != gridHeight - 1) allowedDirections.Add(_directionUp);
            if (center.y != 0) allowedDirections.Add(_directionLeft);
            if (center.y != gridWidth - 1) allowedDirections.Add(_directionRight);
            return allowedDirections;
        }

        /// <summary>
        /// Fisher-Yates Shuffle Algorithm to shuffle a list.
        /// </summary>
        private static List<T> ShuffleList<T>(List<T> listToShuffle)
        {
            if (listToShuffle == null) return listToShuffle;

            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = _random.Next(i + 1);
                (listToShuffle[k], listToShuffle[i]) = (listToShuffle[i], listToShuffle[k]);
            }
            return listToShuffle;
        }
    }
}