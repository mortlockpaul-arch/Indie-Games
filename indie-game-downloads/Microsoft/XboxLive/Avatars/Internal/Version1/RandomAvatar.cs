#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class RandomAvatar
{
	public struct RandomCategory(ComponentCategories category, byte[] aChanges)
	{
		public ComponentCategories m_CategoryMask = category;

		public byte[] m_aChances = aChanges;
	}

	public struct RandomAssetInfo(byte category, Guid asset, AvatarGender bodyMask, AvatarGender randomBodyMask, ComponentColors[] colorSets)
	{
		public byte m_iCategory = category;

		public Guid m_Asset = asset;

		public byte m_BodyMask = (byte)bodyMask;

		public byte m_RandomBodyMask = (byte)randomBodyMask;

		public ComponentColors[] m_customColors = colorSets;
	}

	public struct DynamicData
	{
		public int m_cAvatars;

		public AvatarGender m_BodyMask;

		public int[] m_aAvatarsPerBodyType;

		public byte[] m_aBodyTypes;

		public ComponentCategories[] m_aRequiredCategoryMasks;

		public ComponentCategories[] m_aCategoryMasks;

		public ComponentCategories[] m_aFoundCategoryMasks;

		public byte[] m_aHeights;

		public byte[] m_aWeights;

		public int[][] m_aAssets;

		public byte[][] m_aDynamicColors;

		public short[] m_aManifestOrder;

		public BitArray m_HeightUsage;

		public BitArray m_WeightUsage;

		public BitArray m_AssetUsage;

		public BitArray[] m_aColorUsage;

		public void Initialize(int count)
		{
			int num = 3;
			m_cAvatars = count;
			m_aAvatarsPerBodyType = new int[num];
			m_aBodyTypes = new byte[count];
			m_aRequiredCategoryMasks = new ComponentCategories[num];
			m_aCategoryMasks = new ComponentCategories[count];
			m_aFoundCategoryMasks = new ComponentCategories[count];
			m_aHeights = new byte[count];
			m_aWeights = new byte[count];
			m_aDynamicColors = new byte[9][];
			m_aAssets = new int[XAVATARTOC_CATEGORY_COUNT][];
			for (int i = 0; i < XAVATARTOC_CATEGORY_COUNT; i++)
			{
				m_aAssets[i] = new int[count];
			}
			m_HeightUsage = new BitArray(c_aBodyHeights.Length);
			m_WeightUsage = new BitArray(c_aBodyWeights.Length);
			m_AssetUsage = new BitArray(s_RandomAssets.Length);
			m_aColorUsage = new BitArray[9];
			m_HeightUsage.SetAll(value: false);
			m_WeightUsage.SetAll(value: false);
			m_AssetUsage.SetAll(value: false);
			for (int j = 0; j < m_aColorUsage.Length; j++)
			{
				int num2 = c_aRandomColorTable[j].Length;
				if (num2 == 0)
				{
					num2 = c_aColorTable[j].Length;
				}
				m_aColorUsage[j] = new BitArray(num2);
				m_aColorUsage[j].SetAll(value: false);
				m_aDynamicColors[j] = new byte[count];
			}
			GetValueSet(s_random, m_HeightUsage, m_aHeights, count);
			GetValueSet(s_random, m_WeightUsage, m_aWeights, count);
			for (int k = 0; k < m_aColorUsage.Length; k++)
			{
				GetValueSet(s_random, m_aColorUsage[k], m_aDynamicColors[k], count);
			}
			m_aManifestOrder = new short[count];
			for (int l = 0; l < count; l++)
			{
				m_aManifestOrder[l] = (short)l;
			}
			for (int m = 0; m < count; m++)
			{
				int num3 = s_random.Next(count - 1);
				short num4 = m_aManifestOrder[num3];
				m_aManifestOrder[num3] = m_aManifestOrder[m];
				m_aManifestOrder[m] = num4;
			}
		}

		public void GetValueSet(Random random, BitArray domain, byte[] result, int length)
		{
			int num = 0;
			Debug.Assert(domain.Count > 0);
			for (int i = 0; i < domain.Count; i++)
			{
				if (!domain.Get(i))
				{
					num++;
				}
			}
			int num2 = 0;
			while (num2 < length)
			{
				if (num == 0)
				{
					domain.SetAll(value: false);
					num = domain.Count;
				}
				int num3 = random.Next(num--);
				int num4 = 0;
				for (num4 = 0; num4 < domain.Count; num4++)
				{
					if (!domain.Get(num4))
					{
						if (num3 == 0)
						{
							break;
						}
						num3--;
					}
				}
				Debug.Assert(num3 == 0);
				result[num2++] = (byte)num4;
				domain.Set(num4, value: true);
			}
		}
	}

	public static int XAVATARTOC_CATEGORY_COUNT;

	public static int XAVATAR_BODY_COUNT;

	public static int XAVATARTOC_ASSET_INDEX_INVALID;

	public static float[] c_aBodyHeights;

	public static float[] c_aBodyWeights;

	public static Colorb[] c_aBodyColors;

	public static Colorb[] c_aBodyColorsRandom;

	public static Colorb[] c_aHairColors;

	public static Colorb[] c_aHairColorsRandomForBody2_11_12;

	public static Colorb[] c_aHairColorsRandomForBody5_14_15;

	public static Colorb[] c_aHairColorsRandomForBody6_13;

	public static Colorb[] c_aHairColorsRandomForBody1_8_17_18;

	public static Colorb[] c_aHairColorsRandomForBody4_7;

	public static Colorb[] c_aHairColorsRandomForBody9_16;

	public static Colorb[] c_aFaceColors;

	public static Colorb[] c_aEyeColors;

	public static Colorb[] c_aEyeColorsRandom;

	public static Colorb[] c_aEyeShadowColors;

	public static Colorb[] c_aLipColors;

	public static Colorb[] c_aLipColorsRandom;

	public static Colorb[] c_aNoColor;

	public static Colorb[][] c_aColorTable;

	public static Colorb[][] c_aRandomColorTable;

	public static readonly RandomCategory[] c_aRandomCategories;

	public static readonly ComponentColors[] s_ColorTable_00000008_0048_0001_C1C8_F109A19CB2E0;

	public static readonly ComponentColors[] s_ColorTable_00000008_008E_0001_C1C8_F109A19CB2E0;

	public static readonly ComponentColors[] s_ColorTable_00000008_0113_0002_C1C8_F109A19CB2E0;

	public static readonly ComponentColors[] s_ColorTable_00000008_012C_0002_C1C8_F109A19CB2E0;

	public static readonly RandomAssetInfo[] s_RandomAssets;

	public static Random s_random;

	public DynamicData m_data;

	public RandomAvatar()
	{
		m_data = default(DynamicData);
	}

	public AvatarManifest[] CreateAvatars(AvatarGender bodyMask, int avatarsCount)
	{
		AvatarManifest[] array = new AvatarManifest[avatarsCount];
		m_data.Initialize(avatarsCount);
		m_data.m_BodyMask = bodyMask;
		if (!BuildBodyTable())
		{
			return null;
		}
		if (!BuildCategoryTable())
		{
			return null;
		}
		if (!BuildAssetTable())
		{
			return null;
		}
		for (int i = 0; i < m_data.m_cAvatars; i++)
		{
			int num = m_data.m_aManifestOrder[i];
			array[num] = CreateManifest(i);
			if (array[num] == null)
			{
				return null;
			}
		}
		return array;
	}

	public bool BuildBodyTable()
	{
		int num = m_data.m_cAvatars;
		int num2 = ((m_data.m_BodyMask <= AvatarGender.Female) ? 1 : 2);
		Debug.Assert(m_data.m_BodyMask != AvatarGender.Unknown);
		if (m_data.m_cAvatars / 2 / num2 > 0)
		{
			for (int i = 0; i < XAVATAR_BODY_COUNT; i++)
			{
				if (((uint)m_data.m_BodyMask & (uint)(1 << i)) != 0)
				{
					m_data.m_aAvatarsPerBodyType[i] = m_data.m_cAvatars / 2 / num2;
					num -= m_data.m_aAvatarsPerBodyType[i];
				}
			}
		}
		for (int j = 0; j < XAVATAR_BODY_COUNT; j++)
		{
			if (((uint)m_data.m_BodyMask & (uint)(1 << j)) != 0)
			{
				if (--num2 <= 0)
				{
					m_data.m_aAvatarsPerBodyType[j] += num;
					break;
				}
				int num3 = s_random.Next(num + 1);
				m_data.m_aAvatarsPerBodyType[j] += num3;
				num -= num3;
			}
		}
		int num4 = 0;
		for (byte b = 0; b < XAVATAR_BODY_COUNT; b++)
		{
			for (int k = 0; k < m_data.m_aAvatarsPerBodyType[b]; k++)
			{
				m_data.m_aBodyTypes[num4++] = b;
			}
		}
		Debug.Assert(num4 == m_data.m_cAvatars);
		return true;
	}

	public bool BuildCategoryTable()
	{
		for (int i = 0; i < XAVATAR_BODY_COUNT; i++)
		{
			for (int j = 0; j < c_aRandomCategories.Length; j++)
			{
				if (100 == c_aRandomCategories[j].m_aChances[i])
				{
					m_data.m_aRequiredCategoryMasks[i] |= c_aRandomCategories[j].m_CategoryMask;
				}
			}
		}
		Debug.Assert(m_data.m_cAvatars <= m_data.m_aCategoryMasks.Length);
		for (int k = 0; k < m_data.m_cAvatars; k++)
		{
			int num = m_data.m_aBodyTypes[k];
			for (int l = 0; l < c_aRandomCategories.Length; l++)
			{
				double num2 = (double)(int)c_aRandomCategories[l].m_aChances[num] / 100.0;
				double num3 = s_random.NextDouble();
				if (num3 < num2)
				{
					m_data.m_aCategoryMasks[k] |= c_aRandomCategories[l].m_CategoryMask;
				}
				else
				{
					Debug.Assert((c_aRandomCategories[l].m_CategoryMask & m_data.m_aRequiredCategoryMasks[num]) == 0);
				}
			}
		}
		return true;
	}

	public bool BuildAssetTable()
	{
		for (int i = 0; i < c_aRandomCategories.Length; i++)
		{
			for (int j = 0; j < m_data.m_cAvatars; j++)
			{
				if (!BuildAssetTable(i, j))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool BuildAssetTable(int iCategory, int avatarIndex)
	{
		int num = 1 << (int)m_data.m_aBodyTypes[avatarIndex];
		int num2 = 0;
		Debug.Assert(iCategory < m_data.m_aAssets.Length);
		Debug.Assert(avatarIndex < m_data.m_aAssets[iCategory].Length);
		m_data.m_aAssets[iCategory][avatarIndex] = XAVATARTOC_ASSET_INDEX_INVALID;
		for (int i = 0; i < s_RandomAssets.Length; i++)
		{
			if (s_RandomAssets[i].m_iCategory == iCategory && (s_RandomAssets[i].m_RandomBodyMask & num) != 0 && !m_data.m_AssetUsage.Get(i))
			{
				num2++;
			}
		}
		if (num2 == 0)
		{
			for (int j = 0; j < s_RandomAssets.Length; j++)
			{
				if (s_RandomAssets[j].m_iCategory == iCategory && (s_RandomAssets[j].m_RandomBodyMask & num) != 0)
				{
					m_data.m_AssetUsage.Set(j, value: false);
					num2++;
				}
			}
		}
		if (num2 == 0)
		{
			return true;
		}
		int num3 = s_random.Next(num2);
		for (int k = 0; k < s_RandomAssets.Length; k++)
		{
			if (s_RandomAssets[k].m_iCategory == iCategory && (s_RandomAssets[k].m_RandomBodyMask & num) != 0 && !m_data.m_AssetUsage.Get(k))
			{
				if (num3 == 0)
				{
					m_data.m_AssetUsage.Set(k, value: true);
					m_data.m_aAssets[iCategory][avatarIndex] = (short)k;
					break;
				}
				num3--;
			}
		}
		Debug.Assert(num3 == 0);
		return true;
	}

	public AvatarManifest CreateManifest(int avatarIndex)
	{
		int num = m_data.m_aBodyTypes[avatarIndex];
		AvatarManifestV1 manifest = new AvatarManifestV1();
		manifest.m_Dirty = true;
		if (!SetBodySize(ref manifest, avatarIndex))
		{
			return null;
		}
		if (!SetBlendShapes(ref manifest, avatarIndex))
		{
			return null;
		}
		if (!SetTextures(ref manifest, avatarIndex))
		{
			return null;
		}
		if (!SetDynamicColors(ref manifest, avatarIndex))
		{
			return null;
		}
		if (!SetModels(ref manifest, avatarIndex))
		{
			return null;
		}
		if ((m_data.m_aFoundCategoryMasks[avatarIndex] & m_data.m_aRequiredCategoryMasks[num]) != m_data.m_aRequiredCategoryMasks[num])
		{
			throw new AvatarException("Unable to load a required asset");
		}
		return manifest;
	}

	public bool SetBodySize(ref AvatarManifestV1 manifest, int avatarIndex)
	{
		float heightFactor = c_aBodyHeights[m_data.m_aHeights[avatarIndex]];
		float widthFactor = c_aBodyWeights[m_data.m_aWeights[avatarIndex]];
		manifest.WidthFactor = widthFactor;
		manifest.HeightFactor = heightFactor;
		return true;
	}

	public uint lsb(uint v, int bitsize)
	{
		for (int i = 0; i < bitsize; i++)
		{
			if ((v & (1 << i)) > 0)
			{
				return (uint)i;
			}
		}
		uint num = 0u;
		return num - 1;
	}

	public bool SetBlendShape(ref AvatarManifestV1 manifest, int avatarIndex, BlendShapeType eShape, ComponentCategories categoryMask)
	{
		if ((m_data.m_aCategoryMasks[avatarIndex] & categoryMask) == 0)
		{
			return true;
		}
		uint num = lsb((uint)categoryMask, 32);
		int iAsset = m_data.m_aAssets[num][avatarIndex];
		Guid assetId = GetAssetId(iAsset);
		if (assetId == Guid.Empty)
		{
			return true;
		}
		manifest.SetBlendShape(eShape, assetId);
		m_data.m_aFoundCategoryMasks[avatarIndex] |= categoryMask;
		return true;
	}

	public bool SetBlendShapes(ref AvatarManifestV1 manifest, int avatarIndex)
	{
		if (!SetBlendShape(ref manifest, avatarIndex, BlendShapeType.Chin, ComponentCategories.Chin))
		{
			return false;
		}
		if (!SetBlendShape(ref manifest, avatarIndex, BlendShapeType.Nose, ComponentCategories.Nose))
		{
			return false;
		}
		if (!SetBlendShape(ref manifest, avatarIndex, BlendShapeType.Ear, ComponentCategories.Ears))
		{
			return false;
		}
		return true;
	}

	public bool SetTexture(ref AvatarManifestV1 manifest, int avatarIndex, DynamicTextureType eTexture, ComponentCategories categoryMask)
	{
		if ((m_data.m_aCategoryMasks[avatarIndex] & categoryMask) == 0)
		{
			return true;
		}
		uint num = lsb((uint)categoryMask, 32);
		int num2 = m_data.m_aAssets[num][avatarIndex];
		if (num2 == -1)
		{
			return true;
		}
		Guid assetId = GetAssetId(num2);
		if (assetId == Guid.Empty)
		{
			return true;
		}
		AvatarManifestV1.ReplacementTexture texture = new AvatarManifestV1.ReplacementTexture
		{
			m_LinkedAssetId = Guid.Empty,
			m_Placement = 
			{
				m_Scale = 1f,
				m_Rotation = 0f,
				m_TranslationU = 0f,
				m_TranslationV = 0f
			},
			m_TextureAssetId = assetId
		};
		manifest.SetReplacementTexture(eTexture, texture);
		m_data.m_aFoundCategoryMasks[avatarIndex] |= categoryMask;
		return true;
	}

	public bool SetTextures(ref AvatarManifestV1 manifest, int avatarIndex)
	{
		if (!SetTexture(ref manifest, avatarIndex, DynamicTextureType.Eye, ComponentCategories.Eyes))
		{
			return false;
		}
		if (!SetTexture(ref manifest, avatarIndex, DynamicTextureType.Eyebrow, ComponentCategories.Eyebrows))
		{
			return false;
		}
		if (!SetTexture(ref manifest, avatarIndex, DynamicTextureType.Mouth, ComponentCategories.Mouth))
		{
			return false;
		}
		if (!SetTexture(ref manifest, avatarIndex, DynamicTextureType.FacialHair, ComponentCategories.FacialHair))
		{
			return false;
		}
		if (!SetTexture(ref manifest, avatarIndex, DynamicTextureType.SkinFeatures, ComponentCategories.FacialOther))
		{
			return false;
		}
		if (!SetTexture(ref manifest, avatarIndex, DynamicTextureType.EyeShadow, ComponentCategories.EyeShadow))
		{
			return false;
		}
		return true;
	}

	public bool SetHairDynamicColors(ref AvatarManifestV1 manifest, int avatarIndex)
	{
		Colorb color = XAvatarGetLinkedBodyHairColor(m_data.m_aDynamicColors[0][avatarIndex]);
		manifest.SetDynamicColor(DynamicColorType.Hair, color);
		return true;
	}

	public Colorb XAvatarGetLinkedBodyHairColor(int ulSkinColorIndex)
	{
		Colorb colorb = new Colorb(0, 0, 0);
		Random random = new Random();
		int num = 0;
		for (int i = 0; i < c_aBodyColors.Length; i++)
		{
			if (c_aBodyColorsRandom[ulSkinColorIndex].red == c_aBodyColors[i].red && c_aBodyColorsRandom[ulSkinColorIndex].green == c_aBodyColors[i].green && c_aBodyColorsRandom[ulSkinColorIndex].blue == c_aBodyColors[i].blue)
			{
				num = i + 1;
				break;
			}
		}
		switch (num)
		{
		case 2:
		case 3:
		case 10:
		case 11:
		case 12:
			return c_aHairColorsRandomForBody2_11_12[random.Next(c_aHairColorsRandomForBody2_11_12.Length - 1)];
		case 5:
		case 14:
		case 15:
			return c_aHairColorsRandomForBody5_14_15[random.Next(c_aHairColorsRandomForBody5_14_15.Length - 1)];
		case 6:
		case 13:
			return c_aHairColorsRandomForBody6_13[random.Next(c_aHairColorsRandomForBody6_13.Length - 1)];
		case 1:
		case 8:
		case 17:
		case 18:
			return c_aHairColorsRandomForBody1_8_17_18[random.Next(c_aHairColorsRandomForBody1_8_17_18.Length - 1)];
		case 4:
		case 7:
			return c_aHairColorsRandomForBody4_7[random.Next(c_aHairColorsRandomForBody4_7.Length - 1)];
		case 9:
		case 16:
			return c_aHairColorsRandomForBody9_16[random.Next(c_aHairColorsRandomForBody9_16.Length - 1)];
		default:
			return c_aHairColorsRandomForBody2_11_12[random.Next(c_aHairColorsRandomForBody2_11_12.Length - 1)];
		}
	}

	public bool SetDynamicColor(ref AvatarManifestV1 manifest, int avatarIndex, DynamicColorType eColor)
	{
		Colorb color = ((c_aRandomColorTable[(int)eColor].Length == 0) ? c_aColorTable[(int)eColor][m_data.m_aDynamicColors[(int)eColor][avatarIndex]] : c_aRandomColorTable[(int)eColor][m_data.m_aDynamicColors[(int)eColor][avatarIndex]]);
		manifest.SetDynamicColor(eColor, color);
		return true;
	}

	public bool SetDynamicColors(ref AvatarManifestV1 manifest, int avatarIndex)
	{
		if (!SetDynamicColor(ref manifest, avatarIndex, DynamicColorType.Skin))
		{
			return false;
		}
		if (!SetHairDynamicColors(ref manifest, avatarIndex))
		{
			return false;
		}
		if (!SetDynamicColor(ref manifest, avatarIndex, DynamicColorType.Iris))
		{
			return false;
		}
		if (!SetDynamicColor(ref manifest, avatarIndex, DynamicColorType.EyeShadow))
		{
			return false;
		}
		if (!SetDynamicColor(ref manifest, avatarIndex, DynamicColorType.Mouth))
		{
			return false;
		}
		if (!SetDynamicColor(ref manifest, avatarIndex, DynamicColorType.SkinFeatures1))
		{
			return false;
		}
		manifest.SetDynamicColor(DynamicColorType.SkinFeatures2, Utilities.ColorbFromVector4(manifest.GetDynamicColor(DynamicColorType.SkinFeatures1)));
		manifest.SetDynamicColor(DynamicColorType.Eyebrow, Utilities.ColorbFromVector4(manifest.GetDynamicColor(DynamicColorType.Hair)));
		manifest.SetDynamicColor(DynamicColorType.FacialHair, Utilities.ColorbFromVector4(manifest.GetDynamicColor(DynamicColorType.Hair)));
		return true;
	}

	public bool SetModel(ref AvatarManifestV1 manifest, int avatarIndex, int iCategory)
	{
		ComponentCategories componentCategories = (ComponentCategories)(1 << iCategory);
		if ((m_data.m_aCategoryMasks[avatarIndex] & componentCategories) == 0)
		{
			return true;
		}
		int num = m_data.m_aAssets[iCategory][avatarIndex];
		if (num == -1)
		{
			return true;
		}
		Guid assetId = GetAssetId(num);
		if (assetId == Guid.Empty)
		{
			return true;
		}
		ComponentInfo componentInfo = new ComponentInfo
		{
			m_ComponentMask = (AvatarComponentMasks)componentCategories,
			m_AssetId = assetId
		};
		if (s_RandomAssets[num].m_customColors != null)
		{
			ComponentColors[] customColors = s_RandomAssets[num].m_customColors;
			int num2 = s_random.Next(customColors.Length);
			componentInfo.m_CustomColors0 = Utilities.ColorbFromVector4(customColors[num2].CustomColor0);
			componentInfo.m_CustomColors1 = Utilities.ColorbFromVector4(customColors[num2].CustomColor1);
			componentInfo.m_CustomColors2 = Utilities.ColorbFromVector4(customColors[num2].CustomColor2);
		}
		if (ComponentCategories.Body == componentCategories)
		{
			manifest.m_BodyComponentInfo = new ComponentDescription(componentInfo, Guid.Empty);
		}
		else if (ComponentCategories.Head == componentCategories)
		{
			manifest.m_HeadComponentInfo = new ComponentDescription(componentInfo, Guid.Empty);
		}
		else
		{
			manifest.SetComponentInfo(componentInfo);
		}
		m_data.m_aFoundCategoryMasks[avatarIndex] |= componentCategories;
		return true;
	}

	public bool SetModels(ref AvatarManifestV1 manifest, int avatarIndex)
	{
		ComponentCategories componentCategories = m_data.m_aCategoryMasks[avatarIndex] & ComponentCategories.Models;
		for (int i = 0; i < 32; i++)
		{
			if (((uint)componentCategories & (uint)(1 << i)) != 0 && !SetModel(ref manifest, avatarIndex, i))
			{
				return false;
			}
		}
		return true;
	}

	public Guid GetAssetId(int iAsset)
	{
		if (XAVATARTOC_ASSET_INDEX_INVALID == iAsset)
		{
			return Guid.Empty;
		}
		Debug.Assert(iAsset < s_RandomAssets.Length);
		return s_RandomAssets[iAsset].m_Asset;
	}

	static RandomAvatar()
	{
		XAVATARTOC_CATEGORY_COUNT = 25;
		XAVATAR_BODY_COUNT = 2;
		XAVATARTOC_ASSET_INDEX_INVALID = 65535;
		c_aBodyHeights = new float[5] { -1f, -0.5f, 0f, 0.5f, 1f };
		c_aBodyWeights = new float[5] { -1f, -0.5f, 0f, 0.5f, 1f };
		c_aBodyColors = new Colorb[18]
		{
			new Colorb(110, 65, 36),
			new Colorb(215, 170, 113),
			new Colorb(252, 238, 221),
			new Colorb(100, 65, 27),
			new Colorb(190, 135, 73),
			new Colorb(248, 219, 185),
			new Colorb(64, 40, 14),
			new Colorb(152, 101, 47),
			new Colorb(230, 188, 142),
			new Colorb(219, 202, 183),
			new Colorb(227, 169, 118),
			new Colorb(215, 185, 113),
			new Colorb(240, 179, 147),
			new Colorb(222, 145, 77),
			new Colorb(199, 152, 76),
			new Colorb(227, 148, 107),
			new Colorb(211, 121, 61),
			new Colorb(168, 125, 63)
		};
		c_aBodyColorsRandom = new Colorb[12]
		{
			new Colorb(110, 65, 36),
			new Colorb(215, 170, 113),
			new Colorb(100, 65, 27),
			new Colorb(190, 135, 73),
			new Colorb(152, 101, 47),
			new Colorb(230, 188, 142),
			new Colorb(227, 169, 118),
			new Colorb(215, 185, 113),
			new Colorb(240, 179, 147),
			new Colorb(222, 145, 77),
			new Colorb(199, 152, 76),
			new Colorb(168, 125, 63)
		};
		c_aHairColors = new Colorb[27]
		{
			new Colorb(byte.MaxValue, 252, 181),
			new Colorb(228, 211, 129),
			new Colorb(222, 207, 186),
			new Colorb(254, 239, 124),
			new Colorb(220, 183, 79),
			new Colorb(191, 167, 131),
			new Colorb(247, 215, 72),
			new Colorb(189, 147, 75),
			new Colorb(152, 128, 84),
			new Colorb(144, 102, 54),
			new Colorb(133, 84, 26),
			new Colorb(110, 83, 38),
			new Colorb(116, 81, 49),
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(227, 128, 47),
			new Colorb(146, 52, 14),
			new Colorb(226, 222, 221),
			new Colorb(184, 94, 34),
			new Colorb(104, 38, 24),
			new Colorb(142, 130, 116),
			new Colorb(117, 62, 31),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aHairColorsRandomForBody2_11_12 = new Colorb[22]
		{
			new Colorb(222, 207, 186),
			new Colorb(220, 183, 79),
			new Colorb(191, 167, 131),
			new Colorb(189, 147, 75),
			new Colorb(152, 128, 84),
			new Colorb(144, 102, 54),
			new Colorb(133, 84, 26),
			new Colorb(110, 83, 38),
			new Colorb(116, 81, 49),
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(227, 128, 47),
			new Colorb(146, 52, 14),
			new Colorb(184, 94, 34),
			new Colorb(104, 38, 24),
			new Colorb(142, 130, 116),
			new Colorb(117, 62, 31),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aHairColorsRandomForBody5_14_15 = new Colorb[17]
		{
			new Colorb(220, 183, 79),
			new Colorb(189, 147, 75),
			new Colorb(144, 102, 54),
			new Colorb(133, 84, 26),
			new Colorb(110, 83, 38),
			new Colorb(116, 81, 49),
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(146, 52, 14),
			new Colorb(184, 94, 34),
			new Colorb(104, 38, 24),
			new Colorb(117, 62, 31),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aHairColorsRandomForBody6_13 = new Colorb[24]
		{
			new Colorb(228, 211, 129),
			new Colorb(254, 239, 124),
			new Colorb(220, 183, 79),
			new Colorb(191, 167, 131),
			new Colorb(247, 215, 72),
			new Colorb(189, 147, 75),
			new Colorb(152, 128, 84),
			new Colorb(144, 102, 54),
			new Colorb(133, 84, 26),
			new Colorb(110, 83, 38),
			new Colorb(116, 81, 49),
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(227, 128, 47),
			new Colorb(146, 52, 14),
			new Colorb(184, 94, 34),
			new Colorb(104, 38, 24),
			new Colorb(142, 130, 116),
			new Colorb(117, 62, 31),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aHairColorsRandomForBody1_8_17_18 = new Colorb[11]
		{
			new Colorb(110, 83, 38),
			new Colorb(116, 81, 49),
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(146, 52, 14),
			new Colorb(104, 38, 24),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aHairColorsRandomForBody4_7 = new Colorb[9]
		{
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(146, 52, 14),
			new Colorb(104, 38, 24),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aHairColorsRandomForBody9_16 = new Colorb[21]
		{
			new Colorb(228, 211, 129),
			new Colorb(220, 183, 79),
			new Colorb(189, 147, 75),
			new Colorb(152, 128, 84),
			new Colorb(144, 102, 54),
			new Colorb(133, 84, 26),
			new Colorb(110, 83, 38),
			new Colorb(116, 81, 49),
			new Colorb(85, 57, 33),
			new Colorb(55, 33, 22),
			new Colorb(73, 52, 33),
			new Colorb(31, 18, 11),
			new Colorb(13, 13, 13),
			new Colorb(227, 128, 47),
			new Colorb(146, 52, 14),
			new Colorb(184, 94, 34),
			new Colorb(104, 38, 24),
			new Colorb(142, 130, 116),
			new Colorb(117, 62, 31),
			new Colorb(82, 52, 54),
			new Colorb(79, 70, 61)
		};
		c_aFaceColors = new Colorb[27]
		{
			new Colorb(232, 200, 79),
			new Colorb(230, 115, 184),
			new Colorb(102, 188, 220),
			new Colorb(225, 132, 23),
			new Colorb(127, 57, 121),
			new Colorb(42, 176, 171),
			new Colorb(209, 57, 39),
			new Colorb(95, 66, 148),
			new Colorb(66, 101, 188),
			new Colorb(138, 191, 85),
			new Colorb(188, 174, 137),
			new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
			new Colorb(57, 146, 81),
			new Colorb(140, 96, 58),
			new Colorb(139, 139, 139),
			new Colorb(77, 85, 35),
			new Colorb(101, 68, 40),
			new Colorb(97, 97, 97),
			new Colorb(170, 124, 101),
			new Colorb(215, 95, 71),
			new Colorb(236, 182, 190),
			new Colorb(173, 95, 71),
			new Colorb(170, 29, 38),
			new Colorb(207, 89, 105),
			new Colorb(89, 51, 47),
			new Colorb(129, 39, 31),
			new Colorb(178, 79, 125)
		};
		c_aEyeColors = new Colorb[18]
		{
			new Colorb(146, 178, 202),
			new Colorb(136, 176, 73),
			new Colorb(174, 152, 67),
			new Colorb(99, 129, 167),
			new Colorb(114, 127, 53),
			new Colorb(130, 80, 29),
			new Colorb(42, 114, 164),
			new Colorb(55, 81, 42),
			new Colorb(91, 53, 30),
			new Colorb(145, 122, 81),
			new Colorb(206, 206, 204),
			new Colorb(123, 123, 149),
			new Colorb(49, 33, 20),
			new Colorb(89, 93, 92),
			new Colorb(143, 116, 151),
			new Colorb(33, 33, 33),
			new Colorb(36, 63, 83),
			new Colorb(171, 2, 7)
		};
		c_aEyeColorsRandom = new Colorb[15]
		{
			new Colorb(146, 178, 202),
			new Colorb(136, 176, 73),
			new Colorb(174, 152, 67),
			new Colorb(99, 129, 167),
			new Colorb(114, 127, 53),
			new Colorb(130, 80, 29),
			new Colorb(42, 114, 164),
			new Colorb(55, 81, 42),
			new Colorb(91, 53, 30),
			new Colorb(145, 122, 81),
			new Colorb(206, 206, 204),
			new Colorb(49, 33, 20),
			new Colorb(89, 93, 92),
			new Colorb(33, 33, 33),
			new Colorb(36, 63, 83)
		};
		c_aEyeShadowColors = new Colorb[26]
		{
			new Colorb(186, 114, 182),
			new Colorb(170, 166, 201),
			new Colorb(247, 90, 135),
			new Colorb(154, 47, 125),
			new Colorb(119, 109, 211),
			new Colorb(197, 73, 109),
			new Colorb(99, 26, 70),
			new Colorb(110, 80, 164),
			new Colorb(169, 206, 240),
			new Colorb(178, 176, 93),
			new Colorb(byte.MaxValue, 226, 163),
			new Colorb(83, 149, 202),
			new Colorb(193, 195, 53),
			new Colorb(254, 200, 84),
			new Colorb(25, 83, 129),
			new Colorb(97, 112, 31),
			new Colorb(224, 110, 50),
			new Colorb(243, 188, 123),
			new Colorb(193, 165, 144),
			new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
			new Colorb(192, 137, 80),
			new Colorb(179, 125, 89),
			new Colorb(162, 168, 155),
			new Colorb(144, 79, 51),
			new Colorb(106, 57, 25),
			new Colorb(0, 0, 0)
		};
		c_aLipColors = new Colorb[27]
		{
			new Colorb(253, 106, 104),
			new Colorb(229, 137, 149),
			new Colorb(213, 146, 130),
			new Colorb(208, 14, 14),
			new Colorb(198, 82, 101),
			new Colorb(181, 97, 87),
			new Colorb(173, 45, 45),
			new Colorb(159, 93, 105),
			new Colorb(131, 70, 63),
			new Colorb(207, 133, 98),
			new Colorb(235, 125, 128),
			new Colorb(239, 194, 139),
			new Colorb(135, 87, 65),
			new Colorb(245, 104, 107),
			new Colorb(236, 168, 83),
			new Colorb(92, 59, 44),
			new Colorb(203, 103, 88),
			new Colorb(188, 117, 53),
			new Colorb(212, 101, 82),
			new Colorb(218, 97, 212),
			new Colorb(120, 109, 213),
			new Colorb(177, 74, 55),
			new Colorb(185, 57, 150),
			new Colorb(110, 81, 165),
			new Colorb(138, 26, 14),
			new Colorb(143, 45, 94),
			new Colorb(0, 0, 0)
		};
		c_aLipColorsRandom = new Colorb[21]
		{
			new Colorb(253, 106, 104),
			new Colorb(229, 137, 149),
			new Colorb(213, 146, 130),
			new Colorb(208, 14, 14),
			new Colorb(198, 82, 101),
			new Colorb(181, 97, 87),
			new Colorb(173, 45, 45),
			new Colorb(159, 93, 105),
			new Colorb(131, 70, 63),
			new Colorb(207, 133, 98),
			new Colorb(235, 125, 128),
			new Colorb(239, 194, 139),
			new Colorb(135, 87, 65),
			new Colorb(245, 104, 107),
			new Colorb(236, 168, 83),
			new Colorb(92, 59, 44),
			new Colorb(203, 103, 88),
			new Colorb(188, 117, 53),
			new Colorb(212, 101, 82),
			new Colorb(177, 74, 55),
			new Colorb(138, 26, 14)
		};
		c_aNoColor = new Colorb[0];
		c_aColorTable = new Colorb[9][] { c_aBodyColors, c_aHairColors, c_aLipColors, c_aEyeColors, c_aHairColors, c_aEyeShadowColors, c_aHairColors, c_aFaceColors, c_aFaceColors };
		c_aRandomColorTable = new Colorb[9][] { c_aBodyColorsRandom, c_aNoColor, c_aLipColorsRandom, c_aEyeColorsRandom, c_aNoColor, c_aNoColor, c_aNoColor, c_aNoColor, c_aNoColor };
		RandomCategory[] array = new RandomCategory[25]
		{
			new RandomCategory(ComponentCategories.Head, new byte[2] { 100, 100 }),
			new RandomCategory(ComponentCategories.Body, new byte[2] { 100, 100 }),
			new RandomCategory(ComponentCategories.Hair, new byte[2] { 100, 100 }),
			new RandomCategory(ComponentCategories.Shirt, new byte[2] { 100, 100 }),
			new RandomCategory(ComponentCategories.Trousers, new byte[2] { 100, 100 }),
			new RandomCategory(ComponentCategories.Shoes, new byte[2] { 100, 100 }),
			new RandomCategory(ComponentCategories.Hat, new byte[2] { 20, 5 }),
			new RandomCategory(ComponentCategories.Gloves, new byte[2] { 5, 5 }),
			new RandomCategory(ComponentCategories.Glasses, new byte[2] { 10, 10 }),
			new RandomCategory(ComponentCategories.Wristwear, new byte[2] { 5, 20 }),
			new RandomCategory(ComponentCategories.Earrings, new byte[2] { 1, 75 }),
			new RandomCategory(ComponentCategories.Ring, new byte[2] { 5, 75 }),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory),
			default(RandomCategory)
		};
		ref RandomCategory reference = ref array[12];
		byte[] aChanges = new byte[2];
		reference = new RandomCategory(ComponentCategories.Carryable, aChanges);
		ref RandomCategory reference2 = ref array[13];
		reference2 = new RandomCategory(ComponentCategories.Eyes, new byte[2] { 100, 100 });
		ref RandomCategory reference3 = ref array[14];
		reference3 = new RandomCategory(ComponentCategories.Eyebrows, new byte[2] { 100, 100 });
		ref RandomCategory reference4 = ref array[15];
		reference4 = new RandomCategory(ComponentCategories.Mouth, new byte[2] { 100, 100 });
		ref RandomCategory reference5 = ref array[16];
		reference5 = new RandomCategory(ComponentCategories.FacialHair, new byte[2] { 10, 0 });
		ref RandomCategory reference6 = ref array[17];
		reference6 = new RandomCategory(ComponentCategories.FacialOther, new byte[2] { 20, 40 });
		ref RandomCategory reference7 = ref array[18];
		aChanges = new byte[2];
		reference7 = new RandomCategory(ComponentCategories.EyeShadow, aChanges);
		ref RandomCategory reference8 = ref array[19];
		reference8 = new RandomCategory(ComponentCategories.Nose, new byte[2] { 100, 100 });
		ref RandomCategory reference9 = ref array[20];
		reference9 = new RandomCategory(ComponentCategories.Chin, new byte[2] { 100, 100 });
		ref RandomCategory reference10 = ref array[21];
		reference10 = new RandomCategory(ComponentCategories.Ears, new byte[2] { 100, 100 });
		ref RandomCategory reference11 = ref array[22];
		aChanges = new byte[2];
		reference11 = new RandomCategory(ComponentCategories.Shape, aChanges);
		ref RandomCategory reference12 = ref array[23];
		aChanges = new byte[2];
		reference12 = new RandomCategory(ComponentCategories.Animation, aChanges);
		ref RandomCategory reference13 = ref array[24];
		aChanges = new byte[2];
		reference13 = new RandomCategory(ComponentCategories.Costume, aChanges);
		c_aRandomCategories = array;
		s_ColorTable_00000008_0048_0001_C1C8_F109A19CB2E0 = new ComponentColors[9]
		{
			new ComponentColors(new Colorb[3]
			{
				new Colorb(50, 134, 218),
				new Colorb(50, 134, 218),
				new Colorb(50, 134, 218)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(128, 128, 128),
				new Colorb(128, 128, 128),
				new Colorb(128, 128, 128)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(35, 35, 35),
				new Colorb(35, 35, 35),
				new Colorb(35, 35, 35)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(128, 64, 0),
				new Colorb(128, 64, 0),
				new Colorb(128, 64, 0)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, byte.MaxValue, 0),
				new Colorb(byte.MaxValue, byte.MaxValue, 0),
				new Colorb(byte.MaxValue, byte.MaxValue, 0)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, 141, 28),
				new Colorb(byte.MaxValue, 141, 28),
				new Colorb(byte.MaxValue, 141, 28)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(215, 41, 9),
				new Colorb(215, 41, 9),
				new Colorb(215, 41, 9)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(91, 134, 13),
				new Colorb(91, 134, 13),
				new Colorb(91, 134, 13)
			})
		};
		s_ColorTable_00000008_008E_0001_C1C8_F109A19CB2E0 = new ComponentColors[1]
		{
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, 0, 0),
				new Colorb(0, byte.MaxValue, 0),
				new Colorb(0, 0, byte.MaxValue)
			})
		};
		s_ColorTable_00000008_0113_0002_C1C8_F109A19CB2E0 = new ComponentColors[9]
		{
			new ComponentColors(new Colorb[3]
			{
				new Colorb(35, 35, 35),
				new Colorb(35, 35, 35),
				new Colorb(35, 35, 35)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(97, 231, 103),
				new Colorb(97, 231, 103),
				new Colorb(97, 231, 103)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, byte.MaxValue, 0),
				new Colorb(byte.MaxValue, byte.MaxValue, 0),
				new Colorb(byte.MaxValue, byte.MaxValue, 0)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, 141, 28),
				new Colorb(byte.MaxValue, 141, 28),
				new Colorb(byte.MaxValue, 141, 28)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(217, 0, 0),
				new Colorb(217, 0, 0),
				new Colorb(217, 0, 0)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, 183, 219),
				new Colorb(byte.MaxValue, 183, 219),
				new Colorb(byte.MaxValue, 183, 219)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(166, 15, 93),
				new Colorb(166, 15, 93),
				new Colorb(166, 15, 93)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(15, 93, 166),
				new Colorb(15, 93, 166),
				new Colorb(15, 93, 166)
			})
		};
		s_ColorTable_00000008_012C_0002_C1C8_F109A19CB2E0 = new ComponentColors[9]
		{
			new ComponentColors(new Colorb[3]
			{
				new Colorb(50, 134, 218),
				new Colorb(50, 134, 218),
				new Colorb(50, 134, 218)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue),
				new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(128, 128, 128),
				new Colorb(128, 128, 128),
				new Colorb(128, 128, 128)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(35, 35, 35),
				new Colorb(35, 35, 35),
				new Colorb(35, 35, 35)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(128, 64, 0),
				new Colorb(128, 64, 0),
				new Colorb(128, 64, 0)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, byte.MaxValue, 0),
				new Colorb(byte.MaxValue, byte.MaxValue, 0),
				new Colorb(byte.MaxValue, byte.MaxValue, 0)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(byte.MaxValue, 141, 28),
				new Colorb(byte.MaxValue, 141, 28),
				new Colorb(byte.MaxValue, 141, 28)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(215, 41, 9),
				new Colorb(215, 41, 9),
				new Colorb(215, 41, 9)
			}),
			new ComponentColors(new Colorb[3]
			{
				new Colorb(91, 134, 13),
				new Colorb(91, 134, 13),
				new Colorb(91, 134, 13)
			})
		};
		s_RandomAssets = new RandomAssetInfo[470]
		{
			new RandomAssetInfo(1, new Guid("{00000002-0000-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(1, new Guid("{00000002-0001-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(0, new Guid("{00000001-0002-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(5, new Guid("{00000020-002c-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-002d-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-002e-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-002f-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0030-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0031-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0032-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0034-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0035-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0036-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0037-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0038-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-0039-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-003a-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-003b-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-003c-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-03d8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-003e-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0044-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0048-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, s_ColorTable_00000008_0048_0001_C1C8_F109A19CB2E0),
			new RandomAssetInfo(3, new Guid("{00000008-004b-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-004f-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0050-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0052-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0054-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0056-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0058-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0060-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0061-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0062-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0063-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0064-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0065-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-0067-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-006a-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-006e-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-006f-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(3, new Guid("{00000008-008e-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, s_ColorTable_00000008_008E_0001_C1C8_F109A19CB2E0),
			new RandomAssetInfo(4, new Guid("{00000010-008f-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0090-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0091-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0092-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0094-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0096-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0098-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-0099-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-009a-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-009c-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-009d-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-009e-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-009f-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a1-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a2-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a3-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a4-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a7-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(4, new Guid("{00000010-00a9-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00b5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00b6-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00b7-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00b8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00b9-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00ba-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00bb-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-00bc-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f2-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f3-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f4-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f6-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f7-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00bd-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00be-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00bf-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00c1-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00c2-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00c5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00c6-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00c9-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00ca-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00cb-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00cc-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-03c2-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(8, new Guid("{00000100-00cd-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00ce-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00cf-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00d0-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00d1-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00d5-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00d8-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00d9-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00da-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-00db-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-03c3-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(8, new Guid("{00000100-03c4-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-00dc-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00dd-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00de-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00df-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00e0-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00e1-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00e2-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(9, new Guid("{00000200-00e3-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00e4-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00e5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00e6-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00e7-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00e8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00e9-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00ea-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00eb-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00ed-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00ee-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00ef-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00f0-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-00f3-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(6, new Guid("{00000040-03c5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00f4-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00f5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00f6-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00f7-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00f8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00f9-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00fa-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(11, new Guid("{00000800-00fb-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(5, new Guid("{00000020-00fc-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-00fd-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-00fe-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-00ff-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0100-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0101-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0102-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0103-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0104-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0105-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0107-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0108-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-0109-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-010a-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-010b-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-010c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(5, new Guid("{00000020-03c6-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-010f-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0111-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0113-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, s_ColorTable_00000008_0113_0002_C1C8_F109A19CB2E0),
			new RandomAssetInfo(3, new Guid("{00000008-0114-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0115-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0117-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-011b-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-011c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-011e-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-011f-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0120-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0122-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0123-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0129-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-012a-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-012c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, s_ColorTable_00000008_012C_0002_C1C8_F109A19CB2E0),
			new RandomAssetInfo(3, new Guid("{00000008-0130-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0131-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0132-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0134-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0136-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0138-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-0139-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-03cd-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-03d2-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(3, new Guid("{00000008-03d9-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0158-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0159-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-015a-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-015b-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-015c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-015e-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-015f-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0160-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0161-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0162-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0163-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0164-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0165-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0166-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0167-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0168-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0169-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-016a-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-016b-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-016c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-016d-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-016e-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-016f-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-0170-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-04ee-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(4, new Guid("{00000010-03d3-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-018a-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-018b-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-018c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-018d-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f9-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04fa-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04fb-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04fc-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04fd-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04fe-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04ff-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-0500-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-0501-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-03c7-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-03d6-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04ec-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04ed-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f0-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(10, new Guid("{00000400-04f1-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-018e-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-018f-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-0190-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-0191-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-0192-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-0193-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-0194-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(9, new Guid("{00000200-0195-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-0196-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-0197-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-0198-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-0199-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-019a-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-019b-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-019c-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-019d-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-019f-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-01a0-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-01a1-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-01a2-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-01a3-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-01a4-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(6, new Guid("{00000040-03d1-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01a6-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01a7-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01a8-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01a9-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01aa-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01ab-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01ac-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(11, new Guid("{00000800-01ad-0002-c1c8-f109a19cb2e0}"), AvatarGender.Female, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01ae-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01b0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01b2-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01b6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01bd-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01bf-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01c3-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01c4-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01c6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01c8-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01ca-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01cc-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01ce-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01d0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01d1-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01d6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01d7-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01da-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01dc-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01de-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01e2-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01e6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01e8-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(2, new Guid("{00000004-01ea-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(2, new Guid("{00000004-01f0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01f2-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-01f4-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01f6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01f8-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01fa-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-01fe-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-0200-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-0201-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-0204-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0206-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-020a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-020e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(2, new Guid("{00000004-0210-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0214-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0216-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0217-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-021b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-021d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-021f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-0221-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0223-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0225-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0227-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0229-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-022b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-022d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-022f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0231-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0233-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0235-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0237-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-0239-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(2, new Guid("{00000004-023b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(2, new Guid("{00000004-023d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-023f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0241-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0245-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0247-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-0249-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-024b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(2, new Guid("{00000004-024f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-0253-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(2, new Guid("{00000004-025b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(2, new Guid("{00000004-025d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(14, new Guid("{00004000-0260-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(14, new Guid("{00004000-0261-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-0262-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-0263-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(14, new Guid("{00004000-0264-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-0266-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(14, new Guid("{00004000-0267-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(14, new Guid("{00004000-0269-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-026a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-026b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(14, new Guid("{00004000-026c-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(14, new Guid("{00004000-026d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-026e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-026f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(14, new Guid("{00004000-0271-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(13, new Guid("{00002000-027a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-027b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-027d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-027e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-027f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0280-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-0281-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0282-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-0283-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0284-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-0285-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0286-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-0287-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-0289-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-028a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-028b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-028c-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-028d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-028e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(18, new Guid("{00040000-028f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0290-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-0291-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0292-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-0293-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-0295-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-0297-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-0298-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-0299-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-029a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-029b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-029c-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-029d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-029e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-029f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02a0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02a1-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02a2-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02a3-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02a4-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02a5-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02a6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02a7-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02a9-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02ab-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02ac-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(18, new Guid("{00040000-02ad-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02ae-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02af-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02b1-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02b3-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02b4-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02b5-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02b7-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02b9-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02ba-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02bb-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02bc-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02bd-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02be-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02bf-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02c0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02c1-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02c2-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02c3-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02c5-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02c6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02c7-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02c8-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02c9-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02ca-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(18, new Guid("{00040000-02cb-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02cc-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(18, new Guid("{00040000-02cd-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02ce-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(18, new Guid("{00040000-02cf-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02d0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(18, new Guid("{00040000-02d1-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(13, new Guid("{00002000-02d2-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(18, new Guid("{00040000-02d3-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(16, new Guid("{00010000-02d4-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02d5-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02d6-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02d7-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02d8-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02d9-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02da-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02db-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02dd-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02de-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02df-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02e0-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02e3-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(16, new Guid("{00010000-02e4-0001-c1c8-f109a19cb2e0}"), AvatarGender.Male, AvatarGender.Male, null),
			new RandomAssetInfo(15, new Guid("{00008000-02e6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(15, new Guid("{00008000-02e7-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(15, new Guid("{00008000-02e9-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(15, new Guid("{00008000-02ea-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(15, new Guid("{00008000-02eb-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(15, new Guid("{00008000-02ec-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(15, new Guid("{00008000-02ee-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(15, new Guid("{00008000-02ef-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(15, new Guid("{00008000-02f0-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(15, new Guid("{00008000-02f6-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(15, new Guid("{00008000-02f9-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(15, new Guid("{00008000-02fa-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(15, new Guid("{00008000-02fc-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(15, new Guid("{00008000-02fd-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(15, new Guid("{00008000-02ff-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(20, new Guid("{00100000-031a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(20, new Guid("{00100000-031b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(20, new Guid("{00100000-031c-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(20, new Guid("{00100000-031d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(20, new Guid("{00100000-031e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(20, new Guid("{00100000-031f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(20, new Guid("{00100000-0320-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(20, new Guid("{00100000-0321-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(19, new Guid("{00080000-0323-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-0324-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-0325-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(19, new Guid("{00080000-0326-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(19, new Guid("{00080000-0327-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Female, null),
			new RandomAssetInfo(19, new Guid("{00080000-0328-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-032a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-032b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(19, new Guid("{00080000-032d-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-032e-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-032f-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(19, new Guid("{00080000-0330-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(19, new Guid("{00080000-0331-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-0333-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(19, new Guid("{00080000-0334-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Male, null),
			new RandomAssetInfo(21, new Guid("{00200000-0336-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(21, new Guid("{00200000-0337-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(21, new Guid("{00200000-0339-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(21, new Guid("{00200000-033a-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(21, new Guid("{00200000-033b-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null),
			new RandomAssetInfo(21, new Guid("{00200000-033c-0003-c1c8-f109a19cb2e0}"), AvatarGender.Both, AvatarGender.Both, null)
		};
		s_random = new Random(DateTime.Now.Second);
	}
}
