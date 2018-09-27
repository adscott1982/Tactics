using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class Node : IComparable<Node>
    {
        public int StartCost { get; private set; }
        public int EndCost { get; private set; }
        public int FullCost { get; private set; }
        public Vector2 WorldPosition { get; private set; }
        public Vector2Int NavGridPosition { get; private set; }
        public Vector2Int TargetPosition { get; private set; }
        public Node CameFrom { get; private set; }

        public Node(Vector2Int navGridPosition, float navGridSize, Vector2 origin, Vector2Int targetPosition, Node cameFrom)
        {
            this.NavGridPosition = navGridPosition;
            this.UpdateCameFromNode(cameFrom);

            this.WorldPosition = new Vector2((navGridPosition.x * navGridSize) + origin.x, (navGridPosition.y * navGridSize) + origin.y);

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

            var min = Math.Min(xDelta, yDelta);
            var max = Math.Max(xDelta, yDelta);

            var diagCost = min * 14;
            var straightCost = (max - min) * 10;
            return diagCost + straightCost;
        }

        public int CompareTo(Node other)
        {
            return this.FullCost.CompareTo(other.FullCost);
        }

        internal bool IsTraversable(Collider2D navCollider, Collider2D wallsCollider, ContactFilter2D contactFilter)
        {
            //if (navCollider is CircleCollider2D)
            //{
            //if (wallsCollider.OverlapPoint(this.WorldPosition))
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}


            if (this.WorldPosition.x > -4 && this.WorldPosition.x < 4)
            {
                if (this.WorldPosition.y > 2 && this.WorldPosition.y < 4)
                {
                    return false;
                }
            }

            return true;
            var contactArray = new Collider2D[1];
                var count = Physics2D.OverlapCircle(this.WorldPosition, ((CircleCollider2D)navCollider).radius, contactFilter, contactArray);

            if (contactArray[0] == navCollider)
            {
                Debug.LogError("hit own collider");
            }
                return count == 0;
            //}
            //else
            //{
            //    var centre = navCollider.bounds.center.AsVector2();
            //    var upperLeft = centre - navCollider.bounds.extents.AsVector2();
            //    var lowerRight = centre + navCollider.bounds.extents.AsVector2();
            //    return Physics2D.OverlapArea(upperLeft, lowerRight, contactFilter, null) == 0;
            //}
            
            // Check if current position overlapcollider returns any value
            //return Physics2D.OverlapCollider(navCollider, contactFilter, null) == 0;
        }
    }
}
