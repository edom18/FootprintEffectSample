Shader "SimpleStamp/StampProjection"
{
    Properties
    {
        _ProjectTex ("Project Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+11"
        }

        Pass
        {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Offset -1, 1

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

            sampler2D _ProjectTex;
            float4x4 _ProjectionMatrix;
            float4 _MatrixParams; // x = near, y = far, z = vertical scale, w = horizontal scale
            float4 _ViewPos;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
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
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 check = i.projUv.xyz / i.projUv.w;

                #if UNITY_UV_STARTS_AT_TOP
                    check.y = check.y + 1.0;
                #endif

                // X axis
                if (check.x < 0.0 || check.x > 1.0)
                {
                    return float4(0, 0, 0, 0);
                }

                // Y axis
                if (check.y < 0.0 || check.y > 1.0)
                {
                    return float4(0, 0, 0, 0);
                }

                // Z axix
                #if defined(PERSPECTIVE)
                    if (-i.projUv.w < 0.0 || -i.projUv.w > _MatrixParams.y)
                    {
                        return float4(0, 0, 0, 0);
                    }
                #elif defined(ORTHOGONAL)
                    float z = check.z - ((_MatrixParams.y + _MatrixParams.x) / (_MatrixParams.y - _MatrixParams.x));
                    if (z < 0.0 || z > 1.0)
                    {
                        return float4(0, 0, 0, 0);
                    }
                #endif

                float3 dir = normalize(_ViewPos.xyz - i.worldPos.xyz);

                float d = dot(dir, i.normal);

                // if normal-vector faced like back, then return no checkking.
                if (d < 0)
                {
                    return float4(0, 0, 0, 0);
                }

                check.x *= _MatrixParams.w;
                check.y *= _MatrixParams.z;

                return tex2D(_ProjectTex, check.xy);
            }
            ENDCG
        }
    }
}
