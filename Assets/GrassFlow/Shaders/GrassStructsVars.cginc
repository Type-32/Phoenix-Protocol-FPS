

//Comment/Uncomment this depending on your indirect instancing setting
//#define INDIRECT_INSTANCING

#define MIN_BLADE_SEGMENTS 2
#define MAX_BLADE_SEGMENTS 5
#define BLADE_SEG_DIFF (MAX_BLADE_SEGMENTS - MIN_BLADE_SEGMENTS)

//SRP STUFF
#if !defined(SRP)
	#ifndef SHADOW_CASTER
		//#define GRASS_SHADOW_COORDS(num) float4 _ShadowCoord : TEXCOORD5;
		#define GRASS_SHADOW_COORDS(num) UNITY_SHADOW_COORDS(num)
	#else
		#define GRASS_SHADOW_COORDS(num)
	#endif
#else

	#define GRASS_SHADOW_COORDS(num) float3 _ShadowCoord : TEXCOORD5;

	float3 _WorldSpaceLightPos0;
#endif


#if defined(SRP) && defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
	//idunno, i feel like its just not really feasible to support lightmap stuff rn bc too much work
	//theres no easy way to get it integrated into the bake system etc so getting it to receive lightmaps is tricky
	//#define REQUIRES_LIGHTMAP_UV
#endif




//requirement definitions
#if (defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHT_SHADOWS)) && !defined(_ADDITIONAL_LIGHTS_VERTEX) && defined(GF_PPLIGHTS)
#define SRP_FRAGMENT_ADDITIONAL_LIGHTS
#endif

//#if defined(FORWARD_ADD) || defined(SRP_FRAGMENT_ADDITIONAL_LIGHTS)
#define FRAGMENT_REQUIRES_WORLDPOS
//#endif


#if !defined(GRASS_DEPTH)

	#if defined(GF_SPECULAR)
		#if !defined(DEFERRED)
		#define USE_SPECULAR
		#endif
		
		#if defined(USE_SPECULAR)
		#define FRAGMENT_REQUIRES_SPECULAR
		#endif
	#endif

	#if defined(DEFERRED) || defined(GF_PPLIGHTS)\
		 || defined(USE_SPECULAR) || defined(GF_NORMAL_MAP) || defined(FORWARD_ADD)
	#define FRAGMENT_REQUIRES_NORMAL
	#endif
#endif





#if !defined(DEFERRED)
uniform half4 _LightColor0;
#endif

uniform float numTexturesPctUV;

float4x4 objToWorldMatrix;
float4x4 worldToObjMatrix;

//---------------------------------------------------------------------------------
//----------------------------PROPS--------------------------------------
//---------------------------------------------------------------------------------

#if !defined(SURFACE_TESSELATION)
#if defined(SRP) && !defined(SHADER_API_METAL)
CBUFFER_START(UnityPerMaterial)
#endif
#endif

//these arent used in tessellation so dont define them inside of the cbuffer
//otherwise breaks the URP CBUFFER since they dont show up in the shader properties anymore
uniform half bladeWidth;
uniform half bladeSharp;
uniform float widthLODscale;

#if defined(SURFACE_TESSELATION)
#if defined(SRP) && !defined(SHADER_API_METAL)
CBUFFER_START(UnityPerMaterial)
#endif
#endif



//an important note about the cbuffer props:
//it only matters for values represented in the shader properties, not values that are only assigned by script

uniform float4 _noiseScale;
uniform float4 _noiseSpeed;
uniform float3 windDir;
uniform float4 _noiseScale2;
uniform float4 _noiseSpeed2;
uniform float3 windDir2;
uniform float4 windTint;
//uniform float windFlutter;

uniform sampler2D _MainTex;
uniform half4 _MainTex_ST;


uniform float numTextures;
uniform int textureAtlasScalingCutoff;

uniform sampler2D dhfParamMap;
uniform sampler2D    colorMap;
uniform sampler2D     typeMap;
uniform half4 dhfParamMap_ST;
uniform half4    colorMap_ST;
uniform half4     typeMap_ST;

uniform half4 _Color;
uniform bool alphaLock;
uniform half alphaClip;
uniform half _AO;
uniform half bladeHeight;
uniform half ambientCO;
#define diffuseCO (1.0 - ambientCO)
uniform half4 variance;
uniform half3 _LOD;
uniform half grassFade;
uniform half grassFadeSharpness;
uniform half seekSun;
uniform half topViewPush;
uniform half flatnessMult;
uniform half4 flatTint;

uniform sampler3D _NoiseTex;

#if defined(MULTI_SEGMENT)
uniform float bladeLateralCurve;
uniform float bladeVerticalCurve;
uniform float bladeStiffness;
#endif

#if !defined(DEFERRED)

uniform half ambientCOShadow;
uniform half edgeLight;
uniform half edgeLightSharp;

#else
uniform float _Metallic;
uniform float _Gloss;

uniform sampler2D _SpecMap;

uniform sampler2D _OccMap;
uniform float occMult;
#endif

#if defined(GF_SPECULAR)
uniform float specularMult;
uniform float specSmooth;
uniform float specHeight;
uniform float4 specTint;
#endif

#if defined(GF_USE_DITHER) || defined(SHADOW_CASTER) || defined(SEMI_TRANSPARENT)
	uniform sampler3D _DitherMaskLOD;
	#define DITHERMASK_REQUIRED
#endif


#if defined(SURFACE_TESSELATION)
	uniform float tessClose;
	uniform float tessAmnt;
	uniform float tessDispScale;
	uniform float tessTexScale;
	uniform float tessAmntShadow;
	uniform float tessRange;
	uniform float tessPow;
	uniform float tessMin;
	
	
	uniform sampler2D grassDisplacer;
	uniform sampler2D randoMap;
#endif

#if defined(GF_NORMAL_MAP)
	uniform sampler2D bumpMap;
	uniform float normalStrength;
#endif

#if defined(MESH_BUFFER)
	uniform float blendNormal;
#endif

#if defined(SRP) && !defined(SHADER_API_METAL)
CBUFFER_END
#endif




float _instancePct;
float _instanceLod;



//---------------------------------------------------------------------------------
//----------------------------STRUCTS--------------------------------------
//---------------------------------------------------------------------------------



#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2) && !defined(GRASS_DEPTH)
	#define FOG_ON
#endif



struct v2g {
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;

	#if defined(REQUIRES_LIGHTMAP_UV)
	float2 lightmapUV : TEXCOORD1;
	#endif

	#if defined(RENDERMODE_MESH) || defined(MESH_BUFFER)
	float4 norm : NORMAL;
	#endif

	#if defined(MESH_BUFFER)
	float4 col : COLOR;
	#endif

	#if defined(SURFACE_TESSELATION)
		float4 clipPos : TEXCOORD2;
		float4 dhfParams : TEXCOORD3;

		#if !defined(RENDERMODE_MESH)
		float4 terrainPos : TEXCOORD4;
		#endif
	#endif
};

#if defined(MESH_BUFFER)
	uniform int grassIndexCount;
	uniform int terrainIndexCount;
	uniform int terrainTriCount;

	uniform StructuredBuffer<v2g> terrainMesh;
	uniform StructuredBuffer<int> terrainTris;

	uniform StructuredBuffer<v2g> grassMesh;
	uniform StructuredBuffer<int> grassTris;
#endif


#if defined(GRASS_TESSELATION)
struct TessFactors {
    float edge[4] : SV_TessFactor;
    float inside[2] : SV_InsideTessFactor;

	float3 tV : TOP_VERT;
	float3 bV : BTM_VERT;
	float3 widthMod : WMOD;
	float4 bladeCol : BCOL;
};


#elif defined(SURFACE_TESSELATION)

struct TessFactors {
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;

	float2 tessDist : TessDist;
};
#endif



//UNITY_INSTANCING_CBUFFER_START(Props)
//UNITY_DEFINE_INSTANCED_PROP(int, _totalInstances)
//UNITY_DEFINE_INSTANCED_PROP(float, _instanceLod)
//UNITY_INSTANCING_CBUFFER_END


//TERRAIN VARS
#if !defined(RENDERMODE_MESH)

uniform sampler2D terrainHeightMap;
uniform sampler2D terrainNormalMap;
uniform float3 terrainSize;
uniform float3 invTerrainSize;
uniform float2 terrainChunkSize;
uniform float terrainExpansion;
uniform float terrainMapOffset;

//UNITY_INSTANCING_CBUFFER_START(Props)
//UNITY_DEFINE_INSTANCED_PROP(float2, _chunkPos)
//UNITY_DEFINE_INSTANCED_PROP(int, _totalInstances)
//UNITY_DEFINE_INSTANCED_PROP(float, _instanceLod)
//UNITY_INSTANCING_CBUFFER_END
uniform float2 _chunkPos;
#endif



#if !defined(SHADOW_CASTER)
struct g2f {
	float4 pos : SV_POSITION;
	float4 col : COLOR;

	float4 uv : TEXCOORD0;

	#if defined(REQUIRES_LIGHTMAP_UV)
	float2 lightmapUV : TEXCOORD1;
	#endif

	#if defined(FRAGMENT_REQUIRES_WORLDPOS)
	float3 worldPos : TEXCOORD2;
	#endif

	#if defined(FRAGMENT_REQUIRES_NORMAL)
	float4 normal : NORMAL;
	#endif

	#if defined(FRAGMENT_REQUIRES_SPECULAR)
	float3 specVec : SPEC;
	#endif

	#if !defined(DEFERRED)
	GRASS_SHADOW_COORDS(5)
	#endif

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
	#endif	
};
#else
struct g2f {
	float4 pos : SV_POSITION;

	//#if defined(SEMI_TRANSPARENT)
	float4 uv : TEXCOORD0;
	//#endif

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
	#endif
};
#endif

struct VertexData {
	float3 vertex;
	float3 norm;
	float3 color;
	float4 dhfParams; //xyz = density, height, flatten, wind str
	float typeParams; //controls grass texture atlas index
};

struct RippleData {
	float4 pos; // w = strength
	float4 drssParams;//xyzw = decay, radius, sharpness, speed
};

struct Counter {
	uint4 val;
};

uniform StructuredBuffer<RippleData> rippleBuffer;
uniform StructuredBuffer<Counter> rippleCount;

uniform StructuredBuffer<RippleData> forcesBuffer;
uniform int forcesCount;
