Shader "Custom/AlwaysOnTopParticles"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }  
        ZWrite Off 
        Blend SrcAlpha OneMinusSrcAlpha 
        Pass
        {
            SetTexture [_MainTex] { combine texture }
        }
    }
}
