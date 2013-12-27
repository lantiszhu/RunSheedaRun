Shader "Self/iOS LightMapped Shader"
{
	Properties
	{
//		_Color ( "Main Color", Color ) = ( 1, 1, 1, 1 )
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_LightMap ( "Lightmap (RGB)", 2D ) = "lightmap" { LightmapMode }
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Name "BASE"
			Tags {"LightMode" = "Always"}
			
			BindChannels
			{
				Bind "Vertex", vertex
				Bind "texcoord1", texcoord0
			}
			
			SetTexture [_LightMap]
//			{
//				constantColor [_Color]
//				combine texture * constant
//			}
		}
	
		Pass
		{
			Name "BASE"
			Tags {"LightMode" = "Always"}
			
			BindChannels
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
			}
			
			Blend Zero SrcColor
			SetTexture [_MainTex]
			{
				combine texture
			}
		}
	}

	Fallback "VertexLit"
}