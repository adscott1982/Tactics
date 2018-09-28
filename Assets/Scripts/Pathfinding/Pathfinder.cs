using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float navGridSize = 0.1f;
    public float navLineWidth = 0.025f;
    public ContactFilter2D contactFilter2D;

    // Private fields
    private Vector2 targetPosition;
    private List<Node> waypoints = new List<Node>();
    private Collider2D navCollider;
    public Collider2D wallsCollider;
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
        var allOpenNodes = new Dictionary<Vector2Int, Node>();
        var closedNodes = new List<Node>();

        var targetNavGridPosition = targetPosition.GetNavGridCell(this.navGridSize, startPosition);
        Debug.Log($"Target [{targetNavGridPosition.x},{targetNavGridPosition.y}]");

        var targetNode = new Node(targetNavGridPosition, this.navGridSize, startPosition, targetNavGridPosition, null);
        if (!targetNode.IsTraversable(this.navCollider, this.wallsCollider, this.contactFilter2D))
        {
            return false;
        }

        // Create open list - add current position
        var firstNode = new Node(new Vector2Int(0, 0), this.navGridSize, startPosition, targetNavGridPosition, null);
        orderedOpenNodes.Add(firstNode);
        allOpenNodes.Add(firstNode.NavGridPosition, firstNode);

        while (orderedOpenNodes.Count > 0)
        {
            var text = $"Ordered: ";
            foreach (var orderedOpenNode in orderedOpenNodes)
            {
                text +=
                    $"[{orderedOpenNode.NavGridPosition.x},{orderedOpenNode.NavGridPosition.y}={orderedOpenNode.FullCost}]";
            }

            // Select the node in openNodes with lowest full cost
            var currentNode = orderedOpenNodes.First();
            orderedOpenNodes.Remove(currentNode);
            allOpenNodes.Remove(currentNode.NavGridPosition);
            closedNodes.Add(currentNode);



            Debug.Log(text);
            // If target reached, exit
            if (currentNode.NavGridPosition == targetNavGridPosition)
            {
                break;
            }

            foreach(var neighbour in this.GetNeighbours(currentNode, startPosition, targetNavGridPosition).ToList())
            {
                Debug.Log($"Checking [{neighbour.NavGridPosition.x},{neighbour.NavGridPosition.y}] start: {neighbour.StartCost}, end {neighbour.EndCost}, full {neighbour.FullCost}");

                // If not walkable, continue
                if (!neighbour.IsTraversable(this.navCollider, this.wallsCollider, this.contactFilter2D))
                {
                    Debug.Log($"{neighbour.NavGridPosition.x},{neighbour.NavGridPosition.y} not traversable, continuing");
                    continue;
                }

                // If in closed, continue
                if (closedNodes.FirstOrDefault(n => n.NavGridPosition == neighbour.NavGridPosition) != null)
                {
                    Debug.Log($"[{neighbour.NavGridPosition.x},{neighbour.NavGridPosition.y}] already closed, continuing");
                    continue;
                }

                // If not in open list, add to open list
                //var oldOpen = orderedOpenNodes.FirstOrDefault(n => n.NavGridPosition == neighbour.NavGridPosition);
                if (!allOpenNodes.TryGetValue(neighbour.NavGridPosition, out var oldOpen))
                {
                    Debug.Log($"[{neighbour.NavGridPosition.x},{neighbour.NavGridPosition.y}] is new, adding to open");
                    orderedOpenNodes.Add(neighbour);
                    allOpenNodes.Add(neighbour.NavGridPosition, neighbour);
                }
                else
                {
                    // If the new node calculation has a better cost, replace it.
                    if (neighbour.FullCost < oldOpen.FullCost)
                    {
                        Debug.Log($"{neighbour.NavGridPosition.x},{neighbour.NavGridPosition.y} already present in open, replacing, old cost {oldOpen.FullCost}, new cost {neighbour.FullCost}");
                        //orderedOpenNodes.Remove(oldOpen);
                        orderedOpenNodes.Add(neighbour);
                        allOpenNodes[neighbour.NavGridPosition] = neighbour;
                    }
                    else
                    {
                        Debug.Log($"{neighbour.NavGridPosition.x},{neighbour.NavGridPosition.y} already present in open, but higher than old cost {oldOpen.FullCost}, new cost {neighbour.FullCost}");

                    }
                }
            }
        }

        // Step 2: Raycast 2D from final waypoint to current position / first waypoint forward until finding first line of sight connection.
        // use collider.Cast from the start position to the target
        // Remove intermediate waypoints
        // Perform Step 2 again but for second last waypoint, until worked back to the first two waypoints

        var nextNode = closedNodes.Last();
        while (nextNode != null)
        {
            this.waypoints.Add(nextNode);
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
