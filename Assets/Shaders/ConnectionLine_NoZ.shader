Shader "Custom/ConnectionLine_NoZ"
{
	Properties
	{
		Intensity("Intensity", Float) = 1
		Color_2A2C9E43("FlowColor", Color) = (1, 0, 0, 1)
		Vector1_F24E8D48("FlowIntensity", Float) = 8
	}
		SubShader
	{
		Tags
	{
		"RenderPipeline" = "UniversalPipeline"
		"RenderType" = "Opaque"
		"Queue" = "Geometry+5"
	}

		Pass
	{
		Name "Pass"
		Tags
	{
		// LightMode: <None>
	}

	// Render State
		Blend One Zero, One Zero
		Cull Back
		ZTest Always
		ZWrite Off
		// ColorMask: <None>


		HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		// Pragmas
#pragma prefer_hlslcc gles
#pragma exclude_renderers d3d11_9x
#pragma target 2.0
#pragma multi_compile_fog
#pragma multi_compile_instancing

		// Keywords
#pragma multi_compile _ LIGHTMAP_ON
#pragma multi_compile _ DIRLIGHTMAP_COMBINED
#pragma shader_feature _ _SAMPLE_GI
		// GraphKeywords: <None>

		// Defines
#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_TEXCOORD0
#define ATTRIBUTES_NEED_COLOR
#define VARYINGS_NEED_TEXCOORD0
#define VARYINGS_NEED_COLOR
#define SHADERPASS_UNLIT

		// Includes
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		float Intensity;
	float4 Color_2A2C9E43;
	float Vector1_F24E8D48;
	CBUFFER_END

		// Graph Functions

		void Unity_Multiply_float(float A, float B, out float Out)
	{
		Out = A * B;
	}

	void Unity_Add_float(float A, float B, out float Out)
	{
		Out = A + B;
	}

	void Unity_Fraction_float(float In, out float Out)
	{
		Out = frac(In);
	}

	void Unity_Subtract_float(float A, float B, out float Out)
	{
		Out = A - B;
	}

	void Unity_Power_float(float A, float B, out float Out)
	{
		Out = pow(A, B);
	}

	void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
	{
		Out = A * B;
	}

	void Unity_Add_float4(float4 A, float4 B, out float4 Out)
	{
		Out = A + B;
	}

	// Graph Vertex
	// GraphVertex: <None>

	// Graph Pixel
	struct SurfaceDescriptionInputs
	{
		float4 uv0;
		float4 VertexColor;
		float3 TimeParameters;
	};

	struct SurfaceDescription
	{
		float3 Color;
		float Alpha;
		float AlphaClipThreshold;
	};

	SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
	{
		SurfaceDescription surface = (SurfaceDescription)0;
		float4 _UV_86A5C27E_Out_0 = IN.uv0;
		float _Split_D056E9C9_R_1 = _UV_86A5C27E_Out_0[0];
		float _Split_D056E9C9_G_2 = _UV_86A5C27E_Out_0[1];
		float _Split_D056E9C9_B_3 = _UV_86A5C27E_Out_0[2];
		float _Split_D056E9C9_A_4 = _UV_86A5C27E_Out_0[3];
		float _Multiply_BF481C49_Out_2;
		Unity_Multiply_float(_Split_D056E9C9_R_1, 0.1, _Multiply_BF481C49_Out_2);
		float _Add_87D285FE_Out_2;
		Unity_Add_float(_Multiply_BF481C49_Out_2, IN.TimeParameters.x * 4, _Add_87D285FE_Out_2);
		float _Fraction_490F079B_Out_1;
		Unity_Fraction_float(_Add_87D285FE_Out_2, _Fraction_490F079B_Out_1);
		float _Subtract_D425F718_Out_2;
		Unity_Subtract_float(_Fraction_490F079B_Out_1, 0.5, _Subtract_D425F718_Out_2);
		float _Multiply_8F86D6CD_Out_2;
		Unity_Multiply_float(2, _Subtract_D425F718_Out_2, _Multiply_8F86D6CD_Out_2);
		float _Power_18456B19_Out_2;
		Unity_Power_float(_Multiply_8F86D6CD_Out_2, 2, _Power_18456B19_Out_2);
		float _Subtract_11B73091_Out_2;
		Unity_Subtract_float(1, _Power_18456B19_Out_2, _Subtract_11B73091_Out_2);
		float _Power_D744DFF2_Out_2;
		Unity_Power_float(_Subtract_11B73091_Out_2, 40, _Power_D744DFF2_Out_2);
		float4 _Property_A87F3A3E_Out_0 = Color_2A2C9E43;
		float4 _Multiply_C521FECE_Out_2;
		Unity_Multiply_float((_Power_D744DFF2_Out_2.xxxx), _Property_A87F3A3E_Out_0, _Multiply_C521FECE_Out_2);
		float _Property_A695D8A6_Out_0 = Vector1_F24E8D48;
		float4 _Multiply_9757C3F4_Out_2;
		Unity_Multiply_float(_Multiply_C521FECE_Out_2, (_Property_A695D8A6_Out_0.xxxx), _Multiply_9757C3F4_Out_2);
		float _Property_57CCA90C_Out_0 = Intensity;
		float4 _Multiply_F647EB22_Out_2;
		Unity_Multiply_float(IN.VertexColor, (_Property_57CCA90C_Out_0.xxxx), _Multiply_F647EB22_Out_2);
		float4 _Add_D7C42DEF_Out_2;
		Unity_Add_float4(_Multiply_9757C3F4_Out_2, _Multiply_F647EB22_Out_2, _Add_D7C42DEF_Out_2);
		float _Split_6830E5A7_R_1 = IN.VertexColor[0];
		float _Split_6830E5A7_G_2 = IN.VertexColor[1];
		float _Split_6830E5A7_B_3 = IN.VertexColor[2];
		float _Split_6830E5A7_A_4 = IN.VertexColor[3];
		surface.Color = (_Add_D7C42DEF_Out_2.xyz);
		surface.Alpha = _Split_6830E5A7_A_4;
		surface.AlphaClipThreshold = 0;
		return surface;
	}

	// --------------------------------------------------
	// Structs and Packing

	// Generated Type: Attributes
	struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 color : COLOR;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
#endif
	};

	// Generated Type: Varyings
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float4 texCoord0;
		float4 color;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
	};

	// Generated Type: PackedVaryings
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
#endif
		float4 interp00 : TEXCOORD0;
		float4 interp01 : TEXCOORD1;
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
	};

	// Packed Type: Varyings
	PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output = (PackedVaryings)0;
		output.positionCS = input.positionCS;
		output.interp00.xyzw = input.texCoord0;
		output.interp01.xyzw = input.color;
#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
#endif
		return output;
	}

	// Unpacked Type: Varyings
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output = (Varyings)0;
		output.positionCS = input.positionCS;
		output.texCoord0 = input.interp00.xyzw;
		output.color = input.interp01.xyzw;
#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
#endif
		return output;
	}

	// --------------------------------------------------
	// Build Graph Inputs

	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
	{
		SurfaceDescriptionInputs output;
		ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





		output.uv0 = input.texCoord0;
		output.VertexColor = input.color;
		output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

		return output;
	}


	// --------------------------------------------------
	// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

	ENDHLSL
	}

		Pass
	{
		Name "ShadowCaster"
		Tags
	{
		"LightMode" = "ShadowCaster"
	}

		// Render State
		Blend One Zero, One Zero
		Cull Back
		ZTest LEqual
		ZWrite On
		// ColorMask: <None>


		HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		// Pragmas
#pragma prefer_hlslcc gles
#pragma exclude_renderers d3d11_9x
#pragma target 2.0
#pragma multi_compile_instancing

		// Keywords
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		// GraphKeywords: <None>

		// Defines
#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_COLOR
#define VARYINGS_NEED_COLOR
#define SHADERPASS_SHADOWCASTER

		// Includes
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		float Intensity;
	float4 Color_2A2C9E43;
	float Vector1_F24E8D48;
	CBUFFER_END

		// Graph Functions
		// GraphFunctions: <None>

		// Graph Vertex
		// GraphVertex: <None>

		// Graph Pixel
		struct SurfaceDescriptionInputs
	{
		float4 VertexColor;
	};

	struct SurfaceDescription
	{
		float Alpha;
		float AlphaClipThreshold;
	};

	SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
	{
		SurfaceDescription surface = (SurfaceDescription)0;
		float _Split_6830E5A7_R_1 = IN.VertexColor[0];
		float _Split_6830E5A7_G_2 = IN.VertexColor[1];
		float _Split_6830E5A7_B_3 = IN.VertexColor[2];
		float _Split_6830E5A7_A_4 = IN.VertexColor[3];
		surface.Alpha = _Split_6830E5A7_A_4;
		surface.AlphaClipThreshold = 0;
		return surface;
	}

	// --------------------------------------------------
	// Structs and Packing

	// Generated Type: Attributes
	struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 color : COLOR;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
#endif
	};

	// Generated Type: Varyings
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float4 color;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
	};

	// Generated Type: PackedVaryings
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
#endif
		float4 interp00 : TEXCOORD0;
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
	};

	// Packed Type: Varyings
	PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output = (PackedVaryings)0;
		output.positionCS = input.positionCS;
		output.interp00.xyzw = input.color;
#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
#endif
		return output;
	}

	// Unpacked Type: Varyings
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output = (Varyings)0;
		output.positionCS = input.positionCS;
		output.color = input.interp00.xyzw;
#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
#endif
		return output;
	}

	// --------------------------------------------------
	// Build Graph Inputs

	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
	{
		SurfaceDescriptionInputs output;
		ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





		output.VertexColor = input.color;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

		return output;
	}


	// --------------------------------------------------
	// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

	ENDHLSL
	}

		Pass
	{
		Name "DepthOnly"
		Tags
	{
		"LightMode" = "DepthOnly"
	}

		// Render State
		Blend One Zero, One Zero
		Cull Back
		ZTest LEqual
		ZWrite On
		ColorMask 0


		HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		// Pragmas
#pragma prefer_hlslcc gles
#pragma exclude_renderers d3d11_9x
#pragma target 2.0
#pragma multi_compile_instancing

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
#define ATTRIBUTES_NEED_NORMAL
#define ATTRIBUTES_NEED_TANGENT
#define ATTRIBUTES_NEED_COLOR
#define VARYINGS_NEED_COLOR
#define SHADERPASS_DEPTHONLY

		// Includes
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		float Intensity;
	float4 Color_2A2C9E43;
	float Vector1_F24E8D48;
	CBUFFER_END

		// Graph Functions
		// GraphFunctions: <None>

		// Graph Vertex
		// GraphVertex: <None>

		// Graph Pixel
		struct SurfaceDescriptionInputs
	{
		float4 VertexColor;
	};

	struct SurfaceDescription
	{
		float Alpha;
		float AlphaClipThreshold;
	};

	SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
	{
		SurfaceDescription surface = (SurfaceDescription)0;
		float _Split_6830E5A7_R_1 = IN.VertexColor[0];
		float _Split_6830E5A7_G_2 = IN.VertexColor[1];
		float _Split_6830E5A7_B_3 = IN.VertexColor[2];
		float _Split_6830E5A7_A_4 = IN.VertexColor[3];
		surface.Alpha = _Split_6830E5A7_A_4;
		surface.AlphaClipThreshold = 0;
		return surface;
	}

	// --------------------------------------------------
	// Structs and Packing

	// Generated Type: Attributes
	struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 color : COLOR;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
#endif
	};

	// Generated Type: Varyings
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float4 color;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
	};

	// Generated Type: PackedVaryings
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
#endif
		float4 interp00 : TEXCOORD0;
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
#endif
	};

	// Packed Type: Varyings
	PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output = (PackedVaryings)0;
		output.positionCS = input.positionCS;
		output.interp00.xyzw = input.color;
#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
#endif
		return output;
	}

	// Unpacked Type: Varyings
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output = (Varyings)0;
		output.positionCS = input.positionCS;
		output.color = input.interp00.xyzw;
#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
#endif
#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
#endif
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
#endif
		return output;
	}

	// --------------------------------------------------
	// Build Graph Inputs

	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
	{
		SurfaceDescriptionInputs output;
		ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





		output.VertexColor = input.color;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

		return output;
	}


	// --------------------------------------------------
	// Main

#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

	ENDHLSL
	}

	}

	FallBack "Hidden/Shader Graph/FallbackError"
}
