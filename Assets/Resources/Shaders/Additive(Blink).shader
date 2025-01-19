Shader "Unlit/Additive(Blink)"
{
   Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FlickerStrength ("Flicker Strength", Range(0, 1)) = 0.5
        _FlickerSpeed ("Flicker Speed", Range(0, 10)) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _FlickerStrength;
            float _FlickerSpeed;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float random (float x)
            {
                return frac(sin(x)*43758.5453123);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Generate a random flicker effect
                float flicker = random(_Time.x);
                flicker = lerp(1.0, flicker, _FlickerStrength);
                flicker *= abs(sin(_Time.y * _FlickerSpeed));

                // Apply flicker to the color
                col.rgb *= flicker;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
