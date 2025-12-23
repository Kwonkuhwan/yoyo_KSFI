Shader "Custom/FlowingWater3D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed", Float) = 1.0
        _FlowDirection ("Flow Direction", Vector) = (1, 0, 0, 0)
        _RotationAngle ("Rotation Angle (Degrees)", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FlowSpeed;
            float4 _FlowDirection;
            float _RotationAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // UV 회전 함수
            float2 RotateUV(float2 uv, float angle)
            {
                float rad = radians(angle);
                float cosA = cos(rad);
                float sinA = sin(rad);

                float2x2 rotationMatrix = float2x2(
                    cosA, -sinA,
                    sinA,  cosA
                );

                return mul(rotationMatrix, uv);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 시간 기반 UV 이동
                float2 flowOffset = _FlowDirection.xy * _FlowSpeed * _Time.y;

                // 기본 UV 이동
                float2 uv = i.uv + flowOffset;

                // 파이프 회전 처리
                uv = RotateUV(uv, _RotationAngle);

                // 텍스처 샘플링
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}