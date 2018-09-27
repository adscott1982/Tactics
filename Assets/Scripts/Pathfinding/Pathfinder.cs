using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float navGridSize = 0.1f;
    public float navLineWidth = 0.025f;

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
            this.lineRenderer.positionCount = this.waypoints.Count;

            for (var i = 0; i < this.waypoints.Count; i++)
            {
                var waypointPosition = this.waypoints[i].NavGridPosition.GetPosition(this.transform.position, this.navGridSize);
                this.lineRenderer.SetPosition(i, waypointPosition.AsVector3());
            }

            //for (var i = 0; i < this.waypoints.Count - 1; i++)
            //{
            //    var waypointPosition = this.waypoints[i].NavGridPosition.GetPosition(this.transform.position, this.navGridSize);
            //    var nextWaypointPosition = this.waypoints[i + 1].NavGridPosition.GetPosition(this.transform.position, this.navGridSize);
            //    Debug.DrawLine(waypointPosition, nextWaypointPosition, Color.red);
            //    Debug.DrawLine(waypointPosition, new Vector2(waypointPosition.x - 0.05f, waypointPosition.y), Color.green);
            //}
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
        var orderedOpenNodes = new SortedSet<Node>();
        var closedNodes = new List<Node>();

        var targetNavGridPosition = targetPosition.GetNavGridCell(this.navGridSize, startPosition);

        // Create open list - add current position
        var firstNode = new Node(new Vector2Int(0, 0), this.navGridSize, startPosition, targetNavGridPosition, null);
        orderedOpenNodes.Add(firstNode);

        // ** loop
        var iteration = 0;
        while (orderedOpenNodes.Count > 0)
        {
            iteration++;
            
            // Select the node in openNodes with lowest full cost
            Debug.Log($"Checking {orderedOpenNodes.Count} open nodes ordered by full cost, first {orderedOpenNodes.First().FullCost}, last {orderedOpenNodes.First().FullCost}");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var currentNode = orderedOpenNodes.First();
            
            var timeToGetSorted = sw.RecordAndRestart();
            //Debug.Log($"Lowest cost node selected, cell [{currentNode.NavGridPosition.x}, {currentNode.NavGridPosition.y}], start cost: {currentNode.StartCost}, end cost {currentNode.EndCost}");
            orderedOpenNodes.Remove(currentNode);
            var timeToRemove = sw.RecordAndRestart();
            // Remove this node from open, and add to closed
            closedNodes.Add(currentNode);
            var timeToAddToClosed = sw.RecordAndRestart();
            // If current is the target position cell, then target found, go to next step
            if (currentNode.NavGridPosition == targetNavGridPosition)
            {
                break;
            }

            var curX = currentNode.NavGridPosition.x;
            var curY = currentNode.NavGridPosition.y;

            var newNodes = 0;
            var recalculatedNodes = 0;

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

                    // if neighbour is already closed, skip to next
                    if (closedNodes.Any(n => n.NavGridPosition == navGridPosition))
                    {
                        continue;
                    }

                    
                    var node = new Node(navGridPosition, this.navGridSize, startPosition, targetNavGridPosition, currentNode);

                    // if neighbour is not navigable add to closed and continue
                    //if (node.IsTraversable(this.navCollider)

                    var oldNode = orderedOpenNodes.FirstOrDefault(n => n.NavGridPosition == node.NavGridPosition);
                    if (oldNode != null)
                    {
                        recalculatedNodes++;
                        if (node.StartCost < oldNode.StartCost)
                        {
                            orderedOpenNodes.Add(node);
                        }
                    }
                    else
                    {
                        newNodes++;
                        orderedOpenNodes.Add(node);
                    }
                }
            }


            var timeToCalculateNeighbours = sw.RecordAndRestart();
            Debug.Log($"Iteration [{iteration}], getsorted: {timeToGetSorted.TotalMilliseconds:F3} ms, remove: {timeToRemove.TotalMilliseconds:F3} ms,timeToAdd: {timeToAddToClosed.TotalMilliseconds:F3} ms, calcNeighbours: {timeToCalculateNeighbours.TotalMilliseconds:F3} ms");
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
}
