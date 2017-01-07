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
    public static Vector2 RandomPosition(this Rect rect)
    {
        float x = Random.Range(rect.xMin, rect.xMax);
        float y = Random.Range(rect.yMin, rect.yMax);

        return new Vector2(x, y);
    }

    public static Rect WorldRect(this Camera camera)
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = camera.orthographicSize * 2;

        var width = cameraHeight * screenAspect;
        var height = cameraHeight;
        var x = camera.transform.position.x - (width / 2);
        var y = camera.transform.position.y - (height / 2);

        return new Rect(x, y, width, height); ;
    }
}
