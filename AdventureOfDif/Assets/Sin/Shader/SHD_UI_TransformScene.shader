// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DifShader/SHD_UI_TransformScene"
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

        _FillValue("滑移参数(_FillValue)", Range( -2 , 2)) = 0.4789405
        _Float4("控制带宽参数", Range( 0 , 0.5)) = 0.1651324
        _Color1("主颜色", Color) = (0.1176471,0.7725491,0.9803922,1)
        _Texture0("细节贴图", 2D) = "white" {}
        _Color0("细节贴图颜色", Color) = (0.4431373,0.854902,0.9803922,1)
        _Float3("细节贴图密度", Range( 0 , 10)) = 3
        _Float0("波点密度", Range( 0 , 30)) = 12
        _Float2("波点旋转", Range( 0 , 1)) = 0
        _DetaiTex("中央圆圈贴图", 2D) = "black" {}
        _Color2("中央圆圈颜色", Color) = (1,1,1,0)
        _Float5("中央圆圈的大小", Range( 0 , 10)) = 1
        _Float6("中央圆圈的位移x", Range( -3 , 3)) = 0
        _Float7("中央圆圈的位移y", Range( -3 , 3)) = 0

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


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
            uniform float4 _Color0;
            uniform sampler2D _Texture0;
            uniform float _Float3;
            uniform sampler2D _DetaiTex;
            uniform float _Float5;
            uniform float _Float6;
            uniform float _Float7;
            uniform float4 _Color2;
            uniform float _FillValue;
            uniform float _Float4;
            uniform float _Float0;
            uniform float _Float2;
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

                float4 screenPos = IN.ase_texcoord3;
                float4 appendResult3 = (float4(screenPos.x , ( screenPos.y / ( _ScreenParams.x / _ScreenParams.y ) ) , 0.0 , 0.0));
                float4 ScreenUV47 = appendResult3;
                float4 appendResult35 = (float4(_Float3 , _Float3 , 0.0 , 0.0));
                float2 normalizeResult37 = normalize( float2( 1,-1 ) );
                float3 lerpResult44 = lerp( _Color1.rgb , _Color0.rgb , tex2D( _Texture0, ( ( ScreenUV47 * appendResult35 ) + float4( ( normalizeResult37 * _Time.y ), 0.0 , 0.0 ) ).xy ).r);
                float4 appendResult67 = (float4(_Float5 , _Float5 , 0.0 , 0.0));
                float4 appendResult71 = (float4(_Float6 , _Float7 , 0.0 , 0.0));
                float cos59 = cos( ( _Time.y * -1.0 ) );
                float sin59 = sin( ( _Time.y * -1.0 ) );
                float2 rotator59 = mul( ( ( ScreenUV47 * appendResult67 ) + appendResult71 ).xy - float2( 0.5,0.5 ) , float2x2( cos59 , -sin59 , sin59 , cos59 )) + float2( 0.5,0.5 );
                float4 tex2DNode62 = tex2D( _DetaiTex, rotator59 );
                float3 lerpResult63 = lerp( lerpResult44 , ( tex2DNode62.rgb * _Color2.rgb ) , tex2DNode62.a);
                float4 ase_positionSSNorm = screenPos / screenPos.w;
                ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
                float temp_output_14_0 = ( ( ase_positionSSNorm.x + ase_positionSSNorm.y ) / 2.0 );
                float smoothstepResult53 = smoothstep( ( _FillValue + ( _Float4 * -0.5 ) ) , ( _FillValue + ( _Float4 * 0.91 ) ) , temp_output_14_0);
                float cos7 = cos( _Float2 );
                float sin7 = sin( _Float2 );
                float2 rotator7 = mul( ( appendResult3 * ( _Float0 / 0.2 ) ).xy - float2( 0,0 ) , float2x2( cos7 , -sin7 , sin7 , cos7 )) + float2( 0,0 );
                float2 uv8 = 0;
                float3 unityVoronoy8 = UnityVoronoi(rotator7,0.0,2.0,uv8);
                float4 appendResult41 = (float4(lerpResult63 , step( smoothstepResult53 , unityVoronoy8.x )));
                

                half4 color = appendResult41;

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
Node;AmplifyShaderEditor.CommentaryNode;12;-3200,496;Inherit;False;1876;543.95;波点生成;12;1;2;3;4;5;6;7;9;10;11;8;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScreenParams;10;-3136,768;Inherit;True;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenPosInputsNode;1;-3136,560;Float;True;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;-2832,784;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;9;-2608,672;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;77;-3200,-1984;Inherit;False;1513.32;698.8;中央圆圈;14;66;57;67;72;73;61;65;71;69;76;60;59;62;78;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-2352,576;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;56;-3200,-1232;Inherit;False;1927.49;938.8;细节贴图混合;15;26;34;29;35;37;48;30;32;31;23;36;38;40;44;42;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-2144,768;Inherit;False;ScreenUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-3120,-1760;Inherit;False;Property;_Float5;中央圆圈的大小;10;0;Create;False;0;0;0;False;0;False;1;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;26;-3056,-624;Inherit;False;Constant;_运动方向;运动方向;4;0;Create;True;0;0;0;False;0;False;1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;34;-3152,-784;Inherit;False;Property;_Float3;细节贴图密度;5;0;Create;False;0;0;0;False;0;False;3;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-2800,-1872;Inherit;False;47;ScreenUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;67;-2816,-1792;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-3152,-1664;Inherit;False;Property;_Float6;中央圆圈的位移x;11;0;Create;False;0;0;0;False;0;False;0;0;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-3152,-1552;Inherit;False;Property;_Float7;中央圆圈的位移y;12;0;Create;False;0;0;0;False;0;False;0;0;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2464,880;Float;False;Property;_Float0;波点密度;6;0;Create;False;0;0;0;False;0;False;12;0;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;55;-3200,-224;Inherit;False;1277.91;682.9;Comment;11;13;49;50;15;54;16;14;51;52;53;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-2816,-816;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalizeNode;37;-2784,-640;Inherit;False;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-3072,-1120;Inherit;False;47;ScreenUV;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TimeNode;29;-2816,-480;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;61;-2816,-1472;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2608,-1776;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-2816,-1632;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;4;-2128,864;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;13;-3088,-112;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;49;-3136,240;Inherit;False;Property;_Float4;控制带宽参数;1;0;Create;False;0;0;0;False;0;False;0.1651324;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2496,-784;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2560,-896;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-2576,-1472;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;-2448,-1632;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2048,544;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1968,880;Inherit;False;Property;_Float2;波点旋转;7;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2816,320;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-2704,-176;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2816,208;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.91;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-2288,-944;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;23;-2512,-1184;Inherit;True;Property;_Texture0;细节贴图;3;0;Create;False;0;0;0;False;0;False;f9acb734f08d8d848944ccbe353ae224;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RotatorNode;59;-2288,-1616;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;60;-2384,-1936;Inherit;True;Property;_DetaiTex;中央圆圈贴图;8;0;Create;False;0;0;0;False;0;False;c90d2cb30dcdb7b4aa623328305e6272;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;16;-3136,128;Inherit;False;Property;_FillValue;滑移参数(_FillValue);0;0;Create;False;0;0;0;False;0;False;0.4789405;0;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;7;-1792,560;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;14;-2544,-144;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-2592,224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-2592,112;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-2096,-1040;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;38;-2080,-592;Inherit;False;Property;_Color0;细节贴图颜色;4;0;Create;False;0;0;0;False;0;False;0.4431373,0.854902,0.9803922,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;40;-2096,-784;Inherit;False;Property;_Color1;主颜色;2;0;Create;False;0;0;0;False;0;False;0.1176471,0.7725491,0.9803922,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;62;-2000,-1856;Inherit;True;Property;_TextureSample1;Texture Sample 1;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;78;-2032,-1616;Inherit;False;Property;_Color2;中央圆圈颜色;9;0;Create;False;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.VoronoiNode;8;-1536,544;Inherit;True;0;0;1;0;1;False;1;True;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SmoothstepOpNode;53;-2160,80;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;-1536,-928;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1760,-1632;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;20;-736,-32;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;63;-1152,-1312;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;45;-1200,512;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;-466.2703,-440.7343;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-2192,-176;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;42;-3088,-1040;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;32,-240;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;3;DifShader/SHD_UI_TransformScene;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;11;0;10;1
WireConnection;11;1;10;2
WireConnection;9;0;1;2
WireConnection;9;1;11;0
WireConnection;3;0;1;1
WireConnection;3;1;9;0
WireConnection;47;0;3;0
WireConnection;67;0;66;0
WireConnection;67;1;66;0
WireConnection;35;0;34;0
WireConnection;35;1;34;0
WireConnection;37;0;26;0
WireConnection;65;0;57;0
WireConnection;65;1;67;0
WireConnection;71;0;72;0
WireConnection;71;1;73;0
WireConnection;4;0;2;0
WireConnection;30;0;37;0
WireConnection;30;1;29;2
WireConnection;32;0;48;0
WireConnection;32;1;35;0
WireConnection;69;0;61;2
WireConnection;76;0;65;0
WireConnection;76;1;71;0
WireConnection;5;0;3;0
WireConnection;5;1;4;0
WireConnection;50;0;49;0
WireConnection;15;0;13;1
WireConnection;15;1;13;2
WireConnection;54;0;49;0
WireConnection;31;0;32;0
WireConnection;31;1;30;0
WireConnection;59;0;76;0
WireConnection;59;2;69;0
WireConnection;7;0;5;0
WireConnection;7;2;6;0
WireConnection;14;0;15;0
WireConnection;51;0;16;0
WireConnection;51;1;50;0
WireConnection;52;0;16;0
WireConnection;52;1;54;0
WireConnection;36;0;23;0
WireConnection;36;1;31;0
WireConnection;62;0;60;0
WireConnection;62;1;59;0
WireConnection;8;0;7;0
WireConnection;53;0;14;0
WireConnection;53;1;51;0
WireConnection;53;2;52;0
WireConnection;44;0;40;5
WireConnection;44;1;38;5
WireConnection;44;2;36;1
WireConnection;79;0;62;5
WireConnection;79;1;78;5
WireConnection;20;0;53;0
WireConnection;20;1;8;0
WireConnection;63;0;44;0
WireConnection;63;1;79;0
WireConnection;63;2;62;4
WireConnection;45;0;8;0
WireConnection;41;0;63;0
WireConnection;41;3;20;0
WireConnection;19;0;14;0
WireConnection;0;0;41;0
ASEEND*/
//CHKSM=D2A9695D65A4CCF0BF623E87C8EC06B0C16AA0ED