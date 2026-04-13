Shader "Custom/BlackPulsingSmoke"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (0,0,0,1)
        _Speed ("Scroll Speed", Float) = 1
        _Scale ("UV Scale", Float) = 1
        _Distort ("Distort Strength", Float) = 0.05
        _AlphaMin ("Min Alpha", Range(0,1)) = 0.3
        _AlphaMax ("Max Alpha", Range(0,1)) = 1.0
        _RandOffset ("Rand Phase Offset", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;
            float _Scale;
            float _Distort;
            float _AlphaMin;
            float _AlphaMax;
            float _RandOffset;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed + _RandOffset;

                float2 offset = float2(
                    sin(time * 0.8),
                    cos(time * 1.1)
                ) * _Distort;

                float2 uv = i.uv * _Scale + offset;

                fixed4 tex = tex2D(_MainTex, uv);
                
                // 计算动态 alpha
                float alphaPulse = (sin(time * 2.0) * 0.5 + 0.5); // 0~1
                float finalAlpha = lerp(_AlphaMin, _AlphaMax, alphaPulse);
                
                tex *= _Color;
                tex.a *= finalAlpha;

                return tex;
            }
            ENDCG
        }
    }
}