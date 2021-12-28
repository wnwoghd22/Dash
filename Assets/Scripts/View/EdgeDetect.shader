Shader "Hidden/Custom/EdgeDetect"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    //const float offset = 1.0 / 300.0;

    //float2 offsets[9] = {
    //    float2(-offset, offset), // top-left
    //    float2(0.0f, offset), // top-center
    //    float2(offset, offset), // top-right
    //    float2(-offset, 0.0f),   // center-left
    //    float2(0.0f, 0.0f),   // center-center
    //    float2(offset, 0.0f),   // center-right
    //    float2(-offset, -offset), // bottom-left
    //    float2(0.0f, -offset), // bottom-center
    //    float2(offset, -offset)  // bottom-right    
    //};

    float _EdgeDetectIntensity;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        const float offset = 1.0 / 300.0;

        float2 offsets[9] = {
            float2(-offset, offset), // top-left
            float2(0.0f, offset), // top-center
            float2(offset, offset), // top-right
            float2(-offset, 0.0f),   // center-left
            float2(0.0f, 0.0f),   // center-center
            float2(offset, 0.0f),   // center-right
            float2(-offset, -offset), // bottom-left
            float2(0.0f, -offset), // bottom-center
            float2(offset, -offset)  // bottom-right    
        };
        /*float kernel[9] = {
            -1, -1, -1,
            -1, 9, -1,
            -1, -1, -1
        };*/
        float kernel[9] = {
            1, 1, 1,
            1, -8, 1,
            1, 1, 1
        };


        float2 uv = i.texcoord;

        float4 sampleTex[9];

        int idx;

        for (idx = 0; idx < 9; idx++)
        {
            //sampleTex[i] = vec3(texture(screenTexture, TexCoords.st + offsets[i]));
            sampleTex[idx] = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offsets[idx]);
            //sampleTex[idx] = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0.001, 0.001));
        }
        float4 col = float4(0.0, 0.0, 0.0, 0.0);

        for (idx = 0; idx < 9; idx++)
        {
            //col += (sampleTex[idx] * kernel[idx]);
            if (idx == 4)
                col += (sampleTex[idx] * (1 - 9 * _EdgeDetectIntensity));
            else
                col += (sampleTex[idx] * _EdgeDetectIntensity);
        }

        return col;
        //return float4(col.rgb, 1.0);

        //return sampleTex[0];
    }

        ENDHLSL

        SubShader
    {
        Cull Off ZWrite Off ZTest Always

            Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}