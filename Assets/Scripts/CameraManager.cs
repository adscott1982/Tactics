using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera AttachedCamera;
    private bool isPanning;
    private Vector2 previousMousePosition;
    private Vector2 currentMousePosition;
    private float zoomDelta;

    /// <summary>
    /// Initialization method.
    /// </summary>
    private void Start ()
    {
        this.isPanning = false;
	}

	/// <summary>
    /// Called every frame.
    /// </summary>
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
        this.currentMousePosition = Input.mousePosition;

        if (!this.isPanning)
        {
            this.previousMousePosition = this.currentMousePosition;
            return;
        }

        Debug.Log($"Mouse X: {this.currentMousePosition.x}\tMouse Y: {this.currentMousePosition.y}");

        var mouseDelta = this.currentMousePosition - this.previousMousePosition;

        var worldDelta = Camera.main.ScreenToWorldPoint(this.previousMousePosition) - Camera.main.ScreenToWorldPoint(this.previousMousePosition + mouseDelta);
        Debug.Log($"Delta X: {mouseDelta.x}\tDeltaY: {mouseDelta.y}");
        this.previousMousePosition = this.currentMousePosition;
        
        this.transform.position += worldDelta;
    }
}
