Shader "Custom/Lit"
{
	
    Properties
    {
        _BaseColor("Color", Color) = (1.0,1.0,1.0,1.0)
        _BaseMap("MainTex", 2D) = "white"{}
    	_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [Toggle(_CLIPPING)] _Clipping ("Alpha Clip", Float) = 0
    	[Toggle(_RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1
        [Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
    	[KeywordEnum(On, Clip, Dither, Off)] _Shadows ("Shadows", Float) = 0
    }

    SubShader
    {
        Pass
        {
            Tags {"LightMode" = "CustomLit"}
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            HLSLPROGRAM
            #pragma target 3.5
            #pragma shader_feature _ _SHADOWS_CLIP _SHADOWS_DITHER
			#pragma shader_feature _PREMULTIPLY_ALPHA
            #pragma shader_feature _RECEIVE_SHADOWS
            #pragma multi_compile _ _DIRECTIONAL_PCF3 _DIRECTIONAL_PCF5 _DIRECTIONAL_PCF7
            #pragma multi_compile _ _CASCADE_BLEND_SOFT _CASCADE_BLEND_DITHER
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"

            #define MAX_DIRECTIONAL_LIGHT_COUNT 4
            
            CBUFFER_START(_CustomLight)
	            int _DirectionalLightCount;
	            float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	            float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
				float4 _DirectionalLightShadowData[MAX_DIRECTIONAL_LIGHT_COUNT];
            CBUFFER_END
            
            #include "Surface.hlsl"
            #include "Shadows.hlsl"
            #include "Light.hlsl"
            #include "BRDF.hlsl"
            #include "GI.hlsl"
            #include "Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
	            UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
	            UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
				UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
	            UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
	            UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
            
            struct Attributes
            {
                float3 positionOS   : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            	GI_ATTRIBUTE_DATA
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                half4 positionHCS  : SV_POSITION;
                half3 normalWS : NORMAL;
                half3 positionWS : VAR_POSITION;
            	float2 uv : VAR_BASE_UV;
            	GI_VARYINGS_DATA
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
            	TRANSFER_GI_DATA(IN, OUT);

                OUT.positionWS = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
				OUT.uv = IN.uv * baseST.xy + baseST.zw;
                
                return OUT;
            }

            half4 frag (Varyings IN) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);

            	half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 base = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
            	base.rgb *= baseMap.rgb;
#if defined(_SHADOWS_CLIP)
				clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
#elif defined(_SHADOWS_DITHER)
				float dither = InterleavedGradientNoise(IN.positionHCS.xy, 0);
				clip(base.a - dither);
#endif
                Surface surface;
            	surface.position = IN.positionWS;
	            surface.normal = normalize(IN.normalWS);
                surface.viewDirection = normalize(_WorldSpaceCameraPos - IN.positionWS);
            	surface.depth = -TransformWorldToView(IN.positionWS).z;
	            surface.color = base.rgb;
	            surface.alpha = base.a;
                surface.metallic = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Metallic);
	            surface.smoothness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Smoothness);
            	surface.dither = InterleavedGradientNoise(IN.positionHCS.xy, 0);

#if defined(_PREMULTIPLY_ALPHA)
				BRDF brdf = GetBRDF(surface, true);
#else
				BRDF brdf = GetBRDF(surface);
#endif
            	GI gi = GetGI(GI_FRAGMENT_DATA(IN), surface);
                half3 color = GetLighting(surface, brdf, gi);
                
                return float4(color, surface.alpha);
            }



            ENDHLSL
        }
    	
    	Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			ColorMask 0

			HLSLPROGRAM
			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma vertex ShadowCasterPassVertex
			#pragma fragment ShadowCasterPassFragment
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float3 positionOS : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			Varyings ShadowCasterPassVertex (Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				float3 positionWS = TransformObjectToWorld(input.positionOS);
				output.positionCS = TransformWorldToHClip(positionWS);

				//to make sure out of view vertices won't get clamped due to shadow pancaking
				#if UNITY_REVERSED_Z
					output.positionCS.z =
						min(output.positionCS.z, output.positionCS.w * UNITY_NEAR_CLIP_VALUE);
				#else
					output.positionCS.z =
						max(output.positionCS.z, output.positionCS.w * UNITY_NEAR_CLIP_VALUE);
				#endif

				return output;
			}

			void ShadowCasterPassFragment (Varyings input) {
				UNITY_SETUP_INSTANCE_ID(input);
			}
			
			ENDHLSL
		}
    }
	
	CustomEditor "CustomShaderGUI"
}
