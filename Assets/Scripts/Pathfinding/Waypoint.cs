using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class Waypoint
    {
        public Vector2 Position { get; }
        public Waypoint Parent { get; private set; }

        public Waypoint(Vector2 position, Waypoint parent)
        {
            this.Position = position;
            this.Parent = parent;
        }
    }
}
