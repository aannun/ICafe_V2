#pragma once
#include "Core.h"

class DX
{
public:
	static IDXGIFactory2* GetFactory()
	{
		static IDXGIFactory2* factory = nullptr;
		if (factory)
			return factory;

		if (CreateDXGIFactory2(0, __uuidof(IDXGIFactory2), (void **)&factory) != S_OK)
		{
			icafe::Die("unable to create factory");
		}
		return factory;
	}
};
