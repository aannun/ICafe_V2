#pragma once

#include "RenderTarget.h"
#include "ShaderObject.h"

class Render_Data
{
public:
	Render_Data(Device &device, UINT width, UINT height)
	{
		render_target_texture = new Texture(device, width, height);
		render_target = new RenderTarget(*render_target_texture);
		render_target_object = new ShaderObject(*render_target_texture);
	}

	~Render_Data()
	{
		delete render_target_object;
		delete render_target;
		delete render_target_texture;
	}

	Texture* render_target_texture;
	ShaderObject* render_target_object;
	RenderTarget* render_target;
};