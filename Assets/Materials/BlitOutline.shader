Shader "ImageEffects/BlitOutline"
{
    Properties
    {
        // _MainTex set by Blit call: https://docs.unity3d.com/ScriptReference/Graphics.Blit.html
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (0, 0, 0, 1)
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            half4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                const int width = 2;  // Need to set const width here so compiler can unroll loops below
                
                float a = 0.0;

                UNITY_UNROLL
                for (int x_offset = -width; x_offset <= width; x_offset++)
                {
                    UNITY_UNROLL
                    for (int y_offset = -width; y_offset <= width; y_offset++)
                    {
                        a += tex2D(_MainTex, float2(i.uv.x + x_offset * _MainTex_TexelSize.x,
                                                    i.uv.y + y_offset * _MainTex_TexelSize.y)).a;
                    }
                }

                a = min(a, 1.0);

                const fixed4 sourceColor = tex2D(_MainTex, i.uv);
                const fixed4 outlineColor = fixed4(_Color.rgb, _Color.a * a);
                
                return lerp(outlineColor, sourceColor, sourceColor.a);
            }
            ENDCG
        }
    }
}
