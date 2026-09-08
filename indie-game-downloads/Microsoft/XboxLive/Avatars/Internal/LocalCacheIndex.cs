using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class LocalCacheIndex
{
	public const int SerializeVersion = 1;

	public const int MaxFileNameLen = 255;

	public const double SessionMinDiffTimeSpan = 3600.0;

	public Dictionary<string, LocalCacheRecord> m_dRecords = new Dictionary<string, LocalCacheRecord>();

	public static AutoResetEvent m_writeFinished = new AutoResetEvent(initialState: false);

	public static int ReadInt(StreamReader sr)
	{
		int num = sr.Read();
		int num2 = sr.Read();
		int num3 = sr.Read();
		int num4 = sr.Read();
		if (num == -1 || num2 == -1 || num3 == -1 || num4 == -1)
		{
			return -1;
		}
		return (num4 << 24) | (num3 << 16) | (num2 << 8) | num;
	}

	public static long ReadLong(StreamReader sr)
	{
		int num = ReadInt(sr);
		int num2 = ReadInt(sr);
		if (num2 == -1)
		{
			return -1L;
		}
		return ((long)num2 << 32) | (uint)num;
	}

	public static void WriteInt(StreamWriter sw, int value)
	{
		sw.Write((char)(value & 0xFF));
		sw.Write((char)((value >> 8) & 0xFF));
		sw.Write((char)((value >> 16) & 0xFF));
		sw.Write((char)((value >> 24) & 0xFF));
	}

	public static void WriteLong(StreamWriter sw, long value)
	{
		WriteInt(sw, (int)value);
		WriteInt(sw, (int)(value >>> 32));
	}

	public void WriteFinished(IAsyncResult ar)
	{
		m_writeFinished.Set();
	}

	public bool Serialize(Stream stream)
	{
		if (stream == null)
		{
			return false;
		}
		StreamWriter streamWriter = new StreamWriter(new MemoryStream());
		WriteInt(streamWriter, 1);
		WriteInt(streamWriter, m_dRecords.Count);
		foreach (KeyValuePair<string, LocalCacheRecord> dRecord in m_dRecords)
		{
			WriteLong(streamWriter, dRecord.Value.FileSize);
			WriteLong(streamWriter, dRecord.Value.LastAccessTime);
			WriteInt(streamWriter, dRecord.Key.Length);
			streamWriter.Write(dRecord.Key);
		}
		streamWriter.Flush();
		long length = streamWriter.BaseStream.Length;
		if (length == 0)
		{
			return true;
		}
		streamWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
		byte[] buffer = new byte[length];
		streamWriter.BaseStream.Read(buffer, 0, (int)length);
		IAsyncResult asyncResult = stream.BeginWrite(buffer, 0, (int)length, WriteFinished, null);
		m_writeFinished.WaitOne();
		stream.EndWrite(asyncResult);
		return true;
	}

	public bool Deserialize(Stream stream)
	{
		if (stream == null)
		{
			return false;
		}
		StreamReader streamReader = new StreamReader(stream);
		StringBuilder stringBuilder = new StringBuilder();
		int num = ReadInt(streamReader);
		if (num != 1)
		{
			return false;
		}
		m_dRecords.Clear();
		int num2 = ReadInt(streamReader);
		if (num2 < 0)
		{
			return false;
		}
		int num3 = 0;
		while (num3 < num2)
		{
			long num4 = ReadLong(streamReader);
			if (num4 == -1)
			{
				break;
			}
			long num5 = ReadLong(streamReader);
			if (num5 == -1 || num3 > num2)
			{
				break;
			}
			int num6 = ReadInt(streamReader);
			if (num6 < 0 || num6 > 255)
			{
				break;
			}
			char[] array = new char[num6];
			if (streamReader.ReadBlock(array, 0, num6) != num6)
			{
				break;
			}
			stringBuilder.Append(array, 0, num6);
			m_dRecords.Add(stringBuilder.ToString(), new LocalCacheRecord(num4, num5));
			num3++;
			stringBuilder.Remove(0, num6);
		}
		return num3 == num2;
	}

	public void Initialize()
	{
		m_dRecords.Clear();
	}

	public void Initialize(string[] fileNames)
	{
		m_dRecords.Clear();
		int length = fileNames.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			m_dRecords.Add(fileNames[i], new LocalCacheRecord(0L, 0L));
		}
	}

	public bool TryGetFileInfo(string fileName, out LocalCacheRecord fileInfo)
	{
		return m_dRecords.TryGetValue(fileName, out fileInfo);
	}

	public void UpdateFileInfo(string fileName, LocalCacheRecord fileInfo)
	{
		if (m_dRecords.TryGetValue(fileName, out var value))
		{
			value.LastAccessTime = fileInfo.LastAccessTime;
			value.FileSize = fileInfo.FileSize;
		}
		else
		{
			m_dRecords.Add(fileName, fileInfo);
		}
	}

	public bool Remove(string fileName)
	{
		return m_dRecords.Remove(fileName);
	}

	public void Merge(LocalCacheIndex localCacheIndex)
	{
		foreach (KeyValuePair<string, LocalCacheRecord> dRecord in m_dRecords)
		{
			if (!localCacheIndex.TryGetFileInfo(dRecord.Key, out var _))
			{
				localCacheIndex.UpdateFileInfo(dRecord.Key, dRecord.Value);
			}
		}
	}

	public void UpdateBy(LocalCacheIndex localCacheIndex)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, LocalCacheRecord> dRecord in m_dRecords)
		{
			if (localCacheIndex.TryGetFileInfo(dRecord.Key, out var fileInfo))
			{
				if (fileInfo.LastAccessTime != 0)
				{
					dRecord.Value.LastAccessTime = fileInfo.LastAccessTime;
					dRecord.Value.FileSize = fileInfo.FileSize;
				}
			}
			else
			{
				list.Add(dRecord.Key);
			}
		}
		foreach (string item in list)
		{
			m_dRecords.Remove(item);
		}
		localCacheIndex.Merge(this);
	}

	public static void InsertCandidate(LinkedList<LocalCacheSortRecord> sortedCandidates, string candidateName, LocalCacheRecord candidate)
	{
		if (candidate.LastAccessTime == 0)
		{
			sortedCandidates.AddFirst(new LocalCacheSortRecord(candidateName, candidate));
			return;
		}
		for (LinkedListNode<LocalCacheSortRecord> linkedListNode = sortedCandidates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value.LastAccessTime != 0L && linkedListNode.Value.LastAccessTime >= candidate.LastAccessTime)
			{
				sortedCandidates.AddBefore(linkedListNode, new LocalCacheSortRecord(candidateName, candidate));
				return;
			}
		}
		sortedCandidates.AddLast(new LocalCacheSortRecord(candidateName, candidate));
	}

	public static int CompareByFileSizeDescending(LocalCacheSortRecord a, LocalCacheSortRecord b)
	{
		if (a == null)
		{
			if (b == null)
			{
				return 0;
			}
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		if (a.FileSize == b.FileSize)
		{
			return 0;
		}
		if (a.FileSize > b.FileSize)
		{
			return -1;
		}
		return 1;
	}

	public List<string> GetListOfReleaseCandidates(long requiredSize)
	{
		List<string> list = new List<string>();
		LinkedList<LocalCacheSortRecord> linkedList = new LinkedList<LocalCacheSortRecord>();
		foreach (KeyValuePair<string, LocalCacheRecord> dRecord in m_dRecords)
		{
			InsertCandidate(linkedList, dRecord.Key, dRecord.Value);
		}
		List<LocalCacheSortRecord> list2 = new List<LocalCacheSortRecord>();
		LinkedListNode<LocalCacheSortRecord> linkedListNode = linkedList.First;
		long num = linkedListNode?.Value.LastAccessTime ?? 0;
		long num2 = 0L;
		while (true)
		{
			bool flag = true;
			if (linkedListNode == null || (linkedListNode.Value.LastAccessTime != 0L && new TimeSpan(linkedListNode.Value.LastAccessTime - num).TotalSeconds > 3600.0))
			{
				list2.Sort(CompareByFileSizeDescending);
				foreach (LocalCacheSortRecord item in list2)
				{
					list.Add(item.FileName);
				}
				if (num2 >= requiredSize || linkedListNode == null)
				{
					break;
				}
				num = linkedListNode.Value.LastAccessTime;
				list2.Clear();
			}
			list2.Add(linkedListNode.Value);
			if (linkedListNode.Value.LastAccessTime != 0)
			{
				num2 += linkedListNode.Value.FileSize;
			}
			linkedListNode = linkedListNode.Next;
		}
		return list;
	}
}
