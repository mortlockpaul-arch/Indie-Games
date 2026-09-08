using System;
using Microsoft.XboxLive.Avatars.Internal;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarLoadedEventArgs : EventArgs
{
	public Avatar Avatar { get; private set; }

	public AvatarAssetLoadResult Result { get; private set; }

	public AvatarLoadedEventArgs(Avatar avatar, AvatarAssetLoadResult result)
	{
		Avatar = avatar;
		Result = result;
	}
}
