#pragma once

#include "PluginSettings.h"
#include "FileManager.h"
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif
	// File IO
	PLUGIN_API void LoadMap(char* filePath);
	PLUGIN_API void SaveMap(char* filePath);

	// World Map Accessors
	PLUGIN_API char* getBlock();
	PLUGIN_API void setBlock(char blockArray[], int width, int height, int depth);

	// return world information
	PLUGIN_API int getWidth();
	PLUGIN_API int getHeight();
	PLUGIN_API int getDepth();

#ifdef __cplusplus
}
#endif
