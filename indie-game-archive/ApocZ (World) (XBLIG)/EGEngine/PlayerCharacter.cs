namespace EGEngine;

public class PlayerCharacter : StorageHelper
{
	public static void Write(byte[] buff, PlayerBase playerRef)
	{
		if (DataEncoder.IsBusySave_Wait)
		{
			int idx = StorageHelper.CharacterDataOffset;
			StorageHelper.SetVersion(buff, ref idx, StorageHelper.CharacterVersion);
			StorageHelper.WriteInt(buff, ref idx, 16);
			buff[idx++] = playerRef.CharacterIndex;
			buff[idx++] = (byte)playerRef.ShirtIndex;
			buff[idx++] = (byte)playerRef.PantstIndex;
			buff[idx++] = 108;
			buff[idx++] = 35;
			buff[idx++] = 8;
			buff[idx++] = 51;
			buff[idx++] = 127;
			buff[idx++] = 108;
			buff[idx++] = 16;
			buff[idx++] = 128;
			buff[idx++] = 74;
			buff[idx++] = 45;
			buff[idx++] = 88;
			buff[idx++] = 47;
			buff[idx++] = 96;
		}
	}

	public static void Read(byte[] buff, PlayerBase playerRef)
	{
		int idx = StorageHelper.CharacterDataOffset;
		if (StorageHelper.TestVersion(buff, ref idx, StorageHelper.CharacterVersion))
		{
			idx += StorageHelper.CharacterVersion.Length;
			int e = 0;
			StorageHelper.ReadInt(buff, ref idx, ref e);
			playerRef.CharacterIndex = buff[idx++];
			playerRef.ShirtIndex = (int)buff[idx++];
			playerRef.PantstIndex = (int)buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
			_ = buff[idx++];
		}
	}
}
