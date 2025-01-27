Shader "Hidden/Bloom Replace"
{
    //替换标签是Bloom的shader
    SubShader
    {
        Tags
        {
            "RenderType" = "Bloom"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;

            half4 frag(v2f_img i) : COLOR {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }

    //替换标签是BloomTransparent的shader
    SubShader
    {
        Tags
        {
            "RenderType" = "BloomTransparent"
        }
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off Fog
        {
            Mode Off
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;

            half4 frag(v2f_img i) : COLOR {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }

    //替换标签是Opaque的shader，这里直接渲染为黑色
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            half4 frag(v2f_img i) : COLOR {
                return half4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    Fallback Off
}