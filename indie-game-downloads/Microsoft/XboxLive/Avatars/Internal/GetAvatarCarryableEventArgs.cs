using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal;

public class GetAvatarCarryableEventArgs : EventArgs
{
	public AvatarCarryable Carryable { get; set; }

	public AvatarException Exception { get; set; }
}
