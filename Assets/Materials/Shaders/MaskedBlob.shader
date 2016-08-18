Shader "Unlit/MaskedBlob"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white"{}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent+1" } 
		
		LOD 100

		Pass
		{
		// Only render pixels whose value in the stencil buffer equals 1.
			Stencil {
			  Ref 1
			  Comp NotEqual
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"
			
			//Input data to the vertex program, vertex positiosn and u,v co-ordinates for each vertex
			struct data
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _MainColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			data vert (data v)
			{
				data o;
				//USe the model-view-projection matrix to calculate screen space positions of the vertices
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				
				return o;
			}
			
			//Result from vertex program, is passed to fragment program
			float4 frag (data i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			
			}
			ENDCG
		}
	}
}
