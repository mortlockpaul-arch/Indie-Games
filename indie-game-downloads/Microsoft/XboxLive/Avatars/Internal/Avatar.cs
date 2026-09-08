using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal;

public class Avatar
{
	public Skeleton.SkeletonVersion m_SkeletonVersion;

	public Skeleton m_Skeleton;

	public List<AvatarComponent> m_Models = new List<AvatarComponent>();

	public AvatarCarryable m_Carryable;

	public AvatarManifest m_Manifest;

	public AvatarManifest Manifest => m_Manifest;

	public Skeleton Skeleton => m_Skeleton;

	public Skeleton.SkeletonVersion SkeletonVersion => m_SkeletonVersion;

	public AvatarCarryable Carryable
	{
		get
		{
			return m_Carryable;
		}
		set
		{
			m_Carryable = value;
		}
	}

	public List<AvatarComponent> Models => m_Models;

	public Avatar(AvatarManifest manifest)
	{
		m_SkeletonVersion = Skeleton.SkeletonVersion.Invalid;
		m_Manifest = manifest;
	}

	public void AddComponent(AvatarComponent component)
	{
		m_Models.Add(component);
	}
}
