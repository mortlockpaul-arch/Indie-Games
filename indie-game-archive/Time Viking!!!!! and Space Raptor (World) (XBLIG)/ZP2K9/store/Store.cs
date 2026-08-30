using System;
using System.IO;
using System.Threading;
using IMAK3Z0MB1EGAEM;
using IMAK3Z0MB1EGAEM.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace ZP2K9.store;

public class Store
{
	public const int STORE_SCORES = 0;

	public StorageDevice device;

	private IAsyncResult deviceResult;

	public StorageContainer container;

	public bool pendingDevice;

	private bool containerOpen;

	public bool retry;

	public bool failed;

	private int threadedWriteType;

	public string mapPath = "map";

	public void GetDevice()
	{
		if (!Guide.IsVisible)
		{
			deviceResult = StorageDevice.BeginShowSelector((PlayerIndex)ZombieGame.mainPlayerIndex, null, null);
			pendingDevice = true;
		}
		else
		{
			retry = true;
		}
	}

	public void Update()
	{
		if (retry)
		{
			GetDevice();
		}
		if (pendingDevice && deviceResult.IsCompleted)
		{
			device = StorageDevice.EndShowSelector(deviceResult);
			pendingDevice = false;
			if (!CheckDeviceFail())
			{
				Read();
			}
		}
	}

	private bool CheckDeviceFail()
	{
		if (pendingDevice)
		{
			return true;
		}
		if (device == null)
		{
			return true;
		}
		if (!device.IsConnected)
		{
			return true;
		}
		return false;
	}

	private void OpenContainer()
	{
		if (!containerOpen)
		{
			IAsyncResult asyncResult = device.BeginOpenContainer("zp2k5", null, null);
			long ticks = DateTime.UtcNow.Ticks;
			while (!asyncResult.IsCompleted)
			{
				long num = DateTime.UtcNow.Ticks - ticks;
				float num2 = (float)num / 10000000f;
				if (num2 > 1f)
				{
					throw new Exception("Open container timed out.");
				}
			}
			container = device.EndOpenContainer(asyncResult);
		}
		containerOpen = true;
	}

	private void CommitContainer()
	{
		containerOpen = false;
		container.Dispose();
		while (!container.IsDisposed)
		{
		}
	}

	public void Write()
	{
		if (device == null)
		{
			return;
		}
		Thread thread = new Thread(ThreadedWrite);
		lock (device)
		{
			thread.Start();
		}
	}

	public void ThreadedWrite()
	{
		if (CheckDeviceFail())
		{
			return;
		}
		try
		{
			OpenContainer();
		}
		catch
		{
			return;
		}
		string text = "";
		text = "scores.zdx";
		try
		{
			BinaryWriter binaryWriter = new BinaryWriter(container.OpenFile(text, FileMode.OpenOrCreate, FileAccess.Write));
			try
			{
				HighScores.Write(binaryWriter);
			}
			catch
			{
			}
			binaryWriter.Close();
			CommitContainer();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	public void Read()
	{
		if (CheckDeviceFail())
		{
			return;
		}
		try
		{
			OpenContainer();
		}
		catch
		{
			return;
		}
		string text = "";
		text = "scores.zdx";
		if (!container.FileExists(text))
		{
			HighScores.Init();
			return;
		}
		try
		{
			BinaryReader binaryReader = new BinaryReader(container.OpenFile(text, FileMode.Open, FileAccess.Read));
			try
			{
				HighScores.Read(binaryReader);
			}
			catch
			{
			}
			binaryReader.Close();
			CommitContainer();
		}
		catch
		{
		}
	}
}
