using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BLOCK_ID
{
    AIR = 0,
    DIRT,
    GRASS,
    MARBLE,
    COLUMN_BASE,
    COLUMN_MID,
    COLUMN_TOP,
    NUM_BLOCK_TYPES
}

public class BlockProperties
{
    public BlockProperties(GameObject prefab, bool transparent = false)
    {
        m_prefab = prefab;
        m_isTransparent = transparent;
    }

    public GameObject m_prefab { get; private set; }
    public bool m_isTransparent { get; private set; }
}

public class BlockDatabase : MonoBehaviour
{
    [Header("Dependencies")]
    // Declare different block Prefabs here
    // AIR doesn't get a prefab because it doesn't render
    public GameObject block_dirt;
    public GameObject block_grass;
    public GameObject block_marble;
    public GameObject block_columnBase;
    public GameObject block_columnMid;
    public GameObject block_columnTop;
    //

    private BlockProperties[] blockData;

    // TODO: how to make return of GetBlockPrefab constant? Dont want anything editing the contents
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
        BlockProperties marble = new BlockProperties(block_marble, false);
        BlockProperties columnBase = new BlockProperties(block_columnBase, true);
        BlockProperties columnMid = new BlockProperties(block_columnMid, true);
        BlockProperties columnTop = new BlockProperties(block_columnTop, true);

        blockData = new BlockProperties[(int)BLOCK_ID.NUM_BLOCK_TYPES]
        {
            air,            // 0
            dirt,           // 1
            grass,          // 2
            marble,
            columnBase,     // 3
            columnMid,
            columnTop
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
