#pragma once
#include "Texture.h"

class ShaderObject
{
public:
	ShaderObject(Texture& texture) : texture(texture)
	{
		if (texture.GetDevice().GetDXHandle()->CreateShaderResourceView1(texture.GetDXHandle(), nullptr, &srv) != S_OK)
		{
			icafe::Die("unable to create shader resource view");
		}
	}

	ShaderObject(Texture& texture, DXGI_FORMAT format) : texture(texture)
	{
		D3D11_SHADER_RESOURCE_VIEW_DESC1 desc = {};
		desc.Format = format;
		desc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
		desc.Texture2D.MipLevels = 1;
		if (texture.GetDevice().GetDXHandle()->CreateShaderResourceView1(texture.GetDXHandle(), &desc, &srv) != S_OK)
		{
			icafe::Die("unable to create shader resource view");
		}
	}

	void Bind(UINT gpu_register)
	{
		texture.GetDevice().GetDXContext()->PSSetShaderResources(gpu_register, 1, (ID3D11ShaderResourceView **)&srv);
	}

	~ShaderObject()
	{
		srv->Release();
	}

private:
	ID3D11ShaderResourceView1* srv;
	Texture& texture;
};