// Upgrade NOTE: upgraded instancing buffer 'NewAmplifyShader1' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader 1"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.3
		_TileMapTextureRValue("TileMap Texture R Value", 2D) = "white" {}
		_GroundColor("Ground Color", Color) = (1,0,0,0)
		_UndergroundColor(" Underground Color", Color) = (1,1,1,0)
		_01Ratio("0-1 Ratio", Float) = 5
		_MoveSpeed("Move Speed", Float) = 0.2
		_FluidColor("Fluid Color", Color) = (1,0,0,0)
		_FluidBackColor("Fluid Back Color", Color) = (1,1,1,0)
		_FluidBackColorDepth("Fluid Back Color Depth", Range( 0 , 1)) = 0.2
		_Emission("Emission", Range( 0 , 100)) = 3.236728
		_NoiseTillingSize("Noise Tilling Size", Float) = 0
		_EyeColor("Eye Color", Color) = (1,0,0,0)
		_EyeEmission("Eye Emission", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest-1" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _UndergroundColor;
		uniform float4 _GroundColor;
		uniform sampler2D _TileMapTextureRValue;
		uniform float _NoiseTillingSize;
		uniform float _MoveSpeed;
		uniform float _01Ratio;
		uniform float4 _FluidColor;
		uniform float4 _FluidBackColor;
		uniform float _FluidBackColorDepth;
		uniform float4 _EyeColor;
		uniform float _EyeEmission;
		uniform float _Cutoff = 0.3;

		UNITY_INSTANCING_BUFFER_START(NewAmplifyShader1)
			UNITY_DEFINE_INSTANCED_PROP(float4, _TileMapTextureRValue_ST)
#define _TileMapTextureRValue_ST_arr NewAmplifyShader1
			UNITY_DEFINE_INSTANCED_PROP(float, _Emission)
#define _Emission_arr NewAmplifyShader1
		UNITY_INSTANCING_BUFFER_END(NewAmplifyShader1)


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _TileMapTextureRValue_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_TileMapTextureRValue_ST_arr, _TileMapTextureRValue_ST);
			float2 uv_TileMapTextureRValue = i.uv_texcoord * _TileMapTextureRValue_ST_Instance.xy + _TileMapTextureRValue_ST_Instance.zw;
			float4 tex2DNode2 = tex2D( _TileMapTextureRValue, uv_TileMapTextureRValue );
			float clampResult136 = clamp( ( tex2DNode2.r + tex2DNode2.g + tex2DNode2.b ) , 0.0 , 1.0 );
			float4 lerpResult150 = lerp( _UndergroundColor , _GroundColor , clampResult136);
			o.Albedo = lerpResult150.rgb;
			float2 temp_output_148_0 = ( float2( 1,1 ) * _NoiseTillingSize );
			float3 ase_worldPos = i.worldPos;
			float2 uv_TexCoord65 = i.uv_texcoord * temp_output_148_0 + ( ase_worldPos + float3( ( _Time.y * float2( 1,1 ) * _MoveSpeed ) ,  0.0 ) ).xy;
			float simplePerlin2D67 = snoise( uv_TexCoord65 );
			float2 uv_TexCoord63 = i.uv_texcoord * temp_output_148_0 + ( ase_worldPos + float3( ( _Time.y * float2( -1,-1 ) * _MoveSpeed ) ,  0.0 ) ).xy;
			float simplePerlin2D68 = snoise( uv_TexCoord63 );
			float2 uv_TexCoord66 = i.uv_texcoord * temp_output_148_0 + ( ase_worldPos + float3( ( _Time.y * float2( 1,-1 ) * _MoveSpeed ) ,  0.0 ) ).xy;
			float simplePerlin2D69 = snoise( uv_TexCoord66 );
			float2 uv_TexCoord64 = i.uv_texcoord * temp_output_148_0 + ( ase_worldPos + float3( ( _Time.y * float2( -1,1 ) * _MoveSpeed ) ,  0.0 ) ).xy;
			float simplePerlin2D70 = snoise( uv_TexCoord64 );
			float clampResult37 = clamp( ( ( simplePerlin2D67 * simplePerlin2D68 * simplePerlin2D69 * simplePerlin2D70 ) * _01Ratio ) , 0.0 , 1.0 );
			float temp_output_1_0 = tex2DNode2.r;
			float4 temp_cast_9 = (0.0).xxxx;
			float4 temp_cast_10 = (1.0).xxxx;
			float4 clampResult141 = clamp( ( ( clampResult37 * _FluidColor * temp_output_1_0 ) + ( temp_output_1_0 * _FluidBackColor * _FluidBackColorDepth ) ) , temp_cast_9 , temp_cast_10 );
			float _Emission_Instance = UNITY_ACCESS_INSTANCED_PROP(_Emission_arr, _Emission);
			o.Emission = ( ( clampResult141 * _Emission_Instance ) + ( tex2DNode2.b * _EyeColor * _EyeEmission ) ).rgb;
			o.Alpha = 1;
			float lerpResult159 = lerp( 1.0 , 0.0 , tex2DNode2.g);
			clip( lerpResult159 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
688;781.3334;1304;590;4799.735;484.9191;2.075844;True;False
Node;AmplifyShaderEditor.Vector2Node;109;-6903.997,-2363.06;Float;False;Constant;_Vector3;Vector 3;5;0;Create;True;0;0;False;0;-1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;41;-7164.673,-2713.364;Float;False;Property;_MoveSpeed;Move Speed;5;0;Create;True;0;0;False;0;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;112;-6909.847,-2058.944;Float;False;Constant;_Vector5;Vector 5;5;0;Create;True;0;0;False;0;1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;113;-6918.65,-1768.45;Float;False;Constant;_Vector6;Vector 6;5;0;Create;True;0;0;False;0;-1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TimeNode;43;-6967.944,-1572.196;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;108;-6909.997,-2683.06;Float;False;Constant;_Vector2;Vector 2;5;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;54;-6689.684,-1572.746;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;51;-6945.339,-1405.179;Float;False;Constant;_v2;v2;3;0;Create;True;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;149;-6997.939,-1283.823;Float;False;Property;_NoiseTillingSize;Noise Tilling Size;10;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-6646.428,-1749.99;Float;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-6635.046,-2052.964;Float;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-6624.87,-2712.021;Float;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-6634.399,-2372.479;Float;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-6441.478,-2377.909;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-6693.939,-1413.823;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-6433.922,-2717.452;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;114;-6443.035,-1751.35;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-6439.93,-2057.021;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;65;-6243.187,-2718.801;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-6235.452,-1754.393;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;63;-6234.973,-2381.232;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-6236.624,-2055.344;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;70;-5952.72,-1817.76;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;69;-5953.33,-2117.723;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;67;-5944.062,-2791.607;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;68;-5950.014,-2430.592;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-5654.556,-1652.695;Float;False;Property;_01Ratio;0-1 Ratio;4;0;Create;True;0;0;False;0;5;1000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-5644.837,-1796.891;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-5395.701,-1565.428;Float;False;Constant;_Float7;Float 7;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-5448.971,-1800.188;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-4892.232,-962.7935;Float;True;Property;_TileMapTextureRValue;TileMap Texture R Value;1;0;Create;True;0;0;False;0;ba0b1731d62c23048b65f430a7ae54cf;ba0b1731d62c23048b65f430a7ae54cf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;74;-5405.059,-1475.546;Float;False;Constant;_Float8;Float 8;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;147;-4430.416,-544.2497;Float;False;Property;_FluidColor;Fluid Color;6;0;Create;True;0;0;False;0;1,0,0,0;1,0,0.07585334,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;77;-4447.736,-367.7873;Float;False;Property;_FluidBackColor;Fluid Back Color;7;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;37;-5183.204,-1797.016;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-4480.953,-185.5953;Float;False;Property;_FluidBackColorDepth;Fluid Back Color Depth;8;0;Create;True;0;0;False;0;0.2;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;1;-4462.781,-743.3434;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-4077.095,-984.7072;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-4080.475,-741.009;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;133;-4428.013,-1095.365;Float;False;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;131;-4430.013,-1250.364;Float;False;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;132;-4427.013,-1176.365;Float;False;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;146;-3798.407,-837.4659;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;138;-3755.21,-931.7753;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-3753.516,-1015.775;Float;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-4058.457,-1277.212;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-3674.625,173.1234;Float;False;Property;_EyeEmission;Eye Emission;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;164;-4468.884,-87.04464;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-3566.413,-605.4614;Float;False;InstancedProperty;_Emission;Emission;9;0;Create;True;0;0;False;0;3.236728;2;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;141;-3522.645,-832.405;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;166;-3732.749,-7.475037;Float;False;Property;_EyeColor;Eye Color;11;0;Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;158;-4012.128,-395.6198;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-3800.109,-543.6318;Float;True;Constant;_Float3;Float 3;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;140;-3549.629,-1317.989;Float;False;Property;_GroundColor;Ground Color;2;0;Create;True;0;0;False;0;1,0,0,0;0.8980392,0.8980392,0.8980392,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;152;-3541.359,-1497.048;Float;False;Property;_UndergroundColor; Underground Color;3;0;Create;True;0;0;False;0;1,1,1,0;0.7547169,0.7547169,0.7547169,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-3219.38,-813.4823;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-3325.883,-146.5565;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-3974.792,-506.2955;Float;True;Constant;_Float4;Float 4;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;136;-3452.236,-1113.589;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;150;-3275.359,-1115.048;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;159;-3608.094,-416.9548;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-3456.691,-966.1325;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;-2964.687,-694.5793;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-2722.859,-1112.159;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;New Amplify Shader 1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.3;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;50;0;43;2
WireConnection;50;1;113;0
WireConnection;50;2;41;0
WireConnection;52;0;43;2
WireConnection;52;1;112;0
WireConnection;52;2;41;0
WireConnection;53;0;43;2
WireConnection;53;1;108;0
WireConnection;53;2;41;0
WireConnection;56;0;43;2
WireConnection;56;1;109;0
WireConnection;56;2;41;0
WireConnection;58;0;54;0
WireConnection;58;1;56;0
WireConnection;148;0;51;0
WireConnection;148;1;149;0
WireConnection;59;0;54;0
WireConnection;59;1;53;0
WireConnection;114;0;54;0
WireConnection;114;1;50;0
WireConnection;57;0;54;0
WireConnection;57;1;52;0
WireConnection;65;0;148;0
WireConnection;65;1;59;0
WireConnection;64;0;148;0
WireConnection;64;1;114;0
WireConnection;63;0;148;0
WireConnection;63;1;58;0
WireConnection;66;0;148;0
WireConnection;66;1;57;0
WireConnection;70;0;64;0
WireConnection;69;0;66;0
WireConnection;67;0;65;0
WireConnection;68;0;63;0
WireConnection;71;0;67;0
WireConnection;71;1;68;0
WireConnection;71;2;69;0
WireConnection;71;3;70;0
WireConnection;36;0;71;0
WireConnection;36;1;72;0
WireConnection;37;0;36;0
WireConnection;37;1;73;0
WireConnection;37;2;74;0
WireConnection;1;0;2;1
WireConnection;145;0;37;0
WireConnection;145;1;147;0
WireConnection;145;2;1;0
WireConnection;143;0;1;0
WireConnection;143;1;77;0
WireConnection;143;2;144;0
WireConnection;133;0;2;3
WireConnection;131;0;2;1
WireConnection;132;0;2;2
WireConnection;146;0;145;0
WireConnection;146;1;143;0
WireConnection;134;0;131;0
WireConnection;134;1;132;0
WireConnection;134;2;133;0
WireConnection;164;0;2;3
WireConnection;141;0;146;0
WireConnection;141;1;137;0
WireConnection;141;2;138;0
WireConnection;158;0;2;2
WireConnection;122;0;141;0
WireConnection;122;1;123;0
WireConnection;165;0;164;0
WireConnection;165;1;166;0
WireConnection;165;2;167;0
WireConnection;136;0;134;0
WireConnection;136;1;137;0
WireConnection;136;2;138;0
WireConnection;150;0;152;0
WireConnection;150;1;140;0
WireConnection;150;2;136;0
WireConnection;159;0;160;0
WireConnection;159;1;161;0
WireConnection;159;2;158;0
WireConnection;168;0;122;0
WireConnection;168;1;165;0
WireConnection;0;0;150;0
WireConnection;0;2;168;0
WireConnection;0;10;159;0
ASEEND*/
//CHKSM=3CF97C762DB12F3C8839821117021ACD2FDD49FE