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
			"Queue" = "Transparent" //"AlphaTest" 
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"ForceNoShadowCasting" = "True"
		}
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "Always" }
			ZWrite Off //?
			AlphaToMask True
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

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
