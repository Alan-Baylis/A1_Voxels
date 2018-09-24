using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IntPos
{
    public IntPos(Vector3 vecPos)
    {
        x = (int)vecPos.x;
        y = (int)vecPos.y;
        z = (int)vecPos.z;
    }

    public int x;
    public int y;
    public int z;
    public Vector3 Vec3()
    {
        return new Vector3(x, y, z);
    }
}

struct Block
{

}

public class GameController : MonoBehaviour
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

    // Place a block into the world
    public bool PlaceBlock(char blockTypeID, Vector3 position)
    {
        try
        {

            if (blockTypeID != 0)
            {
                IntPos blockPos = new IntPos(position);
                blocks[blockPos.x, blockPos.y, blockPos.z] = blockTypeID;
                _blockArray[blockPos.x, blockPos.y, blockPos.z] = (Instantiate(blockDatabase.GetBlockPrefab(blockTypeID), blockPos.Vec3(), Quaternion.identity));
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("PlaceBlock: " + ex.Message);
            return false;
        }
        return true;
    }

    // Place a block into the world
    public bool PlaceBlock(char blockTypeID, IntPos blockPos)
    {
        try
        {
            if (blockTypeID != 0)
            {
                blocks[blockPos.x, blockPos.y, blockPos.z] = blockTypeID;
                _blockArray[blockPos.x, blockPos.y, blockPos.z] = (Instantiate(blockDatabase.GetBlockPrefab(blockTypeID), blockPos.Vec3(), Quaternion.identity));
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("PlaceBlock: " + ex.Message);
            return false;
        }

        return true;
    }

    // Remove a block from the world
    public bool RemoveBlock(IntPos blockPos)
    {
        try
        {
            blocks[blockPos.x, blockPos.y, blockPos.z] = (char)BLOCK_ID.AIR;
            Destroy(_blockArray[blockPos.x, blockPos.y, blockPos.z]);
            UpdateBlockNeighborhood(blockPos);
        }
        catch (System.Exception ex)
        {
            Debug.Log("RemoveBlock: " + ex.Message);
            return false;
        }

        return true;
    }

    // update neighboring blocks to a passed position so that Blocks are be spawned only when they can be seen
    void UpdateBlockNeighborhood(IntPos pos)
    {
        // if the target block is transparent, then its neighborhood might need to have blocks added to it
        if (blockDatabase.IsTransparent((BLOCK_ID)blocks[pos.x, pos.y, pos.z]))
        {

            //// x
            {
                IntPos newPos = pos;
                newPos.x += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    PlaceBlock(blocks[newPos.x, newPos.y, newPos.z], newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.x -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    PlaceBlock(blocks[newPos.x, newPos.y, newPos.z], newPos);
                }
            }


            //// y
            {
                IntPos newPos = pos;
                newPos.y += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    PlaceBlock(blocks[newPos.x, newPos.y, newPos.z], newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.y -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    PlaceBlock(blocks[newPos.x, newPos.y, newPos.z], newPos);
                }
            }


            //// z
            {
                IntPos newPos = pos;
                newPos.z += 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    PlaceBlock(blocks[newPos.x, newPos.y, newPos.z], newPos);
                }
            }

            {
                IntPos newPos = pos;
                newPos.z -= 1;

                if (_blockArray[newPos.x, newPos.y, newPos.z] == null)
                {
                    PlaceBlock(blocks[newPos.x, newPos.y, newPos.z], newPos);
                }
            }
        }
    }

    // Private Methods
    private void _Regenerate()
    {

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

        UpdateVisibleBlocksOnCreation();

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

    void UpdateVisibleBlocksOnCreation()
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
}

