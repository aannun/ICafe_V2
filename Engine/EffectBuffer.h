#pragma once

#include "Mesh.h"
#include "VertexShader.h"
#include "PixelShader.h"
#include "ShaderObject.h"
#include "RenderData.h"

class EffectBuffer
{
public:
	EffectBuffer(Device &device, VertexShader &vertex_shader, PixelShader &pixel_shader, UINT width, UINT height) : vertex_shader(vertex_shader), pixel_shader(pixel_shader)
	{
		//create screen data
		screen = new Mesh(device, 6);
		float screen_vertices[] = {
			-1, 1,
			1, 1,
			-1, -1,
			1, 1,
			1, -1,
			-1, -1
		};
		float screen_uvs[] = {
			0, 0,
			1, 0,
			0, 1,
			1, 0,
			1, 1,
			0, 1
		};

		screen->AddBuffer(sizeof(screen_vertices), sizeof(float) * 2, screen_vertices);
		screen->AddBuffer(sizeof(screen_uvs), sizeof(float) * 2, screen_uvs);

		render_data = new Render_Data(device, width, height);
	}

	ShaderObject* Draw(ShaderObject *render_object)
	{
		render_data->render_target->Clear({ 0,0,0,0 });

		vertex_shader.Bind();
		pixel_shader.Bind();
		pixel_shader.BindResources();

		render_data->render_target->Bind();
		if (render_object) render_object->Bind(0);

		screen->Draw();

		return render_data->render_target_object;
	}

	ShaderObject* GetShaderObject()
	{
		return render_data->render_target_object;
	}

	~EffectBuffer()
	{
		delete screen;
		delete render_data;
	}

private:
	Mesh* screen;
	Render_Data* render_data;

	VertexShader& vertex_shader;
	PixelShader& pixel_shader;
};