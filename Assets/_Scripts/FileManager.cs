using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class FileManager : MonoBehaviour {

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


	// Use this for initialization
	void Start () {

		
		int width = 64;
		int height = 32;
		int depth = 64;

		byte[,,] blocks = new byte[width, height, depth];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				for (int z = 0; z < depth; z++)
				{
					blocks[x, y, z] = (byte)Random.Range(0, 3);
				}
			}
		}

		
		byte[] blocksToSend = new byte[width*height*depth];
		int count = 0;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				for (int z = 0; z < depth; z++)
				{
					blocksToSend[count] = blocks[x ,y ,z];
					count++;
				}
			}
		}
		
		setBlock(blocksToSend, width, height, depth);


		SaveMap("Test.map");
		


		// load and get position
		

		/* 
		LoadMap("Test.map");
		int width = getWidth();
		int height = getHeight();
		int depth = getDepth();
		int size = width * height * depth;

		Debug.Log("Width:  " + width);
		Debug.Log("Height: " + height);
		Debug.Log("Depth:  " + depth);

		 
		byte[] newblocks = new byte[size];

		
		Marshal.Copy(getBlock(), newblocks,0, size);
		*/

		

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
