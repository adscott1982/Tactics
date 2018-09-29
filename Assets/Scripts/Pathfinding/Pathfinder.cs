using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float navGridSize = 0.1f;
    public float navLineWidth = 0.025f;
    public ContactFilter2D contactFilter2D;
    public LineRenderer lineRenderer;

    // Private fields
    private Vector2 targetPosition;
    private Collider2D navCollider;
    public Collider2D wallsCollider;

    public List<Node> Waypoints { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        this.Waypoints = new List<Node>();
        this.navCollider = this.GetComponent<Collider2D>();
        this.lineRenderer.startWidth = navLineWidth;
        this.lineRenderer.endWidth = navLineWidth;
        this.DrawPath = true;
    }

    // Called every frame
    private void Update()
    {
        //this.DrawPathLines();
    }

    private void DrawPathLines()
    {
        if (this.DrawPath && this.Waypoints.Any())
        {
            this.lineRenderer.positionCount = this.Waypoints.Count;

            for (var i = 0; i < this.Waypoints.Count; i++)
            {
                var waypointPosition = this.Waypoints[i].NavGridPosition.GetPosition(this.transform.position, this.navGridSize);
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
                    this.DrawPathLines();
                }
            }
        }
    }

    public bool DrawPath { get; set; }

    private bool CalculateWaypoints(Vector2 targetPosition)
    {
        this.Waypoints.Clear();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var startPosition = this.transform.position.AsVector2();
        var openNodesSorted = new BMinHeap<Node>();
        var allOpenNodes = new Dictionary<Vector2Int, Node>();
        var closedNodes = new Dictionary<Vector2Int, Node>();
        var blockedNodes = new Dictionary<Vector2Int, Node>();

        var targetNavGridPosition = targetPosition.GetNavGridCell(this.navGridSize, startPosition);

        var targetNode = new Node(targetNavGridPosition, this.navGridSize, startPosition, targetNavGridPosition, null);
        if (!targetNode.IsTraversable(this.navCollider, this.contactFilter2D))
        {
            return false;
        }

        // Create open list - add current position
        var firstNode = new Node(new Vector2Int(0, 0), this.navGridSize, startPosition, targetNavGridPosition, null);
        openNodesSorted.Insert(firstNode);
        allOpenNodes.Add(firstNode.NavGridPosition, firstNode);

        while (openNodesSorted.Count > 0)
        {
            // Select the node in openNodes with lowest full cost
            var currentNode = openNodesSorted.ExtractMin();
            allOpenNodes.Remove(currentNode.NavGridPosition);

            closedNodes.Add(currentNode.NavGridPosition, currentNode);

            // If target reached, exit
            if (currentNode.NavGridPosition == targetNavGridPosition)
            {
                break;
            }

            foreach(var neighbour in this.GetNeighbours(currentNode, startPosition, targetNavGridPosition).ToList())
            {
                if (blockedNodes.ContainsKey(neighbour.NavGridPosition))
                {
                    continue;
                }

                // If not walkable, continue
                if (!neighbour.IsTraversable(this.navCollider, this.contactFilter2D))
                {
                    blockedNodes.Add(neighbour.NavGridPosition, neighbour);
                    continue;
                }

                // If in closed nodes, continue
                if (closedNodes.ContainsKey(neighbour.NavGridPosition))
                {
                    continue;
                }

                // If not in open list, add to open list
                if (!allOpenNodes.ContainsKey(neighbour.NavGridPosition))
                {
                    openNodesSorted.Insert(neighbour);
                    allOpenNodes.Add(neighbour.NavGridPosition, neighbour);
                }
                else
                {
                    var oldNode = allOpenNodes[neighbour.NavGridPosition];

                    // If the new node calculation has a better cost than the old one, replace it
                    if (neighbour.FullCost < oldNode.FullCost)
                    {
                        oldNode.UpdateCameFromNode(currentNode);
                    }
                }
            }
        }

        Debug.Log($"Time to get path: {sw.Elapsed.TotalMilliseconds:F2} ms");
        // Step 2: Raycast 2D from final waypoint to current position / first waypoint forward until finding first line of sight connection.
        // use collider.Cast from the start position to the target
        // Remove intermediate waypoints
        // Perform Step 2 again but for second last waypoint, until worked back to the first two waypoints

        var nextNode = closedNodes.Last().Value;
        while (nextNode != null)
        {
            this.Waypoints.Insert(0, nextNode);
            nextNode = nextNode.CameFrom;
        }

        return true;
    }

    private IEnumerable<Node> GetNeighbours(Node currentNode, Vector2 origin, Vector2Int targetCell)
    {
        var curX = currentNode.NavGridPosition.x;
        var curY = currentNode.NavGridPosition.y;

        for (var xOffset = -1; xOffset < 2; xOffset++)
        {
            for (var yOffset = -1; yOffset < 2; yOffset++)
            {
                if (xOffset == 0 & yOffset == 0)
                {
                    continue;
                }

                var navGridPosition = new Vector2Int(curX + xOffset, curY + yOffset);
                yield return new Node(navGridPosition, this.navGridSize, origin, targetCell, currentNode);
            }
        }
    }
}
