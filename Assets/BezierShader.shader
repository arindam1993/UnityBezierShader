Shader "Unlit/BezierShader"
{
	Properties
	{
		_MainColor ("MainColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
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
			
			data vert (data v)
			{
				data o;
				//USe the model-view-projection matrix to calculate screen space positions of the vertices
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				
				
				return o;
			}
			
			//Result from vertex program, is passed to fragment program
			float4 frag (data i) : SV_Target
			{
				float4 col = _MainColor;
				float2 p = i.uv;
				
				//The condition we described earlier
				float val = p.y*p.y - p.x;
				
				//Do not render pixels outside
				if (val > 0.05) discard;
				//Some free anti-aliasing using an alpha of 0.5 of the edges
				if( val >0 && val <= 0.05) col.a = 0.5;
				
				//Point is inside return the main color.
			  	return col; 
				//return float4(p.x,p.y,0,1); 
			}
			ENDCG
		}
	}
}
