#ifndef __EDGE_DETECT__
#define __EDGE_DETECT__

TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

float _EdgeDetectIntensity;
float _KernelOffsetScale;

float4 Frag(VaryingsDefault i) : SV_Target
{
    const float offset = 1.0 / 300.0;

    float3 d = offset * float3(-1, 0, 1) * _KernelOffsetScale;

    float2 uv = i.texcoord;

    float4 col = float4(0.0, 0.0, 0.0, 0.0);

    col =  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xx) * _EdgeDetectIntensity;
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xy) * _EdgeDetectIntensity;
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.xz) * _EdgeDetectIntensity;

    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.yx) * _EdgeDetectIntensity;
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * (1 - 9 * _EdgeDetectIntensity);
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.yz) * _EdgeDetectIntensity;

    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zx) * _EdgeDetectIntensity;
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zy) * _EdgeDetectIntensity;
    col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d.zz) * _EdgeDetectIntensity;

    return col;
}

#endif