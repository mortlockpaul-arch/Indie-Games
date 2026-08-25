using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace DepthAttack.CS;

public class Continue : GameComponent
{
	public const string pcStrContainerName = "DepthAttack00";

	public const string pcStrContainerSaveDataName = "DepthAttackContinue.dat";

	public long plngScore = 0L;

	public int pintStage = 1;

	public bool pflgRead = false;

	public Continue(Game game)
		: base(game)
	{
		pflgRead = false;
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public void PlayerCopy(int playerNo)
	{
		Game1.score.plngScore = plngScore;
	}

	public void ContinueCopy(int playerNo)
	{
		plngScore = Game1.score.plngScore;
	}

	public void ContinueRead()
	{
		if (Game1.pStorageDevice == null)
		{
			return;
		}
		IAsyncResult asyncResult = Game1.pStorageDevice.BeginOpenContainer("DepthAttack00", null, null);
		try
		{
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = Game1.pStorageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (!storageContainer.FileExists("DepthAttackContinue.dat"))
			{
				storageContainer.Dispose();
				return;
			}
			try
			{
				Stream stream = storageContainer.OpenFile("DepthAttackContinue.dat", FileMode.Open);
				byte[] array = new byte[8];
				byte[] array2 = new byte[4];
				stream.Read(array, 0, 8);
				plngScore = BitConverter.ToInt64(array, 0);
				stream.Read(array2, 0, 4);
				pintStage = BitConverter.ToInt32(array2, 0);
				stream.Read(array2, 0, 4);
				stream.Close();
				pflgRead = true;
			}
			catch
			{
			}
			storageContainer.Dispose();
		}
		catch
		{
			Game1.pStorageDevice = null;
		}
	}

	public void ContinueSave()
	{
		if (Game1.pStorageDevice == null)
		{
			return;
		}
		IAsyncResult asyncResult = Game1.pStorageDevice.BeginOpenContainer("DepthAttack00", null, null);
		try
		{
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = Game1.pStorageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			try
			{
				Stream stream = storageContainer.OpenFile("DepthAttackContinue.dat", FileMode.Create);
				byte[] array = new byte[8];
				byte[] array2 = new byte[4];
				array = BitConverter.GetBytes(plngScore);
				stream.Write(array, 0, 8);
				array2 = BitConverter.GetBytes(pintStage);
				stream.Write(array2, 0, 4);
				stream.Close();
			}
			catch
			{
			}
			storageContainer.Dispose();
			pflgRead = true;
		}
		catch
		{
			Game1.pStorageDevice = null;
		}
	}

	private byte byteBool(bool flgBool)
	{
		if (flgBool)
		{
			return 1;
		}
		return 0;
	}

	private bool boolByte(byte flgByte)
	{
		if (flgByte == 1)
		{
			return true;
		}
		return false;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}
}
