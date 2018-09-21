using UnityEngine;

public class Agent : MonoBehaviour, ISelectable
{
    public GameObject SelectionIndicator;
    private SpriteRenderer selectionIndicatorSprite;

    public bool IsSelected { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        this.selectionIndicatorSprite = SelectionIndicator.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        this.selectionIndicatorSprite.enabled = this.IsSelected;
    }
}
