using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePointsScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int points; //initialized at start
    public GameObject Player;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    //NOTE: For this to work, you need BOTH circle collider and rigibody2D on pacman.
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "PacMan") {
            Player.SendMessage("ScorePoints", points);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            points = 0;
        }
    }
}
