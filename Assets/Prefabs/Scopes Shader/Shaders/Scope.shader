Shader "Simon Mercuzot/Scope" {
    Properties{
        _ReticleTex("Reticle Texture", 2D) = "white" {}
        _ReticleColor("Reticle Color", Color) = (1,1,1,2)
        _ReticleScale("Reticle Scale", Range(1, 20)) = 1
        _ReticleDistance("Reticle Distance", Range(0, 100)) = 50
        _ZoomTex("Zoom Render Texture", 2D) = "white" {}
        _ZoomScale("Zoom Scale", Range(0.1,20)) = 1
        _VignetteRadius("Vignette Radius", Range(0, 20)) = 0.25
        _VignetteSmoothness("Vignette Smoothness", Range(0, 10)) = 0.25
        _VignetteAlpha("Vignette Alpha", Range(0,1)) = 0
        _CameraDistance("Use Camera Distance", Range(0,2)) = 0
    }
        SubShader{
              Tags { "RenderType" = "Opaque" "Queue" = "Transparent" "ForceNoShadowCasting" = "True" }
              LOD 200
              Blend SrcAlpha OneMinusSrcAlpha

        //Zoom

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
            };

            sampler2D _ZoomTex;
            float4 _ZoomTex_ST;
            float _ZoomScale;

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 pos : TEXCOORD1;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _ZoomTex);
                o.pos = UnityObjectToViewPos(v.vertex);         //transform vertex into eye space
                o.normal = mul(UNITY_MATRIX_IT_MV, v.normal);   //transform normal into eye space
                o.tangent = mul(UNITY_MATRIX_IT_MV, v.tangent); //transform normal into eye space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float _CameraDistance;
            float _VignetteAlpha;
            fixed _VignetteRadius, _VignetteSmoothness;

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);    //get normal of this fragment
                float3 tangent = normalize(i.tangent);  //get tangent
                float3 cameraDir = normalize(i.pos);    //direction to eye space origin, normalize(i.pos - float3(0, 0, 0))

                float3 offset = cameraDir + normal;     //normal is facing towards camera, cameraDir - -normal

                float3x3 mat = float3x3(
                    tangent,
                    cross(normal, tangent),
                    normal
                    );


                offset = mul(mat, offset);  //transform offset into tangent space
                float camDist = distance(i.worldPos, _WorldSpaceCameraPos) * 10;

                if (_CameraDistance > 0)
                    offset *= camDist;

                float2 uv = (i.uv + (offset.xy * _ZoomScale));   //sample and scale
                fixed4 col = tex2D(_ZoomTex, uv + float2(0.5, 0.5));

                //Vignette
                fixed aspectRatio = _ScreenParams.x / _ScreenParams.y;
                fixed2 position = (uv);
                position.x *= aspectRatio;
                position.y *= aspectRatio;
                fixed len = length(position) * 2;

                _VignetteRadius /= camDist;
                _VignetteSmoothness /= camDist;

                col.rgb *= lerp(1, 0, smoothstep(_VignetteRadius, _VignetteRadius + _VignetteSmoothness, len));
                col.a = _VignetteAlpha;
                return col;
            }

              ENDCG
        }
        

        //Reticle

        Pass{
              CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                    float3 tangent : TANGENT;
                };

                sampler2D _ReticleTex;
                float4 _ReticleTex_ST;
                fixed4 _ReticleColor;
                float _ReticleScale;
                float _ReticleDistance;
                float _CameraDistance;

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 pos : TEXCOORD1;
                    float3 normal : NORMAL;
                    float3 tangent : TANGENT;
                    float4 worldPos : TEXCOORD2;
                };

                v2f vert(appdata v) {

                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _ReticleTex);
                    o.pos = UnityObjectToViewPos(v.vertex);         //transform vertex into eye space
                    o.normal = mul(UNITY_MATRIX_IT_MV, v.normal);   //transform normal into eye space
                    o.tangent = mul(UNITY_MATRIX_IT_MV, v.tangent); //transform normal into eye space
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {

                    float3 normal = normalize(i.normal);    //get normal of this fragment
                    float3 tangent = normalize(i.tangent);  //get tangent
                    float3 cameraDir = normalize(i.pos);    //direction to eye space origin, normalize(i.pos - float3(0, 0, 0))

                    float3 offset = cameraDir + normal;     //normal is facing towards camera, cameraDir - -normal

                    float3x3 mat = float3x3(
                        tangent,
                        cross(normal, tangent),
                        normal
                    );

                    offset = mul(mat, offset);  //transform offset into tangent space
                    float camDist = distance(i.worldPos, _WorldSpaceCameraPos) * 10;

                    if (_CameraDistance > 0)
                        offset *= camDist;

                    float2 uv = (i.uv + (offset.xy * _ReticleDistance) - float2(0.5, 0.5)) / _ReticleScale;   //sample and scale
                    return tex2D(_ReticleTex, uv + float2(0.5, 0.5)) * _ReticleColor;                              //shift sample to center of texture
                }

                 ENDCG
          }
   
    }
}