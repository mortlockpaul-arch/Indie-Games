using System;
using Microsoft.XboxLive.Avatars.Internal;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarAssetLoadContext
{
	public EventHandler<AvatarLoadedEventArgs> OnAvatarLoaded;

	public AvatarDescription AvatarDescription { get; private set; }

	public AvatarManifest AvatarManifest { get; private set; }

	public AvatarAssetLoadContext(AvatarDescription desc, AvatarManifest manifest)
	{
		AvatarDescription = desc;
		AvatarManifest = manifest;
	}
}
