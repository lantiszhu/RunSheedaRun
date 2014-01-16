Shader "Self/Surface Shader (Fog Off)"
{
	Properties
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		Cull Back
		
		Fog {Mode Off}

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