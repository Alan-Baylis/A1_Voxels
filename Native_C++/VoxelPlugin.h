#pragma once

#include "LibSettings.h"

#ifdef __cplusplus
extern "C"
{
	LIB_API char* Greet();
	LIB_API void SetGreeting(char* newGreeting);
	LIB_API int Add(int a, int b);
}
#endif
