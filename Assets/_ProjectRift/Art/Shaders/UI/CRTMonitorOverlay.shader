Shader "ProjectRift/UI/CRT Monitor Overlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (0.55, 1, 0.42, 0.18)
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.35
        _ScanlineCount ("Scanline Count", Range(40, 900)) = 140
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.08
        _FlickerIntensity ("Flicker Intensity", Range(0, 1)) = 0.03
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.2
        _RollSpeed ("Roll Speed", Range(-4, 4)) = -0.3
        _RollSize ("Roll Size", Range(0.01, 0.35)) = 0.08
        _RollIntensity ("Roll Intensity", Range(0, 1)) = 0.12
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest LEqual
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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Tint;
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _NoiseIntensity;
            float _FlickerIntensity;
            float _VignetteIntensity;
            float _RollSpeed;
            float _RollSize;
            float _RollIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float Random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float time = _Time.y;

                float scanlinePattern = step(0.5, frac(i.uv.y * _ScanlineCount));
                float scanlineAlpha = scanlinePattern * _ScanlineIntensity;

                float noise = Random(float2(i.uv.x * 220.0, i.uv.y * 180.0 + floor(time * 24.0)));
                float noiseAlpha = noise * _NoiseIntensity;

                float flickerAlpha = abs(sin(time * 55.0)) * _FlickerIntensity;

                float2 centered = i.uv * 2.0 - 1.0;
                float edge = saturate(dot(centered, centered));
                float vignetteAlpha = smoothstep(0.35, 1.25, edge) * _VignetteIntensity;

                float rollPosition = frac(time * _RollSpeed);
                float rollDistance = abs(i.uv.y - rollPosition);
                rollDistance = min(rollDistance, 1.0 - rollDistance);
                float rollAlpha = smoothstep(_RollSize, 0.0, rollDistance) * _RollIntensity;

                float alpha = saturate(scanlineAlpha + noiseAlpha + flickerAlpha + vignetteAlpha + rollAlpha);
                fixed3 color = _Tint.rgb;

                return fixed4(color, alpha * _Tint.a * i.color.a);
            }
            ENDCG
        }
    }
}
