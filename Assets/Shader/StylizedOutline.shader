// Equivalente en Unity (URP) del Geometry Nodes "StylizedOutline" de Blender.
// Blender:  Time -> /8 -> Snap(1) -> +Position -> Noise(fBM) -> MapRange -> *Normal -> SetPosition -> FlipFaces
// Unity:    todo se hace en el vertex shader + Cull Front (= Flip Faces). Muy barato para movil.
Shader "Custom/StylizedOutline"
{
    Properties
    {
        _OutlineColor ("Color", Color) = (0,0,0,1)
        _Thickness    ("Grosor (multiplicador)", Float) = 1

        [Header(Noise  Blender Noise Texture)]
        _NoiseScale   ("Scale", Float) = 1.3
        _Octaves      ("Detail (octavas, 1-4)", Range(1,4)) = 3   // Detail 2.0 ~ 3 octavas
        _Roughness    ("Roughness", Range(0,1)) = 0.5
        _Lacunarity   ("Lacunarity", Float) = 2.0

        [Header(Map Range)]
        _FromMin      ("From Min", Float) = 0.24
        _FromMax      ("From Max", Float) = 4.0
        _ToMin        ("To Min", Float) = 0
        _ToMax        ("To Max", Float) = 1

        [Header(Tiempo escalonado)]
        _FPS          ("Frames por segundo de la escena", Float) = 24
        _FrameStep    ("Divide (frame / N)", Float) = 8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry+1" }

        Pass
        {
            Name "Outline"
            Cull Front      // Flip Faces
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _Thickness, _NoiseScale, _Octaves, _Roughness, _Lacunarity;
                float _FromMin, _FromMax, _ToMin, _ToMax, _FPS, _FrameStep;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            // --- value noise 3D barato (sin texturas) ---
            float hash31(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.zyx + 31.32);
                return frac((p.x + p.y) * p.z);
            }

            float vnoise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float n000 = hash31(i);
                float n100 = hash31(i + float3(1,0,0));
                float n010 = hash31(i + float3(0,1,0));
                float n110 = hash31(i + float3(1,1,0));
                float n001 = hash31(i + float3(0,0,1));
                float n101 = hash31(i + float3(1,0,1));
                float n011 = hash31(i + float3(0,1,1));
                float n111 = hash31(i + float3(1,1,1));
                return lerp(lerp(lerp(n000,n100,f.x), lerp(n010,n110,f.x), f.y),
                            lerp(lerp(n001,n101,f.x), lerp(n011,n111,f.x), f.y), f.z);
            }

            // fBM con Normalize (como Blender)
            float fbm(float3 p)
            {
                float sum = 0, amp = 1, norm = 0;
                [unroll] for (int k = 0; k < 4; k++)
                {
                    float w = saturate(_Octaves - k);      // permite Detail fraccionario
                    sum  += vnoise(p) * amp * w;
                    norm += amp * w;
                    p   *= _Lacunarity;
                    amp *= _Roughness;
                }
                return sum / max(norm, 1e-5);
            }

            Varyings vert(Attributes v)
            {
                // Scene Time -> Divide(8) -> Snap(1)  : el ruido solo cambia cada N frames
                float frame = _Time.y * _FPS;
                float t = floor(frame / _FrameStep);

                // Position + t  (en Blender se suma el escalar a los 3 ejes)
                float3 p = (v.positionOS.xyz + t) * _NoiseScale;

                float n = fbm(p);

                // Map Range (clamp)
                float r = saturate((n - _FromMin) / (_FromMax - _FromMin));
                r = lerp(_ToMin, _ToMax, r);

                // Set Position: Offset = Normal * r
                float3 posOS = v.positionOS.xyz + normalize(v.normalOS) * r * _Thickness;

                Varyings o;
                o.positionCS = TransformObjectToHClip(posOS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target { return _OutlineColor; }
            ENDHLSL
        }
    }
}
