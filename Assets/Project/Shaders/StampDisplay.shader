Shader "SimpleStamp/StampDisplay"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+10"
        }

        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv2 : TEXCOORD1;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;

                // Expand a vertex with normal because this shader will draw same position of base model.
                // So it will abord Z fight.
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * 0.0001);
                o.uv = v.uv2;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                if (col.a > 0.0)
                {
                    col.a = 1.0;
                }
                return col;
            }
            ENDCG
        }
    }
}
