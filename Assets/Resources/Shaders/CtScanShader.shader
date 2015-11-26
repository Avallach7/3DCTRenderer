Shader "Custom/CtScanShader" 
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,0.5)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"                = "Transparent"
			"RenderType"           = "Transparent"
			"IgnoreProjector"      = "True"
			"ForceNoShadowCasting" = "True"
		}
		LOD 100

		Pass
		{
			ZWrite Off // On causes horrible artifacts
			Blend SrcAlpha OneMinusSrcAlpha
			//AlphaToMask True // True makes mesh flat and very transparent
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
		
				struct appdata_t 
				{
					float4 vertex : POSITION;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					return o;
				}

				fixed4 _Color;

				fixed4 frag(v2f i) : COLOR
				{
					return _Color;
				}
			ENDCG
		}
	}
}
