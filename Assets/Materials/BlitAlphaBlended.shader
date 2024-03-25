Shader "ImageEffects/BlitAlphaBlended"
{
    Properties
    {
        // _MainTex set by Blit call: https://docs.unity3d.com/ScriptReference/Graphics.Blit.html
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
        // Blend One OneMinusSrcAlpha // Premultiplied transparency
        // Blend One One // Additive
        // Blend OneMinusDstColor One // Soft additive
        // Blend DstColor Zero // Multiplicative
        // Blend DstColor SrcColor // 2x multiplicative

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

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
