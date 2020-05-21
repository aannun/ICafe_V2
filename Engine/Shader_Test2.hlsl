Texture2D buffer : register(t0);
SamplerState ss : register(s0);

struct ps_in
{
	float4 position : SV_Position;
	float2 uv : UVOUT;
};

float test;

cbuffer Test : register(b0)
{
	float test1;
	float ss3;
}

float4 main(ps_in IN) : SV_Target
{
	float4 color = buffer.Sample(ss, IN.uv);
	return color + float4(0,1,0,0);
}