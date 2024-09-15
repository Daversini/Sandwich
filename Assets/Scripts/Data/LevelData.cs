using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Holds data related to a specific level, including spawn points and grid dimensions.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Scriptable Objects/LevelData")]
    public class LevelData : ScriptableObject
    {
        public List<Vector2Int> SpawnPoints;
        public Vector2Int StartPoint;
        public Vector2Int EndPoint;
        public int Index;
        public int Width;
        public int Height;
    }
}