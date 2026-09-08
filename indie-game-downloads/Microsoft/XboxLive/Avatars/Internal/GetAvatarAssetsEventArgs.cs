using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarAssetsEventArgs : EventArgs
{
	public Avatar Avatar { get; set; }

	public AvatarException Exception { get; set; }
}
