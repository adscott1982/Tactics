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
	
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            this.isPanning = false;
        }

        this.previousMousePosition = this.currentMousePosition;
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
        Camera.main.orthographicSize *= 1 + (this.zoomDelta / 10);
    }

    private void HandleInputs()
    {
        if (Input.GetMouseButton(1))
        {
            this.isPanning = true;
        }
        else
        {
            this.isPanning = false;
        }

        this.zoomDelta = -Input.GetAxis("Mouse ScrollWheel");
    }



    private void HandlePanning()
    {
        this.currentMousePosition = Input.mousePosition;

        Debug.Log(string.Format("Mouse X: {0}\tMouse Y: {1}", this.currentMousePosition.x, this.currentMousePosition.y));

        if (!this.isPanning)
        {
            this.previousMousePosition = this.currentMousePosition;
            return;
        }

        var mouseDelta = this.currentMousePosition - this.previousMousePosition;

        var worldDelta = Camera.main.ScreenToWorldPoint(this.previousMousePosition) - Camera.main.ScreenToWorldPoint(this.previousMousePosition + mouseDelta);
        Debug.Log(string.Format("Delta X: {0}\tDeltaY: {1}", mouseDelta.x, mouseDelta.y));
        this.previousMousePosition = this.currentMousePosition;
        
        this.transform.position += worldDelta;
    }
}
