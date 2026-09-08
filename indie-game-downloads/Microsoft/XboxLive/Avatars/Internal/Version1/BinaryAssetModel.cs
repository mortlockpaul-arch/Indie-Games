using System;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class BinaryAssetModel : BinaryAssetShapeOverride
{
	public ShaderConstantOverride[] m_ShaderConstantOverrides;

	public ComponentInfo m_ComponentDescription;

	public bool m_ContainShapeOverrides = false;

	public ShaderConstantOverride[] ShaderConstantOverrides => m_ShaderConstantOverrides;

	public BinaryAssetModel(ComponentInfo description, int shaderOverridesCount, AvatarComponentMasks mask)
		: base(description.m_AssetId, mask)
	{
		m_ComponentDescription = description;
		m_AssetType = BinaryAssetParserType.Model;
		m_ShaderConstantOverrides = new ShaderConstantOverride[shaderOverridesCount];
	}

	public void SetAssetId(Guid id)
	{
		m_AssetId = id;
		m_ComponentDescription.m_AssetId = id;
	}

	public override bool Validate(BinaryAssetParseContext context)
	{
		AssetMetadataParser metadata = GetMetadata();
		if (metadata == null)
		{
			Logger.Log(new DebugLog(this, $"Missing metadata in {m_AssetId}"));
			return false;
		}
		if ((metadata.BodyTypeMask & context.m_BodyType) == 0)
		{
			Logger.Log(new DebugLog(this, $"Gender mismatch in {m_AssetId}"));
			return false;
		}
		if (metadata.AssetType != BinaryAssetType.Component)
		{
			Logger.Log(new DebugLog(this, $"Asset type mismatch in {m_AssetId}"));
			return false;
		}
		if (m_ComponentMask != AvatarComponentMasks.None)
		{
			uint assetTypeDetails = metadata.AssetTypeDetails;
			AvatarComponentMasks avatarComponentMasks = (AvatarComponentMasks)((int)assetTypeDetails & -8388609);
			if (avatarComponentMasks != m_ComponentMask)
			{
				Logger.Log(new DebugLog(this, $"Invalid component category mask in asset {m_AssetId}. Mask {m_ComponentMask} expected and {avatarComponentMasks} get"));
				return false;
			}
		}
		if (context.m_skeletonVersion != metadata.AssetSkeletonVersion)
		{
			Logger.Log(new DebugLog(this, $"Skeleton version does not match. Required {context.m_skeletonVersion}, received {metadata.AssetSkeletonVersion} for component {m_AssetId}"));
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
			Logger.Log(new DebugLog(this, $"Missing metadata in {m_AssetId}"));
			return false;
		}
		if ((m_Cache.m_Metadata.BodyTypeMask & context.m_BodyType) == 0)
		{
			Logger.Log(new DebugLog(this, $"Gender mismatch in {m_AssetId}"));
			return false;
		}
		if (m_Cache.m_Metadata.AssetType != BinaryAssetType.Component)
		{
			Logger.Log(new DebugLog(this, $"Asset type mismatch in {m_AssetId}"));
			return false;
		}
		if (m_ComponentMask != AvatarComponentMasks.None)
		{
			uint assetTypeDetails = m_Cache.m_Metadata.AssetTypeDetails;
			AvatarComponentMasks avatarComponentMasks = (AvatarComponentMasks)((int)assetTypeDetails & -8388609);
			if (avatarComponentMasks != m_ComponentMask)
			{
				Logger.Log(new DebugLog(this, $"Invalid component category mask in asset {m_AssetId}. Mask {m_ComponentMask} expected and {avatarComponentMasks} get"));
				return false;
			}
		}
		if (context.m_skeletonVersion != m_Cache.m_Metadata.m_skeletonVersion)
		{
			Logger.Log(new DebugLog(this, $"Skeleton version does not match. Required {context.m_skeletonVersion}, received {m_Cache.m_Metadata.m_skeletonVersion} for component {m_AssetId}"));
			return false;
		}
		return true;
	}

	public override bool ProcessOverridesFromStream(BinaryAssetParseContext context)
	{
		if (m_ContainShapeOverrides)
		{
			return base.ProcessOverridesFromStream(context);
		}
		return true;
	}

	public override bool ProcessOverridesFromCache(BinaryAssetParseContext context)
	{
		if (m_ContainShapeOverrides)
		{
			return base.ProcessOverridesFromCache(context);
		}
		return true;
	}

	public override bool ProcessComponentsFromStream(BinaryAssetParseContext context)
	{
		AssetMetadataParser metadata = GetMetadata();
		if (metadata == null)
		{
			return false;
		}
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
		if (!iterator.FindFirst(StructuredBinaryBlockId.Model))
		{
			return false;
		}
		int modelIndex = 0;
		if ((context.m_CombinedComponentMask & AvatarComponentMasks.Hat) != AvatarComponentMasks.None && m_ComponentDescription.m_ComponentMask == AvatarComponentMasks.Hair)
		{
			iterator.NextBlock();
			if (!iterator.Find(StructuredBinaryBlockId.Model))
			{
				iterator.FindFirst(StructuredBinaryBlockId.Model);
			}
			else
			{
				modelIndex = 1;
			}
		}
		AvatarComponent model = new AvatarComponent();
		model.m_AvatarComponentManifest = new ComponentManifest(m_ComponentDescription, m_ShaderConstantOverrides, modelIndex);
		ProcessModel(iterator, ref model, context);
		iterator.FirstBlock();
		m_ContainShapeOverrides = iterator.FindFirst(StructuredBinaryBlockId.ShapeOverrides);
		context.m_Target.AddComponent(model);
		return true;
	}

	public void ProcessModel(BlockIterator iterator, ref AvatarComponent model, BinaryAssetParseContext context)
	{
		Vector4[] array = new Vector4[3];
		AssetModelParser assetModelParser = new AssetModelParser(model, context.m_CoordinateSystem);
		if ((int)iterator.Length <= 0)
		{
			Logger.Log(new DebugLog(this, "block size <= 0; strb file is corrupted"));
			throw new AvatarException(Resources.InvalidStrbFileText);
		}
		DecompressStream stream = new DecompressStream(iterator, (int)iterator.Length);
		assetModelParser.Parse(stream, context.m_ResourceFactory);
		int num = m_ShaderConstantOverrides.Length;
		while (--num >= 0)
		{
			ShaderConstantOverride shaderConstantOverride = m_ShaderConstantOverrides[num];
			ShaderParameterData paramData = default(ShaderParameterData);
			paramData.Constant.SetValue(shaderConstantOverride.m_Value);
			OverrideShaderParameter(model, shaderConstantOverride.m_Constant, paramData);
			if (shaderConstantOverride.m_Constant >= ShaderParameterUsage.PixelConstantColorCustom0 && shaderConstantOverride.m_Constant <= ShaderParameterUsage.PixelConstantColorCustom2)
			{
				ref Vector4 reference = ref array[(int)(shaderConstantOverride.m_Constant - 22)];
				reference = shaderConstantOverride.m_Value;
			}
		}
		if (iterator.FindFirst(StructuredBinaryBlockId.CustomColorTable))
		{
			ComponentColorTable colorTable = AssetCustomColorTableParser.Parse(iterator);
			ApplyColorTable(model, array, colorTable);
		}
	}

	public static void OverrideShaderParameter(AvatarComponent model, ShaderParameterUsage paramUsage, ShaderParameterData paramData)
	{
		if (paramUsage == ShaderParameterUsage.None)
		{
			return;
		}
		int num = model.m_Batches.Length;
		while (--num >= 0)
		{
			ShaderInstance shaderInstance = model.m_ShaderInstance[num];
			int num2 = shaderInstance.ShaderParameters.Length;
			while (--num2 >= 0)
			{
				if (shaderInstance.ShaderParameters[num2].usage == paramUsage)
				{
					shaderInstance.ShaderParameters[num2].data = paramData;
				}
			}
		}
	}

	public override bool ProcessComponentsFromCache(BinaryAssetParseContext context)
	{
		if (!(m_Cache is CachedBinaryAssetModel cachedBinaryAssetModel))
		{
			return false;
		}
		m_SkeletonVersion = m_Cache.m_Metadata.m_skeletonVersion;
		int count = cachedBinaryAssetModel.Models.Count;
		if (count == 0)
		{
			return false;
		}
		int num = 0;
		if ((context.m_CombinedComponentMask & AvatarComponentMasks.Hat) != AvatarComponentMasks.None && m_ComponentDescription.ComponentMask == AvatarComponentMasks.Hair && count > 1)
		{
			num = 1;
		}
		AvatarComponent model = new AvatarComponent();
		model.m_AvatarComponentManifest = new ComponentManifest(m_ComponentDescription, m_ShaderConstantOverrides, num);
		if (!ProcessModel(cachedBinaryAssetModel.Models[num], cachedBinaryAssetModel.ColorTable, ref model))
		{
			return false;
		}
		context.m_Target.AddComponent(model);
		m_ContainShapeOverrides = cachedBinaryAssetModel.m_ShapeOverrides.Count > 0;
		return true;
	}

	public bool ProcessModel(AvatarComponent cachedModel, ComponentColorTable cachedColorTable, ref AvatarComponent model)
	{
		int num = cachedModel.m_Textures.Length;
		model.m_Textures = new IBaseTextureAnimated[num];
		for (int i = 0; i < num; i++)
		{
			model.m_Textures[i] = cachedModel.m_Textures[i];
		}
		int num2 = cachedModel.m_Batches.Length;
		model.m_Batches = new TriangleBatch[num2];
		for (int j = 0; j < num2; j++)
		{
			ref TriangleBatch reference = ref model.m_Batches[j];
			reference = cachedModel.m_Batches[j].Clone();
		}
		int num3 = cachedModel.m_ShaderInstance.Length;
		model.m_ShaderInstance = new ShaderInstance[num3];
		for (int k = 0; k < num3; k++)
		{
			ref ShaderInstance reference2 = ref model.m_ShaderInstance[k];
			reference2 = cachedModel.m_ShaderInstance[k].Clone();
		}
		Vector4[] array = new Vector4[3];
		int num4 = m_ShaderConstantOverrides.Length;
		int num5 = num4;
		while (--num5 >= 0)
		{
			ShaderConstantOverride shaderConstantOverride = m_ShaderConstantOverrides[num5];
			ShaderParameterData paramData = default(ShaderParameterData);
			paramData.Constant.SetValue(shaderConstantOverride.m_Value);
			OverrideShaderParameter(model, shaderConstantOverride.m_Constant, paramData);
			if (shaderConstantOverride.m_Constant >= ShaderParameterUsage.PixelConstantColorCustom0 && shaderConstantOverride.m_Constant <= ShaderParameterUsage.PixelConstantColorCustom2)
			{
				ref Vector4 reference3 = ref array[(int)(shaderConstantOverride.m_Constant - 22)];
				reference3 = shaderConstantOverride.m_Value;
			}
		}
		ApplyColorTable(model, array, cachedColorTable);
		return true;
	}

	public static void ApplyColorTable(AvatarComponent model, Vector4[] customColors, ComponentColorTable colorTable)
	{
		if (colorTable == null)
		{
			return;
		}
		bool flag = false;
		int num = colorTable.Colors.Length;
		int num2 = num;
		while (--num2 >= 0)
		{
			Vector4 customColor = colorTable.Colors[num2].CustomColor0;
			Vector4 customColor2 = colorTable.Colors[num2].CustomColor1;
			Vector4 customColor3 = colorTable.Colors[num2].CustomColor2;
			if (Vector4.DistanceSquared(customColors[0], customColor) + Vector4.DistanceSquared(customColors[1], customColor2) + Vector4.DistanceSquared(customColors[2], customColor3) < 1E-05f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			ShaderParameterData paramData = default(ShaderParameterData);
			paramData.Constant.SetValue(colorTable.Colors[0].CustomColor0);
			OverrideShaderParameter(model, ShaderParameterUsage.PixelConstantColorCustom0, paramData);
			paramData.Constant.SetValue(colorTable.Colors[0].CustomColor1);
			OverrideShaderParameter(model, ShaderParameterUsage.PixelConstantColorCustom1, paramData);
			paramData.Constant.SetValue(colorTable.Colors[0].CustomColor2);
			OverrideShaderParameter(model, ShaderParameterUsage.PixelConstantColorCustom2, paramData);
		}
	}

	public override CachedBinaryAsset CreateCacheItem()
	{
		return new CachedBinaryAssetModel(m_AssetId);
	}
}
