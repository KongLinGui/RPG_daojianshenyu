// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "RPG/offline+1" {   
    Properties {   
        _Color ("Main Color", Color) = (1,1,1,1)   
        _Texstr("Texturestrength",float)=1
        _MainTex ("MainTexture (RGB)", 2D) = "white" {}  
        _outcolor("_outcolor", Color) = (1,1,1,1)   
        _amount("_amount",float)=0.2 
        _bgcolor("_bgcolor", Color) = (1,1,1,1)   
        _AddColor("_AddColor", Color) = (0,0,0,1)  
        _AddStr("_AddStr",float)=0 
        //_TextTex("_TextTex (RGB)", 2D) = "black" {}   
    }   
    SubShader    
    {     
        Tags { "Queue" = "Transparent"  }   //"LightMode" = "ForwardBase" "LightMode" = "ForwardAdd"
        //Blend SrcAlpha OneMinusSrcAlpha 
        LOD 4000    
        
        // First pass does reflection cubemap   
        Pass    
        {     
        	//Blend SrcAlpha OneMinusSrcAlpha
        	Blend One One
        	cull front
        	ZWrite off
        	//offset -100,-100
        	//Blend one one
        	ZTest Greater   //ZTest (Less | Greater | LEqual | GEqual | Equal | NotEqual | Always) 
        	Lighting Off
			CGPROGRAM  
			#pragma vertex vert  
			#pragma fragment frag  
			#include "UnityCG.cginc"    
			
			struct v2f {   
				float4 pos : SV_POSITION;   
				float2 uv : TEXCOORD0;    
				float3 normal :TEXCOORD1;
			};   
			
			uniform float4 _MainTex_ST;   
			float4  _ObjectSpaceLightPos0;  
			float _amount;
			v2f vert(appdata_tan v)   
			{   
				v2f o;   
				float4 v2 = v.vertex; 
				o.pos = UnityObjectToClipPos (v2);  
				o.normal = mul((float3x3)unity_ObjectToWorld,v.normal );
				return o;    
			}    
			float4 _bgcolor;
			half4 frag (v2f i) : COLOR   
			{    
				half4 result;
				result = _bgcolor; 
				return result;
				
			}    
			ENDCG   
        }   
        Pass    
        {    
        	Blend SrcAlpha OneMinusSrcAlpha
        	//Blend SrcAlpha One
        	cull off
        	ZWrite off
        	ZTest LEqual   
			CGPROGRAM  
			#pragma vertex vert  
			#pragma fragment frag  
			#include "UnityCG.cginc"   
			
			struct v2f {   
				float4 pos : SV_POSITION;    
				float3 normal :TEXCOORD1;
			};   
			    
			float _amount; 
			v2f vert(appdata_tan v)   
			{   
				v2f o;   
				float4 v2 = v.vertex;
				v2.xyz +=v.normal*_amount;
				o.pos = UnityObjectToClipPos (v2);  
				//o.normal = mul(_Object2World,v.normal );
				//o.pos.xyz +=o.normal*_amount;
				return o;    
			}    
			float4 _outcolor;
			half4 frag (v2f i) : COLOR   
			{   
			
				half4 result;
				result = _outcolor; 
				return result;
				
			}    
			ENDCG   
        }   
        Pass    
        {    
        	Tags {"LightMode" = "ForwardBase"}//
        	Cull Off
        	ZWrite On 
        	ZTest LEqual  //ZTest (Less | Greater | LEqual | GEqual | Equal | NotEqual | Always)
        	//AlphaTest Greater 0.5
        	//AlphaToMask True
        	//Lighting On
        	//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM  
			#pragma target 3.0
			#pragma vertex vert  
			#pragma fragment frag  
			#include "UnityCG.cginc"   
			#include "Lighting.cginc"
			struct v2f {   
				float4 pos : SV_POSITION;   
				float2 uv : TEXCOORD0;    
				float3 normal :TEXCOORD1;
			};   
			
			uniform float4 _MainTex_ST;      
			v2f vert(appdata_tan v)   
			{   
				v2f o;   
				float4 v2 = v.vertex;
				 
				o.pos = UnityObjectToClipPos (v2); 
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex); 
				o.normal = mul((float3x3)unity_ObjectToWorld,v.normal );
				return o;    
			}   
			
			sampler2D _MainTex;  
			float _Texstr;  
			float4 _Color; 
			float4 _AddColor;
			float _AddStr;
			half4 frag (v2f i) : COLOR   
			{   
				
				half4 result;
				half4 texcol = tex2D (_MainTex, i.uv); 
				result = texcol; 
				result.rgb *=_Texstr; 
				result *= _Color;
				//result.rgb += _AddColor.rgb * _AddStr;
				result.rgb = lerp(result.rgb,_AddColor.rgb, _AddStr);
				clip(result.a - 0.5);
				return result; 
			}    
			ENDCG   
        }   
    }   
}  