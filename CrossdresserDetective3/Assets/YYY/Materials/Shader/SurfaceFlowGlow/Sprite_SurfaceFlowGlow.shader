Shader "FTGirl/Sprite Surface Flow Glow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Sprite Tint", Color) = (1,1,1,1)

        [HDR] _FlowColor ("Flow Color", Color) = (1.0, 0.65, 0.2, 1.0)
        _FlowIntensity ("Flow Intensity", Range(0, 8)) = 2.0
        _FlowSpeed ("Flow Speed", Range(-5, 5)) = 1.0
        _FlowWidth ("Flow Width", Range(0.01, 0.8)) = 0.18
        _FlowSoftness ("Flow Softness", Range(0.001, 0.4)) = 0.06
        _FlowAngle ("Flow Angle", Range(0, 360)) = 90

        _BaseBrightness ("Base Brightness", Range(0, 2)) = 1.0
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.0
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.0

        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.01
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
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            fixed4 _FlowColor;

            float _FlowIntensity;
            float _FlowSpeed;
            float _FlowWidth;
            float _FlowSoftness;
            float _FlowAngle;

            float _BaseBrightness;
            float _PulseAmount;
            float _PulseSpeed;
            float _AlphaCutoff;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            float GetFlowBand(float2 uv)
            {
                float rad = radians(_FlowAngle);
                float2 dir = float2(cos(rad), sin(rad));

                // 把 UV 投影到指定方向轴上
                float coord = dot(uv - 0.5, dir);

                // 循环滚动
                float p = frac(coord - _Time.y * _FlowSpeed);

                // 计算距离光带中心的距离
                float d = abs(p - 0.5);

                float halfWidth = _FlowWidth * 0.5;

                return 1.0 - smoothstep(
                    halfWidth,
                    halfWidth + _FlowSoftness,
                    d
                );
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, IN.uv) * IN.color;

                // 完全透明区域直接丢弃，避免任何外轮廓
                clip(col.a - _AlphaCutoff);

                float flow = GetFlowBand(IN.uv);

                // 可选轻微呼吸
                float pulse = 1.0;
                if (_PulseAmount > 0.0001)
                {
                    pulse += sin(_Time.y * _PulseSpeed) * _PulseAmount;
                }

                // 基础亮度
                col.rgb *= _BaseBrightness * pulse;

                // 只在 Sprite 自身表面叠加流光
                // 乘 alpha 确保半透明边缘也不会产生明显外溢
                col.rgb += _FlowColor.rgb
                         * flow
                         * _FlowIntensity
                         * col.a;

                return col;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
