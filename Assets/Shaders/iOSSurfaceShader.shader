Shader "Self/iOS Surface Shader"
{
	Properties
	{
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
					combine texture
				}
			}
		}
	}
}