using System;
using System.Collections.Generic;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public class ComponentManifest
{
	public ComponentInfo m_ComponentInfo;

	public List<Guid> m_AssetIds = new List<Guid>();

	public ShaderConstantOverride[] m_ShaderOverrides;

	public int m_ModelIndex;

	public ComponentManifest(ComponentInfo componentInfo, ShaderConstantOverride[] shaderOverrides, int modelIndex)
	{
		m_ComponentInfo = componentInfo;
		m_ShaderOverrides = shaderOverrides;
		m_ModelIndex = modelIndex;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ComponentManifest))
		{
			return false;
		}
		return Equals(obj as ComponentManifest);
	}

	public override int GetHashCode()
	{
		int num = m_ComponentInfo.GetHashCode() ^ m_AssetIds.Count ^ m_ComponentInfo.CustomColor0.CompositeArgb ^ m_ComponentInfo.CustomColor1.CompositeArgb ^ m_ComponentInfo.CustomColor2.CompositeArgb ^ m_ModelIndex;
		int num2 = m_ShaderOverrides.Length;
		for (int i = 0; i < num2; i++)
		{
			num ^= m_ShaderOverrides[i].GetHashCode();
		}
		num2 = m_AssetIds.Count;
		for (int j = 0; j < num2; j++)
		{
			num ^= m_AssetIds[j].GetHashCode();
		}
		return num;
	}

	public bool Equals(ComponentManifest other)
	{
		if (m_ComponentInfo.AssetId != other.m_ComponentInfo.AssetId)
		{
			return false;
		}
		if (m_ModelIndex != other.m_ModelIndex)
		{
			return false;
		}
		if (m_AssetIds.Count != other.m_AssetIds.Count)
		{
			return false;
		}
		if (m_ComponentInfo.CustomColor0 != other.m_ComponentInfo.CustomColor0)
		{
			return false;
		}
		if (m_ComponentInfo.CustomColor1 != other.m_ComponentInfo.CustomColor1)
		{
			return false;
		}
		if (m_ComponentInfo.CustomColor2 != other.m_ComponentInfo.CustomColor2)
		{
			return false;
		}
		if (m_ShaderOverrides.Length != other.m_ShaderOverrides.Length)
		{
			return false;
		}
		int num = m_ShaderOverrides.Length;
		for (int i = 0; i < num; i++)
		{
			if (!m_ShaderOverrides[i].Equals(other.m_ShaderOverrides[i]))
			{
				return false;
			}
		}
		num = m_AssetIds.Count;
		for (int j = 0; j < num; j++)
		{
			if (!m_AssetIds[j].Equals(other.m_AssetIds[j]))
			{
				return false;
			}
		}
		return true;
	}

	public void AddAsset(Guid assetId)
	{
		m_AssetIds.Add(assetId);
	}
}
