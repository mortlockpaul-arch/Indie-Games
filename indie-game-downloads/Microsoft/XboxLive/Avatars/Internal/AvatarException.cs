using System;

namespace Microsoft.XboxLive.Avatars.Internal;

[Serializable]
public class AvatarException : Exception
{
	public AvatarException()
	{
	}

	public AvatarException(string message)
		: base(message)
	{
	}

	public AvatarException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
