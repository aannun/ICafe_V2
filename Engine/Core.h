#pragma once
#include <iostream>
#include <string>
#include <vector>
#include <memory>
#define _USE_MATH_DEFINES 
#include <cmath>

#include <Windows.h>
#include <dxgi1_6.h>
#include <d3d11_4.h>
#include <d3dcompiler.h>

namespace icafe
{
	void Die(std::string message)
	{
		MessageBox(nullptr, message.c_str(), nullptr, MB_OK);
		std::exit(-1);
	}
}