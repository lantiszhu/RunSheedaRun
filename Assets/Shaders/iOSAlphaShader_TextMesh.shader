Shader "Self/iOS Alpha Shader TextMesh"
{
	Properties
	{
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off
		
		Cull Back
		
		Blend OneMinusSrcAlpha SrcAlpha 
		
		Tags { Queue = Transparent }

		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					combine one-texture
				}
			}
		}
	}
}