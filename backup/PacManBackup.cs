using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//currentNode, MoveToNode.
//once we go a direction, we are moving to a neighbor.
//function to keep track of us hitting the neighbor
//capture previous direction.
//need Overshot function to keep track if we overshot.

public class PacManBackup : MonoBehaviour
{
    //We want to keep track of CurrentNode, TargetNode, and PreviousNode.
    //
    public float speed = 4.0f;
    private Vector2 direction;
    private NodeIntersectionScript CurrentNode;
    private Dictionary<(int, int), NodeIntersectionScript> board;
    private Vector2[] AvailableDirections;
    private int directionInteger; //to make things faster. If -1, then we aren't moving
    void Start()
    {
        direction = Vector2.zero;
        directionInteger = -1;
        board = GameObject.Find("Game").GetComponent<GameBoardScript>().board;
        CurrentNode = board[((int)transform.position.x, (int)transform.position.y)]; //initialize
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //CheckCurrentNode(transform.position); //might need to change to hashtable so no index errors. 
        Move();
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void Move() {
        if (Input.GetKeyDown("j")) {
            if (canMove(2)) {
                direction = Vector2.left;
                transform.localScale = new Vector3(-1,1,1);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                directionInteger = 2;
            }
        }
        else if (Input.GetKeyDown("k")) {
            if (canMove(3)) {
                direction = Vector2.right;
                transform.localScale = new Vector3(1,1,1);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                directionInteger = 3;
            }
        }
        else if (Input.GetKeyDown("i")) {
            if (canMove(0)) {
                direction = Vector2.up;
                transform.localScale = new Vector3(1,1,1);
                transform.localRotation = Quaternion.Euler(0, 0, 90);
                directionInteger = 0;
            }
        }
        else if (Input.GetKeyDown("m")) {
            if (canMove(1)){
                direction = Vector2.down;
                transform.localScale = new Vector3(1,1,1);
                transform.localRotation = Quaternion.Euler(0, 0, 270);
                directionInteger = 1;
            }
        }
    }

    bool canMove(int num) {
        if (AvailableDirections[num] != Vector2.zero) {
            return true;
        }
        return false;
    }

    void updateDirections() {
        AvailableDirections = CurrentNode.validDirections;
        string temp = "";
        if (AvailableDirections[0] == Vector2.zero) {
            temp += "up: no ";
        }
        else {
            temp += "up: " + AvailableDirections[0].magnitude.ToString() + " ";
        }
        if (AvailableDirections[1] == Vector2.zero) {
            temp += "down: no ";
        }
        else {
            temp += "down: " + AvailableDirections[1].magnitude.ToString() + " ";
        }
        if (AvailableDirections[2] == Vector2.zero) {
            temp += "left: no ";
        }
        else {
            temp += "left: " + AvailableDirections[2].magnitude.ToString() + " ";
        }
        if (AvailableDirections[3] == Vector2.zero) {
            temp += "right: no";
        }
        else {
            temp += "right: " + AvailableDirections[3].magnitude.ToString() + " ";
        }
        Debug.Log("directions available: " + temp);
        if (directionInteger == -1) {
            direction = Vector2.zero;
        }
        else {
        //now check direction. if direction isn't in any of the validDirections
        //then set direction = zero so we don't get out of bounds.
            if (AvailableDirections[directionInteger] == Vector2.zero) {
                direction = Vector2.zero;
                directionInteger = -1;
            }
        }
    }

    //This checks the current node we have passed which will give us the available direction
    void CheckCurrentNode(Vector2 pos) {
        //check and updates our valid directions
        if (board.ContainsKey(((int)pos.x, (int)pos.y))) {
            if (CurrentNode == board[((int)pos.x, (int)pos.y)]) {
                return;
            }
            CurrentNode = board[((int)pos.x, (int)pos.y)];
            transform.position = CurrentNode.transform.position;
            //check current direction. If we are at a node
            updateDirections();
            Debug.Log("we are at: " + CurrentNode.name + " and coords x: " + ((int)pos.x).ToString() + ", y: " + ((int)pos.y).ToString());
        }
    }

    //we should have a MOVING STATE. and PREVIOUS STATE. You can only switch ONCE.
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