#pragma once
#include "Device.h"
#include "ShaderBuffer.h"

class PixelShader
{
public:
	PixelShader(Device& device, const void* byte_code, SIZE_T byte_code_size) : device(device)
	{
		if (device.GetDXHandle()->CreatePixelShader(byte_code, byte_code_size, nullptr, &pixel_shader) != S_OK)
		{
			icafe::Die("unable to create pixel shader");
		}

		CreateShaderBuffers(byte_code, byte_code_size);
	}

	void Bind()
	{
		device.GetDXContext()->PSSetShader(pixel_shader, nullptr, 0);
	}

	~PixelShader()
	{
		pixel_shader->Release();

		for (size_t i = 0; i < shader_buffers.size(); i++)
			delete shader_buffers[i];
		shader_buffers.clear();
		shader_buffers.shrink_to_fit();
	}

	void UpdateVariableValue(unsigned int register_index, std::string name, void* value)
	{
		auto buffer = GetBuffer(register_index);
		if (buffer == nullptr)
			return;

		buffer->UpdateVariableValue(name, value);
	}

private:

	ShaderBuffer* GetBuffer(int register_index)
	{
		for (int i = 0; i < shader_buffers.size(); i++)
		{
			if (shader_buffers[i]->GetRegisterIndex() == register_index)
				return shader_buffers[i];
		}
		return nullptr;
	}

	void CreateShaderBuffers(const void* byte_code, SIZE_T byte_code_size)
	{
		ID3D11ShaderReflection* pReflector;
		D3DReflect(byte_code, byte_code_size, IID_ID3D11ShaderReflection, (void**)&pReflector);

		unsigned int register_index = 0;
		D3D11_SHADER_DESC desc;
		pReflector->GetDesc(&desc);

		for (int i = 0; i < desc.ConstantBuffers; ++i)
		{
			ID3D11ShaderReflectionConstantBuffer *buffer = pReflector->GetConstantBufferByIndex(i);

			D3D11_SHADER_BUFFER_DESC buffer_desc;
			buffer->GetDesc(&buffer_desc);

			for (unsigned int k = 0; k < desc.BoundResources; ++k)
			{
				D3D11_SHADER_INPUT_BIND_DESC ibdesc;
				pReflector->GetResourceBindingDesc(k, &ibdesc);

				if (!strcmp(ibdesc.Name, buffer_desc.Name))
					register_index = ibdesc.BindPoint;
			}

			//
			//Add constant buffer
			//
			ShaderBuffer* shaderbuffer = new ShaderBuffer(device, register_index, buffer_desc.Name, buffer, &buffer_desc);
			shader_buffers.push_back(shaderbuffer);
		}
	}

private:
	Device& device;
	ID3D11PixelShader *pixel_shader;
	
	std::vector<ShaderBuffer*> shader_buffers;
};