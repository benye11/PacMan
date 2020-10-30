using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManControl : MonoBehaviour
{
    //We want to keep track of CurrentNode, TargetNode, and PreviousNode.
    //
    public float speed = 4.0f;
    private Vector2 direction = Vector2.zero;
    //we don't want to tell pacman to move immediately when we press the button, we want to know where pacman is going to move
    //where pacman will move next. whenever we set that button, we set button to button press.
    //when pacman gets to an intersection, it will ask what's my next direction can i move there and if i can i will go
    //then set next direction = direction
    private Vector2 nextDirection; //set netDirection equal to direction based on button pressed and see if we can go somewhere

    private NodeIntersectionScript CurrentNode; //stores what PacMan's current Node is.
    private NodeIntersectionScript TargetNode;
    private NodeIntersectionScript PreviousNode;
    // Start is called before the first frame update
    void Start()
    {
        //this runs after. So if we do speed = 30 here, it overrides inspector given parameters
        NodeIntersectionScript node = GetNodeAtPosition(transform.localPosition);

        if (node != null) { //NOTE: it can be null
            CurrentNode = node;
            Debug.Log("Current Node: " + CurrentNode);
        }
        else {
            Debug.Log("null");
        }

        direction = Vector2.left; //pacman always faces left
        ChangePosition(direction);
    }

    // Update is called once per frame
    void Update()
    {
        //KeyDown keeps track of you holding down button
        //ButtonDown is one time press
        //We can do getAxis but this might be more smooth
        //MoveToNode(direction); must be in if statements, otherwise called every frame regardless of input
        if (Input.GetKeyDown("j")) {
            //direction = Vector2.left;
            transform.localScale = new Vector3(-1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            //MoveToNode(direction);
            ChangePosition(Vector2.left);

        }
        else if (Input.GetKeyDown("k")) {
            //direction = Vector2.right;
            transform.localScale = new Vector3(1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            //MoveToNode(direction);
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown("i")) {
            //direction = Vector2.up;
            transform.localScale = new Vector3(1,1,1); //scale to standard
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            //MoveToNode(direction);
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown("m")) {
            //direction = Vector2.down;
            transform.localScale = new Vector3(1,1,1); //scale to standard
            transform.localRotation = Quaternion.Euler(0, 0, 270);
            //MoveToNode(direction);
            ChangePosition(Vector2.down);
        }
        //Time.deltaTime is how much time has passed since last frame was drawn, so we do this to make it time-dependent instead of frame-dependent
        if (TargetNode != CurrentNode && TargetNode != null) {
            if (OverShotTarget()) {
                CurrentNode = TargetNode;

                transform.localPosition = CurrentNode.transform.position;
                NodeIntersectionScript moveToNode = CanMove(nextDirection);

                if (moveToNode != null) {
                    direction = nextDirection;
                }
                if (moveToNode == null) {
                    moveToNode = CanMove(direction);
                }

                if (moveToNode != null) {
                    TargetNode = moveToNode;
                    PreviousNode = CurrentNode;
                    CurrentNode = null;
                }
                else {
                    direction = Vector2.zero;
                }
            }
            else {
                //continue moving as long as target not overshot
                transform.position += (Vector3)(direction * speed * Time.deltaTime);
            }
        }
    }

    NodeIntersectionScript GetNodeAtPosition (Vector2 pos) {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoardScript>().board[(int)pos.x, (int)pos.y];
        //creates gameobject called tile and grabbing the position

        if (tile != null) { //make sure not null, because null reference error. I don't think this will happen but safety check
            return tile.GetComponent<NodeIntersectionScript>();
        }
        else {
            Debug.Log("null tile");
            return null;
        }
    }

    NodeIntersectionScript CanMove(Vector2 vector) { //it's going to check if there's a valid direction in where we pressed the button
        NodeIntersectionScript moveToNode = null;

        for (int i = 0; i < CurrentNode.neighbors.Length; i++) {
            if (CurrentNode.validDirections[i] == vector) { //if equal to direction
                moveToNode = CurrentNode.neighbors[i];
                break;
            }
        }
        return moveToNode;
    }

    void MoveToNode(Vector2 vector) {
        NodeIntersectionScript moveToNode = CanMove(vector);

        if (moveToNode != null) {
            transform.localPosition = moveToNode.transform.position;
            CurrentNode = moveToNode;
        }
    }

    void ChangePosition(Vector2 vector) {
        //change position to direction we want to move to
        if (vector != direction) {
            nextDirection = vector; //only store if not equal to direction
        }
        
        if (CurrentNode != null) {
            NodeIntersectionScript moveToNode = CanMove(vector);
            if (moveToNode != null) {
                direction = vector;
                TargetNode = moveToNode;
                PreviousNode = CurrentNode;
                CurrentNode = null; //we are traveling in between so technically no current node
            }
        }
    }

    //if nodeToSelf > nodeToTarget, then we overshot. otherwise we can continue going
    bool OverShotTarget() {
        float nodeToTarget = LengthFromNode(TargetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode(Vector2 targetPosition) {
        Vector2 vector = targetPosition - (Vector2)PreviousNode.transform.position;
        return vector.sqrMagnitude;
    }
    
}
