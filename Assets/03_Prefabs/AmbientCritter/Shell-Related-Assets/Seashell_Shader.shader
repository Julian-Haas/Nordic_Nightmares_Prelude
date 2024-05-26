Shader "Custom/Seashell_Shader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert
        
        struct Input
        {
            float3 worldPos;
        };

        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _SeashellBuffer; // Add a sampler for the compute buffer

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Access seashell data from the compute buffer
            float4 seashellData = tex2Dlod(_SeashellBuffer, float4(IN.worldPos.xy, 0.0, 0.0));

            // Extract position, scale, and rotation from the seashell data
            float3 position = seashellData.xyz;
            float3 scale = seashellData.xyz;
            float4 rotation = seashellData.w;

            // Transform the surface position using the seashell data
            float3 worldPos = mul(rotation, IN.worldPos) * scale + position;

            // Sample the main texture
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

            // Assign the color and texture to the surface output
            o.Albedo = mainTex.rgb * _Color.rgb;

            // Set the world position for proper lighting
            o.Normal = normalize(IN.worldPos - position);
            o.Alpha = mainTex.a;

        }
        ENDCG
    }
        FallBack "Diffuse"
}