Shader "Self/iOS Additive Blending Tint Shader"
{
	Properties
	{
		_Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
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
					ConstantColor [_Color]
					combine texture + constant
				}
			}
		}
	}
}