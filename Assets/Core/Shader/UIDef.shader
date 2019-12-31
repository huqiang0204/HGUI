Shader "Custom/UIDef"
{
	Properties
	{
		 _MainTex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_STex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData]_TTex("Sprite Texture", 2D) = "white" {}
		  [PerRendererData]_FTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[PerRendererData]_ClipRect("_ClipRect",Vector) = (-10000,-10000,10000,10000)
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

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
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 uv : TEXCOORD0;
					float2   uv1 : TEXCOORD1;
					float2   uv2 : TEXCOORD2;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv  : TEXCOORD0;
					float2   uv1 : TEXCOORD1;
					float2 uv2 : TEXCOORD2;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				fixed4 _Color;
				float4 _ClipRect;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.uv = IN.uv;
					OUT.uv1 = IN.uv1;
					OUT.uv2 = IN.uv2;
					OUT.color = IN.color * _Color;
					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _STex;
				sampler2D _TTex;
				sampler2D _FTex;
				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color;
					if (IN.uv1.x == 0)
					{
						if(IN.uv1.y==0)
							color = tex2D(_MainTex, IN.uv);
						else color = tex2D(_STex, IN.uv);
					}
                   else
                   {
						if (IN.uv1.y == 0)
							color = tex2D(_TTex, IN.uv);
						else color = tex2D(_FTex, IN.uv);
                   }
					color *= IN.color;
					if (IN.uv2.x< _ClipRect.x || IN.uv2.x > _ClipRect.z || IN.uv2.y < _ClipRect.y || IN.uv2.y > _ClipRect.w)
						color.a = 0;
					return color;
				}
			ENDCG
			}
		}
}
