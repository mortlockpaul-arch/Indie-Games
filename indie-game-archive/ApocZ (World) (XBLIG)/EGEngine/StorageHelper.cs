using System.IO;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class StorageHelper
{
	public const byte MaxTents = 8;

	public const byte MaxTentContents = 192;

	public const byte PlayerStatusLength = 70;

	public const byte PlayerInvetoryLength = 1;

	public const byte PlayerWorldItemsLength = 1;

	public const byte PlayerCharacterLength = 16;

	public static byte[] StatusVersion = new byte[12]
	{
		65, 112, 111, 99, 90, 24, 32, 129, 201, 6,
		4, 8
	};

	public static byte[] InventoryVersion = new byte[12]
	{
		65, 112, 111, 99, 90, 4, 16, 0, 197, 3,
		5, 1
	};

	public static byte[] ItemsVersion = new byte[12]
	{
		65, 112, 111, 99, 90, 124, 60, 129, 76, 4,
		3, 2
	};

	public static byte[] CharacterVersion = new byte[12]
	{
		65, 112, 111, 99, 90, 54, 150, 29, 11, 3,
		7, 6
	};

	public static int StatusDataOffset = 0;

	public static int CharacterDataOffset = 0;

	public static int InvetoryDataOffset = 0;

	public static int WorldItemsDataOffset = 0;

	public StorageHelper()
	{
		StatusDataOffset = 0;
		CharacterDataOffset = 70 + StatusVersion.Length + 8;
		InvetoryDataOffset = CharacterDataOffset + 16 + CharacterVersion.Length + 8;
		WorldItemsDataOffset = InvetoryDataOffset + InventoryVersion.Length + 160;
	}

	public static bool SetVersion(byte[] buff, ref int idx, byte[] e)
	{
		for (int i = 0; i < e.Length; i++)
		{
			if (idx >= buff.Length)
			{
				break;
			}
			buff[idx++] = e[i];
		}
		return idx < buff.Length;
	}

	public static bool TestVersion(byte[] buff, ref int idx, byte[] e)
	{
		int num = 0;
		int num2 = idx;
		while (num < e.Length && num2 < buff.Length && e[num] == buff[num2])
		{
			num++;
			num2++;
		}
		return num >= e.Length;
	}

	public static void ReadInt(Stream wr, ref int e)
	{
		e = wr.ReadByte();
		e |= wr.ReadByte() << 8;
		e |= wr.ReadByte() << 16;
		e |= wr.ReadByte() << 24;
	}

	public static void WriteInt(Stream wr, int e)
	{
		wr.WriteByte((byte)(e & 0xFF));
		wr.WriteByte((byte)((e >> 8) & 0xFF));
		wr.WriteByte((byte)((e >> 16) & 0xFF));
		wr.WriteByte((byte)((e >> 24) & 0xFF));
	}

	public static void ReadInt(byte[] wr, ref int idx, ref int e)
	{
		e = wr[idx++];
		e |= wr[idx++] << 8;
		e |= wr[idx++] << 16;
		e |= wr[idx++] << 24;
	}

	public static void WriteInt(byte[] wr, ref int idx, int e)
	{
		wr[idx++] = (byte)(e & 0xFF);
		wr[idx++] = (byte)(e >> 8);
		wr[idx++] = (byte)(e >> 16);
		wr[idx++] = (byte)(e >> 24);
	}

	public static Vector4 WriteVector(int e)
	{
		Vector4 zero = Vector4.Zero;
		zero.X = (int)(byte)(e & 0xFF);
		zero.Y = (int)(byte)(e >> 8);
		zero.Z = (int)(byte)(e >> 16);
		zero.W = (int)(byte)(e >> 24);
		return zero;
	}
}
