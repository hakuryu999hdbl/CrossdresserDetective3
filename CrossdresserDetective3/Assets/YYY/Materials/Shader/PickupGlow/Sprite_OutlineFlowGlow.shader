Shader "FTGirl/Sprite Outline Flow Glow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Sprite Tint", Color) = (1,1,1,1)

        [HDR] _OutlineColor ("Outline / Glow Color", Color) = (1.0, 0.45, 0.0, 1.0)
        _OutlineWidth ("Outline Width (px)", Range(1, 8)) = 2

        _BaseGlow ("Base Glow", Range(0, 5)) = 1.0
        _FlowGlow ("Flow Highlight", Range(0, 10)) = 4.0
        _FlowSpeed ("Flow Speed", Range(-5, 5)) = 1.0
        _FlowWidth ("Flow Width", Range(0.02, 0.8)) = 0.18
        _FlowSoftness ("Flow Softness", Range(0.001, 0.3)) = 0.05
        _FlowAngle ("Flow Angle", Range(0, 360)) = 90

        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.05
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
            float4 _MainTex_TexelSize;

            fixed4 _Color;
            fixed4 _OutlineColor;

            float _OutlineWidth;
            float _BaseGlow;
            float _FlowGlow;
            float _FlowSpeed;
            float _FlowWidth;
            float _FlowSoftness;
            float _FlowAngle;
            float _AlphaCutoff;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            float SampleAlpha(float2 uv)
            {
                return tex2D(_MainTex, uv).a;
            }

            // 在透明像素上检测周围是否存在不透明像素，从而生成“外轮廓”
            float GetOuterOutline(float2 uv, float centerAlpha)
            {
                float2 px = _MainTex_TexelSize.xy * _OutlineWidth;

                float a = 0.0;

                // 8方向
                a = max(a, SampleAlpha(uv + float2( px.x,  0)));
                a = max(a, SampleAlpha(uv + float2(-px.x,  0)));
                a = max(a, SampleAlpha(uv + float2( 0,  px.y)));
                a = max(a, SampleAlpha(uv + float2( 0, -px.y)));

                a = max(a, SampleAlpha(uv + float2( px.x,  px.y)));
                a = max(a, SampleAlpha(uv + float2(-px.x,  px.y)));
                a = max(a, SampleAlpha(uv + float2( px.x, -px.y)));
                a = max(a, SampleAlpha(uv + float2(-px.x, -px.y)));

                // 额外半径采样，让 3~8px 时轮廓更连续
                float2 px2 = px * 0.5;
                a = max(a, SampleAlpha(uv + float2( px2.x,  0)));
                a = max(a, SampleAlpha(uv + float2(-px2.x,  0)));
                a = max(a, SampleAlpha(uv + float2( 0,  px2.y)));
                a = max(a, SampleAlpha(uv + float2( 0, -px2.y)));

                // 只保留原图外部
                float outside = 1.0 - smoothstep(_AlphaCutoff, _AlphaCutoff + 0.05, centerAlpha);
                return saturate(a) * outside;
            }

            // 生成沿指定角度不断滚动的高亮带
            float GetFlow(float2 uv)
            {
                float rad = radians(_FlowAngle);
                float2 dir = float2(cos(rad), sin(rad));

                // 将 UV 投影到方向轴上
                float coord = dot(uv - 0.5, dir);

                // 0~1 循环
                float p = frac(coord - _Time.y * _FlowSpeed);

                // 把循环位置转换成到高亮中心的距离
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
                fixed4 sprite = tex2D(_MainTex, IN.uv) * IN.color;

                float outline = GetOuterOutline(IN.uv, sprite.a);
                float flow = GetFlow(IN.uv);

                // 普通轮廓 + 滚动高亮
                float glowIntensity = _BaseGlow + flow * _FlowGlow;

                fixed4 glow = _OutlineColor;
                glow.rgb *= glowIntensity;
                glow.a *= outline;

                // 原图区域优先显示原图；透明区域显示轮廓光
                fixed4 result = sprite;

                float outsideMask = outline * (1.0 - sprite.a);
                result.rgb += glow.rgb * outsideMask;
                result.a = saturate(sprite.a + glow.a);

                return result;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
