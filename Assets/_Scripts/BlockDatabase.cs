using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BLOCK_ID
{
    AIR = 0,
    DIRT,
    GRASS,
    NUM_BLOCK_TYPES
}

public class BlockProperties
{
    public BlockProperties(GameObject prefab, bool transparent = false)
    {
        m_prefab = prefab;
        m_isTransparent = transparent;
    }

    public GameObject m_prefab;
    public bool m_isTransparent = false;
}

public class BlockDatabase : MonoBehaviour
{
    [Header("Dependencies")]
    // Declare different block Prefabs here
    // AIR doesn't get a prefab because it doesn't render
    public GameObject block_dirt;
    public GameObject block_grass;
    //

    private BlockProperties[] blockData;

    // how to make return of this function constant?
    public GameObject GetBlockPrefab(char blockTypeID)
    {
        return blockData[blockTypeID].m_prefab;
    }

    public bool IsTransparent(BLOCK_ID type)
    {
        return blockData[(int)type].m_isTransparent;
    }

    private void Awake()
    {
        BlockProperties air = new BlockProperties(null, true);
        BlockProperties dirt = new BlockProperties(block_dirt, false);
        BlockProperties grass = new BlockProperties(block_grass, false);

        blockData = new BlockProperties[(int)BLOCK_ID.NUM_BLOCK_TYPES]
        {
            air,    // 0
            dirt,   // 1
            grass   // 2
        };
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
