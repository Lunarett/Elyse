Shader "Pulsar/TerrainSteepnessShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SteepTex ("Steep (RGB)", 2D) = "white" {}
        _Steepness ("Steepness Threshold", Range(0, 1)) = 0.5
        _SteepTexScale ("Steep Texture Scale", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "TerrainCompatible"="True" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SteepTex;
            float3 worldNormal;
        };

        sampler2D _MainTex;
        sampler2D _SteepTex;
        float _Steepness;
        float2 _SteepTexScale;

        void vert(inout appdata_full v, out Input o)
        {
            o.uv_MainTex = v.texcoord.xy;
            o.uv_SteepTex = v.texcoord.xy * _SteepTexScale.xy;
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            float steepness = 1.0 - abs(dot(normalize(IN.worldNormal), float3(0,1,0)));
            steepness = smoothstep(_Steepness - 0.1, _Steepness + 0.1, steepness);
            
            fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 steepCol = tex2D(_SteepTex, IN.uv_SteepTex);
            o.Albedo = lerp(mainCol, steepCol, steepness).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
