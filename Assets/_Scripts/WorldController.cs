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

public class WorldController : MonoBehaviour
{

    [Header("Dependencies")]
    public BlockDatabase blockDatabase;
    public GameObject player;
    public GameObject spawnPoint;

    [Header("World Size")]
    public int width = 64;
    public int height = 32;
    public int depth = 64;

    [Header("Generator Settings")]
    public float MinPower = 16.0f;
    public float MaxPower = 24.0f;

    // Private Instance Variables
    private char[,,] blocks;
    private GameObject[,,] _blockArray;

    // Use this for initialization
    void Start()
    {
        blocks = new char[width, height, depth];
        _blockArray = new GameObject[width, height, depth];

        spawnPoint.transform.position = new Vector3(width * 0.5f, depth * 0.5f, height + 10.0f);

        player.transform.position = spawnPoint.transform.position;

        // set up world reference so Block Commands can use it
        BlockCommand.s_world = this;

        _Regenerate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _Regenerate();
        }
    }

    public char GetBlockID(IntPos blockPos)
    {
        return blocks[blockPos.x, blockPos.y, blockPos.z];
    }

    /// <summary>
    /// Place a block into the world, will NOT place air blocks
    /// </summary>
    /// <param name="blockTypeID"></param>
    /// <param name="blockPos"></param>
    /// <returns> 
    /// true if the operation succeeded 
    /// </returns>
    public bool PlaceBlock(char blockTypeID, Vector3 blockPos)
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
    public bool PlaceBlock(char blockTypeID, IntPos blockPos)
    {
        bool opResult = false;

        try
        {
            if (blockTypeID != (char)BLOCK_ID.AIR && blocks[blockPos.x, blockPos.y, blockPos.z] == (char)BLOCK_ID.AIR)
            {
                blocks[blockPos.x, blockPos.y, blockPos.z] = blockTypeID;
                _blockArray[blockPos.x, blockPos.y, blockPos.z] = (Instantiate(blockDatabase.GetBlockPrefab(blockTypeID), blockPos.Vec3(), Quaternion.identity));

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
            if (blocks[blockPos.x, blockPos.y, blockPos.z] != (char)BLOCK_ID.AIR)
            {
                // fill with air
                blocks[blockPos.x, blockPos.y, blockPos.z] = (char)BLOCK_ID.AIR;
                Destroy(_blockArray[blockPos.x, blockPos.y, blockPos.z]);
                UpdateBlockNeighborhood(blockPos);

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
    void UpdateBlockNeighborhood(IntPos pos)
    {
        // if the center block is transparent, then its neighborhood might need to have blocks added to it
        if (blockDatabase.IsTransparent((BLOCK_ID)blocks[pos.x, pos.y, pos.z]))
        {
            //// x
            {
                IntPos newPos = pos;
                newPos.x += 1;

                // if there is no gameobject next to it...
                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    // fill that gameobject spot with the corresponding block type that should be in that voxel (if it is supposed to be air then PlaceBlock will not place one)
                    ShowBlockAtPosition(newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.x -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    ShowBlockAtPosition(newPos);
                }
            }


            //// y
            {
                IntPos newPos = pos;
                newPos.y += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    ShowBlockAtPosition(newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.y -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    ShowBlockAtPosition(newPos);
                }
            }


            //// z
            {
                IntPos newPos = pos;
                newPos.z += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    ShowBlockAtPosition(newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.z -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    ShowBlockAtPosition(newPos);
                }
            }
        }
        else
        {
            // The target block is opaque, therefore some blocks may need to be hidden
            // TODO: hide blocks that are no longer visible because of an addition of a block on top of it
        }
    }

    // Private Methods
    private void _Regenerate()
    {
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
                        blocks[x, y, z] = (char)Random.Range(1, 3);
                    }

                }
            }
        }

        InstantiateBlocksFromVoxelData();

        // spawn position
        player.transform.position = spawnPoint.transform.position;
    }
    void MakeAllAir()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    blocks[x, y, z] = (char)0;

                    if (_blockArray[x, y, z] != null)
                    {
                        Destroy(_blockArray[x, y, z]);
                    }
                }
            }
        }
    }

    void InstantiateBlocksFromVoxelData()
    {
        // create gameobjects for each block
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    char blockTypeToGenerate = blocks[x, y, z];

                    if (blockTypeToGenerate != 0 && (
                        (x == 0 || blocks[x - 1, y, z] == 0) ||
                        (y == 0 || blocks[x, y - 1, z] == 0) ||
                        (z == 0 || blocks[x, y, z - 1] == 0) ||
                        (x == width - 1 || blocks[x + 1, y, z] == 0) ||
                        (y == height - 1 || blocks[x, y + 1, z] == 0) ||
                        (z == depth - 1 || blocks[x, y, z + 1] == 0)))
                    {
                        _blockArray[x, y, z] = (Instantiate(blockDatabase.GetBlockPrefab(blockTypeToGenerate), new Vector3(x, y, z), Quaternion.identity));
                    }
                    else
                    {
                        _blockArray[x, y, z] = null;
                    }
                }
            }
        }
    }


    private bool ShowBlockAtPosition(IntPos pos)
    {
        bool opResult = false;

        try
        {
            if (blocks[pos.x, pos.y, pos.z] != (char)BLOCK_ID.AIR)
            {
                _blockArray[pos.x, pos.y, pos.z] = (Instantiate(blockDatabase.GetBlockPrefab(blocks[pos.x, pos.y, pos.z]), pos.Vec3(), Quaternion.identity));
                opResult = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: PlaceBlock: " + ex.Message);
        }

        return opResult;
    }

    // places a block as if dropped from the sky like a connect-four piece
    void stackBlockOnSurface(int x, int y)
    {

    }
}

