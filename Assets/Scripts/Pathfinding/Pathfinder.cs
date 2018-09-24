using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float navGridSize = 0.1f;

    // Private fields
    private Vector2 targetPosition;
    private List<Waypoint> waypoints = new List<Waypoint>();
    private Collider2D collider;


    // Start is called before the first frame update
    private void Start()
    {
        this.collider = this.GetComponent<Collider2D>();
        this.DrawPath = true;
    }

    // Called every frame
    private void Update()
    {
        
    }

    public Vector2 TargetPosition
    {
        get => this.targetPosition;
        set
        {
            if (this.targetPosition != value)
            {
                if (this.CalculateWaypoints(value))
                {
                    this.targetPosition = value;
                }
            }
        }
    }

    public bool DrawPath { get; set; }

    private bool CalculateWaypoints(Vector2 targetPosition)
    {
        this.waypoints.Clear();

        // Step 1: A* Pathfind to target using specified nav grid size, checking each cell for collider collision with sphere, or rectangle

        // Step 2: Raycast 2D from final waypoint to current position / first waypoint forward until finding first line of sight connection.
        // Remove intermediate waypoints
        // Perform Step 2 again but for second last waypoint, until worked back to the first two waypoints

        this.waypoints.Add(new Waypoint(targetPosition, null));
        return true;
    }

}
