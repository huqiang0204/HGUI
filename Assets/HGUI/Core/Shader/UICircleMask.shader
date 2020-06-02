Shader "Custom/UICircleMask"
{
	Properties
	{
		  [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		  [PerRendererData]_Color("Tint", Color) = (1,1,1,1)
		  [PerRendererData]_Rdius("Rdius", Range(0,0.5)) = 0.5
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

				sampler2D _MainTex;
				float _Rdius;
				fixed4 _Color;
				float4 _Rect;
				float4 _FillColor;
				v2f vert(appdata_t IN)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.uv = IN.uv;
					OUT.uv = IN.uv;
					OUT.uv1 = IN.uv1;
					OUT.uv2 = IN.uv2;
					OUT.color = IN.color;
					return OUT;
				}
				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color = tex2D(_MainTex, IN.uv);
				    float x=IN.uv.x-0.5;
					float y=IN.uv.y-0.5;
					if (_Rdius * _Rdius < x * x + y * y)
						color.a = 0;
					if (IN.uv2.x < _Rect.x || IN.uv2.x > _Rect.z || IN.uv2.y < _Rect.y || IN.uv2.y > _Rect.w)
						color.a = 0;
					return color;
				}
			ENDCG
			}
		}
}
