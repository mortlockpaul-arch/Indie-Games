using System;
using System.IO;
using DebugSample;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Storage;
using XSIXNARuntime;
using XnaLibrary.Blade;

namespace Infinity;

internal class Global
{
	public const string ApplicationName = "3D Infinity";

	public const string SystemFileName = "System.dat";

	public const int SCORE_MAX = 9999999;

	public static Cue bgm;

	public static XSISASContainer SASData;

	public static SaveData SaveData;

	public static readonly Rectangle ScreenArea;

	public static PlayerIndex CurrentPlayer;

	public static float GameSpeed { get; set; }

	public static TimeRuler TimeRuler { get; set; }

	public static AsyncLoader AsyncLoader { get; set; }

	public static bool Save(StorageComponent storage)
	{
		try
		{
			if (!IsStorageConnected(storage))
			{
				return false;
			}
			try
			{
				StorageContainer val = storage.OpenContainer();
				try
				{
					string storagePath = StorageComponent.GetStoragePath(val, "System.dat");
					if (string.IsNullOrEmpty(storagePath))
					{
						return false;
					}
					using BinaryWriter writer = new BinaryWriter(File.OpenWrite(storagePath));
					SaveData.Write(writer);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (IOException)
			{
				return false;
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool Load(StorageComponent storage)
	{
		try
		{
			if (!IsStorageConnected(storage))
			{
				return false;
			}
			try
			{
				StorageContainer val = storage.OpenContainer();
				try
				{
					string storagePath = StorageComponent.GetStoragePath(val, "System.dat");
					if (string.IsNullOrEmpty(storagePath))
					{
						return false;
					}
					if (!File.Exists(storagePath))
					{
						return false;
					}
					using BinaryReader reader = new BinaryReader(File.OpenRead(storagePath));
					SaveData = SaveData.Read(reader, SaveData);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (IOException)
			{
				SaveData = new SaveData();
				return false;
			}
			catch (ArgumentException)
			{
				SaveData = new SaveData();
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				SaveData = new SaveData();
				return false;
			}
			catch (Exception)
			{
				SaveData = new SaveData();
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static bool IsStorageConnected(StorageComponent storage)
	{
		if (storage == null || !storage.IsConnected)
		{
			return false;
		}
		return true;
	}

	static Global()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		bgm = null;
		SASData = new XSISASContainer();
		SaveData = new SaveData();
		ScreenArea = new Rectangle(0, 0, 1280, 720);
		CurrentPlayer = (PlayerIndex)0;
	}
}
