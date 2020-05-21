#pragma once
#include "DX.h"

class Adapter
{
public:
	Adapter()
	{
		// create a DXGI factory, required for operating system
		// integration, like enumerating adapters and creating swap chains
		auto factory = DX::GetFactory();

		// find the best adapter (hardware and max dedicated unshared memory)
		UINT i = 0;
		IDXGIAdapter1* current_adapter = nullptr;
		SIZE_T best_memory = 0;
		// enum adapters returns DXGI_ERROR_NOT_FOUND if the specified index
		// does not exist
		while (factory->EnumAdapters1(i++, &current_adapter) != DXGI_ERROR_NOT_FOUND)
		{
			DXGI_ADAPTER_DESC1 desc;
			if (current_adapter->GetDesc1(&desc) != S_OK)
			{
				icafe::Die("unable to get adapter description");
			}
			std::wcout << desc.Description << " " << desc.DedicatedVideoMemory << std::endl;
			if (desc.Flags == DXGI_ADAPTER_FLAG_NONE && desc.DedicatedVideoMemory > best_memory)
			{
				best_memory = desc.DedicatedVideoMemory;
				adapter = current_adapter;
			}
		}

		if (!adapter)
		{
			icafe::Die("unable to find a valid adapter");
		}

	}

	IDXGIAdapter1* GetDXHandle() { return adapter; }

private:
	IDXGIAdapter1* adapter;
};