using System;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class Node
    {
        public float StartCost { get; private set; }
        public float EndCost { get; private set; }
        public float FullCost { get; private set; }

        public Vector2Int NavGridPosition { get; private set; }
        public Vector2 Position { get; private set; }
        public Node CameFrom { get; private set; }

        public Node(Vector2Int navGridPosition, Vector2 position, Vector2 targetPosition, Node cameFrom)
        {
            this.NavGridPosition = navGridPosition;
            this.Position = position;
            this.UpdateCameFromNode(cameFrom);

            // Distance from this position to target - efficient?
            this.EndCost = Vector2.Distance(position, targetPosition);

            this.FullCost = this.StartCost + this.EndCost;
        }

        public void UpdateCameFromNode(Node cameFrom)
        {
            this.CameFrom = cameFrom;

            if (cameFrom == null)
            {
                this.StartCost = 0f;
            }
            else
            {
                // Start cost is current distance travelled to the previous node + new distance
                this.StartCost = cameFrom.StartCost + Vector2.Distance(cameFrom.Position, this.Position);
            }

            this.FullCost = this.StartCost + this.EndCost;
        }
    }
}
