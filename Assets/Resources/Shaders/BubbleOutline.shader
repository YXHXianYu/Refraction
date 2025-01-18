Shader "Sprites/BubbleOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _OutlineColorX("OutlineColorX", Color) = (1,1,1,1)
        _OutlineColorY("OutlineColorY", Color) = (1,1,1,1)
        _CenterPosX("CenterPosX", Float) = 0.5
        _CenterPosY("CenterPosY", Float) = 0.5
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
            float _CenterPosX;
            float _CenterPosY;

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
                // 该像素的世界坐标
                float2 worldPos = IN.texcoord.xy;
                float deltaX = abs(worldPos.x - _CenterPosX);
                float deltaY = abs(worldPos.y - _CenterPosY);
                // 计算X和Y方向的权重
                float weightX = smoothstep(-0.5, 0.5, deltaX);
                float weightY = smoothstep(-0.5, 0.5, deltaY);
                // 计算混合因子
                float mixFactorX = clamp(weightX, 0.0, 1.0);
                float mixFactorY = clamp(weightY, 0.0, 1.0);
                fixed4 mixedColorX = lerp(_OutlineColorX, c, mixFactorX);
                fixed4 mixedColorY = lerp(_OutlineColorY, c, mixFactorY);
                // 最终颜色插值，考虑四个角的影响
                fixed4 finalColor = lerp(mixedColorX, mixedColorY, 0.5);
                finalColor.rgb = pow(finalColor.rgb, 1.2); // 对颜色进行伽马校正
                return finalColor;
            }
            ENDCG
        }
    }
}