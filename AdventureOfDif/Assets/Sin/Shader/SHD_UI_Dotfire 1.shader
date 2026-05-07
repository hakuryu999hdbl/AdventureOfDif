// Made with Amplify Shader Editor v1.9.8.1  123
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DifShader/SHD_UI_DotFire1"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _Texture0("遮罩贴图", 2D) = "white" {}
        _Float5("遮罩贴图强度", Range( 0 , 2)) = 1
        _Float3("火焰运动速度", Range( 0 , 5)) = 1
        _Color1("火焰颜色", Color) = (1,0.2705882,0.5921569,1)
        [Toggle]_ToggleSwitch0("波点是否翻转", Float) = 1
        _Float0("波点密度", Range( 0 , 30)) = 3
        _Float2("波点旋转", Range( 0 , 1)) = 0
        _Float4("波点强度", Range( 0.1 , 2)) = 1
        [HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            #define ASE_VERSION 19801

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                float4 ase_texcoord3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform float4 _Color1;
            uniform sampler2D _Texture0;
            uniform float4 _Texture0_ST;
            uniform float _Float5;
            uniform float _Float3;
            uniform float _Float4;
            uniform float _ToggleSwitch0;
            uniform float _Float0;
            uniform float _Float2;
            		float2 voronoihash33( float2 p )
            		{
            			
            			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
            			return frac( sin( p ) *43758.5453);
            		}
            
            		float voronoi33( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
            		{
            			float2 n = floor( v );
            			float2 f = frac( v );
            			float F1 = 8.0;
            			float F2 = 8.0; float2 mg = 0;
            			for ( int j = -1; j <= 1; j++ )
            			{
            				for ( int i = -1; i <= 1; i++ )
            			 	{
            			 		float2 g = float2( i, j );
            			 		float2 o = voronoihash33( n + g );
            					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            					float d = 0.707 * sqrt(dot( r, r ));
            			 		if( d<F1 ) {
            			 			F2 = F1;
            			 			F1 = d; mg = g; mr = r; id = o;
            			 		} else if( d<F2 ) {
            			 			F2 = d;
            			
            			 		}
            			 	}
            			}
            			return F1;
            		}
            
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
            


            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float4 ase_positionCS = UnityObjectToClipPos( v.vertex );
                float4 screenPos = ComputeScreenPos( ase_positionCS );
                OUT.ase_texcoord3 = screenPos;
                

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float4 color58 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
                float2 uv_Texture0 = IN.texcoord.xy * _Texture0_ST.xy + _Texture0_ST.zw;
                float time33 = 11.64;
                float2 voronoiSmoothId33 = 0;
                float2 normalizeResult40 = normalize( float2( -1,-1 ) );
                float2 temp_output_35_0 = (IN.texcoord.xy*1.0 + ( ( ( _Time.y * _Float3 ) / 2.0 ) * normalizeResult40 ));
                float2 coords33 = temp_output_35_0 * 5.0;
                float2 id33 = 0;
                float2 uv33 = 0;
                float voroi33 = voronoi33( coords33, time33, id33, uv33, 0, voronoiSmoothId33 );
                float simplePerlin2D43 = snoise( temp_output_35_0*-1.68 );
                simplePerlin2D43 = simplePerlin2D43*0.5 + 0.5;
                float smoothstepResult52 = smoothstep( 0.35 , 1.0 , ( ( tex2D( _Texture0, uv_Texture0 ).r * _Float5 ) + ( voroi33 * simplePerlin2D43 * 0.5 ) ));
                float4 screenPos = IN.ase_texcoord3;
                float4 appendResult13 = (float4(screenPos.x , screenPos.y , 0.0 , 0.0));
                float cos14 = cos( _Float2 );
                float sin14 = sin( _Float2 );
                float2 rotator14 = mul( ( appendResult13 * ( _Float0 / 0.2 ) ).xy - float2( 0,0 ) , float2x2( cos14 , -sin14 , sin14 , cos14 )) + float2( 0,0 );
                float2 uv15 = 0;
                float3 unityVoronoy15 = UnityVoronoi(rotator14,0.0,2.0,uv15);
                float temp_output_65_0 = ( 1.0 - step( pow( smoothstepResult52 , _Float4 ) , (( _ToggleSwitch0 )?( (1.0 + (unityVoronoy15.x - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ):( unityVoronoy15.x )) ) );
                float4 lerpResult55 = lerp( float4( color58.rgb , 0.0 ) , _Color1 , temp_output_65_0);
                clip( 0.01 - 0.01);
                float4 appendResult59 = (float4(lerpResult55.rgb , temp_output_65_0));
                

                half4 color = appendResult59;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.TimeNode;39;-2096,224;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-2144,416;Inherit;False;Property;_Float3;火焰运动速度;2;0;Create;False;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;37;-1920,496;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;-1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-1872,320;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;40;-1696,528;Inherit;False;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;62;-1696,352;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;32;-2000,720;Inherit;False;1876;546.75;半调生成;11;4;19;13;20;18;11;10;7;14;15;63;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;34;-1520,288;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1472,432;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;4;-1952,784;Float;True;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-1264,1104;Float;False;Property;_Float0;波点密度;5;0;Create;False;0;0;0;False;0;False;3;0;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-1152,784;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-928,1088;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1456,-256;Inherit;True;Property;_Texture0;遮罩贴图;0;0;Create;False;0;0;0;False;0;False;d642e791fe7bcd748be4bae5d7d7e7c2;d642e791fe7bcd748be4bae5d7d7e7c2;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ScaleAndOffsetNode;35;-1248,288;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-848,768;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-768,1104;Inherit;False;Property;_Float2;波点旋转;6;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1136,-256;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;77;-1072,-16;Inherit;False;Property;_Float5;遮罩贴图强度;1;0;Create;False;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;33;-880,128;Inherit;True;0;1;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;11.64;False;2;FLOAT;5;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.NoiseGeneratorNode;43;-912,400;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;-1.68;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-688,432;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-592,176;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;14;-592,784;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-720,-144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-432,-96;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;15;-336,768;Inherit;True;0;0;1;0;1;False;1;True;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;73;-224,208;Inherit;False;Property;_Float4;波点强度;7;0;Create;False;0;0;0;False;0;False;1;0;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;61;-32,608;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;52;-192,-32;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.35;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;72;64,96;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;76;240,624;Inherit;False;Property;_ToggleSwitch0;波点是否翻转;4;0;Create;False;0;0;0;False;0;False;1;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;48;368,224;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;58;160,-304;Inherit;False;Constant;_Color2;Color 2;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;54;160,-112;Inherit;False;Property;_Color1;火焰颜色;3;0;Create;False;0;0;0;False;0;False;1,0.2705882,0.5921569,1;1,0.2705882,0.5921569,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;65;560,144;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;432,-176;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;80;768,144;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.01;False;2;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;-1424,880;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenParams;10;-1936,992;Inherit;True;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;7;-1632,1008;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;1056,48;Inherit;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1408,96;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;3;DifShader/SHD_UI_DotFire;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;True;True;False;5;False;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;7;False;_StencilComp;3;False;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;67;0;39;2
WireConnection;67;1;66;0
WireConnection;40;0;37;0
WireConnection;62;0;67;0
WireConnection;38;0;62;0
WireConnection;38;1;40;0
WireConnection;13;0;4;1
WireConnection;13;1;4;2
WireConnection;20;0;19;0
WireConnection;35;0;34;0
WireConnection;35;2;38;0
WireConnection;18;0;13;0
WireConnection;18;1;20;0
WireConnection;1;0;2;0
WireConnection;33;0;35;0
WireConnection;43;0;35;0
WireConnection;50;0;33;0
WireConnection;50;1;43;0
WireConnection;50;2;71;0
WireConnection;14;0;18;0
WireConnection;14;2;63;0
WireConnection;78;0;1;1
WireConnection;78;1;77;0
WireConnection;42;0;78;0
WireConnection;42;1;50;0
WireConnection;15;0;14;0
WireConnection;61;0;15;0
WireConnection;52;0;42;0
WireConnection;72;0;52;0
WireConnection;72;1;73;0
WireConnection;76;0;15;0
WireConnection;76;1;61;0
WireConnection;48;0;72;0
WireConnection;48;1;76;0
WireConnection;65;0;48;0
WireConnection;55;0;58;5
WireConnection;55;1;54;0
WireConnection;55;2;65;0
WireConnection;80;0;65;0
WireConnection;11;0;4;2
WireConnection;11;1;7;0
WireConnection;7;0;10;1
WireConnection;7;1;10;2
WireConnection;59;0;55;0
WireConnection;59;3;80;0
WireConnection;0;0;59;0
ASEEND*/
//CHKSM=894F3C6AB9FD545182D838AE82B74C97D429CE66