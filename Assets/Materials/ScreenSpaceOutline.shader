Shader "ImageEffects/ScreenSpaceOutline"
{
    Properties
    {
        // _MainTex set by Blit call: https://docs.unity3d.com/ScriptReference/Graphics.Blit.html
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
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
            sampler2D _CameraDepthTexture;
            half4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                const int width = 2;  // Need to set const width here so compiler can unroll loops below
                
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float x = 1 / _ScreenParams.x;
                float y = 1 / _ScreenParams.y;

                float a = 0.0;

                UNITY_UNROLL
                for (int x_offset = -width; x_offset <= width; x_offset++)
                {
                    UNITY_UNROLL
                    for (int y_offset = -width; y_offset <= width; y_offset++)
                    {
                        // a is actually depth_sum. Also accumulate depth_hits to calculate transparency?
                        a += tex2D(_CameraDepthTexture, float2(i.uv.x + x_offset*x, i.uv.y + y_offset*y)).r;
                    }
                }

                // TODO: don't use hardcoded 25 to take average. Fix issue with inner outline on merged sprites
                if (tex2D(_CameraDepthTexture, i.uv).r >= (a / 25))
                {
                    return col;
                }

                if (a < 0.01)
                {
                    return col;
                }
                
                a = min(a, 1.0);

                // TODO: don't adjust alpha of color, instead blend between _MainTex and outline color

                return fixed4(_Color.rgb, _Color.a*a);
            }
            ENDCG
        }
    }
}
