










#if defined(GRASS_TESSELATION)

	v2g TessVertex (v2g v, uint instID : SV_InstanceID) {

		v2g p;
		p.vertex = v.vertex;
		p.vertex.w = instID;

		#if defined(RENDERMODE_MESH)
			p.norm = v.norm;
			p.uv = v.uv;
		#else
			//terrain
			p.uv = _chunkPos;
		#endif

		return p;
	}
	
	inline float TessellationEdgeFactor () {
		return 1;
	}
	
	
	TessFactors GrassPatchConstant (InputPatch<v2g, 3> patch, uint pID : SV_PrimitiveID) {
	
		//TODO: i've realized i glossed over that you can have difference factors for the edges
		//it might be possible to get more efficient tesselations for bendable grass if you do async edges
	
		TessFactors f;
	    f.edge[0] = TessellationEdgeFactor();
	    f.edge[1] = TessellationEdgeFactor();
	    f.edge[2] = TessellationEdgeFactor();
	    f.edge[3] = TessellationEdgeFactor();
		f.inside[0] = TessellationEdgeFactor();
		f.inside[1] = TessellationEdgeFactor();
	
		float instID = patch[0].vertex.w;
	
		CreateBladeFromSource(pID, instID, patch, f);
	
		return f;
	}
	
	[domain("quad")]
	[outputcontrolpoints(3)]
	[outputtopology("triangle_cw")]
	[partitioning("integer")]
	[patchconstantfunc("GrassPatchConstant")]
	v2g GrassHull (InputPatch<v2g, 3> patch, uint id : SV_OutputControlPointID) {
		return patch[id];
	}
	
	
	[domain("quad")]
	g2f GrassDomain (TessFactors f, OutputPatch<v2g, 4> patch, float2 UV : SV_DomainLocation) {
	
		g2f o;
		o.uv = 1;
	
	
		float3 widthMod = f.widthMod * lerp(1, bladeSharp, UV.y);
		widthMod *= (UV.x * 2 - 1);
	
		float3 wPos = lerp(f.bV, f.tV, UV.y);
		wPos += widthMod;
	
		#if !defined(SHADOW_CASTER)
		//color nonsense
		o.col = f.bladeCol;
		#endif
	
		SET_WORLDPOS(o, float4(wPos, 1));
		o.pos = GetClipPos(wPos, o.pos);
		SET_UV(float3(UV, o.pos.z));
		TRANSFER_GRASS_SHADOW(o, wPos);
		return o;
	}

#elif defined(SURFACE_TESSELATION)

	v2g TessVertex (v2g v, uint instID : SV_InstanceID) {

		v2g p;		

		#if defined(RENDERMODE_MESH)
			p.norm = v.norm;
			p.uv = v.uv;
			p.vertex = mul(grassToWorld, v.vertex);
		#else

			//terrain uv based on vertex pos since its just a plane
			p.uv = v.vertex.xz;
			p.terrainPos = float4(GetHeightmapSimple(p.uv), 1);
			p.vertex = mul(grassToWorld, p.terrainPos);
		#endif
		
		
		p.dhfParams = tex2Dlod(dhfParamMap, float4(p.uv.xy, 0, 0));

		p.clipPos = GetClipPos(p.vertex.xyz, p.clipPos);
		p.clipPos /= p.clipPos.w;

		#if defined(SHADER_API_METAL)
		//use for rng since primitive ID is borked on metal
		p.clipPos.w = tex2Dlod(randoMap, float4(v.vertex.xz, 0, 0)).rgbg * 1000.213564;
		#endif

		p.vertex = v.vertex;
		p.vertex.w = instID;

		return p;
	}


	#if defined (SHADOW_CASTER)
	
		#define CAM_POS UNITY_MATRIX_I_V._m03_m13_m23
		#define isShadow any(abs(CAM_POS - _WorldSpaceCameraPos) > 0.15)
	
		#define TESS_FACTOR (isShadow ? tessAmntShadow : tessAmnt)
	#else
		#define TESS_FACTOR tessAmnt
	#endif

	inline float TessellationEdgeFactor (float viewDist) {

		return TESS_FACTOR * viewDist;
	}


	#define aThird (1.0 / 3.0)

	TessFactors GrassPatchConstant (InputPatch<v2g, 3> patch) {

		TessFactors f;

		//its so dumb that i have to do this but for some reason i cant condtinoally cull on metal
		//but it works fine if its not conditional and just return the empty tessfactors at the end
		float factor = 1;
		float minTess = tessMin;

		#if defined(SHADER_API_METAL)
		#define CullTri(value)\
			minTess = value;\
			factor = value;\
			f.edge[0] = value;\
			f.edge[1] = value;\
			f.edge[2] = value;\
			f.inside =  value;\
			f.tessDist =  value
		#else
		#define CullTri(value)\
			f.edge[0] = value;\
			f.edge[1] = value;\
			f.edge[2] = value;\
			f.inside =  value;\
			f.tessDist =  value;\
			return f;
		#endif
		

		#if defined(RENDERMODE_MESH)
		float3 edgeCenter = (patch[0].vertex + patch[1].vertex + patch[2].vertex ) * aThird;
		#else
		float3 edgeCenter = (patch[0].terrainPos + patch[1].terrainPos + patch[2].terrainPos ) * aThird;
		#endif

		float3 dir = mul(grassToWorld, float4(edgeCenter, 1)) - _WorldSpaceCameraPos;
		float camDist = rsqrt(dot(dir, dir));
		float distanceFade = saturate(pow(camDist * grassFade, grassFadeSharpness));

		if(distanceFade <= 0.0005 || camDist > tessClose){
			CullTri(0);
		}
				

		

		#ifdef RENDERMODE_MESH
		#define HandleNorm(vd) vd.norm = patch[0].norm
		#else
		#define HandleNorm(vd) vd.norm = float3(0, 1, 0)
		#endif


		float4 p0 = patch[0].clipPos;
		float4 p1 = patch[1].clipPos;
		float4 p2 = patch[2].clipPos;

		float4 dhf0 = patch[0].dhfParams;
		float4 dhf1 = patch[1].dhfParams;
		float4 dhf2 = patch[2].dhfParams;

		float2 uv0 = patch[0].uv;
		float2 uv1 = patch[1].uv;
		float2 uv2 = patch[2].uv;


		VertexData cVD;
		cVD.vertex = edgeCenter;
		cVD.color = 0;
		cVD.typeParams = 0;
		HandleNorm(cVD);

		#if defined(SHADER_API_METAL)
		//for whatever reason sampling in tessellation on metal is super broken
		//dont want it to count for density since it cant change, but average the heights for culling purposes
		cVD.dhfParams = float4(0, (dhf0.y + dhf1.y + dhf2.y) * aThird, 0, 0);
		#else
		float2 uvC = (uv0 + uv1 + uv2) * aThird;
		cVD.dhfParams = tex2Dlod(dhfParamMap, float4(uvC, 0, 0));
		cVD.dhfParams.z = saturate(1.0 - cVD.dhfParams.z) * 0.75;
		#endif

		//cull zero density tris
		float4 densities = float4(
			dhf0.x,
			dhf1.x,
			dhf2.x,
			cVD.dhfParams.x
		);
		
		if(all(densities == 0)){
			CullTri(0);
		}


		float finalHeight = cVD.dhfParams.y * bladeHeight * 2;
		float4 tV = float4(TP_Vert(cVD, 0, finalHeight, 1).xyz, 1);
		//float4 tV = float4(cVD.vertex + cVD.norm * finalHeight, 1);
		tV = mul(grassToWorld, tV);
		tV = GetClipPos(tV.xyz, tV);
		tV /= tV.w;
		
		float viewDistance = saturate(pow(camDist * tessRange, tessPow));
		f.tessDist = viewDistance;
		factor *= TessellationEdgeFactor(viewDistance);


		//discard triangles outside of field of view
		//in the worst case this seems to not really save performance
		//in the best case it can save 1-2ms on render times
		//soo.. i think its worth doing probably :)

		float4 px = float4(p0.x, p1.x, p2.x, tV.x).xyzw;
		float4 py = float4(p0.y, p1.y, p2.y, tV.y).xyzw;
		float4 pz = float4(p0.z, p1.z, p2.z, tV.z).xyzw;
		
		//this is complete ass and i have no idea if if makes any sense
		//it "seems" to make clip y consistent on openGL vs DX11 type beats
		//but nowhere on the internet even mentions theres a difference in the first place
		//on top of that this seems backward from what you would expect anyway if you were to have to do this
		//so i have no freaking idea
		if (_ProjectionParams.x > 0)
			py = -py;

		#define edgeLimit 1.1
		#define Depth01(depth) (1.0 / (depth * _ZBufferParams.x + _ZBufferParams.y))

		if(
			all(px < -edgeLimit) || all(px > edgeLimit)
			|| all(py < -edgeLimit) //had to remove bottom edge limit because inaccurate/ nbd tho
			|| all(Depth01(pz) < 0)
		){
			CullTri(0);
		}
		

		#define minFactor(value) max(value, minTess)

		//#define CheckFactor(edgeDensites) minFactor(factor * edgeDensites)
		factor = minFactor(factor);
		#define CheckFactor(edgeDensites) factor
		
	    f.edge[0] = CheckFactor((densities.y + densities.z) * 0.5);
	    f.edge[1] = CheckFactor((densities.z + densities.x) * 0.5);
	    f.edge[2] = CheckFactor((densities.x + densities.y) * 0.5);

		f.inside  = CheckFactor((densities.x + densities.y + densities.z) * aThird);
	
		return f;
	}


	[domain("tri")]
	[outputcontrolpoints(3)]
	[outputtopology("triangle_cw")]
	[partitioning("fractional_even")]
	[patchconstantfunc("GrassPatchConstant")]
	v2g GrassHull (InputPatch<v2g, 3> patch, uint id : SV_OutputControlPointID) {
		return patch[id];
	}


	

	[domain("tri")]
	g2f GrassDomain (TessFactors f, OutputPatch<v2g, 3> patch, float3 UV : SV_DomainLocation, uint pID : SV_PrimitiveID) {
		
		VertexData lvd;

		float3 scaledUV = UV;
		//not sure this is actually necessary, it mattered at first but eh
		//basically scales the source tris slightly so they overlap and hide seams
		//i don't think the seams are actually an issue though
		//float3 scaledUV = UV * 1.05 - 0.016;

		lvd.vertex = INTERPOLATE_BC(patch, .vertex, scaledUV);
		//lvd.uv = INTERPOLATE_BC(patch, .uv.xy, UV);

		#if defined(RENDERMODE_MESH)
		lvd.norm = INTERPOLATE_BC(patch, .norm, UV);
		#endif

		
		g2f o;
		o.uv = float4(UV, 0);


		#if !defined(SHADER_API_METAL)
		float primID = pID;
		#else
		//use separate rng seed for metal
		float primID = patch[0].clipPos.w;
		#endif

		float instID = patch[0].vertex.w;
		CreateBladeFromSource(primID, instID, patch, lvd, f, o);

		//o.pos = UnityObjectToClipPos(lvd.vertex);
		return o;
	}
	
#endif