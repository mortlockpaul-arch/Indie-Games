namespace EGEngine;

public class PlayerStatus : StorageHelper
{
	public static void Write(byte[] buff, PlayerBase playerRef)
	{
		if (DataEncoder.IsBusySave_Wait)
		{
			int idx = StorageHelper.StatusDataOffset;
			StorageHelper.SetVersion(buff, ref idx, StorageHelper.StatusVersion);
			StorageHelper.WriteInt(buff, ref idx, 70);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.vecPosition.X);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.vecPosition.Y);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.vecPosition.Z);
			StorageHelper.WriteInt(buff, ref idx, (int)(playerRef.vecDirection.X * 1024f));
			StorageHelper.WriteInt(buff, ref idx, (int)(playerRef.vecDirection.Y * 1024f));
			StorageHelper.WriteInt(buff, ref idx, (int)(playerRef.vecDirection.Z * 1024f));
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.Angles.X);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.Angles.Y);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.Angles.Z);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.BloodLevel);
			StorageHelper.WriteInt(buff, ref idx, (int)(playerRef.BloodLoss * 1024f));
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.WaterLevel);
			StorageHelper.WriteInt(buff, ref idx, (int)playerRef.FoodLevel);
			buff[idx++] = 128;
			buff[idx++] = 74;
			buff[idx++] = 45;
			buff[idx++] = 88;
			buff[idx++] = 108;
			buff[idx++] = 16;
			buff[idx++] = 47;
			buff[idx++] = 96;
			buff[idx++] = 12;
			buff[idx++] = 189;
			StorageHelper.WriteInt(buff, ref idx, playerRef.CurrentDay);
			StorageHelper.WriteInt(buff, ref idx, (int)(LevelOutside.SunAngle * 1024f));
		}
	}

	public static bool Read(byte[] buff, PlayerBase playerRef)
	{
		bool result = true;
		int idx = StorageHelper.StatusDataOffset;
		int e = 0;
		int e2 = 0;
		int e3 = 0;
		int e4 = 0;
		int e5 = 0;
		float x = 0f;
		float y = 0f;
		float z = 0f;
		int e6 = 0;
		int e7 = 0;
		int e8 = 0;
		int e9 = 0;
		int e10 = 0;
		int e11 = 0;
		int e12 = 0;
		int e13 = 0;
		int e14 = 0;
		if (StorageHelper.TestVersion(buff, ref idx, StorageHelper.StatusVersion))
		{
			idx += StorageHelper.StatusVersion.Length;
			StorageHelper.ReadInt(buff, ref idx, ref e);
			StorageHelper.ReadInt(buff, ref idx, ref e2);
			StorageHelper.ReadInt(buff, ref idx, ref e3);
			StorageHelper.ReadInt(buff, ref idx, ref e4);
			StorageHelper.ReadInt(buff, ref idx, ref e5);
			x = (float)e5 / 1024f;
			StorageHelper.ReadInt(buff, ref idx, ref e5);
			y = (float)e5 / 1024f;
			StorageHelper.ReadInt(buff, ref idx, ref e5);
			z = (float)e5 / 1024f;
			StorageHelper.ReadInt(buff, ref idx, ref e6);
			StorageHelper.ReadInt(buff, ref idx, ref e7);
			StorageHelper.ReadInt(buff, ref idx, ref e8);
			StorageHelper.ReadInt(buff, ref idx, ref e9);
			StorageHelper.ReadInt(buff, ref idx, ref e10);
			StorageHelper.ReadInt(buff, ref idx, ref e11);
			StorageHelper.ReadInt(buff, ref idx, ref e12);
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
			StorageHelper.ReadInt(buff, ref idx, ref e13);
			StorageHelper.ReadInt(buff, ref idx, ref e14);
		}
		if (e9 > 0 && e9 <= 100)
		{
			playerRef.vecPosition.X = e2;
			playerRef.vecPosition.Y = e3;
			playerRef.vecPosition.Z = e4;
			playerRef.vecDirection.X = x;
			playerRef.vecDirection.Y = y;
			playerRef.vecDirection.Z = z;
			playerRef.Angles.X = e6;
			playerRef.Angles.Y = e7;
			playerRef.Angles.Z = e8;
			playerRef.BloodLevel = e9;
			playerRef.BloodLoss = (float)e10 / 1024f;
			playerRef.WaterLevel = e11;
			playerRef.FoodLevel = e12;
			playerRef.CurrentDay = e13;
			LevelOutside.SunAngle = (float)e14 / 1024f;
		}
		else
		{
			result = false;
		}
		playerRef.ValidateNewPosition();
		return result;
	}
}
