#pragma once
#include "Device.h"
#include "ShaderBuffer.h"
#include "ShaderResource.h"

class PixelShader
{
public:
	PixelShader(Device& device, const void* byte_code, SIZE_T byte_code_size) : device(device)
	{
		if (device.GetDXHandle()->CreatePixelShader(byte_code, byte_code_size, nullptr, &pixel_shader) != S_OK)
		{
			icafe::Die("unable to create pixel shader");
		}

		CreateShaderData(byte_code, byte_code_size);
	}

	void Bind()
	{
		device.GetDXContext()->PSSetShader(pixel_shader, nullptr, 0);
	}

	void BindResources()
	{
		if (shader_buffer != nullptr)
			shader_buffer->Bind();
		for (int i = 0; i < shader_resources.size(); i++)
			shader_resources[i]->Bind();
	}

	~PixelShader()
	{
		pixel_shader->Release();

		delete shader_buffer;
		for (size_t i = 0; i < shader_resources.size(); i++)
			delete shader_resources[i];

		shader_resources.clear();
		shader_resources.shrink_to_fit();
	}

	void UpdateVariableValue(unsigned int register_index, std::string name, void* value)
	{
		if (shader_buffer == nullptr)
			return;

		shader_buffer->UpdateVariableValue(name, value);
	}

	void UpdateTexture(unsigned int register_index, Texture* texture)
	{
		auto resource = GetResource(register_index);
		if (resource == nullptr)
			return;

		resource->Upload(texture);
	}

private:

	ShaderResource* GetResource(int register_index)
	{
		for (int i = 0; i < shader_resources.size(); i++)
		{
			if (shader_resources[i]->GetRegisterIndex() == register_index)
				return shader_resources[i];
		}
		return nullptr;
	}

	void CreateShaderData(const void* byte_code, SIZE_T byte_code_size)
	{
		ID3D11ShaderReflection* pReflector;
		D3DReflect(byte_code, byte_code_size, IID_ID3D11ShaderReflection, (void**)&pReflector);

		unsigned int register_index = 0;
		D3D11_SHADER_DESC desc;
		pReflector->GetDesc(&desc);

		for (int i = 0; i < desc.BoundResources; ++i)
		{
			D3D11_SHADER_INPUT_BIND_DESC resource_desc;
			pReflector->GetResourceBindingDesc(i, &resource_desc);

			ShaderResource* resource = new ShaderResource(device, &resource_desc);
			shader_resources.push_back(resource);
		}

		for (int i = 0; i < desc.ConstantBuffers; ++i)
		{
			ID3D11ShaderReflectionConstantBuffer *buffer = pReflector->GetConstantBufferByIndex(i);

			D3D11_SHADER_BUFFER_DESC buffer_desc;
			buffer->GetDesc(&buffer_desc);

			if (!strcmp(buffer_desc.Name, "$Globals"))
			{
				register_index = 0;
				ShaderBuffer* shader_buffer = new ShaderBuffer(device, register_index, buffer_desc.Name, buffer, &buffer_desc);
				break;
			}

			/*for (int k = 0; k < desc.BoundResources; ++k)
			{
				D3D11_SHADER_INPUT_BIND_DESC ibdesc;
				pReflector->GetResourceBindingDesc(k, &ibdesc);

				if (!strcmp(ibdesc.Name, buffer_desc.Name))
					register_index = ibdesc.BindPoint;
			}*/
		}
	}

private:
	Device& device;
	ID3D11PixelShader *pixel_shader;

	std::vector<ShaderResource*> shader_resources;
	ShaderBuffer* shader_buffer;
};