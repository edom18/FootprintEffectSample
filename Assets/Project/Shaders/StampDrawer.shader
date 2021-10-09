Shader "SimpleStamp/StampDrawer"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile PERSPECTIVE ORTHOGONAL

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 projUv : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _ProjectTex;

            float4x4 _ProjectionMatrix;
            float4 _MainTex_ST;
            float4 _MatrixParams; // x = near, y = far, z = vertical scale, w = horizontal scale
            float4 _ViewPos;

            v2f vert(appdata v)
            {
                v2f o;

                float2 uv2 = v.uv2;

                #if UNITY_UV_STARTS_AT_TOP
                    uv2.y = 1.0 - uv2.y;
                #endif
                uv2 = uv2 * 2.0 - 1.0;

                o.vertex = float4(uv2, 0.0, 1.0);
                o.uv = v.uv;
                o.uv2 = v.uv2;
                o.normal = normalize(mul(v.normal, unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                float4x4 mat = mul(_ProjectionMatrix, unity_ObjectToWorld);
                o.projUv = mul(mat, v.vertex);
                #if UNITY_UV_STARTS_AT_TOP
                    o.projUv.y *= -1.0;
                #endif

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 check = i.projUv.xyz / i.projUv.w;

                #if !UNITY_UV_STARTS_AT_TOP
                    check.y = check.y - 1.0;
                #endif

                // X axis
                if (check.x < 0.0 || check.x > 1.0)
                {
                    return tex2D(_MainTex, i.uv2);
                }

                // Y axis
                if (check.y < -1.0 || check.y > 0.0)
                {
                    return tex2D(_MainTex, i.uv2);
                }

                // Z axix
                #if defined(PERSPECTIVE)
                    if (-i.projUv.w < 0.0 || -i.projUv.w > _MatrixParams.y)
                    {
                        return tex2D(_MainTex, i.uv2);
                    }
                #elif defined(ORTHOGONAL)
                    float z = check.z - ((_MatrixParams.y + _MatrixParams.x) / (_MatrixParams.y - _MatrixParams.x));
                    if (z < 0.0 || z > 1.0)
                    {
                        return tex2D(_MainTex, i.uv2);
                    }
                #endif

                float3 dir = normalize(_ViewPos.xyz - i.worldPos.xyz);

                float d = dot(dir, i.normal);

                // if normal-vector faced like back, then return no checkking.
                if (d < 0)
                {
                    return tex2D(_MainTex, i.uv2);
                }

                fixed4 col = tex2D(_MainTex, i.uv2);
                check.x *= _MatrixParams.w;
                check.y += 1.0;
                check.y *= _MatrixParams.z;

                fixed4 proj = tex2D(_ProjectTex, check.xy);
                
                return lerp(col, proj, proj.a);
            }
            ENDCG
        }
    }
}
