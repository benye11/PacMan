using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScript : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector2 direction;
    private NodeIntersectionScript LeftTeleport;
    private NodeIntersectionScript RightTeleport;
    public NodeIntersectionScript StartNode;
    private NodeIntersectionScript CurrentNode;
    private NodeIntersectionScript TargetNode;
    public NodeIntersectionScript HomeNode; //this is the Node we go back to after being a ghost.
    System.Random randomizer;
    private Animator anim;
    public GameObject Player;
    Queue<NodeIntersectionScript> Path;
    bool Fright;
    // Start is called before the first frame update
    void Start()
    {
        //NOTE: board will be null if we add BlinkyScript to our script order of execution under project settings
        randomizer = new System.Random();
        anim = GetComponent<Animator>();
        Player = GameObject.FindWithTag("Player");
        transform.position = StartNode.transform.position; //this should fix things up
        LeftTeleport = GameObject.Find("pellet_left_teleport").GetComponent<NodeIntersectionScript>();
        RightTeleport = GameObject.Find("pellet_right_teleport").GetComponent<NodeIntersectionScript>();
        Queue<NodeIntersectionScript> Path = new Queue<NodeIntersectionScript>();
        Fright = false;
        CurrentNode = StartNode;
        Debug.Log("BLINKY START Node: " + CurrentNode.name);
        SelectRandomTargetNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (Fright) {
            FrightMode();
        }
        else {
            NormalMode();
        }
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void NormalMode() {
        if (OverShotTarget()) {
            CurrentNode = TargetNode;
            if (CurrentNode == LeftTeleport || CurrentNode == RightTeleport) {
                Teleport(CurrentNode);
            }
            else {
                SelectRandomTargetNode();
            }
        }
    }
    
    bool OverShotTarget() {
        float nodeToTarget = LengthFromNode(TargetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.position);
        return nodeToSelf >= nodeToTarget;
    }

    float LengthFromNode(Vector2 targetPosition) {
        Vector2 vector = targetPosition - (Vector2)CurrentNode.transform.position;
        return vector.sqrMagnitude;
    }

    void SelectRandomTargetNode() {
        int temp = randomizer.Next(0,CurrentNode.neighbors.Length);
        TargetNode = CurrentNode.neighbors[temp];
        direction = CurrentNode.validDirections[temp];
        UpdateAnimation(direction);
    }

    void UpdateAnimation(Vector2 vector) {
        if (vector == Vector2.up) {
            anim.SetInteger("SpriteDirectionFacing", 0);
        }
        else if (vector == Vector2.down) {
            anim.SetInteger("SpriteDirectionFacing", 1);
        }
        else if (vector == Vector2.left) {
            anim.SetInteger("SpriteDirectionFacing", 2);
        }
        else if (vector == Vector2.right) {
            anim.SetInteger("SpriteDirectionFacing", 3);
        }
    }

    void Teleport(NodeIntersectionScript Node) {
        if (Node == LeftTeleport) {
            transform.position = RightTeleport.transform.position;
            CurrentNode = RightTeleport;
            SelectRandomTargetNode();
        }
        else {
            transform.position = LeftTeleport.transform.position;
            CurrentNode = LeftTeleport;
            SelectRandomTargetNode();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Player.SendMessage("TriggerDeath");
        }
    }

    void FrightMode() {
        //Find a Path to HomeNode.
        if (OverShotTarget()) {
            if (Path.Count > 0) {
            Path.Dequeue();
            direction = (TargetNode.transform.position - CurrentNode.transform.position).normalized;
            //transform.position = TargetNode.transform.position; //not necessary
            CurrentNode = TargetNode;
            TargetNode = Path.Peek();
            }
            else {
                Fright = false;
            }
        }
    }

    //player calls this
    void TriggerFright() {
        Queue<NodeIntersectionScript> Path = DepthFirstSearch(TargetNode);
        Fright = true;
    }

    Queue<NodeIntersectionScript> BreadthFirstSearch(NodeIntersectionScript Node) {
        return null;
    }

    Queue<NodeIntersectionScript> DepthFirstSearch(NodeIntersectionScript Node) {
        HashSet<NodeIntersectionScript> Visited = new HashSet<NodeIntersectionScript>();
        Stack<(NodeIntersectionScript, Queue<NodeIntersectionScript>)> TraversalStack = new Stack<(NodeIntersectionScript, Queue<NodeIntersectionScript>)>();
        //Queue<NodeIntersectionScript> NodePath = new Queue<NodeIntersectionScript>();
        TraversalStack.Push((Node, new Queue<NodeIntersectionScript>()));
        while (TraversalStack.Count != 0) {
             (NodeIntersectionScript,Queue<NodeIntersectionScript>) temp = TraversalStack.Pop();
             if (Visited.Contains(temp.Item1)) {
                 continue;
             }
             if (temp.Item1 == HomeNode) {
                 return temp.Item2;
             }
             else {
                 for (int i = 0; i < temp.Item1.neighbors.Length; i++) {
                //It must overshootTarget first.
                Queue<NodeIntersectionScript> NodePath = temp.Item2;
                NodePath.Enqueue(temp.Item1.neighbors[i]);
                TraversalStack.Push((temp.Item1.neighbors[i], NodePath));
                Visited.Add(temp.Item1.neighbors[i]);
        }
             }
        }
        return new Queue<NodeIntersectionScript>();
    }

    string LogOutPut(Vector2 vector) {
        if (vector == Vector2.up) {
            return "up";
        }
        else if (vector == Vector2.down) {
            return "down";
        }
        else if (vector == Vector2.left) {
            return "left";
        }
        else if (vector == Vector2.right) {
            return "right";
        }
        return "not any directional vector";
    }
}
