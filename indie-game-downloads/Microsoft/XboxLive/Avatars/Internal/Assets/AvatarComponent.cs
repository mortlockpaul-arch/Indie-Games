using System;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public class AvatarComponent
{
	public IBaseTextureAnimated[] m_Textures;

	public TriangleBatch[] m_Batches;

	public ShaderInstance[] m_ShaderInstance;

	public ComponentManifest m_AvatarComponentManifest;

	public Guid AssetId => m_AvatarComponentManifest.m_ComponentInfo.m_AssetId;

	public ComponentInfo AvatarComponentInfo => m_AvatarComponentManifest.m_ComponentInfo;

	public ComponentManifest Manifest => m_AvatarComponentManifest;

	public ShaderInstance[] ShaderInstanceParameters => m_ShaderInstance;

	public TriangleBatch[] TriangleBatches => m_Batches;

	public IBaseTextureAnimated GetTexture(int index)
	{
		return m_Textures[index];
	}

	public int GetTexturesCount()
	{
		return m_Textures.Length;
	}

	public IBaseTextureAnimated[] GetTextures()
	{
		return m_Textures;
	}

	public virtual int GetMemoryUsage()
	{
		int num = 0;
		TriangleBatch[] batches = m_Batches;
		TriangleBatch[] array = batches;
		foreach (TriangleBatch triangleBatch in array)
		{
			num += triangleBatch.GetMemoryUsage();
		}
		IBaseTextureAnimated[] textures = m_Textures;
		IBaseTextureAnimated[] array2 = textures;
		foreach (IBaseTextureAnimated baseTextureAnimated in array2)
		{
			num += baseTextureAnimated.GetMemoryUsage();
		}
		return num + m_ShaderInstance.Length * 32;
	}
}
