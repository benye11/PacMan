using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//currentNode, MoveToNode.
//once we go a direction, we are moving to a neighbor.
//function to keep track of us hitting the neighbor
//capture previous direction.
//need Overshot function to keep track if we overshot.

//the issue is that we need to SELECT/SET A DIRECTION SUCH THAT:
//Until we reach TargetNode or reverse, we cannot do any other directions.
//Currently, our direction is just influenced by our node.

public class PacManScript : MonoBehaviour
{
    //We want to keep track of CurrentNode, TargetNode, and PreviousNode.
    //public GUISkin skin; //this thing crashed my unity ):
    public float speed = 4.0f;
    private Vector2 direction;
    private Vector2 oppositeDirection;
    public NodeIntersectionScript StartNode;
    public NodeIntersectionScript CurrentNode;
    public NodeIntersectionScript TargetNode;
    private Vector2[] AvailableDirections;
    private Vector2 nextDirection; //you can make input directions in advance.
    private string oppositeDirectionOutput;
    private int score;
    private int highscore;
    public Text CurrentScore;
    public Text Lives;
    private int lives;
    public Animator anim;
    private NodeIntersectionScript LeftTeleport;
    private NodeIntersectionScript RightTeleport;
    private int pelletcount;
    public GameManager gameManager;
    public GameObject GameOverObject;
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (StoreValues.level < 5) {
            lives = 3;
        }
        else if (StoreValues.level > 10) {
            lives = 1;
        }
        else if (StoreValues.level > 5) {
            lives = 2;
        }
        Lives.text = "Lives\n" + lives.ToString() + "UP";
        anim = GetComponent<Animator>();
        pelletcount = 0;
        CurrentScore.text = "Current Score" + "\n" + "0";
        score = 0;
        direction = Vector2.zero;
        oppositeDirection = Vector2.zero;
        LeftTeleport = GameObject.Find("pellet_left_teleport").GetComponent<NodeIntersectionScript>();
        RightTeleport = GameObject.Find("pellet_right_teleport").GetComponent<NodeIntersectionScript>();
        CurrentNode = StartNode;//GameObject.Find("pellet (59)").GetComponent<NodeIntersectionScript>();
        Debug.Log("started with pellet: " + CurrentNode.name);
        TargetNode = null;
        transform.localScale = new Vector3(-1,1,1);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void LockInDirection(Vector2 vector) {
        direction = vector;
        oppositeDirection = vector * (-1);
    }
    
    void Teleport(NodeIntersectionScript Node) {
        if (Node == LeftTeleport) {
            transform.position = RightTeleport.transform.position;
            CurrentNode = RightTeleport;
            TargetNode = MoveToTargetNode(Vector2.left);
            direction = Vector2.left;
            oppositeDirection = Vector2.right;
            ChangeOrientation(Vector2.left);
        }
        else {
            transform.position = LeftTeleport.transform.position;
            CurrentNode = LeftTeleport;
            TargetNode = MoveToTargetNode(Vector2.right);
            direction = Vector2.right;
            oppositeDirection = Vector2.left;
            ChangeOrientation(Vector2.right);
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if (GameOverObject.activeInHierarchy == true) {
            if (Input.GetKeyDown("y")) {
                gameManager.SendMessage("RestartGame");
                SceneManager.LoadScene("SampleScene");
            }
            else if (Input.GetKeyDown("n")) {
                Application.Quit();
            }
        }
        Move();
        if (TargetNode != CurrentNode && TargetNode != null) {
            if (OverShotTarget()) {
                //unlock our Lock.
                //Debug.Log("Current Node: " + CurrentNode.name);
                //Debug.Log("Hit Target Node " + TargetNode.name);
                CurrentNode = TargetNode;
                //Debug.Log("New current Node: " + CurrentNode.name);
                transform.localPosition = CurrentNode.transform.position;
                if (CurrentNode == LeftTeleport || CurrentNode == RightTeleport) {
                    Teleport(CurrentNode);
                }
                else {
                //the issue might be that we update currentNode and not TargetNode yet.
                Vector2 tempDirection = nextDirection;
                NodeIntersectionScript moveToNode = MoveToTargetNode(tempDirection); //bruhhh.
                if (moveToNode == null) {
                    tempDirection = direction;
                    moveToNode = MoveToTargetNode(direction);
                    oppositeDirection = direction * (-1); //if 0, then 0. //adding this fixed everything
                    if (moveToNode == null) {
                        direction = Vector2.zero;
                        nextDirection = Vector2.zero; //might not be necessary
                        oppositeDirection = Vector2.zero; //might not be necessary
                        TargetNode = null;
                    }
                    else {
                        //Debug.Log("New Target Node");
                        TargetNode = moveToNode;
                    }
                }
                else {
                    direction = nextDirection;
                    oppositeDirection = direction * (-1); //adding this fixed everything
                    TargetNode = moveToNode;
                }
                if (tempDirection != null) {
                    ChangeOrientation(tempDirection);
                }
                /*
                if (moveToNode == null) {
                    Debug.Log("can't go in this direction");
                    //transform.localPosition = CurrentNode.transform.position;
                    direction = Vector2.zero;
                    oppositeDirection = Vector2.zero;
                    nextDirection = Vector2.zero; //will this break anything? nope.
                    TargetNode = null;
                }
                else {
                    TargetNode = moveToNode;
                }*/
            }
            }
        }  
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void PelletsScorePoints(int points) {
        score += points;
        gameManager.SendMessage("UpdateHighScore", score);
        CurrentScore.text = "Current Score" + "\n" + score.ToString();
        pelletcount += 1;
        //Debug.Log("pelletcount: " + pelletcount.ToString()); //we have 233 pellets.
        if (pelletcount >= 233) {
            StartNewLevel(); //test
        }
    }

    void OtherScorePoints(int points) {
        score += points;
        gameManager.SendMessage("UpdateHighScore", score);
        CurrentScore.text = "Current Score" + "\n" + score.ToString();
    }

    void Move() {
        if (Input.GetKeyDown("i")) {
            nextDirection = Vector2.up;
                if (Vector2.up == oppositeDirection) {
                    ReverseDirection(Vector2.up);
                    ChangeOrientation(Vector2.up);
                }
            if (TargetNode == null) {
                TargetNode = MoveToTargetNode(Vector2.up);
                if (TargetNode) {
                    LockInDirection(Vector2.up);
                    ChangeOrientation(Vector2.up);
                }
            }
        }
        else if (Input.GetKeyDown("m")) {
            nextDirection = Vector2.down;
                if (Vector2.down == oppositeDirection) {
                    ReverseDirection(Vector2.down);
                    ChangeOrientation(Vector2.down);
                }
            if (TargetNode == null) {
                TargetNode = MoveToTargetNode(Vector2.down);
                if (TargetNode) {
                    LockInDirection(Vector2.down);
                    ChangeOrientation(Vector2.down);
                }
            }
        }
        else if (Input.GetKeyDown("j")) {
            nextDirection = Vector2.left;
                if (Vector2.left == oppositeDirection) {
                    ReverseDirection(Vector2.left);
                    ChangeOrientation(Vector2.left);
                }
            if (TargetNode == null) {
                TargetNode = MoveToTargetNode(Vector2.left);
                if (TargetNode) {
                    LockInDirection(Vector2.left);
                    ChangeOrientation(Vector2.left);
                }
            }
        }
        else if (Input.GetKeyDown("k")) {
            nextDirection = Vector2.right;
                if (Vector2.right == oppositeDirection) {
                    ReverseDirection(Vector2.right);
                    ChangeOrientation(Vector2.right);
                }
            if (TargetNode == null) {
                TargetNode = MoveToTargetNode(Vector2.right);
                if (TargetNode) {
                    LockInDirection(Vector2.right);
                    ChangeOrientation(Vector2.right);
                }
            }
        }
    }

    void ReverseDirection(Vector2 vector) {
        NodeIntersectionScript temp = TargetNode;
        TargetNode = CurrentNode;
        CurrentNode = temp;
        oppositeDirection = direction;
        direction = vector;
    }

    void ChangeOrientation(Vector2 vector) {
        if (vector == Vector2.up) {
            transform.localScale = new Vector3(1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            //Debug.Log("change orientation to be up");
        }
        else if (vector == Vector2.down) {
            transform.localScale = new Vector3(1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
            //Debug.Log("change orientation to be down");
        }
        else if (vector == Vector2.left) {
            transform.localScale = new Vector3(-1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            //Debug.Log("change orientation to be left");
        }
        else if (vector == Vector2.right) {
            transform.localScale = new Vector3(1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            //Debug.Log("change orientation to be right");
        }
    }

    NodeIntersectionScript MoveToTargetNode(Vector2 vector) {
        NodeIntersectionScript moveToNode = null;

        for (int i = 0; i < CurrentNode.neighbors.Length; i++) {
            if (CurrentNode.validDirections[i] == vector) { //if equal to direction
                moveToNode = CurrentNode.neighbors[i];
                break;
            }
        }
        return moveToNode;
    }

    //we should have a MOVING STATE. and PREVIOUS STATE. You can only switch ONCE.
    bool OverShotTarget() {
        float nodeToTarget = LengthFromNode(TargetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);
        return nodeToSelf >= nodeToTarget;
    }

    float LengthFromNode(Vector2 targetPosition) {
        Vector2 vector = targetPosition - (Vector2)CurrentNode.transform.position;
        return vector.sqrMagnitude;
    }

    void TriggerDeath() {
        lives -= 1;
        Lives.text = "Lives\n" + lives.ToString() + "UP";
        direction = Vector2.zero;
        oppositeDirection = Vector2.zero;
        anim.SetBool("DeathTrigger", true);
        Invoke("RenderInvisible", 2f);
    }

    void RenderInvisible() {
        anim.SetBool("DeathTrigger", false);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        if (lives <= 0) {
            GameOver();
        }
        Invoke("Respawn", 1.5f);
    }

    void Respawn() {
            //anim.SetBool("DeathTrigger", false);
            transform.position = StartNode.transform.position;
            CurrentNode = StartNode;
            TargetNode = null;
            transform.localScale = new Vector3(-1,1,1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            direction = Vector2.zero;
            oppositeDirection = Vector2.zero;
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            //anim.enabled = true;
    }

    void StartNewLevel() {
        gameManager.SendMessage("StartLevel");
        SceneManager.LoadScene("SampleScene");
    }
    
    void GameOver() {
        CancelInvoke(); //cancel all invoke statements and respawn
        GameOverObject.SetActive(true);
        CurrentScore.enabled = false;
        Lives.enabled = false;
    }

}