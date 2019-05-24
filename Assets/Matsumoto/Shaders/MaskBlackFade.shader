Shader "FadeShader/MaskBlackFade"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MaskTex("MaskTex", 2D) = "white" {}
		_Ratio("Ratio", Range(0, 1)) = 0
		_Size("Size", Range(0, 10)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			fixed _Ratio;
			fixed _Size;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				half x = _ScreenParams.x / _ScreenParams.y;
				half2 uv = i.uv;
				uv -= 0.5;
				uv /= (1 - _Ratio) * _Size;
				uv.x *= x;
				uv += 0.5;
				fixed4 mask = tex2D(_MaskTex, saturate(uv));
				//* (1 - _Ratio)

				return col * mask;
			}
			ENDCG
		}
	}
}
