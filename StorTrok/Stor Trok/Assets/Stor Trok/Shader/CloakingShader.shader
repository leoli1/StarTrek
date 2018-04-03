Shader "Custom/CloakingShader" {
	Properties {
		_MainTex("Main Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_P("P", Range(-1,1)) = 0.25
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "Queue" = "Transparent" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True"}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha:fade nolighting

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input{
			float2 uv_tex;
			float3 norm;
			float3 view_dir;
		};
		sampler2D _MainTex;
		fixed4 _Color;
		float _P;

		void surf (Input IN, inout SurfaceOutput o){
			float4 c = tex2D(_MainTex, IN.uv_tex);//*_Color;
			o.Albedo = c.rgb;
			float border = 1-(abs(dot(IN.view_dir, IN.norm)));
			float alpha = (border*(1-_P) +_P);
			o.Alpha = c.a*alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
