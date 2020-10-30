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
    Stack<NodeIntersectionScript> Path;
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
        Path = new Stack<NodeIntersectionScript>();
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
        //check for fright might not be necessary since we turn off collider
        if (other.tag == "Player" && Fright == true) {
            Player.SendMessage("TriggerDeath");
        }
    }

    void FrightMode() {
        //Debug.Log("IN FRIGHT MODE");
        //Find a Path to HomeNode.
        if (Path.Count != 0 && OverShotTarget()) {
            Debug.Log("[FRIGHTMODE]: OVERSHOT TARGET");
            Debug.Log("DEQUEUED " + Path.Pop().name);
            Debug.Log("[FRIGHTMODE] CurrentNode: " + CurrentNode.name);
            if (TargetNode == null) {
                Debug.Log("[FRIGHTMODE] TargetNode is null");
                //this means we're already at HomeNode.
                //this shouldn't happen. There should always be a targetNode
            }
            else {
                //Debug.Log("TargetNode: " + TargetNode.name);
                CurrentNode = TargetNode;
                if (CurrentNode == LeftTeleport || CurrentNode == RightTeleport) {
                    Teleport(CurrentNode);
                }
                if (Path.Count != 0) {
                    TargetNode = Path.Peek();
                    direction = (TargetNode.transform.position - CurrentNode.transform.position).normalized;
                }
                else if (Path.Count == 0) {
                    //this should mean we are at HomeNode now. Then we can exit out of the house.
                    TargetNode = CurrentNode.neighbors[0]; //top_middle
                    direction = Vector2.up;
                    Fright = false;
                    anim.SetBool("FrightMode", false);
                    anim.SetInteger("SpriteDirectionFacing", 0);
                    GetComponent<Collider2D>().enabled = true;
                }
            }
        }
        //Debug.Log("[FRIGHTMODE] Have not OverShot yet");
    }

    //player calls this
    //NOTE: INITIALIZING A STACK WITH ANOTHER STACK ACTUALLY PUTS IT IN REVERSE ORDER.
    void TriggerFright() {
        Fright = true;
        anim.SetBool("FrightMode", true);
        GetComponent<Collider2D>().enabled = false;
        Debug.Log("Started with TargetNode: " + TargetNode.name);
        Path = new Stack<NodeIntersectionScript>(BreadthFirstSearch(TargetNode));
        int i = 0;
        //Debug.Log("[TriggerFright] first node: " + Path.Peek().name);
        foreach (NodeIntersectionScript Node in Path) {
            if (Node == HomeNode) {
                Debug.Log("#" + i.ToString() + ": This is HomeNode: " + Node.name);
            }
            else {
                Debug.Log("#" + i.ToString() + ": Path Node for " + gameObject.name + ": " + Node.name);
            }
            i++;
        }
    }

    //working. Use this for scatter and AI behavior
    Stack<NodeIntersectionScript> BreadthFirstSearch(NodeIntersectionScript Node) {
        if (HomeNode == Node) {
            return null;
        }
        //node, prevnode, stack.
        Stack<NodeIntersectionScript> temp1 = new Stack<NodeIntersectionScript>();
        temp1.Push(HomeNode);
        Queue<(NodeIntersectionScript, NodeIntersectionScript, Stack<NodeIntersectionScript>)> TraversalQueue = new Queue<(NodeIntersectionScript, NodeIntersectionScript, Stack<NodeIntersectionScript>)>();
        //HashSet<NodeIntersectionScript> Visited = new HashSet<NodeIntersectionScript>();
        TraversalQueue.Enqueue((HomeNode, null, temp1));
        while (TraversalQueue.Count > 0) {
            (NodeIntersectionScript, NodeIntersectionScript, Stack<NodeIntersectionScript>) temp = TraversalQueue.Dequeue();
            NodeIntersectionScript tempNode = temp.Item1;
            NodeIntersectionScript prevNode = temp.Item2;
            Stack<NodeIntersectionScript> currPath = new Stack<NodeIntersectionScript>(temp.Item3);
            if (tempNode == Node) {
                return currPath;
            }
            for (int i = 0; i < tempNode.neighbors.Length; i++) {
                if (tempNode.neighbors[i] != prevNode) {
                    Stack<NodeIntersectionScript> currPathNext = new Stack<NodeIntersectionScript>(currPath);
                    currPathNext.Push(tempNode.neighbors[i]);
                    TraversalQueue.Enqueue((tempNode.neighbors[i], tempNode, currPathNext));
                }
            }
        }
        Debug.Log("returning null");
        return null;
    }

    //use this for AI behavior
    Stack<NodeIntersectionScript> DepthFirstSearch(NodeIntersectionScript Node) {
        if (Node == TargetNode) { //check this.
            return null;
        }
        //HashSet<NodeIntersectionScript> Visited = new HashSet<NodeIntersectionScript>();
        //Stack<NodeIntersectionScript> TraversalStack = new Stack<NodeIntersectionScript>();
        Queue<(NodeIntersectionScript, Stack<NodeIntersectionScript>, HashSet<NodeIntersectionScript>)> TraversalQueue = new Queue<(NodeIntersectionScript, Stack<NodeIntersectionScript>, HashSet<NodeIntersectionScript>)>();
        TraversalQueue.Enqueue((HomeNode, new Stack<NodeIntersectionScript>(), new HashSet<NodeIntersectionScript>()));
        while (TraversalQueue.Count > 0) {
            (NodeIntersectionScript, Stack<NodeIntersectionScript>, HashSet<NodeIntersectionScript>) temp = TraversalQueue.Dequeue();
            HashSet<NodeIntersectionScript> Visited = new HashSet<NodeIntersectionScript>(temp.Item3);
            Debug.Log("Currently at: " + temp.Item1.name);
            if (Visited.Contains(temp.Item1)) {
                continue;
            }
            Stack<NodeIntersectionScript> TraversalStack = new Stack<NodeIntersectionScript>(temp.Item2); //flipped.
            if (temp.Item1 == Node) {
                TraversalStack.Push(Node);
                return TraversalStack;
            }
            Visited.Add(temp.Item1);
            TraversalStack.Push(temp.Item1); //we pop. then say we have visited node.
            NodeIntersectionScript[] Neighbors = temp.Item1.neighbors;
            for (int i = 0; i < Neighbors.Length; i++) {
                Debug.Log(temp.Item1.name +  "'s neighbors: " + Neighbors[i].name);
                if (Visited.Contains(Neighbors[i]) == false) {
                    if (Neighbors[i] == Node) {
                        Stack<NodeIntersectionScript> FinalStack = new Stack<NodeIntersectionScript>(TraversalStack); //re-flip.
                        FinalStack.Push(Node);
                        return FinalStack;
                    }
                    Stack<NodeIntersectionScript> TempStack = new Stack<NodeIntersectionScript>(TraversalStack); //re-flip.
                    TempStack.Push(Neighbors[i]);
                    TraversalQueue.Enqueue((Neighbors[i], TempStack, Visited));
                }
            }
        }
        return null;
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
