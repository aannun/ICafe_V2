#pragma once

#include "Adapter.h"

class Device
{
public:
	Device(Adapter& adapter)
	{
		// this is the list of features we want to support
		D3D_FEATURE_LEVEL feature_levels[] = {
			D3D_FEATURE_LEVEL_11_1,
			D3D_FEATURE_LEVEL_11_0,
		};
		// this will be filled with the selected feature level
		D3D_FEATURE_LEVEL feature_level;

		// a device is a logical representation of an adapter, allowed to create objects
		// (buffers, textures, shaders, ...) in the gpu.
		// a context is a channel for issuing commands
		if (D3D11CreateDevice(adapter.GetDXHandle(), D3D_DRIVER_TYPE_UNKNOWN,
			nullptr, 0, feature_levels, _countof(feature_levels), D3D11_SDK_VERSION,
			(ID3D11Device**)&device, &feature_level, (ID3D11DeviceContext**)&context) != S_OK)
		{
			icafe::Die("unable to create device and context");
		}

		context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

		// setup RS (rasterization options)
		D3D11_RASTERIZER_DESC rs_desc = {};
		rs_desc.CullMode = D3D11_CULL_NONE; // never cull triangles
		rs_desc.FillMode = D3D11_FILL_SOLID; // draw solid (can be D3D11_FILL_WIREFRAME)
		if (device->CreateRasterizerState(&rs_desc, &rs) != S_OK)
		{
			icafe::Die("unable to create rasterizer state");
		}
		context->RSSetState(rs);

		D3D11_DEPTH_STENCIL_DESC dsd_desc = {};
		dsd_desc.DepthEnable = true;
		dsd_desc.DepthFunc = D3D11_COMPARISON_LESS;
		dsd_desc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
		if (device->CreateDepthStencilState(&dsd_desc, &dss) != S_OK)
		{
			icafe::Die("unable to create depth stencil state");
		}

		context->OMSetDepthStencilState(dss, 0);

		D3D11_SAMPLER_DESC ss_desc = {};
		ss_desc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
		ss_desc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
		ss_desc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
		ss_desc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
		if (device->CreateSamplerState(&ss_desc, &ss) != S_OK)
		{
			icafe::Die("unable to create sampler state");
		}
		context->PSSetSamplers(0, 1, &ss);
	}

	void Draw(UINT nvertices)
	{
		context->Draw(nvertices, 0);
	}

	void SetupRenderTargets(std::initializer_list<ID3D11RenderTargetView1*> targets, float width,
		float height, ID3D11DepthStencilView* depth_target = nullptr)
	{

		context->OMSetRenderTargets(targets.size(), (ID3D11RenderTargetView**)targets.begin(), depth_target);
		// set the rendering area (viewport)
		D3D11_VIEWPORT viewport = {};
		viewport.Width = width;
		viewport.Height = height;
		viewport.MinDepth = 0;
		viewport.MaxDepth = 1;
		context->RSSetViewports(1, &viewport);
	}

	void ClearState()
	{
		context->ClearState();
	}

	ID3D11Device5 * GetDXHandle() { return device; }
	ID3D11DeviceContext4* GetDXContext() { return context; }

private:
	ID3D11Device5* device;
	ID3D11DeviceContext4* context;
	ID3D11RasterizerState* rs;
	ID3D11DepthStencilState* dss;
	ID3D11SamplerState *ss;
};
