Shader "Unlit/RayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float flash = sin(_Time.y * 3.0) * 0.3 + 0.7;
                col *= flash;

                /*
                // 添加通道偏移
                float3 offset = float3(0.01, 0.05, 0.0); // X轴偏移，可根据需要调整
                fixed4 rCol = tex2D(_MainTex, i.uv + offset.xy); // R通道偏移
                fixed4 gCol = tex2D(_MainTex, i.uv); // G通道正常
                fixed4 bCol = tex2D(_MainTex, i.uv - offset.xy); // B通道偏移
                col = fixed4(rCol.r, gCol.g, bCol.b, col.a);
                */
                
                return col;
            }
            ENDCG
        }
    }
}