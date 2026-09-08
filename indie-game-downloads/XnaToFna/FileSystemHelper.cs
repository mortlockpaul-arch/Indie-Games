using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XnaToFna;

public static class FileSystemHelper
{
	public static readonly bool MONO_IOMAP_ALL = Environment.GetEnvironmentVariable("MONO_IOMAP") == "all";

	public static readonly char[] DirectorySeparatorChars = new char[2] { '/', '\\' };

	private static Dictionary<string, string[]> _CachedDirectories = new Dictionary<string, string[]>();

	private static Dictionary<string, string[]> _CachedTargets = new Dictionary<string, string[]>();

	private static Dictionary<char, Dictionary<string, string>> _CachedChanges = new Dictionary<char, Dictionary<string, string>>();

	public static string[] GetDirectories(string path)
	{
		if (_CachedDirectories.TryGetValue(path, out var value))
		{
			return value;
		}
		try
		{
			value = Directory.GetDirectories(path);
		}
		catch
		{
			value = null;
		}
		_CachedDirectories[path] = value;
		return value;
	}

	public static string[] GetTargets(string path)
	{
		if (_CachedTargets.TryGetValue(path, out var value))
		{
			return value;
		}
		try
		{
			value = Directory.GetFileSystemEntries(path);
		}
		catch
		{
			value = null;
		}
		_CachedTargets[path] = value;
		return value;
	}

	public static string GetDirectory(string path, string next)
	{
		return GetNext(GetDirectories(path), next);
	}

	public static string GetTarget(string path, string next)
	{
		return GetNext(GetTargets(path), next);
	}

	public static string GetNext(string[] possible, string next)
	{
		if (possible == null)
		{
			return null;
		}
		for (int i = 0; i < possible.Length; i++)
		{
			string fileName = Path.GetFileName(possible[i]);
			if (string.Equals(next, fileName, StringComparison.InvariantCultureIgnoreCase))
			{
				return fileName;
			}
		}
		return null;
	}

	public static string FixPath(string path)
	{
		return ChangePath(path, Path.DirectorySeparatorChar);
	}

	public static string BreakPath(string path)
	{
		return ChangePath(path, '\\');
	}

	public static string ChangePath(string path, char separator)
	{
		if (!MONO_IOMAP_ALL)
		{
			string result = path;
			if (Directory.Exists(path) || File.Exists(path))
			{
				return result;
			}
			result = path.Replace('/', separator).Replace('\\', separator);
			if (Directory.Exists(result) || File.Exists(result))
			{
				return result;
			}
		}
		if (!_CachedChanges.TryGetValue(separator, out var value))
		{
			value = (_CachedChanges[separator] = new Dictionary<string, string>());
		}
		if (value.TryGetValue(path, out var value2))
		{
			return value2;
		}
		string[] array = path.Split(DirectorySeparatorChars);
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		if (Path.IsPathRooted(path))
		{
			if (flag = stringBuilder.Length == 0)
			{
				stringBuilder.Append(separator);
			}
			else
			{
				stringBuilder.Append(array[0]);
			}
		}
		for (int i = 1; i < array.Length; i++)
		{
			string text = ((i >= array.Length - 1) ? GetTarget(stringBuilder.ToString(), array[i]) : GetDirectory(stringBuilder.ToString(), array[i]));
			text = text ?? array[i];
			if (i != 1 || !flag)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(text);
		}
		return value[path] = stringBuilder.ToString();
	}
}
