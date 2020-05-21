#pragma once

#include "GPUBuffer.h"
#include "GPUConstBuffer.h"

class Mesh
{
public:
	Mesh(Device& device, UINT nvertices) : device(device), nvertices(nvertices)
	{

	}

	void AddBuffer(UINT size, UINT stride, void *data)
	{
		auto buffer = new GPUBuffer(device, size, stride, data);
		buffers.push_back(buffer);
	}

	void Draw()
	{
		for (int i = 0; i < buffers.size(); i++)
		{
			buffers[i]->Bind(i);
		}

		device.GetDXContext()->Draw(nvertices, 0);
	}

	~Mesh()
	{
		for (size_t i = 0; i < buffers.size(); i++)
			delete buffers[i];
		buffers.clear();
		buffers.shrink_to_fit();
	}

private:
	std::vector<GPUBuffer*> buffers;
	Device& device;
	UINT nvertices;

};