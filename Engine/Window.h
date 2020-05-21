#pragma once

#include "Device.h"

class Window
{
public:
	Window(Device& device, UINT width, UINT height, std::string title) :
		width(width),
		height(height),
		device(device)
	{
		window_class_name = title + "_class";
		win32_instance = GetModuleHandle(nullptr);
		// create a win32 window class (required for creating windows)
		WNDCLASS wclass = {};
		wclass.lpszClassName = window_class_name.c_str();
		wclass.hInstance = win32_instance;
		wclass.lpfnWndProc = DefWindowProc;

		RegisterClass(&wclass);

		// the window size is cutted by the border/caption size, so
		// we need to compute the right window size for having
		// a drawable area of 1024x1024

		DWORD style = WS_CAPTION;
		RECT rect = { 0, 0, (LONG)width, (LONG)height };

		// this will compute the right dimension
		AdjustWindowRect(&rect, style, false);

		window = CreateWindow(window_class_name.c_str(), title.c_str(),
			style, CW_USEDEFAULT, CW_USEDEFAULT,
			rect.right - rect.left, rect.bottom - rect.top, nullptr, nullptr, win32_instance, nullptr);

		if (!window)
		{
			icafe::Die("unable to create window");
		}

		// prepare the swap chain configuration
		DXGI_SWAP_CHAIN_DESC1 sc_desc = {};
		sc_desc.BufferCount = 1;
		sc_desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		sc_desc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		sc_desc.Width = width;
		sc_desc.Height = height;
		sc_desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
		sc_desc.SampleDesc.Count = 1;
		sc_desc.SampleDesc.Quality = 0;

		if (DX::GetFactory()->CreateSwapChainForHwnd(device.GetDXHandle(), window, &sc_desc, nullptr, nullptr, &swap_chain) != S_OK)
		{
			icafe::Die("unable to create swap chain");
		}

		// make the window visible (win32)
		ShowWindow(window, SW_SHOW);

	}

	Device& GetDevice() { return device; }

	void Present()
	{
		// bit blit the back buffer to the front buffer, waiting for vsync
		swap_chain->Present(1, 0);
	}

	ID3D11Texture2D* GetDXTexture()
	{
		// retrieve the texture mapped to the swap chain
		ID3D11Texture2D* swap_chain_texture = nullptr;
		if (swap_chain->GetBuffer(0, __uuidof(ID3D11Texture2D), (void **)&swap_chain_texture))
		{
			icafe::Die("unable to get texture from swap chain");
		}
		return swap_chain_texture;
	}

	~Window()
	{
		swap_chain->Release();
		DestroyWindow(window);
	}

private:
	std::string window_class_name;
	HINSTANCE win32_instance;
	UINT width;
	UINT height;
	HWND window;
	IDXGISwapChain1* swap_chain;
	Device& device;
};
