using UnityEngine;

namespace Data
{
    /// <summary>
    /// Represents the data needed to reverse a move, including the original position, rotation, and stacking details.
    /// </summary>
    public struct InverseMove 
    {
        public SwipeableObject Itself;
        public SwipeableObject CameFrom;
        public Transform Parent;
        public GameObject Stack;
        public Quaternion TargetRotation;
        public Vector3 StartingPoint;
        public Vector3 FinalPoint;
        public Vector3 RotationPivotPosition;
        public int StackCount;

        public InverseMove(SwipeableObject itself, SwipeableObject cameFrom, Vector3 startingPoint, Vector3 rotationPivotPosition, Quaternion targetRotation)
        {
            Itself = itself;
            CameFrom = cameFrom;
            Parent = itself.transform;
            Stack = itself.Data.Stack;
            StackCount = itself.Data.StackCount;
            TargetRotation = targetRotation;
            StartingPoint = startingPoint;
            FinalPoint = itself.Data.Stack.transform.position;
            RotationPivotPosition = rotationPivotPosition;
        }
    }
}