Shader "Custom/RetroVertexLitSmooth"
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
                // Calculate the pixel size
                float2 pixelSize = _PixelationAmount;

                // Calculate the position of the bottom-left corner of the nearest "big pixel"
                float2 bottomLeft = floor(i.uv / pixelSize) * pixelSize;

                // Calculate the positions of the four corners of the "big pixel"
                float2 topLeft = bottomLeft + float2(0, pixelSize.y);
                float2 topRight = bottomLeft + float2(pixelSize.x, pixelSize.y);
                float2 bottomRight = bottomLeft + float2(pixelSize.x, 0);

                // Sample the texture at the four corners
                half4 bl = tex2D(_MainTex, bottomLeft);
                half4 tl = tex2D(_MainTex, topLeft);
                half4 tr = tex2D(_MainTex, topRight);
                half4 br = tex2D(_MainTex, bottomRight);

                // Calculate the interpolation weights
                float2 f = frac(i.uv / pixelSize);

                // Perform bilinear interpolation
                half4 tA = lerp(bl, br, f.x);
                half4 tB = lerp(tl, tr, f.x);
                half4 color = lerp(tA, tB, f.y);

                // Simple vertex lighting
                float3 lightDir = normalize(float3(0, 1, 0)); // Light direction (from above)
                float NdotL = max(0, dot(i.normal, lightDir));
                half4 litColor = color * NdotL;

                return litColor;
            }
            ENDCG
        }
    }
}