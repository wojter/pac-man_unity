using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeControler : MonoBehaviour
{
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveUp = false;
    public bool canMoveDown = false;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;

    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D[] hitsDown;
        //Shoot raycast (line) going down
        hitsDown = Physics2D.RaycastAll(transform.position, -Vector2.up);

        //Loop through all of the gameobjet that the raycast hits
        for  (int i = 0; i < hitsDown.Length; i++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.4f)
            {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsUp;
        //Shoot raycast (line) going Up
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        //Loop through all of the gameobjet that the raycast hits
        for (int i = 0; i < hitsUp.Length; i++)
        {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
            if (distance < 0.4f)
            {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsRight;
        //Shoot raycast (line) going Right
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        //Loop through all of the gameobjet that the raycast hits
        for (int i = 0; i < hitsRight.Length; i++)
        {
            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
            if (distance < 0.4f)
            {
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsLeft;
        //Shoot raycast (line) going Left
        hitsLeft = Physics2D.RaycastAll(transform.position, Vector2.left);

        //Loop through all of the gameobjet that the raycast hits
        for (int i = 0; i < hitsLeft.Length; i++)
        {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
            if (distance < 0.4f)
            {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
