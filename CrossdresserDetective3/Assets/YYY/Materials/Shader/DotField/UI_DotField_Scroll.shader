Shader "FTGirl/UI DotField Scroll"
{
 Properties
 {
  [PerRendererData]_MainTex("Dot Texture",2D)="white"{}
  _Color("Tint",Color)=(1,1,1,1)
  _TilingX("Tiling X",Range(.25,8))=1
  _TilingY("Tiling Y",Range(.25,8))=1
  _SpeedX("Scroll Speed X",Range(-2,2))=.03
  _SpeedY("Scroll Speed Y",Range(-2,2))=0
  _PulseAmount("Pulse Amount",Range(0,1))=.12
  _PulseSpeed("Pulse Speed",Range(0,10))=1.2
  _WaveAmount("Wave Amount",Range(0,.05))=.002
  _WaveSpeed("Wave Speed",Range(0,10))=.8
  _Alpha("Global Alpha",Range(0,1))=1
  _StencilComp("Stencil Comparison",Float)=8
  _Stencil("Stencil ID",Float)=0
  _StencilOp("Stencil Operation",Float)=0
  _StencilWriteMask("Stencil Write Mask",Float)=255
  _StencilReadMask("Stencil Read Mask",Float)=255
  _ColorMask("Color Mask",Float)=15
 }
 SubShader
 {
  Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="False"}
  Stencil{Ref[_Stencil] Comp[_StencilComp] Pass[_StencilOp] ReadMask[_StencilReadMask] WriteMask[_StencilWriteMask]}
  Cull Off Lighting Off ZWrite Off ZTest[unity_GUIZTestMode]
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask[_ColorMask]

  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   #include "UnityUI.cginc"
   #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

   struct appdata{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
   struct v2f{float4 vertex:SV_POSITION;fixed4 color:COLOR;float2 uv:TEXCOORD0;float4 worldPosition:TEXCOORD1;};

   sampler2D _MainTex;
   fixed4 _Color;
   float _TilingX,_TilingY,_SpeedX,_SpeedY,_PulseAmount,_PulseSpeed,_WaveAmount,_WaveSpeed,_Alpha;
   float4 _ClipRect;

   v2f vert(appdata IN)
   {
    v2f o;
    o.worldPosition=IN.vertex;
    o.vertex=UnityObjectToClipPos(IN.vertex);
    o.uv=IN.uv;
    o.color=IN.color*_Color;
    return o;
   }

   fixed4 frag(v2f IN):SV_Target
   {
    float t=_Time.y;
    float2 uv=IN.uv*float2(_TilingX,_TilingY);
    uv+=float2(_SpeedX,_SpeedY)*t;

    uv.x+=sin(t*_WaveSpeed+IN.uv.y*6.28318)*_WaveAmount;
    uv.y+=cos(t*_WaveSpeed*.73+IN.uv.x*6.28318)*_WaveAmount;

    fixed4 col=tex2D(_MainTex,uv)*IN.color;
    float pulse=1.0+sin(t*_PulseSpeed)*_PulseAmount;
    col.rgb*=pulse;
    col.a*=_Alpha;

    #ifdef UNITY_UI_CLIP_RECT
     col.a*=UnityGet2DClipping(IN.worldPosition.xy,_ClipRect);
    #endif

    return col;
   }
   ENDCG
  }
 }
 FallBack "UI/Default"
}