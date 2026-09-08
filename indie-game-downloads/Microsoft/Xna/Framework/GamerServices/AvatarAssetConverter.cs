#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.XboxLive.Avatars.Internal;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarAssetConverter
{
	private static int[] TextureUsagePriority = new int[13]
	{
		0, 0, 1, 2, 3, 4, 11, 9, 10, 7,
		8, 6, 5
	};

	private GraphicsDevice graphicsDevice;

	public AvatarAssetConverter(GraphicsDevice graphicsDevice)
	{
		this.graphicsDevice = graphicsDevice;
	}

	private ColorParameters ExtractColorParameters(ShaderInstance shader)
	{
		ColorParameters result = default(ColorParameters);
		ShaderParameter[] shaderParameters = shader.ShaderParameters;
		for (int i = 0; i < shaderParameters.Length; i++)
		{
			ShaderParameter shaderParameter = shaderParameters[i];
			if (shaderParameter.type == ShaderParameterType.PixelConstant)
			{
				Vector3 vector = new Vector3(shaderParameter.data.Constant.f0, shaderParameter.data.Constant.f1, shaderParameter.data.Constant.f2);
				switch (shaderParameter.usage)
				{
				case ShaderParameterUsage.PixelConstantColorCustom0:
					result.Custom0 = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorCustom1:
					result.Custom1 = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorCustom2:
					result.Custom2 = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorEyebrow:
					result.EyebrowTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorEyeShadow:
					result.EyeShadowTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorFacialHair:
					result.FacialHairTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorHair:
					result.HairTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorIris:
					result.IrisTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorMouth:
					result.MouthTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorRimLight:
					result.RimLightColor = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorSkin:
					result.SkinTone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorSkinFeature1:
					result.SkinFeature1Tone = vector;
					break;
				case ShaderParameterUsage.PixelConstantColorSkinFeature2:
					result.SkinFeature2Tone = vector;
					break;
				case ShaderParameterUsage.PixelConstantReflectivity:
					result.Reflectivity = vector;
					break;
				case ShaderParameterUsage.PixelConstantTransparency:
					result.Transparency = vector;
					break;
				}
			}
		}
		return result;
	}

	private static void SortTextureLayers(ShaderParameter[] shaderParams)
	{
		int num = shaderParams.Length;
		for (int i = 2; i <= num; i++)
		{
			int num2 = i;
			while (num2-- > 1 && shaderParams[num2].type == ShaderParameterType.Texture && shaderParams[num2 - 1].type == ShaderParameterType.Texture && TextureUsagePriority[(int)shaderParams[num2 - 1].usage] >= TextureUsagePriority[(int)shaderParams[num2].usage])
			{
				ShaderParameter shaderParameter = shaderParams[num2];
				ref ShaderParameter reference = ref shaderParams[num2];
				reference = shaderParams[num2 - 1];
				shaderParams[num2 - 1] = shaderParameter;
			}
		}
	}

	private VertexBuffer CreateTexCoordBuffer(Microsoft.XboxLive.MathUtilities.Vector2[] texCoords)
	{
		VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(TexCoordVertex), texCoords.Length, BufferUsage.WriteOnly);
		vertexBuffer.SetData(texCoords);
		return vertexBuffer;
	}

	private DynamicVertexBuffer CreateDynamicTexCoordBuffer(int vertexCount)
	{
		return new DynamicVertexBuffer(graphicsDevice, typeof(TexCoordVertex), vertexCount, BufferUsage.WriteOnly);
	}

	private VertexBuffer CreateTexCoordBuffer2(Microsoft.XboxLive.MathUtilities.Vector2[] texCoords)
	{
		VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(TexCoordVertex2), texCoords.Length, BufferUsage.WriteOnly);
		vertexBuffer.SetData(texCoords);
		return vertexBuffer;
	}

	private SamplerState CreateSamplerState(TextureWrapModes wrapModeFlags)
	{
		SamplerState samplerState = new SamplerState();
		samplerState.AddressU = (((wrapModeFlags & TextureWrapModes.WrapU) <= TextureWrapModes.None) ? TextureAddressMode.Clamp : TextureAddressMode.Wrap);
		samplerState.AddressV = (((wrapModeFlags & TextureWrapModes.WrapV) <= TextureWrapModes.None) ? TextureAddressMode.Clamp : TextureAddressMode.Wrap);
		samplerState.AddressW = TextureAddressMode.Wrap;
		samplerState.Filter = TextureFilter.Linear;
		samplerState.MaxAnisotropy = 0;
		samplerState.MaxMipLevel = 0;
		samplerState.MipMapLevelOfDetailBias = 0f;
		return samplerState;
	}

	private void SetTextureColorDiffuseEffectAlpha(AvatarRenderPass targetPass, Texture2D texture, Vector3 diffuseColor)
	{
		AlphaTestEffect alphaTestEffect = (AlphaTestEffect)(targetPass.effect = new AlphaTestEffect(graphicsDevice));
		targetPass.textures = new Texture2D[1];
		targetPass.textures[0] = texture;
		alphaTestEffect.Texture = texture;
		alphaTestEffect.AlphaFunction = CompareFunction.Greater;
		alphaTestEffect.ReferenceAlpha = 128;
		alphaTestEffect.VertexColorEnabled = true;
		alphaTestEffect.DiffuseColor = diffuseColor;
	}

	private void SetTextureEffect(AvatarRenderPass targetPass, Texture2D texture)
	{
		BasicEffect basicEffect = (BasicEffect)(targetPass.effect = new BasicEffect(graphicsDevice));
		targetPass.textures = new Texture2D[1];
		targetPass.textures[0] = texture;
		basicEffect.TextureEnabled = true;
		basicEffect.Texture = texture;
		basicEffect.VertexColorEnabled = true;
	}

	private void SetTextureColorDiffuseEffect(AvatarRenderPass targetPass, Texture2D texture, Vector3 diffuseColor)
	{
		BasicEffect basicEffect = (BasicEffect)(targetPass.effect = new BasicEffect(graphicsDevice));
		targetPass.textures = new Texture2D[1];
		targetPass.textures[0] = texture;
		basicEffect.TextureEnabled = true;
		basicEffect.Texture = texture;
		basicEffect.VertexColorEnabled = true;
		basicEffect.DiffuseColor = diffuseColor;
	}

	private void SetTextureColorDiffuseEffect(AvatarRenderPass targetPass, Texture2D[] textures, Vector3 diffuseColor)
	{
		BasicEffect basicEffect = (BasicEffect)(targetPass.effect = new BasicEffect(graphicsDevice));
		targetPass.textures = textures;
		basicEffect.TextureEnabled = true;
		basicEffect.Texture = textures[0];
		basicEffect.VertexColorEnabled = true;
		basicEffect.DiffuseColor = diffuseColor;
	}

	private void SetColorDiffuseEffect(AvatarRenderPass targetPass, Vector3 diffuseColor)
	{
		BasicEffect basicEffect = (BasicEffect)(targetPass.effect = new BasicEffect(graphicsDevice));
		basicEffect.TextureEnabled = false;
		basicEffect.VertexColorEnabled = true;
		basicEffect.DiffuseColor = diffuseColor;
	}

	private void SetDualTextureEffect(AvatarRenderPass targetPass, Texture2D texture1, Texture2D texture2, Vector3 diffuseColor)
	{
		DualTextureEffect dualTextureEffect = (DualTextureEffect)(targetPass.effect = new DualTextureEffect(graphicsDevice));
		targetPass.textures = new Texture2D[2];
		targetPass.textures[0] = texture1;
		targetPass.textures[1] = texture2;
		dualTextureEffect.Texture = texture1;
		dualTextureEffect.Texture2 = texture2;
		dualTextureEffect.DiffuseColor = diffuseColor;
		dualTextureEffect.VertexColorEnabled = true;
	}

	private DualTextureEffect CreateReflection(Texture2D reflection, Texture2D mask, float reflectivity)
	{
		DualTextureEffect dualTextureEffect = new DualTextureEffect(graphicsDevice);
		dualTextureEffect.Texture = reflection;
		dualTextureEffect.Texture2 = mask;
		dualTextureEffect.DiffuseColor = new Vector3(0.5f * reflectivity);
		dualTextureEffect.VertexColorEnabled = false;
		return dualTextureEffect;
	}

	private BasicEffect CreateReflection(Texture2D reflection, float reflectivity)
	{
		BasicEffect basicEffect = new BasicEffect(graphicsDevice);
		basicEffect.Texture = reflection;
		basicEffect.DiffuseColor = new Vector3(reflectivity);
		basicEffect.VertexColorEnabled = false;
		return basicEffect;
	}

	private SurfaceFormat GetSurfaceFormat(TextureDataFormat textureFormat)
	{
		return (textureFormat & (TextureDataFormat)(-9)) switch
		{
			TextureDataFormat.RGBA => SurfaceFormat.Color, 
			TextureDataFormat.Dxt1 => SurfaceFormat.Dxt1, 
			TextureDataFormat.Dxt3 => SurfaceFormat.Dxt3, 
			TextureDataFormat.Dxt5 => SurfaceFormat.Dxt5, 
			_ => throw new Exception("Internal error: Unknown / unsupported texture format."), 
		};
	}

	private bool CheckTextureTransparent(AnimatedTexture texture)
	{
		if (texture == null)
		{
			return true;
		}
		TextureDataFormat rawDataFormat = texture.RawDataFormat;
		if ((rawDataFormat & TextureDataFormat.Empty) != TextureDataFormat.Dxt1)
		{
			return texture.IsEmptyTransparent;
		}
		switch (texture.RawDataFormat)
		{
		case TextureDataFormat.Dxt1:
		{
			byte[] rawData2 = texture.GetRawData();
			int num2 = rawData2.Length;
			for (int j = 0; j < num2; j += 8)
			{
				ushort num3 = (ushort)((rawData2[j + 1] << 8) | rawData2[j]);
				ushort num4 = (ushort)((rawData2[j + 3] << 8) | rawData2[j + 2]);
				if (num3 > num4)
				{
					return false;
				}
				if ((rawData2[j + 4] & rawData2[j + 5] & rawData2[j + 6] & rawData2[j + 7]) != 255)
				{
					return false;
				}
			}
			return true;
		}
		case TextureDataFormat.Dxt4:
		case TextureDataFormat.Dxt5:
		{
			byte[] rawData = texture.GetRawData();
			int num = rawData.Length;
			for (int i = 0; i < num; i += 16)
			{
				byte b = rawData[i];
				byte b2 = rawData[i + 1];
				if (b <= b2)
				{
					if (b2 == 0)
					{
						if ((rawData[i + 2] | rawData[i + 3] | rawData[i + 4] | rawData[i + 5] | rawData[i + 6] | rawData[i + 7]) != 0)
						{
							return false;
						}
						continue;
					}
					if (rawData[i + 2] != 182)
					{
						return false;
					}
					if (rawData[i + 3] != 109)
					{
						return false;
					}
					if (rawData[i + 4] != 219)
					{
						return false;
					}
					if (rawData[i + 5] != 182)
					{
						return false;
					}
					if (rawData[i + 6] != 109)
					{
						return false;
					}
					if (rawData[i + 7] != 219)
					{
						return false;
					}
				}
				else
				{
					if (b2 != 0)
					{
						return false;
					}
					if (rawData[i + 2] != 73)
					{
						return false;
					}
					if (rawData[i + 3] != 146)
					{
						return false;
					}
					if (rawData[i + 4] != 36)
					{
						return false;
					}
					if (rawData[i + 5] != 73)
					{
						return false;
					}
					if (rawData[i + 6] != 146)
					{
						return false;
					}
					if (rawData[i + 7] != 36)
					{
						return false;
					}
				}
			}
			return true;
		}
		default:
			return false;
		}
	}

	private Texture2D CreateTexture(AnimatedTexture avatarTexture)
	{
		TextureDataFormat textureDataFormat = avatarTexture.RawDataFormat & (TextureDataFormat)(-9);
		if (textureDataFormat == TextureDataFormat.RGBA)
		{
			return CreatePow2Texture(avatarTexture.Width, avatarTexture.Height, avatarTexture.GetPixels());
		}
		return CreatePow2Texture(avatarTexture.Width, avatarTexture.Height, avatarTexture.RawDataFormat, avatarTexture.GetRawData());
	}

	private Texture2D CreateTexturePremultiplied(IBaseTextureAnimated avatarTexture)
	{
		int width = avatarTexture.Width;
		int height = avatarTexture.Height;
		int num = width * height;
		Colorb[] pixels = avatarTexture.GetPixels();
		Colorb[] array = new Colorb[num];
		for (int i = 0; i < num; i++)
		{
			uint alpha = pixels[i].alpha;
			array[i].alpha = (byte)alpha;
			array[i].red = (byte)(pixels[i].red * alpha / 255);
			array[i].green = (byte)(pixels[i].green * alpha / 255);
			array[i].blue = (byte)(pixels[i].blue * alpha / 255);
		}
		return CreatePow2Texture(width, height, array);
	}

	private byte[] TransformIntensityMap(AnimatedTexture avatarTexture, Vector3 colorParam1, Vector3 colorParam2, Vector3 colorParam3)
	{
		Color color = new Color(colorParam1.X, colorParam1.Y, colorParam1.Z);
		Color color2 = new Color(colorParam2.X, colorParam2.Y, colorParam2.Z);
		Color color3 = new Color(colorParam3.X, colorParam3.Y, colorParam3.Z);
		SurfaceFormat surfaceFormat = GetSurfaceFormat(avatarTexture.RawDataFormat);
		Debug.Assert(surfaceFormat == SurfaceFormat.Dxt1 || surfaceFormat == SurfaceFormat.Dxt5);
		int num = ((surfaceFormat == SurfaceFormat.Dxt1) ? 8 : 16);
		int num2 = (avatarTexture.Width + 3 >> 2) * (avatarTexture.Height + 3 >> 2);
		int num3 = num * num2;
		byte[] rawData = avatarTexture.GetRawData();
		byte[] array = new byte[rawData.Length];
		rawData.CopyTo(array, 0);
		for (int i = num - 8; i < num3; i += num)
		{
			int num4 = array[i];
			int num5 = array[i + 1];
			int num6 = array[i + 2];
			int num7 = array[i + 3];
			int num8 = ((num4 & 0x1F) << 1) | ((num4 >> 4) & 1);
			int num9 = (((num5 << 8) | num4) & 0x7E0) >> 5;
			int num10 = ((num5 & 0xF8) >> 2) | ((num5 >> 7) & 1);
			int num11 = (color.R * num10 + color2.R * num9 + color3.R * num8 + 127) / 255;
			int num12 = (color.G * num10 + color2.G * num9 + color3.G * num8 + 127) / 255;
			int num13 = (color.B * num10 + color2.B * num9 + color3.B * num8 + 127) / 255;
			if (num11 > 63)
			{
				num11 = 63;
			}
			if (num12 > 63)
			{
				num12 = 63;
			}
			if (num13 > 63)
			{
				num13 = 63;
			}
			array[i] = (byte)((num12 << 5) | (num13 >> 1));
			array[i + 1] = (byte)(((num11 << 2) & 0xF8) | (num12 >> 3));
			num8 = ((num6 & 0x1F) << 1) | ((num6 >> 4) & 1);
			num9 = (((num7 << 8) | num6) & 0x7E0) >> 5;
			num10 = ((num7 & 0xF8) >> 2) | ((num7 >> 7) & 1);
			num11 = (color.R * num10 + color2.R * num9 + color3.R * num8 + 127) / 255;
			num12 = (color.G * num10 + color2.G * num9 + color3.G * num8 + 127) / 255;
			num13 = (color.B * num10 + color2.B * num9 + color3.B * num8 + 127) / 255;
			if (num11 > 63)
			{
				num11 = 63;
			}
			if (num12 > 63)
			{
				num12 = 63;
			}
			if (num13 > 63)
			{
				num13 = 63;
			}
			array[i + 2] = (byte)((num12 << 5) | (num13 >> 1));
			array[i + 3] = (byte)(((num11 << 2) & 0xF8) | (num12 >> 3));
		}
		if (surfaceFormat == SurfaceFormat.Dxt1)
		{
			for (int j = 0; j < num3; j += num)
			{
				byte b = array[j];
				byte b2 = array[j + 1];
				byte b3 = array[j + 2];
				byte b4 = array[j + 3];
				ushort num14 = (ushort)((rawData[j + 1] << 8) | rawData[j]);
				ushort num15 = (ushort)((rawData[j + 3] << 8) | rawData[j + 2]);
				bool flag = num14 <= num15;
				num14 = (ushort)((b2 << 8) | b);
				num15 = (ushort)((b4 << 8) | b3);
				bool flag2 = num14 <= num15;
				if (flag2 != flag)
				{
					if (flag)
					{
						array[j] = b3;
						array[j + 1] = b4;
						array[j + 2] = b;
						array[j + 3] = b2;
						int num16 = array[j + 4];
						array[j + 4] = (byte)(num16 ^ ((~num16 & 0xAA) >> 1));
						num16 = array[j + 5];
						array[j + 5] = (byte)(num16 ^ ((~num16 & 0xAA) >> 1));
						num16 = array[j + 6];
						array[j + 6] = (byte)(num16 ^ ((~num16 & 0xAA) >> 1));
						num16 = array[j + 7];
						array[j + 7] = (byte)(num16 ^ ((~num16 & 0xAA) >> 1));
					}
					else if (num14 == num15)
					{
						array[j + 4] = 0;
						array[j + 5] = 0;
						array[j + 6] = 0;
						array[j + 7] = 0;
					}
					else
					{
						array[j] = b3;
						array[j + 1] = b4;
						array[j + 2] = b;
						array[j + 3] = b2;
						array[j + 4] ^= byte.MaxValue;
						array[j + 5] ^= byte.MaxValue;
						array[j + 6] ^= byte.MaxValue;
						array[j + 7] ^= byte.MaxValue;
					}
				}
			}
		}
		return array;
	}

	private AnimatedTexture CreateIntensityTexture(AnimatedTexture avatarTexture, Vector3 colorParam1, Vector3 colorParam2, Vector3 colorParam3)
	{
		byte[] array = TransformIntensityMap(avatarTexture, colorParam1, colorParam2, colorParam3);
		if (array == null)
		{
			return null;
		}
		DxtDecoder dxtDecoder = new DxtDecoder();
		int width = avatarTexture.Width;
		int height = avatarTexture.Height;
		int[] array2 = dxtDecoder.UnpackImage(array, width, height, avatarTexture.RawDataFormat);
		int num = width * height;
		Colorb[] array3 = new Colorb[num];
		for (int i = 0; i < num; i++)
		{
			array3[i].CompositeArgb = array2[i];
		}
		AnimatedTexture animatedTexture = new AnimatedTexture(avatarTexture.Width, avatarTexture.Height, 1, TextureDataFormat.RGBA);
		animatedTexture.SetTextureLayer(0, array3);
		return animatedTexture;
	}

	private Texture2D CreateIntensityMap(AnimatedTexture avatarTexture, Vector3 colorParam1, Vector3 colorParam2, Vector3 colorParam3)
	{
		byte[] array = TransformIntensityMap(avatarTexture, colorParam1, colorParam2, colorParam3);
		if (array == null)
		{
			return null;
		}
		return CreatePow2Texture(avatarTexture.Width, avatarTexture.Height, avatarTexture.RawDataFormat, array);
	}

	private Texture2D[] CreateIntensityMapLayers(AnimatedTexture avatarTexture, Vector3 colorParam1, Vector3 colorParam2, Vector3 colorParam3)
	{
		Color color = new Color(colorParam1.X, colorParam1.Y, colorParam1.Z);
		Color color2 = new Color(colorParam2.X, colorParam2.Y, colorParam2.Z);
		Color color3 = new Color(colorParam3.X, colorParam3.Y, colorParam3.Z);
		int layersCount = avatarTexture.LayersCount;
		Texture2D[] array = new Texture2D[layersCount];
		SurfaceFormat surfaceFormat = GetSurfaceFormat(avatarTexture.RawDataFormat);
		if (surfaceFormat == SurfaceFormat.Dxt5)
		{
			int num = ((surfaceFormat == SurfaceFormat.Dxt1) ? 8 : 16);
			int num2 = (avatarTexture.Width + 3 >> 2) * (avatarTexture.Height + 3 >> 2);
			int num3 = num * num2;
			byte[] array2 = new byte[avatarTexture.GetRawData().Length];
			for (int i = 0; i < layersCount; i++)
			{
				avatarTexture.GetTextureLayerRawData(i).CopyTo(array2, 0);
				for (int j = num - 8; j < num3; j += num)
				{
					int num4 = array2[j];
					int num5 = array2[j + 1];
					int num6 = array2[j + 2];
					int num7 = array2[j + 3];
					int num8 = ((num4 & 0x1F) << 1) | ((num4 >> 4) & 1);
					int num9 = (((num5 << 8) | num4) & 0x7E0) >> 5;
					int num10 = ((num5 & 0xF8) >> 2) | ((num5 >> 7) & 1);
					int num11 = (color.R * num10 + color2.R * num9 + color3.R * num8 + 127) / 255;
					int num12 = (color.G * num10 + color2.G * num9 + color3.G * num8 + 127) / 255;
					int num13 = (color.B * num10 + color2.B * num9 + color3.B * num8 + 127) / 255;
					if (num11 > 63)
					{
						num11 = 63;
					}
					if (num12 > 63)
					{
						num12 = 63;
					}
					if (num13 > 63)
					{
						num13 = 63;
					}
					array2[j] = (byte)((num12 << 5) | (num13 >> 1));
					array2[j + 1] = (byte)(((num11 << 2) & 0xF8) | (num12 >> 3));
					num8 = ((num6 & 0x1F) << 1) | ((num6 >> 4) & 1);
					num9 = (((num7 << 8) | num6) & 0x7E0) >> 5;
					num10 = ((num7 & 0xF8) >> 2) | ((num7 >> 7) & 1);
					num11 = (color.R * num10 + color2.R * num9 + color3.R * num8 + 127) / 255;
					num12 = (color.G * num10 + color2.G * num9 + color3.G * num8 + 127) / 255;
					num13 = (color.B * num10 + color2.B * num9 + color3.B * num8 + 127) / 255;
					if (num11 > 63)
					{
						num11 = 63;
					}
					if (num12 > 63)
					{
						num12 = 63;
					}
					if (num13 > 63)
					{
						num13 = 63;
					}
					array2[j + 2] = (byte)((num12 << 5) | (num13 >> 1));
					array2[j + 3] = (byte)(((num11 << 2) & 0xF8) | (num12 >> 3));
				}
				array[i] = CreatePow2Texture(avatarTexture.Width, avatarTexture.Height, avatarTexture.RawDataFormat, array2);
			}
			return array;
		}
		throw new InvalidOperationException("Only DXT4/5 compression is supported for intensity maps");
	}

	private Texture2D CreateAlphaColorTexture(AnimatedTexture avatarTexture, bool inverse)
	{
		int num = (inverse ? 255 : 0);
		Colorb[] pixels = avatarTexture.GetPixels();
		int num2 = avatarTexture.Width * avatarTexture.Height;
		Colorb[] array = new Colorb[num2];
		for (int i = 0; i < num2; i++)
		{
			byte b = (byte)(num ^ pixels[i].alpha);
			ref Colorb reference = ref array[i];
			reference = new Colorb(b, b, b, byte.MaxValue);
		}
		return CreatePow2Texture(avatarTexture.Width, avatarTexture.Height, array);
	}

	private BlendState TransparentBlendPassColor()
	{
		BlendState blendState = new BlendState();
		blendState.AlphaBlendFunction = BlendFunction.Add;
		blendState.ColorBlendFunction = BlendFunction.Add;
		blendState.ColorSourceBlend = Blend.SourceAlpha;
		blendState.AlphaSourceBlend = Blend.SourceAlpha;
		blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
		blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		blendState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		return blendState;
	}

	private BlendState TransparentBlendPassColorDecal0()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.One;
		blendState.AlphaSourceBlend = Blend.One;
		blendState.ColorDestinationBlend = Blend.Zero;
		blendState.AlphaDestinationBlend = Blend.Zero;
		blendState.ColorWriteChannels = ColorWriteChannels.Alpha;
		return blendState;
	}

	private BlendState TransparentBlendPassColorDecal1()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.DestinationAlpha;
		blendState.AlphaSourceBlend = Blend.DestinationAlpha;
		blendState.ColorDestinationBlend = Blend.InverseDestinationAlpha;
		blendState.AlphaDestinationBlend = Blend.InverseDestinationAlpha;
		blendState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		return blendState;
	}

	private BlendState TransparentBlendPassColorDecal2()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.DestinationAlpha;
		blendState.AlphaSourceBlend = Blend.DestinationAlpha;
		blendState.ColorDestinationBlend = Blend.One;
		blendState.AlphaDestinationBlend = Blend.One;
		blendState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		return blendState;
	}

	private BlendState TransparentBlendPassFull0()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.One;
		blendState.AlphaSourceBlend = Blend.One;
		blendState.ColorDestinationBlend = Blend.Zero;
		blendState.AlphaDestinationBlend = Blend.Zero;
		blendState.ColorWriteChannels = ColorWriteChannels.Alpha;
		return blendState;
	}

	private BlendState TransparentBlendPassFull1()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.DestinationAlpha;
		blendState.AlphaSourceBlend = Blend.DestinationAlpha;
		blendState.ColorDestinationBlend = Blend.InverseDestinationAlpha;
		blendState.AlphaDestinationBlend = Blend.InverseDestinationAlpha;
		blendState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		return blendState;
	}

	private BlendState TransparentBlendPassFull2()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.Zero;
		blendState.AlphaSourceBlend = Blend.Zero;
		blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
		blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		blendState.ColorWriteChannels = ColorWriteChannels.Alpha;
		return blendState;
	}

	private BlendState TransparentBlendPassFull3()
	{
		BlendState blendState = new BlendState();
		blendState.ColorSourceBlend = Blend.DestinationAlpha;
		blendState.AlphaSourceBlend = Blend.DestinationAlpha;
		blendState.ColorDestinationBlend = Blend.One;
		blendState.AlphaDestinationBlend = Blend.One;
		blendState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		return blendState;
	}

	private Color[] ResampleImage(Colorb[] input, int inputWidth, int inputHeight, int outputWidth, int outputHeight)
	{
		Color[] array = new Color[outputWidth * outputHeight];
		int num = 0;
		int num2 = 0;
		int num3 = (inputWidth << 8) / outputWidth;
		int num4 = (inputHeight << 8) / outputHeight;
		for (int i = 0; i < outputHeight; i++)
		{
			int num5 = inputWidth * ((num2 + 128) & 0x7FFFFF00);
			for (int j = 0; j < outputWidth; j++)
			{
				Colorb colorb = input[num5 + 128 >> 8];
				ref Color reference = ref array[num++];
				reference = new Color(colorb.red, colorb.green, colorb.blue, colorb.alpha);
				num5 += num3;
			}
			num2 += num4;
		}
		return array;
	}

	private bool IsPow2(int x)
	{
		return x > 0 && (x & (x - 1)) == 0;
	}

	private int RoundUpToPow2(int x)
	{
		x--;
		x |= x >> 1;
		x |= x >> 2;
		x |= x >> 4;
		x |= x >> 8;
		x |= x >> 16;
		return x + 1;
	}

	private Texture2D CreatePow2Texture(int width, int height, Colorb[] pixels)
	{
		if (IsPow2(width) && IsPow2(height))
		{
			int num = width * height;
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i].A = pixels[i].alpha;
				array[i].R = pixels[i].red;
				array[i].G = pixels[i].green;
				array[i].B = pixels[i].blue;
			}
			Texture2D texture2D = new Texture2D(graphicsDevice, width, height, mipMap: false, SurfaceFormat.Color);
			texture2D.SetData(array);
			return texture2D;
		}
		int num2 = RoundUpToPow2(width);
		int num3 = RoundUpToPow2(height);
		Color[] data = ResampleImage(pixels, width, height, num2, num3);
		Texture2D texture2D2 = new Texture2D(graphicsDevice, num2, num3, mipMap: false, SurfaceFormat.Color);
		texture2D2.SetData(data);
		return texture2D2;
	}

	private Texture2D CreatePow2Texture(int width, int height, TextureDataFormat format, byte[] textureData)
	{
		if (IsPow2(width) && IsPow2(height))
		{
			Texture2D texture2D = new Texture2D(graphicsDevice, width, height, mipMap: false, GetSurfaceFormat(format));
			texture2D.SetData(textureData);
			return texture2D;
		}
		DxtDecoder dxtDecoder = new DxtDecoder();
		int[] array = dxtDecoder.UnpackImage(textureData, width, height, format);
		Colorb[] array2 = new Colorb[width * height];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].CompositeArgb = array[i];
		}
		int num = RoundUpToPow2(width);
		int num2 = RoundUpToPow2(height);
		Color[] data = ResampleImage(array2, width, height, num, num2);
		Texture2D texture2D2 = new Texture2D(graphicsDevice, num, num2, mipMap: false, SurfaceFormat.Color);
		texture2D2.SetData(data);
		return texture2D2;
	}

	private IndexBuffer CreateDecalIndexBuffer(TriangleBatch srcBatch, int textureLayer)
	{
		int num = 0;
		int num2 = srcBatch.Triangles.Length;
		short[] array = new short[3 * num2];
		Microsoft.XboxLive.MathUtilities.Vector2[] array2 = srcBatch.Vertices.TextureCoordinates[textureLayer];
		IndexedTriangle[] triangles = srcBatch.Triangles;
		for (int i = 0; i < num2; i++)
		{
			int num3 = 15;
			IndexedTriangle indexedTriangle = triangles[i];
			if (array2[indexedTriangle.i1].X > 0.05f)
			{
				num3 &= -2;
			}
			if (array2[indexedTriangle.i1].X < 0.95f)
			{
				num3 &= -3;
			}
			if (array2[indexedTriangle.i1].Y > 0.05f)
			{
				num3 &= -5;
			}
			if (array2[indexedTriangle.i1].Y < 0.95f)
			{
				num3 &= -9;
			}
			if (array2[indexedTriangle.i2].X > 0.05f)
			{
				num3 &= -2;
			}
			if (array2[indexedTriangle.i2].X < 0.95f)
			{
				num3 &= -3;
			}
			if (array2[indexedTriangle.i2].Y > 0.05f)
			{
				num3 &= -5;
			}
			if (array2[indexedTriangle.i2].Y < 0.95f)
			{
				num3 &= -9;
			}
			if (array2[indexedTriangle.i3].X > 0.05f)
			{
				num3 &= -2;
			}
			if (array2[indexedTriangle.i3].X < 0.95f)
			{
				num3 &= -3;
			}
			if (array2[indexedTriangle.i3].Y > 0.05f)
			{
				num3 &= -5;
			}
			if (array2[indexedTriangle.i3].Y < 0.95f)
			{
				num3 &= -9;
			}
			if (num3 == 0)
			{
				array[num++] = (short)indexedTriangle.i1;
				array[num++] = (short)indexedTriangle.i2;
				array[num++] = (short)indexedTriangle.i3;
			}
		}
		IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), num, BufferUsage.WriteOnly);
		indexBuffer.SetData(array, 0, num);
		return indexBuffer;
	}

	public AvatarRenderableModel CreateRenderBatch(AvatarComponent avatarComponent, int modelIndex, List<FacialExpression> facialExpressions)
	{
		TriangleBatch srcBatch = avatarComponent.TriangleBatches[modelIndex];
		ShaderInstance shader = avatarComponent.ShaderInstanceParameters[modelIndex];
		IBaseTextureAnimated[] textures = avatarComponent.GetTextures();
		int num = srcBatch.Triangles.Length;
		int num2 = srcBatch.Vertices.Positions.Length;
		AvatarRenderableModel avatarRenderableModel = new AvatarRenderableModel(graphicsDevice, num, num2);
		avatarRenderableModel.Manifest = avatarComponent.Manifest;
		avatarRenderableModel.ModelIndex = modelIndex;
		float num3 = 255f;
		ColorParameters colorParameters = ExtractColorParameters(shader);
		ShaderParameter[] array = new ShaderParameter[shader.ShaderParameters.Length];
		shader.ShaderParameters.CopyTo(array, 0);
		SortTextureLayers(array);
		switch (shader.ShaderId)
		{
		case ShaderId.BodyOpaque:
		{
			ShaderParameter[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				ShaderParameter shaderParameter5 = array3[j];
				if (shaderParameter5.type != ShaderParameterType.Texture)
				{
					continue;
				}
				AnimatedTexture animatedTexture5 = textures[shaderParameter5.data.Texture.TextureIndex] as AnimatedTexture;
				if (!CheckTextureTransparent(animatedTexture5))
				{
					int uvLayer = shaderParameter5.data.Texture.UvLayer;
					if (shaderParameter5.usage == ShaderParameterUsage.TextureColor)
					{
						AvatarRenderPassAlpha avatarRenderPassAlpha = new AvatarRenderPassAlpha();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPassAlpha);
						avatarRenderPassAlpha.usage = shaderParameter5.usage;
						SetTextureColorDiffuseEffectAlpha(avatarRenderPassAlpha, CreateTexture(animatedTexture5), ColorParameters.White);
						avatarRenderPassAlpha.samplerStatePrimary = CreateSamplerState(shaderParameter5.data.Texture.textureWrapMode);
						avatarRenderPassAlpha.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer]);
					}
					else if (shaderParameter5.usage == ShaderParameterUsage.TextureIntensity)
					{
						AvatarRenderPass avatarRenderPass5 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass5);
						avatarRenderPass5.usage = shaderParameter5.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass5, CreateIntensityMap(animatedTexture5, colorParameters.Custom0, colorParameters.Custom1, colorParameters.Custom2), ColorParameters.White);
						avatarRenderPass5.samplerStatePrimary = CreateSamplerState(shaderParameter5.data.Texture.textureWrapMode);
						avatarRenderPass5.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer]);
					}
					else if (shaderParameter5.usage == ShaderParameterUsage.TextureDecal)
					{
						AvatarRenderPass avatarRenderPass6 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass6);
						avatarRenderPass6.usage = shaderParameter5.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass6, CreateTexture(animatedTexture5), ColorParameters.White);
						avatarRenderPass6.samplerStatePrimary = CreateSamplerState(shaderParameter5.data.Texture.textureWrapMode);
						avatarRenderPass6.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer]);
					}
				}
			}
			break;
		}
		case ShaderId.HeadOpaque:
		{
			AvatarRenderPass avatarRenderPass7 = new AvatarRenderPassBasic();
			SetColorDiffuseEffect(avatarRenderPass7, colorParameters.SkinTone);
			avatarRenderPass7.samplerStatePrimary = CreateSamplerState(TextureWrapModes.None);
			avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
			FacialExpression facialExpression = new FacialExpression();
			ShaderParameter[] array4 = array;
			for (int k = 0; k < array4.Length; k++)
			{
				ShaderParameter shaderParameter6 = array4[k];
				if (shaderParameter6.type != ShaderParameterType.Texture)
				{
					continue;
				}
				AnimatedTexture animatedTexture6 = textures[shaderParameter6.data.Texture.TextureIndex] as AnimatedTexture;
				if (!CheckTextureTransparent(animatedTexture6))
				{
					int uvLayer2 = shaderParameter6.data.Texture.UvLayer;
					if (shaderParameter6.usage == ShaderParameterUsage.TextureSkinFeatures)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMap(animatedTexture6, colorParameters.SkinFeature1Tone, colorParameters.SkinFeature2Tone, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureEyeShadow)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMap(animatedTexture6, colorParameters.EyeShadowTone, ColorParameters.White, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureMouth)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMapLayers(animatedTexture6, colorParameters.MouthTone, ColorParameters.White, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
						facialExpression.avatarMouth = avatarRenderPass7;
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureEyeLeft)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMapLayers(animatedTexture6, colorParameters.IrisTone, ColorParameters.White, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
						facialExpression.avatarEyeLeft = avatarRenderPass7;
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureEyeRight)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMapLayers(animatedTexture6, colorParameters.IrisTone, ColorParameters.White, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
						facialExpression.avatarEyeRight = avatarRenderPass7;
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureFacialHair)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						Texture2D texture9 = CreateIntensityMap(animatedTexture6, colorParameters.FacialHairTone, ColorParameters.White, colorParameters.SkinTone);
						SetTextureColorDiffuseEffect(avatarRenderPass7, texture9, ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureEyebrowLeft)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMapLayers(animatedTexture6, colorParameters.EyebrowTone, ColorParameters.White, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
						facialExpression.avatarEyebrowLeft = avatarRenderPass7;
					}
					else if (shaderParameter6.usage == ShaderParameterUsage.TextureEyebrowRight)
					{
						avatarRenderPass7 = new AvatarRenderPassBasic();
						avatarRenderableModel.opaquePasses.Add(avatarRenderPass7);
						avatarRenderPass7.usage = shaderParameter6.usage;
						SetTextureColorDiffuseEffect(avatarRenderPass7, CreateIntensityMapLayers(animatedTexture6, colorParameters.EyebrowTone, ColorParameters.White, colorParameters.SkinTone), ColorParameters.White);
						avatarRenderPass7.samplerStatePrimary = CreateSamplerState(shaderParameter6.data.Texture.textureWrapMode);
						avatarRenderPass7.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer2]);
						avatarRenderPass7.indexBuffer = CreateDecalIndexBuffer(srcBatch, uvLayer2);
						facialExpression.avatarEyebrowRight = avatarRenderPass7;
					}
				}
			}
			facialExpressions.Add(facialExpression);
			break;
		}
		case ShaderId.BodyShinyOpaque:
		{
			AnimatedTexture animatedTexture7 = null;
			ShaderParameter shaderParameter7 = default(ShaderParameter);
			ShaderParameter[] array5 = array;
			for (int l = 0; l < array5.Length; l++)
			{
				ShaderParameter shaderParameter8 = array5[l];
				if (shaderParameter8.type != ShaderParameterType.Texture)
				{
					continue;
				}
				AnimatedTexture animatedTexture8 = textures[shaderParameter8.data.Texture.TextureIndex] as AnimatedTexture;
				if (CheckTextureTransparent(animatedTexture8))
				{
					continue;
				}
				int uvLayer3 = shaderParameter8.data.Texture.UvLayer;
				if (shaderParameter8.usage == ShaderParameterUsage.TextureColor)
				{
					shaderParameter7 = shaderParameter8;
					animatedTexture7 = animatedTexture8;
					AvatarRenderPass avatarRenderPass8 = new AvatarRenderPassAlpha();
					avatarRenderPass8.usage = shaderParameter8.usage;
					SetTextureColorDiffuseEffectAlpha(avatarRenderPass8, CreateTexture(animatedTexture8), ColorParameters.White);
					avatarRenderPass8.samplerStatePrimary = CreateSamplerState(shaderParameter8.data.Texture.textureWrapMode);
					avatarRenderPass8.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer3]);
					avatarRenderableModel.opaquePasses.Add(avatarRenderPass8);
				}
				else if (shaderParameter8.usage == ShaderParameterUsage.TextureIntensity)
				{
					AvatarRenderPass avatarRenderPass9 = new AvatarRenderPassBasic();
					avatarRenderableModel.opaquePasses.Add(avatarRenderPass9);
					avatarRenderPass9.usage = shaderParameter8.usage;
					SetTextureColorDiffuseEffect(avatarRenderPass9, CreateIntensityMap(animatedTexture8, colorParameters.Custom0, colorParameters.Custom1, colorParameters.Custom2), ColorParameters.White);
					avatarRenderPass9.samplerStatePrimary = CreateSamplerState(shaderParameter8.data.Texture.textureWrapMode);
					avatarRenderPass9.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[uvLayer3]);
				}
				else if (shaderParameter8.usage == ShaderParameterUsage.TextureReflection)
				{
					AvatarRenderPassShiny avatarRenderPassShiny2 = (avatarRenderableModel.shinyPass = new AvatarRenderPassShiny(graphicsDevice, srcBatch.Vertices.Positions.Length));
					if (CheckTextureTransparent(animatedTexture7))
					{
						avatarRenderPassShiny2.environmentTexture = CreateTexture(animatedTexture8);
						avatarRenderPassShiny2.effect = CreateReflection(avatarRenderPassShiny2.environmentTexture, colorParameters.Reflectivity.X * 0.5f);
						avatarRenderPassShiny2.environmentMapMaskSmaplerState = null;
						avatarRenderPassShiny2.texCoordBufferMask = null;
					}
					else
					{
						avatarRenderPassShiny2.environmentTexture = CreateTexture(animatedTexture8);
						avatarRenderPassShiny2.environnmentMask = CreateAlphaColorTexture(animatedTexture7, inverse: false);
						avatarRenderPassShiny2.effect = CreateReflection(avatarRenderPassShiny2.environmentTexture, avatarRenderPassShiny2.environnmentMask, colorParameters.Reflectivity.X * 0.5f);
						avatarRenderPassShiny2.texCoordBufferMask = CreateTexCoordBuffer2(srcBatch.Vertices.TextureCoordinates[shaderParameter7.data.Texture.UvLayer]);
						avatarRenderPassShiny2.environmentMapMaskSmaplerState = CreateSamplerState(shaderParameter7.data.Texture.textureWrapMode);
					}
				}
			}
			break;
		}
		case ShaderId.BodyTransparent:
		case ShaderId.BodyShinyTransparent:
		{
			AnimatedTexture animatedTexture = null;
			AnimatedTexture animatedTexture2 = null;
			AnimatedTexture animatedTexture3 = null;
			Texture2D texture2D = null;
			Texture2D texture2D2 = null;
			ShaderParameter shaderParameter = default(ShaderParameter);
			ShaderParameter shaderParameter2 = default(ShaderParameter);
			ShaderParameter shaderParameter3 = default(ShaderParameter);
			ShaderParameter[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ShaderParameter shaderParameter4 = array2[i];
				if (shaderParameter4.type == ShaderParameterType.Texture)
				{
					AnimatedTexture animatedTexture4 = textures[shaderParameter4.data.Texture.TextureIndex] as AnimatedTexture;
					if (shaderParameter4.usage == ShaderParameterUsage.TextureColor)
					{
						shaderParameter = shaderParameter4;
						animatedTexture3 = animatedTexture4;
						texture2D2 = CreateTexture(animatedTexture3);
					}
					else if (shaderParameter4.usage == ShaderParameterUsage.TextureIntensity)
					{
						shaderParameter2 = shaderParameter4;
						animatedTexture2 = animatedTexture4;
					}
					else if (shaderParameter4.usage == ShaderParameterUsage.TextureDecal)
					{
						shaderParameter3 = shaderParameter4;
						animatedTexture = animatedTexture4;
					}
					else if (shaderParameter4.usage == ShaderParameterUsage.TextureReflection)
					{
						texture2D = CreateTexture(animatedTexture4);
					}
				}
			}
			float val = 1f - colorParameters.Transparency.X;
			val = Math.Max(val, 0f);
			val = Math.Min(val, 1f);
			num3 = 255f * val;
			if (animatedTexture2 != null && CheckTextureTransparent(animatedTexture2))
			{
				animatedTexture2 = null;
			}
			if (animatedTexture != null && CheckTextureTransparent(animatedTexture))
			{
				animatedTexture = null;
			}
			if (animatedTexture2 == null)
			{
				if (animatedTexture == null)
				{
					AvatarRenderPass avatarRenderPass = new AvatarRenderPassBasic();
					avatarRenderableModel.transparentPasses.Add(avatarRenderPass);
					avatarRenderPass.usage = ShaderParameterUsage.TextureColor;
					SetTextureEffect(avatarRenderPass, texture2D2);
					avatarRenderPass.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
					avatarRenderPass.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
					avatarRenderPass.blendState = TransparentBlendPassColor();
				}
				else
				{
					animatedTexture = animatedTexture.CreateRgbaTexture();
					Texture2D texture = CreateTexturePremultiplied(animatedTexture);
					Texture2D texture2 = CreateAlphaColorTexture(animatedTexture, inverse: true);
					AvatarRenderPass avatarRenderPass2 = new AvatarRenderPassBasic();
					avatarRenderableModel.transparentPasses.Add(avatarRenderPass2);
					avatarRenderPass2.usage = ShaderParameterUsage.TextureColor;
					SetTextureEffect(avatarRenderPass2, texture2D2);
					avatarRenderPass2.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
					avatarRenderPass2.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
					avatarRenderPass2.blendState = TransparentBlendPassColorDecal0();
					avatarRenderPass2.textures = null;
					avatarRenderPass2 = new AvatarRenderPassBasic();
					avatarRenderableModel.transparentPasses.Add(avatarRenderPass2);
					avatarRenderPass2.usage = ShaderParameterUsage.TextureDecal;
					SetTextureEffect(avatarRenderPass2, texture);
					avatarRenderPass2.samplerStatePrimary = CreateSamplerState(shaderParameter3.data.Texture.textureWrapMode);
					avatarRenderPass2.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter3.data.Texture.UvLayer]);
					avatarRenderPass2.blendState = TransparentBlendPassColorDecal1();
					avatarRenderPass2 = new AvatarRenderPassBasic();
					avatarRenderableModel.transparentPasses.Add(avatarRenderPass2);
					avatarRenderPass2.usage = ShaderParameterUsage.TextureColor;
					SetDualTextureEffect(avatarRenderPass2, texture2D2, texture2, new Vector3(0.5f));
					avatarRenderPass2.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
					avatarRenderPass2.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
					avatarRenderPass2.samplerStateSecondary = CreateSamplerState(shaderParameter3.data.Texture.textureWrapMode);
					avatarRenderPass2.texCoordBufferSecondary = CreateTexCoordBuffer2(srcBatch.Vertices.TextureCoordinates[shaderParameter3.data.Texture.UvLayer]);
					avatarRenderPass2.blendState = TransparentBlendPassColorDecal2();
				}
			}
			else if (animatedTexture == null)
			{
				AnimatedTexture avatarTexture = CreateIntensityTexture(animatedTexture2, colorParameters.Custom0, colorParameters.Custom1, colorParameters.Custom2);
				Texture2D texture3 = CreateTexturePremultiplied(avatarTexture);
				Texture2D texture4 = CreateAlphaColorTexture(avatarTexture, inverse: true);
				AvatarRenderPass avatarRenderPass3 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass3);
				avatarRenderPass3.usage = ShaderParameterUsage.TextureColor;
				SetTextureEffect(avatarRenderPass3, texture2D2);
				avatarRenderPass3.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
				avatarRenderPass3.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
				avatarRenderPass3.blendState = TransparentBlendPassColorDecal0();
				avatarRenderPass3.textures = null;
				avatarRenderPass3 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass3);
				avatarRenderPass3.usage = ShaderParameterUsage.TextureIntensity;
				SetTextureEffect(avatarRenderPass3, texture3);
				avatarRenderPass3.samplerStatePrimary = CreateSamplerState(shaderParameter2.data.Texture.textureWrapMode);
				avatarRenderPass3.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter2.data.Texture.UvLayer]);
				avatarRenderPass3.blendState = TransparentBlendPassColorDecal1();
				avatarRenderPass3 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass3);
				avatarRenderPass3.usage = ShaderParameterUsage.TextureColor;
				SetDualTextureEffect(avatarRenderPass3, texture2D2, texture4, new Vector3(0.5f));
				avatarRenderPass3.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
				avatarRenderPass3.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
				avatarRenderPass3.samplerStateSecondary = CreateSamplerState(shaderParameter2.data.Texture.textureWrapMode);
				avatarRenderPass3.texCoordBufferSecondary = CreateTexCoordBuffer2(srcBatch.Vertices.TextureCoordinates[shaderParameter2.data.Texture.UvLayer]);
				avatarRenderPass3.blendState = TransparentBlendPassColorDecal2();
			}
			else
			{
				Texture2D texture5 = CreateTexture(animatedTexture);
				Texture2D texture6 = CreateTexturePremultiplied(animatedTexture);
				AnimatedTexture avatarTexture2 = CreateIntensityTexture(animatedTexture2, colorParameters.Custom0, colorParameters.Custom1, colorParameters.Custom2);
				Texture2D texture7 = CreateTexturePremultiplied(avatarTexture2);
				Texture2D texture8 = CreateAlphaColorTexture(avatarTexture2, inverse: true);
				AvatarRenderPass avatarRenderPass4 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass4);
				avatarRenderPass4.usage = ShaderParameterUsage.TextureColor;
				SetTextureEffect(avatarRenderPass4, texture2D2);
				avatarRenderPass4.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
				avatarRenderPass4.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
				avatarRenderPass4.blendState = TransparentBlendPassFull0();
				avatarRenderPass4.textures = null;
				avatarRenderPass4 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass4);
				avatarRenderPass4.usage = ShaderParameterUsage.TextureDecal;
				SetTextureEffect(avatarRenderPass4, texture6);
				avatarRenderPass4.samplerStatePrimary = CreateSamplerState(shaderParameter3.data.Texture.textureWrapMode);
				avatarRenderPass4.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter3.data.Texture.UvLayer]);
				avatarRenderPass4.blendState = TransparentBlendPassFull1();
				avatarRenderPass4 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass4);
				avatarRenderPass4.usage = ShaderParameterUsage.TextureDecal;
				SetTextureEffect(avatarRenderPass4, texture5);
				avatarRenderPass4.samplerStatePrimary = CreateSamplerState(shaderParameter3.data.Texture.textureWrapMode);
				avatarRenderPass4.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter3.data.Texture.UvLayer]);
				avatarRenderPass4.blendState = TransparentBlendPassFull2();
				avatarRenderPass4 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass4);
				avatarRenderPass4.usage = ShaderParameterUsage.TextureIntensity;
				SetTextureEffect(avatarRenderPass4, texture7);
				avatarRenderPass4.samplerStatePrimary = CreateSamplerState(shaderParameter3.data.Texture.textureWrapMode);
				avatarRenderPass4.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter3.data.Texture.UvLayer]);
				avatarRenderPass4.blendState = TransparentBlendPassFull3();
				avatarRenderPass4 = new AvatarRenderPassBasic();
				avatarRenderableModel.transparentPasses.Add(avatarRenderPass4);
				avatarRenderPass4.usage = ShaderParameterUsage.TextureColor;
				SetDualTextureEffect(avatarRenderPass4, texture2D2, texture8, new Vector3(0.5f));
				avatarRenderPass4.samplerStatePrimary = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
				avatarRenderPass4.texCoordBufferPrimary = CreateTexCoordBuffer(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
				avatarRenderPass4.samplerStateSecondary = CreateSamplerState(shaderParameter2.data.Texture.textureWrapMode);
				avatarRenderPass4.texCoordBufferSecondary = CreateTexCoordBuffer2(srcBatch.Vertices.TextureCoordinates[shaderParameter2.data.Texture.UvLayer]);
				avatarRenderPass4.blendState = TransparentBlendPassFull3();
			}
			if (texture2D != null)
			{
				AvatarRenderPassShiny avatarRenderPassShiny = (avatarRenderableModel.shinyPass = new AvatarRenderPassShiny(graphicsDevice, srcBatch.Vertices.Positions.Length));
				if (CheckTextureTransparent(animatedTexture3))
				{
					avatarRenderPassShiny.environmentTexture = texture2D;
					avatarRenderPassShiny.effect = CreateReflection(texture2D, colorParameters.Reflectivity.X * 0.5f);
					avatarRenderPassShiny.environmentMapMaskSmaplerState = null;
					avatarRenderPassShiny.texCoordBufferMask = null;
				}
				else
				{
					avatarRenderPassShiny.environmentTexture = texture2D;
					avatarRenderPassShiny.environnmentMask = CreateAlphaColorTexture(animatedTexture3, inverse: false);
					avatarRenderPassShiny.effect = CreateReflection(avatarRenderPassShiny.environmentTexture, avatarRenderPassShiny.environnmentMask, colorParameters.Reflectivity.X * 0.5f);
					avatarRenderPassShiny.texCoordBufferMask = CreateTexCoordBuffer2(srcBatch.Vertices.TextureCoordinates[shaderParameter.data.Texture.UvLayer]);
					avatarRenderPassShiny.environmentMapMaskSmaplerState = CreateSamplerState(shaderParameter.data.Texture.textureWrapMode);
				}
			}
			break;
		}
		}
		short[] array6 = new short[3 * num];
		for (int m = 0; m < num; m++)
		{
			array6[3 * m] = (short)srcBatch.Triangles[m].i1;
			array6[3 * m + 1] = (short)srcBatch.Triangles[m].i2;
			array6[3 * m + 2] = (short)srcBatch.Triangles[m].i3;
		}
		avatarRenderableModel.indexBuffer.SetData(array6);
		SkinnedVertex skinnedVertex = default(SkinnedVertex);
		avatarRenderableModel.skinnedVertices = new SkinnedVertex[num2];
		PositionColorVertex[] array7 = new PositionColorVertex[num2];
		for (int n = 0; n < num2; n++)
		{
			Microsoft.XboxLive.MathUtilities.Vector3 vector = srcBatch.Vertices.RawPositions[n];
			Microsoft.XboxLive.MathUtilities.Vector3 vector2 = srcBatch.Vertices.RawNormals[n];
			Colorf colorf = srcBatch.Vertices.Color0[n];
			Vector4b vector4b = srcBatch.Vertices.SkinBindings[n];
			Vector4b vector4b2 = srcBatch.Vertices.SkinWeights[n];
			skinnedVertex.position.X = vector.X;
			skinnedVertex.position.Y = vector.Y;
			skinnedVertex.position.Z = vector.Z;
			skinnedVertex.normal.X = vector2.X;
			skinnedVertex.normal.Y = vector2.Y;
			skinnedVertex.normal.Z = vector2.Z;
			skinnedVertex.r = 255f * colorf.red;
			skinnedVertex.g = 255f * colorf.green;
			skinnedVertex.b = 255f * colorf.blue;
			skinnedVertex.a = num3 * colorf.alpha;
			skinnedVertex.i0 = vector4b.X;
			skinnedVertex.i1 = vector4b.Y;
			skinnedVertex.i2 = vector4b.Z;
			skinnedVertex.i3 = vector4b.W;
			skinnedVertex.w0 = (float)(int)vector4b2.X * 0.003921569f;
			skinnedVertex.w1 = (float)(int)vector4b2.Y * 0.003921569f;
			skinnedVertex.w2 = (float)(int)vector4b2.Z * 0.003921569f;
			skinnedVertex.w3 = (float)(int)vector4b2.W * 0.003921569f;
			avatarRenderableModel.skinnedVertices[n] = skinnedVertex;
			ref PositionColorVertex reference = ref array7[n];
			reference = new PositionColorVertex(skinnedVertex.position, Color.White);
		}
		avatarRenderableModel.vertexBuffer.SetData(array7);
		return avatarRenderableModel;
	}
}
