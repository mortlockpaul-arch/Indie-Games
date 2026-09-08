using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class IOUtils
{
	internal static string FindFirstFile(Assembly assem, string PathName, FileAttributes Attributes)
	{
		string text = null;
		checked
		{
			string text2;
			if (PathName.Length > 0 && PathName[PathName.Length - 1] == Path.DirectorySeparatorChar)
			{
				text = Path.GetFullPath(PathName);
				text2 = "*.*";
			}
			else
			{
				if (PathName.Length == 0)
				{
					text2 = "*.*";
				}
				else
				{
					text2 = Path.GetFileName(PathName);
					text = Path.GetDirectoryName(PathName);
					if (text2 == null || text2.Length == 0 || Operators.CompareString(text2, ".", TextCompare: false) == 0)
					{
						text2 = "*.*";
					}
				}
				if (text == null || text.Length == 0)
				{
					if (Path.IsPathRooted(PathName))
					{
						text = Path.GetPathRoot(PathName);
					}
					else
					{
						text = Environment.CurrentDirectory;
						if (text[text.Length - 1] != Path.DirectorySeparatorChar)
						{
							text += Conversions.ToString(Path.DirectorySeparatorChar);
						}
					}
				}
				else if (text[text.Length - 1] != Path.DirectorySeparatorChar)
				{
					text += Conversions.ToString(Path.DirectorySeparatorChar);
				}
				if (Operators.CompareString(text2, "..", TextCompare: false) == 0)
				{
					text += "..\\";
					text2 = "*.*";
				}
			}
			FileSystemInfo[] fileSystemInfos;
			try
			{
				fileSystemInfos = Directory.GetParent(text + text2).GetFileSystemInfos(text2);
			}
			catch (SecurityException ex)
			{
				throw ex;
			}
			catch (IOException e) when (Marshal.GetHRForException(e) == -2147024875)
			{
				throw ExceptionUtils.VbMakeException(52);
			}
			catch (StackOverflowException ex2)
			{
				throw ex2;
			}
			catch (OutOfMemoryException ex3)
			{
				throw ex3;
			}
			catch (Exception)
			{
				return "";
			}
			AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(assem);
			assemblyData.m_DirFiles = fileSystemInfos;
			assemblyData.m_DirNextFileIndex = 0;
			assemblyData.m_DirAttributes = Attributes;
			if (fileSystemInfos == null || fileSystemInfos.Length == 0)
			{
				return "";
			}
			return FindFileFilter(assemblyData);
		}
	}

	internal static string FindNextFile(Assembly assem)
	{
		AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(assem);
		if (assemblyData.m_DirFiles == null)
		{
			throw new ArgumentException(System.SR.DIR_IllegalCall);
		}
		if (assemblyData.m_DirNextFileIndex > assemblyData.m_DirFiles.GetUpperBound(0))
		{
			assemblyData.m_DirFiles = null;
			assemblyData.m_DirNextFileIndex = 0;
			return null;
		}
		return FindFileFilter(assemblyData);
	}

	private static string FindFileFilter(AssemblyData oAssemblyData)
	{
		FileSystemInfo[] dirFiles = oAssemblyData.m_DirFiles;
		int num = oAssemblyData.m_DirNextFileIndex;
		checked
		{
			while (true)
			{
				if (num > dirFiles.GetUpperBound(0))
				{
					oAssemblyData.m_DirFiles = null;
					oAssemblyData.m_DirNextFileIndex = 0;
					return null;
				}
				FileSystemInfo fileSystemInfo = dirFiles[num];
				if ((fileSystemInfo.Attributes & (FileAttributes.Hidden | FileAttributes.System | FileAttributes.Directory)) == 0 || (fileSystemInfo.Attributes & oAssemblyData.m_DirAttributes) != FileAttributes.None)
				{
					break;
				}
				num++;
			}
			oAssemblyData.m_DirNextFileIndex = num + 1;
			return dirFiles[num].Name;
		}
	}
}
