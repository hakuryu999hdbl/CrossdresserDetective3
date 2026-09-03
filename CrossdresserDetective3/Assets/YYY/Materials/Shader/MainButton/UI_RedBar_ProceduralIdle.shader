Shader "FTGirl/UI RedBar Procedural Idle"
{
 Properties
 {
  [PerRendererData]_MainTex("Sprite Texture",2D)="white"{}
  _Color("Tint",Color)=(1,1,1,1)
  _EdgeWidth("Edge Width",Range(.01,.5))=.2
  _EdgeFade("Edge Fade",Range(0,1))=.3
  _Speed("Speed",Range(0,10))=1.5
  _Density("Particle Density",Range(4,240))=60
  _Amount("Particle Amount",Range(0,1))=.55
  _Softness("Softness",Range(.001,.3))=.07
  _DriftX("Horizontal Drift",Range(-5,5))=.8
  _DriftY("Vertical Drift",Range(-5,5))=.25
  _LeftAmount("Left Amount",Range(0,1))=1
  _RightAmount("Right Amount",Range(0,1))=1
  _StencilComp("Stencil Comparison",Float)=8
  _Stencil("Stencil ID",Float)=0
  _StencilOp("Stencil Operation",Float)=0
  _StencilWriteMask("Stencil Write Mask",Float)=255
  _StencilReadMask("Stencil Read Mask",Float)=255
  _ColorMask("Color Mask",Float)=15
 }
 SubShader
 {
  Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
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
   float _EdgeWidth,_EdgeFade,_Speed,_Density,_Amount,_Softness,_DriftX,_DriftY,_LeftAmount,_RightAmount;
   float4 _ClipRect;

   float hash21(float2 p)
   {
    p=frac(p*float2(123.34,456.21));
    p+=dot(p,p+45.32);
    return frac(p.x*p.y);
   }

   float noise(float2 p)
   {
    float2 i=floor(p),f=frac(p);
    f=f*f*(3.-2.*f);
    return lerp(lerp(hash21(i),hash21(i+float2(1,0)),f.x),
                lerp(hash21(i+float2(0,1)),hash21(i+1),f.x),f.y);
   }

   float fbm(float2 p)
{
    float v = 0;

    v += noise(p) * 0.46;
    v += noise(p * 2.03 + 17.1) * 0.26;
    v += noise(p * 4.07 + 31.7) * 0.16;

    // 新增超细颗粒层
    v += noise(p * 8.13 + 57.3) * 0.08;
    v += noise(p * 16.27 + 91.4) * 0.04;

    return saturate(v);
}

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
    fixed4 col=tex2D(_MainTex,IN.uv)*IN.color;
    float t=_Time.y*_Speed;

    float left=(1-smoothstep(0,max(_EdgeWidth,.0001),IN.uv.x))*_LeftAmount;
    float right=(1-smoothstep(0,max(_EdgeWidth,.0001),1-IN.uv.x))*_RightAmount;
    float edge=saturate(max(left,right));

    float2 p=IN.uv*_Density+float2(t*_DriftX,t*_DriftY);
    float n1=fbm(p);
    float n2=fbm(p*.67+float2(-t*.61,t*.37)+23.7);

    // 每个小区域还有自己的时间闪动，所以不是单纯整张 Noise 平移
    float2 cell=floor(IN.uv*_Density);
    float flicker=sin(t*3.2+hash21(cell)*12.0)*.5+.5;
    float n=saturate(n1*.55+n2*.30+flicker*.15);

    float threshold=lerp(.25,.82,_Amount);
    float particles=smoothstep(threshold-_Softness,threshold+_Softness,n);

    // 中间永远保持原图；左右边缘才被动态颗粒侵蚀
    float dynamicEdge=lerp(_EdgeFade,1.0,particles);
    col.a*=lerp(1.0,dynamicEdge,edge);

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