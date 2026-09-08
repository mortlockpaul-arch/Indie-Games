#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class AvatarManifestV1 : AvatarManifest
{
	public struct TexturePlacement
	{
		public float m_Scale;

		public float m_Rotation;

		public float m_TranslationU;

		public float m_TranslationV;
	}

	public struct ReplacementTexture
	{
		public Guid m_TextureAssetId;

		public Guid m_LinkedAssetId;

		public TexturePlacement m_Placement;
	}

	public struct BlendShape
	{
		public Guid m_BlendShapeAssetId;
	}

	public static readonly AvatarComponentMasks[] s_RequiredComponents = new AvatarComponentMasks[4]
	{
		AvatarComponentMasks.Shoes,
		AvatarComponentMasks.Trousers,
		AvatarComponentMasks.Shirt,
		AvatarComponentMasks.Hair
	};

	public static readonly DynamicTextureType[] s_RequiredTextures = new DynamicTextureType[3]
	{
		DynamicTextureType.Mouth,
		DynamicTextureType.Eye,
		DynamicTextureType.Eyebrow
	};

	public static readonly BlendShapeType[] s_RequiredBlendShapes = new BlendShapeType[3]
	{
		BlendShapeType.Chin,
		BlendShapeType.Nose,
		BlendShapeType.Ear
	};

	public static readonly ComponentInfo[,] s_DefaultRequiredComponents = new ComponentInfo[2, 4]
	{
		{
			new ComponentInfo(AvatarAssetsPack.GrungeTrainers, AvatarComponentMasks.Shoes, new Colorb(0), new Colorb(0), new Colorb(0)),
			new ComponentInfo(AvatarAssetsPack.Jeans, AvatarComponentMasks.Trousers, new Colorb(0), new Colorb(0), new Colorb(0)),
			new ComponentInfo(AvatarAssetsPack.MalePowerTee, AvatarComponentMasks.Shirt, new Colorb(148, 214, 20), new Colorb(0), new Colorb(0)),
			new ComponentInfo(AvatarAssetsPack.ShortAndSpikey, AvatarComponentMasks.Hair, new Colorb(0), new Colorb(0), new Colorb(0))
		},
		{
			new ComponentInfo(AvatarAssetsPack.Sandals, AvatarComponentMasks.Shoes, new Colorb(0), new Colorb(0), new Colorb(0)),
			new ComponentInfo(AvatarAssetsPack.PolkadotSkirt, AvatarComponentMasks.Trousers, new Colorb(0), new Colorb(0), new Colorb(0)),
			new ComponentInfo(AvatarAssetsPack.TeeWithBelt, AvatarComponentMasks.Shirt, new Colorb(0), new Colorb(0), new Colorb(0)),
			new ComponentInfo(AvatarAssetsPack.EvenLength, AvatarComponentMasks.Hair, new Colorb(0), new Colorb(0), new Colorb(0))
		}
	};

	public static readonly List<Guid> s_CoreAssets = new List<Guid>(new Guid[10]
	{
		AvatarAssetsPack.GrungeTrainers,
		AvatarAssetsPack.Jeans,
		AvatarAssetsPack.MalePowerTee,
		AvatarAssetsPack.ShortAndSpikey,
		AvatarAssetsPack.Sandals,
		AvatarAssetsPack.PolkadotSkirt,
		AvatarAssetsPack.TeeWithBelt,
		AvatarAssetsPack.EvenLength,
		AvatarAssetsPack.FemaleBody,
		AvatarAssetsPack.MaleBody
	});

	public static readonly byte[] s_TocAssetVersionId = new byte[6] { 241, 9, 161, 156, 178, 224 };

	public float m_WeightFactor;

	public float m_HeightFactor;

	public byte[] m_OwnerXuid;

	public byte[] m_ConsoleId;

	public BlendShape[] m_BlendShapes;

	public ReplacementTexture[] m_ReplacementTextures;

	public Colorb[] m_DynamicColors;

	public ComponentDescription m_BodyComponentInfo;

	public ComponentDescription m_HeadComponentInfo;

	public bool m_Dirty;

	public List<ComponentDescription> m_ComponentInfo = new List<ComponentDescription>();

	public ComponentInfo[] m_PreviousRequiredComponentInfo;

	public bool DressDefaultClothes { get; set; }

	public ComponentDescription BodyComponentInfo => m_BodyComponentInfo;

	public ComponentDescription HeadComponentInfo => m_HeadComponentInfo;

	public float HeightFactor
	{
		get
		{
			return m_HeightFactor;
		}
		set
		{
			m_HeightFactor = value;
		}
	}

	public float WidthFactor
	{
		get
		{
			return m_WeightFactor;
		}
		set
		{
			m_WeightFactor = value;
		}
	}

	public override AvatarGender BodyType
	{
		get
		{
			Guid assetId = m_BodyComponentInfo.m_ComponentInfo.m_AssetId;
			if (assetId == AvatarAssetsPack.MaleBody)
			{
				return AvatarGender.Male;
			}
			if (assetId == AvatarAssetsPack.FemaleBody)
			{
				return AvatarGender.Female;
			}
			return AvatarGender.Unknown;
		}
	}

	public AvatarManifestV1()
	{
		m_ConsoleId = new byte[5];
		m_OwnerXuid = new byte[8];
		m_BlendShapes = new BlendShape[3];
		m_ReplacementTextures = new ReplacementTexture[6];
		m_DynamicColors = new Colorb[9];
		m_PreviousRequiredComponentInfo = new ComponentInfo[4];
		m_VersionNumber = 1;
		DressDefaultClothes = true;
	}

	public override AvatarManifest Clone()
	{
		AvatarManifestV1 avatarManifestV = new AvatarManifestV1();
		avatarManifestV.m_WeightFactor = m_WeightFactor;
		avatarManifestV.m_HeightFactor = m_HeightFactor;
		avatarManifestV.DressDefaultClothes = DressDefaultClothes;
		avatarManifestV.m_VersionNumber = m_VersionNumber;
		avatarManifestV.m_DynamicColors = m_DynamicColors.Clone() as Colorb[];
		avatarManifestV.m_ReplacementTextures = m_ReplacementTextures.Clone() as ReplacementTexture[];
		avatarManifestV.m_PreviousRequiredComponentInfo = m_PreviousRequiredComponentInfo.Clone() as ComponentInfo[];
		avatarManifestV.m_HeadComponentInfo = new ComponentDescription();
		avatarManifestV.m_HeadComponentInfo.m_ComponentInfo = m_HeadComponentInfo.m_ComponentInfo;
		avatarManifestV.m_HeadComponentInfo.m_OverrideAsset = m_HeadComponentInfo.m_OverrideAsset;
		avatarManifestV.m_ComponentInfo = new List<ComponentDescription>();
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			ComponentDescription componentDescription = new ComponentDescription();
			componentDescription.m_ComponentInfo = m_ComponentInfo[i].m_ComponentInfo;
			componentDescription.m_OverrideAsset = m_ComponentInfo[i].m_OverrideAsset;
			avatarManifestV.m_ComponentInfo.Add(componentDescription);
		}
		avatarManifestV.m_BodyComponentInfo = new ComponentDescription();
		avatarManifestV.m_BodyComponentInfo.m_ComponentInfo = m_BodyComponentInfo.m_ComponentInfo;
		avatarManifestV.m_BodyComponentInfo.m_OverrideAsset = m_BodyComponentInfo.m_OverrideAsset;
		avatarManifestV.m_BlendShapes = m_BlendShapes.Clone() as BlendShape[];
		avatarManifestV.m_Dirty = m_Dirty;
		avatarManifestV.m_OwnerXuid = m_OwnerXuid;
		avatarManifestV.m_ConsoleId = m_ConsoleId;
		return avatarManifestV;
	}

	public static bool ValidateAssetId(Guid guid)
	{
		if (AssetLoader.GetAssetGuidType(guid) == AssetGuidType.TOC)
		{
			byte[] array = guid.ToByteArray();
			for (int i = 0; i < 6; i++)
			{
				if (s_TocAssetVersionId[i] != array[i + 10])
				{
					return false;
				}
			}
		}
		return true;
	}

	public void UpdatePreviousComponents(AvatarComponentMasks mask)
	{
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			if ((m_ComponentInfo[i].m_ComponentInfo.ComponentMask & mask) <= AvatarComponentMasks.None)
			{
				continue;
			}
			for (uint num = 0u; num < s_RequiredComponents.Length; num++)
			{
				if (m_ComponentInfo[i].m_ComponentInfo.ComponentMask == s_RequiredComponents[num])
				{
					ref ComponentInfo reference = ref m_PreviousRequiredComponentInfo[num];
					reference = m_ComponentInfo[i].m_ComponentInfo;
					break;
				}
			}
		}
	}

	public override void ReplaceComponent(ComponentInfo newComponent)
	{
		Debug.Assert(m_PreviousRequiredComponentInfo.Length == s_RequiredComponents.Length);
		ClearComponents(newComponent.m_ComponentMask);
		ComponentDescription item = new ComponentDescription(newComponent, Guid.Empty);
		m_ComponentInfo.Add(item);
		m_Dirty = true;
	}

	public Vector4 GetDynamicColor(DynamicColorType type)
	{
		return Utilities.ColorbToVector4(m_DynamicColors[(int)type]);
	}

	public void SetDynamicColor(DynamicColorType type, Colorb color)
	{
		m_DynamicColors[(int)type] = color;
	}

	public bool GetRequiredComponentsPresent()
	{
		AvatarComponentMasks combinedComponentMask = GetCombinedComponentMask();
		for (int i = 0; i < s_RequiredComponents.Length; i++)
		{
			if ((combinedComponentMask & s_RequiredComponents[i]) == 0)
			{
				return false;
			}
		}
		return true;
	}

	public override bool RemoveComponents(AvatarComponentMasks mask)
	{
		bool flag = true;
		AvatarGender bodyType = BodyType;
		int num = ((bodyType != AvatarGender.Male) ? 1 : 0);
		Debug.Assert(bodyType != AvatarGender.Unknown);
		if (bodyType == AvatarGender.Unknown)
		{
			return false;
		}
		m_Dirty = true;
		bool flag2;
		for (uint num2 = 0u; num2 < s_RequiredComponents.Length; num2++)
		{
			if ((mask & s_RequiredComponents[num2]) == 0)
			{
				continue;
			}
			ComponentInfo componentInfo = GetComponentInfo(s_RequiredComponents[num2], AssetMatchingMode.Exact);
			if (!componentInfo.IsEmpty)
			{
				flag2 = componentInfo.m_AssetId != s_DefaultRequiredComponents[num, (int)checked((nint)unchecked((long)num2))].m_AssetId && SetComponentInfo(s_DefaultRequiredComponents[num, (int)checked((nint)unchecked((long)num2))]);
				if (flag && !flag2)
				{
					flag = flag2;
				}
			}
		}
		ClearComponents(mask);
		flag2 = EquipRequiredComponents(fUseDefaultsIfNecessary: false);
		if (flag && !flag2)
		{
			flag = flag2;
		}
		return flag;
	}

	public bool GetRequiredBlendShapesPresent()
	{
		for (int i = 0; i < s_RequiredBlendShapes.Length; i++)
		{
			if (Guid.Empty == m_BlendShapes[i].m_BlendShapeAssetId)
			{
				return false;
			}
		}
		return true;
	}

	public bool GetRequiredReplacementTexturesPresent()
	{
		for (int i = 0; i < s_RequiredTextures.Length; i++)
		{
			if (Guid.Empty == m_ReplacementTextures[(int)s_RequiredTextures[i]].m_TextureAssetId)
			{
				return false;
			}
		}
		return true;
	}

	public ReplacementTexture GetReplacementTexture(DynamicTextureType type)
	{
		return m_ReplacementTextures[(int)type];
	}

	public void SetReplacementTexture(DynamicTextureType type, ReplacementTexture texture)
	{
		m_ReplacementTextures[(int)type] = texture;
	}

	public Guid GetBlendShape(BlendShapeType type)
	{
		return m_BlendShapes[(int)type].m_BlendShapeAssetId;
	}

	public void SetBlendShape(BlendShapeType type, Guid asset)
	{
		m_BlendShapes[(int)type].m_BlendShapeAssetId = asset;
	}

	public ComponentInfo GetComponentInfo(AvatarComponentMasks mask, AssetMatchingMode matchingMode)
	{
		int componentInfoCount = GetComponentInfoCount();
		switch (matchingMode)
		{
		case AssetMatchingMode.Exact:
		{
			for (int k = 0; k < componentInfoCount; k++)
			{
				if (m_ComponentInfo[k].m_ComponentInfo.m_ComponentMask == mask)
				{
					return m_ComponentInfo[k].m_ComponentInfo;
				}
			}
			return ComponentInfo.Zero;
		}
		case AssetMatchingMode.Any:
		{
			for (int j = 0; j < componentInfoCount; j++)
			{
				if ((m_ComponentInfo[j].m_ComponentInfo.m_ComponentMask & mask) != AvatarComponentMasks.None)
				{
					return m_ComponentInfo[j].m_ComponentInfo;
				}
			}
			return ComponentInfo.Zero;
		}
		case AssetMatchingMode.All:
		{
			for (int i = 0; i < componentInfoCount; i++)
			{
				if ((m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask & mask) == mask)
				{
					return m_ComponentInfo[i].m_ComponentInfo;
				}
			}
			return ComponentInfo.Zero;
		}
		default:
			return ComponentInfo.Zero;
		}
	}

	public int FindComponent(AvatarComponentMasks mask, AssetMatchingMode matchingMode)
	{
		int componentInfoCount = GetComponentInfoCount();
		switch (matchingMode)
		{
		case AssetMatchingMode.Exact:
		{
			for (int k = 0; k < componentInfoCount; k++)
			{
				if (m_ComponentInfo[k].m_ComponentInfo.m_ComponentMask == mask)
				{
					return k;
				}
			}
			return -1;
		}
		case AssetMatchingMode.Any:
		{
			for (int j = 0; j < componentInfoCount; j++)
			{
				if ((m_ComponentInfo[j].m_ComponentInfo.m_ComponentMask & mask) != AvatarComponentMasks.None)
				{
					return j;
				}
			}
			return -1;
		}
		case AssetMatchingMode.All:
		{
			for (int i = 0; i < componentInfoCount; i++)
			{
				if ((m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask & mask) == mask)
				{
					return i;
				}
			}
			return -1;
		}
		default:
			return -1;
		}
	}

	public int GetComponentInfoCount()
	{
		return m_ComponentInfo.Count;
	}

	public bool SetComponentInfo(ComponentInfo info)
	{
		m_Dirty = true;
		Debug.Assert(m_PreviousRequiredComponentInfo.Length == s_RequiredComponents.Length);
		for (uint num = 0u; num < s_RequiredComponents.Length; num++)
		{
			if (info.m_ComponentMask == s_RequiredComponents[num])
			{
				m_PreviousRequiredComponentInfo[num] = info;
				break;
			}
		}
		ClearComponents(info.m_ComponentMask);
		ComponentDescription item = new ComponentDescription(info, Guid.Empty);
		m_ComponentInfo.Add(item);
		return EquipRequiredComponents(fUseDefaultsIfNecessary: false);
	}

	public void ClearComponents(AvatarComponentMasks mask)
	{
		int num = 0;
		while (num < m_ComponentInfo.Count)
		{
			if ((m_ComponentInfo[num].m_ComponentInfo.m_ComponentMask & mask) != AvatarComponentMasks.None)
			{
				m_ComponentInfo.RemoveAt(num);
				m_Dirty = true;
			}
			else
			{
				num++;
			}
		}
	}

	public override List<ComponentInfo> GetComponents(AvatarComponentMasks mask)
	{
		List<ComponentInfo> list = new List<ComponentInfo>();
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			if ((mask & m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask) != AvatarComponentMasks.None)
			{
				list.Add(m_ComponentInfo[i].m_ComponentInfo);
			}
		}
		return list;
	}

	public bool EquipRequiredComponents(bool fUseDefaultsIfNecessary)
	{
		AvatarGender bodyType = BodyType;
		int num = ((bodyType != AvatarGender.Male) ? 1 : 0);
		Debug.Assert(bodyType != AvatarGender.Unknown);
		if (bodyType == AvatarGender.Unknown)
		{
			return false;
		}
		AvatarComponentMasks combinedComponentMask = GetCombinedComponentMask();
		for (int i = 0; i < s_RequiredComponents.Length; i++)
		{
			if ((s_RequiredComponents[i] & combinedComponentMask) == 0)
			{
				ComponentDescription componentDescription = new ComponentDescription(m_PreviousRequiredComponentInfo[i], Guid.Empty);
				if (fUseDefaultsIfNecessary && componentDescription.m_ComponentInfo.m_AssetId == Guid.Empty)
				{
					componentDescription.m_ComponentInfo = s_DefaultRequiredComponents[num, i];
				}
				if (componentDescription.m_ComponentInfo.AssetId != Guid.Empty)
				{
					m_ComponentInfo.Add(componentDescription);
				}
				m_Dirty = true;
			}
		}
		return true;
	}

	public bool ReplaceMissingComponents(bool fUseDefaultsIfNecessary)
	{
		AvatarGender bodyType = BodyType;
		int num = ((bodyType != AvatarGender.Male) ? 1 : 0);
		if (bodyType == AvatarGender.Unknown)
		{
			return false;
		}
		AvatarComponentMasks combinedComponentMask = GetCombinedComponentMask();
		for (int i = 0; i < s_RequiredComponents.Length; i++)
		{
			if ((s_RequiredComponents[i] & combinedComponentMask) == 0)
			{
				ComponentDescription componentDescription = new ComponentDescription(m_PreviousRequiredComponentInfo[i], Guid.Empty);
				m_PreviousRequiredComponentInfo[i] = default(ComponentInfo);
				if (fUseDefaultsIfNecessary && componentDescription.m_ComponentInfo.m_AssetId == Guid.Empty)
				{
					componentDescription.m_ComponentInfo = s_DefaultRequiredComponents[num, i];
				}
				if (componentDescription.m_ComponentInfo.AssetId != Guid.Empty)
				{
					m_ComponentInfo.Add(componentDescription);
				}
				m_Dirty = true;
			}
		}
		return true;
	}

	public AvatarComponentMasks GetCombinedComponentMask()
	{
		AvatarComponentMasks avatarComponentMasks = AvatarComponentMasks.None;
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			avatarComponentMasks |= m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask;
		}
		return avatarComponentMasks;
	}

	public AvatarComponentMasks GetUserCombinedComponentMask(AvatarComponentMasks componentMask)
	{
		AvatarComponentMasks avatarComponentMasks = AvatarComponentMasks.None;
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			if ((m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask & componentMask) != AvatarComponentMasks.None)
			{
				avatarComponentMasks |= m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask;
			}
		}
		return avatarComponentMasks;
	}

	public override bool IsComponentPresent(AvatarComponentType componentId)
	{
		AvatarComponentMasks avatarComponentMasks = (AvatarComponentMasks)(1 << (int)componentId);
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			if ((m_ComponentInfo[i].m_ComponentInfo.m_ComponentMask & avatarComponentMasks) != AvatarComponentMasks.None)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAssetPresent(Guid asset)
	{
		if (m_BodyComponentInfo.m_ComponentInfo.m_AssetId == asset)
		{
			return true;
		}
		if (m_HeadComponentInfo.m_ComponentInfo.m_AssetId == asset)
		{
			return true;
		}
		int count = m_ComponentInfo.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_ComponentInfo[i].m_ComponentInfo.m_AssetId == asset)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCoreAsset(Guid asset)
	{
		return s_CoreAssets.Contains(asset);
	}

	public void RemoveAsset(Guid asset)
	{
		if (m_BodyComponentInfo.m_ComponentInfo.m_AssetId == asset)
		{
			m_BodyComponentInfo.m_ComponentInfo = default(ComponentInfo);
			m_Dirty = true;
		}
		if (m_HeadComponentInfo.m_ComponentInfo.m_AssetId == asset)
		{
			m_HeadComponentInfo.m_ComponentInfo = default(ComponentInfo);
			m_Dirty = true;
		}
		int count = m_ComponentInfo.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_ComponentInfo[i].m_ComponentInfo.m_AssetId == asset)
			{
				m_Dirty = true;
				m_ComponentInfo.RemoveAt(i);
				break;
			}
		}
		count = m_PreviousRequiredComponentInfo.Length;
		for (int j = 0; j < count; j++)
		{
			if (m_PreviousRequiredComponentInfo[j].m_AssetId == asset)
			{
				m_PreviousRequiredComponentInfo[j] = default(ComponentInfo);
				break;
			}
		}
		count = m_BlendShapes.Length;
		for (int k = 0; k < count; k++)
		{
			if (m_BlendShapes[k].m_BlendShapeAssetId == asset)
			{
				m_Dirty = true;
				m_BlendShapes[k] = default(BlendShape);
				break;
			}
		}
		count = m_ReplacementTextures.Length;
		for (int l = 0; l < count; l++)
		{
			if (m_ReplacementTextures[l].m_TextureAssetId == asset)
			{
				m_Dirty = true;
				m_ReplacementTextures[l] = default(ReplacementTexture);
				break;
			}
		}
	}

	public Guid ResolveDependencies(IDataManager downloadManager, Guid assetId)
	{
		AvatarAssetDependency dependentAssets = AvatarAssetsDependenciesResolver.GetDependentAssets(downloadManager, assetId);
		if (dependentAssets != null)
		{
			if (dependentAssets.m_ModifiedAssetList == null)
			{
				return dependentAssets.m_DependentAssetId;
			}
			int num = dependentAssets.m_ModifiedAssetList.Length;
			for (int i = 0; i < num; i++)
			{
				if (IsAssetPresent(dependentAssets.m_ModifiedAssetList[i]))
				{
					return dependentAssets.m_DependentAssetId;
				}
			}
		}
		return Guid.Empty;
	}

	public override void UpdateDependencies(IDataManager downloadManager)
	{
		if (downloadManager == null)
		{
			throw new ArgumentNullException("dataManager");
		}
		m_BodyComponentInfo.m_OverrideAsset = ResolveDependencies(downloadManager, m_BodyComponentInfo.m_ComponentInfo.AssetId);
		m_HeadComponentInfo.m_OverrideAsset = ResolveDependencies(downloadManager, m_HeadComponentInfo.m_ComponentInfo.AssetId);
		int count = m_ComponentInfo.Count;
		for (int i = 0; i < count; i++)
		{
			m_ComponentInfo[i].m_OverrideAsset = ResolveDependencies(downloadManager, m_ComponentInfo[i].m_ComponentInfo.AssetId);
		}
		int num = m_ReplacementTextures.Length;
		for (int j = 0; j < num; j++)
		{
			m_ReplacementTextures[j].m_LinkedAssetId = ResolveDependencies(downloadManager, m_ReplacementTextures[j].m_TextureAssetId);
		}
	}

	public bool Validate()
	{
		AvatarGender bodyType = BodyType;
		if (bodyType == AvatarGender.Female || bodyType == AvatarGender.Male)
		{
			return true;
		}
		return false;
	}

	public static bool operator ==(AvatarManifestV1 a1, AvatarManifestV1 b1)
	{
		if ((object)a1 == b1)
		{
			return true;
		}
		if (a1.Version != b1.Version)
		{
			return false;
		}
		if (a1.m_WeightFactor != b1.m_WeightFactor)
		{
			return false;
		}
		if (a1.m_HeightFactor != b1.m_HeightFactor)
		{
			return false;
		}
		if (a1.m_DynamicColors.Length != b1.m_DynamicColors.Length)
		{
			return false;
		}
		for (int i = 0; i < a1.m_DynamicColors.Length; i++)
		{
			if (!(a1.m_DynamicColors[i] == b1.m_DynamicColors[i]))
			{
				return false;
			}
		}
		if (a1.m_ReplacementTextures.Length != b1.m_ReplacementTextures.Length)
		{
			return false;
		}
		for (int j = 0; j < a1.m_ReplacementTextures.Length; j++)
		{
			if (a1.m_ReplacementTextures[j].m_LinkedAssetId != b1.m_ReplacementTextures[j].m_LinkedAssetId)
			{
				return false;
			}
			float num = 1E-06f;
			if (Math.Abs(a1.m_ReplacementTextures[j].m_Placement.m_Rotation - b1.m_ReplacementTextures[j].m_Placement.m_Rotation) > num)
			{
				return false;
			}
			if (Math.Abs(a1.m_ReplacementTextures[j].m_Placement.m_Scale - b1.m_ReplacementTextures[j].m_Placement.m_Scale) > num)
			{
				return false;
			}
			if (Math.Abs(a1.m_ReplacementTextures[j].m_Placement.m_TranslationU - b1.m_ReplacementTextures[j].m_Placement.m_TranslationU) > num)
			{
				return false;
			}
			if (Math.Abs(a1.m_ReplacementTextures[j].m_Placement.m_TranslationV - b1.m_ReplacementTextures[j].m_Placement.m_TranslationV) > num)
			{
				return false;
			}
			if (a1.m_ReplacementTextures[j].m_TextureAssetId != b1.m_ReplacementTextures[j].m_TextureAssetId)
			{
				return false;
			}
		}
		if (a1.m_PreviousRequiredComponentInfo.Length != b1.m_PreviousRequiredComponentInfo.Length)
		{
			return false;
		}
		for (int k = 0; k < a1.m_PreviousRequiredComponentInfo.Length; k++)
		{
			if (a1.m_PreviousRequiredComponentInfo[k] != b1.m_PreviousRequiredComponentInfo[k])
			{
				return false;
			}
		}
		if (a1.m_HeadComponentInfo != b1.m_HeadComponentInfo)
		{
			return false;
		}
		if (a1.m_BodyComponentInfo != b1.m_BodyComponentInfo)
		{
			return false;
		}
		if (a1.m_ComponentInfo.Count != b1.m_ComponentInfo.Count)
		{
			return false;
		}
		for (int l = 0; l < b1.m_ComponentInfo.Count; l++)
		{
			int m;
			for (m = 0; m < b1.m_ComponentInfo.Count && !(a1.m_ComponentInfo[m] == b1.m_ComponentInfo[l]); m++)
			{
			}
			if (m == b1.m_ComponentInfo.Count)
			{
				return false;
			}
		}
		if (a1.m_BlendShapes.Length != b1.m_BlendShapes.Length)
		{
			return false;
		}
		for (int n = 0; n < a1.m_BlendShapes.Length; n++)
		{
			if (a1.m_BlendShapes[n].m_BlendShapeAssetId != b1.m_BlendShapes[n].m_BlendShapeAssetId)
			{
				return false;
			}
		}
		return true;
	}

	public static bool operator !=(AvatarManifestV1 a, AvatarManifestV1 b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AvatarManifestV1))
		{
			return false;
		}
		AvatarManifestV1 avatarManifestV = (AvatarManifestV1)obj;
		return this == avatarManifestV;
	}

	public override int GetHashCode()
	{
		int num = m_OwnerXuid.GetHashCode() + m_HeadComponentInfo.GetHashCode() + m_BodyComponentInfo.GetHashCode();
		for (int i = 0; i < m_ComponentInfo.Count; i++)
		{
			num += m_ComponentInfo[i].GetHashCode();
		}
		for (int j = 0; j < m_PreviousRequiredComponentInfo.Length; j++)
		{
			num += m_PreviousRequiredComponentInfo[j].GetHashCode();
		}
		for (int k = 0; k < m_ReplacementTextures.Length; k++)
		{
			num += m_ReplacementTextures[k].GetHashCode();
		}
		for (int l = 0; l < m_DynamicColors.Length; l++)
		{
			num += m_DynamicColors[l].GetHashCode();
		}
		for (int m = 0; m < m_BlendShapes.Length; m++)
		{
			num += m_BlendShapes[m].m_BlendShapeAssetId.GetHashCode();
		}
		return num + ((int)(m_WeightFactor * 32f) + (int)(m_HeightFactor * 32f));
	}

	public static bool LoadBlendShape(EndianStream stream, out BlendShape blendShape)
	{
		blendShape.m_BlendShapeAssetId = stream.ReadGuid();
		if (!ValidateAssetId(blendShape.m_BlendShapeAssetId))
		{
			return false;
		}
		return true;
	}

	public static bool LoadReplacementTexture(EndianStream stream, out ReplacementTexture replacementTexture)
	{
		replacementTexture = default(ReplacementTexture);
		replacementTexture.m_TextureAssetId = stream.ReadGuid();
		if (!ValidateAssetId(replacementTexture.m_TextureAssetId))
		{
			return false;
		}
		replacementTexture.m_Placement.m_Scale = stream.ReadFloat();
		replacementTexture.m_Placement.m_Rotation = stream.ReadFloat();
		replacementTexture.m_Placement.m_TranslationU = stream.ReadFloat();
		replacementTexture.m_Placement.m_TranslationV = stream.ReadFloat();
		return true;
	}

	public static bool LoadComponentInfo(EndianStream stream, out ComponentInfo componentInfo)
	{
		componentInfo = default(ComponentInfo);
		componentInfo.m_AssetId = stream.ReadGuid();
		if (!ValidateAssetId(componentInfo.m_AssetId))
		{
			return false;
		}
		componentInfo.m_ComponentMask = (AvatarComponentMasks)stream.ReadShort();
		stream.ReadShort();
		byte alpha = (byte)stream.ReadByte();
		byte red = (byte)stream.ReadByte();
		byte green = (byte)stream.ReadByte();
		byte blue = (byte)stream.ReadByte();
		componentInfo.m_CustomColors0 = new Colorb(red, green, blue, alpha);
		alpha = (byte)stream.ReadByte();
		red = (byte)stream.ReadByte();
		green = (byte)stream.ReadByte();
		blue = (byte)stream.ReadByte();
		componentInfo.m_CustomColors1 = new Colorb(red, green, blue, alpha);
		alpha = (byte)stream.ReadByte();
		red = (byte)stream.ReadByte();
		green = (byte)stream.ReadByte();
		blue = (byte)stream.ReadByte();
		componentInfo.m_CustomColors2 = new Colorb(red, green, blue, alpha);
		return true;
	}

	public bool InitFromBinary(EndianStream stream)
	{
		m_WeightFactor = stream.ReadFloat();
		m_HeightFactor = stream.ReadFloat();
		for (int i = 0; i < m_BlendShapes.Length; i++)
		{
			if (!LoadBlendShape(stream, out m_BlendShapes[i]))
			{
				return false;
			}
		}
		for (int j = 0; j < m_ReplacementTextures.Length; j++)
		{
			if (!LoadReplacementTexture(stream, out m_ReplacementTextures[j]))
			{
				return false;
			}
		}
		for (int k = 0; k < m_DynamicColors.Length; k++)
		{
			byte alpha = (byte)stream.ReadByte();
			byte red = (byte)stream.ReadByte();
			byte green = (byte)stream.ReadByte();
			byte blue = (byte)stream.ReadByte();
			ref Colorb reference = ref m_DynamicColors[k];
			reference = new Colorb(red, green, blue, alpha);
		}
		if (!LoadComponentInfo(stream, out var componentInfo))
		{
			return false;
		}
		if (componentInfo.ComponentMask != AvatarComponentMasks.Body)
		{
			return false;
		}
		if (componentInfo.AssetId == Guid.Empty)
		{
			return false;
		}
		m_BodyComponentInfo = new ComponentDescription(componentInfo, Guid.Empty);
		if (!LoadComponentInfo(stream, out componentInfo))
		{
			return false;
		}
		if (componentInfo.ComponentMask != AvatarComponentMasks.Head)
		{
			return false;
		}
		if (componentInfo.AssetId == Guid.Empty)
		{
			return false;
		}
		m_HeadComponentInfo = new ComponentDescription(componentInfo, Guid.Empty);
		for (int l = 0; l < 13; l++)
		{
			if (!LoadComponentInfo(stream, out componentInfo))
			{
				return false;
			}
			if (componentInfo.m_AssetId != Guid.Empty)
			{
				m_ComponentInfo.Add(new ComponentDescription(componentInfo, Guid.Empty));
			}
		}
		for (int m = 0; m < m_PreviousRequiredComponentInfo.Length; m++)
		{
			if (!LoadComponentInfo(stream, out componentInfo))
			{
				return false;
			}
			m_PreviousRequiredComponentInfo[m] = componentInfo;
		}
		stream.Read(m_ConsoleId, 0, 5);
		stream.Read(m_OwnerXuid, 0, 8);
		m_Dirty = true;
		return Validate();
	}

	public static void SaveBlendShape(EndianStream stream, BlendShape blendShape)
	{
		stream.WriteGuid(blendShape.m_BlendShapeAssetId);
	}

	public static void SaveReplacementTexture(EndianStream stream, ReplacementTexture replacementTexture)
	{
		stream.WriteGuid(replacementTexture.m_TextureAssetId);
		stream.WriteFloat(replacementTexture.m_Placement.m_Scale);
		stream.WriteFloat(replacementTexture.m_Placement.m_Rotation);
		stream.WriteFloat(replacementTexture.m_Placement.m_TranslationU);
		stream.WriteFloat(replacementTexture.m_Placement.m_TranslationV);
	}

	public static void SaveColor(EndianStream stream, Colorb color)
	{
		stream.WriteByte(color.alpha);
		stream.WriteByte(color.red);
		stream.WriteByte(color.green);
		stream.WriteByte(color.blue);
	}

	public static void SaveComponentInfo(EndianStream stream, ComponentInfo component)
	{
		stream.WriteGuid(component.AssetId);
		stream.WriteShort((short)component.m_ComponentMask);
		stream.WriteShort(0);
		SaveColor(stream, component.m_CustomColors0);
		SaveColor(stream, component.m_CustomColors1);
		SaveColor(stream, component.m_CustomColors2);
	}

	public override byte[] SaveToBinary()
	{
		byte[] array = new byte[1000];
		MemoryStream inputStream = new MemoryStream(array);
		EndianStream endianStream = new EndianStream(inputStream);
		endianStream.WriteUint(0u);
		endianStream.WriteFloat(m_WeightFactor);
		endianStream.WriteFloat(m_HeightFactor);
		int num = m_BlendShapes.Length;
		int i;
		for (i = 0; i < num; i++)
		{
			SaveBlendShape(endianStream, m_BlendShapes[i]);
		}
		if (i < 3)
		{
			BlendShape blendShape = default(BlendShape);
			for (; i < 3; i++)
			{
				SaveBlendShape(endianStream, blendShape);
			}
		}
		num = m_ReplacementTextures.Length;
		for (i = 0; i < num; i++)
		{
			SaveReplacementTexture(endianStream, m_ReplacementTextures[i]);
		}
		if (i < 6)
		{
			ReplacementTexture replacementTexture = default(ReplacementTexture);
			for (; i < 6; i++)
			{
				SaveReplacementTexture(endianStream, replacementTexture);
			}
		}
		Debug.Assert(9 == m_DynamicColors.Length);
		for (i = 0; i < 9; i++)
		{
			SaveColor(endianStream, m_DynamicColors[i]);
		}
		SaveComponentInfo(endianStream, m_BodyComponentInfo.m_ComponentInfo);
		SaveComponentInfo(endianStream, m_HeadComponentInfo.m_ComponentInfo);
		num = m_ComponentInfo.Count;
		for (i = 0; i < num; i++)
		{
			SaveComponentInfo(endianStream, m_ComponentInfo[i].m_ComponentInfo);
		}
		if (i < 13)
		{
			ComponentInfo component = default(ComponentInfo);
			for (; i < 13; i++)
			{
				SaveComponentInfo(endianStream, component);
			}
		}
		num = m_PreviousRequiredComponentInfo.Length;
		for (i = 0; i < num; i++)
		{
			SaveComponentInfo(endianStream, m_PreviousRequiredComponentInfo[i]);
		}
		endianStream.Write(m_ConsoleId, 0, 5);
		endianStream.Write(m_OwnerXuid, 0, 8);
		return array;
	}
}
