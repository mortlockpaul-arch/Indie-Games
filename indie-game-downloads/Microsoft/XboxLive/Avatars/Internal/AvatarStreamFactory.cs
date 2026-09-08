using System;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class AvatarStreamFactory
{
	public abstract Stream CreateStream(Guid avatarAsset);

	public AvatarStreamFactory()
	{
	}
}
