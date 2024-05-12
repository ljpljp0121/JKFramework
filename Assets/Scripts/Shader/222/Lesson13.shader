

Shader "Unlit/Lesson13"
{
    Properties
    {
        _MyInt("MtInt",Int) = 1
        _MyFloat("MyFloat",Float) = 2.1
        _MyRange("MyRange",Range(1,5)) = 2
        _MyColor("MyColor",Color) = (0,0,0,0)
        _MyVector("MyVector",Vector) = (0,0,0,0)

        _My2D("My2D",2D) = ""{}
        _MyCube("MyCube",Cube) = ""{}
        _My3D("My3D",3D) = ""{}
    }
    SubShader
    {
        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _MyInt;
            float _MyFloat;
            sampler2D _My2D;
            fixed _MyRange;
            fixed4 _MyColor;
            float4 _MyVector;
            samplerCUBE _MyCube;
            sampler3D _My3D;

            struct a2v
            {
                //��������(����ģ�Ϳռ�)
                float4 vertex:POSITION;
                //���㷨��(����ģ�Ϳռ�)
                float3 normal:NORMAL;
                //��������
                float2 uv:TEXCOORD0;
            };


            struct v2f
            {
                //�ü��ռ��µ�����
                float4 position:SV_POSITION ;  
                //���㷨��(����ģ�Ϳռ�)
                float3 normal:NORMAL;
                //��������
                float2 uv:TEXCOORD0;
            };

            v2f vert(a2v data)
            {
                //��Ҫ���ݸ�ƬԪ��ɫ��������
                v2f v2fData;
                v2fData.position = UnityObjectToClipPos(data.vertex);
                v2fData.normal = data.normal;
                v2fData.uv = data.uv;
                return v2fData;
            }

            fixed4 frag(v2f data):SV_Target
            {
                fixed4 COLOR = tex2D(_My2D,data.uv);
                return COLOR;
            }
            ENDCG
        }
    }
}
