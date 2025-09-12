// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DifShader/SHD_VFX_BaoZha2"
{
	Properties
	{
		_TextureSample0("噪声贴图", 2D) = "white" {}
		_Float2("噪声贴图大小控制", Range( 0 , 5)) = 1
		_Vector0("流动速度", Vector) = (0,0.1,0,0)
		[HDR]_InsideFire("内焰", Color) = (1,0.08018869,0.08018869,0)
		[HDR]_OutFireCol("外焰", Color) = (0.6792453,0.02883587,0.02883587,0)
		[HDR]_DustCol("烟雾淡", Color) = (0.0471698,0.006897471,0.01309935,0)
		_Color0("烟雾深", Color) = (0,0,0,0)
		_DissolveTex("溶解贴图", 2D) = "white" {}
		_Float3("溶解贴图大小控制", Range( 0 , 5)) = 1
		_Vector1("流动速度", Vector) = (0,0.1,0,0)
		_softDis("软溶解", Range( 0 , 0.49)) = 0.2097939
		_Float5("顶点偏移幅度", Range( 0 , 0.5)) = 0.03

	}

	SubShader
	{
		

		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add, Add
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		

		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			#define ASE_VERSION 19801


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _TextureSample0;
			uniform float2 _Vector0;
			uniform float _Float2;
			uniform float _Float5;
			uniform float4 _InsideFire;
			uniform float4 _OutFireCol;
			uniform float4 _Color0;
			uniform float4 _DustCol;
			uniform float _softDis;
			uniform sampler2D _DissolveTex;
			uniform float2 _Vector1;
			uniform float _Float3;


			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 temp_cast_0 = (_Float2).xx;
				float2 texCoord97 = v.ase_texcoord * temp_cast_0 + float2( 0,0 );
				float2 panner114 = ( 1.0 * _Time.y * _Vector0 + texCoord97);
				float4 tex2DNode78 = tex2Dlod( _TextureSample0, float4( panner114, 0, 0.0) );
				float3 ase_normalWS = UnityObjectToWorldNormal( v.ase_normal );
				
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord1;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = ( tex2DNode78.r * _Float5 * ase_normalWS );
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}

			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 temp_cast_0 = (_Float2).xx;
				float2 texCoord97 = i.ase_texcoord1.xy * temp_cast_0 + float2( 0,0 );
				float2 panner114 = ( 1.0 * _Time.y * _Vector0 + texCoord97);
				float4 tex2DNode78 = tex2D( _TextureSample0, panner114 );
				float temp_output_127_0 = ( ( 1.0 - i.ase_texcoord1.xy.y ) * tex2DNode78.r );
				float4 texCoord141 = i.ase_texcoord1;
				texCoord141.xy = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float Custom1X143 = texCoord141.z;
				float3 lerpResult110 = lerp( _InsideFire.rgb , _OutFireCol.rgb , step( temp_output_127_0 , Custom1X143 ));
				float4 lerpResult129 = lerp( float4( _Color0.rgb , 0.0 ) , _DustCol , temp_output_127_0);
				float Custom1Y144 = texCoord141.w;
				float4 lerpResult81 = lerp( float4( lerpResult110 , 0.0 ) , lerpResult129 , step( temp_output_127_0 , Custom1Y144 ));
				float2 temp_cast_4 = (_Float3).xx;
				float2 texCoord99 = i.ase_texcoord1.xy * temp_cast_4 + float2( 0,0 );
				float2 panner117 = ( 1.0 * _Time.y * _Vector1 + texCoord99);
				float4 texCoord142 = i.ase_texcoord2;
				texCoord142.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float Custom1Z145 = texCoord142.x;
				float smoothstepResult91 = smoothstep( _softDis , ( 1.0 - _softDis ) , saturate( ( ( ( ( pow( i.ase_texcoord1.xy.y , 3.0 ) * 0.0 ) + tex2D( _DissolveTex, panner117 ).r ) + 1.0 ) - ( Custom1Z145 * 3.0 ) ) ));
				float4 appendResult152 = (float4(lerpResult81.rgb , smoothstepResult91));
				

				finalColor = appendResult152;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.RangedFloatNode;100;-3392,-16;Inherit;False;Property;_Float3;溶解贴图大小控制;11;0;Create;False;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-3440,-448;Inherit;False;Property;_Float2;噪声贴图大小控制;2;0;Create;False;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;125;-3168,-832;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;99;-3104,-32;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;116;-3168,208;Inherit;False;Property;_Vector1;流动速度;12;0;Create;False;0;0;0;False;0;False;0,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;115;-3136,-240;Inherit;False;Property;_Vector0;流动速度;3;0;Create;False;0;0;0;False;0;False;0,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;97;-3104,-464;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;117;-2768,64;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;142;-2992,-1120;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;140;-2272,-192;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;114;-2768,-336;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;141;-2992,-1296;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;85;-2512,48;Inherit;True;Property;_DissolveTex;溶解贴图;10;0;Create;False;0;0;0;False;0;False;-1;065da0b36eba63c468b56a47ba0ec074;065da0b36eba63c468b56a47ba0ec074;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RegisterLocalVarNode;145;-2704,-1104;Inherit;False;Custom1Z;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-2032,-96;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-2496,-432;Inherit;True;Property;_TextureSample0;噪声贴图;1;0;Create;False;0;0;0;False;0;False;-1;317a32f8fa9506d4e900d62eb734f7ab;317a32f8fa9506d4e900d62eb734f7ab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;126;-2896,-832;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;143;-2704,-1296;Inherit;False;Custom1X;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;149;-1744,368;Inherit;False;145;Custom1Z;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;133;-1856,-64;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-2048,-528;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-2704,-1216;Inherit;False;Custom1Y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;146;-2208,-624;Inherit;False;143;Custom1X;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-1552,48;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-1488,320;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;109;-1792,-768;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;107;-2160,-912;Inherit;False;Property;_OutFireCol;外焰;5;1;[HDR];Create;False;0;0;0;False;0;False;0.6792453,0.02883587,0.02883587,0;0.6792453,0.02883587,0.02883587,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;83;-2160,-1120;Inherit;False;Property;_InsideFire;内焰;4;1;[HDR];Create;False;0;0;0;False;0;False;1,0.08018869,0.08018869,0;1,0.08018869,0.08018869,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;84;-688,-512;Inherit;False;Property;_DustCol;烟雾淡;6;1;[HDR];Create;False;0;0;0;False;0;False;0.0471698,0.006897471,0.01309935,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;131;-672,-304;Inherit;False;Property;_Color0;烟雾深;7;0;Create;False;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;147;-1600,-240;Inherit;False;144;Custom1Y;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;87;-1312,144;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-1152,464;Inherit;False;Property;_softDis;软溶解;14;0;Create;False;0;0;0;False;0;False;0.2097939;0;0;0.49;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;110;-1520,-848;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;79;-1184,-384;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;129;-288,-288;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;88;-992,176;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;93;-880,384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;113;-720,752;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;111;-768,656;Inherit;False;Property;_Float5;顶点偏移幅度;15;0;Create;False;0;0;0;False;0;False;0.03;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;81;48,-464;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;91;-624,144;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-432,544;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-1664,-304;Inherit;False;Property;_Float0;烟雾大小过渡;8;0;Create;False;0;0;0;False;0;False;0.4091436;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-2224,-688;Inherit;False;Property;_Float4;内外焰过渡;9;0;Create;False;0;0;0;False;0;False;0.5371781;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1808,304;Inherit;False;Property;_Float1;溶解;13;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;152;384,-304;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;151;672,-416;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;100;5;DifShader/SHD_VFX_BaoZha2;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;5;False;;10;False;;True;1;False;;1;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;99;0;100;0
WireConnection;97;0;98;0
WireConnection;117;0;99;0
WireConnection;117;2;116;0
WireConnection;140;0;125;2
WireConnection;114;0;97;0
WireConnection;114;2;115;0
WireConnection;85;1;117;0
WireConnection;145;0;142;1
WireConnection;150;0;140;0
WireConnection;78;1;114;0
WireConnection;126;0;125;2
WireConnection;143;0;141;3
WireConnection;133;0;150;0
WireConnection;133;1;85;1
WireConnection;127;0;126;0
WireConnection;127;1;78;1
WireConnection;144;0;141;4
WireConnection;86;0;133;0
WireConnection;90;0;149;0
WireConnection;109;0;127;0
WireConnection;109;1;146;0
WireConnection;87;0;86;0
WireConnection;87;1;90;0
WireConnection;110;0;83;5
WireConnection;110;1;107;5
WireConnection;110;2;109;0
WireConnection;79;0;127;0
WireConnection;79;1;147;0
WireConnection;129;0;131;5
WireConnection;129;1;84;0
WireConnection;129;2;127;0
WireConnection;88;0;87;0
WireConnection;93;0;92;0
WireConnection;81;0;110;0
WireConnection;81;1;129;0
WireConnection;81;2;79;0
WireConnection;91;0;88;0
WireConnection;91;1;92;0
WireConnection;91;2;93;0
WireConnection;112;0;78;1
WireConnection;112;1;111;0
WireConnection;112;2;113;0
WireConnection;152;0;81;0
WireConnection;152;3;91;0
WireConnection;151;0;152;0
WireConnection;151;1;112;0
ASEEND*/
//CHKSM=0C9E06708C3B9989C48EDAE91D0BF64247C173DD