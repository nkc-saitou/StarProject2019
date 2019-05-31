
Shader "Unlit/Unlit_Noise"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Noise("NoiseX", Range(0, 1)) = 0
		_Offset("Offset", Vector) = (0, 0, 0, 0)
		_RGBNoise("RGBNoise", Range(0, 1)) = 0
		_Intencity("Intencity", Float) = 1
		_SinNoiseWidth("SinNoiseWidth", Float) = 1
		_SinNoiseScale("SinNoiseScale", Float) = 1
		_SinNoiseOffset("SinNoiseOffset", Float) = 1
		_ScanLineTail("Tail", Float) = 0.5
		_ScanLineSpeed("TailSpeed", Float) = 100
		_Alpha("Alpha", Float) = 1
	}

		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		    LOD 100

		    ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appData {
						  float4 vertex : POSITION;
						  float2 texcoord : TEXCOORD0;
					 };

					 struct v2f {
						 float4 vertex : SV_POSITION;
						 half2 texcoord : TEXCOORD0;
						 UNITY_FOG_COORDS(1)
					 };

					 sampler2D _MainTex;
					 float4 _MainTex_ST;
					 float _NoiseX;
					 float2 _Offset;
					 float _RGBNoise;
					 float _Intencity;
					 float _SinNoiseWidth;
					 float _SinNoiseScale;
					 float _SinNoiseOffset;
					 float _ScanLineTail;
					 float _ScanLineSpeed;
					 float _Alpha;

					 v2f vert(appData v) {
						 v2f o;
						 o.vertex = UnityObjectToClipPos(v.vertex);
						 o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						 UNITY_TRANSFER_FOG(o, o.vertex);
						 o.texcoord = v.texcoord;
						 return o;
					 }

					 float rand(float2 co) {
						 return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
					 }

					 float2 mod(float2 a, float b) {
						 return a - floor(a / b) * b;
					 }

					 fixed4 frag(v2f i) : SV_Target
					 {
						 float2 inUV = i.texcoord;
						 float2 uv = i.texcoord - 0.5;

						 // UV座標を再計算し、画面を歪ませる
						 float vignet = length(uv);
						 uv /= 1 - vignet * 0.2;
						 float2 texUV = uv + 0.5;

						 //// 画面外なら描画しない
						 //if (max(abs(uv.y) - 0.5, abs(uv.x) - 0.5) > 0) {
							// return float4(0, 0, 0, 1);
							// /*if (_RGBNoise < 0.8f) {
							//	 
							// }*/
						 //}

						 // 色を計算
						 float3 col;

						 // ノイズ、オフセットを適用
						 texUV.x += sin(texUV.y * _SinNoiseWidth + _SinNoiseOffset) * _SinNoiseScale;
						 texUV += _Offset;
						 texUV.x += (rand(floor(texUV.y * 500) + _Time.y) - 0.5) * _NoiseX;
						 texUV = mod(texUV, 1);

						 // 色を取得、RGBを少しずつずらす
						 col.r = tex2D(_MainTex, texUV).r;
						 col.g = tex2D(_MainTex, texUV - float2(0.002, 0)).g;
						 col.b = tex2D(_MainTex, texUV - float2(0.004, 0)).b;

						 // RGBノイズ
						 if (rand((rand(floor(texUV.y * 500) + _Time.y) - 0.5) + _Time.y) < _RGBNoise) {
							 col.r = rand(uv + float2(123 + _Time.y, 0));
							 col.g = rand(uv + float2(123 + _Time.y, 1));
							 col.b = rand(uv + float2(123 + _Time.y, 2));
						 }

						 // ピクセルごとに描画するRGBを決める
						/* float floorX = fmod(inUV.x * _ScreenParams.x / 3, 1);
						 col.r *= floorX > 0.3333;
						 col.g *= floorX < 0.3333 || floorX > 0.6666;
						 col.b *= floorX < 0.6666;*/

						 // スキャンラインを描画
						 float ScanLineColor = sin(_Time.y * 10 + uv.y * 500) / 2 + 0.5;
						 col *= 0.5 + clamp(ScanLineColor + 0.5, 0, 1) * 0.5;

						 // スキャンラインの残像を描画
						 float tail = clamp((frac(uv.y + _Time.y * _ScanLineSpeed) - 1 + _ScanLineTail) / min(_ScanLineTail, 1), 0, 1);
						 col *= tail;

						 // 画面端を暗くする
						 col *= 1 - vignet * 1.3;

						 // 明るさ補正
						 col = saturate(col * _Intencity);

						 UNITY_APPLY_FOG(i.fogCoord, col);

						 return float4(col, _Alpha);
					 }
				ENDCG
			}
		}
}
