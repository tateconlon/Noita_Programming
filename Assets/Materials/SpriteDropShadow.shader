Shader "Sprite/Sprite Drop Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Shadow Color", Color) = (0, 0, 0, 1)
         //Color Alpha is used so it's easy to tweak in inspector 
        //what the shadow alpha is since shadow is almost always black
        _ColorAlpha ("Shadow Alpha", float) = 0.4  
    }
    
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always
        Blend One OneMinusSrcAlpha
        
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
            fixed4 _Color;
            float _ColorAlpha;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = col.a * _ColorAlpha;
                col.rgb = _Color.rgb * col.a;
                return col;
            }
            ENDCG
        }
    }
}
