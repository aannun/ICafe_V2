#pragma once
#include "Device.h"

class GPUBuffer
{
public:
	GPUBuffer(Device& device, UINT size, UINT stride, const void* data = nullptr) :
		device(device), stride(stride)
	{
		D3D11_BUFFER_DESC buf_desc = {};
		buf_desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
		buf_desc.ByteWidth = size;
		buf_desc.Usage = D3D11_USAGE_DEFAULT;

		D3D11_SUBRESOURCE_DATA* buf_data_ptr = nullptr;

		if (data)
		{
			// like textures, but without pitching
			D3D11_SUBRESOURCE_DATA buf_data = {};
			buf_data.pSysMem = data;
			buf_data_ptr = &buf_data;
		}

		if (device.GetDXHandle()->CreateBuffer(&buf_desc, buf_data_ptr, &buffer) != S_OK)
		{
			icafe::Die("unable to create buffer");
		}
	}

	void Bind(UINT slot)
	{
		UINT strides = { stride }; // how many bytes to read for each vertex
		UINT offsets = { 0 };
		// NOTE: IASetVertexBuffers takes an array of buffers, strides and offsets
		// but we can call it multiple times for different slots
		device.GetDXContext()->IASetVertexBuffers(slot, 1, &buffer, &strides, &offsets);
	}

	~GPUBuffer()
	{
		buffer->Release();
	}

	ID3D11Buffer* GetDXHandle() { return buffer; }
private:
	ID3D11Buffer* buffer;
	Device& device;
	UINT stride;
};
