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

//public enum BLOCK_PROPERTY_FLAGS
//{
//
//}


public class BlockDatabase : MonoBehaviour
{
    [Header("Dependencies")]
    // Declare different block Prefabs here
    // AIR doesn't get a prefab because it doesn't render
   // public GameObject block_dirt;
   // public GameObject block_grass;
   // public GameObject block_marble;
   // public GameObject block_columnBase;
   // public GameObject block_columnMid;
   // public GameObject block_columnTop;
    //
    //public AirBlockProperties           air;
    //public DirtBlockProperties          dirt;
    //public GrassBlockProperties         grass;
    //public MarbleBlockProperties        marble;
    //public ColumnBaseBlockProperties    columnBase;
    //public ColumnMidBlockProperties     columnMid;
    //public ColumnTopBlockProperties     columnTop;


    private BlockProperties[] blockData;

    // TODO: how to make return of GetBlockPrefab constant? Dont want anything editing the contents
    public GameObject GetBlockPrefab(byte blockTypeID)
    {
        return blockData[blockTypeID].m_prefab;
    }

    public bool IsTransparent(BLOCK_ID type)
    {
        return blockData[(int)type].m_isTransparent;
    }

    public BlockProperties GetProperties(BLOCK_ID type)
    {
        return blockData[(int)type];
    }

    private void Awake()
    {
        //BlockProperties air = new BlockProperties(null, true, false);
        //BlockProperties dirt = new BlockProperties(block_dirt, false);
        //BlockProperties grass = new BlockProperties(block_grass, false);
        //BlockProperties marble = new BlockProperties(block_marble, false);
        //BlockProperties columnBase = new BlockProperties(block_columnBase, true);
        //BlockProperties columnMid = new BlockProperties(block_columnMid, true);
        //BlockProperties columnTop = new BlockProperties(block_columnTop, true);

        blockData = new BlockProperties[(int)BLOCK_ID.NUM_BLOCK_TYPES]
        {
            //air,            // 0
            //dirt,           // 1
            //grass,          // 2
            //marble,
            //columnBase,     // 3
            //columnMid,
            //columnTop

            GetComponent<AirBlockProperties         >(),
            GetComponent<DirtBlockProperties        >(),
            GetComponent<GrassBlockProperties       >(),
            GetComponent<MarbleBlockProperties      >(),
            GetComponent<ColumnBaseBlockProperties  >(),
            GetComponent<ColumnMidBlockProperties   >(),
            GetComponent<ColumnTopBlockProperties   >()
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
