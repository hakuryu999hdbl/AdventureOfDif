// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DifShader/SHD_VFX_BaoZha"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_Float0("Voronoi噪点速度", Range( 0 , 5)) = 1.271385
		_Float1("Voronoi噪点Scale", Range( 0 , 100)) = 20.63163
		[HDR]_Color("内焰", Color) = (1,0.4708344,0.3632075,0)
		[HDR]_Color0("黑尘", Color) = (0,0,0,0)

	}


	Category
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest LEqual
			
			Pass {

				CGPROGRAM
				#define ASE_VERSION 19801

				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif

				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.5
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};


				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float4 _Color;
				uniform float4 _Color0;
				uniform float _Float0;
				uniform float _Float1;
				inline float2 UnityVoronoiRandomVector( float2 UV, float offset )
				{
					float2x2 m = float2x2( 15.27, 47.63, 99.41, 89.98 );
					UV = frac( sin(mul(UV, m) ) * 46839.32 );
					return float2( sin(UV.y* +offset ) * 0.5 + 0.5, cos( UV.x* offset ) * 0.5 + 0.5 );
				}
				
				//x - Out y - Cells
				float3 UnityVoronoi( float2 UV, float AngleOffset, float CellDensity, inout float2 mr )
				{
					float2 g = floor( UV * CellDensity );
					float2 f = frac( UV * CellDensity );
					float t = 8.0;
					float3 res = float3( 8.0, 0.0, 0.0 );
				
					for( int y = -1; y <= 1; y++ )
					{
						for( int x = -1; x <= 1; x++ )
						{
							float2 lattice = float2( x, y );
							float2 offset = UnityVoronoiRandomVector( lattice + g, AngleOffset );
							float d = distance( lattice + offset, f );
				
							if( d < res.x )
							{
								mr = f - lattice - offset;
								res = float3( d, offset.x, offset.y );
							}
						}
					}
					return res;
				}
				


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float4 texCoord2 = i.texcoord;
					texCoord2.xy = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 uv1 = 0;
					float3 unityVoronoy1 = UnityVoronoi(texCoord2.xy,( _Time.y * _Float0 ),_Float1,uv1);
					float Custom1Y40 = texCoord2.w;
					float clampResult27 = clamp( pow( unityVoronoy1.x , Custom1Y40 ) , 0.0 , 1.0 );
					float4 lerpResult37 = lerp( _Color , float4( _Color0.rgb , 0.0 ) , clampResult27);
					float4 break30 = lerpResult37;
					float clampResult38 = clamp( unityVoronoy1.x , 0.0 , 1.0 );
					float Custom1X39 = texCoord2.z;
					float clampResult28 = clamp( step( ( 1.0 - clampResult38 ) , Custom1X39 ) , 0.0 , 1.0 );
					float4 appendResult24 = (float4(break30.r , break30.g , break30.b , clampResult28));
					

					fixed4 col = appendResult24;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
	}
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.SimpleTimeNode;8;-2496,-96;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2512,176;Inherit;False;Property;_Float0;Voronoi噪点速度;0;0;Create;False;0;0;0;False;0;False;1.271385;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-2336,-400;Inherit;True;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2208,16;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2096,144;Inherit;False;Property;_Float1;Voronoi噪点Scale;1;0;Create;False;0;0;0;False;0;False;20.63163;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1904,-272;Inherit;False;Custom1Y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;1;-1824,-160;Inherit;True;0;0;1;1;1;False;1;True;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;2;False;2;FLOAT;12.84;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.GetLocalVarNode;42;-1648,-32;Inherit;True;40;Custom1Y;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;38;-1184,208;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-1904,-352;Inherit;False;Custom1X;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;31;-1424,-176;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-1056,608;Inherit;False;39;Custom1X;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;15;-1120,-736;Inherit;False;Property;_Color;内焰;2;1;[HDR];Create;False;0;0;0;False;0;False;1,0.4708344,0.3632075,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;35;-1104,-320;Inherit;False;Property;_Color0;黑尘;3;1;[HDR];Create;False;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;26;-896,160;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;27;-1168,-32;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;23;96,160;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;37;0,-544;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;28;320,160;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;30;240,-192;Inherit;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;44;-1632,-592;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;46;-1376,-592;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;43;-2064,-576;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;45;-1792,-528;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.49;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1744,288;Inherit;False;Property;_Float3;Pow;4;0;Create;False;0;0;0;False;0;False;2.518237;0;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;496,-32;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;49;-1104,-528;Inherit;False;Property;_Color1;外焰;5;1;[HDR];Create;False;0;0;0;False;0;False;0.5283019,0.2179316,0.1270915,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StepOpNode;50;-736,-64;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;51;-432,-368;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;22;672,-32;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;11;DifShader/SHD_VFX_BaoZha;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;True;2;5;False;;10;False;;2;5;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;False;0;False;;False;False;False;False;False;False;False;False;False;True;2;False;;True;3;False;;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;7;0;8;0
WireConnection;7;1;6;0
WireConnection;40;0;2;4
WireConnection;1;0;2;0
WireConnection;1;1;7;0
WireConnection;1;2;13;0
WireConnection;38;0;1;0
WireConnection;39;0;2;3
WireConnection;31;0;1;0
WireConnection;31;1;42;0
WireConnection;26;0;38;0
WireConnection;27;0;31;0
WireConnection;23;0;26;0
WireConnection;23;1;41;0
WireConnection;37;0;15;0
WireConnection;37;1;35;5
WireConnection;37;2;27;0
WireConnection;28;0;23;0
WireConnection;30;0;37;0
WireConnection;44;0;45;0
WireConnection;44;1;45;0
WireConnection;44;2;45;0
WireConnection;44;3;45;0
WireConnection;46;0;44;0
WireConnection;45;0;43;2
WireConnection;24;0;30;0
WireConnection;24;1;30;1
WireConnection;24;2;30;2
WireConnection;24;3;28;0
WireConnection;50;0;27;0
WireConnection;51;0;49;0
WireConnection;51;1;15;0
WireConnection;51;2;50;0
WireConnection;22;0;24;0
ASEEND*/
//CHKSM=BCC922784348DBAEA5F57D3867759EBD46ABB078