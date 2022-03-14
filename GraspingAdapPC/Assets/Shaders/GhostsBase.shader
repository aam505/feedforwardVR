Shader "Custom/GhostsBase"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

	// https://docs.unity3d.com/Manual/SL-CullAndDepth.html
	SubShader
	{
		Tags {"Queue" = "Geometry+1" "RenderType" = "Opaque"}

		/*
		Pass {
			ZWrite On
			ColorMask 0
		}
		*/

		Pass {
			ZWrite On
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = float4(v.texcoord.xy, 0, 0);
				return o;
			}

			sampler2D _MainTex;

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				if (color.a < 0.5) {
					discard;
				}
				
				return float4(1,1,1,1);
				
			}
			ENDCG
		}
	}

}
