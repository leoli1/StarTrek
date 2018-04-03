// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Animated Refractive Water" {
     Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
         _ReflectColor ("Reflection Color", Color) = (0.5, 0.5, 0.5, 1)
         _UVScroll  ("UV Scroll Speed", float) = 0.5
         _DistAmt  ("Distortion", range (0,128)) = 10
         _Lerp ("Reflection Blend (0 Cubemap, 1 Refraction)", Range (0,1)) = 0.5
         _BriLvl ("Reflection Brightness Level", Range (0.5,3)) = 1.5
         _BumpMap1 ("Normalmap 1", 2D) = "bump" {}
         _BumpMap2 ("Normalmap 2", 2D) = "bump" {}
         _Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
     }
     SubShader {
         GrabPass { }
        
         Tags { "Queue"="Background+11" "IgnoreProjector"="True" "RenderType"="Opaque" }
         LOD 200
        
         CGPROGRAM
         #pragma exclude_renderers gles
         #pragma vertex vert
         #pragma surface surf HalfLambert noambient 
         #include "UnityCG.cginc"
         #pragma target 3.0
  
         float4 _Color, _ReflectColor, _GrabTexture_TexelSize;
         sampler2D _BumpMap1, _BumpMap2, _GrabTexture;
         samplerCUBE _Cube;
         fixed _DistAmt, _UVScroll, _Lerp, _BriLvl;
  
         struct Input {
             float2 uv_BumpMap1;
             float4 proj : TEXCOORD;
             float3 worldRefl;
             INTERNAL_DATA
         };
                
            //Vertex Vert
         void vert (inout appdata_full v, out Input o) {
         UNITY_INITIALIZE_OUTPUT(Input,o);
             float4 oPos = UnityObjectToClipPos(v.vertex);
             #if UNITY_UV_STARTS_AT_TOP
                 float scale = -1.0;
             #else
                 float scale = 1.0;
             #endif
             o.proj.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
             o.proj.zw = oPos.zw;
         }
          
         void surf (Input IN, inout SurfaceOutput o) {
             //Animated Normals
             fixed2 UVOffset1 = IN.uv_BumpMap1;
             fixed2 UVOffset2 = IN.uv_BumpMap1;
             fixed xValue = (_UVScroll/2) * _Time;
             UVOffset1 += fixed (xValue);
             UVOffset2 -= fixed (xValue);
             half3 n1 = UnpackNormal(tex2D(_BumpMap1, UVOffset1));
             half3 n2 = UnpackNormal(tex2D(_BumpMap2, UVOffset2));
             
             //Normals          
             half3 NewNormals = (n1/1.5) + (n2/1.5);
             o.Normal = NewNormals.rgb;
             
             //Refraction Texture Grab
             float2 offset = NewNormals * _DistAmt * _GrabTexture_TexelSize.xy;
             IN.proj.xy = offset * IN.proj.z + IN.proj.xy;
             half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(IN.proj));
                                     
             //Cubemap Reflection
             float3 worldRefl = WorldReflectionVector (IN, o.Normal);
             fixed4 refltex = texCUBE (_Cube, worldRefl);
             fixed4 Emiss = (refltex * _ReflectColor) * (col * _BriLvl);
             
             half3 B = lerp(Emiss.rgb, col.rgb, _Lerp);
             half3 Diff = B.rgb * _Color.rgb;
                         
             //Output
             o.Albedo = Diff.rgb;
         }
         
         //Lighting Model
         half4 LightingHalfLambert (SurfaceOutput s, half3 lightDir, half atten)
                 {
                     half NdotL = dot(s.Normal, lightDir);
                     half diff = NdotL * 0.5 + 0.5;
                     half4 c;
                     
                     c.rgb = s.Albedo * _LightColor0.rgb * ( diff * atten * 2);
                     c.a = s.Alpha;
                     return c;
                 }
         ENDCG
     }
     FallBack "Transparent/Diffuse"
 }