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

public class GameController : MonoBehaviour
{

    [Header("Dependencies")]
    public BlockData blockData;
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
    private List<GameObject> _blockArray;

    // Use this for initialization
    void Start()
    {
        blocks = new char[width, height, depth];
        _blockArray = new List<GameObject>();

		spawnPoint.transform.position = new Vector3(width * 0.5f, depth *0.5f, height + 10.0f);

        player.transform.position = spawnPoint.transform.position;

        _Regenerate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _Regenerate();
        }
    }

    // Place a block into the world
    public void PlaceBlock(char blockTypeID, Vector3 position)
    {
        IntPos blockPos = new IntPos(position);
        blocks[blockPos.x, blockPos.y, blockPos.z] = blockTypeID;
        _blockArray.Add(Instantiate(blockData.GetBlockData(blockTypeID), blockPos.Vec3(), Quaternion.identity));
    }

    // Place a block into the world
    public void PlaceBlock(char blockTypeID, IntPos blockPos)
    {
        blocks[blockPos.x, blockPos.y, blockPos.z] = blockTypeID;
        _blockArray.Add(Instantiate(blockData.GetBlockData(blockTypeID), blockPos.Vec3(), Quaternion.identity));
    }

    // Private Methods
    private void _Regenerate()
    {

        if (_blockArray.Count > 0)
        {
            foreach (var blk in _blockArray)
            {
                Destroy(blk);
            }

            _blockArray.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        blocks[x, y, z] = (char)0;
                    }
                }
            }
        }

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
                        _blockArray.Add(Instantiate(blockData.GetBlockData(blockTypeToGenerate), new Vector3(x, y, z), Quaternion.identity));
                }
            }
        }

        player.transform.position = spawnPoint.transform.position;
    }
}
