using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Quasar.ContentPipeline;

namespace Quasar.Global;

public static class DirectoryManager
{
	public class FileInfo
	{
		private string name;

		private string fullPath;

		public string Name => name;

		public string FullPath => fullPath;

		public FileInfo(string name, DirectoryInfo parent)
		{
			this.name = name;
			if (parent != null)
			{
				fullPath = parent.FullPath + name;
			}
			else
			{
				fullPath = name;
			}
		}
	}

	public class DirectoryInfo
	{
		private string name;

		private string fullPath;

		private List<FileInfo> files = new List<FileInfo>();

		private List<DirectoryInfo> directories = new List<DirectoryInfo>();

		public string Name => name;

		public string FullPath => fullPath;

		public DirectoryInfo(string name, DirectoryInfo parent)
		{
			this.name = name;
			if (parent != null)
			{
				fullPath = parent.FullPath + name + '/';
			}
			else
			{
				fullPath = ((name.Length > 0) ? (name + '/') : "");
			}
		}

		public void AddFile(FileInfo file)
		{
			files.Add(file);
		}

		public void AddDirectory(DirectoryInfo directory)
		{
			directories.Add(directory);
		}

		public DirectoryInfo GetDirectory(string path)
		{
			foreach (DirectoryInfo directory in directories)
			{
				if (string.Equals(path, directory.Name, StringComparison.InvariantCultureIgnoreCase))
				{
					return directory;
				}
			}
			return null;
		}

		public FileInfo[] GetFiles(string pattern)
		{
			pattern = pattern.Replace(".", "\\.").Replace('?', '.').Replace("*", ".*");
			FileInfo[] array = new FileInfo[files.Count];
			int newSize = 0;
			foreach (FileInfo file in files)
			{
				if (Regex.IsMatch(file.Name, pattern))
				{
					array[newSize++] = file;
				}
			}
			Array.Resize(ref array, newSize);
			return array;
		}

		public FileInfo[] GetFiles()
		{
			return GetFiles(recursive: false);
		}

		public bool FileExists(string filename)
		{
			foreach (FileInfo file in files)
			{
				if (string.Equals(filename, file.Name, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public int GetFileCount(bool recursive)
		{
			int num = files.Count;
			if (recursive)
			{
				foreach (DirectoryInfo directory in directories)
				{
					num += directory.GetFileCount(recursive: true);
				}
			}
			return num;
		}

		private int GetFiles(FileInfo[] f, int index, bool recursive)
		{
			int num = 0;
			foreach (FileInfo file in files)
			{
				f[index + num] = file;
				num++;
			}
			if (recursive)
			{
				foreach (DirectoryInfo directory in directories)
				{
					num += directory.GetFiles(f, index + num, recursive: true);
				}
			}
			return num;
		}

		public FileInfo[] GetFiles(bool recursive)
		{
			int fileCount = GetFileCount(recursive);
			FileInfo[] array = new FileInfo[fileCount];
			int num = 0;
			foreach (FileInfo file in files)
			{
				array[num++] = file;
			}
			if (recursive)
			{
				foreach (DirectoryInfo directory in directories)
				{
					num += directory.GetFiles(array, num, recursive: true);
				}
			}
			return array;
		}
	}

	private static DirectoryInfo rootInfo;

	public static FileInfo[] GetFiles(string path, string pattern)
	{
		DirectoryInfo directory = GetDirectory(path);
		if (directory == null)
		{
			return new FileInfo[0];
		}
		return directory.GetFiles(pattern);
	}

	public static FileInfo[] GetFiles(string path, bool recursive)
	{
		DirectoryInfo directory = GetDirectory(path);
		if (directory == null)
		{
			return new FileInfo[0];
		}
		return directory.GetFiles(recursive);
	}

	public static DirectoryInfo GetDirectory(string path)
	{
		string[] array = path.Split('/');
		int i = 0;
		int num = array.Length;
		if (path.StartsWith("/"))
		{
			i++;
		}
		if (path.EndsWith("/"))
		{
			num--;
		}
		DirectoryInfo directory = rootInfo;
		for (; i < num; i++)
		{
			directory = directory.GetDirectory(array[i]);
			if (directory == null)
			{
				return null;
			}
		}
		return directory;
	}

	public static DirectoryInfo GetDirectoryFromPath(string filepath)
	{
		string[] array = filepath.Split('/');
		int i = 0;
		int num = array.Length - 1;
		if (filepath.StartsWith("/"))
		{
			i++;
		}
		DirectoryInfo directory = rootInfo;
		for (; i < num; i++)
		{
			directory = directory.GetDirectory(array[i]);
			if (directory == null)
			{
				return null;
			}
		}
		return directory;
	}

	public static bool FileExists(string filepath)
	{
		string[] array = filepath.Split('/');
		int i = 0;
		int num = array.Length - 1;
		if (filepath.StartsWith("/"))
		{
			i++;
		}
		DirectoryInfo directory = rootInfo;
		for (; i < num; i++)
		{
			directory = directory.GetDirectory(array[i]);
			if (directory == null)
			{
				return false;
			}
		}
		return directory.FileExists(array[array.Length - 1]);
	}

	public static FileInfo[] GetFilesFromPath(string filepath)
	{
		string[] array = filepath.Split('/');
		int i = 0;
		int num = array.Length - 1;
		if (filepath.StartsWith("/"))
		{
			i++;
		}
		DirectoryInfo directory = rootInfo;
		for (; i < num; i++)
		{
			directory = directory.GetDirectory(array[i]);
			if (directory == null)
			{
				return new FileInfo[0];
			}
		}
		string pattern = array[array.Length - 1];
		return directory.GetFiles(pattern);
	}

	public static void Initialize()
	{
		try
		{
			rootInfo = new DirectoryInfo("", null);
			if (File.Exists(Engine.ContentManager.RootDirectory + "/DirectoryInfo.xnb"))
			{
				XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("DirectoryInfo");
				XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
				LoadDirectory(xDocument.Root, rootInfo);
			}
			else
			{
				LoadDirectory(Engine.ContentManager.RootDirectory, rootInfo);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void LoadDirectory(XElement xe, DirectoryInfo directory)
	{
		foreach (XElement item in xe.Elements("Directory"))
		{
			DirectoryInfo directory2 = new DirectoryInfo(XDocHelper.GetAttribute(item, "name"), directory);
			directory.AddDirectory(directory2);
			LoadDirectory(item, directory2);
		}
		foreach (XElement item2 in xe.Elements("File"))
		{
			FileInfo file = new FileInfo(XDocHelper.GetAttribute(item2, "name"), directory);
			directory.AddFile(file);
		}
	}

	private static void LoadDirectory(string path, DirectoryInfo directory)
	{
		System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
		LoadDirectory(di, directory);
	}

	private static void LoadDirectory(System.IO.DirectoryInfo di, DirectoryInfo directory)
	{
		System.IO.FileInfo[] files = di.GetFiles();
		foreach (System.IO.FileInfo fileInfo in files)
		{
			FileInfo file = new FileInfo(Path.GetFileNameWithoutExtension(fileInfo.Name), directory);
			directory.AddFile(file);
		}
		System.IO.DirectoryInfo[] directories = di.GetDirectories();
		foreach (System.IO.DirectoryInfo directoryInfo in directories)
		{
			DirectoryInfo directory2 = new DirectoryInfo(directoryInfo.Name, directory);
			LoadDirectory(directoryInfo, directory2);
			directory.AddDirectory(directory2);
		}
	}
}
