// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Test7-1" {
    Properties {
        //_Float ("Float", Float) = 0.0
        //_Range ("Range", Range(0.0, 1.0)) = 0.0
        //_Vector ("Vector", Vector) = (1, 1, 1, 1)
        _Color ("Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _Cutout ("Cutout", Range(-0.1, 1.1)) = 0.0
        _MainTex ("MainTex", 2D) = "black"
        _Speed ("Speed", Vector) = (1, 1, 0, 0)
        _Emiss ("Emiss", Float) = 1.0
    }
    SubShader {

        Tags { "Queue" = "Transparent" }

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutout;
            float4 _Speed;
            float _Emiss;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                //float4 color: COLOR;

            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;//´¢´æÆ÷
                //float2 pos_uv : TEXCOORD1;
                float3 normal_world : TEXCOORD1;
                float3 view_world : TEXCOORD2;
            };

            v2f vert(appdata v) {
                v2f o;
                //float4 pos_view = mul(UNITY_MATRIX_V, pos_world);
                //float4 pos_clip = mul(UNITY_MATRIX_P, pos_view);
                //o.pos = pos_clip;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal_world = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                float3 pos_world = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.view_world = normalize(_WorldSpaceCameraPos.xyz - pos_world);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            float4 frag(v2f i) : SV_TARGET {
                //half4 gradient = tex2D(_MainTex, i.uv + _Time.y * _Speed.xy).r;
                //half4 noise = tex2D(_MainTex,i.uv + _Time.y * _Speed.zw).r;
                //clip(gradient -noise - _Cutout);
                float3 normal_world = normalize(i.normal_world);
                float3 view_world = normalize(i.view_world);
                float NdotV = dot(normal_world,view_world);
                float rim = 1.0 - NdotV;
                return rim.xxxx;
                //half3 col = _Color.xyz * _Emiss;
                //half alpha = saturate(tex2D(_MainTex, i.uv).r * _Color.a * _Emiss);
                //return float4(col, alpha);
            }
            ENDCG
        }
    }
}