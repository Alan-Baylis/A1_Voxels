using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Like a Vector3, but with integer members instead of floats. Used for voxel world array accesses
/// </summary>
public struct IntPos
{
    public IntPos(Vector3 vecPos)
    {
        x = (int)vecPos.x;
        y = (int)vecPos.y;
        z = (int)vecPos.z;
    }

    public IntPos(int a_x, int a_y, int a_z)
    {
        x = a_x;
        y = a_y;
        z = a_z;
    }

    public int x;
    public int y;
    public int z;
    public Vector3 Vec3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class WorldController : MonoBehaviour
{

    [Header("Dependencies")]
    public BlockDatabase blockDatabase;
    public GameObject player;
    public GameObject spawnPoint;

    [Header("World Size")]
    public int width;
    public int height;
    public int depth;

    [Header("Generator Settings")]
    public float MinPower = 16.0f;
    public float MaxPower = 24.0f;

    // Private Instance Variables
    private byte[,,] _blocks;
    private GameObject[,,] _blockArray;

    public GameObject theWorld;


    // public read-only properties
    public byte[,,] Blocks
    {
        get
        {
            return this._blocks;
        }

        set
        {
            this._blocks = value;
        }
    }

    public GameObject[,,] BlockArray
    {
        get
        {
            return this._blockArray;
        }

        set
        {
            this._blockArray = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        // set up world reference so Block Commands can use it
        BlockCommand.s_world = this;

        Regenerate(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Regenerate(false);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MakeAllAir();
        }
    }

    public byte GetBlockID(IntPos blockPos)
    {
        return _blocks[blockPos.x, blockPos.y, blockPos.z];
    }

    /// <summary>
    /// Place a block into the world, will NOT place air blocks
    /// </summary>
    /// <param name="blockTypeID"></param>
    /// <param name="blockPos"></param>
    /// <returns> 
    /// true if the operation succeeded 
    /// </returns>
    public bool PlaceBlock(byte blockTypeID, Vector3 blockPos)
    {
        return PlaceBlock(blockTypeID, new IntPos(blockPos));
    }

    /// <summary>
    /// Place a block into the world, will NOT place air blocks
    /// </summary>
    /// <param name="blockTypeID"></param>
    /// <param name="blockPos"></param>
    /// <returns> 
    /// true if the operation succeeded 
    /// </returns>
    public bool PlaceBlock(byte blockTypeID, IntPos blockPos)
    {
        return PlaceBlock(blockTypeID, blockPos.x, blockPos.y, blockPos.z);
    }

    /// <summary>
    /// Place a block into the world, will NOT place air blocks
    /// </summary>
    /// <param name="blockTypeID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool PlaceBlock(byte blockTypeID, int x, int y, int z)
    {
        bool opResult = false;

        try
        {
            if (blockTypeID != (byte)BLOCK_ID.AIR && _blocks[x, y, z] == (byte)BLOCK_ID.AIR)
            {
                _blocks[x, y, z] = blockTypeID;
                _blockArray[x, y, z] = (Instantiate(blockDatabase.GetBlockPrefab(blockTypeID), new Vector3(x, y, z), Quaternion.identity));

                _blockArray[x, y, z].transform.SetParent(theWorld.transform);

                opResult = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: PlaceBlock: " + ex.Message);
        }

        return opResult;
    }

    /// <summary>
    /// Remove a block from the world and replace it with air. Will NOT remove air blocks
    /// </summary>
    /// <param name="blockPos"></param>
    /// <returns> 
    /// true if the operation succeeded 
    /// </returns>
    public bool RemoveBlock(IntPos blockPos)
    {
        bool opResult = false;

        try
        {
            if (_blocks[blockPos.x, blockPos.y, blockPos.z] != (byte)BLOCK_ID.AIR)
            {
                // fill with air
                _blocks[blockPos.x, blockPos.y, blockPos.z] = (byte)BLOCK_ID.AIR;
                Destroy(_blockArray[blockPos.x, blockPos.y, blockPos.z]);
                _UpdateBlockNeighborhood(blockPos);

                opResult = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: RemoveBlock: " + ex.Message);
        }

        return opResult;
    }

    public bool RemoveBlock(int x, int y, int z)
    {
        bool opResult = false;

        try
        {
            if (_blocks[x, y, z] != (byte)BLOCK_ID.AIR)
            {
                // fill with air
                _blocks[x, y, z] = (byte)BLOCK_ID.AIR;
                Destroy(_blockArray[x, y, z]);
                _UpdateBlockNeighborhood(new IntPos(x, y, z));

                opResult = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: RemoveBlock: " + ex.Message);
        }

        return opResult;
    }

    // update neighboring blocks to a passed position so that Block Gameobjects are spawned only when they can be seen
    private void _UpdateBlockNeighborhood(IntPos pos)
    {
        // if the center block is transparent, then its neighborhood might need to have blocks added to it
        if (blockDatabase.IsTransparent((BLOCK_ID)_blocks[pos.x, pos.y, pos.z]))
        {
            //// x
            {
                IntPos newPos = pos;
                newPos.x += 1;

                // if there is no gameobject next to it...
                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    // fill that gameobject spot with the corresponding block type that should be in that voxel (if it is supposed to be air then PlaceBlock will not place one)
                    _ShowBlockAtPosition(newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.x -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    _ShowBlockAtPosition(newPos);
                }
            }


            //// y
            {
                IntPos newPos = pos;
                newPos.y += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    _ShowBlockAtPosition(newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.y -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    _ShowBlockAtPosition(newPos);
                }
            }


            //// z
            {
                IntPos newPos = pos;
                newPos.z += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    _ShowBlockAtPosition(newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.z -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    _ShowBlockAtPosition(newPos);
                }
            }
        }
        else
        {
            // The target block is opaque, therefore some blocks may need to be hidden
            // TODO: hide blocks that are no longer visible because of an addition of a block on top of it
        }
    }
    public void MakeAllAir()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    _blocks[x, y, z] = (byte)0;

                    if (_blockArray[x, y, z] != null)
                    {
                        Destroy(_blockArray[x, y, z]);
                    }
                }
            }
        }

        System.Array.Clear(_blockArray, 0, width * height * depth);
    }

    public void RemoveAllBlocksFromWorld()
    {
        foreach (Transform child in theWorld.transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void Regenerate(bool fromSavedMap)
    {
        if (!fromSavedMap)
        {

            RemoveAllBlocksFromWorld();

            _blocks = new byte[width, height, depth];
            _blockArray = new GameObject[width, height, depth];
            MakeAllAir();


            float rand = Random.Range(MinPower, MaxPower);

            float offsetX = Random.Range(-1024.0f, 1024.0f);
            float offsetY = Random.Range(-1024.0f, 1024.0f);

            // Generation
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (y < Mathf.PerlinNoise((x + offsetX) / rand, (z + offsetY) / rand) * height)
                        {
                            _blocks[x, y, z] = (byte)Random.Range(1, 3);
                        }

                        _blockArray[x, y, z] = null;
                    }
                }
            }
        }


        // Private Methods
        _InstantiateBlocksFromVoxelData();

        // player spawn position
        Vector3 playerSpawnPos = new Vector3(width * 0.5f, height + 10.0f, depth * 0.5f);
        spawnPoint.transform.position = playerSpawnPos;
        player.transform.position = spawnPoint.transform.position;
        IntPos playerPos = new IntPos(player.transform.position);


        if (!fromSavedMap)
        {
            // create platform
            int spawnPlatformWidth = 3;

            _GeneratePlatform(playerPos.x, playerPos.z, spawnPlatformWidth, 2, 5, BLOCK_ID.MARBLE);
            _GeneratePlatform(playerPos.x, playerPos.z, 1, 1, 1, BLOCK_ID.MARBLE);

            _GenerateColumn(playerPos.x - spawnPlatformWidth, playerPos.z - spawnPlatformWidth);
            _GenerateColumn(playerPos.x + spawnPlatformWidth, playerPos.z + spawnPlatformWidth);

            _GenerateColumn(playerPos.x - spawnPlatformWidth, playerPos.z + spawnPlatformWidth);
            _GenerateColumn(playerPos.x + spawnPlatformWidth, playerPos.z - spawnPlatformWidth);

            _StackBlockOnSurface(playerPos.x, playerPos.z, (byte)BLOCK_ID.COLUMN_BASE);
        }

    }


    private void _InstantiateBlocksFromVoxelData()
    {
        // create gameobjects for each block
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    byte blockTypeToGenerate = _blocks[x, y, z];

                    if (blockTypeToGenerate != 0 && (
                        (x == 0 || _blocks[x - 1, y, z] == 0) ||
                        (y == 0 || _blocks[x, y - 1, z] == 0) ||
                        (z == 0 || _blocks[x, y, z - 1] == 0) ||
                        (x == width - 1 || _blocks[x + 1, y, z] == 0) ||
                        (y == height - 1 || _blocks[x, y + 1, z] == 0) ||
                        (z == depth - 1 || _blocks[x, y, z + 1] == 0)))
                    {
                        _blockArray[x, y, z] = (Instantiate(blockDatabase.GetBlockPrefab(blockTypeToGenerate), new Vector3(x, y, z), Quaternion.identity));
                        _blockArray[x, y, z].transform.SetParent(theWorld.transform);
                    }
                    else
                    {
                        _blockArray[x, y, z] = null;
                    }
                }
            }
        }
    }


    private bool _ShowBlockAtPosition(IntPos pos)
    {
        bool opResult = false;

        try
        {
            if (_blocks[pos.x, pos.y, pos.z] != (byte)BLOCK_ID.AIR)
            {
                _blockArray[pos.x, pos.y, pos.z] = (Instantiate(blockDatabase.GetBlockPrefab(_blocks[pos.x, pos.y, pos.z]), pos.Vec3(), Quaternion.identity));
                _blockArray[pos.x, pos.y, pos.z].transform.SetParent(theWorld.transform);
                opResult = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: PlaceBlock: " + ex.Message);
        }

        return opResult;
    }

    // places a block as if dropped from the sky like a connect-four piece. returns y-position of placed block. -1 if it couldnt place
    private int _StackBlockOnSurface(int x, int z, byte blockType)
    {
        for (int y = (height - 2); y >= 0; y--)
        {
            // if a solid block is found,
            if (blockDatabase.GetProperties((BLOCK_ID)_blocks[x, y, z]).m_canBePlacedUpon)
            {
                // place a block on top of it
                if (PlaceBlock(blockType, x, y + 1, z))
                {
                    return y + 1;
                }
            }
        }
        return -1;
    }

    private void _GeneratePlatform(int xCenter, int zCenter, int width, int platformHeight, int platformDepth, BLOCK_ID blockType)
    {
        int platformAlt = _StackBlockOnSurface(xCenter, zCenter, (byte)blockType);

        for (int y = platformAlt + platformHeight - 1; (y > platformAlt - platformDepth) && y >= 0; y--)
        {
            for (int x = xCenter - width; x <= xCenter + width; x++)
            {
                for (int z = zCenter - width; z <= zCenter + width; z++)
                {
                    if (_blocks[x, y, z] != (int)BLOCK_ID.AIR)
                    {
                        RemoveBlock(x, y, z);
                    }

                    PlaceBlock((byte)blockType, x, y, z);
                }
            }
        }
    }
    private void _GenerateColumn(int x, int z)
    {
        _StackBlockOnSurface(x, z, (byte)BLOCK_ID.MARBLE);
        _StackBlockOnSurface(x, z, (byte)BLOCK_ID.COLUMN_BASE);
        _StackBlockOnSurface(x, z, (byte)BLOCK_ID.COLUMN_MID);
        _StackBlockOnSurface(x, z, (byte)BLOCK_ID.COLUMN_TOP);
        _StackBlockOnSurface(x, z, (byte)BLOCK_ID.MARBLE);
    }
}

