// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Emission("Emission", Float) = 0
		_0Color("0 Color", Color) = (0,0,0,0)
		_1Color("1 Color", Color) = (0,0,0,0)
		_MonochromeRatio(" Monochrome Ratio", Float) = 0
		_NoiseTillingSize("Noise Tilling Size", Vector) = (5,5,0,0)
		_01Ratio("0-1 Ratio", Float) = 0
		_MoveSpeed("Move Speed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _1Color;
		uniform float4 _0Color;
		uniform float2 _NoiseTillingSize;
		uniform float _MoveSpeed;
		uniform float _01Ratio;
		uniform float _MonochromeRatio;
		uniform float _Emission;


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
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode1 = tex2D( _TextureSample0, uv_TextureSample0 );
			o.Albedo = tex2DNode1.rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float3 ase_worldPos = i.worldPos;
			float2 uv_TexCoord27 = i.uv_texcoord * _NoiseTillingSize + ( ase_worldPos + ( _Time.y * _MoveSpeed ) ).xy;
			float simplePerlin2D25 = snoise( uv_TexCoord27 );
			float2 uv_TexCoord56 = i.uv_texcoord * _NoiseTillingSize + ( ase_worldPos + ( _Time.y * ( _MoveSpeed * -1.0 ) ) ).xy;
			float simplePerlin2D63 = snoise( uv_TexCoord56 );
			float clampResult70 = clamp( ( ( simplePerlin2D25 * simplePerlin2D63 ) * _01Ratio ) , 0.0 , 1.0 );
			float4 lerpResult33 = lerp( _1Color , _0Color , round( clampResult70 ));
			float4 lerpResult2 = lerp( temp_cast_1 , ( 1.0 * lerpResult33 ) , round( ( tex2DNode1.r * _MonochromeRatio ) ));
			o.Emission = ( lerpResult2 * _Emission ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
30;160.6667;2539;1043;1896.607;553.1326;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;47;-3403.292,1011.108;Float;False;Property;_MoveSpeed;Move Speed;7;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-3410.626,1379.254;Float;False;Constant;_Float6;Float 6;9;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;61;-3419.509,1195.211;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;39;-3413.924,857.6414;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-3257.545,1352.241;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;40;-3114.885,1083.443;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-3067.138,1327.72;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-3061.553,990.1499;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;28;-2885.081,811.3091;Float;False;Property;_NoiseTillingSize;Noise Tilling Size;5;0;Create;True;0;0;False;0;5,5;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-2852.53,1324.262;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-2846.945,986.6926;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-2630.58,886.7458;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;56;-2636.166,1224.316;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;63;-2345.295,1153.024;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;25;-2339.708,815.4542;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-2102.652,1141.262;Float;False;Property;_01Ratio;0-1 Ratio;6;0;Create;True;0;0;False;0;0;1E+11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2104.933,1031.068;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1903.431,1259.682;Float;False;Constant;_Float7;Float 7;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-1900.431,1337.682;Float;False;Constant;_Float8;Float 8;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1909.067,1015.694;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;70;-1771.431,1092.682;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-1644.992,661.5369;Float;False;Property;_0Color;0 Color;2;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;-1638.688,470.9314;Float;False;Property;_1Color;1 Color;3;0;Create;True;0;0;False;0;0,0,0,0;1,0.8427867,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-1145.644,-83.09145;Float;False;Property;_MonochromeRatio; Monochrome Ratio;4;0;Create;True;0;0;False;0;0;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1379.185,-359.4843;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;cd460ee4ac5c1e746b7a734cc7cc64dd;f20f33fdd3de86044a83229c5a2c6ccd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RoundOpNode;43;-1693.693,1019.556;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1007.629,-189.9136;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1182.543,894.6924;Float;True;Constant;_Float3;Float 3;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;-1213.697,770.7249;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-985.2572,778.0616;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-987.3334,19.33262;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;21;-755.6668,-149.02;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2;-517.5366,105.0379;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-550.7493,270.7863;Float;True;Property;_Emission;Emission;1;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-210.5366,112.0379;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;New Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;68;0;47;0
WireConnection;68;1;69;0
WireConnection;62;0;61;2
WireConnection;62;1;68;0
WireConnection;46;0;39;2
WireConnection;46;1;47;0
WireConnection;58;0;40;0
WireConnection;58;1;62;0
WireConnection;42;0;40;0
WireConnection;42;1;46;0
WireConnection;27;0;28;0
WireConnection;27;1;42;0
WireConnection;56;0;28;0
WireConnection;56;1;58;0
WireConnection;63;0;56;0
WireConnection;25;0;27;0
WireConnection;67;0;25;0
WireConnection;67;1;63;0
WireConnection;44;0;67;0
WireConnection;44;1;45;0
WireConnection;70;0;44;0
WireConnection;70;1;71;0
WireConnection;70;2;72;0
WireConnection;43;0;70;0
WireConnection;22;0;1;1
WireConnection;22;1;23;0
WireConnection;33;0;35;0
WireConnection;33;1;10;0
WireConnection;33;2;43;0
WireConnection;36;0;37;0
WireConnection;36;1;33;0
WireConnection;21;0;22;0
WireConnection;2;0;34;0
WireConnection;2;1;36;0
WireConnection;2;2;21;0
WireConnection;3;0;2;0
WireConnection;3;1;4;0
WireConnection;0;0;1;0
WireConnection;0;2;3;0
ASEEND*/
//CHKSM=C0EBFF7D2BB57D8961424694D5E224D9E8D1FC1F