using System.Runtime.CompilerServices;

namespace Microsoft.Xna.Framework;

public static class FrameworkResources
{
	[SpecialName]
	public static string get_GamerServicesNotInitialized()
	{
		return "GamerServices not initialized";
	}

	[SpecialName]
	public static string get_NoGraphicsDevice()
	{
		return "No graphics device";
	}

	[SpecialName]
	public static string get_ObjectDisposedException()
	{
		return "Object disposed";
	}

	[SpecialName]
	public static string get_IAsyncNotFromBegin()
	{
		return "IAsyncResult not from Begin";
	}

	[SpecialName]
	public static string get_ResourceDataMustBeCorrectSize()
	{
		return "Resource data size invalid";
	}

	[SpecialName]
	public static string get_BindPoseNotAvailable()
	{
		return "Bind pose not available";
	}
}
