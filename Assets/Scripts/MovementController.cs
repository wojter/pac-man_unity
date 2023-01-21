using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject currentNode;
    public float speed = 4f;

    public string direction = "";
    public string lastMovingDirection = "";

    public bool canWarp = true;
    
    // Start is called before the first frame update
    void Awake()
    {
        lastMovingDirection = "left";
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        NodeController currentNodeController = currentNode.GetComponent<NodeController>();

        transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);

        bool reverseDirection = false;
        if (
            (direction == "left" && lastMovingDirection == "right")
            || (direction == "right" && lastMovingDirection == "left")
            || (direction == "up" && lastMovingDirection == "down")
            || (direction == "down" && lastMovingDirection == "up")
           )
        {
            reverseDirection = true;
        }

        //FIgure out if we're at the center of our current node
        if ((transform.position.x == currentNode.transform.position.x && transform.position.y == currentNode.transform.position.y) || reverseDirection)
        {
            //If we reached the center of the left warp, warp to the right warp
            if (currentNodeController.isWrapLeftNode && canWarp)
            {
                currentNode = gameManager.rightWarpNode;
                direction = "left";
                lastMovingDirection = "left";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            //If we rached the center of the right warp, warp to the left warp
            else if (currentNodeController.isWarpRightNode &&canWarp)
            {
                currentNode = gameManager.leftWarpNode;
                direction = "right";
                lastMovingDirection = "right";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            //Otherwise, finth the next node we re going to moving towards
            else
            {
            //Get the next node from our node controller using our current dirction
            GameObject newNode = currentNodeController.GetNodeFromDirection(direction);
            if (newNode != null)
            {
                currentNode = newNode;
                lastMovingDirection = direction;
            }
            else
            {
                direction = lastMovingDirection;
                newNode = currentNodeController.GetNodeFromDirection(direction);
                if (newNode != null)
                {
                    currentNode = newNode;
                }
            }
            }


        } 
        //We aren't in the center of a node
        else
        {
            canWarp = true;
        }
    }
    public void SetDirection (string newDirection)
    {
        direction = newDirection;
    }
}
