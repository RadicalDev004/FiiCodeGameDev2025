Shader "Custom/StencilMaskURP"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1" }

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            ColorMask 0
            ZWrite Off
        }
    }
}
