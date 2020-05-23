#pragma once

#include "Core.h"

class ShaderVariable
{
public:
	std::string name;
	SIZE_T length;
	SIZE_T offset;
};

class ShaderBuffer
{
public:
	ShaderBuffer(Device &device, unsigned int register_index, std::string name, ID3D11ShaderReflectionConstantBuffer* buffer, D3D11_SHADER_BUFFER_DESC* desc)
	{
		this->register_index = register_index;
		this->name = name;

		for (unsigned int i = 0; i < desc->Variables; ++i)
		{
			ID3D11ShaderReflectionVariable* variable = NULL;
			variable = buffer->GetVariableByIndex(i);

			D3D11_SHADER_VARIABLE_DESC vdesc;
			variable->GetDesc(&vdesc);

			ShaderVariable* shadervariable = new ShaderVariable();
			shadervariable->name = vdesc.Name;
			shadervariable->length = vdesc.Size;
			shadervariable->offset = vdesc.StartOffset;
			mSize += vdesc.Size;
			variables.push_back(shadervariable);
		}
		float val[]{ 1.0 };

		this->buffer = new GPUConstBuffer(device, mSize, (void*)(val));
		tmp_data = new char[mSize];
	}

	ShaderVariable* GetVariableFromName(std::string name)
	{
		for (size_t i = 0; i < variables.size(); i++)
		{
			if (variables[i]->name == name)
				return variables[i];
		}
		return nullptr;
	}

	int GetRegisterIndex()
	{
		return register_index;
	}

	void UpdateVariableValue(std::string name, void *data)
	{
		auto variable = GetVariableFromName(name);
		if (variable == nullptr) return;

		memcpy(tmp_data + variable->offset, data, variable->length);

		//buffer->BindPS(register_index, tmp_data);
	}

	void Bind()
	{
		buffer->BindPS(register_index, tmp_data);
	}

	~ShaderBuffer()
	{
		for (size_t i = 0; i < variables.size(); i++)
			delete variables[i];
		variables.clear();
		variables.shrink_to_fit();

		delete buffer;
	}

private:
	int register_index;
	std::string name;
	SIZE_T mSize;

	std::vector<ShaderVariable*> variables;
	GPUConstBuffer* buffer;
	
	char* tmp_data;
};