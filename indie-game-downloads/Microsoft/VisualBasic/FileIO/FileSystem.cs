using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic.FileIO;

public class FileSystem
{
	private enum CopyOrMove
	{
		Copy,
		Move
	}

	private enum FileOrDirectory
	{
		File,
		Directory
	}

	private enum UIOptionInternal
	{
		OnlyErrorDialogs = 2,
		AllDialogs,
		NoUI
	}

	private sealed class DirectoryNode
	{
		private string m_Path;

		private string m_TargetPath;

		private Collection<DirectoryNode> m_SubDirs;

		internal string Path => m_Path;

		internal string TargetPath => m_TargetPath;

		internal Collection<DirectoryNode> SubDirs => m_SubDirs;

		internal DirectoryNode(string DirectoryPath, string TargetDirectoryPath)
		{
			m_Path = DirectoryPath;
			m_TargetPath = TargetDirectoryPath;
			m_SubDirs = new Collection<DirectoryNode>();
			string[] directories = Directory.GetDirectories(m_Path);
			foreach (string text in directories)
			{
				string targetDirectoryPath = System.IO.Path.Combine(m_TargetPath, System.IO.Path.GetFileName(text));
				m_SubDirs.Add(new DirectoryNode(text, targetDirectoryPath));
			}
		}
	}

	private sealed class TextSearchHelper
	{
		private string m_SearchText;

		private bool m_IgnoreCase;

		private Decoder m_Decoder;

		private char[] m_PreviousCharBuffer;

		private bool m_CheckPreamble;

		private byte[] m_Preamble;

		internal TextSearchHelper(Encoding Encoding, string Text, bool IgnoreCase)
		{
			m_PreviousCharBuffer = Array.Empty<char>();
			m_CheckPreamble = true;
			m_Decoder = Encoding.GetDecoder();
			m_Preamble = Encoding.GetPreamble();
			m_IgnoreCase = IgnoreCase;
			if (m_IgnoreCase)
			{
				m_SearchText = Text.ToUpper(CultureInfo.CurrentCulture);
			}
			else
			{
				m_SearchText = Text;
			}
		}

		internal bool IsTextFound(byte[] ByteBuffer, int Count)
		{
			int num = 0;
			checked
			{
				if (m_CheckPreamble)
				{
					if (BytesMatch(ByteBuffer, m_Preamble))
					{
						num = m_Preamble.Length;
						Count -= m_Preamble.Length;
					}
					m_CheckPreamble = false;
					if (Count <= 0)
					{
						return false;
					}
				}
				int charCount = m_Decoder.GetCharCount(ByteBuffer, num, Count);
				char[] array = new char[m_PreviousCharBuffer.Length + charCount - 1 + 1];
				Array.Copy(m_PreviousCharBuffer, 0, array, 0, m_PreviousCharBuffer.Length);
				m_Decoder.GetChars(ByteBuffer, num, Count, array, m_PreviousCharBuffer.Length);
				if (array.Length > m_SearchText.Length)
				{
					if (m_PreviousCharBuffer.Length != m_SearchText.Length)
					{
						m_PreviousCharBuffer = new char[m_SearchText.Length - 1 + 1];
					}
					Array.Copy(array, array.Length - m_SearchText.Length, m_PreviousCharBuffer, 0, m_SearchText.Length);
				}
				else
				{
					m_PreviousCharBuffer = array;
				}
				if (m_IgnoreCase)
				{
					return new string(array).Contains(m_SearchText, StringComparison.OrdinalIgnoreCase);
				}
				return new string(array).Contains(m_SearchText);
			}
		}

		private static bool BytesMatch(byte[] BigBuffer, byte[] SmallBuffer)
		{
			if ((BigBuffer.Length < SmallBuffer.Length) | (SmallBuffer.Length == 0))
			{
				return false;
			}
			checked
			{
				int num = SmallBuffer.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (BigBuffer[i] != SmallBuffer[i])
					{
						return false;
					}
				}
				return true;
			}
		}
	}

	private static readonly char[] m_SeparatorChars = new char[3]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar,
		Path.VolumeSeparatorChar
	};

	public static ReadOnlyCollection<DriveInfo> Drives
	{
		get
		{
			Collection<DriveInfo> collection = new Collection<DriveInfo>();
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo item in drives)
			{
				collection.Add(item);
			}
			return new ReadOnlyCollection<DriveInfo>(collection);
		}
	}

	public static string CurrentDirectory
	{
		get
		{
			return NormalizePath(Directory.GetCurrentDirectory());
		}
		set
		{
			Directory.SetCurrentDirectory(value);
		}
	}

	public static string CombinePath(string baseDirectory, string relativePath)
	{
		if (Operators.CompareString(baseDirectory, "", TextCompare: false) == 0)
		{
			throw ExceptionUtils.GetArgumentNullException("baseDirectory", System.SR.General_ArgumentEmptyOrNothing_Name, "baseDirectory");
		}
		if (Operators.CompareString(relativePath, "", TextCompare: false) == 0)
		{
			return baseDirectory;
		}
		baseDirectory = Path.GetFullPath(baseDirectory);
		return NormalizePath(Path.Combine(baseDirectory, relativePath));
	}

	public static bool DirectoryExists(string directory)
	{
		return Directory.Exists(directory);
	}

	public static bool FileExists(string file)
	{
		if (!string.IsNullOrEmpty(file) && (file.EndsWith(Conversions.ToString(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) | file.EndsWith(Conversions.ToString(Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)))
		{
			return false;
		}
		return File.Exists(file);
	}

	public static ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, SearchOption searchType)
	{
		return FindInFiles(directory, containsText, ignoreCase, searchType, (string[])null);
	}

	public static ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, SearchOption searchType, params string[] fileWildcards)
	{
		ReadOnlyCollection<string> readOnlyCollection = FindFilesOrDirectories(FileOrDirectory.File, directory, searchType, fileWildcards);
		if (Operators.CompareString(containsText, "", TextCompare: false) != 0)
		{
			Collection<string> collection = new Collection<string>();
			foreach (string item in readOnlyCollection)
			{
				if (FileContainsText(item, containsText, ignoreCase))
				{
					collection.Add(item);
				}
			}
			return new ReadOnlyCollection<string>(collection);
		}
		return readOnlyCollection;
	}

	public static ReadOnlyCollection<string> GetDirectories(string directory)
	{
		return FindFilesOrDirectories(FileOrDirectory.Directory, directory, SearchOption.SearchTopLevelOnly, null);
	}

	public static ReadOnlyCollection<string> GetDirectories(string directory, SearchOption searchType, params string[] wildcards)
	{
		return FindFilesOrDirectories(FileOrDirectory.Directory, directory, searchType, wildcards);
	}

	public static DirectoryInfo GetDirectoryInfo(string directory)
	{
		return new DirectoryInfo(directory);
	}

	public static DriveInfo GetDriveInfo(string drive)
	{
		return new DriveInfo(drive);
	}

	public static FileInfo GetFileInfo(string file)
	{
		file = NormalizeFilePath(file, "file");
		return new FileInfo(file);
	}

	public static ReadOnlyCollection<string> GetFiles(string directory)
	{
		return FindFilesOrDirectories(FileOrDirectory.File, directory, SearchOption.SearchTopLevelOnly, null);
	}

	public static ReadOnlyCollection<string> GetFiles(string directory, SearchOption searchType, params string[] wildcards)
	{
		return FindFilesOrDirectories(FileOrDirectory.File, directory, searchType, wildcards);
	}

	public static string GetName(string path)
	{
		return Path.GetFileName(path);
	}

	public static string GetParentPath(string path)
	{
		Path.GetFullPath(path);
		if (IsRoot(path))
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName("path", System.SR.IO_GetParentPathIsRoot_Path, path);
		}
		return Path.GetDirectoryName(path.TrimEnd(new char[2]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		}));
	}

	public static string GetTempFileName()
	{
		return Path.GetTempFileName();
	}

	public static TextFieldParser OpenTextFieldParser(string file)
	{
		return new TextFieldParser(file);
	}

	public static TextFieldParser OpenTextFieldParser(string file, params string[] delimiters)
	{
		TextFieldParser textFieldParser = new TextFieldParser(file);
		textFieldParser.SetDelimiters(delimiters);
		textFieldParser.TextFieldType = FieldType.Delimited;
		return textFieldParser;
	}

	public static TextFieldParser OpenTextFieldParser(string file, params int[] fieldWidths)
	{
		TextFieldParser textFieldParser = new TextFieldParser(file);
		textFieldParser.SetFieldWidths(fieldWidths);
		textFieldParser.TextFieldType = FieldType.FixedWidth;
		return textFieldParser;
	}

	public static StreamReader OpenTextFileReader(string file)
	{
		return OpenTextFileReader(file, Encoding.UTF8);
	}

	public static StreamReader OpenTextFileReader(string file, Encoding encoding)
	{
		file = NormalizeFilePath(file, "file");
		return new StreamReader(file, encoding, detectEncodingFromByteOrderMarks: true);
	}

	public static StreamWriter OpenTextFileWriter(string file, bool append)
	{
		return OpenTextFileWriter(file, append, Encoding.UTF8);
	}

	public static StreamWriter OpenTextFileWriter(string file, bool append, Encoding encoding)
	{
		file = NormalizeFilePath(file, "file");
		return new StreamWriter(file, append, encoding);
	}

	public static byte[] ReadAllBytes(string file)
	{
		return File.ReadAllBytes(file);
	}

	public static string ReadAllText(string file)
	{
		return File.ReadAllText(file);
	}

	public static string ReadAllText(string file, Encoding encoding)
	{
		return File.ReadAllText(file, encoding);
	}

	public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName)
	{
		CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName, overwrite: false, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite)
	{
		CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName, overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI)
	{
		CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName, overwrite: false, ToUIOptionInternal(showUI), UICancelOption.ThrowException);
	}

	public static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI, UICancelOption onUserCancel)
	{
		CopyOrMoveDirectory(CopyOrMove.Copy, sourceDirectoryName, destinationDirectoryName, overwrite: false, ToUIOptionInternal(showUI), onUserCancel);
	}

	public static void CopyFile(string sourceFileName, string destinationFileName)
	{
		CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName, overwrite: false, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void CopyFile(string sourceFileName, string destinationFileName, bool overwrite)
	{
		CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName, overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void CopyFile(string sourceFileName, string destinationFileName, UIOption showUI)
	{
		CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName, overwrite: false, ToUIOptionInternal(showUI), UICancelOption.ThrowException);
	}

	public static void CopyFile(string sourceFileName, string destinationFileName, UIOption showUI, UICancelOption onUserCancel)
	{
		CopyOrMoveFile(CopyOrMove.Copy, sourceFileName, destinationFileName, overwrite: false, ToUIOptionInternal(showUI), onUserCancel);
	}

	public static void CreateDirectory(string directory)
	{
		directory = Path.GetFullPath(directory);
		if (File.Exists(directory))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_FileExists_Path, directory);
		}
		Directory.CreateDirectory(directory);
	}

	public static void DeleteDirectory(string directory, DeleteDirectoryOption onDirectoryNotEmpty)
	{
		DeleteDirectoryInternal(directory, onDirectoryNotEmpty, UIOptionInternal.NoUI, RecycleOption.DeletePermanently, UICancelOption.ThrowException);
	}

	public static void DeleteDirectory(string directory, UIOption showUI, RecycleOption recycle)
	{
		DeleteDirectoryInternal(directory, DeleteDirectoryOption.DeleteAllContents, ToUIOptionInternal(showUI), recycle, UICancelOption.ThrowException);
	}

	public static void DeleteDirectory(string directory, UIOption showUI, RecycleOption recycle, UICancelOption onUserCancel)
	{
		DeleteDirectoryInternal(directory, DeleteDirectoryOption.DeleteAllContents, ToUIOptionInternal(showUI), recycle, onUserCancel);
	}

	public static void DeleteFile(string file)
	{
		DeleteFileInternal(file, UIOptionInternal.NoUI, RecycleOption.DeletePermanently, UICancelOption.ThrowException);
	}

	public static void DeleteFile(string file, UIOption showUI, RecycleOption recycle)
	{
		DeleteFileInternal(file, ToUIOptionInternal(showUI), recycle, UICancelOption.ThrowException);
	}

	public static void DeleteFile(string file, UIOption showUI, RecycleOption recycle, UICancelOption onUserCancel)
	{
		DeleteFileInternal(file, ToUIOptionInternal(showUI), recycle, onUserCancel);
	}

	public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName)
	{
		CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName, overwrite: false, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite)
	{
		CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName, overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI)
	{
		CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName, overwrite: false, ToUIOptionInternal(showUI), UICancelOption.ThrowException);
	}

	public static void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI, UICancelOption onUserCancel)
	{
		CopyOrMoveDirectory(CopyOrMove.Move, sourceDirectoryName, destinationDirectoryName, overwrite: false, ToUIOptionInternal(showUI), onUserCancel);
	}

	public static void MoveFile(string sourceFileName, string destinationFileName)
	{
		CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName, overwrite: false, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void MoveFile(string sourceFileName, string destinationFileName, bool overwrite)
	{
		CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName, overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException);
	}

	public static void MoveFile(string sourceFileName, string destinationFileName, UIOption showUI)
	{
		CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName, overwrite: false, ToUIOptionInternal(showUI), UICancelOption.ThrowException);
	}

	public static void MoveFile(string sourceFileName, string destinationFileName, UIOption showUI, UICancelOption onUserCancel)
	{
		CopyOrMoveFile(CopyOrMove.Move, sourceFileName, destinationFileName, overwrite: false, ToUIOptionInternal(showUI), onUserCancel);
	}

	public static void RenameDirectory(string directory, string newName)
	{
		directory = Path.GetFullPath(directory);
		ThrowIfDevicePath(directory);
		if (IsRoot(directory))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_DirectoryIsRoot_Path, directory);
		}
		if (!Directory.Exists(directory))
		{
			throw ExceptionUtils.GetDirectoryNotFoundException(System.SR.IO_DirectoryNotFound_Path, directory);
		}
		if (Operators.CompareString(newName, "", TextCompare: false) == 0)
		{
			throw ExceptionUtils.GetArgumentNullException("newName", System.SR.General_ArgumentEmptyOrNothing_Name, "newName");
		}
		string fullPathFromNewName = GetFullPathFromNewName(GetParentPath(directory), newName, "newName");
		EnsurePathNotExist(fullPathFromNewName);
		Directory.Move(directory, fullPathFromNewName);
	}

	public static void RenameFile(string file, string newName)
	{
		file = NormalizeFilePath(file, "file");
		ThrowIfDevicePath(file);
		if (!File.Exists(file))
		{
			throw ExceptionUtils.GetFileNotFoundException(file, System.SR.IO_FileNotFound_Path, file);
		}
		if (Operators.CompareString(newName, "", TextCompare: false) == 0)
		{
			throw ExceptionUtils.GetArgumentNullException("newName", System.SR.General_ArgumentEmptyOrNothing_Name, "newName");
		}
		string fullPathFromNewName = GetFullPathFromNewName(GetParentPath(file), newName, "newName");
		EnsurePathNotExist(fullPathFromNewName);
		File.Move(file, fullPathFromNewName);
	}

	public static void WriteAllBytes(string file, byte[] data, bool append)
	{
		CheckFilePathTrailingSeparator(file, "file");
		FileStream fileStream = null;
		try
		{
			FileMode mode = ((!append) ? FileMode.Create : FileMode.Append);
			fileStream = new FileStream(file, mode, FileAccess.Write, FileShare.Read);
			fileStream.Write(data, 0, data.Length);
		}
		finally
		{
			fileStream?.Close();
		}
	}

	public static void WriteAllText(string file, string text, bool append)
	{
		WriteAllText(file, text, append, Encoding.UTF8);
	}

	public static void WriteAllText(string file, string text, bool append, Encoding encoding)
	{
		CheckFilePathTrailingSeparator(file, "file");
		StreamWriter streamWriter = null;
		try
		{
			if (append && File.Exists(file))
			{
				StreamReader streamReader = null;
				try
				{
					streamReader = new StreamReader(file, encoding, detectEncodingFromByteOrderMarks: true);
					char[] buffer = new char[10];
					streamReader.Read(buffer, 0, 10);
					encoding = streamReader.CurrentEncoding;
				}
				catch (IOException)
				{
				}
				finally
				{
					streamReader?.Close();
				}
			}
			streamWriter = new StreamWriter(file, append, encoding);
			streamWriter.Write(text);
		}
		finally
		{
			streamWriter?.Close();
		}
	}

	internal static string NormalizeFilePath(string Path, string ParamName)
	{
		CheckFilePathTrailingSeparator(Path, ParamName);
		return NormalizePath(Path);
	}

	internal static string NormalizePath(string Path)
	{
		return GetLongPath(RemoveEndingSeparator(System.IO.Path.GetFullPath(Path)));
	}

	internal static void CheckFilePathTrailingSeparator(string path, string paramName)
	{
		if (Operators.CompareString(path, "", TextCompare: false) == 0)
		{
			throw ExceptionUtils.GetArgumentNullException(paramName);
		}
		if (path.EndsWith(Conversions.ToString(Path.DirectorySeparatorChar), StringComparison.Ordinal) | path.EndsWith(Conversions.ToString(Path.AltDirectorySeparatorChar), StringComparison.Ordinal))
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName(paramName, System.SR.IO_FilePathException);
		}
	}

	private static void AddToStringCollection(Collection<string> StrCollection, string[] StrArray)
	{
		if (StrArray == null)
		{
			return;
		}
		foreach (string item in StrArray)
		{
			if (!StrCollection.Contains(item))
			{
				StrCollection.Add(item);
			}
		}
	}

	private static void CopyOrMoveDirectory(CopyOrMove operation, string sourceDirectoryName, string destinationDirectoryName, bool overwrite, UIOptionInternal showUI, UICancelOption onUserCancel)
	{
		VerifyUICancelOption("onUserCancel", onUserCancel);
		string text = NormalizePath(sourceDirectoryName);
		string text2 = NormalizePath(destinationDirectoryName);
		ThrowIfDevicePath(text);
		ThrowIfDevicePath(text2);
		if (!Directory.Exists(text))
		{
			throw ExceptionUtils.GetDirectoryNotFoundException(System.SR.IO_DirectoryNotFound_Path, sourceDirectoryName);
		}
		if (IsRoot(text))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_DirectoryIsRoot_Path, sourceDirectoryName);
		}
		if (File.Exists(text2))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_FileExists_Path, destinationDirectoryName);
		}
		if (text2.Equals(text, StringComparison.OrdinalIgnoreCase))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_SourceEqualsTargetDirectory);
		}
		if (text2.Length > text.Length && text2.Substring(0, text.Length).Equals(text, StringComparison.OrdinalIgnoreCase) && text2[text.Length] == Path.DirectorySeparatorChar)
		{
			throw ExceptionUtils.GetInvalidOperationException(System.SR.IO_CyclicOperation);
		}
		if (showUI != UIOptionInternal.NoUI && Environment.UserInteractive)
		{
			ShellCopyOrMove(operation, FileOrDirectory.Directory, text, text2, showUI, onUserCancel);
		}
		else
		{
			FxCopyOrMoveDirectory(operation, text, text2, overwrite);
		}
	}

	private static void FxCopyOrMoveDirectory(CopyOrMove operation, string sourceDirectoryPath, string targetDirectoryPath, bool overwrite)
	{
		if ((operation == CopyOrMove.Move) & !Directory.Exists(targetDirectoryPath) & IsOnSameDrive(sourceDirectoryPath, targetDirectoryPath))
		{
			Directory.CreateDirectory(GetParentPath(targetDirectoryPath));
			try
			{
				Directory.Move(sourceDirectoryPath, targetDirectoryPath);
				return;
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}
		Directory.CreateDirectory(targetDirectoryPath);
		DirectoryNode sourceDirectoryNode = new DirectoryNode(sourceDirectoryPath, targetDirectoryPath);
		ListDictionary listDictionary = new ListDictionary();
		CopyOrMoveDirectoryNode(operation, sourceDirectoryNode, overwrite, listDictionary);
		if (listDictionary.Count > 0)
		{
			IOException ex3 = new IOException(System.SR.IO_CopyMoveRecursive);
			IDictionaryEnumerator enumerator = listDictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				DictionaryEntry dictionaryEntry = ((current != null) ? ((DictionaryEntry)current) : default(DictionaryEntry));
				ex3.Data.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
			throw ex3;
		}
	}

	private static void CopyOrMoveDirectoryNode(CopyOrMove Operation, DirectoryNode SourceDirectoryNode, bool Overwrite, ListDictionary Exceptions)
	{
		try
		{
			if (!Directory.Exists(SourceDirectoryNode.TargetPath))
			{
				Directory.CreateDirectory(SourceDirectoryNode.TargetPath);
			}
		}
		catch (Exception ex)
		{
			if (ex is IOException || ex is UnauthorizedAccessException || ex is DirectoryNotFoundException || ex is NotSupportedException || ex is SecurityException)
			{
				Exceptions.Add(SourceDirectoryNode.Path, ex.Message);
				return;
			}
			throw;
		}
		if (!Directory.Exists(SourceDirectoryNode.TargetPath))
		{
			Exceptions.Add(SourceDirectoryNode.TargetPath, ExceptionUtils.GetDirectoryNotFoundException(System.SR.IO_DirectoryNotFound_Path, SourceDirectoryNode.TargetPath));
			return;
		}
		string[] files = Directory.GetFiles(SourceDirectoryNode.Path);
		foreach (string text in files)
		{
			try
			{
				CopyOrMoveFile(Operation, text, Path.Combine(SourceDirectoryNode.TargetPath, Path.GetFileName(text)), Overwrite, UIOptionInternal.NoUI, UICancelOption.ThrowException);
			}
			catch (Exception ex2)
			{
				if (ex2 is IOException || ex2 is UnauthorizedAccessException || ex2 is SecurityException || ex2 is NotSupportedException)
				{
					Exceptions.Add(text, ex2.Message);
					continue;
				}
				throw;
			}
		}
		foreach (DirectoryNode subDir in SourceDirectoryNode.SubDirs)
		{
			CopyOrMoveDirectoryNode(Operation, subDir, Overwrite, Exceptions);
		}
		if (Operation != CopyOrMove.Move)
		{
			return;
		}
		try
		{
			Directory.Delete(SourceDirectoryNode.Path, recursive: false);
		}
		catch (Exception ex3)
		{
			if (ex3 is IOException || ex3 is UnauthorizedAccessException || ex3 is SecurityException || ex3 is DirectoryNotFoundException)
			{
				Exceptions.Add(SourceDirectoryNode.Path, ex3.Message);
				return;
			}
			throw;
		}
	}

	private static void CopyOrMoveFile(CopyOrMove operation, string sourceFileName, string destinationFileName, bool overwrite, UIOptionInternal showUI, UICancelOption onUserCancel)
	{
		VerifyUICancelOption("onUserCancel", onUserCancel);
		string text = NormalizeFilePath(sourceFileName, "sourceFileName");
		string text2 = NormalizeFilePath(destinationFileName, "destinationFileName");
		ThrowIfDevicePath(text);
		ThrowIfDevicePath(text2);
		if (!File.Exists(text))
		{
			throw ExceptionUtils.GetFileNotFoundException(sourceFileName, System.SR.IO_FileNotFound_Path, sourceFileName);
		}
		if (Directory.Exists(text2))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_DirectoryExists_Path, destinationFileName);
		}
		Directory.CreateDirectory(GetParentPath(text2));
		if (showUI != UIOptionInternal.NoUI && Environment.UserInteractive)
		{
			ShellCopyOrMove(operation, FileOrDirectory.File, text, text2, showUI, onUserCancel);
		}
		else if (operation == CopyOrMove.Copy || text.Equals(text2, StringComparison.OrdinalIgnoreCase))
		{
			File.Copy(text, text2, overwrite);
		}
		else if (overwrite)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				try
				{
					if (!NativeMethods.MoveFileEx(text, text2, 11))
					{
						ThrowWinIOError(Marshal.GetLastWin32Error());
					}
					return;
				}
				catch (Exception)
				{
					throw;
				}
			}
			File.Delete(text2);
			File.Move(text, text2);
		}
		else
		{
			File.Move(text, text2);
		}
	}

	private static void DeleteDirectoryInternal(string directory, DeleteDirectoryOption onDirectoryNotEmpty, UIOptionInternal showUI, RecycleOption recycle, UICancelOption onUserCancel)
	{
		VerifyDeleteDirectoryOption("onDirectoryNotEmpty", onDirectoryNotEmpty);
		VerifyRecycleOption("recycle", recycle);
		VerifyUICancelOption("onUserCancel", onUserCancel);
		string fullPath = Path.GetFullPath(directory);
		ThrowIfDevicePath(fullPath);
		if (!Directory.Exists(fullPath))
		{
			throw ExceptionUtils.GetDirectoryNotFoundException(System.SR.IO_DirectoryNotFound_Path, directory);
		}
		if (IsRoot(fullPath))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_DirectoryIsRoot_Path, directory);
		}
		if (showUI != UIOptionInternal.NoUI && Environment.UserInteractive)
		{
			ShellDelete(fullPath, showUI, recycle, onUserCancel, FileOrDirectory.Directory);
		}
		else
		{
			Directory.Delete(fullPath, onDirectoryNotEmpty == DeleteDirectoryOption.DeleteAllContents);
		}
	}

	private static void DeleteFileInternal(string file, UIOptionInternal showUI, RecycleOption recycle, UICancelOption onUserCancel)
	{
		VerifyRecycleOption("recycle", recycle);
		VerifyUICancelOption("onUserCancel", onUserCancel);
		string text = NormalizeFilePath(file, "file");
		ThrowIfDevicePath(text);
		if (!File.Exists(text))
		{
			throw ExceptionUtils.GetFileNotFoundException(file, System.SR.IO_FileNotFound_Path, file);
		}
		if (showUI != UIOptionInternal.NoUI && Environment.UserInteractive)
		{
			ShellDelete(text, showUI, recycle, onUserCancel, FileOrDirectory.File);
		}
		else
		{
			File.Delete(text);
		}
	}

	private static void EnsurePathNotExist(string Path)
	{
		if (File.Exists(Path))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_FileExists_Path, Path);
		}
		if (Directory.Exists(Path))
		{
			throw ExceptionUtils.GetIOException(System.SR.IO_DirectoryExists_Path, Path);
		}
	}

	private static bool FileContainsText(string FilePath, string Text, bool IgnoreCase)
	{
		int num = 1024;
		FileStream fileStream = null;
		checked
		{
			try
			{
				fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				Encoding encoding = Encoding.Default;
				byte[] array = new byte[num - 1 + 1];
				int num2 = 0;
				num2 = fileStream.Read(array, 0, array.Length);
				if (num2 > 0)
				{
					MemoryStream stream = new MemoryStream(array, 0, num2);
					StreamReader streamReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
					streamReader.ReadLine();
					encoding = streamReader.CurrentEncoding;
				}
				int num3 = Math.Max(encoding.GetMaxByteCount(Text.Length), num);
				TextSearchHelper textSearchHelper = new TextSearchHelper(encoding, Text, IgnoreCase);
				if (num3 > num)
				{
					array = (byte[])Utils.CopyArray(array, new byte[num3 - 1 + 1]);
					int num4 = fileStream.Read(array, num2, array.Length - num2);
					num2 += num4;
				}
				do
				{
					if (num2 > 0 && textSearchHelper.IsTextFound(array, num2))
					{
						return true;
					}
					num2 = fileStream.Read(array, 0, array.Length);
				}
				while (num2 > 0);
				return false;
			}
			catch (Exception ex)
			{
				if ((ex is IOException) | (ex is NotSupportedException) | (ex is SecurityException) | (ex is UnauthorizedAccessException))
				{
					return false;
				}
				throw;
			}
			finally
			{
				fileStream?.Close();
			}
		}
	}

	private static ReadOnlyCollection<string> FindFilesOrDirectories(FileOrDirectory FileOrDirectory, string directory, SearchOption searchType, string[] wildcards)
	{
		Collection<string> collection = new Collection<string>();
		FindFilesOrDirectories(FileOrDirectory, directory, searchType, wildcards, collection);
		return new ReadOnlyCollection<string>(collection);
	}

	private static void FindFilesOrDirectories(FileOrDirectory FileOrDirectory, string directory, SearchOption searchType, string[] wildcards, Collection<string> Results)
	{
		VerifySearchOption("searchType", searchType);
		directory = NormalizePath(directory);
		if (wildcards != null)
		{
			for (int i = 0; i < wildcards.Length; i = checked(i + 1))
			{
				if (Operators.CompareString(wildcards[i].TrimEnd(), "", TextCompare: false) == 0)
				{
					throw ExceptionUtils.GetArgumentNullException("wildcards", System.SR.IO_GetFiles_NullPattern);
				}
			}
		}
		if (wildcards == null || wildcards.Length == 0)
		{
			AddToStringCollection(Results, FindPaths(FileOrDirectory, directory, null));
		}
		else
		{
			foreach (string wildCard in wildcards)
			{
				AddToStringCollection(Results, FindPaths(FileOrDirectory, directory, wildCard));
			}
		}
		if (searchType == SearchOption.SearchAllSubDirectories)
		{
			string[] directories = Directory.GetDirectories(directory);
			foreach (string directory2 in directories)
			{
				FindFilesOrDirectories(FileOrDirectory, directory2, searchType, wildcards, Results);
			}
		}
	}

	private static string[] FindPaths(FileOrDirectory FileOrDirectory, string directory, string wildCard)
	{
		if (FileOrDirectory == FileOrDirectory.Directory)
		{
			if (Operators.CompareString(wildCard, "", TextCompare: false) == 0)
			{
				return Directory.GetDirectories(directory);
			}
			return Directory.GetDirectories(directory, wildCard);
		}
		if (Operators.CompareString(wildCard, "", TextCompare: false) == 0)
		{
			return Directory.GetFiles(directory);
		}
		return Directory.GetFiles(directory, wildCard);
	}

	private static string GetFullPathFromNewName(string Path, string NewName, string ArgumentName)
	{
		if (NewName.IndexOfAny(m_SeparatorChars) >= 0)
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName(ArgumentName, System.SR.IO_ArgumentIsPath_Name_Path, ArgumentName, NewName);
		}
		string text = RemoveEndingSeparator(System.IO.Path.GetFullPath(System.IO.Path.Combine(Path, NewName)));
		if (!GetParentPath(text).Equals(Path, StringComparison.OrdinalIgnoreCase))
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName(ArgumentName, System.SR.IO_ArgumentIsPath_Name_Path, ArgumentName, NewName);
		}
		return text;
	}

	private static string GetLongPath(string FullPath)
	{
		try
		{
			if (IsRoot(FullPath))
			{
				return FullPath;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(GetParentPath(FullPath));
			if (File.Exists(FullPath))
			{
				return directoryInfo.GetFiles(Path.GetFileName(FullPath))[0].FullName;
			}
			if (Directory.Exists(FullPath))
			{
				return directoryInfo.GetDirectories(Path.GetFileName(FullPath))[0].FullName;
			}
			return FullPath;
		}
		catch (Exception ex)
		{
			if (ex is ArgumentException || ex is ArgumentNullException || ex is PathTooLongException || ex is NotSupportedException || ex is DirectoryNotFoundException || ex is SecurityException || ex is UnauthorizedAccessException)
			{
				return FullPath;
			}
			throw;
		}
	}

	private static bool IsOnSameDrive(string Path1, string Path2)
	{
		Path1 = Path1.TrimEnd(new char[2]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		});
		Path2 = Path2.TrimEnd(new char[2]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		});
		return string.Equals(Path.GetPathRoot(Path1), Path.GetPathRoot(Path2), StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsRoot(string Path)
	{
		if (!System.IO.Path.IsPathRooted(Path))
		{
			return false;
		}
		Path = Path.TrimEnd(new char[2]
		{
			System.IO.Path.DirectorySeparatorChar,
			System.IO.Path.AltDirectorySeparatorChar
		});
		return string.Equals(Path, System.IO.Path.GetPathRoot(Path), StringComparison.OrdinalIgnoreCase);
	}

	private static string RemoveEndingSeparator(string Path)
	{
		if (System.IO.Path.IsPathRooted(Path) && Path.Equals(System.IO.Path.GetPathRoot(Path), StringComparison.OrdinalIgnoreCase))
		{
			return Path;
		}
		return Path.TrimEnd(new char[2]
		{
			System.IO.Path.DirectorySeparatorChar,
			System.IO.Path.AltDirectorySeparatorChar
		});
	}

	private static void ShellCopyOrMove(CopyOrMove Operation, FileOrDirectory TargetType, string FullSourcePath, string FullTargetPath, UIOptionInternal ShowUI, UICancelOption OnUserCancel)
	{
		NativeMethods.SHFileOperationType operationType = ((Operation != CopyOrMove.Copy) ? NativeMethods.SHFileOperationType.FO_MOVE : NativeMethods.SHFileOperationType.FO_COPY);
		NativeMethods.ShFileOperationFlags operationFlags = GetOperationFlags(ShowUI);
		string fullSource = FullSourcePath;
		if (TargetType == FileOrDirectory.Directory)
		{
			if (Directory.Exists(FullTargetPath))
			{
				fullSource = Path.Combine(FullSourcePath, "*");
			}
			else
			{
				Directory.CreateDirectory(GetParentPath(FullTargetPath));
			}
		}
		ShellFileOperation(operationType, operationFlags, fullSource, FullTargetPath, OnUserCancel, TargetType);
		if (((Operation == CopyOrMove.Move) & (TargetType == FileOrDirectory.Directory)) && Directory.Exists(FullSourcePath) && Directory.GetDirectories(FullSourcePath).Length == 0 && Directory.GetFiles(FullSourcePath).Length == 0)
		{
			Directory.Delete(FullSourcePath, recursive: false);
		}
	}

	private static void ShellDelete(string FullPath, UIOptionInternal ShowUI, RecycleOption recycle, UICancelOption OnUserCancel, FileOrDirectory FileOrDirectory)
	{
		NativeMethods.ShFileOperationFlags shFileOperationFlags = GetOperationFlags(ShowUI);
		if (recycle == RecycleOption.SendToRecycleBin)
		{
			shFileOperationFlags |= NativeMethods.ShFileOperationFlags.FOF_ALLOWUNDO;
		}
		ShellFileOperation(NativeMethods.SHFileOperationType.FO_DELETE, shFileOperationFlags, FullPath, null, OnUserCancel, FileOrDirectory);
	}

	private static void ShellFileOperation(NativeMethods.SHFileOperationType OperationType, NativeMethods.ShFileOperationFlags OperationFlags, string FullSource, string FullTarget, UICancelOption OnUserCancel, FileOrDirectory FileOrDirectory)
	{
		NativeMethods.SHFILEOPSTRUCT lpFileOp = GetShellOperationInfo(OperationType, OperationFlags, FullSource, FullTarget);
		int num;
		try
		{
			num = NativeMethods.SHFileOperation(ref lpFileOp);
			NativeMethods.SHChangeNotify(145439u, 3u, IntPtr.Zero, IntPtr.Zero);
		}
		catch (Exception)
		{
			throw;
		}
		if (lpFileOp.fAnyOperationsAborted)
		{
			if (OnUserCancel == UICancelOption.ThrowException)
			{
				throw new OperationCanceledException();
			}
		}
		else if (num != 0)
		{
			ThrowWinIOError(ToWinIOErrorCode(num));
		}
	}

	private static NativeMethods.SHFILEOPSTRUCT GetShellOperationInfo(NativeMethods.SHFileOperationType OperationType, NativeMethods.ShFileOperationFlags OperationFlags, string SourcePath, string TargetPath = null)
	{
		return GetShellOperationInfo(OperationType, OperationFlags, new string[1] { SourcePath }, TargetPath);
	}

	private static NativeMethods.SHFILEOPSTRUCT GetShellOperationInfo(NativeMethods.SHFileOperationType OperationType, NativeMethods.ShFileOperationFlags OperationFlags, string[] SourcePaths, string TargetPath = null)
	{
		NativeMethods.SHFILEOPSTRUCT result = default(NativeMethods.SHFILEOPSTRUCT);
		result.wFunc = (uint)OperationType;
		result.fFlags = (ushort)OperationFlags;
		result.pFrom = GetShellPath(SourcePaths);
		if (TargetPath == null)
		{
			result.pTo = null;
		}
		else
		{
			result.pTo = GetShellPath(TargetPath);
		}
		result.hNameMappings = IntPtr.Zero;
		try
		{
			result.hwnd = Process.GetCurrentProcess().MainWindowHandle;
		}
		catch (Exception ex)
		{
			if (!(ex is SecurityException) && !(ex is InvalidOperationException) && !(ex is NotSupportedException))
			{
				throw;
			}
			result.hwnd = IntPtr.Zero;
		}
		result.lpszProgressTitle = string.Empty;
		return result;
	}

	private static NativeMethods.ShFileOperationFlags GetOperationFlags(UIOptionInternal ShowUI)
	{
		NativeMethods.ShFileOperationFlags shFileOperationFlags = NativeMethods.ShFileOperationFlags.FOF_NOCONFIRMMKDIR | NativeMethods.ShFileOperationFlags.FOF_NO_CONNECTED_ELEMENTS;
		if (ShowUI == UIOptionInternal.OnlyErrorDialogs)
		{
			shFileOperationFlags |= NativeMethods.ShFileOperationFlags.FOF_SILENT | NativeMethods.ShFileOperationFlags.FOF_NOCONFIRMATION;
		}
		return shFileOperationFlags;
	}

	private static string GetShellPath(string FullPath)
	{
		return GetShellPath(new string[1] { FullPath });
	}

	private static string GetShellPath(string[] FullPaths)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in FullPaths)
		{
			stringBuilder.Append(text + "\0");
		}
		return stringBuilder.ToString();
	}

	private static void ThrowIfDevicePath(string path)
	{
		if (path.StartsWith("\\\\.\\", StringComparison.Ordinal))
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName("path", System.SR.IO_DevicePath);
		}
	}

	private static void ThrowWinIOError(int errorCode)
	{
		switch (errorCode)
		{
		case 2:
			throw new FileNotFoundException();
		case 3:
			throw new DirectoryNotFoundException();
		case 5:
			throw new UnauthorizedAccessException();
		case 206:
			throw new PathTooLongException();
		case 15:
			throw new DriveNotFoundException();
		case 995:
		case 1223:
			throw new OperationCanceledException();
		default:
			throw new IOException(new Win32Exception(errorCode).Message, Marshal.GetHRForLastWin32Error());
		}
	}

	private static UIOptionInternal ToUIOptionInternal(UIOption showUI)
	{
		return showUI switch
		{
			UIOption.AllDialogs => UIOptionInternal.AllDialogs, 
			UIOption.OnlyErrorDialogs => UIOptionInternal.OnlyErrorDialogs, 
			_ => throw new InvalidEnumArgumentException("showUI", (int)showUI, typeof(UIOption)), 
		};
	}

	private static int ToWinIOErrorCode(int errorCode)
	{
		return errorCode switch
		{
			113 => 183, 
			114 => 87, 
			115 => 17, 
			116 => 5, 
			117 => 1223, 
			118 => 161, 
			120 => 5, 
			121 => 206, 
			122 => 87, 
			124 => 161, 
			125 => 87, 
			126 => 183, 
			128 => 183, 
			129 => 206, 
			130 => 29, 
			131 => 29, 
			132 => 29, 
			133 => 223, 
			134 => 30, 
			135 => 30, 
			136 => 30, 
			183 => 206, 
			1026 => 3, 
			65536 => 31, 
			65652 => 5, 
			_ => errorCode, 
		};
	}

	private static void VerifyDeleteDirectoryOption(string argName, DeleteDirectoryOption argValue)
	{
		if (argValue != DeleteDirectoryOption.DeleteAllContents && argValue != DeleteDirectoryOption.ThrowIfDirectoryNonEmpty)
		{
			throw new InvalidEnumArgumentException(argName, (int)argValue, typeof(DeleteDirectoryOption));
		}
	}

	private static void VerifyRecycleOption(string argName, RecycleOption argValue)
	{
		if (argValue != RecycleOption.DeletePermanently && argValue != RecycleOption.SendToRecycleBin)
		{
			throw new InvalidEnumArgumentException(argName, (int)argValue, typeof(RecycleOption));
		}
	}

	private static void VerifySearchOption(string argName, SearchOption argValue)
	{
		if (argValue != SearchOption.SearchAllSubDirectories && argValue != SearchOption.SearchTopLevelOnly)
		{
			throw new InvalidEnumArgumentException(argName, (int)argValue, typeof(SearchOption));
		}
	}

	private static void VerifyUICancelOption(string argName, UICancelOption argValue)
	{
		if (argValue != UICancelOption.DoNothing && argValue != UICancelOption.ThrowException)
		{
			throw new InvalidEnumArgumentException(argName, (int)argValue, typeof(UICancelOption));
		}
	}
}
