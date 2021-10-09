﻿Shader "Unlit/Footprint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
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

            sampler2D _MainTex;
            float _Scale;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 mask = tex2Dlod(_MainTex, float4(o.uv, 0, 0));
                v.vertex.y -= mask.r * mask.a * _Scale;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 shiftX = float2(_MainTex_TexelSize.x, 0);
                float2 shiftZ = float2(0, _MainTex_TexelSize.y);

                float texX = tex2D(_MainTex, i.uv.xy + shiftX).r * 2.0 - 1.0;
                float texx = tex2D(_MainTex, i.uv.xy - shiftX).r * 2.0 - 1.0;
                float texZ = tex2D(_MainTex, i.uv.xy + shiftZ).r * 2.0 - 1.0;
                float texz = tex2D(_MainTex, i.uv.xy - shiftZ).r * 2.0 - 1.0;

                // 偏微分により説ベクトルを求める
                float3 du = float3(1.0, (texX - texx), 0.0);
                float3 dv = float3(0.0, (texZ - texz), 1.0);

                float3 n = normalize(cross(dv, du));

                float diff = dot(n, _WorldSpaceLightPos0.xyz);
                diff = max(0.3, diff);

                return float4(diff.xxx, 1.0);
            }
            ENDCG
        }
    }
}
