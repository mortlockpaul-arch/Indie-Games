using System;
using Microsoft.XboxLive.Avatars.Internal.Animations;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarAnimationEventArgs : EventArgs
{
	public AvatarAnimation Animation { get; set; }

	public AvatarException Exception { get; set; }
}
