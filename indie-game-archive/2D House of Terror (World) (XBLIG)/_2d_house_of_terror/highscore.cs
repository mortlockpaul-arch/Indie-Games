using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace _2d_house_of_terror;

public class highscore
{
	public class list
	{
		public string[] names;

		public long[] scores;

		public short[] face_id;

		public list()
		{
		}

		public list(int entry_num)
		{
			names = new string[entry_num];
			scores = new long[entry_num];
			face_id = new short[entry_num];
		}
	}

	private list scorelist;

	private string list_filename;

	public highscore(string filename)
	{
		list_filename = filename;
		scorelist = new list(4);
		if (!load())
		{
			fill_default();
		}
	}

	private void fill_default()
	{
		for (int i = 0; i < scorelist.names.Length; i++)
		{
			scorelist.names[i] = ((i % 4 == 0) ? "Jimmy" : ((i % 4 == 1) ? "Sam" : ((i % 4 == 3) ? "Erik" : "Billy")));
			scorelist.face_id[i] = (short)(i % 4);
			scorelist.scores[i] = 0L;
		}
	}

	public bool load()
	{
		if (game_mgr.use_storage)
		{
			if (game_mgr.storage_dev != null && game_mgr.storage_dev.IsConnected)
			{
				StorageContainer storageContainer = null;
				try
				{
					IAsyncResult asyncResult = game_mgr.storage_dev.BeginOpenContainer("2DHouseOfTerror", null, null);
					asyncResult.AsyncWaitHandle.WaitOne();
					storageContainer = game_mgr.storage_dev.EndOpenContainer(asyncResult);
					asyncResult.AsyncWaitHandle.Close();
				}
				catch
				{
					storageContainer?.Dispose();
					return false;
				}
				if (storageContainer == null)
				{
					return false;
				}
				if (!storageContainer.FileExists(list_filename))
				{
					storageContainer.Dispose();
					return false;
				}
				Stream stream;
				try
				{
					stream = storageContainer.OpenFile(list_filename, FileMode.Open);
				}
				catch (IOException)
				{
					storageContainer.Dispose();
					return false;
				}
				if (stream == null)
				{
					storageContainer.Dispose();
					return false;
				}
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(list));
				try
				{
					scorelist = (list)xmlSerializer.Deserialize(stream);
				}
				catch (InvalidOperationException)
				{
					storageContainer.Dispose();
					return false;
				}
				stream.Close();
				try
				{
					storageContainer.Dispose();
				}
				catch (InvalidOperationException)
				{
					return true;
				}
				return true;
			}
			return false;
		}
		fill_default();
		return true;
	}

	public bool save()
	{
		if (game_mgr.use_storage)
		{
			if (game_mgr.storage_dev != null && game_mgr.storage_dev.IsConnected)
			{
				StorageContainer storageContainer = null;
				try
				{
					IAsyncResult asyncResult = game_mgr.storage_dev.BeginOpenContainer("2DHouseOfTerror", null, null);
					asyncResult.AsyncWaitHandle.WaitOne();
					storageContainer = game_mgr.storage_dev.EndOpenContainer(asyncResult);
					asyncResult.AsyncWaitHandle.Close();
				}
				catch
				{
					storageContainer?.Dispose();
					return false;
				}
				if (storageContainer == null)
				{
					return false;
				}
				if (storageContainer.FileExists(list_filename))
				{
					storageContainer.DeleteFile(list_filename);
				}
				Stream stream = storageContainer.CreateFile(list_filename);
				if (stream == null)
				{
					storageContainer.Dispose();
					return false;
				}
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(list));
				try
				{
					xmlSerializer.Serialize(stream, scorelist);
				}
				catch (InvalidOperationException)
				{
					storageContainer.Dispose();
					return false;
				}
				stream.Close();
				try
				{
					storageContainer.Dispose();
				}
				catch
				{
					return false;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public bool is_highscore(long score)
	{
		return score > scorelist.scores[scorelist.scores.Length - 1];
	}

	public void insert_new(long score, string name, short face_id)
	{
		if (!is_highscore(score))
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < scorelist.scores.Length; i++)
		{
			if (score > scorelist.scores[i])
			{
				num = i;
				break;
			}
		}
		for (int num2 = scorelist.scores.Length - 1; num2 > num; num2--)
		{
			scorelist.scores[num2] = scorelist.scores[num2 - 1];
			scorelist.names[num2] = scorelist.names[num2 - 1];
			scorelist.face_id[num2] = scorelist.face_id[num2 - 1];
		}
		scorelist.scores[num] = score;
		scorelist.names[num] = name;
		scorelist.face_id[num] = face_id;
	}

	public list get_list()
	{
		return scorelist;
	}
}
