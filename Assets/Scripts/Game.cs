using UnityEngine;

/// <summary>Class containing information about the current application process.</summary>
public class Game : MonoBehaviour
{
    /// <summary>Whether the application has focus.</summary>
    public static bool HasFocus { get; private set; }

    // Called when the application gains or loses focus
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            HasFocus = false;
        }
        else
        {
            HasFocus = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        HasFocus = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
