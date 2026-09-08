#define DEBUG
using System;
using System.Diagnostics;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Version1;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AvatarManifestEditor
{
	public enum RemovableComponents
	{
		NotRemovable,
		Earrings,
		Glasses,
		Gloves,
		Hat,
		Ring,
		Wristwear,
		FacialHair,
		EyeShadow,
		SkinFeatures,
		Prop
	}

	private AvatarManifestV1 m_Manifest;

	private AssetLoader m_AvatarApi;

	private IDataManager m_dataManager;

	private bool m_dirty;

	public float AvatarWidthFactor
	{
		get
		{
			return m_Manifest.WidthFactor;
		}
		set
		{
			if (value <= 1f && value >= -1f)
			{
				m_Manifest.WidthFactor = value;
				return;
			}
			throw new ArgumentOutOfRangeException("The value must be in range [-1,1]");
		}
	}

	public float AvatarHeightFactor
	{
		get
		{
			return m_Manifest.HeightFactor;
		}
		set
		{
			if (value <= 1f && value >= -1f)
			{
				m_Manifest.HeightFactor = value;
				return;
			}
			throw new ArgumentOutOfRangeException("The value must be in range [-1,1]");
		}
	}

	public bool DressDefaultClothes
	{
		get
		{
			return m_Manifest.DressDefaultClothes;
		}
		set
		{
			m_Manifest.DressDefaultClothes = value;
		}
	}

	public AvatarManifest Manifest => m_Manifest;

	public bool IsDirty => m_dirty;

	public byte[] OwnerXuid
	{
		get
		{
			return m_Manifest.m_OwnerXuid;
		}
		set
		{
			m_Manifest.m_OwnerXuid = value;
		}
	}

	public byte[] ConsoleId
	{
		get
		{
			return m_Manifest.m_ConsoleId;
		}
		set
		{
			m_Manifest.m_ConsoleId = value;
		}
	}

	public AvatarManifestEditor(AvatarManifest manifest, IDataManager downloadManager, AssetLoader avatarApi)
	{
		if (manifest == null)
		{
			throw new ArgumentNullException("manifest");
		}
		int version = manifest.Version;
		int num = version;
		if (num == 1)
		{
			m_Manifest = manifest as AvatarManifestV1;
			m_dirty = false;
			m_AvatarApi = avatarApi;
			m_dataManager = downloadManager;
			return;
		}
		throw new ArgumentException("manifest", "Invalid manifest");
	}

	public void UpdateDependencies()
	{
		if (m_dataManager == null)
		{
			throw new InvalidOperationException("Data manager has not beed specified");
		}
		m_Manifest.UpdateDependencies(m_dataManager);
		m_dirty = false;
	}

	public void SetManekinColorScheme()
	{
		for (DynamicColorType dynamicColorType = DynamicColorType.Skin; dynamicColorType < DynamicColorType.Count; dynamicColorType++)
		{
			Colorb color = Utilities.ColorbFromVector4(m_Manifest.GetDynamicColor(dynamicColorType));
			int num = color.red + color.green + color.blue + 765;
			color.red = (byte)((num + color.red) / 7);
			color.green = (byte)((num + color.green) / 7);
			color.blue = (byte)((num + color.blue) / 7);
			m_Manifest.SetDynamicColor(dynamicColorType, color);
		}
		Colorb color2 = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		m_Manifest.SetDynamicColor(DynamicColorType.Skin, color2);
		m_dirty = true;
	}

	public void SetAvatarColor(DynamicColorType colorType, Colorb avatarColor)
	{
		if (colorType == DynamicColorType.EyeShadow && m_Manifest.GetReplacementTexture(DynamicTextureType.EyeShadow).m_TextureAssetId == Guid.Empty)
		{
			UpdateEyeShadow();
		}
		m_Manifest.SetDynamicColor(colorType, avatarColor);
		m_dirty = true;
	}

	public Colorb GetAvatarColor(DynamicColorType colorType)
	{
		return Utilities.ColorbFromVector4(m_Manifest.GetDynamicColor(colorType));
	}

	public void RemoveAllComponents()
	{
		m_Manifest.ClearComponents(AvatarComponentMasks.Head | AvatarComponentMasks.Body | AvatarComponentMasks.Shirt | AvatarComponentMasks.Trousers | AvatarComponentMasks.Shoes | AvatarComponentMasks.Hat | AvatarComponentMasks.Gloves | AvatarComponentMasks.Glasses | AvatarComponentMasks.Wristwear | AvatarComponentMasks.Earrings | AvatarComponentMasks.Ring | AvatarComponentMasks.Carryable);
		m_dirty = true;
	}

	public EditorAssetInfo ReplaceAsset(EditorAssetInfo newAsset)
	{
		EditorAssetInfo editorAssetInfo = new EditorAssetInfo();
		_ = newAsset.m_assetId;
		if (!(newAsset.m_assetId == Guid.Empty) && newAsset.m_assetType == BinaryAssetType.Unknown)
		{
			EditorAssetInfo assetType = GetAssetType(newAsset.m_assetId);
			newAsset.m_assetType = assetType.m_assetType;
			newAsset.m_assetDetails = assetType.m_assetDetails;
		}
		editorAssetInfo.m_removeEyeShadow = false;
		if (newAsset.CustomColorType < DynamicColorType.Count)
		{
			if (newAsset.CustomColorType == DynamicColorType.EyeShadow && m_Manifest.GetReplacementTexture(DynamicTextureType.EyeShadow).m_TextureAssetId == Guid.Empty)
			{
				editorAssetInfo.m_removeEyeShadow = true;
			}
			Vector4 dynamicColor = m_Manifest.GetDynamicColor(newAsset.CustomColorType);
			editorAssetInfo.SetAssetCustomColor(Utilities.ColorbFromVector4(dynamicColor), newAsset.CustomColorType);
			SetAvatarColor(newAsset.CustomColorType, newAsset.Color);
		}
		if (newAsset.m_removeEyeShadow)
		{
			AvatarManifestV1.ReplacementTexture texture = new AvatarManifestV1.ReplacementTexture
			{
				m_TextureAssetId = Guid.Empty,
				m_LinkedAssetId = Guid.Empty,
				m_Placement = 
				{
					m_Rotation = 0f,
					m_Scale = 1f,
					m_TranslationU = 0f,
					m_TranslationV = 0f
				}
			};
			m_Manifest.SetReplacementTexture(DynamicTextureType.EyeShadow, texture);
		}
		editorAssetInfo.m_assetType = newAsset.m_assetType;
		editorAssetInfo.m_assetDetails = newAsset.m_assetDetails;
		switch (newAsset.m_assetType)
		{
		case BinaryAssetType.Texture:
		{
			DynamicTextureType assetDetails2 = (DynamicTextureType)newAsset.m_assetDetails;
			AvatarManifestV1.ReplacementTexture texture2 = new AvatarManifestV1.ReplacementTexture
			{
				m_LinkedAssetId = Guid.Empty,
				m_TextureAssetId = newAsset.m_assetId,
				m_Placement = 
				{
					m_Rotation = 0f,
					m_Scale = 1f,
					m_TranslationU = 0f,
					m_TranslationV = 0f
				}
			};
			editorAssetInfo.m_assetId = m_Manifest.GetReplacementTexture(assetDetails2).m_TextureAssetId;
			m_Manifest.SetReplacementTexture(assetDetails2, texture2);
			if (assetDetails2 == DynamicTextureType.Eye && m_Manifest.GetReplacementTexture(DynamicTextureType.EyeShadow).m_TextureAssetId != Guid.Empty)
			{
				UpdateEyeShadow();
			}
			break;
		}
		case BinaryAssetType.Unknown:
		case BinaryAssetType.Animation:
			throw new AvatarException(Resources.InvalidAssetTypeText);
		case BinaryAssetType.Component:
		{
			ComponentInfo newComponent = new ComponentInfo
			{
				m_AssetId = newAsset.m_assetId,
				m_ComponentMask = (AvatarComponentMasks)newAsset.m_assetDetails
			};
			if (newAsset.m_useComponentColor)
			{
				newComponent.m_CustomColors0 = Utilities.ColorbFromVector4(newAsset.m_componentColor.CustomColor0);
				newComponent.m_CustomColors1 = Utilities.ColorbFromVector4(newAsset.m_componentColor.CustomColor1);
				newComponent.m_CustomColors2 = Utilities.ColorbFromVector4(newAsset.m_componentColor.CustomColor2);
			}
			editorAssetInfo.m_previousAssets = m_Manifest.GetComponents(newComponent.m_ComponentMask);
			if (editorAssetInfo.m_previousAssets.Count > 0)
			{
				ComponentInfo componentInfo = editorAssetInfo.m_previousAssets[0];
				editorAssetInfo.m_assetId = componentInfo.AssetId;
				editorAssetInfo.m_useComponentColor = true;
				editorAssetInfo.m_componentColor.CustomColor0 = Utilities.Vector4FromInt(componentInfo.CustomColor0.CompositeArgb);
				editorAssetInfo.m_componentColor.CustomColor1 = Utilities.Vector4FromInt(componentInfo.CustomColor1.CompositeArgb);
				editorAssetInfo.m_componentColor.CustomColor2 = Utilities.Vector4FromInt(componentInfo.CustomColor2.CompositeArgb);
			}
			else
			{
				editorAssetInfo.m_assetId = Guid.Empty;
			}
			if (newAsset.m_assetId == Guid.Empty)
			{
				m_Manifest.RemoveComponents(newComponent.m_ComponentMask);
			}
			else if (newAsset.m_previousAssets != null && newAsset.m_previousAssets.Count > 0)
			{
				AvatarComponentMasks avatarComponentMasks = AvatarComponentMasks.None;
				foreach (ComponentInfo previousAsset in newAsset.m_previousAssets)
				{
					avatarComponentMasks |= previousAsset.ComponentMask;
				}
				m_Manifest.UpdatePreviousComponents(avatarComponentMasks);
				m_Manifest.ClearComponents(avatarComponentMasks);
				foreach (ComponentInfo previousAsset2 in newAsset.m_previousAssets)
				{
					m_Manifest.m_ComponentInfo.Add(new ComponentDescription(previousAsset2, Guid.Empty));
				}
				m_Manifest.EquipRequiredComponents(fUseDefaultsIfNecessary: true);
			}
			else
			{
				m_Manifest.UpdatePreviousComponents(newComponent.ComponentMask);
				m_Manifest.ReplaceComponent(newComponent);
				m_Manifest.EquipRequiredComponents(fUseDefaultsIfNecessary: true);
			}
			break;
		}
		case BinaryAssetType.ShapeOverride:
		case BinaryAssetType.ShapeOverridePost:
		{
			BlendShapeType assetDetails = (BlendShapeType)newAsset.m_assetDetails;
			editorAssetInfo.m_assetId = m_Manifest.GetBlendShape(assetDetails);
			m_Manifest.SetBlendShape(assetDetails, newAsset.m_assetId);
			break;
		}
		}
		TryEquipAccessories(newAsset);
		m_Manifest.m_Dirty = true;
		m_dirty = true;
		return editorAssetInfo;
	}

	public EditorAssetInfo GetComponentInfo(EditorAssetInfo info)
	{
		EditorAssetInfo editorAssetInfo = new EditorAssetInfo();
		editorAssetInfo.m_assetType = info.m_assetType;
		editorAssetInfo.m_assetDetails = info.m_assetDetails;
		if (editorAssetInfo.m_assetType == BinaryAssetType.Unknown && info.m_assetId != Guid.Empty)
		{
			EditorAssetInfo assetType = GetAssetType(info.m_assetId);
			editorAssetInfo.m_assetType = assetType.m_assetType;
			editorAssetInfo.m_assetDetails = assetType.m_assetDetails;
		}
		editorAssetInfo.m_removeEyeShadow = false;
		if (info.CustomColorType < DynamicColorType.Count)
		{
			if (info.CustomColorType == DynamicColorType.EyeShadow && m_Manifest.GetReplacementTexture(DynamicTextureType.EyeShadow).m_TextureAssetId == Guid.Empty)
			{
				editorAssetInfo.m_removeEyeShadow = true;
			}
			Vector4 dynamicColor = m_Manifest.GetDynamicColor(info.CustomColorType);
			editorAssetInfo.SetAssetCustomColor(Utilities.ColorbFromVector4(dynamicColor), info.CustomColorType);
		}
		switch (editorAssetInfo.m_assetType)
		{
		case BinaryAssetType.Texture:
		{
			DynamicTextureType assetDetails2 = (DynamicTextureType)editorAssetInfo.m_assetDetails;
			editorAssetInfo.m_assetId = m_Manifest.GetReplacementTexture(assetDetails2).m_TextureAssetId;
			break;
		}
		case BinaryAssetType.Unknown:
		case BinaryAssetType.Animation:
			throw new AvatarException(Resources.InvalidAssetTypeText);
		case BinaryAssetType.Component:
		{
			AvatarComponentMasks mask = (AvatarComponentMasks)editorAssetInfo.m_assetDetails;
			editorAssetInfo.m_previousAssets = m_Manifest.GetComponents(mask);
			if (editorAssetInfo.m_previousAssets.Count > 0)
			{
				ComponentInfo componentInfo = editorAssetInfo.m_previousAssets[0];
				editorAssetInfo.m_assetId = componentInfo.AssetId;
				Colorb[] colors = new Colorb[3] { componentInfo.CustomColor0, componentInfo.CustomColor1, componentInfo.CustomColor2 };
				editorAssetInfo.m_componentColor = new ComponentColors(colors);
			}
			break;
		}
		case BinaryAssetType.ShapeOverride:
		case BinaryAssetType.ShapeOverridePost:
		{
			BlendShapeType assetDetails = (BlendShapeType)editorAssetInfo.m_assetDetails;
			editorAssetInfo.m_assetId = m_Manifest.GetBlendShape(assetDetails);
			break;
		}
		}
		return editorAssetInfo;
	}

	private EditorAssetInfo GetAssetType(Guid guid)
	{
		BlendShapeType blendShapeAssetType = AssetLoader.GetBlendShapeAssetType(guid);
		EditorAssetInfo editorAssetInfo = new EditorAssetInfo();
		if (blendShapeAssetType != BlendShapeType.Count)
		{
			editorAssetInfo.m_assetType = BinaryAssetType.ShapeOverride;
			editorAssetInfo.m_assetDetails = (uint)blendShapeAssetType;
			return editorAssetInfo;
		}
		DynamicTextureType textureAssetType = AssetLoader.GetTextureAssetType(guid);
		if (textureAssetType != DynamicTextureType.Count)
		{
			editorAssetInfo.m_assetType = BinaryAssetType.Texture;
			editorAssetInfo.m_assetDetails = (uint)textureAssetType;
			return editorAssetInfo;
		}
		ComponentCategories componentTypeFromAssetId = AssetLoader.GetComponentTypeFromAssetId(guid);
		if (componentTypeFromAssetId == ComponentCategories.Animation)
		{
			editorAssetInfo.m_assetType = BinaryAssetType.Animation;
			editorAssetInfo.m_assetDetails = 0u;
			return editorAssetInfo;
		}
		editorAssetInfo.m_assetType = BinaryAssetType.Component;
		editorAssetInfo.m_assetDetails = (uint)(componentTypeFromAssetId & ComponentCategories.Models);
		return editorAssetInfo;
	}

	protected void UpdateEyeShadow()
	{
		AvatarManifestV1.ReplacementTexture texture = new AvatarManifestV1.ReplacementTexture
		{
			m_LinkedAssetId = Guid.Empty
		};
		Guid textureAssetId = m_Manifest.GetReplacementTexture(DynamicTextureType.Eye).m_TextureAssetId;
		AvatarAssetDependency dependentAssets = AvatarAssetsDependenciesResolver.GetDependentAssets(m_dataManager, textureAssetId);
		texture.m_TextureAssetId = dependentAssets.m_DependentAssetId;
		if (texture.m_TextureAssetId == Guid.Empty)
		{
			throw new AvatarException("Failed to resolver eye shadow texture.");
		}
		texture.m_Placement.m_Rotation = 0f;
		texture.m_Placement.m_Scale = 1f;
		texture.m_Placement.m_TranslationU = 0f;
		texture.m_Placement.m_TranslationV = 0f;
		m_Manifest.SetReplacementTexture(DynamicTextureType.EyeShadow, texture);
	}

	protected void TryEquipAccessories(EditorAssetInfo info)
	{
		if (info.m_accessories == null || info.m_accessories.Count == 0)
		{
			return;
		}
		foreach (ComponentInfo accessory in info.m_accessories)
		{
			AvatarComponentMasks componentMask = accessory.ComponentMask;
			int num = m_Manifest.FindComponent(componentMask, AssetMatchingMode.Any);
			if (num == -1)
			{
				m_Manifest.SetComponentInfo(accessory);
			}
		}
	}

	public void SetOutfit(AvatarManifest manifest)
	{
		if (!(manifest is AvatarManifestV1 avatarManifestV))
		{
			return;
		}
		Debug.Assert(avatarManifestV.BodyComponentInfo.m_ComponentInfo.AssetId == m_Manifest.BodyComponentInfo.m_ComponentInfo.AssetId);
		StripNonOutfitComponents(m_Manifest);
		for (int i = 0; i < avatarManifestV.m_ComponentInfo.Count; i++)
		{
			ComponentInfo componentInfo = avatarManifestV.m_ComponentInfo[i].m_ComponentInfo;
			if (componentInfo.ComponentMask != AvatarComponentMasks.Hair)
			{
				m_Manifest.SetComponentInfo(componentInfo);
			}
		}
		m_Manifest.SetDynamicColor(DynamicColorType.EyeShadow, Utilities.ColorbFromVector4(avatarManifestV.GetDynamicColor(DynamicColorType.EyeShadow)));
		m_Manifest.SetDynamicColor(DynamicColorType.Mouth, Utilities.ColorbFromVector4(avatarManifestV.GetDynamicColor(DynamicColorType.Mouth)));
		AvatarManifestV1.ReplacementTexture replacementTexture = avatarManifestV.GetReplacementTexture(DynamicTextureType.EyeShadow);
		if (replacementTexture.m_TextureAssetId != Guid.Empty)
		{
			replacementTexture.m_TextureAssetId = m_Manifest.GetReplacementTexture(DynamicTextureType.Eye).m_LinkedAssetId;
			m_Manifest.SetReplacementTexture(DynamicTextureType.EyeShadow, replacementTexture);
		}
		else
		{
			AvatarManifestV1.ReplacementTexture texture = new AvatarManifestV1.ReplacementTexture
			{
				m_TextureAssetId = Guid.Empty
			};
			m_Manifest.SetReplacementTexture(DynamicTextureType.EyeShadow, texture);
		}
	}

	public static void StripNonOutfitComponents(AvatarManifest manifest)
	{
		if (manifest is AvatarManifestV1 avatarManifestV)
		{
			avatarManifestV.ClearComponents(AvatarComponentMasks.Head | AvatarComponentMasks.Body | AvatarComponentMasks.Shirt | AvatarComponentMasks.Trousers | AvatarComponentMasks.Shoes | AvatarComponentMasks.Hat | AvatarComponentMasks.Gloves | AvatarComponentMasks.Glasses | AvatarComponentMasks.Wristwear | AvatarComponentMasks.Earrings | AvatarComponentMasks.Ring | AvatarComponentMasks.Carryable);
		}
	}

	public void RemoveComponents(RemovableComponents mask)
	{
		AvatarManifestV1.ReplacementTexture texture = default(AvatarManifestV1.ReplacementTexture);
		switch (mask)
		{
		case RemovableComponents.Earrings:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Earrings);
			break;
		case RemovableComponents.Glasses:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Glasses);
			break;
		case RemovableComponents.Gloves:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Gloves);
			break;
		case RemovableComponents.Hat:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Hat);
			break;
		case RemovableComponents.Ring:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Ring);
			break;
		case RemovableComponents.Wristwear:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Wristwear);
			break;
		case RemovableComponents.FacialHair:
			m_Manifest.SetReplacementTexture(DynamicTextureType.FacialHair, texture);
			break;
		case RemovableComponents.EyeShadow:
			m_Manifest.SetReplacementTexture(DynamicTextureType.EyeShadow, texture);
			break;
		case RemovableComponents.SkinFeatures:
			m_Manifest.SetReplacementTexture(DynamicTextureType.SkinFeatures, texture);
			break;
		case RemovableComponents.Prop:
			m_Manifest.RemoveComponents(AvatarComponentMasks.Carryable);
			break;
		default:
			throw new InvalidOperationException("Shouldn't get here");
		}
	}

	public bool IsAssetPresent(Guid assetId)
	{
		EditorAssetInfo assetType = GetAssetType(assetId);
		switch (assetType.m_assetType)
		{
		case BinaryAssetType.Component:
			return m_Manifest.IsAssetPresent(assetId);
		case BinaryAssetType.Texture:
		{
			DynamicTextureType assetDetails2 = (DynamicTextureType)assetType.m_assetDetails;
			return m_Manifest.GetReplacementTexture(assetDetails2).m_TextureAssetId == assetId;
		}
		case BinaryAssetType.ShapeOverride:
		{
			BlendShapeType assetDetails = (BlendShapeType)assetType.m_assetDetails;
			return m_Manifest.GetBlendShape(assetDetails) == assetId;
		}
		default:
			return false;
		}
	}

	public Guid GetComponentGuid(string avatarComponentName)
	{
		try
		{
			AvatarComponentMasks mask = (AvatarComponentMasks)Enum.Parse(typeof(AvatarComponentMasks), avatarComponentName, ignoreCase: true);
			return m_Manifest.GetComponentInfo(mask, AssetMatchingMode.All).AssetId;
		}
		catch (ArgumentException)
		{
		}
		try
		{
			DynamicTextureType type = (DynamicTextureType)Enum.Parse(typeof(DynamicTextureType), avatarComponentName, ignoreCase: true);
			return m_Manifest.GetReplacementTexture(type).m_TextureAssetId;
		}
		catch (ArgumentException)
		{
		}
		BlendShapeType type2 = (BlendShapeType)Enum.Parse(typeof(BlendShapeType), avatarComponentName, ignoreCase: true);
		return m_Manifest.GetBlendShape(type2);
	}
}
