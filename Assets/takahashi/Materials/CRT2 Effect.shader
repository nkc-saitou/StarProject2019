// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CRT Effect"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_Float0("Float 0", Float) = 1
		_meido("meido", Float) = 7.1
		_Float8("Float 8", Float) = -0.19
		[Toggle]_ToggleSwitch0("Toggle Switch0", Float) = 0
		_emi("emi", Float) = 2.16
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform float _Float0;
		uniform float _Float8;
		uniform float _ToggleSwitch0;
		uniform float _meido;
		uniform float _emi;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float temp_output_88_0 = ( 1.0 / 3.0 );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 temp_output_129_0 = ( ase_objectScale * _Float0 );
			float2 uv_TexCoord79 = i.uv_texcoord * temp_output_129_0.xy;
			float temp_output_80_0 = frac( uv_TexCoord79.x );
			float temp_output_78_0 = step( ( temp_output_88_0 * 2.0 ) , temp_output_80_0 );
			float4 color101 = IsGammaSpace() ? float4(0,0,1,0) : float4(0,0,1,0);
			float lerpResult93 = lerp( 1.0 , 0.0 , step( temp_output_88_0 , temp_output_80_0 ));
			float4 color102 = IsGammaSpace() ? float4(1,0,0,0) : float4(1,0,0,0);
			float lerpResult106 = lerp( 1.0 , 0.0 , ( temp_output_78_0 + lerpResult93 ));
			float4 color103 = IsGammaSpace() ? float4(0,1,0,0) : float4(0,1,0,0);
			float4 temp_output_109_0 = ( tex2D( _TextureSample1, uv_TextureSample1 ) * ( ( temp_output_78_0 * color101 ) + ( lerpResult93 * color102 ) + ( lerpResult106 * color103 ) ) );
			float2 uv_TexCoord275 = i.uv_texcoord * ( float3( float2( 3,3 ) ,  0.0 ) * temp_output_129_0 ).xy;
			float temp_output_279_0 = frac( uv_TexCoord275.x );
			float4 _Vector5 = float4(0,0,0,1);
			float temp_output_331_0 = (_Vector5.z + (( _Float8 / 2.0 ) - ( -1.0 / 3.0 )) * (_Vector5.w - _Vector5.z) / (_Vector5.y - ( -1.0 / 3.0 )));
			float2 uv_TexCoord358 = i.uv_texcoord * ( float3( float2( 1,1 ) ,  0.0 ) * temp_output_129_0 ).xy;
			float temp_output_359_0 = frac( uv_TexCoord358.y );
			float temp_output_361_0 = ( 1.0 / ( 3.0 * 2.0 ) );
			float temp_output_219_0 = ( 1.0 / 3.0 );
			float4 appendResult217 = (float4(temp_output_219_0 , temp_output_219_0 , 0.0 , 0.0));
			float2 uv_TexCoord231 = i.uv_texcoord * temp_output_129_0.xy + ( -1.0 * appendResult217 ).xy;
			float4 _Vector0 = float4(0,1,-1,1);
			float2 temp_cast_7 = (_Vector0.x).xx;
			float2 temp_cast_8 = (_Vector0.y).xx;
			float2 temp_cast_9 = (_Vector0.z).xx;
			float2 temp_cast_10 = (_Vector0.w).xx;
			float2 _Vector2 = float2(1,3);
			float temp_output_316_0 = ( ( _Vector2.x / _Vector2.y ) + _Float8 );
			float4 appendResult222 = (float4(( temp_output_219_0 * 0.0 ) , temp_output_219_0 , 0.0 , 0.0));
			float2 uv_TexCoord230 = i.uv_texcoord * temp_output_129_0.xy + ( -1.0 * appendResult222 ).xy;
			float2 temp_cast_13 = (_Vector0.x).xx;
			float2 temp_cast_14 = (_Vector0.y).xx;
			float2 temp_cast_15 = (_Vector0.z).xx;
			float2 temp_cast_16 = (_Vector0.w).xx;
			float4 appendResult223 = (float4(( temp_output_219_0 * -1.0 ) , temp_output_219_0 , 0.0 , 0.0));
			float2 uv_TexCoord229 = i.uv_texcoord * temp_output_129_0.xy + ( -1.0 * appendResult223 ).xy;
			float2 temp_cast_19 = (_Vector0.x).xx;
			float2 temp_cast_20 = (_Vector0.y).xx;
			float2 temp_cast_21 = (_Vector0.z).xx;
			float2 temp_cast_22 = (_Vector0.w).xx;
			float2 uv_TexCoord207 = i.uv_texcoord * temp_output_129_0.xy + appendResult217.xy;
			float2 temp_cast_25 = (_Vector0.x).xx;
			float2 temp_cast_26 = (_Vector0.y).xx;
			float2 temp_cast_27 = (_Vector0.z).xx;
			float2 temp_cast_28 = (_Vector0.w).xx;
			float2 uv_TexCoord206 = i.uv_texcoord * temp_output_129_0.xy + appendResult222.xy;
			float2 temp_cast_31 = (_Vector0.x).xx;
			float2 temp_cast_32 = (_Vector0.y).xx;
			float2 temp_cast_33 = (_Vector0.z).xx;
			float2 temp_cast_34 = (_Vector0.w).xx;
			float2 uv_TexCoord163 = i.uv_texcoord * temp_output_129_0.xy + appendResult223.xy;
			float2 temp_cast_37 = (_Vector0.x).xx;
			float2 temp_cast_38 = (_Vector0.y).xx;
			float2 temp_cast_39 = (_Vector0.z).xx;
			float2 temp_cast_40 = (_Vector0.w).xx;
			float2 _Vector6 = float2(0,1);
			float clampResult385 = clamp( ( ( ( step( temp_output_279_0 , temp_output_331_0 ) * step( ( 1.0 - temp_output_279_0 ) , temp_output_331_0 ) ) - ( ( 1.0 - step( temp_output_359_0 , ( temp_output_361_0 * 5.0 ) ) ) + step( temp_output_359_0 , temp_output_361_0 ) ) ) + lerp(0.0,( step( distance( (temp_cast_9 + (frac( uv_TexCoord231 ) - temp_cast_7) * (temp_cast_10 - temp_cast_9) / (temp_cast_8 - temp_cast_7)) , float2( 0,0 ) ) , temp_output_316_0 ) + step( distance( (temp_cast_15 + (frac( uv_TexCoord230 ) - temp_cast_13) * (temp_cast_16 - temp_cast_15) / (temp_cast_14 - temp_cast_13)) , float2( 0,0 ) ) , temp_output_316_0 ) + step( distance( (temp_cast_21 + (frac( uv_TexCoord229 ) - temp_cast_19) * (temp_cast_22 - temp_cast_21) / (temp_cast_20 - temp_cast_19)) , float2( 0,0 ) ) , temp_output_316_0 ) + step( distance( (temp_cast_27 + (frac( uv_TexCoord207 ) - temp_cast_25) * (temp_cast_28 - temp_cast_27) / (temp_cast_26 - temp_cast_25)) , float2( 0,0 ) ) , temp_output_316_0 ) + step( distance( (temp_cast_33 + (frac( uv_TexCoord206 ) - temp_cast_31) * (temp_cast_34 - temp_cast_33) / (temp_cast_32 - temp_cast_31)) , float2( 0,0 ) ) , temp_output_316_0 ) + step( distance( (temp_cast_39 + (frac( uv_TexCoord163 ) - temp_cast_37) * (temp_cast_40 - temp_cast_39) / (temp_cast_38 - temp_cast_37)) , float2( 0,0 ) ) , temp_output_316_0 ) ),_ToggleSwitch0) ) , _Vector6.x , _Vector6.y );
			float4 lerpResult268 = lerp( float4( 0,0,0,0 ) , temp_output_109_0 , clampResult385);
			o.Albedo = lerpResult268.rgb;
			float4 lerpResult273 = lerp( float4( 0,0,0,0 ) , ( temp_output_109_0 * _meido ) , clampResult385);
			o.Emission = ( lerpResult273 * _emi ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
577.3334;963.3334;2264;1066;2539.83;576.4194;1;False;False
Node;AmplifyShaderEditor.CommentaryNode;290;-4584.44,-2178.258;Float;False;718.3328;557.1606;pos;10;224;220;222;217;223;227;219;226;225;221;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-4530.055,-1989.46;Float;False;Constant;_Float7;Float 7;4;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-4529.055,-2072.46;Float;False;Constant;_Float6;Float 6;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;226;-4362.311,-1812.021;Float;False;Constant;_Float10;Float 10;4;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;225;-4357.796,-1924.438;Float;False;Constant;_Float9;Float 9;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;219;-4388.056,-2071.46;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectScaleNode;330;-4446.686,-820.6833;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;130;-4430.808,-607.0154;Float;False;Property;_Float0;Float 0;5;0;Create;True;0;0;False;0;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-4177.71,-1932.632;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-4192.109,-1815.041;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-4233.948,-766.1432;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;291;-4186.118,-3038.021;Float;False;Constant;_Vector1;Vector 1;5;0;Create;True;0;0;False;0;3,3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;223;-4005.057,-1790.46;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;217;-4005.967,-2078.537;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;222;-4004.994,-1934.991;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-3780.572,-2383.464;Float;False;Constant;_Float5;Float 5;4;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;79;-4031.524,-758.8299;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;363;-4412.475,-2262.938;Float;False;Constant;_Float11;Float 11;6;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;-3530.822,-2284.35;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;372;-4172.063,-3231.111;Float;False;Constant;_Vector4;Vector 4;5;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-3530.759,-2465.509;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;-3529.822,-2135.35;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FractNode;126;-3787.247,-758.5256;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;387;-3975.629,-3018.31;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-3679.907,-461.635;Float;False;Constant;_3;3;2;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-3678.974,-553.5901;Float;False;Constant;_1;1;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;362;-4236.475,-2314.938;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;231;-3319.679,-2410.574;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;230;-3319.678,-2288.882;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;206;-3337.125,-1441.697;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;229;-3316.817,-2167.351;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;207;-3337.125,-1563.388;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;80;-3617.65,-754.8537;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;88;-3511.137,-552.7889;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;388;-3964.663,-3211.938;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;275;-3779.606,-3066.027;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;163;-3334.264,-1320.165;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;92;-3528.152,-462.0485;Float;False;Constant;_2;2;2;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;173;-3375.462,-1876.31;Float;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;False;0;0,1,-1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;338;-3629.111,-2836.153;Float;False;Constant;_Float12;Float 12;5;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;246;-3081.429,-2480.379;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;361;-4064.475,-2322.938;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;248;-3074.429,-2163.379;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;343;-3241.267,-2709.965;Float;False;Constant;_Float17;Float 17;5;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;244;-3079.637,-1277.434;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;339;-3620.411,-2756.155;Float;False;Constant;_Float13;Float 13;5;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;358;-3778.941,-3226.857;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;292;-3109.388,-2968.943;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;247;-3080.429,-2325.379;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-3296.532,-178.9374;Float;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;326;-2888.859,-1835.642;Float;False;Property;_Float8;Float 8;7;0;Create;True;0;0;False;0;-0.19;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;318;-3070.281,-1977.242;Float;False;Constant;_Vector2;Vector 2;5;0;Create;True;0;0;False;0;1,3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FractNode;243;-3092.43,-1448.406;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;241;-3086.782,-1625.719;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;366;-4050.58,-2497.434;Float;False;Constant;_Float14;Float 14;6;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-3295.553,-257.9929;Float;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3279.001,-520.8663;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;82;-3343.512,-754.0833;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;170;-2927.254,-1278.697;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;279;-2961.806,-2974.259;Float;False;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;238;-2922.422,-2487.631;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;239;-2935.873,-2147.623;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;240;-2927.233,-2315.864;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;212;-2911.369,-1677.113;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;210;-2918.613,-1468.841;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;359;-3467.182,-3221.281;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;365;-3856.979,-2494.234;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;336;-3462.297,-2726.037;Float;False;Constant;_Vector5;Vector 5;5;0;Create;True;0;0;False;0;0,0,0,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;340;-3427.235,-2836.04;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;78;-3091.146,-690.6473;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;191;-2913.96,-1973.584;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;342;-3090.213,-2724.996;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;93;-3124.393,-257.4281;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;236;-2718.031,-2310.964;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;208;-2717.964,-1471.691;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;174;-2726.634,-1277.315;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;316;-2738.948,-1961.676;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;237;-2707.945,-2486;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;211;-2720.046,-1672.656;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;232;-2718.56,-2151.303;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;331;-2933.6,-2725.24;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-3364.166,126.5239;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;364;-3272.118,-3431.952;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;315;-2717.373,-2755.327;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;234;-2544.406,-2298.815;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;314;-2548.968,-2758.927;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;235;-2547.388,-2488.633;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;233;-2545.432,-2104.654;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;176;-2555.387,-1290.272;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;213;-2556.504,-1700.327;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;209;-2554.901,-1496.356;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;360;-3271.182,-3221.281;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;367;-3044.298,-3433.386;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;313;-2549.842,-3032.185;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;103;-3100.612,365.7763;Float;False;Constant;_center;center;2;0;Create;True;0;0;False;0;0,1,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;102;-3099.566,-50.22858;Float;False;Constant;_left;left;2;0;Create;True;0;0;False;0;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;101;-3133.125,-463.7733;Float;False;Constant;_right;right;2;0;Create;True;0;0;False;0;0,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;106;-3138.76,126.8369;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;370;-2576.394,-3352.03;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;354;-2225.717,-2836.322;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;245;-2285.584,-1811.256;Float;True;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-2730.393,130.5184;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-2747.843,-690.1663;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-2747.856,-269.3443;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;391;-2037.64,-2017.452;Float;False;Property;_ToggleSwitch0;Toggle Switch0;8;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;379;-1952.858,-2706.722;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;131;-2576.508,-849.8813;Float;True;Property;_TextureSample1;Texture Sample 1;10;0;Create;True;0;0;False;0;d7ea06d0b17c1f2449da59482578de9c;d7ea06d0b17c1f2449da59482578de9c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;105;-2456.637,-551.6854;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;386;-1907.153,-1759.774;Float;False;Constant;_Vector6;Vector 6;6;0;Create;True;0;0;False;0;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;357;-1745.729,-2023.722;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-2222.965,-593.0357;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;394;-2353.949,-218.8893;Float;False;Property;_meido;meido;6;0;Create;True;0;0;False;0;7.1;7.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;396;-2097.83,-400.4194;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;385;-1643.981,-1802.54;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-1726.118,-248.2455;Float;False;Property;_emi;emi;9;0;Create;True;0;0;False;0;2.16;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;273;-1891.814,-477.3899;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;267;-1495.48,-441.7882;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;268;-1778.869,-1371.819;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;393;-1736.261,-173.8312;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;35;-957.072,-738.7272;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;CRT Effect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;219;0;220;0
WireConnection;219;1;221;0
WireConnection;224;0;219;0
WireConnection;224;1;225;0
WireConnection;227;0;219;0
WireConnection;227;1;226;0
WireConnection;129;0;330;0
WireConnection;129;1;130;0
WireConnection;223;0;227;0
WireConnection;223;1;219;0
WireConnection;217;0;219;0
WireConnection;217;1;219;0
WireConnection;222;0;224;0
WireConnection;222;1;219;0
WireConnection;79;0;129;0
WireConnection;259;0;187;0
WireConnection;259;1;222;0
WireConnection;186;0;187;0
WireConnection;186;1;217;0
WireConnection;260;0;187;0
WireConnection;260;1;223;0
WireConnection;126;0;79;1
WireConnection;387;0;291;0
WireConnection;387;1;129;0
WireConnection;362;0;221;0
WireConnection;362;1;363;0
WireConnection;231;0;129;0
WireConnection;231;1;186;0
WireConnection;230;0;129;0
WireConnection;230;1;259;0
WireConnection;206;0;129;0
WireConnection;206;1;222;0
WireConnection;229;0;129;0
WireConnection;229;1;260;0
WireConnection;207;0;129;0
WireConnection;207;1;217;0
WireConnection;80;0;126;0
WireConnection;88;0;89;0
WireConnection;88;1;90;0
WireConnection;388;0;372;0
WireConnection;388;1;129;0
WireConnection;275;0;387;0
WireConnection;163;0;129;0
WireConnection;163;1;223;0
WireConnection;246;0;231;0
WireConnection;361;0;220;0
WireConnection;361;1;362;0
WireConnection;248;0;229;0
WireConnection;244;0;163;0
WireConnection;358;0;388;0
WireConnection;292;0;275;1
WireConnection;247;0;230;0
WireConnection;243;0;206;0
WireConnection;241;0;207;0
WireConnection;91;0;88;0
WireConnection;91;1;92;0
WireConnection;82;0;88;0
WireConnection;82;1;80;0
WireConnection;170;0;244;0
WireConnection;170;1;173;1
WireConnection;170;2;173;2
WireConnection;170;3;173;3
WireConnection;170;4;173;4
WireConnection;279;0;292;0
WireConnection;238;0;246;0
WireConnection;238;1;173;1
WireConnection;238;2;173;2
WireConnection;238;3;173;3
WireConnection;238;4;173;4
WireConnection;239;0;248;0
WireConnection;239;1;173;1
WireConnection;239;2;173;2
WireConnection;239;3;173;3
WireConnection;239;4;173;4
WireConnection;240;0;247;0
WireConnection;240;1;173;1
WireConnection;240;2;173;2
WireConnection;240;3;173;3
WireConnection;240;4;173;4
WireConnection;212;0;241;0
WireConnection;212;1;173;1
WireConnection;212;2;173;2
WireConnection;212;3;173;3
WireConnection;212;4;173;4
WireConnection;210;0;243;0
WireConnection;210;1;173;1
WireConnection;210;2;173;2
WireConnection;210;3;173;3
WireConnection;210;4;173;4
WireConnection;359;0;358;2
WireConnection;365;0;361;0
WireConnection;365;1;366;0
WireConnection;340;0;338;0
WireConnection;340;1;339;0
WireConnection;78;0;91;0
WireConnection;78;1;80;0
WireConnection;191;0;318;1
WireConnection;191;1;318;2
WireConnection;342;0;326;0
WireConnection;342;1;343;0
WireConnection;93;0;95;0
WireConnection;93;1;94;0
WireConnection;93;2;82;0
WireConnection;236;0;240;0
WireConnection;208;0;210;0
WireConnection;174;0;170;0
WireConnection;316;0;191;0
WireConnection;316;1;326;0
WireConnection;237;0;238;0
WireConnection;211;0;212;0
WireConnection;232;0;239;0
WireConnection;331;0;342;0
WireConnection;331;1;340;0
WireConnection;331;2;336;2
WireConnection;331;3;336;3
WireConnection;331;4;336;4
WireConnection;86;0;78;0
WireConnection;86;1;93;0
WireConnection;364;0;359;0
WireConnection;364;1;365;0
WireConnection;315;0;279;0
WireConnection;234;0;236;0
WireConnection;234;1;316;0
WireConnection;314;0;315;0
WireConnection;314;1;331;0
WireConnection;235;0;237;0
WireConnection;235;1;316;0
WireConnection;233;0;232;0
WireConnection;233;1;316;0
WireConnection;176;0;174;0
WireConnection;176;1;316;0
WireConnection;213;0;211;0
WireConnection;213;1;316;0
WireConnection;209;0;208;0
WireConnection;209;1;316;0
WireConnection;360;0;359;0
WireConnection;360;1;361;0
WireConnection;367;0;364;0
WireConnection;313;0;279;0
WireConnection;313;1;331;0
WireConnection;106;0;95;0
WireConnection;106;1;94;0
WireConnection;106;2;86;0
WireConnection;370;0;367;0
WireConnection;370;1;360;0
WireConnection;354;0;313;0
WireConnection;354;1;314;0
WireConnection;245;0;235;0
WireConnection;245;1;234;0
WireConnection;245;2;233;0
WireConnection;245;3;213;0
WireConnection;245;4;209;0
WireConnection;245;5;176;0
WireConnection;100;0;106;0
WireConnection;100;1;103;0
WireConnection;98;0;78;0
WireConnection;98;1;101;0
WireConnection;99;0;93;0
WireConnection;99;1;102;0
WireConnection;391;1;245;0
WireConnection;379;0;354;0
WireConnection;379;1;370;0
WireConnection;105;0;98;0
WireConnection;105;1;99;0
WireConnection;105;2;100;0
WireConnection;357;0;379;0
WireConnection;357;1;391;0
WireConnection;109;0;131;0
WireConnection;109;1;105;0
WireConnection;396;0;109;0
WireConnection;396;1;394;0
WireConnection;385;0;357;0
WireConnection;385;1;386;1
WireConnection;385;2;386;2
WireConnection;273;1;396;0
WireConnection;273;2;385;0
WireConnection;267;0;273;0
WireConnection;267;1;133;0
WireConnection;268;1;109;0
WireConnection;268;2;385;0
WireConnection;393;0;394;0
WireConnection;35;0;268;0
WireConnection;35;2;267;0
ASEEND*/
//CHKSM=A92DCF4389CE8454FD8703055DDDF398F9417F29