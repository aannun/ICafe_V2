
struct vs_in
{
	float2 position : POSITION;
	float2 uv : UVIN;
};

struct vs_out
{
	float4 position : SV_Position;
	float2 uv : UVOUT;
};

vs_out main(vs_in IN)
{
	vs_out OUT;
	OUT.position = float4(IN.position, 0, 1);
	OUT.uv = IN.uv;
	return OUT;
}
