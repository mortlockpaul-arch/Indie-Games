using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderableModelCacheKey
{
	private ComponentManifest m_Manifest;

	private int m_ModelIndex;

	public AvatarRenderableModelCacheKey(ComponentManifest manifest, int modelIndex)
	{
		m_Manifest = manifest;
		m_ModelIndex = modelIndex;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AvatarRenderableModelCacheKey))
		{
			return false;
		}
		return Equals((AvatarRenderableModelCacheKey)obj);
	}

	public bool Equals(AvatarRenderableModelCacheKey other)
	{
		if (m_ModelIndex != other.m_ModelIndex)
		{
			return false;
		}
		if (!m_Manifest.Equals(other.m_Manifest))
		{
			return false;
		}
		return true;
	}

	public static bool operator ==(AvatarRenderableModelCacheKey key1, AvatarRenderableModelCacheKey key2)
	{
		return key1.Equals(key2);
	}

	public static bool operator !=(AvatarRenderableModelCacheKey key1, AvatarRenderableModelCacheKey key2)
	{
		return !key1.Equals(key2);
	}

	public override int GetHashCode()
	{
		return m_ModelIndex + m_Manifest.GetHashCode();
	}
}
