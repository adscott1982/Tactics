using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float navGridSize = 0.1f;
    public float navLineWidth = 0.01f;

    // Private fields
    private Vector2 targetPosition;
    private List<Node> waypoints = new List<Node>();
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
            //this.lineRenderer.positionCount = this.waypoints.Count + 1;
            //this.lineRenderer.SetPosition(0, this.transform.position);
            
            //for (var i = 0; i < this.waypoints.Count; i++)
            //{
            //    this.lineRenderer.SetPosition(i + 1, this.waypoints[i].Position.AsVector3());
            //}

            for (var i = 0; i < this.waypoints.Count - 1; i++)
            {
                Debug.DrawLine(this.waypoints[i].Position, this.waypoints[i + 1].Position, Color.red);
                Debug.DrawLine(this.waypoints[i].Position, new Vector2(this.waypoints[i].Position.x - 0.05f, this.waypoints[i].Position.y), Color.green);
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

        var startPosition = this.transform.position.AsVector2();
        var openNodes = new List<Node>();
        var closedNodes = new List<Node>();

        var targetCell = GetNavGridCell(this.navGridSize, startPosition, targetPosition);
        

        // Step 1: A* Pathfind to target using specified nav grid size, checking each cell for collider collision with sphere, or rectangle

        // Create open list - add current position
        openNodes.Add(new Node(new Vector2Int(0, 0), startPosition, targetPosition, null));

        // ** loop

        while (openNodes.Any())
        {
            // Select the node in openNodes with lowest full cost
            var currentNode = openNodes.OrderBy(node => node.FullCost).First();

            // Remove this node from open, and add to closed
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // If current is the target position cell, then target found, go to next step
            if (currentNode.NavGridPosition == targetCell)
            {
                break;
            }

            var curX = currentNode.NavGridPosition.x;
            var curY = currentNode.NavGridPosition.y;

            // else, go through each neighbour of the current node, creating node or updating camefrom
            for (var xOffset = -1; xOffset < 2; xOffset++)
            {
                for (var yOffset = -1; yOffset < 2; yOffset++)
                {
                    if (xOffset == 0 & yOffset == 0)
                    {
                        continue;
                    }

                    var navGridPosition = new Vector2Int(curX + xOffset, curY + yOffset);

                    // if neighbour with circle collider intersects grid collider or neighbour is already closed, skip to next
                    if (closedNodes.Any(n => n.NavGridPosition == navGridPosition))
                    {
                        continue;
                    }

                    var position = new Vector2((navGridPosition.x * this.navGridSize) + startPosition.x, (navGridPosition.y * this.navGridSize) + startPosition.y);
                    var node = new Node(navGridPosition, position, targetPosition, currentNode);

                    var oldNode = openNodes.FirstOrDefault(n => n.NavGridPosition == node.NavGridPosition);

                    if (oldNode == null)
                    {
                        openNodes.Add(node);
                        continue;
                    }

                    if (oldNode.StartCost > node.StartCost)
                    {
                        openNodes.Remove(oldNode);
                        openNodes.Add(node);
                    }
                }
            }
        }

        // Step 2: Raycast 2D from final waypoint to current position / first waypoint forward until finding first line of sight connection.
        // Remove intermediate waypoints
        // Perform Step 2 again but for second last waypoint, until worked back to the first two waypoints

        var nextNode = closedNodes.Last();
        while (nextNode != null)
        {
            this.waypoints.Add(nextNode);
            nextNode = nextNode.CameFrom;
        }

        //this.waypoints.Add(new Waypoint(targetPosition, null));
        return true;
    }

    private static Vector2Int GetNavGridCell(float navGridSize, Vector2 origin, Vector2 targetPosition)
    {
        var targetPositionOffset = targetPosition - origin;
        var targetCellX = (int)Math.Round(targetPositionOffset.x / navGridSize);
        var targetCellY = (int)Math.Round(targetPositionOffset.y / navGridSize);
        return new Vector2Int(targetCellX, targetCellY);
    }
}
