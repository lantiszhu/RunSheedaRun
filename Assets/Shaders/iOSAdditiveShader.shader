Shader "Self/iOS Simple Additive Shader"
{
	Properties
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off
		
		Cull Back
		
		Blend One One
		
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