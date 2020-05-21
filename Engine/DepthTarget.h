#pragma once
#include "Texture.h"

class DepthTarget
{
public:
	DepthTarget(Texture& texture) : texture(texture)
	{
		// a render target view is an object able to receive rasterization
		// output (like an opengl framebuffer)
		if (texture.GetDevice().GetDXHandle()->CreateDepthStencilView(texture.GetDXHandle(), nullptr, &dsv) != S_OK)
		{
			icafe::Die("unable to create depth stencil view");
		}
	}

	DepthTarget(Texture& texture, DXGI_FORMAT format) : texture(texture)
	{
		// a render target view is an object able to receive rasterization
		// output (like an opengl framebuffer)
		D3D11_DEPTH_STENCIL_VIEW_DESC desc = {};
		desc.Format = format;
		desc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
		if (texture.GetDevice().GetDXHandle()->CreateDepthStencilView(texture.GetDXHandle(), &desc, &dsv) != S_OK)
		{
			icafe::Die("unable to create depth stencil view");
		}
	}

	void Clear()
	{
		texture.GetDevice().GetDXContext()->ClearDepthStencilView(dsv, D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1, 0);
	}

	void BindOnlyDepth()
	{
		texture.GetDevice().GetDXContext()->OMSetRenderTargets(0, nullptr, dsv);
		// set the rendering area (viewport)
		D3D11_VIEWPORT viewport = {};
		viewport.Height = (float)texture.GetHeight();
		viewport.Width = (float)texture.GetWidth();
		viewport.MinDepth = 0;
		viewport.MaxDepth = 1;
		texture.GetDevice().GetDXContext()->RSSetViewports(1, &viewport);
	}

	ID3D11DepthStencilView* GetDXHandle() { return dsv; }

	~DepthTarget()
	{
		dsv->Release();
	}

private:
	ID3D11DepthStencilView* dsv;
	Texture& texture;
};