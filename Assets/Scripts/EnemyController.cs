using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum GhostNodesStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodesStatesEnum ghostNodeState;

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostType ghostType;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public MovementController movementController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodesStatesEnum.startNode;
            startingNode = ghostNodeStart;
        }
        else if (ghostType == GhostType.pink)
        {
            ghostNodeState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
        }
        else if (ghostType == GhostType.blue)
        {
            ghostNodeState = GhostNodesStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange)
        {
            ghostNodeState = GhostNodesStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;  //fast move ghost to position 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodesStatesEnum.movingInNodes)
        {
            //Determine next game node to go to
            if (ghostType == GhostType.red)
            {
                DetermineRedGhostDiretion();
            }
        }
        else if (ghostNodeState == GhostNodesStatesEnum.respawning)
        {
            //Determine quickest direction to home

        }
        else
        {
            //If we are redy to leave our home
            if (readyToLeaveHome)
            {
                //If we are in the left or right home node, move to the center
                if (ghostNodeState == GhostNodesStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.centerNode;
                    movementController.SetDirection("right");
                }
                else if(ghostNodeState == GhostNodesStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                else if (ghostNodeState == GhostNodesStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.startNode;
                    movementController.SetDirection("up");

                }
                else if (ghostNodeState == GhostNodesStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }

    void DetermineRedGhostDiretion()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }

    void DetermineBluedGhostDiretion()
    {

    }

    void DeterminePinkGhostDiretion()
    {

    }

    void DetermineOrangeGhostDiretion()
    {

    }

    string GetClosestDirection(Vector2 target)
    {
        float shorterstDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        //If we can move up and we aren't reversing
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            //Get the node above us
            GameObject nodeUp = nodeController.nodeUp;
            //Get the distance between our top node, and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);


            //If this is the shortest distance so fa set our direction
            if (distance < shorterstDistance || shorterstDistance == 0)
            {
                shorterstDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            //Get the node above us
            GameObject nodeDown = nodeController.nodeDown;
            //Get the distance between our top node, and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);


            //If this is the shortest distance so fa set our direction
            if (distance < shorterstDistance || shorterstDistance == 0)
            {
                shorterstDistance = distance;
                newDirection = "down";
            }
        }
        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            //Get the node above us
            GameObject nodeLeft = nodeController.nodeLeft;
            //Get the distance between our top node, and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);


            //If this is the shortest distance so fa set our direction
            if (distance < shorterstDistance || shorterstDistance == 0)
            {
                shorterstDistance = distance;
                newDirection = "left";
            }
        }
        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            //Get the node above us
            GameObject nodeRight = nodeController.nodeRight;
            //Get the distance between our top node, and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);


            //If this is the shortest distance so fa set our direction
            if (distance < shorterstDistance || shorterstDistance == 0)
            {
                shorterstDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }

}
