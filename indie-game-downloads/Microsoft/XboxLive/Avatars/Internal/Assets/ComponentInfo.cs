using System;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public struct ComponentInfo
{
	public Guid m_AssetId;

	public AvatarComponentMasks m_ComponentMask;

	public Colorb m_CustomColors0;

	public Colorb m_CustomColors1;

	public Colorb m_CustomColors2;

	public static readonly ComponentInfo Zero;

	public bool IsEmpty
	{
		get
		{
			bool flag = true;
			return m_AssetId == Guid.Empty;
		}
	}

	public Colorb CustomColor0 => m_CustomColors0;

	public Colorb CustomColor1 => m_CustomColors1;

	public Colorb CustomColor2 => m_CustomColors2;

	public AvatarComponentMasks ComponentMask => m_ComponentMask;

	public Guid AssetId => m_AssetId;

	public ComponentInfo(Guid assetId, AvatarComponentMasks componentMask, Colorb customColor1, Colorb customColor2, Colorb customColor3)
	{
		m_AssetId = assetId;
		m_ComponentMask = componentMask;
		m_CustomColors0 = customColor1;
		m_CustomColors1 = customColor2;
		m_CustomColors2 = customColor3;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ComponentInfo componentInfo) || 1 == 0)
		{
			return false;
		}
		return this == componentInfo;
	}

	public override int GetHashCode()
	{
		int num = m_CustomColors0.GetHashCode() + m_CustomColors1.GetHashCode() + m_CustomColors2.GetHashCode();
		return num + (int)m_ComponentMask + m_AssetId.GetHashCode();
	}

	public static bool operator ==(ComponentInfo a, ComponentInfo b)
	{
		if (!(a.m_AssetId == b.m_AssetId))
		{
			return false;
		}
		if (a.m_ComponentMask != b.m_ComponentMask)
		{
			return false;
		}
		if (!(a.m_CustomColors0 == b.m_CustomColors0))
		{
			return false;
		}
		if (!(a.m_CustomColors1 == b.m_CustomColors1))
		{
			return false;
		}
		if (!(a.m_CustomColors2 == b.m_CustomColors2))
		{
			return false;
		}
		return true;
	}

	public static bool operator !=(ComponentInfo a, ComponentInfo b)
	{
		return !(a == b);
	}
}
