using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static bool HasFocus { get; private set; }

    public static bool JustRegainedFocus { get; private set; }

    public static bool HadFocusLastFrame { get; private set; }

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
        if (HasFocus != HadFocusLastFrame)
        {
            HadFocusLastFrame = HasFocus;
            JustRegainedFocus = true;
        }
        else
        {
            JustRegainedFocus = false;
        }
    }
}
