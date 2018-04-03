// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlanetShader2" {
	Properties {
		_MainTex ("MainTex (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _AtmosphereColor("Atmosphere Color", Color) = (0,0,1,1)
        _AtmosphereRange("Atmosphere Value", Range(0,1)) = 0.5
	}
	Category{
	Tags { "Queue" = "Transparent"}
	SubShader {
	Blend SrcAlpha One
	Pass{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma vertex vert
        #pragma fragment frag Standard fullforwardshadows
        #include "UnityCG.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct appdata_t {
            float4 vertex : POSITION;
            float4 color : COLOR;
            float2 texcoord: TEXCOORD0;
            float3 normal : NORMAL0;
        };

        struct v2f {   
            float4 vertex : POSITION;  
            float4 color : COLOR;
            float2 texcoord: TEXCOORD0;
            float3 normal : TEXCOORD1;
            float3 view : TEXCOORD2;
        };

        float4 _MainTex_ST;
        float4 _Color;
        float4 _AtmosphereColor;
        float _AtmosphereRange;

        v2f vert (appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.color = _Color;
            o.texcoord = v.texcoord;
            o.view = normalize(ObjSpaceViewDir(v.vertex));
            o.normal = v.normal;
           
            return o;
        }
           
        sampler2D _MainTex;

		half4 frag(v2f i) :COLOR0
        {
            float4 result = i.color * tex2D(_MainTex, i.texcoord);
            float d = abs(dot(i.view, i.normal));
            float alpha = 1;
            if (d>_AtmosphereRange){
            	alpha = pow(d,4);///_AtmosphereRange;
            	result = _AtmosphereColor;
            }
            result.a *= alpha;
            return result;
        }
		ENDCG
	}
	}
}
}
