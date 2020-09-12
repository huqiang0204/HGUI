Shader "Custom/UIDef"
{
	Properties
	{
		 [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_STex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_TTex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_FTex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_Color("Tint", Color) = (1,1,1,1)
		 [PerRendererData]_Rect("_ClipRect",Vector) = (0,0,1,1)
		 [PerRendererData]_FillColor("Fill color", Vector) = (0,0,0,0)
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent+100"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Off //[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				Name "Default"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex  : POSITION;
					float4 color  : COLOR;
					float2 uv : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float2 uv2 : TEXCOORD2;
					float2 uv3 : TEXCOORD3;
					float2 uv4 : TEXCOORD4;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv  : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float2 uv2 : TEXCOORD2;
					float2 uv3 : TEXCOORD3;
					float2 uv4 : TEXCOORD4;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.uv = IN.uv;
					OUT.uv1 = IN.uv1;
					OUT.uv2 = IN.uv2;
					OUT.uv3 = IN.uv3;
					OUT.uv4 = IN.uv4;
					OUT.color = IN.color;
					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _STex;
				sampler2D _TTex;
				sampler2D _FTex;
				float4 _Rect;
				float4 _Color;
				float4 _FillColor;
				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color;
			     	float2 uv = IN.uv;
					uv.x *= IN.uv4.x;
					uv.y *= IN.uv4.y;
					uv.x += IN.uv3.x;
					uv.y += IN.uv3.y;
					if (IN.uv1.x == 0)
					{
						if (IN.uv1.y == 0)
						{
							color = tex2D(_MainTex, uv);
							if(_FillColor.x==0)
								color *= IN.color;
							else { 
								color.xyz = IN.color.xyz;
								color.a *= IN.color.a;
							}
						}
						else
						{ 
							color = tex2D(_STex, uv); 
							if (_FillColor.y == 0)
								color *= IN.color;
							else {
								color.xyz = IN.color.xyz;
								color.a *= IN.color.a;
							}
						}
					}
                   else
                   {
						if (IN.uv1.y == 0)
						{
							color = tex2D(_TTex, uv);
							if (_FillColor.z == 0)
								color *= IN.color;
							else {
								color.xyz = IN.color.xyz;
								color.a *= IN.color.a;
							}
						}
						else
						{ 
							color = tex2D(_FTex, uv); 
							if (_FillColor.w == 0)
								color *= IN.color;
							else {
								color.xyz = IN.color.xyz;
								color.a *= IN.color.a;
							}
						}
                   }
					if (IN.uv2.x < _Rect.x || IN.uv2.x > _Rect.z || IN.uv2.y < _Rect.y || IN.uv2.y > _Rect.w)
						color.a = 0;
					return color;
				}
			ENDCG
			}
		}
}