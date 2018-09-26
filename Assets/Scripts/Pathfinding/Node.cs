using System;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class Node
    {
        public int StartCost { get; private set; }
        public int EndCost { get; private set; }
        public int FullCost { get; private set; }

        public Vector2Int NavGridPosition { get; private set; }
        public Vector2Int TargetPosition { get; private set; }
        public Node CameFrom { get; private set; }

        public Node(Vector2Int navGridPosition, Vector2Int targetPosition, Node cameFrom)
        {
            this.NavGridPosition = navGridPosition;
            this.UpdateCameFromNode(cameFrom);

            this.EndCost = TravelCost(navGridPosition, targetPosition);
            this.FullCost = this.StartCost + this.EndCost;
        }

        public void UpdateCameFromNode(Node cameFrom)
        {
            this.CameFrom = cameFrom;

            if (cameFrom == null)
            {
                this.StartCost = 0;
            }
            else
            {
                // Start cost is current distance travelled to the previous node + new distance
                this.StartCost = cameFrom.StartCost + TravelCost(cameFrom.NavGridPosition, this.NavGridPosition);
            }

            this.FullCost = this.StartCost + this.EndCost;
        }

        private static int TravelCost(Vector2Int origin, Vector2Int destination)
        {
            var xDelta = Math.Abs(origin.x - destination.x);
            var yDelta = Math.Abs(origin.y - destination.y);

            var (min, max) = xDelta < yDelta ? (xDelta, yDelta - xDelta) : (yDelta, xDelta - yDelta);

            var cost = (min * 14) + (max * 10);
            return cost;
        }
    }
}
