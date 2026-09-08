using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetTextureParser
{
	public enum D3DFORMAT
	{
		D3DFMT_DXT1 = 438305106,
		D3DFMT_LIN_DXT1 = 438304850,
		D3DFMT_DXT2 = 438305107,
		D3DFMT_LIN_DXT2 = 438304851,
		D3DFMT_DXT3 = D3DFMT_DXT2,
		D3DFMT_LIN_DXT3 = D3DFMT_LIN_DXT2,
		D3DFMT_DXT3A = 438305146,
		D3DFMT_LIN_DXT3A = 438304890,
		D3DFMT_DXT3A_1111 = 438305149,
		D3DFMT_LIN_DXT3A_1111 = 438304893,
		D3DFMT_DXT4 = 438305108,
		D3DFMT_LIN_DXT4 = 438304852,
		D3DFMT_DXT5 = D3DFMT_DXT4,
		D3DFMT_LIN_DXT5 = D3DFMT_LIN_DXT4
	}

	public IBaseTextureAnimated m_texture;

	public D3DFORMAT m_format;

	public int m_width;

	public int m_height;

	public int m_layerCount;

	public IBaseTextureAnimated Texture => m_texture;

	public static TextureDataFormat RemapFormat(D3DFORMAT format)
	{
		switch (format)
		{
		case D3DFORMAT.D3DFMT_LIN_DXT1:
		case D3DFORMAT.D3DFMT_DXT1:
			return TextureDataFormat.Dxt1;
		case D3DFORMAT.D3DFMT_LIN_DXT2:
		case D3DFORMAT.D3DFMT_DXT2:
			return TextureDataFormat.Dxt2;
		case D3DFORMAT.D3DFMT_LIN_DXT4:
		case D3DFORMAT.D3DFMT_DXT4:
			return TextureDataFormat.Dxt4;
		default:
			return TextureDataFormat.Dxt5;
		}
	}

	public static void SwapEndians(byte[] image)
	{
		for (int i = 0; i < image.Length; i += 2)
		{
			byte b = image[i];
			image[i] = image[i + 1];
			image[i + 1] = b;
		}
	}

	public void Parse(EndianStream stream, IResourceFactory resourceFactory)
	{
		BitStream bitStream = new BitStream(stream);
		m_format = (D3DFORMAT)bitStream.ReadUint(32);
		m_width = bitStream.ReadInt(32);
		m_height = bitStream.ReadInt(32);
		bitStream.ReadInt(32);
		bitStream.ReadInt(32);
		m_layerCount = bitStream.ReadInt(32);
		bool flag = bitStream.ReadBool(1);
		bitStream.ReadBool(1);
		int num = bitStream.ReadInt(32);
		int num2 = bitStream.ReadInt(32);
		if ((uint)m_width > 1024u)
		{
			Logger.Log(new DebugLog(this, Resources.TextureParserError1));
			throw new AvatarException(Resources.TextureParserError1);
		}
		if ((uint)m_height > 1024u)
		{
			Logger.Log(new DebugLog(this, Resources.TextureParserError2));
			throw new AvatarException(Resources.TextureParserError2);
		}
		if ((uint)m_layerCount > 14u)
		{
			Logger.Log(new DebugLog(this, Resources.TextureParserError3));
			throw new AvatarException(Resources.TextureParserError3);
		}
		if (m_layerCount == 0)
		{
			Logger.Log(new DebugLog(this, Resources.TextureParserError3));
			throw new AvatarException(Resources.TextureParserError3);
		}
		TextureDataFormat textureDataFormat = RemapFormat(m_format);
		if (!flag)
		{
			m_texture = resourceFactory.CreateAnimatedTexture(m_width, m_height, m_layerCount, textureDataFormat);
			int num3 = ((textureDataFormat == TextureDataFormat.Dxt1) ? 8 : 16);
			int num4 = m_width + 3 >> 2;
			int num5 = m_height + 3 >> 2;
			if (num4 * num3 != num)
			{
				Logger.Log(new DebugLog(this, Resources.TextureParserError4));
				throw new AvatarException(Resources.TextureParserError4);
			}
			if (num5 != num2)
			{
				Logger.Log(new DebugLog(this, Resources.TextureParserError2));
				throw new AvatarException(Resources.TextureParserError2);
			}
			int num6 = num2 * num;
			for (int i = 0; i < m_layerCount; i++)
			{
				byte[] array = new byte[num6];
				stream.Read(array, 0, num6);
				SwapEndians(array);
				m_texture.SetTextureLayer(i, array, m_width, m_height);
			}
		}
		else
		{
			m_texture = resourceFactory.CreateAnimatedTexture(4, 4, m_layerCount, textureDataFormat | TextureDataFormat.Empty);
		}
	}
}
