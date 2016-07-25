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

			struct data
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _MainColor;
			
			data vert (data v)
			{
				data o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float4 frag (data i) : SV_Target
			{
				float4 col = _MainColor;
				float2 p = i.uv;
				
				float val = p.y*p.y - p.x;
				
				if (val > 0) discard;
			  	return col; 
				//return float4(p.x,p.y,0,1); 
			}
			ENDCG
		}
	}
}
