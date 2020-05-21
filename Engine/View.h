#pragma once

#include "Window.h"
#include "RenderTarget.h"
#include "Mesh.h"
#include "VertexShader.h"
#include "PixelShader.h"
#include "ShaderObject.h"
#include "EffectBuffer.h"

class View
{
public:
	View(Device &device, VertexShader &vertex_shader, PixelShader &pixel_shader, UINT width, UINT height) : screen_vertex_shader(vertex_shader), screen_pixel_shader(pixel_shader)
	{
		window = new Window(device, width, height, "ICafe");

		swap_chain = new Texture(*window);
		swap_chain_render_target = new RenderTarget(*swap_chain);

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
	}

	void Draw()
	{
		ShaderObject* renderer = nullptr;
		for (size_t i = 0; i < effects.size(); i++)
			renderer = effects[i]->Draw(renderer);

		swap_chain_render_target->Clear({ 0, 0, 0, 1 });
		swap_chain_render_target->Bind();

		screen_vertex_shader.Bind();
		screen_pixel_shader.Bind();

		if (renderer) renderer->Bind(0);

		screen->Draw();

		window->Present();

		ClearEffects();
	}

	void AddEffect(EffectBuffer *effect)
	{
		effects.insert(effects.end(), effect);
	}

	void ClearEffects()
	{
		effects.clear();
	}

	~View()
	{
		delete swap_chain_render_target;
		delete swap_chain;
		delete window;
		delete screen;

		for (size_t i = 0; i < effects.size(); i++)
			delete effects[i];
		effects.clear();
		effects.shrink_to_fit();
	}

private:
	Window* window;

	Mesh* screen;
	Texture* swap_chain;
	RenderTarget* swap_chain_render_target;

	VertexShader& screen_vertex_shader;
	PixelShader& screen_pixel_shader;

	std::vector<EffectBuffer*> effects;
};