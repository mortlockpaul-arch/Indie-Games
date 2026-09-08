using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class ComponentDescription
{
	public ComponentInfo m_ComponentInfo;

	public Guid m_OverrideAsset;

	public ComponentDescription()
	{
	}

	public ComponentDescription(ComponentInfo info, Guid overrideAssetId)
	{
		m_ComponentInfo = info;
		m_OverrideAsset = overrideAssetId;
	}

	public static bool operator ==(ComponentDescription a, ComponentDescription b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if (!(a.m_ComponentInfo == b.m_ComponentInfo))
		{
			return false;
		}
		if (!(a.m_OverrideAsset == b.m_OverrideAsset))
		{
			return false;
		}
		return true;
	}

	public static bool operator !=(ComponentDescription a, ComponentDescription b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ComponentDescription))
		{
			return false;
		}
		return this == (ComponentDescription)obj;
	}

	public override int GetHashCode()
	{
		return m_ComponentInfo.GetHashCode() + m_OverrideAsset.GetHashCode();
	}
}
