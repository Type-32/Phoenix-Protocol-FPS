

#define grassToWorld objToWorldMatrix
#define worldToGrass worldToObjMatrix

#define CalcSpecular(specVec, normal, atten) pow(saturate(dot(specVec, normal)), specSmooth * 100) * specularMult * specSmooth * atten * specTint


#if !defined(SRP)
//------------------------STANDARD-------------------------//
//#define grassToWorld unity_ObjectToWorld
//#define worldToGrass unity_WorldToObject
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0))

#define GET_GI(worldNormal) ShadeSH9(worldNormal)
#define GET_LIGHT_DIR(light) _WorldSpaceLightPos0
#define GET_LIGHT_COL(light) _LightColor0.rgb

//the zero is for bogus lightcoords
#define TRANSFER_GRASS_SHADOW(o, wpos) UNITY_TRANSFER_SHADOW(o, 0)

#define GRASS_SHADOW_ATTENUATION(i) UNITY_SHADOW_ATTENUATION(i, i.worldPos)



#elif defined(URP)
//------------------------UNIVERSAL-------------------------//
//#define grassToWorld UNITY_MATRIX_M
//#define worldToGrass UNITY_MATRIX_I_M

#ifdef SRP_SHADOWCASTER
#if UNITY_REVERSED_Z
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));\
				pos.z = min(pos.z, pos.w * UNITY_NEAR_CLIP_VALUE)
#else
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0))\
				pos.z = max(pos.z, pos.w * UNITY_NEAR_CLIP_VALUE)
#endif
#else
#define GetClipPos(worldPos, pos)  mul(UNITY_MATRIX_VP, float4(worldPos, 1.0))
#endif

#define GET_GI(worldNormal) SampleSH(worldNormal)
#define GET_LIGHT_DIR(light) _MainLightPosition.xyz
// unity_LightData.z is 1 when not culled by the culling mask, otherwise 0.
// unity_ProbesOcclusion.x is the mixed light probe occlusion data
#define GET_LIGHT_COL(light) _MainLightColor.rgb * unity_LightData.z * unity_ProbesOcclusion.x


#if defined(SHADOW_CASTER) || defined(DEFERRED)
	#define TRANSFER_GRASS_SHADOW(o, wpos)
#else
	#if defined (SHADOWS_SCREEN)
	#define TRANSFER_GRASS_SHADOW(o, wpos) o._ShadowCoord = ComputeScreenPos(o.pos)
	#else
	#define TRANSFER_GRASS_SHADOW(o, wpos) o._ShadowCoord = (wpos)
	#endif
#endif

#define GRASS_SHADOW_ATTENUATION(i) MainLightRealtimeShadow(TransformWorldToShadowCoord(i._ShadowCoord.xyz))


//annoying that i have to put this here, but due to lame #include order dependancies, yknow
//otherwise,some variables arent decalred yet if it was in the URPInclude.cginc
inline float3 GetURPLighting(float3 wPos, float3 normal){
    float3 lighting = 0;
	int additionalLightsCount = GetAdditionalLightsCount();
	for (int lI = 0; lI < additionalLightsCount; ++lI) {

		#if defined(_ADDITIONAL_LIGHT_SHADOWS)
			//if _ADDITIONAL_LIGHT_SHADOWS is defined then we must be in a version of URP that supports this beefier function
			#if !defined (LIGHTMAP_ON)
			    half4 shadowMask = unity_ProbesOcclusion;
			#else
				//dont really support lightmaps unfortunately
			    half4 shadowMask = half4(1, 1, 1, 1);
			#endif

			Light light = GetAdditionalLight(lI, wPos, shadowMask);

		#else
			Light light = GetAdditionalLight(lI, wPos);
		#endif


		half shade = ambientCO + diffuseCO * saturate(dot(light.direction, normal) + 0.25);
		half atten = light.shadowAttenuation * light.distanceAttenuation;

		#if defined(USE_SPECULAR)
		float3 specVec = normalize(normalize(_WorldSpaceCameraPos.xyz - wPos) + light.direction);
		lighting += light.color * CalcSpecular(specVec, normal, atten);
		#endif

		//col.rgb += light.color * shade * light.shadowAttenuation * light.distanceAttenuation;
		lighting += (light.color * shade * atten);
	}

    return lighting;
}

#endif