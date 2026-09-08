using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class FileSystem
{
	internal static readonly DateTimeFormatInfo m_WriteDateFormatInfo = InitializeWriteDateFormatInfo();

	private static DateTimeFormatInfo InitializeWriteDateFormatInfo()
	{
		return new DateTimeFormatInfo
		{
			DateSeparator = "-",
			ShortDatePattern = "\\#yyyy-MM-dd\\#",
			LongTimePattern = "\\#HH:mm:ss\\#",
			FullDateTimePattern = "\\#yyyy-MM-dd HH:mm:ss\\#"
		};
	}

	public static void ChDir(string Path)
	{
		Path = Strings.RTrim(Path);
		if (Path == null || Path.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Argument_PathNullOrEmpty), 52);
		}
		if (Operators.CompareString(Path, "\\", TextCompare: false) == 0)
		{
			Path = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
		}
		try
		{
			Directory.SetCurrentDirectory(Path);
		}
		catch (FileNotFoundException)
		{
			throw ExceptionUtils.VbMakeException(new FileNotFoundException(System.SR.Format(System.SR.FileSystem_PathNotFound1, Path)), 76);
		}
	}

	[SupportedOSPlatform("windows")]
	public static void ChDrive(char Drive)
	{
		Drive = char.ToUpperInvariant(Drive);
		if (Drive < 'A' || Drive > 'Z')
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Drive"));
		}
		if (!UnsafeValidDrive(Drive))
		{
			throw ExceptionUtils.VbMakeException(new IOException(System.SR.Format(System.SR.FileSystem_DriveNotFound1, Conversions.ToString(Drive))), 68);
		}
		Directory.SetCurrentDirectory(Conversions.ToString(Drive) + Conversions.ToString(Path.VolumeSeparatorChar));
	}

	[SupportedOSPlatform("windows")]
	public static void ChDrive(string Drive)
	{
		if (Drive != null && Drive.Length != 0)
		{
			ChDrive(Drive[0]);
		}
	}

	public static string CurDir()
	{
		return Directory.GetCurrentDirectory();
	}

	[SupportedOSPlatform("windows")]
	public static string CurDir(char Drive)
	{
		Drive = char.ToUpperInvariant(Drive);
		if (Drive < 'A' || Drive > 'Z')
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Drive")), 68);
		}
		string fullPath = Path.GetFullPath(Conversions.ToString(Drive) + Conversions.ToString(Path.VolumeSeparatorChar) + ".");
		if (!UnsafeValidDrive(Drive))
		{
			throw ExceptionUtils.VbMakeException(new IOException(System.SR.Format(System.SR.FileSystem_DriveNotFound1, Conversions.ToString(Drive))), 68);
		}
		return fullPath;
	}

	public static string Dir()
	{
		return IOUtils.FindNextFile(Assembly.GetCallingAssembly());
	}

	[SupportedOSPlatform("windows")]
	public static string Dir(string PathName, FileAttribute Attributes = FileAttribute.Normal)
	{
		if (Attributes == FileAttribute.Volume)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			string text = null;
			if (PathName.Length > 0)
			{
				text = Path.GetPathRoot(PathName);
				if (text[checked(text.Length - 1)] != Path.DirectorySeparatorChar)
				{
					text += Conversions.ToString(Path.DirectorySeparatorChar);
				}
			}
			string lpRootPathName = text;
			int lpVolumeSerialNumber = 0;
			int lpMaximumComponentLength = 0;
			int lpFileSystemFlags = 0;
			if (NativeMethods.GetVolumeInformation(lpRootPathName, stringBuilder, 256, ref lpVolumeSerialNumber, ref lpMaximumComponentLength, ref lpFileSystemFlags, default(nint), 0) != 0)
			{
				return stringBuilder.ToString();
			}
			return "";
		}
		FileAttributes attributes = (FileAttributes)(Attributes | (FileAttribute)0x80);
		return IOUtils.FindFirstFile(Assembly.GetCallingAssembly(), PathName, attributes);
	}

	public static void MkDir(string Path)
	{
		if (Path == null || Path.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Argument_PathNullOrEmpty), 52);
		}
		if (Directory.Exists(Path))
		{
			throw ExceptionUtils.VbMakeException(75);
		}
		Directory.CreateDirectory(Path);
	}

	public static void RmDir(string Path)
	{
		if (Path == null || Path.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Argument_PathNullOrEmpty), 52);
		}
		try
		{
			Directory.Delete(Path);
		}
		catch (DirectoryNotFoundException ex)
		{
			throw ExceptionUtils.VbMakeException(ex, 76);
		}
		catch (StackOverflowException ex2)
		{
			throw ex2;
		}
		catch (OutOfMemoryException ex3)
		{
			throw ex3;
		}
		catch (Exception ex4)
		{
			throw ExceptionUtils.VbMakeException(ex4, 75);
		}
	}

	private static bool PathContainsWildcards(string Path)
	{
		if (Path == null)
		{
			return false;
		}
		if (Path.Contains('*'))
		{
			return true;
		}
		if (Path.Contains('?'))
		{
			return true;
		}
		return false;
	}

	public static void FileCopy(string Source, string Destination)
	{
		if (Source == null || Source.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_PathNullOrEmpty1, "Source")), 52);
		}
		if (Destination == null || Destination.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_PathNullOrEmpty1, "Destination")), 52);
		}
		if (PathContainsWildcards(Source))
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Source")), 52);
		}
		if (PathContainsWildcards(Destination))
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Destination")), 52);
		}
		AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(Assembly.GetCallingAssembly());
		if (CheckFileOpen(assemblyData, Destination, OpenModeTypes.Output))
		{
			throw ExceptionUtils.VbMakeException(new IOException(System.SR.Format(System.SR.FileSystem_FileAlreadyOpen1, Destination)), 55);
		}
		if (CheckFileOpen(assemblyData, Source, OpenModeTypes.Input))
		{
			throw ExceptionUtils.VbMakeException(new IOException(System.SR.Format(System.SR.FileSystem_FileAlreadyOpen1, Source)), 55);
		}
		try
		{
			File.Copy(Source, Destination, overwrite: true);
			File.SetAttributes(Destination, FileAttributes.Archive);
		}
		catch (FileNotFoundException ex)
		{
			throw ExceptionUtils.VbMakeException(ex, 53);
		}
		catch (IOException ex2)
		{
			throw ExceptionUtils.VbMakeException(ex2, 55);
		}
		catch (Exception ex3)
		{
			throw ex3;
		}
	}

	public static DateTime FileDateTime(string PathName)
	{
		if (PathContainsWildcards(PathName))
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "PathName")), 52);
		}
		if (File.Exists(PathName))
		{
			return new FileInfo(PathName).LastWriteTime;
		}
		throw new FileNotFoundException(System.SR.Format(System.SR.FileSystem_FileNotFound1, PathName));
	}

	public static long FileLen(string PathName)
	{
		if (File.Exists(PathName))
		{
			return new FileInfo(PathName).Length;
		}
		throw new FileNotFoundException(System.SR.Format(System.SR.FileSystem_FileNotFound1, PathName));
	}

	public static FileAttribute GetAttr(string PathName)
	{
		char[] anyOf = new char[2] { '*', '?' };
		if (PathName.IndexOfAny(anyOf) >= 0)
		{
			throw ExceptionUtils.VbMakeException(52);
		}
		FileInfo fileInfo = new FileInfo(PathName);
		if (fileInfo.Exists)
		{
			return (FileAttribute)(fileInfo.Attributes & (FileAttributes)0x3F);
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(PathName);
		if (directoryInfo.Exists)
		{
			return (FileAttribute)(directoryInfo.Attributes & (FileAttributes)0x3F);
		}
		if (Path.GetFileName(PathName).Length == 0)
		{
			throw ExceptionUtils.VbMakeException(52);
		}
		throw new FileNotFoundException(System.SR.Format(System.SR.FileSystem_FileNotFound1, PathName));
	}

	public static void Kill(string PathName)
	{
		string text = Path.GetDirectoryName(PathName);
		string searchPattern;
		if (text == null || text.Length == 0)
		{
			text = Environment.CurrentDirectory;
			searchPattern = PathName;
		}
		else
		{
			searchPattern = Path.GetFileName(PathName);
		}
		FileInfo[] files = new DirectoryInfo(text).GetFiles(searchPattern);
		text += Conversions.ToString(Path.PathSeparator);
		checked
		{
			int num = default(int);
			if (files != null)
			{
				int upperBound = files.GetUpperBound(0);
				for (int i = 0; i <= upperBound; i++)
				{
					FileInfo fileInfo = files[i];
					if ((fileInfo.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
					{
						searchPattern = fileInfo.FullName;
						if (CheckFileOpen(ProjectData.GetProjectData().GetAssemblyData(Assembly.GetCallingAssembly()), searchPattern, OpenModeTypes.Any))
						{
							throw ExceptionUtils.VbMakeException(new IOException(System.SR.Format(System.SR.FileSystem_FileAlreadyOpen1, searchPattern)), 55);
						}
						try
						{
							File.Delete(searchPattern);
							num++;
						}
						catch (IOException ex)
						{
							throw ExceptionUtils.VbMakeException(ex, 55);
						}
						catch (Exception ex2)
						{
							throw ex2;
						}
					}
				}
			}
			if (num == 0)
			{
				throw new FileNotFoundException(System.SR.Format(System.SR.KILL_NoFilesFound1, PathName));
			}
		}
	}

	public static void SetAttr(string PathName, FileAttribute Attributes)
	{
		if (PathName == null || PathName.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Argument_PathNullOrEmpty), 52);
		}
		Assembly callingAssembly = Assembly.GetCallingAssembly();
		VB6CheckPathname(ProjectData.GetProjectData().GetAssemblyData(callingAssembly), PathName, OpenMode.Input);
		if ((Attributes | (FileAttribute.ReadOnly | FileAttribute.Hidden | FileAttribute.System | FileAttribute.Archive)) != (FileAttribute.ReadOnly | FileAttribute.Hidden | FileAttribute.System | FileAttribute.Archive))
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Attributes"));
		}
		File.SetAttributes(PathName, (FileAttributes)Attributes);
	}

	private static bool UnsafeValidDrive(char cDrive)
	{
		checked
		{
			int num = cDrive - 65;
			return (UnsafeNativeMethods.GetLogicalDrives() & (long)Math.Round(Math.Pow(2.0, num))) != 0;
		}
	}

	private static void ValidateAccess(OpenAccess Access)
	{
		if (Access != OpenAccess.Default && Access != OpenAccess.Read && Access != OpenAccess.ReadWrite && Access != OpenAccess.Write)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Access"));
		}
	}

	private static void ValidateShare(OpenShare Share)
	{
		if (Share != OpenShare.Default && Share != OpenShare.Shared && Share != OpenShare.LockRead && Share != OpenShare.LockReadWrite && Share != OpenShare.LockWrite)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Share"));
		}
	}

	private static void ValidateMode(OpenMode Mode)
	{
		if (Mode != OpenMode.Input && Mode != OpenMode.Output && Mode != OpenMode.Random && Mode != OpenMode.Append && Mode != OpenMode.Binary)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Mode"));
		}
	}

	public static void FileOpen(int FileNumber, string FileName, OpenMode Mode, OpenAccess Access = OpenAccess.Default, OpenShare Share = OpenShare.Default, int RecordLength = -1)
	{
		try
		{
			ValidateMode(Mode);
			ValidateAccess(Access);
			ValidateShare(Share);
			if (FileNumber < 1 || FileNumber > 255)
			{
				throw ExceptionUtils.VbMakeException(52);
			}
			vbIOOpenFile(Assembly.GetCallingAssembly(), FileNumber, FileName, Mode, Access, Share, RecordLength);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileClose(params int[] FileNumbers)
	{
		try
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(callingAssembly);
			if (FileNumbers == null || FileNumbers.Length == 0)
			{
				CloseAllFiles(assemblyData);
				return;
			}
			int upperBound = FileNumbers.GetUpperBound(0);
			for (int i = 0; i <= upperBound; i = checked(i + 1))
			{
				InternalCloseFile(assemblyData, FileNumbers[i]);
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static void ValidateGetPutRecordNumber(long RecordNumber)
	{
		if (RecordNumber < 1 && RecordNumber != -1)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "RecordNumber")), 63);
		}
	}

	[RequiresUnreferencedCode("The target object type could not be statically analyzed and may be trimmed")]
	public static void FileGetObject(int FileNumber, ref object Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).GetObject(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[RequiresUnreferencedCode("The target object type could not be statically analyzed and may be trimmed")]
	public static void FileGet(int FileNumber, ref ValueType Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[RequiresUnreferencedCode("The target object type could not be statically analyzed and may be trimmed")]
	public static void FileGet(int FileNumber, ref Array Value, long RecordNumber = -1L, bool ArrayIsDynamic = false, bool StringIsFixedLength = false)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber, ArrayIsDynamic, StringIsFixedLength);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref bool Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref byte Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref short Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref int Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref long Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref char Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref float Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref double Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref decimal Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref string Value, long RecordNumber = -1L, bool StringIsFixedLength = false)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber, StringIsFixedLength);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileGet(int FileNumber, ref DateTime Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Get(ref Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[RequiresUnreferencedCode("The origin object type could not be statically analyzed and may be trimmed")]
	public static void FilePutObject(int FileNumber, object Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).PutObject(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[Obsolete("FileSystem.FilePut has been deprecated. Use FilePutObject to write Object types, or coerce FileNumber and RecordNumber to Integer for writing non-Object types.")]
	public static void FilePut(object FileNumber, object Value, object RecordNumber = -1)
	{
		throw new ArgumentException(System.SR.UseFilePutObject);
	}

	[RequiresUnreferencedCode("The origin object type could not be statically analyzed and may be trimmed")]
	public static void FilePut(int FileNumber, ValueType Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[RequiresUnreferencedCode("The origin object type could not be statically analyzed and may be trimmed")]
	public static void FilePut(int FileNumber, Array Value, long RecordNumber = -1L, bool ArrayIsDynamic = false, bool StringIsFixedLength = false)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber, ArrayIsDynamic, StringIsFixedLength);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, bool Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, byte Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, short Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, int Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, long Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, char Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, float Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, double Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, decimal Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, string Value, long RecordNumber = -1L, bool StringIsFixedLength = false)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber, StringIsFixedLength);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FilePut(int FileNumber, DateTime Value, long RecordNumber = -1L)
	{
		try
		{
			ValidateGetPutRecordNumber(RecordNumber);
			GetStream(Assembly.GetCallingAssembly(), FileNumber, (OpenModeTypes)36).Put(Value, RecordNumber);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Print(int FileNumber, params object[] Output)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Print(Output);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void PrintLine(int FileNumber, params object[] Output)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).PrintLine(Output);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[RequiresUnreferencedCode("The target object type could not be statically analyzed and may be trimmed")]
	public static void Input(int FileNumber, ref object Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref bool Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref byte Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref short Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref int Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref long Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref char Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref float Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref double Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref decimal Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref string Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Input(int FileNumber, ref DateTime Value)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).Input(ref Value);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void Write(int FileNumber, params object[] Output)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).WriteHelper(Output);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void WriteLine(int FileNumber, params object[] Output)
	{
		try
		{
			GetStream(Assembly.GetCallingAssembly(), FileNumber).WriteLineHelper(Output);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static string InputString(int FileNumber, int CharCount)
	{
		try
		{
			if (CharCount < 0 || (double)CharCount > 1073741823.5)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "CharCount"));
			}
			VB6File channelObj = GetChannelObj(Assembly.GetCallingAssembly(), FileNumber);
			channelObj.Lock();
			try
			{
				return channelObj.InputString(CharCount);
			}
			finally
			{
				channelObj.Unlock();
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static string LineInput(int FileNumber)
	{
		VB6File stream = GetStream(Assembly.GetCallingAssembly(), FileNumber);
		CheckInputCapable(stream);
		if (stream.EOF())
		{
			throw ExceptionUtils.VbMakeException(62);
		}
		return stream.LineInput();
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static void Lock(int FileNumber)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Lock();
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static void Lock(int FileNumber, long Record)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Lock(Record);
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static void Lock(int FileNumber, long FromRecord, long ToRecord)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Lock(FromRecord, ToRecord);
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static void Unlock(int FileNumber)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Unlock();
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static void Unlock(int FileNumber, long Record)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Unlock(Record);
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	public static void Unlock(int FileNumber, long FromRecord, long ToRecord)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Unlock(FromRecord, ToRecord);
	}

	public static void FileWidth(int FileNumber, int RecordWidth)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).SetWidth(RecordWidth);
	}

	public static int FreeFile()
	{
		Assembly callingAssembly = Assembly.GetCallingAssembly();
		AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(callingAssembly);
		int num = 1;
		do
		{
			if (assemblyData.GetChannelObj(num) == null)
			{
				return num;
			}
			num = checked(num + 1);
		}
		while (num <= 255);
		throw ExceptionUtils.VbMakeException(67);
	}

	public static void Seek(int FileNumber, long Position)
	{
		GetStream(Assembly.GetCallingAssembly(), FileNumber).Seek(Position);
	}

	public static long Seek(int FileNumber)
	{
		return GetStream(Assembly.GetCallingAssembly(), FileNumber).Seek();
	}

	public static bool EOF(int FileNumber)
	{
		return GetStream(Assembly.GetCallingAssembly(), FileNumber).EOF();
	}

	public static long Loc(int FileNumber)
	{
		return GetStream(Assembly.GetCallingAssembly(), FileNumber).LOC();
	}

	public static long LOF(int FileNumber)
	{
		return GetStream(Assembly.GetCallingAssembly(), FileNumber).LOF();
	}

	public static TabInfo TAB()
	{
		TabInfo result = default(TabInfo);
		result.Column = -1;
		return result;
	}

	public static TabInfo TAB(short Column)
	{
		if (Column < 1)
		{
			Column = 1;
		}
		TabInfo result = default(TabInfo);
		result.Column = Column;
		return result;
	}

	public static SpcInfo SPC(short Count)
	{
		if (Count < 1)
		{
			Count = 0;
		}
		SpcInfo result = default(SpcInfo);
		result.Count = Count;
		return result;
	}

	public static OpenMode FileAttr(int FileNumber)
	{
		return GetStream(Assembly.GetCallingAssembly(), FileNumber).GetMode();
	}

	public static void Reset()
	{
		CloseAllFiles(Assembly.GetCallingAssembly());
	}

	[SupportedOSPlatform("windows")]
	public static void Rename(string OldPath, string NewPath)
	{
		AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(Assembly.GetCallingAssembly());
		OldPath = VB6CheckPathname(assemblyData, OldPath, (OpenMode)(-1));
		NewPath = VB6CheckPathname(assemblyData, NewPath, (OpenMode)(-1));
		if (UnsafeNativeMethods.MoveFile(OldPath, NewPath) == 0)
		{
			switch (Marshal.GetLastWin32Error())
			{
			case 2:
				throw ExceptionUtils.VbMakeException(53);
			case 80:
			case 183:
				throw ExceptionUtils.VbMakeException(58);
			case 12:
				throw ExceptionUtils.VbMakeException(75);
			case 17:
				throw ExceptionUtils.VbMakeException(74);
			default:
				throw ExceptionUtils.VbMakeException(5);
			}
		}
	}

	private static VB6File GetStream(Assembly assem, int FileNumber)
	{
		return GetStream(assem, FileNumber, (OpenModeTypes)47);
	}

	private static VB6File GetStream(Assembly assem, int FileNumber, OpenModeTypes mode)
	{
		if (FileNumber < 1 || FileNumber > 255)
		{
			throw ExceptionUtils.VbMakeException(52);
		}
		VB6File channelObj = GetChannelObj(assem, FileNumber);
		if ((OpenModeTypesFromOpenMode(channelObj.GetMode()) | mode) == (OpenModeTypes)0)
		{
			channelObj = null;
			throw ExceptionUtils.VbMakeException(54);
		}
		return channelObj;
	}

	private static OpenModeTypes OpenModeTypesFromOpenMode(OpenMode om)
	{
		return om switch
		{
			OpenMode.Input => OpenModeTypes.Input, 
			OpenMode.Output => OpenModeTypes.Output, 
			OpenMode.Append => OpenModeTypes.Append, 
			OpenMode.Binary => OpenModeTypes.Binary, 
			OpenMode.Random => OpenModeTypes.Random, 
			(OpenMode)(-1) => OpenModeTypes.Any, 
			_ => throw new ArgumentException(System.SR.Argument_InvalidValue, "om"), 
		};
	}

	internal static void CloseAllFiles(Assembly assem)
	{
		CloseAllFiles(ProjectData.GetProjectData().GetAssemblyData(assem));
	}

	internal static void CloseAllFiles(AssemblyData oAssemblyData)
	{
		int num = 1;
		do
		{
			InternalCloseFile(oAssemblyData, num);
			num = checked(num + 1);
		}
		while (num <= 255);
	}

	private static void InternalCloseFile(AssemblyData oAssemblyData, int FileNumber)
	{
		if (FileNumber == 0)
		{
			CloseAllFiles(oAssemblyData);
			return;
		}
		VB6File channelOrNull = GetChannelOrNull(oAssemblyData, FileNumber);
		if (channelOrNull != null)
		{
			oAssemblyData.SetChannelObj(FileNumber, null);
			channelOrNull?.CloseFile();
		}
	}

	internal static string VB6CheckPathname(AssemblyData oAssemblyData, string sPath, OpenMode mode)
	{
		if (sPath.Contains('?') || sPath.Contains('*'))
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidPathChars1, sPath));
		}
		string fullName = new FileInfo(sPath).FullName;
		if (CheckFileOpen(oAssemblyData, fullName, OpenModeTypesFromOpenMode(mode)))
		{
			throw ExceptionUtils.VbMakeException(55);
		}
		return fullName;
	}

	internal static bool CheckFileOpen(AssemblyData oAssemblyData, string sPath, OpenModeTypes NewFileMode)
	{
		int num = 255;
		for (int i = 1; i <= num; i = checked(i + 1))
		{
			VB6File channelOrNull = GetChannelOrNull(oAssemblyData, i);
			if (channelOrNull == null)
			{
				continue;
			}
			OpenMode mode = channelOrNull.GetMode();
			if (string.Equals(sPath, channelOrNull.GetAbsolutePath(), StringComparison.OrdinalIgnoreCase))
			{
				if (NewFileMode == OpenModeTypes.Any)
				{
					return true;
				}
				if (((uint)NewFileMode | (uint)mode) != 1 && ((uint)NewFileMode | (uint)mode | 0x20 | 4) != 36)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void vbIOOpenFile(Assembly assem, int FileNumber, string FileName, OpenMode Mode, OpenAccess Access, OpenShare Share, int RecordLength)
	{
		AssemblyData assemblyData = ProjectData.GetProjectData().GetAssemblyData(assem);
		if (GetChannelOrNull(assemblyData, FileNumber) != null)
		{
			throw ExceptionUtils.VbMakeException(55);
		}
		if (FileName == null || FileName.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(75);
		}
		FileName = new FileInfo(FileName).FullName;
		if (CheckFileOpen(assemblyData, FileName, OpenModeTypesFromOpenMode(Mode)))
		{
			throw ExceptionUtils.VbMakeException(55);
		}
		if (RecordLength != -1 && RecordLength <= 0)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
		if (Mode == OpenMode.Binary)
		{
			RecordLength = 1;
		}
		else if (RecordLength == -1)
		{
			RecordLength = ((Mode != OpenMode.Random) ? 512 : 128);
		}
		if (Share == OpenShare.Default)
		{
			Share = OpenShare.LockReadWrite;
		}
		VB6File oFile;
		switch (Mode)
		{
		case OpenMode.Input:
			if (Access != OpenAccess.Read && Access != OpenAccess.Default)
			{
				throw new ArgumentException(System.SR.FileSystem_IllegalInputAccess);
			}
			oFile = new VB6InputFile(FileName, Share);
			break;
		case OpenMode.Output:
			if (Access != OpenAccess.Write && Access != OpenAccess.Default)
			{
				throw new ArgumentException(System.SR.FileSystem_IllegalOutputAccess);
			}
			oFile = new VB6OutputFile(FileName, Share, fAppend: false);
			break;
		case OpenMode.Random:
			if (Access == OpenAccess.Default)
			{
				Access = OpenAccess.ReadWrite;
			}
			oFile = new VB6RandomFile(FileName, Access, Share, RecordLength);
			break;
		case OpenMode.Append:
			if (Access != OpenAccess.Write && Access != OpenAccess.ReadWrite && Access != OpenAccess.Default)
			{
				throw new ArgumentException(System.SR.FileSystem_IllegalAppendAccess);
			}
			oFile = new VB6OutputFile(FileName, Share, fAppend: true);
			break;
		case OpenMode.Binary:
			if (Access == OpenAccess.Default)
			{
				Access = OpenAccess.ReadWrite;
			}
			oFile = new VB6BinaryFile(FileName, Access, Share);
			break;
		default:
			throw ExceptionUtils.VbMakeException(51);
		}
		AddFileToList(assemblyData, FileNumber, oFile);
	}

	private static void AddFileToList(AssemblyData oAssemblyData, int FileNumber, VB6File oFile)
	{
		if (oFile == null)
		{
			throw ExceptionUtils.VbMakeException(51);
		}
		oFile.OpenFile();
		oAssemblyData.SetChannelObj(FileNumber, oFile);
	}

	internal static VB6File GetChannelObj(Assembly assem, int FileNumber)
	{
		return GetChannelOrNull(ProjectData.GetProjectData().GetAssemblyData(assem), FileNumber) ?? throw ExceptionUtils.VbMakeException(52);
	}

	private static VB6File GetChannelOrNull(AssemblyData oAssemblyData, int FileNumber)
	{
		return oAssemblyData.GetChannelObj(FileNumber);
	}

	private static void CheckInputCapable(VB6File oFile)
	{
		if (!oFile.CanInput())
		{
			throw ExceptionUtils.VbMakeException(54);
		}
	}
}
