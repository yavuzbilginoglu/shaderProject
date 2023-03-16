Shader "Custom/ColorExtractor" {
    Properties {
        _Color("Selected Color", Color) = (1,1,1,1)
        //_NewColor("New Color", Color) = (0,1,0,1)
        _Tolerance("Tolerance", Range(0,1)) = 0.5
        _Smoothness("Smoothness", Range(0,1)) = 0.0
        _MatchMode("Match Mode", Range(0,1)) = 0
        _Invert("Invert", Range(0,1)) = 0
        _Desaturate("Desaturate", Range(0,1)) = 0
        _MapAlpha("Map Alpha", Range(0,1)) = 0

		///dilate-
        _Size("Size", Range(1, 10)) = 3
        _Mixval("Mix Value", Range(0, 1)) = 0.5
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Opaque"}

            Pass {
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
                float4 _Color;
                float _Tolerance;
                float _Smoothness;
                float _MatchMode;
                float _Invert;
                float _Desaturate;
                float _MapAlpha;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float res;
                    fixed4 texColor = tex2D(_MainTex, i.uv);
					

                    if (_MatchMode < 0.5) {
                        res = length(_Color - texColor.rgb);
                    } else {
                        res = dot(normalize(_Color), normalize(texColor.rgb));
                        res = 1.0 - res * res * res;
                    }

                    res = 1.0 - smoothstep(_Tolerance - _Smoothness, _Tolerance + _Smoothness, res);
                    res = abs(_Invert - res);

                    if (_Desaturate > 0.5) {
                        texColor.rgb = lerp(texColor.rgb * 0.299 + texColor.rgb * 0.587 + texColor.rgb * 0.114, texColor.rgb, res);
                    }
                    else if (_MapAlpha < 0.5) {
                        texColor.rgb = float3(res, res, res);
                    }
                    if (_MapAlpha > 0.5) {
                    texColor.a = res;
                    }

                    return texColor;
                    }
                    ENDCG
            }
        }
        FallBack "Diffuse"
}
