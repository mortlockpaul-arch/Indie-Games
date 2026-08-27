using System.Collections.Generic;

namespace EGEngine;

public class PalyerWorldItems : StorageHelper
{
	public static void Write(byte[] buff, List<ItemCls> tents, List<ItemCls> contents)
	{
		if (!DataEncoder.IsBusySave_Wait)
		{
			return;
		}
		int idx = StorageHelper.WorldItemsDataOffset;
		StorageHelper.SetVersion(buff, ref idx, StorageHelper.ItemsVersion);
		if (tents != null)
		{
			int num = ((tents.Count > 8) ? 8 : tents.Count);
			StorageHelper.WriteInt(buff, ref idx, (byte)tents.Count);
			for (int i = 0; i < num; i++)
			{
				tents[i].BufferWrite(buff, ref idx);
			}
		}
		else
		{
			StorageHelper.WriteInt(buff, ref idx, 0);
		}
		if (contents != null)
		{
			int num2 = ((contents.Count > 192) ? 192 : contents.Count);
			StorageHelper.WriteInt(buff, ref idx, (byte)contents.Count);
			for (int j = 0; j < num2; j++)
			{
				contents[j].BufferWrite(buff, ref idx);
			}
		}
		else
		{
			StorageHelper.WriteInt(buff, ref idx, 0);
		}
	}

	public static void Read(byte[] buff, List<ItemCls> tents, List<ItemCls> contents)
	{
		int idx = StorageHelper.WorldItemsDataOffset;
		if (!StorageHelper.TestVersion(buff, ref idx, StorageHelper.ItemsVersion))
		{
			return;
		}
		idx += StorageHelper.ItemsVersion.Length;
		int e = 0;
		StorageHelper.ReadInt(buff, ref idx, ref e);
		if (e > 0)
		{
			if (tents == null)
			{
				tents = new List<ItemCls>();
			}
			else
			{
				tents.Clear();
			}
			for (int i = 0; i < e; i++)
			{
				tents.Add(new ItemCls(buff, ref idx));
			}
		}
		int e2 = 0;
		StorageHelper.ReadInt(buff, ref idx, ref e2);
		if (e2 > 0)
		{
			if (contents == null)
			{
				contents = new List<ItemCls>();
			}
			else
			{
				contents.Clear();
			}
			for (int j = 0; j < e2; j++)
			{
				contents.Add(new ItemCls(buff, ref idx));
			}
		}
	}
}
