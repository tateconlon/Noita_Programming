// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//Modified from: https://www.sector12games.com/skewshear-vertex-shader/
//Also check out: https://www.senocular.com/flash/tutorials/transformmatrix/
Shader "Sprite/Sprite Cutout Shadow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        
        _HorizontalSkew ("Horizontal Skew", Float) = 0
        _VerticalSkew ("Vertical Skew", Float) = 0
        
        _XScale ("X Scale", Float) = 1
        _YScale ("Y Scale", Float) = 1
    }
    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            
            //Needed otherwise we batch and the skewing verticies math doesn't work because it's one big mesh
            //https://docs.unity3d.com/Manual/dynamic-batching.html
            "DisableBatching"="True"    
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            float _HorizontalSkew;
            float _VerticalSkew;
            float _XScale;
            float _YScale;

        
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                
                // Create a skew transformation matrix
                float h = _HorizontalSkew;
                float v = _VerticalSkew;
                float xs = _XScale;
                float ys = _YScale;

                //https://www.senocular.com/flash/tutorials/transformmatrix/
                //matrix math... 4x4 * 4x1. If you multiply it out you get
                // x = xs*x + h*y
                // y = v*x + y*ys
                float4x4 transformMatrix = float4x4(
                    xs,h,0,0,
                    v,ys,0,0,
                    0,0,1,0,
                    0,0,0,1);
                
                float4 skewedVertex = mul(transformMatrix, IN.vertex);
                OUT.vertex = UnityObjectToClipPos(skewedVertex);
                
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                return OUT;
            }
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord);
                c.rgb = _Color.rgb;
                c.a = c.a * _Color.a;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
