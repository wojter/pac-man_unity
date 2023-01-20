using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameObject currentNode;
    public float speed = 4f;

    public string direction = "";
    public string lastMovingDirection = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NodeController currenNodeController = currentNode.GetComponent<NodeController>();

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
            //Get the next node from our node controller using our current dirction
            GameObject newNode = currenNodeController.GetNodeFromDirection(direction);
            if (newNode != null)
            {
                currentNode = newNode;
                lastMovingDirection = direction;
            }
            else
            {
                direction = lastMovingDirection;
                newNode = currenNodeController.GetNodeFromDirection(direction);
                if (newNode != null)
                {
                    currentNode = newNode;
                }
            }
        }
    }
    public void SetDirection (string newDirection)
    {
        direction = newDirection;
    }
}
