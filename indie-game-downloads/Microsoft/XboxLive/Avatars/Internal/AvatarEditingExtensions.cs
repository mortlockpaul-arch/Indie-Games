using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.XboxLive.Avatars.Internal;

public sealed class AvatarEditingExtensions
{
	private AvatarEditingExtensions()
	{
	}

	public static AvatarDescription CreateAvatarDescriptor(AvatarManifest manifest)
	{
		if (manifest == null)
		{
			throw new ArgumentNullException();
		}
		return new AvatarDescription(manifest);
	}

	public static AvatarManifest GetAvatarManifest(AvatarDescription avatarDescription)
	{
		if (avatarDescription == null)
		{
			throw new ArgumentNullException();
		}
		return avatarDescription.AvatarManifest;
	}

	public static Avatar GetAvatar(AvatarRenderer renderer)
	{
		return renderer.Avatar;
	}

	public static void UpdateAvatarManifest(AvatarDescription avatarDescription, AvatarManifest manifest)
	{
		if (avatarDescription == null)
		{
			throw new ArgumentNullException("avatarDescription");
		}
		avatarDescription.UpdateManifest(manifest);
	}

	public static AssetDataManager GetDataManager()
	{
		return AvatarResources.DataMangager;
	}

	public static AssetLoader GetAssetLoader()
	{
		return AvatarResources.AssetLoader;
	}
}
