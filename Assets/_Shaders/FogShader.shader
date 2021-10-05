Shader "Hidden/FogShader"
{
    Properties
    {
        _MainTex ("MTexture", 2D) = "white" {}
        _SecondTex("MTexture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
        }

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _SecondTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) + tex2D(_SecondTex, i.uv);
                // red = 1 blue = 1 -> alpha = 0
                // red = 1 blue = 0 -> alpha = 0.5
                // red = 0 blue = 0 -> alpha = 1
                
                col.a = (-col.r * 1.25f - col.b * 0.75f + 2.0f);
                return fixed4(0,0,0,col.a);
            }
            ENDCG
        }
    }
}
