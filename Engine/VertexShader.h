#pragma once

#include "Device.h"

class VertexShader
{
public:
	VertexShader(Device& device, const void* byte_code, SIZE_T byte_code_size, D3D11_INPUT_ELEMENT_DESC* layout, UINT layout_size) :
		device(device)
	{
		if (device.GetDXHandle()->CreateVertexShader(byte_code, byte_code_size, nullptr, &vertex_shader) != S_OK)
		{
			icafe::Die("unable to create vertex shader");
		}
		// not the input layout is validated with the shader bytecode/opcodes
		if (device.GetDXHandle()->CreateInputLayout(layout, layout_size, byte_code, byte_code_size, &input_layout) != S_OK)
		{
			icafe::Die("invalid input layout");
		}
	}

	void Bind()
	{
		device.GetDXContext()->IASetInputLayout(input_layout);
		device.GetDXContext()->VSSetShader(vertex_shader, nullptr, 0);
	}

	~VertexShader()
	{
		vertex_shader->Release();
		input_layout->Release();
	}

private:
	Device& device;
	ID3D11VertexShader *vertex_shader;
	ID3D11InputLayout* input_layout;
};
