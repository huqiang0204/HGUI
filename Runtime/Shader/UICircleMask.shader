Shader "HGUI/UICircleMask"
{
	Properties
	{
		  _MainTex("Sprite Texture", 2D) = "white" {}
		  [PerRendererData]_Color("Tint", Color) = (1,1,1,1)
		  _Rdius("Rdius", Range(0,0.5)) = 0.5
		_SRect("_SpriteClipRect",Vector) = (0.5,0.5,0.5,0.5)
		 [PerRendererData]_t1("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_t2("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_t3("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_t4("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_t5("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_t6("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_t7("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_Color("Tint", Color) = (1,1,1,1)
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

			Pass
			{
					Cull Off
			Lighting Off
			ZWrite Off
			 //ZTest Off //[unity_GUIZTestMode]
			 Blend SrcAlpha OneMinusSrcAlpha
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

				float _Rdius;
				sampler2D _MainTex;
				sampler2D _t1;
				sampler2D _t2;
				sampler2D _t3;
				sampler2D _t4;
				sampler2D _t5;
				sampler2D _t6;
				sampler2D _t7;
				float4 _SRect;
				float4 _Color;
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
				fixed4 frag(v2f IN) : SV_Target
				{
						half4 color;
					float2 uv = IN.uv;
					uv.x *= IN.uv4.x;
					uv.y *= IN.uv4.y;
					uv.x += IN.uv3.x;
					uv.y += IN.uv3.y;
					if (IN.uv1.x < 0.09)
					{
						color = tex2D(_MainTex, uv);
					}
					else if (IN.uv1.x < 0.19)
					{
						color = tex2D(_t1, uv);
					}
					else  if (IN.uv1.x < 0.29)
					{
						color = tex2D(_t2, uv);
					}
					else  if (IN.uv1.x < 0.39)
					{
						color = tex2D(_t3, uv);
					}
					else if (IN.uv1.x < 0.49)
					{
						color = tex2D(_t4, uv);
					}
					else  if (IN.uv1.x < 0.59)
					{
						color = tex2D(_t5, uv);
					}
					else  if (IN.uv1.x < 0.69)
					{
						color = tex2D(_t6, uv);
					}
					else
					{
						color = tex2D(_t7, uv);
					}
					if (IN.uv1.y < 0.09)
					{
						color.x *= IN.color.x;
						color.y *= IN.color.y;
						color.z *= IN.color.z;
						color.a *= IN.color.a;
					}
					else if (IN.uv1.y < 0.19)
					{
						color.xyz = IN.color.xyz;
						color.a *= IN.color.a;
					}
					else if (IN.uv1.y < 0.29)
					{
						color.a *= IN.color.a;
					}
					else//黑白
					{
						color.rgb = dot(color.rgb, float3(0.22, 0.707, 0.071));
					}
				    float x=IN.uv.x - _SRect.x;
					x /= _SRect.z;
					float y=IN.uv.y - _SRect.y;
					y/= _SRect.w;
					if (_Rdius * _Rdius < x * x + y * y)
						color.a = 0;
					return color;
				}
			ENDCG
			}
		}
}
