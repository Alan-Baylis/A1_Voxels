#pragma once

#ifdef VOXELENGINEPLUGIN_EXPORTS
#define PLUGIN_API __declspec(dllexport)
#elif VOXELENGINEPLUGIN_IMPORTS
#define PLUGIN_API __declspec(dllimport)
#else
#define PLUGIN_API 
#endif
