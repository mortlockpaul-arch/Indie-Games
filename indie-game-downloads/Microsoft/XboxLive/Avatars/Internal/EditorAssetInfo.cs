using System;
using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class EditorAssetInfo
{
	internal Guid m_assetId;

	internal uint m_assetDetails;

	internal BinaryAssetType m_assetType;

	internal List<ComponentInfo> m_previousAssets;

	internal Colorb m_assetDynamicColor;

	internal DynamicColorType m_assetDynamicColorType;

	internal ComponentColors m_componentColor;

	internal bool m_useComponentColor;

	internal bool m_removeEyeShadow;

	internal List<ComponentInfo> m_accessories = null;

	public Guid AssetId => m_assetId;

	public Colorb Color => m_assetDynamicColor;

	public DynamicColorType CustomColorType => m_assetDynamicColorType;

	public ComponentColors ComponentColor => m_componentColor;

	public EditorAssetInfo()
	{
		m_assetId = Guid.Empty;
		m_assetType = BinaryAssetType.Unknown;
		m_assetDynamicColorType = DynamicColorType.Count;
		m_assetDynamicColor = default(Colorb);
		m_useComponentColor = false;
	}

	public EditorAssetInfo(Guid newAsset)
	{
		m_assetId = newAsset;
		m_assetType = BinaryAssetType.Unknown;
		m_assetDynamicColorType = DynamicColorType.Count;
		m_assetDynamicColor = default(Colorb);
		m_useComponentColor = false;
	}

	public EditorAssetInfo(BlendShapeType blendShapeType, Guid blendShapeId)
	{
		m_assetId = blendShapeId;
		m_assetDetails = (uint)blendShapeType;
		m_assetType = BinaryAssetType.ShapeOverride;
		m_assetDynamicColorType = DynamicColorType.Count;
		m_assetDynamicColor = default(Colorb);
		m_useComponentColor = false;
	}

	public EditorAssetInfo(DynamicTextureType textureType, Guid textureId)
	{
		m_assetId = textureId;
		m_assetDetails = (uint)textureType;
		m_assetType = BinaryAssetType.Texture;
		m_assetDynamicColorType = DynamicColorType.Count;
		m_assetDynamicColor = default(Colorb);
	}

	public EditorAssetInfo(Guid newAsset, ComponentColors customColors)
	{
		m_assetId = newAsset;
		m_assetType = BinaryAssetType.Unknown;
		m_assetDynamicColorType = DynamicColorType.Count;
		m_assetDynamicColor = default(Colorb);
		m_useComponentColor = true;
		m_componentColor = customColors;
	}

	public EditorAssetInfo(AvatarComponentMasks removeMask)
	{
		m_assetId = Guid.Empty;
		m_assetDetails = (uint)removeMask;
		m_assetType = BinaryAssetType.Component;
		m_assetDynamicColorType = DynamicColorType.Count;
		m_assetDynamicColor = default(Colorb);
	}

	public void SetAssetCustomColor(Colorb color, DynamicColorType type)
	{
		m_assetDynamicColor = color;
		m_assetDynamicColorType = type;
	}

	public void SetAssetCustomColor(ComponentColors color)
	{
		m_componentColor = color;
		m_useComponentColor = true;
	}

	public void SetAccessories(List<ComponentInfo> accessories)
	{
		m_accessories = accessories;
	}
}
