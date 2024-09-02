// 3D Masking
float4x4  _MaskMatrix;
sampler2D _MaskTexture;
float4    _MaskChannel;
float3    _MaskStretch;
float2    _MaskInvert;

float CW_GetMask(float3 p)
{
	float2 uv = p.xy + 0.5f;

	float3 a = abs(p * _MaskStretch);
	float  b = max(a.x, max(a.y, a.z));
	float  c = saturate((b - 1.0f) * 1000.0f);
	float  m = dot(tex2D(_MaskTexture, uv), _MaskChannel);

	return saturate(_MaskInvert.x + m * _MaskInvert.y + c);
}

// Local masking
sampler2D _LocalMaskTexture;
float4    _LocalMaskChannel;

float CW_GetLocalMask(float2 uv)
{
	return dot(CW_SampleMip0(_LocalMaskTexture, uv), _LocalMaskChannel);
}

// Depth masking
sampler2D_float _DepthTexture;
float4x4        _DepthMatrix;
float           _DepthBias;

float CW_GetDepthMask(float4 vpos)
{
	vpos /= vpos.w;
	vpos.z = 1.0f - vpos.z;

	float plane = vpos.z + _DepthBias;
	float depth = tex2D(_DepthTexture, vpos.xy).r;

	return _DepthBias > -1.0f ? plane > depth : 1.0f;
}