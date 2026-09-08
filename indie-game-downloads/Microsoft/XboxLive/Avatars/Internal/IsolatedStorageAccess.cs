using System.IO;
using System.IO.IsolatedStorage;

namespace Microsoft.XboxLive.Avatars.Internal;

public static class IsolatedStorageAccess
{
	public static IsolatedStorageFileStream CreateIsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, IsolatedStorageFile isf)
	{
		IsolatedStorageFileStream result = null;
		try
		{
			result = new IsolatedStorageFileStream(path, mode, access, share, isf);
		}
		catch (IOException)
		{
		}
		catch (IsolatedStorageException)
		{
		}
		return result;
	}
}
