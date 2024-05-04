Shader "Custom/ScrollingUVUnlitWithEmissionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 0.1
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _EmissionStrength ("Emission Strength", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _ScrollSpeed;
            fixed4 _TintColor;
            fixed4 _EmissionColor;
            float _EmissionStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offset = float2(_Time.y * _ScrollSpeed, 0); // Change _Time.y to _Time.x for horizontal scrolling
                float2 uv = i.uv + offset;
                fixed4 texColor = tex2D(_MainTex, uv) * _TintColor;

                // Add Emission
                fixed4 emissionColor = _EmissionColor * _EmissionStrength;
                texColor.rgb += emissionColor.rgb;

                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}