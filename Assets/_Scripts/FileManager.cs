using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class FileManager : MonoBehaviour
{

    const string DLL_NAME = "VoxelEnginePlugin";

    // File IO
    [DllImport(DLL_NAME)]
    private static extern void LoadMap(string filePath);

    [DllImport(DLL_NAME)]
    private static extern void SaveMap(string filePath);

    // World Map Accessors
    [DllImport(DLL_NAME)]
    private static extern System.IntPtr getBlock();

    [DllImport(DLL_NAME)]
    private static extern void setBlock(byte[] blockArray, int width, int height, int depth);

    [DllImport(DLL_NAME)]
    private static extern int getWidth();

    [DllImport(DLL_NAME)]
    private static extern int getHeight();

    [DllImport(DLL_NAME)]
    private static extern int getDepth();

    //non public declaration
    public WorldController worldController;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            onLoadMap();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            onSaveMap();
        }
    }

    public void onSaveMap()
    {
        Debug.Log("Saving Map Data...");
        int width = worldController.width;
        int height = worldController.height;
        int depth = worldController.depth;

        byte[] blocksToSend = new byte[width * height * depth];
        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    blocksToSend[count] = worldController.Blocks[x, y, z];
                    count++;
                }
            }
        }

        setBlock(blocksToSend, width, height, depth);


        SaveMap("Test.map");
    }


    public void onLoadMap()
    {

        LoadMap("Test.map");
        Debug.Log("Loading Map Data...");
        int width = getWidth();
        int height = getHeight();
        int depth = getDepth();
        int size = width * height * depth;

		worldController.RemoveAllBlocksFromWorld();
		worldController.MakeAllAir();
        worldController.Blocks = new byte[width, height, depth];
        worldController.BlockArray = new GameObject[width, height, depth];
		

        byte[] newblocks = new byte[size];

        // get map data from saved file
        Marshal.Copy(getBlock(), newblocks, 0, size);


        // Block Regeneration
        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    worldController.Blocks[x, y, z] = newblocks[count];
                    count++;
                }
            }
        }

        worldController.Regenerate(true);
    }
}
