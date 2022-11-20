
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles


#define INTERPOLATE_BC(source, fieldName, UV) \
	source[0]fieldName * UV.x + \
	source[1]fieldName * UV.y + \
	source[2]fieldName * UV.z

static float2 minmax = float2(0, 1);
static float2 UVT[3] = {
	float2(minmax.x, minmax.x),
	float2(minmax.y * 0.5, minmax.y),
	float2(minmax.y, minmax.x),
};

static float NaN = sqrt(-1 - 1.0);


void CreateBladeFromSource(uint primId, float instID,
	#if defined(GRASS_TESSELATION)
	InputPatch<v2g, 3> IN, inout TessFactors tessF
	#elif defined(SURFACE_TESSELATION)
	OutputPatch<v2g, 3> IN, VertexData inVD, TessFactors tessF, inout g2f out2f
	#elif defined(MESH_BUFFER)
	inout v2g IN[3], v2g grassVert, inout g2f out2f
	#else
	//geometry
	inout v2g IN[3], inout TriangleStream<g2f> outStream
	#endif
) {

	#if defined(GRASS_TESSELATION)
	#define Cull\
			tessF.tV = NaN;\
			tessF.bV = NaN;\
			tessF.bladeCol = NaN;\
			tessF.widthMod = NaN;\
			return
	#elif defined(SURFACE_TESSELATION) || defined(MESH_BUFFER)
	#define Cull\
			lvd.vertex = NaN;\
			out2f.pos = NaN;\
			return
	#else
	#define Cull return
	#endif

	#define vd1 IN[0]
	#define vd2 IN[1]
	#define vd3 IN[2]

	g2f o = (g2f)0;

	#if defined(UNITY_STEREO_INSTANCING_ENABLED)

	o.instanceID = instID;
	HandleSinglePassStereoInstanced(o); //Insert

	//this is also really dumb but basically we need to make sure that the instID is halved and floored
	//since in stereo instancing each instance is rendered once per eye and they count as separate instances
	//doing this ensures each pair of instances is set back to what the original non stereo instanced id would be
	instID = floor(instID * 0.5);
	#endif


	rngfloat rndSeed = rngfloat(primId, instID) + 2.1378952;
	rngfloat truSeed = rndSeed;

	//this is just some weird backwater hack to get deferred to render at all when not EXPLICITLY using the object matrix
	//because for some god forasaken reason someone, somewhere, some absolute babboon probably decided this as some kind of idiotic optimization
	//EVEN THOUGH I USE MY OWN MATRICES
	//but no, it wasnt bad enough for them to do this to me, they had to do it silently, and without warning or error
	//and WHY ON EARTH this ONLY applies to deferred, i have no idea
	//so here I have to force the object matrix to be used in the shader somewhere so it actually renders
	//this has to be a new level of stupid
	//to add insult to injury i have to feel like an idiot for doing this workaround and will forever feel like im missing something and doing it wrong
	#if defined(SRP)
	rndSeed += UNITY_MATRIX_M._m30;
	#else
	rndSeed += unity_ObjectToWorld._m30;
	#endif


	#if defined(SURFACE_TESSELATION)
	#define buv out2f.uv.xyz

	#if defined(RENDERMODE_MESH)
	//interpolate uv
	float2 rndUV = INTERPOLATE_BC(IN, .uv, buv);
	#else
	//terrain UV comes from vertexPos
	float2 rndUV = inVD.vertex.xz;
	#endif

	float4 tessUV = float4(INTERPOLATE_BC(UVT, .xy, buv), 0, 0);
	if (truRnd(rndSeed) > 0.5) tessUV.xy *= -1;

	float4 rndmSamp = tex2Dlod(randoMap, float4(tessUV.xy
		+ float2(truRnd(rndSeed), truRnd(rndSeed)), 0, 0)
	).rgbg;

	truSeed += buv;
	//rndSeed += grassDisplace;
	//rndSeed += rndmSamp;
	//rndSeed = 0;

	float rndInc = 0;
	#define TexRnd rndmSamp[frac(rndInc += 0.25) * 4]
	//#define TexRnd rndmSamp.x

	#else
	float2 rndUV = float2(rndm(rndSeed), rndm(rndSeed));
	#define TexRnd rndm(rndSeed)
	#endif

	VertexData lvd;
	#if defined(SURFACE_TESSELATION)
	lvd = inVD;
	#endif

	#if !defined(RENDERMODE_MESH)
	//TERRAIN MODE
	GetHeightmapData(lvd, rndUV, rndSeed);

	#if !defined(SURFACE_TESSELATION)
	UNITY_BRANCH
		static const float edgeBound = 0.15;
	if (lvd.vertex.x < -edgeBound || lvd.vertex.z < -edgeBound ||
		lvd.vertex.x > terrainSize.x + edgeBound || lvd.vertex.z > terrainSize.z + edgeBound) {
		//cull blades that are off the edge of the terrain
		Cull;
	}
	#endif
	#else
	//MESH MODE
	LerpVertData(lvd, rndUV, vd1, vd2, vd3, rndSeed);
	#endif



	#if defined(SURFACE_TESSELATION)
	if (lvd.dhfParams.x < 0.05) {
		Cull;
	}
	#else
	UNITY_BRANCH
		if (rndm(rndSeed) > lvd.dhfParams.x) {
			Cull;
		}
	#endif	

	float3 worldPos = mul(grassToWorld, float4(lvd.vertex.xyz, 1));


	float3 toTri = worldPos - _WorldSpaceCameraPos;
	#if defined(GF_USE_DITHER)
	float ditherRand = TexRnd; //have to do this here to make sure the rng stays in sync with the shadows
	#if defined(DEFERRED) || defined(GF_USE_DITHER) || defined(SHADOW_CASTER)
	//variate length to avoid artifacting in dithering
	toTri *= ditherRand * 0.35 + 1.0;
	#endif
	#endif

	//calculate fade alpha
	float camDist = rsqrt(dot(toTri, toTri));
	half alphaBlendo = saturate(pow(camDist * grassFade, grassFadeSharpness));


	#if defined(SHADOW_CASTER)
	float fracFade = alphaBlendo;
	#else
	//makes it so that the distance fade is not dithered by not alphaBlendo for fracFade
	//not sure if this tradeoff is worth it since it allows some minor depth popping at the distance fade
	//but if the distance fade is dithered at lower resolutions the dither pattern becomes fairly apparent
	float fracFade = 1;
	#endif
	if (instID > 0 && instID > (_instanceLod)-1) {
		fracFade = frac((_instanceLod));
		fracFade = (fracFade != 0 ? fracFade : 1);

		fracFade *= alphaBlendo;
		#if !defined(GF_USE_DITHER)
		alphaBlendo = fracFade;
		#endif
	}

	o.uv.w = fracFade;


	#if defined(SURFACE_TESSELATION)

	//prevent Z-fighting
	lvd.vertex += lvd.norm * 0.005;
	lvd.vertex += lvd.norm * pow(saturate(0.02 - camDist), 4) * 10000000;
	//lvd.vertex += lvd.norm * pow(1 - tessF.tessDist, 2) * variance.w + 0.01;

	half closeBlendo = saturate(tessClose - camDist - 0.001) * grassFade;
	alphaBlendo = saturate(alphaBlendo * closeBlendo);

	o.uv.w = saturate(alphaBlendo * 2);
	#undef BILLBOARD


	#else
	UNITY_BRANCH
		if (alphaBlendo < 0.05) {
			Cull;
		}
	#endif

	#if defined(SURFACE_TESSELATION)
	float dScale = tessF.tessDist * tessDispScale;
	float3 grassDisplace = tex2Dlod(grassDisplacer, tessUV * max(1, dScale) * lvd.dhfParams.x);
	#endif



	#if !defined(BILLBOARD)
	//float3 camRight = float3(1, rndm(rndSeed) * 0.5f - 0.25f, rndm(rndSeed) * 0.5f - 0.25f);
	float3 camRight = normalize(float3(truRnd(rndSeed) * 0.5f - 0.25f, truRnd(rndSeed) * 0.5f - 0.25f, truRnd(rndSeed) * 0.5f - 0.25f));
	#else

	//float3 camRight = mul((float3x3)unity_CameraToWorld, float3(1, rndm(rndSeed) * 0.5f - 0.25f, rndm(rndSeed) * 0.5f - 0.25f));

	//gets the camera-right vector
	float3 camRight = unity_WorldToCamera[0].xyz
		+ unity_WorldToCamera[1].xyz * (rndm(rndSeed) * 0.5f - 0.25f)
		+ unity_WorldToCamera[2].xyz * (rndm(rndSeed) * 0.5f - 0.25f)
		;

	//camRight = mul((float3x3)worldToGrass, camRight);
	#endif


	#if defined(SEMI_TRANSPARENT)
	#define _MaxTexAtlasSize 16.0
	float typeSamp = lvd.typeParams * _MaxTexAtlasSize;

	#define typeIdx floor(typeSamp)
	#define typePct frac(typeSamp)

	camDist = 1.0 + (typeIdx < textureAtlasScalingCutoff ? widthLODscale : 0) / camDist;
	#else
	camDist = 1.0 + widthLODscale / camDist;
	#endif




	float widthMult = (1.0 + lvd.dhfParams.z * 0.5) * bladeWidth * Variate(rndm(rndSeed), variance.w) * camDist;
	half3 widthMod = camRight * widthMult;

	float finalHeight = GET_FINAL_HEIGHT(lvd);


	#if defined(MESH_BUFFER)
	#undef BILLBOARD
	float customHeight = finalHeight * grassVert.col.r;
	float normHeight = grassVert.col.g;
	float sharp = lerp(widthMult * (1 - saturate(normHeight)), widthMult, bladeSharp);

	float3x4 tMatrix = GetSurfaceRotationMatrix(lvd.vertex, lvd.norm, float3(sharp, bladeHeight, sharp), TexRnd);
	//zero out y pos before multiplying to keep it flat since height is controlled later by grass positioning
	grassVert.vertex.y = 0;
	lvd.vertex = mul(tMatrix, grassVert.vertex);
	finalHeight *= normHeight;
	float3 worldNormal = lerp(mul((float3x3)tMatrix, grassVert.norm), lvd.norm, blendNormal);
	worldNormal = normalize(GrassToWorldNormal(worldNormal));

	#else
	float3 worldNormal = GrassToWorldNormal(lvd.norm);
	#endif

	#ifdef FRAGMENT_REQUIRES_NORMAL
	o.normal = float4(worldNormal.xyz, 1);
	#endif

	#if defined(SURFACE_TESSELATION)
	//handle weird surface tesselation displacement
	float normHeight = saturate((finalHeight * grassDisplace) / (lvd.dhfParams.y * bladeHeight));
	finalHeight *= grassDisplace + rndmSamp.x * variance.y * normHeight;

	//finalHeight *= tessF.tessDist;
	#endif

	#if defined(GEOMETRY)
	float normHeight = 1;
	#endif

	float3 posVariance = GET_POS_VARIANCE(finalHeight);

	//posVariance = 0; finalHeight = 0;
	float3 tV = TP_Vert(lvd, posVariance, finalHeight, 1);
	float3 lV = worldPos - widthMod;
	float3 rV = worldPos + widthMod;

	#if defined(MESH_BUFFER)	
	finalHeight = customHeight;
		#if defined(MULTI_SEGMENT)
		float fh2 = finalHeight * finalHeight;
		finalHeight = lerp(finalHeight, fh2, bladeStiffness);
		#endif
	#endif

	half noiseSamp;
	float3 windAdd = GetWindAdd(tV, lvd, finalHeight, noiseSamp);
	tV = mul(grassToWorld, float4(tV, 1));
	float3 rippleForce = GetRippleForce(tV, noiseSamp);
	// float3 rippleForce = 0;



	float3 topView = -unity_WorldToCamera[1].xyz * saturate(dot(unity_WorldToCamera[2].xyz, -worldNormal) - 0.5) * (-3 * topViewPush);

	#if defined (SURFACE_TESSELATION) || defined(MESH_BUFFER)
	tV += rippleForce * normHeight;
	tV += windAdd * normHeight;
	tV += topView * normHeight;
	worldPos = tV;
	#else
	//ApplyRipples(tV);
	tV += rippleForce;
	tV += windAdd;
	//push top vert away from camera when look down
	tV += topView;
	#endif






	#if defined(SEMI_TRANSPARENT)

	//since our type pct can only technically store 16 values and can never actually be "1"
	//our 16 values are actually 0-15
	//so we need this value to scale the rnd number into the space of our fractional type pct
	//to be clear, typePct will only ever be 0 to 0.9375 and occur in steps of (1 / 16 = 0.0625)
	const static float rndScale = 1.0 - (1.0 / 16.0); // = 0.9375

	#if defined(SURFACE_TESSELATION)
	//well this kinda sucks but its basically impossible to support type inbetween percentages due to tesselation wibble randoms
	// at least close up is very noticeable that the UV's wont line up for the texture types and get spread across the whole atlas

	//this method uses the next texture down instead of tex 0
	//looks alright but forces use of unwanted textures kinda, but this can be worked around i think
	float uvXL = (TexRnd * rndScale) < typePct ? typeIdx * numTexturesPctUV : saturate((typeIdx - 1) * numTexturesPctUV);

	//this method allows only 100% types, might be better in some situations
	//float uvXL = typePct >= rndScale ? typeIdx * numTexturesPctUV : 0;
	#else

	float uvXL = (TexRnd * rndScale) < typePct ? typeIdx * numTexturesPctUV : 0;
	//float uvXL = typeIdx * numTexturesPctUV;
	#endif

	float uvXR = uvXL + numTexturesPctUV;
	#else
	static float uvXL = 0;
	static float uvXR = 1;
	#endif

	#if !defined(SHADOW_CASTER) && !defined(FORWARD_ADD) && !defined(DEFERRED)

		//noiseSamp = 0.8f + noiseSamp * 0.25f;
		noiseSamp = noiseSamp * 1.5 - 0.5;
		float3 windTintAdd = float3(1, 1, 1) + windTint.rgb * windTint.a * noiseSamp;
		windTintAdd += flatTint.rgb * flatTint.a * lvd.dhfParams.z;

		//TOP Vert - no AO on this one
		//ShadeVert(o, tV, lvd, shade, noiseSamp, alphaBlendo, rndSeed, float2(0.5, 1.0));
		float4 bladeCol = float4(lvd.color * pow(Variate(TexRnd, variance.z), 0.4), alphaBlendo);
		bladeCol.rgb *= windTintAdd;


		#if !defined(GF_PPLIGHTS)
		bladeCol.rgb *= GetMainLighting(tV, worldNormal);
		#endif

		o.col = bladeCol;
	#endif


	#if defined(FORWARD_ADD)
	float4 bladeCol = float4(lvd.color, alphaBlendo);
	o.col = bladeCol;
	#endif

	#if defined(DEFERRED)
	noiseSamp = noiseSamp * 1.5 - 0.5;
	float3 windTintAdd = float3(1, 1, 1) + windTint.rgb * windTint.a * noiseSamp;
	float4 bladeCol = float4(lvd.color * pow(Variate(TexRnd, variance.z), 0.4) * windTintAdd, alphaBlendo);
	o.col = bladeCol;
	o.uv.w *= alphaBlendo;
	#endif

	


	#if !defined(SHADOW_CASTER)
	//Reduce AO a bit on flattened grass and variate AO a smidge
	float3 startCol = bladeCol;
	float aoValue = lerp(
		lvd.dhfParams.z + (TexRnd * 0.2) + _AO + (noiseSamp * 0.2),
		1.0,
		(1.0 - lvd.dhfParams.x) * 0.6
	);
	bladeCol.rgb *= aoValue; // apply AO
	//bladeCol *= lvd.dhfParams.x * 0.5 + 0.5; // reduce AO based on grass density;
	bladeCol.a = alphaBlendo;
	#endif


	#if defined(USE_SPECULAR)

	#if defined(FORWARD_ADD)
	float3 lightDir = normalize(GET_LIGHT_DIR(o) - worldPos);
	#else
	//directional
	float3 lightDir = GET_LIGHT_DIR(o);
	#endif

	float3 specVec = normalize(normalize(_WorldSpaceCameraPos.xyz - worldPos) + lightDir);
	float specHeightMult = lerp(aoValue, 1 + (noiseSamp * 0.2), specHeight);
	#define ApplySpecular(t) o.specVec = specVec * lerp(specHeightMult, 1, t)
	ApplySpecular(normHeight);
	#else
	#define ApplySpecular(t)
	#endif



	#if defined(MESH_BUFFER)

	#if !defined(SHADOW_CASTER)
	o.col = lerp(bladeCol, o.col, normHeight);
	o.col.a = alphaBlendo;
	#endif

	grassVert.uv.x = grassVert.uv.x * numTexturesPctUV + uvXL;

	SET_WORLDPOS(o, worldPos);
	o.pos = GetClipPos(worldPos, o.pos);
	SET_UV(float3(grassVert.uv, o.pos.z));
	TRANSFER_GRASS_SHADOW(o, worldPos);

	out2f = o;

	return;
	#elif defined(GRASS_TESSELATION)
	//TESSELATION

	tessF.tV = tV;
	tessF.bV = worldPos;
	tessF.widthMod = widthMod;

	#if !defined(SHADOW_CASTER)
	tessF.bladeCol = bladeCol;
	#else
	tessF.bladeCol = alphaBlendo;
	#endif

	return;

	#elif defined(SURFACE_TESSELATION)
	//SURFACE_TESSELATION
	#if !defined(SHADOW_CASTER)

	//o.col = lerp(bladeCol, o.col, pow(normHeight, variance.w));
	o.col = lerp(bladeCol, o.col, normHeight);
	//o.col.rgb *= grassDisplace;
	//o.col.rgb = rndmSamp;
	#endif

	#if !defined(BILLBOARD)
	float triH = abs(dot((camRight), float3(1, 0, 0)));
	float3 uvP = inVD.vertex * tessTexScale;
	tessUV.x = MapFrom01(frac(lerp(uvP.x, uvP.z, triH)), uvXL, uvXR);
	#else
	tessUV.x = MapFrom01(frac(tessUV.x), uvXL, uvXR);
	#endif

	tessUV.y = normHeight;


	worldPos = tV;
	SET_WORLDPOS(o, worldPos);
	o.pos = GetClipPos(worldPos, o.pos);
	SET_UV(float3(tessUV.xy, o.pos.z));
	TRANSFER_GRASS_SHADOW(o, worldPos);

	out2f = o;
	return;

	#else

	//GEOMETRY_STAGE
	#define AppendGeo(wPos, uv) \
		worldPos = wPos;\
		SET_WORLDPOS(o, worldPos);\
		o.pos = GetClipPos(worldPos, o.pos);\
		SET_UV(uv);\
		TRANSFER_GRASS_SHADOW(o, worldPos);\
		outStream.Append(o)

	#define PushLevel(posL, uvL, posR, uvR) AppendGeo(posL, uvL); AppendGeo(posR, uvR)
	#define PushLevel0(posL, uvL, posR, uvR) PushLevel(posL, uvL, posR, uvR)
	#define PushLevelF(posL, uvL, posR, uvR) PushLevel(posL, uvL, posR, uvR)


	//Top Verts
	//o.normal = float4(normalize(lerp(worldPos - tV, worldNormal, 0.1)), 1);
	PushLevel0(float4(tV - widthMod * bladeSharp, 1.0), float3(uvXL, 1.0, o.pos.z),
		float4(tV + widthMod * bladeSharp, 1.0), float3(uvXR, 1.0, o.pos.z));


	#if defined(MULTI_SEGMENT) && !defined(GRASS_TESSELATION)

	int bladeSegments = (saturate(_instancePct)) * BLADE_SEG_DIFF + MIN_BLADE_SEGMENTS;
	float iSeg = 1.0 / bladeSegments;

	for (float i = (bladeSegments - 1); i >= 1; i--) {

		float t = i * iSeg;
		float t2 = t * t;

		tV = TP_Vert(lvd, posVariance * t, finalHeight * t, t2);
		tV = mul(grassToWorld, float4(tV, 1));
		tV += (windAdd + rippleForce) * lerp(t2, t, bladeStiffness);
		tV += topView * t;

		#if !defined(SHADOW_CASTER)
		o.col.rgb = lerp(bladeCol.rgb, startCol, t);
		#endif

		ApplySpecular(t);

		float sharpLerp = lerp(1, bladeSharp, t2);


		PushLevel(float4(tV - widthMod * sharpLerp, 1.0), float3(uvXL, t, o.pos.z),
			float4(tV + widthMod * sharpLerp, 1.0), float3(uvXR, t, o.pos.z));
	}
	#endif



	#if !defined(SHADOW_CASTER)
	o.col = bladeCol;

	#ifdef PROCEDURAL
	fillVert.color = bladeCol;
	#endif
	#endif


	#if defined(DEFERRED)
	o.normal.w = aoValue;
	#endif

	ApplySpecular(0);
	//Bottom Verts
	PushLevelF(float4(lV, 1), float3(uvXL, 0.0, o.pos.z),
		float4(rV, 1), float3(uvXR, 0.0, o.pos.z));

	//#define big 1
	//#define side instID * big
	//#define triO worldPos

	//fillVert.color = 1;

	//PushLevel0(triO + float3(0, 0,     0), float3(0, 0, o.pos.z),
	//		   triO + float3(big, 0,   0), float3(1, 0, o.pos.z));
	//PushLevelF(triO + float3(0, big,   0), float3(0, 1, o.pos.z),
	//		   triO + float3(big, big, 0), float3(1, 1, o.pos.z)); 

	//END GEOMETRY
	#endif

}

