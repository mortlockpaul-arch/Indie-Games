using System;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

internal class AnimatedTexture : IBaseTextureAnimated, IBaseTexture
{
	private int m_Width;

	private int m_Height;

	private int m_CurrentLayerIndex;

	private Colorb[][] m_TexturePixels;

	private byte[][] m_TextureRawData;

	private TextureDataFormat m_DataFormat;

	public int Width => m_Width;

	public int Height => m_Height;

	public int LayersCount => m_TexturePixels.Length;

	public bool IsEmptyTransparent => m_DataFormat > TextureDataFormat.Empty;

	public bool IsEmptyOpaque => m_DataFormat == TextureDataFormat.Empty;

	public TextureDataFormat RawDataFormat => m_DataFormat;

	private AnimatedTexture()
	{
		m_DataFormat = TextureDataFormat.Empty;
	}

	public AnimatedTexture(int width, int height, int layersCount, TextureDataFormat format)
	{
		if ((format & TextureDataFormat.Empty) != TextureDataFormat.Dxt1 && (width != 4 || height != 4))
		{
			throw new ArgumentException("Width and Height must be 4 for empty texture format");
		}
		m_Width = width;
		m_Height = height;
		switch (format)
		{
		case TextureDataFormat.Dxt2:
			m_DataFormat = TextureDataFormat.Dxt3;
			break;
		case (TextureDataFormat)9:
			m_DataFormat = (TextureDataFormat)10;
			break;
		case TextureDataFormat.Dxt4:
			m_DataFormat = TextureDataFormat.Dxt5;
			break;
		case (TextureDataFormat)11:
			m_DataFormat = (TextureDataFormat)12;
			break;
		default:
			m_DataFormat = format;
			break;
		}
		m_CurrentLayerIndex = 0;
		m_TextureRawData = new byte[layersCount][];
		m_TexturePixels = new Colorb[layersCount][];
	}

	public AnimatedTexture CreateRgbaTexture()
	{
		AnimatedTexture animatedTexture;
		int num;
		if (m_DataFormat >= TextureDataFormat.Empty)
		{
			if (IsEmptyTransparent)
			{
				return new AnimatedTexture(Width, Height, LayersCount, (TextureDataFormat)13);
			}
			animatedTexture = new AnimatedTexture(Width, Height, LayersCount, TextureDataFormat.RGBA);
			num = Width * Height;
			Colorb[] array = new Colorb[num];
			Colorb colorb = new Colorb(0, 0, 0, byte.MaxValue);
			for (int i = 0; i < num; i++)
			{
				array[i] = colorb;
			}
			for (int j = 0; j < LayersCount; j++)
			{
				animatedTexture.SetTextureLayer(j, array);
			}
			return animatedTexture;
		}
		animatedTexture = new AnimatedTexture(Width, Height, LayersCount, TextureDataFormat.RGBA);
		num = Width * Height;
		for (int k = 0; k < LayersCount; k++)
		{
			Colorb[] array2 = new Colorb[num];
			Colorb[] textureLayerPixels = GetTextureLayerPixels(k);
			textureLayerPixels.CopyTo(array2, 0);
			animatedTexture.SetTextureLayer(k, array2);
		}
		return animatedTexture;
	}

	public int GetMemoryUsage()
	{
		return m_Width * m_Height * m_TexturePixels.Length * 4;
	}

	public void SetTextureLayer(int index, Colorb[] pixels)
	{
		if (m_DataFormat != TextureDataFormat.RGBA)
		{
			throw new ArgumentException("SetTextureLayer requires RGBA texture data format");
		}
		m_TexturePixels[index] = pixels;
		m_TextureRawData[index] = null;
	}

	public void SelectTextureLayer(int index)
	{
		m_CurrentLayerIndex = index;
	}

	public void SetPixels(Colorb[] pixels)
	{
		SetTextureLayer(m_CurrentLayerIndex, pixels);
	}

	public Colorb[] GetPixels()
	{
		return GetTextureLayerPixels(m_CurrentLayerIndex);
	}

	public Colorb[] GetTextureLayerPixels(int index)
	{
		if (m_DataFormat == TextureDataFormat.RGBA)
		{
			return m_TexturePixels[index];
		}
		if ((m_DataFormat & TextureDataFormat.Empty) != TextureDataFormat.Dxt1)
		{
			Colorb[] array = new Colorb[1];
			if (IsEmptyTransparent)
			{
				ref Colorb reference = ref array[0];
				reference = new Colorb(0, 0, 0, 0);
			}
			else
			{
				ref Colorb reference2 = ref array[0];
				reference2 = new Colorb(0, 0, 0, byte.MaxValue);
			}
			return array;
		}
		DxtDecoder dxtDecoder = new DxtDecoder();
		int[] buffer = dxtDecoder.UnpackImage(m_TextureRawData[index], m_Width, m_Height, m_DataFormat);
		return ConvertToColor(buffer);
	}

	public byte[] GetRawData()
	{
		return GetTextureLayerRawData(m_CurrentLayerIndex);
	}

	public void SetPixels(IBaseTexture sourceTexture)
	{
		if (sourceTexture.Width != m_Width || sourceTexture.Height != m_Height)
		{
			throw new ArgumentException("sourceTexture has invalid resolution");
		}
		if (!(sourceTexture is AnimatedTexture animatedTexture))
		{
			throw new ArgumentException("sourceTexture has invalid format");
		}
		if (animatedTexture.RawDataFormat != m_DataFormat)
		{
			throw new ArgumentException("data format does not match");
		}
		if ((m_DataFormat & TextureDataFormat.Empty) != TextureDataFormat.Dxt1)
		{
			m_TexturePixels[m_CurrentLayerIndex] = null;
			m_TextureRawData[m_CurrentLayerIndex] = null;
			return;
		}
		TextureDataFormat dataFormat = m_DataFormat;
		if (dataFormat == TextureDataFormat.RGBA)
		{
			m_TexturePixels[m_CurrentLayerIndex] = animatedTexture.GetPixels();
			m_TextureRawData[m_CurrentLayerIndex] = null;
		}
		else
		{
			m_TexturePixels[m_CurrentLayerIndex] = animatedTexture.GetPixels();
			m_TextureRawData[m_CurrentLayerIndex] = animatedTexture.GetTextureLayerRawData(animatedTexture.m_CurrentLayerIndex);
		}
	}

	public void SetTextureLayer(int layerIndex, byte[] data, int dataWidth, int dataHeight)
	{
		if (m_Width != dataWidth)
		{
			throw new InvalidOperationException("Xna texture wrapper does not support auto stretching");
		}
		if (m_Height != dataHeight)
		{
			throw new InvalidOperationException("Xna texture wrapper does not support auto stretching");
		}
		if ((m_DataFormat & TextureDataFormat.Empty) != TextureDataFormat.Dxt1)
		{
			throw new InvalidOperationException("Cannot set data to empty texture");
		}
		m_TexturePixels[layerIndex] = null;
		m_TextureRawData[layerIndex] = data;
	}

	private static Colorb[] ConvertToColor(int[] buffer)
	{
		int num = buffer.Length;
		Colorb[] array = new Colorb[num];
		for (int i = 0; i < num; i++)
		{
			ref Colorb reference = ref array[i];
			reference = new Colorb(buffer[i]);
		}
		return array;
	}

	public byte[] GetTextureLayerRawData(int layerIndex)
	{
		switch (m_DataFormat)
		{
		case TextureDataFormat.RGBA:
		{
			int num = m_Width * m_Height;
			Colorb[] array = m_TexturePixels[layerIndex];
			byte[] array2 = new byte[num * 4];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				array2[num2++] = array[i].blue;
				array2[num2++] = array[i].green;
				array2[num2++] = array[i].red;
				array2[num2++] = array[i].alpha;
			}
			return array2;
		}
		case TextureDataFormat.Empty:
			return new byte[8];
		case (TextureDataFormat)9:
		case (TextureDataFormat)10:
		case (TextureDataFormat)11:
		case (TextureDataFormat)12:
			return new byte[16];
		case (TextureDataFormat)13:
			return new byte[64];
		default:
			return m_TextureRawData[layerIndex];
		}
	}
}
