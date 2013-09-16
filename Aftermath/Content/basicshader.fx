
struct VsOut
{
    float4 Position   	: SV_POSITION;
    float2 TextureCoords: TEXCOORD0;
	float4 Color		: COLOR;
};

struct PsOut
{
    float4 Color : COLOR0;
};

//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float xAmbient;
bool xShowNormals;
float3 xCamPos;
float3 xCamUp;
float xLighting;




//------- Texture Samplers --------

Texture xTexture;
Texture xTexture2;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
sampler TextureSampler2 = sampler_state { texture = <xTexture2>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//------- Technique: Transparent --------

VsOut BasicVS( float4 inPos : SV_POSITION, float2 inTexCoords: TEXCOORD0, float4 inColor: COLOR)
{	
	VsOut ret = (VsOut)0;
	float4x4 viewProjection = mul (xView, xProjection);
	float4x4 worldViewProjection = mul (xWorld, viewProjection);
    
	ret.Position = mul(inPos, worldViewProjection);	
	ret.TextureCoords = inTexCoords;
    ret.Color = inColor;
	return ret;    
}

PsOut BasicPS(VsOut v) 
{
	PsOut ret = (PsOut)0;
	ret.Color = tex2D(TextureSampler, v.TextureCoords);
	ret.Color *= v.Color;
	return ret;
}

technique Basic
{
	pass Pass0
	{   
		VertexShader = compile vs_4_0 BasicVS();
		PixelShader  = compile ps_4_0 BasicPS();
	}
}

