Shader "FTGirl/Sprite Barrel Warning"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FlashColor ("Warning Color", Color) = (1,0.08,0.025,1)
        _RimWidth ("Rim Width (px)", Range(1,8)) = 4
        [PerRendererData] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        [PerRendererData] _Flip ("Flip", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "Vertex" }
            Lighting On

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex BarrelVert
            #pragma fragment BarrelFrag

            // Match the lighting used by the scene's Spritr_Diffuse material.
            #define _ALPHAPREMULTIPLY_VERTEX_ONLY 1
            #define _FULLRANGE_HARD_RAMP 1
            #define frag LitSpriteFrag
            #include "Assets/Spine/Runtime/spine-unity/Shaders/Sprite/CGIncludes/SpriteVertexLighting.cginc"
            #undef frag

            fixed4 _RendererColor;
            fixed2 _Flip;
            float4 _MainTex_TexelSize;
            half4 _FlashColor;
            half _RimWidth;

            VertexOutput BarrelVert(VertexInput input)
            {
                input.vertex.xy *= _Flip;
                input.color *= _RendererColor;
                return vert(input);
            }

            fixed4 BarrelFrag(VertexOutput input) : SV_Target
            {
                // The existing bomb_on clip lowers green and blue during each flash.
                half flash = saturate(input.color.r - max(input.color.g, input.color.b));
                input.color.gb += flash;
                fixed4 color = LitSpriteFrag(input);

                if (flash > 0 && color.a > 0)
                {
                    float2 pixel = _MainTex_TexelSize.xy * _RimWidth;
                    float2 uv = input.texcoord.xy;
                    half neighbor = min(
                        min(tex2D(_MainTex, uv + float2(pixel.x, 0)).a,
                            tex2D(_MainTex, uv - float2(pixel.x, 0)).a),
                        min(tex2D(_MainTex, uv + float2(0, pixel.y)).a,
                            tex2D(_MainTex, uv - float2(0, pixel.y)).a));
                    half rim = saturate(tex2D(_MainTex, uv).a - neighbor);
                    color.rgb = lerp(color.rgb, _FlashColor.rgb * color.a, flash * rim);
                }

                return color;
            }
            ENDCG
        }
    }
}
