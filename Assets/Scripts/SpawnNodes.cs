using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{
    int numToSpawn = 29;

    public float currentSpawnOffset;
    public float spanwOffset = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.name = "Node";
        //return;
        //if (gameObject.name == "Node")
        //{
        //    for (int i=0; i< numToSpawn; i++)
        //    {
        //        //Clone a new node
        //        GameObject clone = Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y + currentSpawnOffset, 0), Quaternion.identity);
        //        currentSpawnOffset += spanwOffset;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
