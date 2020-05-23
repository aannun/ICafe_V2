#pragma once

#include "Core.h"
#include "ShaderObject.h"

class ShaderResource
{
public:
	ShaderResource(Device &device, D3D11_SHADER_INPUT_BIND_DESC* desc) : device(device)
	{
		register_index = desc->BindPoint;
	}

	int GetRegisterIndex()
	{
		return register_index;
	}

	void Upload(Texture *texture)
	{
		if (shader_object != nullptr)
			delete shader_object;

		shader_object = new ShaderObject(*texture);
	}

	void Bind()
	{
		if (shader_object != nullptr)
			shader_object->Bind(register_index);
	}

	~ShaderResource()
	{
		delete shader_object;
	}

private:
	int register_index;
	std::string name;

	Device& device;
	ShaderObject *shader_object;
};