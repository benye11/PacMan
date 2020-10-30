using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergizerPelletScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject RedGhost;
    public GameObject PinkGhost;
    public GameObject CyanGhost;
    public GameObject OrangeGhost;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        RedGhost = GameObject.Find("RedGhost");
        PinkGhost = GameObject.Find("PinkGhost");
        CyanGhost = GameObject.Find("CyanGhost");
        OrangeGhost = GameObject.Find("OrangeGhost");
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "PacMan") {
            RedGhost.SendMessage("TriggerFright");
            PinkGhost.SendMessage("TriggerFright");
            CyanGhost.SendMessage("TriggerFright");
            OrangeGhost.SendMessage("TriggerFright");
        }
    }
}
