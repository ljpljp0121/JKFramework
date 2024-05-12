// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Test5_2"
{
    Properties
    {
        _MyColor("MyColor",Color) = (1.0,1.0,1.0,1.0)
    }

    SubShader
    {
         
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _MyColor;  

            struct a2v
            {
                float4 vertex :POSITION;                
                float3 normal:NORMAL;
                float4 texcoord : TEXCOORD;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;                
                float3 color :COLOR;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.normal *0.5 + fixed3(0.5,0.5,0.5);
                return o;
            }

            fixed4 frag( v2f i):SV_Target
            {
                fixed3 c = i.color;
                c*= _MyColor.rgba;
                return fixed4(c,1.0);
            }
            ENDCG
        }
    }
}
