#pragma once

#include "View.h"

#include "screen_vs.h"
#include "screen_ps.h"

#include <fstream>

namespace icafe
{
	class Context
	{
	public:
		Context()
		{
			Adapter adapter;
			device = new Device(adapter);

			D3D11_INPUT_ELEMENT_DESC screen_layout_desc[] =
			{
				{"POSITION", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
				{"UVIN", 0, DXGI_FORMAT_R32G32_FLOAT, 1, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
			};

			screen_vertex_shader = new VertexShader(*device, screen_vs, sizeof(screen_vs), screen_layout_desc, _countof(screen_layout_desc));
			screen_pixel_shader = new PixelShader(*device, screen_ps, sizeof(screen_ps));
		}

		void* CreateView(UINT width, UINT height)
		{
			return new View(*device, *screen_vertex_shader, *screen_pixel_shader, width, height);
		}

		void* CreateBuffer(UINT width, UINT height, PixelShader *pixel_shader)
		{
			return new EffectBuffer(*device, *screen_vertex_shader, *pixel_shader, width, height);
		}

		void* CreatePixelShader(void* byte_code, size_t len)
		{
			/*auto p = new PixelShader(*device, byte_code, len);
			p->UpdateTexture(0, nullptr);*/

			return new PixelShader(*device, byte_code, len);
		}

		void* CreateGPUBuffer(UINT size, void* data = nullptr)
		{
			return new GPUConstBuffer(*device, size, data);
		}

		void ClearState()
		{
			device->ClearState();
		}

		~Context()
		{
			delete screen_vertex_shader;
			delete screen_pixel_shader;
			delete device;
		}

		Device* GetDXHandle()
		{
			return device;
		}

	private:
		Device* device;
		VertexShader* screen_vertex_shader;
		PixelShader* screen_pixel_shader;
	};

	struct EffectsContainer
	{
		EffectBuffer effects[10];
	};

	static std::vector<char> ReadAllBytes(char const* filename)
	{
		std::ifstream ifs(filename, std::iostream::binary);
		std::vector<char> bytes((std::istreambuf_iterator<char>(ifs)), (std::istreambuf_iterator<char>()));

		return bytes;
	}

	extern "C" __declspec(dllexport) void* __stdcall Init()
	{
		return new Context();
	}

	extern "C" __declspec(dllexport) void* __stdcall CreateView(Context *context, UINT width, UINT height)
	{
		return context->CreateView(width, height);
	}

	extern "C" __declspec(dllexport) void* __stdcall CreateEffectBuffer(Context* context, UINT width, UINT height, PixelShader* shader)
	{
		return context->CreateBuffer(width, height, shader);
	}

	extern "C" __declspec(dllexport) void __stdcall AddViewEffect(View* view, EffectBuffer* effect)
	{
		view->AddEffect(effect);
	}

	extern "C" __declspec(dllexport) void __stdcall DrawView(View* view)
	{
		view->Draw();
	}

	extern "C" __declspec(dllexport) void __stdcall DrawEffectBuffer(EffectBuffer *buffer)
	{
		buffer->Draw(nullptr);
	}

	extern "C" __declspec(dllexport) void* __stdcall CreatePixelShader(Context* context, char* byte_code)
	{
		std::vector<char> bytes = ReadAllBytes(byte_code);
		return context->CreatePixelShader(bytes.data(), bytes.size());
	}

	extern "C" __declspec(dllexport) void __stdcall DestroyView(View *view)
	{
		delete view;
	}

	extern "C" __declspec(dllexport) void __stdcall DestroyBuffer(EffectBuffer *buffer, PixelShader *shader)
	{
		delete shader;
		delete buffer;
	}

	extern "C" __declspec(dllexport) void __stdcall DieMessage(char* message)
	{
		icafe::Die(message);
	}

	extern "C" __declspec(dllexport) void __stdcall UpdateShaderValue(PixelShader *shader, UINT buffer_register, char* name, void* data)
	{
		shader->UpdateVariableValue(buffer_register, name, data);
	}

	extern "C" __declspec(dllexport) void* __stdcall CreateTexture(Context *context, UINT width, UINT height, UINT pitch, void* data, UINT format = 87)
	{
		Texture *texture = new Texture(*context->GetDXHandle(), width, height, (DXGI_FORMAT)format);
		texture->Upload(data, pitch);

		return texture;
	}

	extern "C" __declspec(dllexport) void __stdcall SetTexture(PixelShader *shader, UINT buffer_register, Texture* texture)
	{
		shader->UpdateTexture(buffer_register, texture);
	}
	
	extern "C" __declspec(dllexport) void __stdcall DestroyTexture(Texture *texture)
	{
		delete texture;
	}

	extern "C" __declspec(dllexport) void __stdcall ClearContext(Context* context)
	{
		context->ClearState();
	}
}