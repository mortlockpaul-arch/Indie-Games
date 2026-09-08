using System;
using System.IO;
using System.Threading;
using Microsoft.XboxLive.Avatars.Internal.Animations;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Version1;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetLoader
{
	public CoordinateSystem m_CoordinateSystem;

	public IResourceFactory m_ResourceFactory;

	public bool m_AllowAlternativeAssets = true;

	public IDataManager m_dataManager;

	public bool m_UseCache = true;

	public static AvatarAssetCacheManager s_AssetCache;

	public CoordinateSystem CoordinateSystem => m_CoordinateSystem;

	public bool AllowAlternativeAvatarAssets
	{
		get
		{
			return m_AllowAlternativeAssets;
		}
		set
		{
			m_AllowAlternativeAssets = value;
		}
	}

	public bool UseCache
	{
		get
		{
			return m_UseCache;
		}
		set
		{
			m_UseCache = value;
		}
	}

	public AssetLoader(IDataManager dataManager, IResourceFactory resourceFactory, CoordinateSystem coordinateSystem)
	{
		if (dataManager == null)
		{
			throw new ArgumentNullException("dataManager");
		}
		if (resourceFactory == null)
		{
			throw new ArgumentNullException("resourceFactory");
		}
		m_CoordinateSystem = coordinateSystem;
		m_ResourceFactory = resourceFactory;
		m_dataManager = dataManager;
		m_UseCache = true;
	}

	public static bool EnableAssetCaching()
	{
		return EnableAssetCaching(33554432);
	}

	public static bool EnableAssetCaching(int cacheSize)
	{
		AvatarAssetCacheManager avatarAssetCacheManager = s_AssetCache;
		if (avatarAssetCacheManager != null)
		{
			avatarAssetCacheManager.SetCacheSize(cacheSize);
			return false;
		}
		s_AssetCache = new AvatarAssetCacheManager(cacheSize);
		return true;
	}

	public static void DisableAssetCaching()
	{
		s_AssetCache = null;
	}

	public static int GetAssetCacheEstimatedMemoryUsage()
	{
		return s_AssetCache?.GetEstimatedMemoryUsage() ?? 0;
	}

	public static void ClearCaches()
	{
		AvatarAssetsDependenciesResolver.InvalidateDependenciesTable();
		s_AssetCache?.ReleaseCache();
	}

	public AvatarAssetCacheManager GetAssetCacheManager()
	{
		if (m_UseCache)
		{
			return s_AssetCache;
		}
		return null;
	}

	public static AvatarAssetCacheManager GetCacheManager()
	{
		return s_AssetCache;
	}

	public Avatar CreateAvatar(AvatarManifest manifest)
	{
		return CreateAvatar(manifest, AvatarComponentMasks.All);
	}

	public static AvatarGender GetAssetBodyType(Guid avatarAssetId)
	{
		byte[] array = avatarAssetId.ToByteArray();
		return (array[6] & 0xF) switch
		{
			0 => AvatarGender.Unknown, 
			1 => AvatarGender.Male, 
			2 => AvatarGender.Female, 
			3 => AvatarGender.Both, 
			_ => AvatarGender.Unknown, 
		};
	}

	public static ComponentCategories GetComponentTypeFromAssetId(Guid avatarAssetId)
	{
		byte[] array = avatarAssetId.ToByteArray();
		byte[] value = new byte[4]
		{
			array[0],
			array[1],
			array[2],
			array[3]
		};
		return (ComponentCategories)BitConverter.ToInt32(value, 0);
	}

	public static AssetGuidType GetAssetGuidType(Guid avatarAssetId)
	{
		byte[] array = avatarAssetId.ToByteArray();
		if ((array[8] & 0xF0) != 192)
		{
			return AssetGuidType.Custom;
		}
		return (array[7] & 0xF) switch
		{
			0 => AssetGuidType.TOC, 
			1 => AssetGuidType.Awardable, 
			2 => AssetGuidType.MarketPlace, 
			15 => AssetGuidType.Custom, 
			_ => AssetGuidType.Custom, 
		};
	}

	public static bool IsStockAsset(Guid avatarAssetId)
	{
		AssetGuidType assetGuidType = GetAssetGuidType(avatarAssetId);
		if (assetGuidType == AssetGuidType.TOC || assetGuidType == AssetGuidType.Custom)
		{
			return true;
		}
		return false;
	}

	public static string GetTitleIdFromAssetId(Guid avatarAssetId)
	{
		if (IsStockAsset(avatarAssetId))
		{
			return Resources.AvatarEditorTitleId;
		}
		int num = 0;
		byte[] array = avatarAssetId.ToByteArray();
		num += array[15];
		num += array[14] << 8;
		num += array[13] << 16;
		num += array[12] << 24;
		return Convert.ToString(num, 16);
	}

	public static DynamicColorType GetColorAssetType(Guid guid)
	{
		ComponentCategories componentTypeFromAssetId = GetComponentTypeFromAssetId(guid);
		DynamicColorType result = DynamicColorType.Count;
		switch (componentTypeFromAssetId)
		{
		case ComponentCategories.Valid:
			result = DynamicColorType.Skin;
			break;
		case ComponentCategories.Hair:
			result = DynamicColorType.Hair;
			break;
		case ComponentCategories.Mouth:
			result = DynamicColorType.Mouth;
			break;
		case ComponentCategories.Eyes:
			result = DynamicColorType.Iris;
			break;
		case ComponentCategories.Eyebrows:
			result = DynamicColorType.Eyebrow;
			break;
		case ComponentCategories.EyeShadow:
			result = DynamicColorType.EyeShadow;
			break;
		case ComponentCategories.FacialHair:
			result = DynamicColorType.FacialHair;
			break;
		case ComponentCategories.FacialOther:
			result = DynamicColorType.SkinFeatures1;
			break;
		}
		return result;
	}

	public static DynamicTextureType GetTextureAssetType(Guid guid)
	{
		ComponentCategories componentTypeFromAssetId = GetComponentTypeFromAssetId(guid);
		DynamicTextureType result = DynamicTextureType.Count;
		switch (componentTypeFromAssetId)
		{
		case ComponentCategories.FacialHair:
			result = DynamicTextureType.FacialHair;
			break;
		case ComponentCategories.Mouth:
			result = DynamicTextureType.Mouth;
			break;
		case ComponentCategories.Eyes:
			result = DynamicTextureType.Eye;
			break;
		case ComponentCategories.Eyebrows:
			result = DynamicTextureType.Eyebrow;
			break;
		case ComponentCategories.EyeShadow:
			result = DynamicTextureType.EyeShadow;
			break;
		case ComponentCategories.FacialOther:
			result = DynamicTextureType.SkinFeatures;
			break;
		}
		return result;
	}

	public static BlendShapeType GetBlendShapeAssetType(Guid guid)
	{
		ComponentCategories componentTypeFromAssetId = GetComponentTypeFromAssetId(guid);
		BlendShapeType result = BlendShapeType.Count;
		switch (componentTypeFromAssetId)
		{
		case ComponentCategories.Chin:
			result = BlendShapeType.Chin;
			break;
		case ComponentCategories.Ears:
			result = BlendShapeType.Ear;
			break;
		case ComponentCategories.Nose:
			result = BlendShapeType.Nose;
			break;
		}
		return result;
	}

	public static int GetAssetVersion(Guid assetId)
	{
		return 1;
	}

	public static int GetAssetVersion(Stream assetStream)
	{
		return 1;
	}

	public bool CheckThreadAccess()
	{
		return true;
	}

	public Avatar CreateAvatar(AvatarManifest manifest, AvatarComponentMasks componentMask)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		if (manifest == null)
		{
			throw new ArgumentException(Resources.ManifestCannotBeNullText);
		}
		int version = manifest.Version;
		if (version == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.Load(manifest.Clone() as AvatarManifestV1, componentMask, m_AllowAlternativeAssets);
		}
		return null;
	}

	public AvatarComponent GetAvatarComponent(Guid componentId, Colorb[] customColors)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(componentId);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadAvatarComponent(componentId, customColors);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AvatarComponent GetAvatarComponent(Stream componentStream, Colorb[] customColors)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(componentStream);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadAvatarComponent(componentStream, customColors);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AvatarCarryable GetAvatarCarryable(Guid carryableId, Skeleton avatarSkeleton, Colorb[] customColors)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(carryableId);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadAvatarCarryable(carryableId, avatarSkeleton, customColors);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AvatarCarryable GetAvatarCarryable(Stream carryableStream, Skeleton avatarSkeleton, Colorb[] customColors)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(carryableStream);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadAvatarCarryable(carryableStream, avatarSkeleton, customColors);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public ComponentColors[] GetComponentColorTable(Guid componentId)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(componentId);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadColorTable(componentId);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public ComponentColors[] GetComponentColorTable(Stream componentStream)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(componentStream);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadColorTable(componentStream);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AvatarAnimation GetAvatarAnimation(Guid animationId)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(animationId);
		if (assetVersion == 1)
		{
			AvatarGetAnimation avatarGetAnimation = new AvatarGetAnimation(m_CoordinateSystem);
			return avatarGetAnimation.Load(animationId, this, m_dataManager);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AvatarAnimation GetAvatarAnimation(Stream animationStream)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(animationStream);
		if (assetVersion == 1)
		{
			AvatarGetAnimation avatarGetAnimation = new AvatarGetAnimation(m_CoordinateSystem);
			return avatarGetAnimation.Load(animationStream);
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AssetSubcategory GetAssetSubcategory(Guid assetId)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		int assetVersion = GetAssetVersion(assetId);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadMetadata(assetId)?.AssetSubcategory ?? AssetSubcategory.None;
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public AssetSubcategory GetAssetSubcategory(Stream assetStream)
	{
		if (!CheckThreadAccess())
		{
			throw new NotSupportedException(Resources.IvalidCallingThread);
		}
		if (assetStream == null)
		{
			throw new ArgumentNullException("assetStream");
		}
		int assetVersion = GetAssetVersion(assetStream);
		if (assetVersion == 1)
		{
			AvatarGetData avatarGetData = new AvatarGetData(m_dataManager, GetAssetCacheManager(), m_ResourceFactory, m_CoordinateSystem);
			return avatarGetData.LoadMetadata(assetStream)?.AssetSubcategory ?? AssetSubcategory.None;
		}
		throw new InvalidOperationException(Resources.UnknownAssetVersionText);
	}

	public void CreateAvatarAsync(AvatarManifest manifest, EventHandler<GetAvatarAssetsEventArgs> eventHandler)
	{
		CreateAvatarAsync(manifest, AvatarComponentMasks.All, eventHandler);
	}

	public void CreateAvatarAsync(AvatarManifest manifest, AvatarComponentMasks componentMask, EventHandler<GetAvatarAssetsEventArgs> eventHandler)
	{
		if (manifest == null)
		{
			throw new ArgumentException(Resources.ManifestCannotBeNullText);
		}
		GetAvatarAssetsAsync getAvatarAssetsAsync = new GetAvatarAssetsAsync(this, manifest.Clone(), componentMask, eventHandler);
		Thread thread = new Thread(getAvatarAssetsAsync.Process);
		thread.Start();
	}

	public void GetAvatarAnimationAsync(Guid animationId, EventHandler<GetAvatarAnimationEventArgs> eventHandler)
	{
		GetAvatarAnimationAsync getAvatarAnimationAsync = new GetAvatarAnimationAsync(this, animationId, eventHandler);
		Thread thread = new Thread(getAvatarAnimationAsync.Process);
		thread.Start();
	}

	public void GetAvatarComponentAsync(Guid componentId, Colorb[] customColors, EventHandler<GetAvatarComponentEventArgs> eventHandler)
	{
		GetAvatarComponentAsync getAvatarComponentAsync = new GetAvatarComponentAsync(this, componentId, customColors, eventHandler);
		Thread thread = new Thread(getAvatarComponentAsync.Process);
		thread.Start();
	}

	public void GetComponentColorTableAsync(Guid componentId, EventHandler<GetComponentColorTableEventArgs> eventHandler)
	{
		GetComponentColorTableAsync getComponentColorTableAsync = new GetComponentColorTableAsync(this, componentId, eventHandler);
		Thread thread = new Thread(getComponentColorTableAsync.Process);
		thread.Start();
	}

	public void GetAvatarCarryableAsync(Guid carryableId, Skeleton avatarSkeleton, Colorb[] customColors, EventHandler<GetAvatarCarryableEventArgs> eventHandler)
	{
		GetAvatarCarryableAsync getAvatarCarryableAsync = new GetAvatarCarryableAsync(this, carryableId, avatarSkeleton, customColors, eventHandler);
		Thread thread = new Thread(getAvatarCarryableAsync.Process);
		thread.Start();
	}
}
