using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarComponentEventArgs : EventArgs
{
	public AvatarComponent Component { get; set; }

	public AvatarException Exception { get; set; }
}
