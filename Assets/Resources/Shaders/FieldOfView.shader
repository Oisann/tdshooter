// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Oisann/FieldOfView" {
	Properties {
		_fow("FOW",2D) = "black"{}
		_ifow("Inside FOW",2D) = "black"{}
		_SourceTex("Normal Vision",2D) = "black"{}
		_FlipImage("flipImage", Float) = 1
		_FogColor("Fog Color", Color) = (0,0,0,1)
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}
		Pass {
			CGPROGRAM
			sampler2D _fow;
			sampler2D _ifow;
			float4 _FogColor;

			//<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
			float2 _fow_TexelSize;
			float2 _ifow_TexelSize;
			float _FlipImage;

			#pragma vertex vert
			#pragma fragment frag
			#pragma only_renderers d3d11 glcore gles gles3 metal // d3d11_9x
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;

				//Despite the fact that we are only drawing a quad to the screen, Unity requires us to multiply vertices by our MVP matrix, presumably to keep things working when inexperienced people try copying code from other shaders.
				o.pos = UnityObjectToClipPos(v.vertex);

				//Also, we need to fix the UVs to match our screen space coordinates. There is a Unity define for this that should normally be used.
				o.uvs = o.pos.xy / 2 + 0.5;

				return o;
			}

			half frag(v2f i) : COLOR {
				#if UNITY_UV_STARTS_AT_TOP
					i.uvs.y = 1 - i.uvs.y;
				#endif
				return 1;
			}

			ENDCG
		}

		GrabPass {
			
		}

		Pass {
			CGPROGRAM
			sampler2D _fow;
			sampler2D _ifow;
			sampler2D _SourceTex;
			fixed4 _FogColor;
			float _FlipImage;

			//we need to declare a sampler2D by the name of "_GrabTexture" that Unity can write to during GrabPass{}
			sampler2D _GrabTexture;

			//<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
			float2 _GrabTexture_TexelSize;

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;

				//Despite the fact that we are only drawing a quad to the screen, Unity requires us to multiply vertices by our MVP matrix, presumably to keep things working when inexperienced people try copying code from other shaders.
				o.pos = UnityObjectToClipPos(v.vertex);

				//Also, we need to fix the UVs to match our screen space coordinates. There is a Unity define for this that should normally be used.
				o.uvs = o.pos.xy / 2 + 0.5;

				return o;
			}

			half4 frag(v2f i) : COLOR {
				//split texel size into smaller words
				float TX_y = _GrabTexture_TexelSize.y;

				if(_FlipImage > 0)
					i.uvs.y = 1 - i.uvs.y;

				//if something already exists underneath the fragment (in the original texture), discard the fragment.
				v2f temp = i;
				temp.uvs.y = 1 - i.uvs.y;
				if(tex2D(_fow, temp.uvs.xy).r > 0)
					return tex2D(_ifow, float2(i.uvs.x, 1 - i.uvs.y));

				fixed4 col = tex2D(_SourceTex, float2(i.uvs.x, 1 - i.uvs.y));
				col = col * _FogColor;
				return col;
			}
			ENDCG
		}
	}
}