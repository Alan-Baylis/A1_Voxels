using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [Header("Dependencies")]
    public GameObject block;
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
                        blocks[x, y, z] = (char)1;
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
                    if (blocks[x, y, z] != 0 && (
                        (x == 0 || blocks[x - 1, y, z] == 0) ||
                        (y == 0 || blocks[x, y - 1, z] == 0) ||
                        (z == 0 || blocks[x, y, z - 1] == 0) ||
                        (x == width - 1 || blocks[x + 1, y, z] == 0) ||
                        (y == height - 1 || blocks[x, y + 1, z] == 0) ||
                        (z == depth - 1 || blocks[x, y, z + 1] == 0)))
                        _blockArray.Add(Instantiate(block, new Vector3(x, y, z), Quaternion.identity));
                }
            }
        }

        player.transform.position = spawnPoint.transform.position;
    }
}
