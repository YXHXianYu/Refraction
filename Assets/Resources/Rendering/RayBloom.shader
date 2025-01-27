Shader "Custom/SelectiveBloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BloomIntensity ("Bloom Intensity", Float) = 1.0
        _BloomLayerMask ("Bloom Layer Mask", Int) = 6
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint layer : SV_RenderTargetArrayIndex; // 获取当前像素的层信息
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                uint layer : TEXCOORD1; // 传递层信息到片段着色器
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BloomIntensity;
            int _BloomLayerMask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.layer = v.layer; // 传递层信息
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 检查当前像素是否属于Bloom层
                if (_BloomLayerMask & (1 << i.layer))
                {
                    // 应用Bloom效果
                    col.rgb *= _BloomIntensity;
                }

                return col;
            }
            ENDCG
        }
    }
}