Shader "ImageEffects/Shadows"
{
    Properties
    {
        _ShadowAccumulationTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (0.1, 0.1, 0.1, 0.2)
        _OffsetX("Pixel Offset X", Int) = 4
        _OffsetY("Pixel Offset X", Int) = -4
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _ShadowAccumulationTex;
            float4 _ShadowAccumulationTex_TexelSize;
            half4 _Color;
            int _OffsetX;
            int _OffsetY;

            fixed4 frag (v2f i) : SV_Target
            {
                const float2 offset = float2(_OffsetX * _ShadowAccumulationTex_TexelSize.x,
                                             _OffsetY * _ShadowAccumulationTex_TexelSize.y);
                const fixed sourceAlpha = tex2D(_ShadowAccumulationTex, i.uv - offset).a;
                return fixed4(_Color.rgb, _Color.a * sourceAlpha);
            }
            ENDCG
        }
    }
}
