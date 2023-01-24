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
    public GhostNodesStatesEnum respawnState;
    public GhostNodesStatesEnum startGhostNodeState;

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

    public bool testRespawn = false;

    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    public bool leftHomeBefore = false;

    public bool isVisible = true;

    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyesSprite;

    public Animator animator;

    public Color color;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        ghostSprite = GetComponent<SpriteRenderer>();
        //eyesSprite = GetComponentInChildren<SpriteRenderer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            startGhostNodeState = GhostNodesStatesEnum.startNode;
            respawnState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeStart;

        }
        else if (ghostType == GhostType.pink)
        {
            startGhostNodeState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodesStatesEnum.centerNode;

        }
        else if (ghostType == GhostType.blue)
        {
            startGhostNodeState = GhostNodesStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            respawnState = GhostNodesStatesEnum.leftNode;

        }
        else if (ghostType == GhostType.orange)
        {
            startGhostNodeState = GhostNodesStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            respawnState = GhostNodesStatesEnum.rightNode;

        }
        //movementController.currentNode = startingNode;
        //transform.position = startingNode.transform.position;
    }


    public void Setup()
    {
        animator.SetBool("moving", false);

        ghostNodeState = startGhostNodeState;
        readyToLeaveHome = false;

        //Reset our ghost back to their home position
        //Debug.Log("startingNode", startingNode);
        //Debug.Log("currentNode", movementController);
        if (movementController != null)
        {
            movementController.currentNode = startingNode;

        }

        movementController.direction = "";
        movementController.lastMovingDirection = "";

        transform.position = startingNode.transform.position;

        //Set theri scatter node index back to 0
        scatterNodeIndex = 0;

        //Set isFrightened
        isFrightened = false;

        leftHomeBefore = false;

        //Set readyToLeaveHome to be false if there are blue or pink
        if (ghostType == GhostType.red)
        {
            readyToLeaveHome = true;
            leftHomeBefore = true;
        }
        else if (ghostType == GhostType.pink)
        {
            readyToLeaveHome = true;
        }

        SetVisible(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (ghostNodeState != GhostNodesStatesEnum.movingInNodes || !gameManager.isPowerPelletRunning)
        {
            isFrightened = false;
        }

        if (!gameManager.isPowerPelletRunning)
        {
            isFrightened = false;
        }

        if (isVisible)
        {
            if (ghostNodeState != GhostNodesStatesEnum.respawning)
            {
                ghostSprite.enabled = true;

            }
            else
            {
                ghostSprite.enabled = false;

            }
            eyesSprite.enabled = true;
        }
        else
        {
            ghostSprite.enabled = false;
            eyesSprite.enabled = false;
        }

        if (isFrightened)
        {
            animator.SetBool("frightened", true);
            eyesSprite.enabled = false;
            ghostSprite.color = new Color(255, 255, 255, 255);
        }
        else
        {
            animator.SetBool("frightened", false);
            animator.SetBool("frightenedBlinking", false);

            ghostSprite.color = color;
        }
        animator.SetBool("moving", true);

        if (!gameManager.gameIsRunning)
        {
            return;
        }

        if (gameManager.powrPelletTimer - gameManager.currentPowerPelletTime <= 3)
        {
            animator.SetBool("frightenedBlinking", true);
        }
        else
        {
            animator.SetBool("frightenedBlinking", false);

        }

        if (testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodesStatesEnum.respawning;
            testRespawn = false;
        }

        if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
        {
            movementController.SetSpeed(1);
        }
        else
        {
            if (isFrightened)
            {
                movementController.SetSpeed(1);
            }
            else if (ghostNodeState == GhostNodesStatesEnum.respawning)
            {
                movementController.SetSpeed(7);
            }
            else
            {
                movementController.SetSpeed(2);
            }
        }
    }

    public void SetFrightened(bool newIsFrightened)
    {
        isFrightened = newIsFrightened;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodesStatesEnum.movingInNodes)
        {
            leftHomeBefore = true;
            //Scatter mode
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                DetermineGhostScatterModeDirection();

            }
            else if (isFrightened)
            {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
            //chase mode
            else
            {
                //Determine next game node to go to
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDiretion();
                }
                else if (ghostType == GhostType.pink)
                {
                    DeterminePinkGhostDiretion();
                }
                else if (ghostType == GhostType.blue)
                {
                    DetermineBlueGhostDiretion();
                }
                else if (ghostType == GhostType.orange)
                {
                    DetermineOrangeGhostDiretion();
                }

            }
        }
        else if (ghostNodeState == GhostNodesStatesEnum.respawning)
        {
            string direction = "";

            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }
            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y)
            {
                if (respawnState == GhostNodesStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodesStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodesStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }
            //If our respawn state is either the left or right, and we got to that node, leave home again
            else if (
                (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y)
                || (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y)
                )
            {
                ghostNodeState = respawnState;
            }
            //We are in the gameboard still, locate our start
            else
            {
                //Determine quickest direction to home
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }


            movementController.SetDirection(direction);
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
                else if (ghostNodeState == GhostNodesStatesEnum.rightNode)
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

    void DetermineGhostScatterModeDirection()
    {
        //If we reached the scatter node, at one of index
        if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
        {
            scatterNodeIndex++;

            if (scatterNodeIndex == scatterNodes.Length - 1)
            {
                scatterNodeIndex = 0;
            }
        }

        string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);

        movementController.SetDirection(direction);

    }
    void DeterminePinkGhostDiretion()
     {
            string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
            float distanceBetweenNodes = 0.35f;

            Vector2 target = gameManager.pacman.transform.position;
            if (pacmansDirection == "left")
            {
                target.x -= distanceBetweenNodes * 2;
            }
            else if (pacmansDirection == "right")
            {
                target.x += distanceBetweenNodes * 2;
            }
            else if (pacmansDirection == "up")
            {
                target.y += distanceBetweenNodes * 2;
            }
            else if (pacmansDirection == "down")
            {
                target.y -= distanceBetweenNodes * 2;
            }

            string direction = GetClosestDirection(target);
            movementController.SetDirection(direction);
     }   
    
       void DetermineBlueGhostDiretion()
       {
            string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
            float distanceBetweenNodes = 0.35f;

            Vector2 target = gameManager.pacman.transform.position;
            if (pacmansDirection == "left")
            {
                target.x -= distanceBetweenNodes * 2;
            }
            else if (pacmansDirection == "right")
            {
                target.x += distanceBetweenNodes * 2;
            }
            else if (pacmansDirection == "up")
            {
                target.y += distanceBetweenNodes * 2;
            }
            else if (pacmansDirection == "down")
            {
                target.y -= distanceBetweenNodes * 2;
            }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);


            string direction = GetClosestDirection(blueTarget);
            movementController.SetDirection(direction);
       }

    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveDown && movementController.lastMovingDirection != "up")
        {
            possibleDirections.Add("down");
        }
        else if (nodeController.canMoveUp && movementController.lastMovingDirection != "down")
        {
            possibleDirections.Add("up");
        }
        else if (nodeController.canMoveRight && movementController.lastMovingDirection != "left")
        {
            possibleDirections.Add("right");
        }
        else if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right")
        {
            possibleDirections.Add("left");
        }

        string direction = "";
        int randomDirectionIndex = Random.Range(0, possibleDirections.Count - 1);
        direction = possibleDirections[randomDirectionIndex];
        return direction;
    }

    void DetermineOrangeGhostDiretion()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 0.35f;

        if (distance < 0)
        {
            distance *= -1;
        }

        // If we are within 8 nodes of pacman, chase him using red's logic
        if (distance <= distanceBetweenNodes * 8)
        {
            DetermineRedGhostDiretion();
        }
        //othwerwise use scatter mode logic
        else
        {
            DetermineGhostScatterModeDirection();
        }
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

    public void SetVisible(bool newIsVisible)
    {
        isVisible = newIsVisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ghostNodeState != GhostNodesStatesEnum.respawning)
        {
            //Get Eaten
            if (isFrightened)
            {
                gameManager.GhostEaten();
                ghostNodeState = GhostNodesStatesEnum.respawning;
            }
            //Eat Player
            else
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }


}
