#pragma once
#include "Device.h"

class GPUConstBuffer
{
public:
	GPUConstBuffer(Device& device, UINT size, const void* data = nullptr) :
		device(device)
	{
		D3D11_BUFFER_DESC buf_desc = {};
		buf_desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
		buf_desc.ByteWidth = size % 16 == 0 ? size : size + (16 - (size % 16));
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
			icafe::Die("unable to create constant buffer");
		}
	}

	void Bind(UINT gpu_register, void *data)
	{
		device.GetDXContext()->UpdateSubresource(buffer, 0, nullptr, data, 0, 0);
		device.GetDXContext()->VSSetConstantBuffers(gpu_register, 1, &buffer);
	}

	void BindPS(UINT gpu_register, void *data)
	{
		device.GetDXContext()->UpdateSubresource(buffer, 0, nullptr, data, 0, 0);
		device.GetDXContext()->PSSetConstantBuffers(gpu_register, 1, &buffer);
	}

	~GPUConstBuffer()
	{
		buffer->Release();
	}

	ID3D11Buffer* GetDXHandle() { return buffer; }
private:
	ID3D11Buffer* buffer;
	Device& device;
};