using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    [Header("Dependencies")]
    // Declare different block Prefabs here
    public GameObject block_dirt;
    public GameObject block_grass;
    //

    enum BLOCK_ID
    {
        AIR = 0,
        DIRT,
        GRASS,
        NUM_BLOCK_TYPES
    }

    private GameObject[] blockData = new GameObject[256];

    // how to make return of this function constant?
    public GameObject GetBlockData(char blockTypeID)
    {
            return blockData[blockTypeID];
    }

    private void Awake()
    {
        blockData[(int)BLOCK_ID.AIR] = null;            // 0
        blockData[(int)BLOCK_ID.DIRT] = block_dirt;     // 1
        blockData[(int)BLOCK_ID.GRASS] = block_grass;   // 2
    }

    // Use this for initialization
    void Start()
{
  
}

// Update is called once per frame
void Update()
{

}
}
