using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Football player object
/// </summary>
public class Player : MonoBehaviour
{
    public Vector2 TargetPosition { get; set; }
    public bool IsSelected { get; set; }
    public bool IsActive { get; set; }
    public float MaxSpeed = 2;

    // Use this for initialization
    public void Start()
	{
	    this.TargetPosition = this.transform.position;
        this.IsActive = true;
        this.IsSelected = false;
	}
	
	// Update is called once per frame
	public void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (this.IsActive) this.Move();
    }

    private void Move()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, this.TargetPosition,
            this.MaxSpeed * Time.deltaTime);
    }
}
