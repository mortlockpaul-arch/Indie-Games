using System;
using System.Collections;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetTexture : BinaryAsset
{
	public const int MaxTexturesPerModel = 18;

	public static ShaderParameterUsage[] MANIFEST_TEXTURE_TO_PARAM_USAGE_MAP = new ShaderParameterUsage[18]
	{
		ShaderParameterUsage.TextureMouth,
		ShaderParameterUsage.None,
		ShaderParameterUsage.None,
		ShaderParameterUsage.TextureEyeLeft,
		ShaderParameterUsage.TextureEyeRight,
		ShaderParameterUsage.None,
		ShaderParameterUsage.TextureEyebrowLeft,
		ShaderParameterUsage.TextureEyebrowRight,
		ShaderParameterUsage.None,
		ShaderParameterUsage.TextureFacialHair,
		ShaderParameterUsage.None,
		ShaderParameterUsage.None,
		ShaderParameterUsage.TextureEyeShadow,
		ShaderParameterUsage.None,
		ShaderParameterUsage.None,
		ShaderParameterUsage.TextureSkinFeatures,
		ShaderParameterUsage.None,
		ShaderParameterUsage.None
	};

	public DynamicTextureType m_Texture;

	public override bool IsCoordinateSystemIndependent => true;

	public BinaryAssetTexture(DynamicTextureType texture, Guid id)
		: base(AvatarComponentMasks.None)
	{
		m_AssetId = id;
		m_Texture = texture;
		m_AssetType = BinaryAssetParserType.Texture;
	}

	public override bool ProcessAssetsFromStream(BinaryAssetParseContext context)
	{
		StructuredBinary structuredBinary = new StructuredBinary();
		m_Stream.Seek(0L, SeekOrigin.Begin);
		if (!structuredBinary.Open(m_Stream))
		{
			return false;
		}
		if (!(structuredBinary.Namespace == BinaryAsset.AvatarAssetGuid))
		{
			return false;
		}
		BlockIterator iterator = structuredBinary.Iterator;
		if (!iterator.Find(StructuredBinaryBlockId.Texture))
		{
			return false;
		}
		if ((int)iterator.Length <= 0)
		{
			Logger.Log(new DebugLog(this, "block size <= 0; strb file is corrupted"));
			return false;
		}
		DecompressStream stream = new DecompressStream(iterator, (int)iterator.Length);
		bool flag = false;
		AssetTextureParser assetTextureParser = new AssetTextureParser();
		Avatar target = context.m_Target;
		int num = target.Models.Count;
		while (--num >= 0)
		{
			AvatarComponent avatarComponent = target.Models[num];
			bool flag2 = false;
			BitArray bitArray = new BitArray(18);
			int texture = (int)m_Texture;
			ShaderParameterUsage[] mANIFEST_TEXTURE_TO_PARAM_USAGE_MAP = MANIFEST_TEXTURE_TO_PARAM_USAGE_MAP;
			for (int i = 0; i < 3; i++)
			{
				ShaderParameterUsage shaderParameterUsage = mANIFEST_TEXTURE_TO_PARAM_USAGE_MAP[texture * 3 + i];
				int num2 = avatarComponent.m_Batches.Length;
				while (--num2 >= 0)
				{
					ShaderInstance shaderInstance = avatarComponent.m_ShaderInstance[num2];
					int num3 = shaderInstance.ShaderParameters.Length;
					while (--num3 >= 0)
					{
						ShaderParameter shaderParameter = shaderInstance.ShaderParameters[num3];
						if (shaderParameter.usage == shaderParameterUsage && !bitArray.Get(shaderParameter.data.Texture.TextureIndex))
						{
							bitArray.Set(shaderParameter.data.Texture.TextureIndex, value: true);
							if (!flag)
							{
								flag = true;
								assetTextureParser.Parse(stream, context.m_ResourceFactory);
							}
							avatarComponent.m_Textures[shaderParameter.data.Texture.TextureIndex] = assetTextureParser.Texture;
							if (!flag2)
							{
								avatarComponent.m_AvatarComponentManifest.AddAsset(m_AssetId);
								flag2 = true;
							}
						}
					}
				}
			}
		}
		return true;
	}

	public override bool Validate(BinaryAssetParseContext context)
	{
		AssetMetadataParser metadata = GetMetadata();
		if (metadata == null)
		{
			return false;
		}
		if ((metadata.BodyTypeMask & context.m_BodyType) == 0)
		{
			return false;
		}
		if (metadata.AssetType != BinaryAssetType.Texture)
		{
			return false;
		}
		uint assetTypeDetails = metadata.AssetTypeDetails;
		DynamicTextureType dynamicTextureType = (DynamicTextureType)assetTypeDetails;
		if (dynamicTextureType != m_Texture)
		{
			return false;
		}
		return true;
	}

	public override bool ValidateFromCache(BinaryAssetParseContext context)
	{
		if (m_Cache == null)
		{
			return false;
		}
		if (m_Cache.m_Metadata == null)
		{
			return false;
		}
		if ((m_Cache.m_Metadata.BodyTypeMask & context.m_BodyType) == 0)
		{
			return false;
		}
		if (m_Cache.m_Metadata.AssetType != BinaryAssetType.Texture)
		{
			return false;
		}
		uint assetTypeDetails = m_Cache.m_Metadata.AssetTypeDetails;
		DynamicTextureType dynamicTextureType = (DynamicTextureType)assetTypeDetails;
		if (dynamicTextureType != m_Texture)
		{
			return false;
		}
		return true;
	}

	public override bool ProcessAssetsFromCache(BinaryAssetParseContext context)
	{
		if (!(m_Cache is CachedBinaryAssetTexture cachedBinaryAssetTexture))
		{
			return false;
		}
		m_SkeletonVersion = m_Cache.m_Metadata.m_skeletonVersion;
		Avatar target = context.m_Target;
		int num = target.m_Models.Count;
		while (--num >= 0)
		{
			AvatarComponent avatarComponent = target.m_Models[num];
			bool flag = false;
			int num2 = 0;
			int texture = (int)m_Texture;
			for (int i = 0; i < 3; i++)
			{
				ShaderParameterUsage shaderParameterUsage = MANIFEST_TEXTURE_TO_PARAM_USAGE_MAP[texture * 3 + i];
				int num3 = avatarComponent.m_Batches.Length;
				while (--num3 >= 0)
				{
					ShaderInstance[] shaderInstance = avatarComponent.m_ShaderInstance;
					int num4 = shaderInstance[num3].ShaderParameters.Length;
					while (--num4 >= 0)
					{
						ShaderParameter shaderParameter = shaderInstance[num3].ShaderParameters[num4];
						if (shaderParameter.usage == shaderParameterUsage && (num2 & (1 << (int)shaderParameter.data.Texture.TextureIndex)) == 0)
						{
							num2 |= 1 << (int)shaderParameter.data.Texture.TextureIndex;
							avatarComponent.m_Textures[shaderParameter.data.Texture.TextureIndex] = cachedBinaryAssetTexture.GetTexture();
							if (!flag)
							{
								avatarComponent.m_AvatarComponentManifest.AddAsset(m_AssetId);
								flag = true;
							}
						}
					}
				}
			}
		}
		return true;
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetTexture(m_AssetId);
	}
}
