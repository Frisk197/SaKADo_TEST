// Anaglyph depth kit

uniform Texture2DArray<float> dk_DepthTexture;
uniform SamplerState bilinearClampSampler;

uniform float4x4 dk_InvProj;
uniform float4x4 dk_InvView;


float SampleDepthNDC(float2 uv, int eye = 0)
{	
    return dk_DepthTexture.SampleLevel(bilinearClampSampler, float3(uv.xy, eye), 0);
}

float4 HCStoWorldH(float4 hcs, int eye = 0)
{
    return mul(dk_InvView, mul(dk_InvProj, hcs));
}

float3 HCStoNDC(float4 hcs)
{
	return (hcs.xyz / hcs.w) * 0.5 + 0.5;
}

float4 NDCtoHCS(float3 ndc)
{
	return float4(ndc * 2.0 - 1.0, 1);
}

float3 NDCtoWorld(float3 ndc, int eye = 0)
{
    float4 hcs = NDCtoHCS(ndc);
    float4 worldH = HCStoWorldH(hcs, eye);
    return worldH.xyz / worldH.w;
}