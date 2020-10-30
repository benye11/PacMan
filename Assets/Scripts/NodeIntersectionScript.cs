using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeIntersectionScript : MonoBehaviour
{
    //Ordering is up, down, left, right
    public NodeIntersectionScript[] neighbors; //neighbors meaning our adjacent pellets
    //up: 0,1
    //down: 0,-1
    //left: -1,0
    //right: 1,0
    public Vector2[] validDirections;

    // Start is called before the first frame update.
    void Start()
    {
        validDirections = new Vector2[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++) {          
            validDirections[i] = (neighbors[i].transform.position - transform.position).normalized;
        }
        
        //dynamically create our relative directions from intersection nodes during runtime
    }

    //NOTE: GameObject.Find is slow. We can store our pellets in array for faster search time
    //Script execution order: sets up the order in which scripts are instantiated
    //We can have GameBoardScript (which stores all our nodes) to load first, then PacMan to load
    //ExecutionOrder means it will run Start() first
}
