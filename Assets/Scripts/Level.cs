using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    private List<ISelectable> allSelectables;
    private List<ISelectable> selectedObjects = new List<ISelectable>();
    private InteractionMode interactionMode;

    // Start is called before the first frame update
    void Start()
    {
        this.allSelectables = GameObject.FindObjectsOfType<Transform>()
            .Where(t => t.HasScriptType<ISelectable>())
            .Select(t => t.GetScriptType<ISelectable>()).ToList();

        var count = this.allSelectables.Count();

        this.interactionMode = InteractionMode.Selection;
    }

    // Update is called once per frame
    void Update()
    {
        this.HandleInputs();
    }


    private void HandleInputs()
    {
        // In future do this by Mouse mode, e.g. selection, additional selection, group selection / panning / plotting
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            switch (this.interactionMode)
            {
                case InteractionMode.Selection:
                    this.PerformSelection(mousePosition);
                    break;
                case InteractionMode.Action:
                    this.PerformActions(mousePosition);
                    break;
                default:
                    break;
            }
        }

        // Exit the game if the user presses escape key
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void PerformSelection(Vector3 mousePosition)
    {
        

        // Find all hits on selectable objects
        var hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, 0)
            .Where(h => h.transform.HasScriptType<ISelectable>());

        if (hits.Any())
        {
            // Select the first hit
            var selectable = hits.First().transform.GetScriptType<ISelectable>();
            selectable.IsSelected = !selectable.IsSelected;
            this.selectedObjects.Add(selectable);
            this.interactionMode = InteractionMode.Action;
        }
    }

    private void PerformActions(Vector3 mousePosition)
    {
        foreach (var selectedObject in this.selectedObjects.Where(o => o is IActionPerformer))
        {
            ((IActionPerformer)selectedObject).PerformAction(mousePosition);
        }

        // Clear selections and reset to selection mode
        this.selectedObjects.ForEach(s => s.IsSelected = false);
        this.selectedObjects.Clear();
        this.interactionMode = InteractionMode.Selection;
    }
}
