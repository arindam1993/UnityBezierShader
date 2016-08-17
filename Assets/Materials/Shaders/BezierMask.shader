Shader "Unlit/BezierMask"
{
	Properties
	{
		_MainColor ("MainColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		ColorMask 0
		LOD 100

		Pass
		{
			// Write the value 1 to the stencil buffer
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
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
				col.a = 0;
				float2 p = i.uv;
				
				//The condition we described earlier
				float val = p.y*p.y - p.x;
				
				//Do not render pixels outside
				if (val < 0) discard;
				
				//Point is inside return the main color.
			  	return col; 
			
			}
			ENDCG
		}
	}
}
