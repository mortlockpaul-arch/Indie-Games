using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic.FileIO;

public class SpecialDirectories
{
	public static string MyDocuments => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal), System.SR.IO_SpecialDirectory_MyDocuments);

	public static string MyMusic => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), System.SR.IO_SpecialDirectory_MyMusic);

	public static string MyPictures => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), System.SR.IO_SpecialDirectory_MyPictures);

	public static string Desktop => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), System.SR.IO_SpecialDirectory_Desktop);

	public static string Programs => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Programs), System.SR.IO_SpecialDirectory_Programs);

	public static string ProgramFiles => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), System.SR.IO_SpecialDirectory_ProgramFiles);

	public static string Temp => GetDirectoryPath(Path.GetTempPath(), System.SR.IO_SpecialDirectory_Temp);

	public static string CurrentUserApplicationData
	{
		[UnconditionalSuppressMessage("ReflectionAnalsys", "IL2026:RequiresUnreferencedCode", Justification = "Trimmer warns because it can't see System.Windows.Forms.Application. If the assembly is there, the trimmer will be able to tell to preserve the method specified.")]
		get
		{
			return GetDirectoryPath(GetWindowsFormsDirectory("System.Windows.Forms.Application", "UserAppDataPath"), System.SR.IO_SpecialDirectory_UserAppData);
		}
	}

	public static string AllUsersApplicationData
	{
		[UnconditionalSuppressMessage("ReflectionAnalsys", "IL2026:RequiresUnreferencedCode", Justification = "Trimmer warns because it can't see System.Windows.Forms.Application. If the assembly is there, the trimmer will be able to tell to preserve the method specified.")]
		get
		{
			return GetDirectoryPath(GetWindowsFormsDirectory("System.Windows.Forms.Application", "CommonAppDataPath"), System.SR.IO_SpecialDirectory_AllUserAppData);
		}
	}

	private static string GetDirectoryPath(string Directory, string DirectoryNameResID)
	{
		if (Operators.CompareString(Directory, "", TextCompare: false) == 0)
		{
			throw ExceptionUtils.GetDirectoryNotFoundException(System.SR.IO_SpecialDirectoryNotExist, DirectoryNameResID);
		}
		return FileSystem.NormalizePath(Directory);
	}

	[RequiresUnreferencedCode("Cannot statically analyze the passed in type.")]
	private static string GetWindowsFormsDirectory(string typeName, string propertyName)
	{
		PropertyInfo propertyInfo = Type.GetType($"{typeName}, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false)?.GetProperty(propertyName);
		if ((object)propertyInfo == null)
		{
			return "";
		}
		return (string)propertyInfo.GetValue(null, BindingFlags.DoNotWrapExceptions, null, null, null);
	}
}
