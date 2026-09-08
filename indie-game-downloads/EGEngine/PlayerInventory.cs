namespace EGEngine;

public class PlayerInventory : StorageHelper
{
	public static void Write(byte[] buff)
	{
		if (DataEncoder.IsBusySave_Wait)
		{
			int idx = StorageHelper.InvetoryDataOffset;
			StorageHelper.SetVersion(buff, ref idx, StorageHelper.InventoryVersion);
			AIBase.PlayerInventory.SaveInventory(buff, ref idx);
		}
	}

	public static void Read(byte[] buff)
	{
		int idx = StorageHelper.InvetoryDataOffset;
		if (StorageHelper.TestVersion(buff, ref idx, StorageHelper.InventoryVersion))
		{
			idx += StorageHelper.InventoryVersion.Length;
			AIBase.PlayerInventory.ReadInventory(buff, ref idx);
		}
	}
}
