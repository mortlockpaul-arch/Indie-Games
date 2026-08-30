using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Maximinus;

public class Storage
{
	private static string filename;

	private static bool IsInit
	{
		get
		{
			if (filename != null)
			{
				return filename != "";
			}
			return false;
		}
	}

	public static IsolatedStorageFile GetUserStore
	{
		get
		{
			if (!IsInit)
			{
				throw new Exception("Storage not initialized");
			}
			return IsolatedStorageFile.GetUserStoreForApplication();
		}
	}

	public static void Initialize(string fname)
	{
		filename = fname;
	}

	public static void RemoveSave()
	{
		try
		{
			if (GetUserStore.FileExists(filename))
			{
				GetUserStore.DeleteFile(filename);
			}
		}
		catch (Exception)
		{
		}
	}

	public static byte[] Load(int wantedBytes, out int readBytes)
	{
		IsolatedStorageFile getUserStore = GetUserStore;
		byte[] array = new byte[wantedBytes];
		readBytes = 0;
		if (getUserStore.FileExists(filename))
		{
			IsolatedStorageFileStream isolatedStorageFileStream = null;
			try
			{
				isolatedStorageFileStream = getUserStore.OpenFile(filename, FileMode.Open);
			}
			catch (IsolatedStorageException)
			{
			}
			if (isolatedStorageFileStream != null)
			{
				try
				{
					readBytes = isolatedStorageFileStream.Read(array, 0, wantedBytes);
				}
				catch (Exception)
				{
				}
				_ = readBytes;
				isolatedStorageFileStream.Close();
			}
		}
		return array;
	}

	public static bool Save(byte[] data)
	{
		if (!IsInit)
		{
			throw new Exception("Storage not initialized");
		}
		IsolatedStorageFileStream isolatedStorageFileStream = null;
		try
		{
			isolatedStorageFileStream = GetUserStore.OpenFile(filename, FileMode.Create);
			if (isolatedStorageFileStream != null)
			{
				isolatedStorageFileStream.Write(data, 0, data.Length);
				isolatedStorageFileStream.Close();
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}
}
