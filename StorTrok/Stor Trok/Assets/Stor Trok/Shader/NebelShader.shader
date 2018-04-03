// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Dynamic Nebula" {
Properties {
        _MainTex ("MainTex (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
}
 
Category {
 
    Tags { "Queue"="Transparent" }
 
    SubShader
    {      
    Blend SrcAlpha One
    Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "Always" }
                       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
 
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
                result.a *= pow(abs(dot(i.view, i.normal)), 4);
                return result;
            }
            ENDCG
        } // end pass
 
    } // end subshader
 
   
} // end category
 
} // end shader