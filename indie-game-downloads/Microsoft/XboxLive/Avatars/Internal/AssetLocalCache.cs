using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetLocalCache
{
	public const string LocalCacheDirectory = "AvatarCache";

	public const string LocalCacheFileIndex = "_CacheIndex.bin";

	public const long DefaultLocalCacheQuota = 1048576L;

	public const long StandardLocalCacheQuota = 12582912L;

	public const long ReserveForLocalCacheQuota = 32768L;

	public static object m_fileAccesLock = new object();

	public static long m_currentQuotaSize = -1L;

	public static bool m_cacheInitialized;

	public long m_askedQuotaSize = 1048576L;

	public long m_reserveForLocalCacheQuota = 32768L;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public EventHandler<IncreaseQuotaEventArgs> IncreaseQuota;

	public long QuotaSize => m_currentQuotaSize;

	public event EventHandler<IncreaseQuotaEventArgs> IncreaseQuota
	{
		[CompilerGenerated]
		add
		{
			EventHandler<IncreaseQuotaEventArgs> eventHandler = this.IncreaseQuota;
			EventHandler<IncreaseQuotaEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<IncreaseQuotaEventArgs> value2 = (EventHandler<IncreaseQuotaEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.IncreaseQuota, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<IncreaseQuotaEventArgs> eventHandler = this.IncreaseQuota;
			EventHandler<IncreaseQuotaEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<IncreaseQuotaEventArgs> value2 = (EventHandler<IncreaseQuotaEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.IncreaseQuota, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public static string TranslateAddressToLocalCachePath(string address)
	{
		string fileName = Path.GetFileName(address);
		return Path.Combine("AvatarCache", fileName);
	}

	public void StoreIsolatedFile(IsolatedStorageFile store, string path, Stream stream)
	{
		try
		{
			if (!LocalCacheInitialize(store))
			{
				return;
			}
			using IsolatedStorageFileStream isolatedStorageFileStream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, store);
			if (isolatedStorageFileStream == null)
			{
				return;
			}
			long position = stream.Position;
			byte[] array = new byte[32768];
			while (true)
			{
				bool flag = true;
				int num = stream.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				isolatedStorageFileStream.Write(array, 0, num);
			}
			isolatedStorageFileStream.Flush();
			stream.Position = position;
		}
		catch (IsolatedStorageException)
		{
		}
	}

	public Stream ReadIsolatedFile(IsolatedStorageFile isf, string path)
	{
		try
		{
			if (!LocalCacheInitialize(isf))
			{
				return null;
			}
			using IsolatedStorageFileStream isolatedStorageFileStream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, isf);
			if (isolatedStorageFileStream != null)
			{
				Stream stream = new MemoryStream();
				byte[] array = new byte[32768];
				while (true)
				{
					bool flag = true;
					int num = isolatedStorageFileStream.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					stream.Write(array, 0, num);
				}
				stream.Seek(0L, SeekOrigin.Begin);
				return stream;
			}
		}
		catch (IsolatedStorageException)
		{
		}
		return null;
	}

	public static bool LocalCacheInitialize(IsolatedStorageFile store)
	{
		bool flag = m_cacheInitialized;
		if (!m_cacheInitialized)
		{
			try
			{
				if (!store.DirectoryExists("AvatarCache"))
				{
					store.CreateDirectory("AvatarCache");
				}
				m_currentQuotaSize = store.Quota;
				flag = InitializeIndexFile(store);
			}
			catch (IsolatedStorageException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
			m_cacheInitialized = flag;
		}
		return flag;
	}

	public void UpdateAccessTime(IsolatedStorageFile store, string fileName, long fileAccessTime)
	{
		if (!LocalCacheInitialize(store))
		{
			return;
		}
		string path = Path.Combine("AvatarCache", "_CacheIndex.bin");
		if (!store.FileExists(path))
		{
			return;
		}
		LocalCacheIndex localCacheIndex = new LocalCacheIndex();
		using IsolatedStorageFileStream isolatedStorageFileStream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, store);
		if (localCacheIndex.Deserialize(isolatedStorageFileStream) && localCacheIndex.TryGetFileInfo(fileName, out var fileInfo))
		{
			localCacheIndex.UpdateFileInfo(fileName, new LocalCacheRecord(fileInfo.FileSize, fileAccessTime));
			isolatedStorageFileStream.Seek(0L, SeekOrigin.Begin);
			localCacheIndex.Serialize(isolatedStorageFileStream);
		}
	}

	public Stream ReadFileFromStorage(string readFilePath)
	{
		Stream stream = null;
		try
		{
			using IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
			if (isolatedStorageFile.FileExists(readFilePath))
			{
				stream = ReadIsolatedFile(isolatedStorageFile, readFilePath);
				if (stream != null)
				{
					UpdateAccessTime(isolatedStorageFile, Path.GetFileName(readFilePath), DateTime.Now.Ticks);
				}
			}
		}
		catch (IsolatedStorageException)
		{
		}
		catch (IOException)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		catch (InvalidOperationException)
		{
		}
		return stream;
	}

	public void StoreEvent(string filePath, Stream writeStream)
	{
		try
		{
			using IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
			if (isolatedStorageFile.FileExists(filePath))
			{
				isolatedStorageFile.DeleteFile(filePath);
			}
			if (IsAvailableSpace(isolatedStorageFile, writeStream))
			{
				StoreIsolatedFile(isolatedStorageFile, filePath, writeStream);
				UpdateIndexFile(isolatedStorageFile, Path.GetFileName(filePath), writeStream.Length, DateTime.Now.Ticks);
			}
		}
		catch (IsolatedStorageException)
		{
		}
		catch (IOException)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		catch (InvalidOperationException)
		{
		}
	}

	public void UpdateIndexFile(IsolatedStorageFile isf, string fileName, long fileLength, long fileAccessTime)
	{
		if (!LocalCacheInitialize(isf))
		{
			return;
		}
		string path = Path.Combine("AvatarCache", "_CacheIndex.bin");
		if (!isf.FileExists(path))
		{
			return;
		}
		LocalCacheIndex localCacheIndex = new LocalCacheIndex();
		bool flag = false;
		using (IsolatedStorageFileStream stream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, isf))
		{
			flag = localCacheIndex.Deserialize(stream);
			if (flag)
			{
				localCacheIndex.UpdateFileInfo(fileName, new LocalCacheRecord(fileLength, fileAccessTime));
			}
		}
		if (!flag)
		{
			return;
		}
		using IsolatedStorageFileStream isolatedStorageFileStream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Open, FileAccess.Write, FileShare.None, isf);
		if (isolatedStorageFileStream != null)
		{
			localCacheIndex.Serialize(isolatedStorageFileStream);
			isolatedStorageFileStream.Flush();
		}
	}

	public static bool InitializeIndexFile(IsolatedStorageFile isf)
	{
		bool result = false;
		if (!isf.DirectoryExists("AvatarCache"))
		{
			return false;
		}
		string[] fileNames = isf.GetFileNames(Path.Combine("AvatarCache", "*.*"));
		string path = Path.Combine("AvatarCache", "_CacheIndex.bin");
		LocalCacheIndex localCacheIndex = new LocalCacheIndex();
		localCacheIndex.Initialize(fileNames);
		localCacheIndex.Remove("_CacheIndex.bin");
		if (!isf.FileExists(path))
		{
			using IsolatedStorageFileStream isolatedStorageFileStream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, isf);
			if (isolatedStorageFileStream != null)
			{
				result = localCacheIndex.Serialize(isolatedStorageFileStream);
				isolatedStorageFileStream.Flush();
			}
		}
		else
		{
			LocalCacheIndex localCacheIndex2 = new LocalCacheIndex();
			bool flag = false;
			using (IsolatedStorageFileStream isolatedStorageFileStream2 = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, isf))
			{
				if (isolatedStorageFileStream2 != null)
				{
					flag = localCacheIndex2.Deserialize(isolatedStorageFileStream2);
				}
			}
			using IsolatedStorageFileStream isolatedStorageFileStream3 = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None, isf);
			if (isolatedStorageFileStream3 != null)
			{
				if (flag)
				{
					localCacheIndex2.UpdateBy(localCacheIndex);
					result = localCacheIndex2.Serialize(isolatedStorageFileStream3);
				}
				else
				{
					result = localCacheIndex.Serialize(isolatedStorageFileStream3);
				}
				isolatedStorageFileStream3.Flush();
			}
		}
		return result;
	}

	public bool IsAvailableSpace(IsolatedStorageFile store, Stream writeStream)
	{
		long num = writeStream.Length + m_reserveForLocalCacheQuota;
		if (store.AvailableFreeSpace < num)
		{
			long num2 = ((m_currentQuotaSize == -1 || m_currentQuotaSize > store.Quota) ? store.Quota : m_currentQuotaSize);
			if (this.IncreaseQuota != null && m_askedQuotaSize <= num2)
			{
				long num3 = num2;
				long recommendedQuota = (m_askedQuotaSize = ((num3 > 1048576) ? (num3 + num3 / 2) : 12582912));
				IncreaseQuotaEventArgs e = new IncreaseQuotaEventArgs(store.AvailableFreeSpace, store.Quota, recommendedQuota);
				this.IncreaseQuota(this, e);
			}
			TryReleaseCacheSpace(store, num - store.AvailableFreeSpace);
			if (store.AvailableFreeSpace < num)
			{
				return false;
			}
		}
		return true;
	}

	public static long TryReleaseCacheSpace(IsolatedStorageFile isf, long requiredSize)
	{
		if (!LocalCacheInitialize(isf))
		{
			return 0L;
		}
		string path = Path.Combine("AvatarCache", "_CacheIndex.bin");
		if (!isf.FileExists(path))
		{
			return 0L;
		}
		long num = 0L;
		bool flag = true;
		LocalCacheIndex localCacheIndex = new LocalCacheIndex();
		using (IsolatedStorageFileStream isolatedStorageFileStream = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, isf))
		{
			if (isolatedStorageFileStream == null || !localCacheIndex.Deserialize(isolatedStorageFileStream))
			{
				return 0L;
			}
			List<string> listOfReleaseCandidates = localCacheIndex.GetListOfReleaseCandidates(requiredSize);
			foreach (string item in listOfReleaseCandidates)
			{
				string text = Path.Combine("AvatarCache", item);
				if (isf.FileExists(text))
				{
					isf.DeleteFile(text);
				}
				if (localCacheIndex.TryGetFileInfo(item, out var fileInfo))
				{
					if (fileInfo.LastAccessTime != 0)
					{
						num += fileInfo.FileSize;
					}
					else
					{
						flag = false;
					}
				}
				localCacheIndex.Remove(item);
				if (flag)
				{
					if (requiredSize <= num)
					{
						break;
					}
				}
				else if (requiredSize <= isf.AvailableFreeSpace)
				{
					break;
				}
			}
		}
		if (num != 0L || !flag)
		{
			using IsolatedStorageFileStream isolatedStorageFileStream2 = IsolatedStorageAccess.CreateIsolatedStorageFileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None, isf);
			if (isolatedStorageFileStream2 != null)
			{
				localCacheIndex.Serialize(isolatedStorageFileStream2);
				isolatedStorageFileStream2.Flush();
				isolatedStorageFileStream2.Close();
			}
		}
		return flag ? num : (-num);
	}

	public Stream TryOpen(string address)
	{
		Stream result = null;
		if (address != null)
		{
			string readFilePath = TranslateAddressToLocalCachePath(address);
			lock (m_fileAccesLock)
			{
				result = ReadFileFromStorage(readFilePath);
			}
		}
		return result;
	}

	public void Store(string address, Stream stream)
	{
		if (address != null)
		{
			string filePath = TranslateAddressToLocalCachePath(address);
			lock (m_fileAccesLock)
			{
				StoreEvent(filePath, stream);
			}
		}
	}

	public bool IncreaseQuotaTo(long size)
	{
		if (size == -1)
		{
			return false;
		}
		try
		{
			using IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
			if (size > isolatedStorageFile.Quota)
			{
				isolatedStorageFile.IncreaseQuotaTo(size);
				m_currentQuotaSize = isolatedStorageFile.Quota;
			}
		}
		catch (IsolatedStorageException)
		{
		}
		catch (IOException)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		catch (InvalidOperationException)
		{
		}
		return m_currentQuotaSize == size;
	}

	public static void Clean()
	{
		lock (m_fileAccesLock)
		{
			try
			{
				using IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
				TryReleaseCacheSpace(isf, long.MaxValue);
			}
			catch (IsolatedStorageException)
			{
			}
			catch (IOException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
		}
	}
}
