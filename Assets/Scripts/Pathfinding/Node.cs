using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class Node : IComparable<Node>, IComparable
    {
        public int StartCost { get; private set; }
        public int EndCost { get; private set; }
        public int FullCost { get; private set; }
        public Vector2 WorldPosition { get; private set; }
        public Vector2Int NavGridPosition { get; private set; }
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
        public override string ToString()
        {
            return $"[{this.NavGridPosition.x},{this.NavGridPosition.y}] {this.FullCost}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return this.NavGridPosition == ((Node)obj).NavGridPosition;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.NavGridPosition.GetHashCode();
        }

        public int CompareTo(Node other)
        {
            if (other.NavGridPosition == this.NavGridPosition)
            {
                return 0;
            }

            if (other.FullCost == this.FullCost)
            {
                if (other.NavGridPosition.x != this.NavGridPosition.x)
                {
                    return this.NavGridPosition.x.CompareTo(other.NavGridPosition.x);
                }

                return this.NavGridPosition.y.CompareTo(other.NavGridPosition.y);
            }

            return this.FullCost.CompareTo(other.FullCost);
        }

        public int CompareTo(object obj)
        {
            return CompareTo((Node)obj);
        }

        internal bool IsTraversable(Collider2D navCollider, ContactFilter2D contactFilter)
        {
            if (!(navCollider is CircleCollider2D))
            {
                return false;
            }

            var contactArray = new Collider2D[1];
            var count = Physics2D.OverlapCircle(this.WorldPosition, ((CircleCollider2D)navCollider).radius, contactFilter, contactArray);

            return count == 0;
        }
    }
}
