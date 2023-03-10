Shader "Custom/HueSaturation" {
    Properties{
        _Hue("Hue", Range(-180, 180)) = 0
        _Saturation("Saturation", Range(0, 5)) = 1
        _Invert("Invert", Range(0, 1)) = 0
        _Sign("Sign", Range(0, 1)) = 1
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
            #pragma surface surf Standard

            sampler2D _MainTex;
            //Hedeflenen renk tonu 
            float _Hue;
            //Hedeflenen renk doygunlugu 
            float _Saturation;
            float _Invert;
            float _Sign;

            float4 _MainTex_ST;

            struct Input {
                float2 uv_MainTex;
            };


            void surf(Input IN, inout SurfaceOutputStandard o) {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

                //Renk tonu ve doygunluk uygula
                float luma = dot(o.Albedo, float3(.299, .587, .114));
                float3 chroma = o.Albedo - float3(luma);

                //(1,1,1) etrafinda chrominance vektorunu dondur
                float s = sqrt(1.0 / 3.0) * sin(radians(_Hue)), c = cos(radians(_Hue)), b = (1.0 / 3.0) * (1.0 - c);
                chroma = float3x3(b + c, b - s, b + s,
                                  b + s, b + c, b - s,
                                  b - s, b + s, b + c) * chroma;

				
				//luma degerini ters cevirme (aydinlik ve karanlik bolgeler yer degisir)
                if (_Invert > 0.5) { luma = 1.0 - luma; }
                o.Albedo = float3(luma)+chroma * _Saturation * _Sign;

                o.Alpha = 1.0;
            }
            ENDCG
    }
        FallBack "Diffuse"
}
