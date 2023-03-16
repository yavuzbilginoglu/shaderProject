
Shader "Custom/dilate" {
    Properties{

        ///dilate-
        _Size("Size", Range(1, 10)) = 3
        _Mixval("Mix Value", Range(0, 1)) = 0.5
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Opaque"}

        Pass{
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Size;
            float _MixVal;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                float3 center = tex2D(_MainTex, uv).rgb;
                float3 vmin = center;
                float3 vmax = center;
                float rend = ceil(_Size * 0.5);
                for (float dist = -floor(_Size * 0.5); dist <= rend; dist += 1.0) {
                    float2 offset = dist * float2(1.0, 0.0);
                    float3 sample = tex2D(_MainTex, uv + offset).rgb;
                    vmin = min(vmin, sample);
                    vmax = max(vmax, sample);
                }
                for (float dist = -floor(_Size * 0.5); dist <= rend; dist += 1.0) {
                    float2 offset = dist * float2(0.0, 1.0);
                    float3 sample = tex2D(_MainTex, uv + offset).rgb;
                    vmin = min(vmin, sample);
                    vmax = max(vmax, sample);
                }
                return fixed4(vmin * (1 - _MixVal) + vmax * _MixVal, 1.0);
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
    }