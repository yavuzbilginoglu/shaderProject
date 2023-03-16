Shader "Custom/TwoPassShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Range(1, 10)) = 3
    }

        SubShader{
            // First Pass
            Pass {
                ZTest Always Cull Off ZWrite Off
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _BlurSize;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float sum = 0.0;
                    float offset = 1.0 / (_ScreenParams.z * _BlurSize * 2 + 1);
                    for (int j = -_BlurSize; j <= _BlurSize; j++) {
                        sum += tex2D(_MainTex, i.uv + float2(j * offset, 0)) * offset;
                    }
                    return sum;
                }
                ENDCG
            }

            // Second Pass
            Pass {
                ZTest Always Cull Off ZWrite Off
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _BlurSize;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float sum = 0.0;
                    float offset = 1.0 / (_ScreenParams.w * _BlurSize * 2 + 1);
                    for (int j = -_BlurSize; j <= _BlurSize; j++) {
                        sum += tex2D(_MainTex, i.uv + float2(0, j * offset)) * offset;
                    }
                    return sum;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
