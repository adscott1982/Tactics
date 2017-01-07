using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private List<Player> players;
	// Use this for initialization
	public void Start ()
    {
        var cameraRect = Camera.main.OrthographicRect();
        CreatePlayers(5, cameraRect);
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
            player.MaxSpeed = 2f;
            player.transform.position = area.RandomPosition();
            players.Add(player);
        }
    } 
}


