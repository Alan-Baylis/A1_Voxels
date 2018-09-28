#include "FileManager.h"

#include <stdio.h>
#include <stdlib.h>
#include <time.h>

#include <string.h>
#include <iostream>


void main() {
	/* initialize random seed: */

	/*
	srand(time(NULL));

	FileManager fileManager;
	
	const int width = 64;
	const int height = 32;
	const int depth = 64;

	char blocks[width][height][depth];

	for (size_t x = 0; x < width; x++)
	{
		for (size_t y = 0; y < height; y++)
		{
			for (size_t z = 0; z < depth; z++)
			{
				blocks[x][y][z] =  '0' + (rand() % 3);

			}
		}
	}


	char newBlock[sizeof(blocks)];
	int count = 0;
	for (size_t x = 0; x < width; x++)
	{
		for (size_t y = 0; y < height; y++)
		{
			for (size_t z = 0; z < depth; z++)
			{
				newBlock[count] = blocks[x][y][z];
				count++;
			}
		}
	}


	fileManager.setBlock(newBlock, width, height, depth);
	fileManager.SaveMap("Test.map");
	*/
	/*
	
	FileManager fileManager;
	
	fileManager.LoadMap("Test.map");


	const int width = fileManager.getWidth();
	const int height = fileManager.getHeight();
	const int depth = fileManager.getDepth();
	const int arraySize = width * height * depth;

	std::cout << "Width: " << std::to_string(width) << " Height: " << std::to_string(height) << " Depth: " << std::to_string(depth) << std::endl;
	
	//char newBlock[arraySize];
	char* newBlock = new char[(arraySize)];

	for (size_t i = 0; i < arraySize; i++)
	{
		newBlock[i] = fileManager.getBlock()[i];
		std::cout << newBlock[i] << std::endl;
	}
	*/
	
	std::string message;
	std::cin >> message;
	
	
}