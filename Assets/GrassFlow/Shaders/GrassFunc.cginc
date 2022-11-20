

#define Variate(rnd, variance) (1.0f - rnd * variance)
#define VariateBalanced(rnd, variance) (rnd * variance - variance * 0.5)
#define RndFloat3 float3(rndm(rndSeed), rndm(rndSeed), rndm(rndSeed))
#define Float4(val) float4(val,val,val,1.0f)
#define Float3(val) float3(val,val,val)

#define lerp3(v1, v2, v3, UV) (lerp(lerp(v1, v2, UV.x), v3, UV.y))

#if !defined(GRASS_DEPTH) || defined(SEMI_TRANSPARENT) || defined(GRASS_TESSELATION)
#if defined(FOG_ON)
#define SET_UV(inuv) o.uv.xyz = float3(TRANSFORM_TEX(inuv.xy, _MainTex), inuv.z)
#else
#define SET_UV(inuv) o.uv.xy = TRANSFORM_TEX(inuv.xy, _MainTex)
#endif
#else
#define SET_UV(uv) 
#endif

#if defined(FRAGMENT_REQUIRES_WORLDPOS) && !defined(SHADOW_CASTER)
#define SET_WORLDPOS(o, inPos) o.worldPos = inPos
#else
#define SET_WORLDPOS(o, inPos)
#endif

#define HandleSinglePassStereoInstanced(v)\
UNITY_SETUP_INSTANCE_ID(v);\
UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)



#define CalcSpecular(specVec, normal, atten) pow(saturate(dot(specVec, normal)), specSmooth * 100) * specularMult * specSmooth * atten * specTint


//float3 VorHash(float3 x)
//{
//	x = float3(dot(x, float3(127.1, 311.7, 74.7)),
//		dot(x, float3(269.5, 183.3, 246.1)),
//		dot(x, float3(113.5, 271.9, 124.6)));
//
//	return frac(sin(x)*43758.5453123);
//}

#define rngfloat float2


//inline float rndm(inout rngfloat x)
//{
//	x = rngfloat(dot(x, rngfloat(127.1, 311.7, 74.7, 211.3)),
//				 dot(x, rngfloat(269.5, 183.3, 246.1, 69.2)),
//				 dot(x, rngfloat(113.5, 271.9, 124.6, 301.3)),
//				 dot(x, rngfloat(308.2, 143.6, 53.4, 192.1)));
//
//	x = frac(sin(x)*43758.5453123);
//
//	return x;
//}

#if defined(SURFACE_TESSELATION)
inline float rndm(inout rngfloat x){

	return frac(x.x);
}
inline float truRnd(inout rngfloat x){

	x = frac(sin(rngfloat(
		dot(x, rngfloat(127.1, 311.7)),
		dot(x, rngfloat(269.5, 183.3))
	)) * 43758.5453123);

	return frac(x.x);
}
#else
//this seems to be about the bare minimum i can get away with on rng before noticeable artifacts start showing up
inline float rndm(inout rngfloat x) {
	//ok so, in my testing this is somehow faster than the one below
	//even though it has one more dot(), idk, nothing makes sense anymore
	//i wanna go home

	x = frac(cos(rngfloat(
		dot(x, rngfloat(127.1, 311.7)),
		dot(x, rngfloat(269.5, 183.3))
	)) * 43758.5453123);

	//x = dot(x, rngfloat(127.1, 311.7));
	//x = frac(sin(x) * 43758.5453123);

	return x.x + 0.00000001;
}
inline float truRnd(inout rngfloat x){
	return rndm(x);
}
#endif

//in my testing at least, somehow this is slower than the above method
//despite the other one having a sin() in it
//perhaps this varies on different hardware but man idk what to do
//inline float rndm(inout rngfloat p)
//{
//	float3 p3 = frac(float3(p.xyx) * .1031);
//	p3 += dot(p3, p3.yzx + 33.33);
//	p = frac((p3.x + p3.y) * p3.z);
//	return p + 0.00000001;
//}

//older rnd method, i think its technically better quality but the rng quality isnt super vital for this application
//static float4 _q = float4(1225.0, 1585.0, 2457.0, 2098.0);
//static float4 _r = float4(1112.0, 367.0, 92.0, 265.0);
//static float4 _a = float4(3423.0, 2646.0, 1707.0, 1999.0);
//static float4 _m = float4(4194287.0, 4194277.0, 4194191.0, 4194167.0);

//inline float rndm(inout rngfloat n) {
//	rngfloat beta = floor(n / _q);
//	rngfloat p = _a * (n - beta * _q) - beta * _r;
//	beta = (sign(-p) + rngfloat(1.0, 1.0, 1.0, 1.0)) * rngfloat(0.5, 0.5, 0.5, 0.5) * _m;
//	n = (p + beta);
//
//	return frac(dot(n / _m, rngfloat(1.0, -1.0, 1.0, -1.0)));
//}


//#define PI 3.14159265358979323846264
//#define stddev 0.3
//#define mean 0.5
//float gaussrand(float2 co)
//{
//	// Box-Muller method for sampling from the normal distribution
//	// http://en.wikipedia.org/wiki/Normal_distribution#Generating_values_from_normal_distribution
//	// This method requires 2 uniform random inputs and produces 2 
//	// Gaussian random outputs.  We'll take a 3rd random variable and use it to
//	// switch between the two outputs.
//
//	float U, V, R, Z;
//	// Add in the CPU-supplied random offsets to generate the 3 random values that
//	// we'll use.
//	U = frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453) + 0.000000000001;
//	V = frac(sin(dot(co.yx, float2(52.9898, 18.233))) * 13758.5453) + 0.000000000001;
//	R = frac(sin(dot(co.xy, float2(62.9898, 38.233))) * 93758.5453) + 0.000000000001;
//	// Switch between the two random outputs.
//	if (R < 0.5)
//		Z = sqrt(-2.0 * log(U)) * sin(2.0 * PI * V);
//	else
//		Z = sqrt(-2.0 * log(U)) * cos(2.0 * PI * V);
//
//	// Apply the stddev and mean.
//	return Z * stddev + mean;
//}

#define map(value, fromSource, toSource) (value - fromSource) / (toSource - fromSource)
#define MapFrom01(value, lower, upper) (lower + (upper - lower) * value)

inline float3 GrassToWorldNormal(in float3 norm)
{
#ifdef UNITY_ASSUME_UNIFORM_SCALING
	return (mul((float3x3)grassToWorld, norm));
#else
	return (mul(norm, (float3x3)worldToGrass));
#endif
}

inline float3 WorldToGrassDir(in float3 dir)
{
	return normalize(mul((float3x3)worldToGrass, dir));
}

inline void SampleVertexData(inout VertexData ovd, float2 UV){

	half4 colSamp = tex2Dlod(colorMap, float4(TRANSFORM_TEX(UV, colorMap), 0, 0));
	ovd.color = lerp(colSamp.rgb, colSamp.rgb * _Color, colSamp.a);

	//ovd.color = colSamp.rgb * colSamp.a;
	//ovd.color = lerp(ovd.color, _Color, ovd.color.x + ovd.color.y + ovd.color.z == 0);

	ovd.dhfParams = tex2Dlod(dhfParamMap, float4(TRANSFORM_TEX(UV, dhfParamMap), 0, 0));
	ovd.dhfParams.z = saturate(1.0 - ovd.dhfParams.z) * 0.75;

	ovd.typeParams = tex2Dlod(typeMap, float4(TRANSFORM_TEX(UV, typeMap), 0, 0)).r;

	//ovd.color = lerp3(v1.color, v2.color, v3.color, UV);
	//ovd.dhfParams = lerp3(v1.dhfParams, v2.dhfParams, v3.dhfParams, UV);
}

#ifdef RENDERMODE_MESH

inline void LerpVertData(inout VertexData ovd, inout float2 UV, v2g v1, v2g v2, v2g v3, inout rngfloat rndSeed) {
	//ovd.test = 1;

	#if defined(SURFACE_TESSELATION)
		//TESSELATIONS
		//uhh don't really need to do anything here :)
	#else
		#ifdef LOWER_QUALITY
			ovd.vertex = lerp3(v2.vertex, v1.vertex, v3.vertex, UV);
			UV = lerp3(v1.uv, v2.uv, v3.uv, UV);
			ovd.norm = lerp3(v3.norm, v1.norm, v2.norm, UV);
		#else
		
			float sqrtR = 1.0 / rsqrt(UV.x);
			float a = 1.0 - sqrtR;
			float b = sqrtR * (1.0 - UV.y);
			float c = sqrtR * UV.y;
		
			#define PointMult(v1, v2, v3) v1 * a + v2 * b + v3 * c
		
			ovd.vertex = PointMult(v1.vertex, v2.vertex, v3.vertex);
			ovd.norm = PointMult(v1.norm, v2.norm, v3.norm);
			UV = PointMult(v1.uv, v2.uv, v3.uv);
		
		#endif
	#endif

	SampleVertexData(ovd, UV);
}
#else

#if !defined(SRP)
float ReadHeightmap(float4 height) {
#if (API_HAS_GUARANTEED_R16_SUPPORT)
	return height.r;
#else
	return (height.r + height.g * 256.0f) / 257.0f; // (255.0f * height.r + 255.0f * 256.0f * height.g) / 65535.0f
#endif
}
#endif


inline float3 GetHeightmapSimple(inout float2 rndUV){

	float3 bladePos;

	bladePos.xz = _chunkPos + terrainChunkSize * rndUV;

	rndUV = bladePos.xz * invTerrainSize.xz;
	rndUV = rndUV * (1 - terrainMapOffset * 2) + terrainMapOffset;

#ifdef BAKED_HEIGHTMAP
	//older baked heightmap mode
	float heightSamp = tex2Dlod(terrainHeightMap, float4(rndUV, 0, 0));
#else 
	//separate stuff for unity 2018.3 or newer when using the heightmap directly from the terrain object
	float heightSamp = ReadHeightmap(tex2Dlod(terrainHeightMap, float4(rndUV, 0, 0))) * 2;
#endif

	bladePos.y = heightSamp * terrainSize.y;

	return bladePos;
}



inline void GetHeightmapData(inout VertexData ovd, inout float2 rndUV, inout rngfloat rndSeed) {
	
	float3 bladePos;

	#if defined(SURFACE_TESSELATION)
	bladePos.xz = _chunkPos + terrainChunkSize * rndUV;
	#else
	float tmpo = rndm(rndSeed) - 0.5;
	float2 chunkVariance = float2(tmpo, tmpo) * rndm(rndSeed) * terrainChunkSize * terrainExpansion;
	bladePos.xz = _chunkPos + terrainChunkSize * rndUV + chunkVariance;
	#endif


	float2 rndUVNoOffset = rndUV;

	rndUVNoOffset = bladePos.xz * invTerrainSize.xz;
	rndUV = rndUVNoOffset * (1 - terrainMapOffset * 2) + terrainMapOffset;

#ifdef BAKED_HEIGHTMAP
	//older baked heightmap mode
	float heightSamp = tex2Dlod(terrainHeightMap, float4(rndUV, 0, 0));

#else 
	//separate stuff for unity 2018.3 or newer when using the heightmap directly from the terrain object
	float heightSamp = ReadHeightmap(tex2Dlod(terrainHeightMap, float4(rndUV, 0, 0))) * 2;
#endif

	float3 normalSamp = tex2Dlod(terrainNormalMap, float4(rndUV, 0, 0));

	bladePos.y = heightSamp * terrainSize.y;

	//bladePos.y += (bladePos.x > terrainSize.x || bladePos.x < 0 || bladePos.z > terrainSize.z || bladePos.z < 0) * -999999.0;

	ovd.vertex = bladePos;

	//not super happy about this calc but i tested it on a super heavy scene and it made
	//no appreciable difference +- 0.1 ms
	//basically just checks if the normal is empty and replaces it if so
	ovd.norm = lerp(normalSamp, float3(0, 1, 0), (normalSamp.x + normalSamp.y + normalSamp.z) == 0);

	SampleVertexData(ovd, rndUVNoOffset);
}
#endif

static float3 upVec = float3(0, 1, 0);
#if defined(RENDERMODE_MESH)
#define UP_VEC normalize(mul((float3x3)worldToGrass, upVec))
#else
#define UP_VEC upVec
#endif

#if defined(SURFACE_TESSELATION)
#define GET_FINAL_HEIGHT(vd) bladeHeight * vd.dhfParams.y
#define GET_POS_VARIANCE(finalHeight) (rndmSamp.yzw * 2 - 1) * variance.x * finalHeight
#else
#define GET_FINAL_HEIGHT(vd) bladeHeight * vd.dhfParams.y * Variate(rndm(rndSeed), variance.y)
#define GET_POS_VARIANCE(finalHeight) VariateBalanced(RndFloat3, variance.x) * finalHeight
#endif







//GEOMETRY 
inline float3 TP_Vert(VertexData vd, float3 posVariance, float finalHeight, float t) {

	float3 pos = vd.vertex + lerp(vd.norm, UP_VEC, seekSun) * finalHeight +
		posVariance +
		vd.dhfParams.z * finalHeight * flatnessMult * -vd.norm;

#ifdef MULTI_SEGMENT
	float3 curveDir = (windDir * 3 + cross(vd.norm, posVariance));
	pos += (
		((bladeLateralCurve * curveDir * t) +
		(bladeVerticalCurve * -vd.norm * t)) * (1 - vd.dhfParams.z) +
		(cross(vd.norm, float3(1, 0, 0)) * bladeLateralCurve * 10 * t) * vd.dhfParams.z * vd.dhfParams.y
		) * finalHeight;
#endif

	//extend length of flattened grass
	pos += (pos - vd.vertex) * vd.dhfParams.z;


	return pos;
}

inline float3 GetWindAdd(in float3 pos, in VertexData vd, in float finalHeight, out half noiseSamp) {

	float4 noiseScale = _noiseScale.xzyw * 0.01;
	float4 noiseOffset = _Time.x * float4(_noiseSpeed.xzy, 0);

	noiseSamp = tex3Dlod(_NoiseTex, float4(pos.xzy, 0) * noiseScale + noiseOffset).r * vd.dhfParams.w;
	//noiseSamp = 1.0;

	//noiseSamp = saturate(noiseSamp - 0.1);
	//apply main wind dir and wind strength from the param map
	float3 windAdd = windDir * noiseSamp;

	

#ifndef LOWER_QUALITY
	noiseScale = _noiseScale2.xzyw * 0.01;
	noiseOffset = _Time.x * float4(_noiseSpeed2.xzy, 0);
	half noiseSamp2 = tex3Dlod(_NoiseTex, float4(pos.zyx, 0) * noiseScale + noiseOffset).r - 0.5;
	//half noiseSamp2 = 0.0;

	windAdd += lerp(Float3(0.0), windDir2 * noiseSamp2, noiseSamp);


	//float flutter = cos(noiseSamp * 100) * windFlutter * saturate(noiseSamp2);
	//windAdd += windDir2 * flutter;
#endif

	//dampen wind effet on flattened grass
	windAdd *= 1.0 - vd.dhfParams.z * 0.9;

#ifndef LOWER_QUALITY
	//increase wind effects on taller grass
	windAdd *= pow(finalHeight, 0.5);
#endif

	return windAdd;
}

inline float3 GetRippleForce(in float3 pos, float noise) {
	uint ripCount = rippleCount[0].val.x; // get current ripple count
	RippleData rip;

	float totalStrength;
	float3 forceDir = 0.001;


	for (uint i = 0; i < ripCount; i++) {
		rip = rippleBuffer[i];
		//float3 toPos = pos - mul(worldToGrass, float4(rip.pos.xyz, 1.0));
		float3 toPos = pos - rip.pos.xyz;
		float localStrength = rip.pos.w * (1.0 - saturate(dot(toPos, toPos) * rip.drssParams.z));
		totalStrength += localStrength;

		forceDir += toPos * localStrength;
	}

	for (int r = 0; r < forcesCount; r++) {
		rip = forcesBuffer[r];
		//float3 toPos = pos - mul(worldToGrass, float4(rip.pos.xyz, 1.0));
		float3 toPos = pos - rip.pos.xyz;
		float localStrength = rip.drssParams.w * (1.0 - saturate(dot(toPos, toPos) * rip.drssParams.z));
		totalStrength += localStrength;

		forceDir += toPos * localStrength;
	}

	return normalize(forceDir) * (saturate(totalStrength) + 0.25 * noise);
}


#if !defined(DEFERRED)
inline float3 GetMainLighting(float3 worldPos, float3 worldNormal){

	half o;

	#if defined(FORWARD_ADD)
	float4 lightDir = GET_LIGHT_DIR(o);
	float3 lightVector = lightDir.xyz - worldPos * lightDir.w;
    float distanceSqr = max(dot(lightVector, lightVector), 0);
    half3 lightDirection = half3(lightVector * rsqrt(distanceSqr));
	#else
	half3 lightDirection = GET_LIGHT_DIR(o);
	#endif

	//half lightAmnt = saturate(1.0 - lightDirection.y);
	half shade = ambientCO + diffuseCO * saturate(dot(lightDirection, worldNormal) + 0.25);
	shade *= 1 + saturate(dot(lightDirection, cross(cross(lightDirection, float3(0, -edgeLightSharp, 0)), worldNormal)) - (edgeLightSharp - edgeLight));
	//shade = saturate(dot(camFwd, lightDirection));
	//shade *= saturate(0.5 + lightDirection.y);

	float3 lighting = GET_LIGHT_COL(o) * shade;

	#if !defined(FORWARD_ADD)
	lighting += max(0, GET_GI(float4(worldNormal, 1)) * ambientCO);
	#endif

	#if defined(SRP)
		//URP LIGHTING
		lighting += GetURPLighting(worldPos, worldNormal);
	#endif

	return lighting;
}
#endif