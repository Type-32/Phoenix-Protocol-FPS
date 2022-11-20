

//include the other goodies
#if defined(URP)
//this needs to be first since it has the CBUFFER macro that the vars need
#include "GrassURPInclude.cginc"
#endif

#include "GrassStructsVars.cginc"
#include "GrassRenderPipelineHelpers.cginc"
#include "GrassFunc.cginc"


#include "GrassMeshBuffer.cginc"
#include "GrassGeometry.cginc"

#if defined(GRASS_TESSELATION) || defined(SURFACE_TESSELATION)
//-------TESSELATION NONSENSE-----------
#include "GrassTesselators.cginc"
#define USING_TESSELATION
#endif


#if defined(DEFERRED)
#include "GrassDeferred.cginc"
#endif



#if defined (MESH_BUFFER)

#undef BILLBOARD

g2f mesh_vertex_shader(uint vertID : SV_VertexID, uint instID : SV_InstanceID){

	uint indexID = vertID;
	uint primitiveID = (indexID / grassIndexCount);


	#if defined(RENDERMODE_MESH)
	uint sourceTriIdx = (primitiveID % terrainTriCount) * 3; //wrap around

	#define ReadVertex(triIdx, index) terrainMesh[terrainTris[triIdx + index]]
	v2g sourceTri[3] = {
		ReadVertex(sourceTriIdx, 0),
		ReadVertex(sourceTriIdx, 1),
		ReadVertex(sourceTriIdx, 2)
	};
	#else
	v2g sourceTri[3];
	#endif

	uint grassIdx = grassTris[indexID % grassIndexCount];
	v2g grassVert = grassMesh[grassIdx];

	g2f o;
	CreateBladeFromSource(primitiveID, instID, sourceTri, grassVert, o);

	return o;
}
#endif


#if !defined(USING_TESSELATION) && !defined(MESH_BUFFER)

v2g vertex_shader(v2g IN, uint inst : SV_InstanceID) {

	v2g o;
	o.vertex.xyz = IN.vertex.xyz;
	o.vertex.w = inst;


#if defined(RENDERMODE_MESH)
	o.norm = IN.norm;
	o.uv = IN.uv;
#else
	//terrain
	o.uv.xy = _chunkPos;
#endif


#ifdef REQUIRES_LIGHTMAP_UV
	o.lightmapUV = IN.lightmapUV;
#endif

	return o;
}


#ifdef LOWER_QUALITY 
[maxvertexcount(3)]
#elif !defined(MULTI_SEGMENT)
[maxvertexcount(4)]
#else
[maxvertexcount(MAX_BLADE_SEGMENTS * 2 + 2)]
#endif
void geometry_shader(triangle v2g IN[3], inout TriangleStream<g2f> outStream, uint primId : SV_PrimitiveID) {

#define vd1 IN[0]

	//found in GrassGeometry.cginc
	CreateBladeFromSource(primId, vd1.vertex.w, IN, outStream);
}
#endif


#if !defined(SHADOW_CASTER)

#if !defined(DEFERRED)
half4 fragment_shader(g2f i) : SV_Target{
#else
FragmentOutput fragment_shader(g2f i) {
#endif

	//spsi shadow stuff
	#if defined(UNITY_STEREO_INSTANCING_ENABLED)
	UNITY_SETUP_INSTANCE_ID(i);
	#endif

	//return 0;
	#if defined(GF_USE_DITHER)
		#if defined(DEFERRED) && !defined(SURFACE_TESSELATION)
			#define _Dither_Alpha i.uv.w
		#else
			#define _Dither_Alpha i.uv.w
		#endif
		//half alphaRef = tex3D(_DitherMaskLOD, float3((i.worldPos.xy + i.worldPos.z)*2, _Dither_Alpha*0.9375 + 0.0001)).a;
		half alphaRef = tex3D(_DitherMaskLOD, float3(i.pos.xy * 0.25, _Dither_Alpha * 0.9375 + 0.0001)).a;
		clip(alphaRef - 0.01);

		#if defined(DEFERRED)
		i.col.a = 1;
		#endif

	#endif

	half4 texCol = tex2D(_MainTex, i.uv);
#if defined(SEMI_TRANSPARENT)
	clip(texCol.a - alphaClip);
#endif
	if(alphaLock) texCol.a = 1;



	#if defined(GF_NORMAL_MAP)
	i.normal.xyz = normalize(i.normal.xyz + UnpackNormal((tex2D(bumpMap, i.uv) - 0.5) * 2) * normalStrength);
	#endif


	float3 lighting = 1;
#ifdef FORWARD_ADD
	UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
	lighting = GetMainLighting(i.worldPos, i.normal) * atten;
#else

	#if defined(GF_PPLIGHTS) && !defined(DEFERRED)
	lighting = GetMainLighting(i.worldPos, i.normal);
	#endif

	#if (defined(SHADOWS_SCREEN) || defined(SRP)) && !defined(DEFERRED)
		float atten = GRASS_SHADOW_ATTENUATION(i);
		lighting *= ambientCOShadow + (1.0 - ambientCOShadow) * atten;
	#else
		float atten = 1;
	#endif
#endif

	
#if defined(USE_SPECULAR)
	float3 spec = CalcSpecular(i.specVec, i.normal, atten);
	i.col.rgb += GET_LIGHT_COL(i) * specTint * spec;

	//i dont think reflections are sensical on grass 🤔
	//float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
	//float3 reflectionDir = reflect(-viewDir, i.normal);
	//float4 envSample = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflectionDir);
	//i.col.rgb += DecodeHDR(envSample, unity_SpecCube0_HDR);
#endif

	texCol.rgb *= lighting;
	texCol = (texCol * i.col);



	UNITY_APPLY_FOG(i.uv.z, texCol);


#if defined(DEFERRED)
	return GrassToGBuffer(i, texCol);
#else
	return texCol;
#endif

}
#endif

#if defined(SHADOW_CASTER)
void fragment_shader(g2f i) {


	half alpha = 1;

#if defined(SEMI_TRANSPARENT)
	alpha = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)).a;
#endif

	//i.uv.w contains alpha from lod stuff
	alpha *= tex3D(_DitherMaskLOD, float3(i.pos.xy * 0.25, i.uv.w * 0.9375 + 0.0001)).a;

	clip(alpha - alphaClip);

}
#endif