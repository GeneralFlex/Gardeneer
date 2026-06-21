Shader "Custom/PixelatedCircle"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0,1)) = 0.35
        _PixelSize ("Pixel Size", Range(0.001,0.1)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float4 _Color;
            float _Radius;
            float _PixelSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Center UVs so (0,0) is middle of quad
                float2 uv = i.uv - 0.5;

                // Snap to pixel grid (pixel centers)
                uv = (floor(uv / _PixelSize) + 0.5) * _PixelSize;

                // Distance from center
                float dist = length(uv);

                // Hard pixel edge
                float alpha = step(dist, _Radius);

                return float4(_Color.rgb, alpha * _Color.a);
            }

            ENDCG
        }
    }
}