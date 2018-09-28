#pragma once

#include "PluginSettings.h"

// Core Libraries (std::)
#include <iostream>
#include <fstream>
#include <string>
#include <direct.h>

class PLUGIN_API FileManager {
public:
	// File IO
	void LoadMap(std::string filePath);
	void SaveMap(std::string filePath);

	// world map accessors
	char* getBlock();
	void setBlock(char blockArray[], int width, int height, int depth);

	// return world information
	int getWidth();
	int getHeight();
	int getDepth();
//private:
	char* blocks = 0;


	int worldWidth = 0;
	int worldHeight = 0;
	int worldDepth = 0;
};
