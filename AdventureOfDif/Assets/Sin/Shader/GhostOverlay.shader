Shader "DifShader/SHD_VFX_GhostOverlay"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        [HDR] _Color ("Ghost Color", Color) = (0, 0.5, 1, 1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

        // 关闭深度写入，开启标准透明混合
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                fixed4 color : COLOR; // 获取顶点颜色
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            // Photoshop 叠加混合逻辑函数
            float OverlayChannel(float base, float blend)
            {
                return (base < 0.5) ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 采样原始贴图
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // 2. 准备底色 (Base) 和 混合色 (Blend)
                // 底色是角色的原始颜色，混合色是脚本控制的 _Color
                float3 base = texColor.rgb;
                float3 blend = _Color.rgb;
                
                // 3. 逐通道执行叠加逻辑
                float3 result;
                result.r = OverlayChannel(base.r, blend.r);
                result.g = OverlayChannel(base.g, blend.g);
                result.b = OverlayChannel(base.b, blend.b);

                // 4. 处理 Alpha
                // 最终透明度 = 贴图采样Alpha * 顶点色Alpha (如果有) * 脚本控制的 _Color.a
                float finalAlpha = texColor.a * _Color.a * i.color.a;

                return fixed4(result, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
