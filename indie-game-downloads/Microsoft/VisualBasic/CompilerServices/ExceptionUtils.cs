using System;
using System.IO;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class ExceptionUtils
{
	internal static Exception VbMakeIllegalForException()
	{
		return VbMakeExceptionEx(92, System.SR.Format(System.SR.ID92));
	}

	internal static Exception VbMakeObjNotSetException()
	{
		return VbMakeExceptionEx(91, System.SR.Format(System.SR.ID91));
	}

	internal static Exception VbMakeException(int hr)
	{
		string sMsg = ((hr <= 0 || hr > 65535) ? "" : Utils.GetResourceString((vbErrors)hr));
		return VbMakeExceptionEx(hr, sMsg);
	}

	internal static Exception VbMakeException(Exception ex, int hr)
	{
		Information.Err().SetUnmappedError(hr);
		return ex;
	}

	internal static Exception VbMakeExceptionEx(int number, string sMsg)
	{
		bool VBDefinedError = default(bool);
		return BuildException(number, sMsg, ref VBDefinedError);
	}

	internal static Exception BuildException(int Number, string Description, ref bool VBDefinedError)
	{
		VBDefinedError = true;
		switch (Number)
		{
		case 3:
		case 20:
		case 94:
		case 100:
			return new InvalidOperationException(Description);
		case 5:
		case 446:
		case 448:
		case 449:
			return new ArgumentException(Description);
		case 438:
			return new MissingMemberException(Description);
		case 6:
			return new OverflowException(Description);
		case 7:
		case 14:
			return new OutOfMemoryException(Description);
		case 9:
			return new IndexOutOfRangeException(Description);
		case 11:
			return new DivideByZeroException(Description);
		case 13:
			return new InvalidCastException(Description);
		case 28:
			return new StackOverflowException(Description);
		case 48:
			return new TypeLoadException(Description);
		case 53:
			return new FileNotFoundException(Description);
		case 62:
			return new EndOfStreamException(Description);
		case 52:
		case 54:
		case 55:
		case 57:
		case 58:
		case 59:
		case 61:
		case 63:
		case 67:
		case 68:
		case 70:
		case 71:
		case 74:
		case 75:
			return new IOException(Description);
		case 76:
		case 432:
			return new FileNotFoundException(Description);
		case 91:
			return new NullReferenceException(Description);
		case 422:
			return new MissingFieldException(Description);
		case 429:
		case 462:
			return new Exception(Description);
		case -2147467261:
			return new AccessViolationException();
		default:
			VBDefinedError = false;
			return new Exception(Description);
		case 0:
			VBDefinedError = false;
			return new Exception(Description);
		}
	}

	internal static ArgumentException GetArgumentExceptionWithArgName(string ArgumentName, string ResourceID, params string[] PlaceHolders)
	{
		return new ArgumentException(System.SR.Format(ResourceID, PlaceHolders), ArgumentName);
	}

	internal static ArgumentNullException GetArgumentNullException(string ArgumentName)
	{
		return new ArgumentNullException(ArgumentName, System.SR.General_ArgumentNullException);
	}

	internal static ArgumentNullException GetArgumentNullException(string ArgumentName, string ResourceID, params string[] PlaceHolders)
	{
		return new ArgumentNullException(ArgumentName, System.SR.Format(ResourceID, PlaceHolders));
	}

	internal static DirectoryNotFoundException GetDirectoryNotFoundException(string ResourceID, params string[] PlaceHolders)
	{
		return new DirectoryNotFoundException(System.SR.Format(ResourceID, PlaceHolders));
	}

	internal static FileNotFoundException GetFileNotFoundException(string FileName, string ResourceID, params string[] PlaceHolders)
	{
		return new FileNotFoundException(System.SR.Format(ResourceID, PlaceHolders), FileName);
	}

	internal static InvalidOperationException GetInvalidOperationException(string ResourceID, params string[] PlaceHolders)
	{
		return new InvalidOperationException(System.SR.Format(ResourceID, PlaceHolders));
	}

	internal static IOException GetIOException(string ResourceID, params string[] PlaceHolders)
	{
		return new IOException(System.SR.Format(ResourceID, PlaceHolders));
	}
}
