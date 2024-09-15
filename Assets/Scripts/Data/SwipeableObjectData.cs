using UnityEngine;

namespace Data
{
    /// <summary>
    /// Stores information about a swipeable object, including its grid position and stack properties.
    /// </summary>
    [System.Serializable]
    public class SwipeableObjectData
    {
        public int Row;
        public int Column;
        public int StackCount;
        public bool Edge;
        public SwipeableObject This;
        public GameObject Stack;

        public SwipeableObjectData(int x, int y)
        {
            Row = x;
            Column = y;
        }
    }
}