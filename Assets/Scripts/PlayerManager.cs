using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private List<Player> players;
    private Player selectedPlayer;
    public int PlayerCount = 5;

	// Use this for initialization
	public void Start ()
    {
        var cameraRect = Camera.main.WorldRect();
        Debug.Log(string.Format("Camera Rectangle: [{0},{1}]", cameraRect.width, cameraRect.height));
	}
	
	// Update is called once per frame
	public void Update ()
    {
		
	}

    private void CreatePlayers(int count, Rect area)
    {
        players = new List<Player>();

        for (int i = 0; i < count; i++)
        {
            var playerObject = Instantiate(Resources.Load(@"Prefabs\Player")) as GameObject;
            var player = playerObject.GetComponent<Player>();
            player.MaxSpeed = Random.value + 1;
            SpriteRenderer sprite = playerObject.GetComponent<SpriteRenderer>();
            sprite.color = Random.ColorHSV();
            
            player.transform.position = area.RandomPosition();
            players.Add(player);
        }
    } 
}


