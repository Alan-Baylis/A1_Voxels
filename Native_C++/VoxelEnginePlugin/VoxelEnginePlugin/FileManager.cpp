#include "FileManager.h"

void FileManager::LoadMap(std::string filePath)
{
	std::string outPath = filePath;

	std::ifstream textFile(outPath, std::ios::in, std::ios::binary);
	if (textFile.is_open()) {

		textFile.read(reinterpret_cast<char*>(&worldWidth), sizeof(worldWidth));
		textFile.read(reinterpret_cast<char*>(&worldHeight), sizeof(worldHeight));
		textFile.read(reinterpret_cast<char*>(&worldDepth), sizeof(worldDepth));

		int arraySize = (worldWidth * worldHeight * worldDepth);
		blocks = new char[arraySize];
		textFile.read(reinterpret_cast<char*>(blocks), arraySize);
		textFile.close();
	}
}

void FileManager::SaveMap(std::string filePath)
{
	std::string outPath = filePath;

	std::ofstream textFile(outPath, std::ios::out, std::ios::binary);
	if (textFile.is_open()) {

		textFile.write(reinterpret_cast<char*>(&worldWidth), sizeof(worldWidth));
		textFile.write(reinterpret_cast<char*>(&worldHeight), sizeof(worldHeight));
		textFile.write(reinterpret_cast<char*>(&worldDepth), sizeof(worldDepth));

		int arraySize = (worldWidth * worldHeight * worldDepth);
		textFile.write(reinterpret_cast<char*>(blocks), arraySize);
		textFile.close();
	}
}

char* FileManager::getBlock()
{
	return blocks;
}

void FileManager::setBlock(char blockArray[], int width, int height, int depth)
{
	
	worldWidth = width;
	worldHeight = height;
	worldDepth = depth;
	int size = width * height * depth;

	blocks = new char[size];
	for (size_t i = 0; i < size; i++)
	{
		blocks[i] = blockArray[i];
	}
}

int FileManager::getWidth()
{
	return worldWidth;
}

int FileManager::getHeight()
{
	return worldHeight;
}

int FileManager::getDepth()
{
	return worldDepth;
}
