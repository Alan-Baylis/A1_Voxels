#pragma once

#ifdef PLUGIN_EXPORT
#define LIB_API __declspec(dllexport)
#endif

#ifdef PLUGIN_IMPORT
#define LIB_API __declspec(dllimport)
#endif