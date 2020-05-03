Shader "Unlit/HeightColor"
{
    Properties
    {
        _ColorBottom ("Bottom Color", Color) = (0, 0, 0, 0)
        _ColorTop ("Top Color", Color) = (1, 1, 1, 1) 
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 worldPos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 svPos : SV_POSITION;
            };

            fixed4 _ColorBottom, _ColorTop;

            v2f vert (float4 localPos : POSITION)
            {
                v2f o;

                o.svPos = UnityObjectToClipPos(localPos);
                o.worldPos = mul(unity_ObjectToWorld, localPos);

                UNITY_TRANSFER_FOG(o,o.svPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = lerp(_ColorBottom, _ColorTop, i.worldPos.y / 10);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
