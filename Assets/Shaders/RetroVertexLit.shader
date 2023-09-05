Shader "Custom/RetroVertexLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelationAmount ("Pixelation Amount", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelationAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Pixelate the texture
                float2 pixelatedUV = floor(i.uv / _PixelationAmount) * _PixelationAmount;
                half4 texColor = tex2D(_MainTex, pixelatedUV);

                // Simple vertex lighting
                float3 lightDir = normalize(float3(0, 1, 0)); // Light direction (from above)
                float NdotL = max(0, dot(i.normal, lightDir));
                half4 litColor = texColor * NdotL;

                return litColor;
            }
            ENDCG
        }
    }
}