using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetTriangleOverrideParser
{
	public int m_GlobalIndexBufferSize;

	public Guid m_OriginalAssetId;

	public int[] m_aDegeneratedTriangleIndexes;

	public Guid GetOriginalAssetId()
	{
		return m_OriginalAssetId;
	}

	public void Parse(Stream stream)
	{
		BitStream bitStream = new BitStream(stream);
		uint num = bitStream.ReadUint(32);
		if (num > 8192)
		{
			Logger.Log(new DebugLog(this, Resources.TriangleOverrideError1));
			throw new AvatarException(Resources.TriangleOverrideError1);
		}
		m_GlobalIndexBufferSize = bitStream.ReadInt(32);
		m_OriginalAssetId = new Guid(bitStream.ReadByteArray(16));
		m_aDegeneratedTriangleIndexes = new int[num];
		IntegerDataUnpacker integerDataUnpacker = new IntegerDataUnpacker();
		integerDataUnpacker.UnpackHeader(bitStream);
		for (int i = 0; i < num; i++)
		{
			integerDataUnpacker.UnpackData(bitStream, out m_aDegeneratedTriangleIndexes[i]);
		}
	}

	public void Apply(AvatarComponent avatarComponent)
	{
		int num = avatarComponent.m_Batches.Length;
		int[] array = new int[num + 1];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			array[i] = num2;
			num2 += avatarComponent.m_Batches[i].Triangles.Length;
		}
		array[num] = num2;
		int num3 = m_aDegeneratedTriangleIndexes.Length;
		for (int j = 0; j < num3; j++)
		{
			int num4 = m_aDegeneratedTriangleIndexes[j];
			for (int k = 1; k <= num; k++)
			{
				if (num4 < array[k])
				{
					k--;
					num4 -= array[k];
					int i2 = avatarComponent.m_Batches[k].Triangles[num4].i1;
					avatarComponent.m_Batches[k].Triangles[num4].i2 = i2;
					avatarComponent.m_Batches[k].Triangles[num4].i3 = i2;
					break;
				}
			}
		}
	}

	public int GetMemoryUsage()
	{
		return m_aDegeneratedTriangleIndexes.Length * 4 + 64;
	}
}
