#pragma once

#include "DepthTarget.h"

class RenderTarget
{
public:
	RenderTarget(Texture& texture) : texture(texture)
	{
		// a render target view is an object able to receive rasterization
		// output (like an opengl framebuffer)
		if (texture.GetDevice().GetDXHandle()->CreateRenderTargetView1(texture.GetDXHandle(), nullptr, &rtv) != S_OK)
		{
			icafe::Die("unable to create render target view");
		}
	}

	void Bind(DepthTarget* depth_target = nullptr)
	{
		ID3D11DepthStencilView* depth_view = nullptr;
		if (depth_target)
		{
			depth_view = depth_target->GetDXHandle();
		}
		texture.GetDevice().GetDXContext()->OMSetRenderTargets(1, (ID3D11RenderTargetView **)&rtv, depth_view);
		// set the rendering area (viewport)
		D3D11_VIEWPORT viewport = {};
		viewport.Height = (float)texture.GetHeight();
		viewport.Width = (float)texture.GetWidth();
		viewport.MinDepth = 0;
		viewport.MaxDepth = 1;
		texture.GetDevice().GetDXContext()->RSSetViewports(1, &viewport);
	}

	void Clear(std::initializer_list<float> color)
	{
		texture.GetDevice().GetDXContext()->ClearRenderTargetView(rtv, color.begin());
	}

	ID3D11RenderTargetView1* GetDXHandle() { return rtv; }

	~RenderTarget()
	{
		rtv->Release();
	}

private:
	ID3D11RenderTargetView1* rtv;
	Texture& texture;
};
