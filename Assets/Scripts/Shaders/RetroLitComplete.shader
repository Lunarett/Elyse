Shader "Custom/RetroVertexLitComplete"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelationAmount ("Pixelation Amount", float) = 0.1
        _FacetAmount ("Facet Amount", float) = 1.0
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
                float3 barycentric : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelationAmount;
            float _FacetAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal * _FacetAmount;  // Multiply by facet amount
                o.barycentric = float3(1 - v.uv.x - v.uv.y, v.uv.x, v.uv.y); // Compute barycentric coordinates
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Pixelation
                float2 pixelSize = _PixelationAmount;
                float2 bottomLeft = floor(i.uv / pixelSize) * pixelSize;
                half4 color = tex2D(_MainTex, bottomLeft);
                
                // Vertex Lighting
                float3 lightDir = normalize(float3(0, 1, 0));
                float NdotL = max(0, dot(i.normal, lightDir));
                half4 litColor = color * NdotL;

                // Simulated Lower Polycount
                float3 bary = i.barycentric;
                bary = step(0.5, bary) / step(0.5, bary);
                float3 newColor = bary.x * i.normal.x + bary.y * i.normal.y + bary.z * i.normal.z;
                
                // Combine all effects
                half4 finalColor = half4(litColor.rgb * newColor, 1);
                
                return finalColor;
            }
            ENDCG
        }
    }
}