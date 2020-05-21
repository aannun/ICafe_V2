struct ps_in
{
	float4 position : SV_Position;
	float2 uv : UVOUT;
};

uniform float test_ext;

cbuffer Test
{
	float test1;
}

float4 main(ps_in IN) : SV_Target
{
	return float4(test1,test_ext,0,0);
}