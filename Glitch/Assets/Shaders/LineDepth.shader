Shader "Unlit/LineDepth"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Lighting Off
        ZWrite On
        ZTest LEqual
        Pass
        {
            Color [_Color]
        }
    }
}
