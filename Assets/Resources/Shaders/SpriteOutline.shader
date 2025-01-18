Shader "Sprites/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _OutlineColorX("OutlineColorX",Color) = (1,1,1,1)
        _OutlineColorY("OutlineColorY",Color) = (1,1,1,1)
        _CheckRange("CheckRange",Range(0,1)) = 0
        _LineWidth("LineWidth",Float) = 0.39
        _CheckAccuracy("CheckAccuracy",Range(0.1,0.99)) = 0.9
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
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _AlphaTex;
            fixed4 _Color;
            fixed4 _OutlineColorX;
            fixed4 _OutlineColorY;
            float _CheckRange;
            float _LineWidth;
            float _CheckAccuracy;

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            }; //


            v2f vert(appdata_t IN) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                return OUT;
            }


            fixed4 SampleSpriteTexture(float2 uv) {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA//
                // get the color from an external texture (usecase: Alpha support for ETC1 on android)
                color.a = tex2D (_AlphaTex, uv).r;
                #endif //ETC1_EXTERNAL_ALPHA

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;
                // 获取物体的世界坐标
                float2 worldPos = IN.texcoord.xy; // 假设只使用X和Y坐标
                // 计算插值因子，基于世界坐标的X和Y分量
                float mixFactor = (worldPos.x + worldPos.y) * 0.5; // 归一化到[0, 1]的范围
                // 确保mixFactor在[0, 1]之间
                mixFactor = clamp(mixFactor, 0.0, 1.0);
                // 根据mixFactor插值两种颜色
                fixed4 mixedColor = lerp(_OutlineColorX, _OutlineColorY, mixFactor);
                // 应用混合颜色
                c = lerp(mixedColor, c, 0.8);
                return c;
            }
            ENDCG
        }
    }
}