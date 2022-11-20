
#if defined (MESH_BUFFER)
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles

float3x4 GetSurfaceRotationMatrix(float3 t, float3 normal, float3 s, float rng){

	float3 h = normalize(float3(0, 1, 0) + normal);
	float4 r = float4(cross(float3(0, 1, 0), h), dot(float3(0, 1, 0), h));

    r.y = rng * 2 - 1;

    r = normalize(r);

	float3x4 res;
	res._m00 = (1.0 - 2.0 * (r.y * r.y + r.z * r.z)) * s.x;
    res._m10 = (r.x * r.y + r.z * r.w) * s.x * 2.0;
    res._m20 = (r.x * r.z - r.y * r.w) * s.x * 2.0;
    res._m01 = (r.x * r.y - r.z * r.w) * s.y * 2.0;
    res._m11 = (1.0 - 2.0 * (r.x * r.x + r.z * r.z)) * s.y;
    res._m21 = (r.y * r.z + r.x * r.w) * s.y * 2.0;
    res._m02 = (r.x * r.z + r.y * r.w) * s.z * 2.0;
    res._m12 = (r.y * r.z - r.x * r.w) * s.z * 2.0;
    res._m22 = (1.0 - 2.0 * (r.x * r.x + r.y * r.y)) * s.z;
    res._m03 = t.x;
    res._m13 = t.y;
    res._m23 = t.z;

    return res;
}



#endif
