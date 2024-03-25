Shader "Limitless/SimpleBloom"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_MainTex);
	SAMPLER(sampler_MainTex);
	TEXTURE2D_X(_BlurTex);
	SAMPLER(sampler_BlurTex);
	CBUFFER_START(UnityPerMaterial)
	half4 _MainTex_TexelSize;
	CBUFFER_END
	half BlurAmount;
	half4 BloomColor;
	half BloomAmount;
	half Threshold;

	struct appdata {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};
	struct v2f {
		half4 pos : SV_POSITION;
		half2 uv  : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};
	struct v2f_Blur {
		half4 pos : SV_POSITION;
		half4 uv : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata i)
	{
		v2f o = (v2f)0;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, half4(i.pos.xyz, 1.0h)));
		o.uv = i.uv;
		return o;
	}

	v2f_Blur vertBlur(appdata i)
	{
		v2f_Blur v = (v2f_Blur)0;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(v);
		half2 uv = i.uv;
		v.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, half4(i.pos.xyz, 1.0h)));
		half2 offset = _MainTex_TexelSize.xy * BlurAmount;
		v.uv = half4(uv - offset, uv + offset);
		return v;
	}
	half4 MultiSampleTexture(half4 uv) {
		half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv.xy));
		color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv.xw));
		color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv.zy));
		color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, UnityStereoTransformScreenSpaceTex(uv.zw));
		color *= 0.25h;
		return color;
	}

	half4 Bloom(v2f_Blur i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 color = MultiSampleTexture(i.uv);
		half color2 = max(color.r, max(color.g, color.b));
		half result = max(0.0h, color2 - Threshold) / max(color2, 0.00001h);
		return color * result;
	}

		half4 Blur(v2f_Blur i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 result = MultiSampleTexture(i.uv);
		return result;
	}

		half4 frag(v2f i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		half2 stUv = UnityStereoTransformScreenSpaceTex(i.uv);
		half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, stUv);
		half4 bloom = SAMPLE_TEXTURE2D_X(_BlurTex, sampler_BlurTex, stUv) * BloomAmount * BloomColor;
		return bloom + color ;
	}

		ENDHLSL

		Subshader
	{
		Pass
		{
		  ZTest Always Cull Off ZWrite Off
		  HLSLPROGRAM
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma vertex vertBlur
		  #pragma fragment Bloom
		  ENDHLSL
		}

			Pass
		{
		  ZTest Always Cull Off ZWrite Off
		  HLSLPROGRAM
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma vertex vertBlur
		  #pragma fragment Blur
		  ENDHLSL
		}

			Pass
		{
		  ZTest Always Cull Off ZWrite Off
		  HLSLPROGRAM
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma vertex vert
		  #pragma fragment frag
		  ENDHLSL
		}
	}
	Fallback off
}