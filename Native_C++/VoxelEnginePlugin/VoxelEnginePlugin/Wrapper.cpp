#include "Wrapper.h"

FileManager fileManager;

PLUGIN_API void LoadMap(char * filePath)
{
	return fileManager.LoadMap(std::string(filePath));
}

PLUGIN_API void SaveMap(char * filePath)
{
	return fileManager.SaveMap(std::string(filePath));
}

PLUGIN_API char * getBlock()
{
	return fileManager.getBlock();
}

PLUGIN_API void setBlock(char blockArray[], int width, int height, int depth)
{
	return fileManager.setBlock(blockArray, width, height, depth);
}

PLUGIN_API int getWidth()
{
	return fileManager.getWidth();
}

PLUGIN_API int getHeight()
{
	return fileManager.getHeight();
}

PLUGIN_API int getDepth()
{
	return fileManager.getDepth();
}



