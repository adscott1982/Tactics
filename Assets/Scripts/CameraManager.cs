using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera AttachedCamera;
    private bool isPanning;
    private Vector2 previousMousePosition;
    private Vector2 currentMousePosition;
    private float zoomDelta;

    // Initialization
    private void Start ()
    {
        this.isPanning = false;
	}

	// Called every frame
    private void Update ()
	{
        this.HandleInputs();
        this.HandlePanning();
        this.HandleZoom();
    }

    private void HandleZoom()
    {
        var worldMouse = Camera.main.ScreenToWorldPoint(this.currentMousePosition);
        var worldRect = Camera.main.WorldRect();
        
        // If the mouse is inside the camera rectangle, perform the zoom
        if (worldRect.Contains(worldMouse))
        {
            Camera.main.orthographicSize *= 1 + (this.zoomDelta / 10);
        }
        
    }

    private void HandleInputs()
    {
        // If starting panning set previous to current mouse position to prevent jump
        if (Input.GetMouseButtonDown(1))
        {
            this.isPanning = true;
            this.currentMousePosition = Input.mousePosition;
            this.previousMousePosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            this.isPanning = false;
        }

        this.zoomDelta = -Input.GetAxis("Mouse ScrollWheel");
    }

    private void HandlePanning()
    {
        if (!this.isPanning)
        {
            return;
        }

        this.currentMousePosition = Input.mousePosition;

        var mouseDelta = this.currentMousePosition - this.previousMousePosition;
        var worldDelta = Camera.main.ScreenToWorldPoint(this.previousMousePosition) - Camera.main.ScreenToWorldPoint(this.previousMousePosition + mouseDelta);

        this.transform.position += worldDelta;
        this.previousMousePosition = this.currentMousePosition;
    }
}
