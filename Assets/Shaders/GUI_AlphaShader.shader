Shader "GUI/Alpha Shader"
{
	Properties
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off
		
		Cull Back
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		Fog {Mode Off}
		
		Tags { Queue = Transparent }

		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					combine texture
				}
			}
		}
	}
}