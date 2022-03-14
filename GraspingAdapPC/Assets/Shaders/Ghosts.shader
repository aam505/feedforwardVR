Shader "Custom/Ghosts"
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
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
		LOD 200

		/*
		Pass {
			ZWrite On
			ColorMask 0
		}
		*/
		

		/*
		Pass {
			ZWrite On
			//ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 vpos : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vpos = UnityObjectToViewPos(v.vertex);
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{

				return fixed4(i.pos.zzz * 10.0, 1);
			}
			ENDCG
		}
		*/

		/*
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Equal
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{

				return fixed4(1,0,0,0.2);
			}
			ENDCG
		}
		*/
		
		// paste in forward rendering passes from Transparent/Diffuse
		UsePass "Transparent/Diffuse/FORWARD"
     
    }
	Fallback "Transparent/VertexLit"

}
