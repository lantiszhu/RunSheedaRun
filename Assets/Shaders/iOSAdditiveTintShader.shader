Shader "Self/iOS Additive Tint Shader"
{
	Properties
	{
		_Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		Cull Back
		
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