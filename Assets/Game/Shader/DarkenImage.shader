Shader "Custom/DarkenImage" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _BackgroundTex ("Background", 2D) = "white" {}
 _Fade ("Fade", float) = 1
}
 
SubShader {
 
	ZTest Always Cull Off ZWrite Off Fog { Mode Off } //Rendering settings
 
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc" 
		//we include "UnityCG.cginc" to use the appdata_img struct
    
		struct v2f {
			float4 pos : POSITION;
			half2 uv : TEXCOORD0;
		};
   
		//Our Vertex Shader 
		v2f vert (appdata_img v){
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
			return o; 
		}
    
		sampler2D _MainTex; //Reference in Pass is necessary to let us use this variable in shaders
		sampler2D _BackgroundTex;
		float _Fade;
    
		//Our Fragment Shader
		fixed4 frag (v2f i) : COLOR{
		fixed4 c1 = tex2D(_MainTex, i.uv)*_Fade; //Get the orginal rendered color 
		fixed4 c2 = tex2D(_BackgroundTex, i.uv)*(1-_Fade); //Get the backgound rendered color 
     
		//Make changes on the color
		fixed4 col = c1+c2;
     
		return col;
	}
	ENDCG
 }
} 
 FallBack "Diffuse"
}