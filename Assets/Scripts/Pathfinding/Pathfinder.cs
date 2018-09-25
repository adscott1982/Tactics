using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float navGridSize = 0.1f;
    public float navLineWidth = 0.1f;

    // Private fields
    private Vector2 targetPosition;
    private List<Waypoint> waypoints = new List<Waypoint>();
    private Collider2D navCollider;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        this.navCollider = this.GetComponent<Collider2D>();
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.startWidth = navLineWidth;
        this.lineRenderer.endWidth = navLineWidth;
        this.DrawPath = true;
    }

    // Called every frame
    private void Update()
    {
        this.DrawPathLines();
        
    }

    private void DrawPathLines()
    {
        if (this.DrawPath && this.waypoints.Any())
        {
            this.lineRenderer.positionCount = this.waypoints.Count + 1;
            this.lineRenderer.SetPosition(0, this.transform.position);
            
            for (var i = 0; i < this.waypoints.Count; i++)
            {
                this.lineRenderer.SetPosition(i + 1, this.waypoints[i].Position.AsVector3());
            }
        }
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
