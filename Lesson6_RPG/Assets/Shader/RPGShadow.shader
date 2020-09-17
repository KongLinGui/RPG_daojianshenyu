// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "RPG/Shdow" {
Properties {   
        _Color ("Main Color", Color) = (1,1,1,1)    
        _MainTex ("MainTexture (RGB)", 2D) = "white" {}   
    }   
    SubShader    
    {     
        Tags { "RenderType"="Transparent" "Queue" = "Transparent+1" "IgnoreProjector"="True" "DisableBatching"="True" }   //"LightMode" = "ForwardBase" "LightMode" = "ForwardAdd"
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 150    
        ZWrite off
        //cull off
        Pass    
        {    
        	//Tags {"LightMode" = "Vertex" }// 
			CGPROGRAM   
			#pragma vertex vert  
			#pragma fragment frag  
			#include "UnityCG.cginc"   
			//#include "Lighting.cginc" 
			struct v2f {   
				float4 pos : SV_POSITION;   
				float2 uv : TEXCOORD0;   
			};   
			
			uniform float4 _MainTex_ST;      
			v2f vert(appdata_tan v)   
			{   
				v2f o;   
				float4 v2 = v.vertex; 
				o.pos = UnityObjectToClipPos (v2);  
				o.uv = v.texcoord; 
				return o;    
			}   
			
			sampler2D _MainTex;  
			float4 _Color;  
			half4 frag (v2f i) : COLOR   
			{   
				half4 result = tex2D (_MainTex, i.uv); 
				result*=_Color;
				//clip(result.a - 0.5);
				return result; 
			}    
			ENDCG   
		}
		
    }   
    Fallback "VertexLit"
}  