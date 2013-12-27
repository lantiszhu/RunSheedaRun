Shader "Self/iOS Alpha Shader Cull Off"
{
	Properties
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off
		
		Cull off
		
		Blend SrcAlpha OneMinusSrcAlpha
		
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