Shader "Custom/SimpleTextureShaderUnlitNoFogWithEmission"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 1)) = 1
        _Value ("Value", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 emission : COLOR;
            };

            sampler2D _MainTex;
            float _Value;
            fixed4 _EmissionColor;
            float _EmissionStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                // Check the value and adjust the color accordingly
                color.rgb = lerp(color.rgb, fixed3(1,1,1), _Value);
                color.a = tex2D(_MainTex, i.uv).a; // Use alpha from input texture

                // Calculate emission
                fixed3 hdrEmission = _EmissionColor.rgb * _EmissionStrength * 5;

                // Apply tone mapping for HDR colors
                fixed3 gammaEmission = LinearToGammaSpace(hdrEmission);

                // Final emission color
                fixed4 emission = fixed4(gammaEmission, 1.0);

                // Add emission to color
                color.rgb += emission.rgb;

                return color;
            }
            ENDCG
        }
    }
}