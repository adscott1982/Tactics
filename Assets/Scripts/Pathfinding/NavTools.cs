using System;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public static class NavTools
    {
        public static Vector2Int GetNavGridCell(this Vector2 targetPosition, float navGridSize, Vector2 origin)
        {
            var targetPositionOffset = targetPosition - origin;
            var targetCellX = (int)Math.Round(targetPositionOffset.x / navGridSize);
            var targetCellY = (int)Math.Round(targetPositionOffset.y / navGridSize);
            return new Vector2Int(targetCellX, targetCellY);
        }

        public static Vector2 GetPosition(this Vector2Int navGridPosition, Vector2 origin, float navGridSize)
        {
            var xOffSet = navGridPosition.x * navGridSize;
            var yOffSet = navGridPosition.y * navGridSize;
            return new Vector2(origin.x + xOffSet, origin.y + yOffSet);
        }
    }
}
