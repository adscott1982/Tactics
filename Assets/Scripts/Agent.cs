using Assets.Scripts.Interfaces;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour, ISelectable, IActionPerformer
{
    public GameObject SelectionIndicator;
    private SpriteRenderer selectionIndicatorSprite;
    private Pathfinder pathFinder;

    public bool IsSelected { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        this.selectionIndicatorSprite = SelectionIndicator.GetComponent<SpriteRenderer>();
        this.pathFinder = this.GetComponent<Pathfinder>();
    }

    // Update is called once per frame
    void Update()
    {
        this.selectionIndicatorSprite.enabled = this.IsSelected;
    }

    private void HandleInputs()
    {
        // In future do this by Mouse mode, e.g. selection, additional selection, group selection / panning / plotting
        if (Input.GetMouseButtonDown(0))
        {
            // Clear
            //this.selectables.ForEach(s => s.IsSelected = false);

            // Find all hits on selectable objects
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, 0)
                .Where(h => h.transform.HasScriptType<ISelectable>());

            if (hits.Any())
            {
                // Select the first hit
                var selectable = hits.First().transform.GetScriptType<ISelectable>();
                selectable.IsSelected = !selectable.IsSelected;
            }
        }
    }

    public void PerformAction(Vector2 mousePosition)
    {
        Debug.Log($"Action performed to X: {mousePosition.x} Y: {mousePosition.y}");
        this.pathFinder.TargetPosition = mousePosition;
    }
}
