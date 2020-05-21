#pragma once

#include "Device.h"

class Texture
{
public:
	Texture(Device& device, UINT width, UINT height, DXGI_FORMAT format = DXGI_FORMAT_R8G8B8A8_UNORM) :
		width(width),
		height(height),
		device(device)
	{
		// create a new texture
		D3D11_TEXTURE2D_DESC tex_desc = {};
		tex_desc.ArraySize = 1;
		if (format == DXGI_FORMAT_D24_UNORM_S8_UINT)
		{
			tex_desc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
		}
		else if (format == DXGI_FORMAT_R24G8_TYPELESS)
		{
			tex_desc.BindFlags = D3D11_BIND_DEPTH_STENCIL | D3D11_BIND_SHADER_RESOURCE;
		}
		else
		{
			tex_desc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;
		}
		tex_desc.Format = format;
		tex_desc.Height = height;
		tex_desc.Width = width;
		tex_desc.MipLevels = 1;
		tex_desc.Usage = D3D11_USAGE_DEFAULT;
		tex_desc.SampleDesc.Count = 1;
		tex_desc.SampleDesc.Quality = 0;

		if (device.GetDXHandle()->CreateTexture2D(&tex_desc, nullptr, &texture) != S_OK)
		{
			icafe::Die("unable to create texture");
		}
	}

	Texture(Window& window) : device(window.GetDevice())
	{
		texture = window.GetDXTexture();
		D3D11_TEXTURE2D_DESC tex_desc;
		texture->GetDesc(&tex_desc);
		width = tex_desc.Width;
		height = tex_desc.Height;
	}

	void Upload(void *data, UINT pitch)
	{
		device.GetDXContext()->UpdateSubresource(texture, 0, nullptr, data, pitch, 0);
	}

	~Texture()
	{
		texture->Release();
	}

	ID3D11Texture2D* GetDXHandle() { return texture; }

	Device& GetDevice() { return device; }

	UINT GetWidth() { return width; }
	UINT GetHeight() { return height; }

private:
	ID3D11Texture2D* texture;
	UINT width;
	UINT height;
	Device& device;
};