using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
}

/// <summary>
/// Returns a random Vector2 position within the bounds of the rectangle.
/// </summary>
public static class CameraExtensions
{
    /// <summary>
    /// Get a random position within a given rectangle.
    /// </summary>
    /// <param name="rect">The rectangle in which to find a random position.</param>
    /// <returns>The <see cref="Vector2"/> containing the position coordinates.</returns>
    public static Vector2 RandomPosition(this Rect rect)
    {
        float x = Random.Range(rect.xMin, rect.xMax);
        float y = Random.Range(rect.yMin, rect.yMax);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Gets a <see cref="Rect"/> in world space for the given orthographic camera.
    /// </summary>
    /// <param name="orthoCamera">The orthographic camera.</param>
    /// <returns>The <see cref="Rect"/> representing the orthographic camera rectangle in world space.</returns>
    public static Rect WorldRect(this Camera orthoCamera)
    {
        if (!orthoCamera.orthographic)
        {
            throw new System.ArgumentException("Camera is not orthographic.");
        }

        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = orthoCamera.orthographicSize * 2;

        var width = cameraHeight * screenAspect;
        var height = cameraHeight;
        var x = orthoCamera.transform.position.x - (width / 2);
        var y = orthoCamera.transform.position.y - (height / 2);

        return new Rect(x, y, width, height); ;
    }
}
