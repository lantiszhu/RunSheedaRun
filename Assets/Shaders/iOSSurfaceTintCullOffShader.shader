Shader "Self/iOS Surface Tint Cull Off Shader"
{
	Properties
	{
		_Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		//ZWrite Off

		Cull Off

		//Blend SrcAlpha OneMinusSrcAlpha

		//Tags {Queue=Transparent}
			
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					ConstantColor [_Color]
					combine texture * constant
				}
			}
		}
	}
}