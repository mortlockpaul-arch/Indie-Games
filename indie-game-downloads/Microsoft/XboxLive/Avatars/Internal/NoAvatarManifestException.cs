using System;

namespace Microsoft.XboxLive.Avatars.Internal;

[Serializable]
public class NoAvatarManifestException : Exception
{
	public NoAvatarManifestException()
	{
	}

	public NoAvatarManifestException(string message)
		: base(message)
	{
	}

	public NoAvatarManifestException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
