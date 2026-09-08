using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Parsers;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Version1;

public class AvatarGetData
{
	public enum Result
	{
		Ok,
		Pending,
		InvalidManifest,
		Failed
	}

	public struct ColorBlendPoint(Vector4 r, Colorb s)
	{
		public Vector4 Rim = r;

		public Colorb Skin = s;
	}

	public struct ParamsUsageMapItem(ShaderParameterUsage u, DynamicColorType c)
	{
		public ShaderParameterUsage use = u;

		public DynamicColorType color = c;
	}

	public class LoadMetaDataContext
	{
		public AssetMetadataParser parser;

		public AssetsAsyncLoadContextCommon common = new AssetsAsyncLoadContextCommon();
	}

	public static readonly Vector4 DefaultRimLight = new Vector4(28f / 51f, 0.5019608f, 0.40392157f, 1.6f);

	public static readonly ColorBlendPoint[] RimLightColours = new ColorBlendPoint[18]
	{
		new ColorBlendPoint(new Vector4(0.40784314f, 0.2509804f, 0.18431373f, 1.6f), new Colorb(101, 60, 35)),
		new ColorBlendPoint(new Vector4(28f / 51f, 0.47058824f, 0.4117647f, 1.6f), new Colorb(198, 157, 113)),
		new ColorBlendPoint(new Vector4(26f / 51f, 26f / 51f, 26f / 51f, 1.6f), new Colorb(232, 220, 221)),
		new ColorBlendPoint(new Vector4(7f / 15f, 0.2784314f, 0.2509804f, 1.6f), new Colorb(92, 60, 26)),
		new ColorBlendPoint(new Vector4(0.5372549f, 0.40784314f, 14f / 51f, 1.6f), new Colorb(175, 128, 73)),
		new ColorBlendPoint(new Vector4(26f / 51f, 23f / 51f, 0.41960785f, 1.6f), new Colorb(229, 202, 185)),
		new ColorBlendPoint(new Vector4(24f / 85f, 0.1764706f, 12f / 85f, 1.6f), new Colorb(59, 36, 13)),
		new ColorBlendPoint(new Vector4(0.42745098f, 0.2784314f, 0.18039216f, 1.6f), new Colorb(140, 93, 46)),
		new ColorBlendPoint(new Vector4(26f / 51f, 0.45490196f, 0.41960785f, 1.6f), new Colorb(212, 173, 142)),
		new ColorBlendPoint(new Vector4(0.5137255f, 0.47058824f, 0.4627451f, 1.6f), new Colorb(202, 183, 183)),
		new ColorBlendPoint(new Vector4(41f / 85f, 0.40784314f, 0.34901962f, 1.6f), new Colorb(209, 156, 118)),
		new ColorBlendPoint(new Vector4(44f / 85f, 0.44313726f, 0.34901962f, 1.6f), new Colorb(198, 171, 113)),
		new ColorBlendPoint(new Vector4(28f / 51f, 0.41960785f, 0.40784314f, 1.6f), new Colorb(221, 165, 147)),
		new ColorBlendPoint(new Vector4(43f / 85f, 16f / 51f, 18f / 85f, 1.6f), new Colorb(205, 134, 77)),
		new ColorBlendPoint(new Vector4(41f / 85f, 37f / 85f, 0.3019608f, 1.6f), new Colorb(184, 140, 76)),
		new ColorBlendPoint(new Vector4(23f / 51f, 16f / 51f, 0.32156864f, 1.6f), new Colorb(209, 136, 107)),
		new ColorBlendPoint(new Vector4(0.4627451f, 0.38039216f, 1f / 3f, 1.6f), new Colorb(195, 111, 60)),
		new ColorBlendPoint(new Vector4(39f / 85f, 29f / 85f, 0.24313726f, 1.6f), new Colorb(155, 115, 62))
	};

	public static readonly ParamsUsageMapItem[] ManifestColorToParamUsageMap = new ParamsUsageMapItem[9]
	{
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorSkin, DynamicColorType.Skin),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorHair, DynamicColorType.Hair),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorMouth, DynamicColorType.Mouth),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorIris, DynamicColorType.Iris),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorEyebrow, DynamicColorType.Eyebrow),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorEyeShadow, DynamicColorType.EyeShadow),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorFacialHair, DynamicColorType.FacialHair),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorSkinFeature1, DynamicColorType.SkinFeatures1),
		new ParamsUsageMapItem(ShaderParameterUsage.PixelConstantColorSkinFeature2, DynamicColorType.SkinFeatures2)
	};

	public AvatarManifestV1 m_Manifest;

	public List<BinaryAsset> m_RequestedAssets;

	public AvatarAssetCacheManager m_AssetCache;

	public CoordinateSystem m_CoordinateSystem;

	public IResourceFactory m_ResourceFactory;

	public IDataManager m_dataManager;

	public Avatar m_Target;

	public List<BinaryAsset> RequestedAssets => m_RequestedAssets;

	public AvatarGetData(IDataManager dataManager, AvatarAssetCacheManager assetCache, IResourceFactory resourceFactory, CoordinateSystem coordinateSystem)
	{
		m_RequestedAssets = new List<BinaryAsset>();
		m_CoordinateSystem = coordinateSystem;
		m_AssetCache = assetCache;
		m_ResourceFactory = resourceFactory;
		m_dataManager = dataManager;
	}

	public Avatar Load(AvatarManifest manifest, AvatarComponentMasks componentMask, bool allowAlternativeAssets)
	{
		m_Manifest = manifest as AvatarManifestV1;
		m_Target = new Avatar(m_Manifest);
		if (m_Manifest.DressDefaultClothes)
		{
			m_Manifest.RemoveComponents((AvatarComponentMasks)(~(int)componentMask));
			DressDefaultClothes(componentMask);
		}
		else
		{
			m_Manifest.ClearComponents((AvatarComponentMasks)(~(int)componentMask));
		}
		CreateAssetsList(componentMask);
		if (!ValidateComponentMasks())
		{
			throw new AvatarException(Resources.ManifestValidationFailed);
		}
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = m_Manifest.GetUserCombinedComponentMask(componentMask);
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = m_Target;
		binaryAssetParseContext.m_BodyType = m_Manifest.BodyType;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		binaryAssetParseContext.m_AssetCache = m_AssetCache;
		if (binaryAssetParseContext.m_AssetCache != null)
		{
			AvatarAssetCacheV1 avatarAssetCacheV = binaryAssetParseContext.m_AssetCache.GetAssetCache(1) as AvatarAssetCacheV1;
			while (true)
			{
				bool flag = true;
				if (avatarAssetCacheV.LoadAssets(m_RequestedAssets, binaryAssetParseContext, m_dataManager))
				{
					break;
				}
				if (!m_Manifest.DressDefaultClothes || !allowAlternativeAssets)
				{
					throw new AvatarException(Resources.FailedToLoadAssetText);
				}
				foreach (BinaryAsset requestedAsset in m_RequestedAssets)
				{
					CachedBinaryAsset.AssetState assetState = CachedBinaryAsset.AssetState.Invalid;
					if (requestedAsset.m_Cache != null)
					{
						assetState = requestedAsset.m_Cache.m_AssetState;
					}
					if (assetState != CachedBinaryAsset.AssetState.Parsed)
					{
						if (m_Manifest.IsCoreAsset(requestedAsset.AssetId))
						{
							throw new AvatarException(Resources.FailedToLoadCoreAssetText);
						}
						Logger.Log(new DebugLog(this, "Failed to load non-stock assets " + requestedAsset.AssetId.ToString() + ", loading previous component instead."));
						m_Manifest.RemoveAsset(requestedAsset.AssetId);
					}
				}
				DressDefaultClothes(componentMask);
				CreateAssetsList(componentMask);
			}
			if (CreateAvatarFromCache(binaryAssetParseContext) != Result.Ok)
			{
				throw new AvatarException(Resources.IncompatibleAssetsText);
			}
		}
		else
		{
			while (true)
			{
				bool flag2 = true;
				LoadAssets();
				Result result = Result.Ok;
				foreach (BinaryAsset requestedAsset2 in m_RequestedAssets)
				{
					if (binaryAssetParseContext.m_skeletonVersion == Skeleton.SkeletonVersion.Invalid)
					{
						AssetMetadataParser metadata = requestedAsset2.GetMetadata();
						if (metadata != null)
						{
							binaryAssetParseContext.m_skeletonVersion = metadata.AssetSkeletonVersion;
						}
					}
					if (requestedAsset2.m_Stream == null || !requestedAsset2.Validate(binaryAssetParseContext))
					{
						if (m_Manifest.IsCoreAsset(requestedAsset2.AssetId))
						{
							throw new AvatarException(Resources.FailedToLoadCoreAssetText);
						}
						if (!m_Manifest.DressDefaultClothes || !allowAlternativeAssets)
						{
							throw new AvatarException(Resources.FailedToLoadAssetText);
						}
						Logger.Log(new DebugLog(this, "Failed to load non-stock assets " + requestedAsset2.AssetId.ToString() + ", loading previous component instead."));
						m_Manifest.RemoveAsset(requestedAsset2.AssetId);
						result = Result.Failed;
					}
				}
				if (result == Result.Ok)
				{
					break;
				}
				DressDefaultClothes(componentMask);
				CreateAssetsList(componentMask);
			}
			if (CreateAvatarFromStreams(binaryAssetParseContext) != Result.Ok)
			{
				throw new AvatarException(Resources.IncompatibleAssetsText);
			}
		}
		GenerateSkeleton(binaryAssetParseContext.m_skeletonVersion);
		FinalizeAssets();
		return m_Target;
	}

	public void LoadAvatarComponent(BinaryAssetModel assetLoader, BinaryAssetParseContext parseContext)
	{
		ComponentInfo componentDescription = assetLoader.m_ComponentDescription;
		ShaderConstantOverride[] shaderConstantOverrides = assetLoader.ShaderConstantOverrides;
		shaderConstantOverrides[0].m_Constant = ShaderParameterUsage.PixelConstantColorCustom0;
		shaderConstantOverrides[0].m_Value = Utilities.ColorbToVector4(componentDescription.m_CustomColors0);
		shaderConstantOverrides[1].m_Constant = ShaderParameterUsage.PixelConstantColorCustom1;
		shaderConstantOverrides[1].m_Value = Utilities.ColorbToVector4(componentDescription.m_CustomColors1);
		shaderConstantOverrides[2].m_Constant = ShaderParameterUsage.PixelConstantColorCustom2;
		shaderConstantOverrides[2].m_Value = Utilities.ColorbToVector4(componentDescription.m_CustomColors2);
		shaderConstantOverrides[3].m_Constant = ShaderParameterUsage.PixelConstantColorRimLight;
		shaderConstantOverrides[3].m_Value = DefaultRimLight;
		m_RequestedAssets = new List<BinaryAsset>();
		m_RequestedAssets.Add(assetLoader);
		if (assetLoader.m_Stream != null)
		{
			if (Validate(parseContext) != Result.Ok)
			{
				throw new AvatarException(Resources.InvalidComponentIdText);
			}
			CreateAvatarFromStreams(parseContext);
		}
		else if (parseContext.m_AssetCache != null)
		{
			AvatarAssetCacheV1 avatarAssetCacheV = parseContext.m_AssetCache.GetAssetCache(1) as AvatarAssetCacheV1;
			if (!avatarAssetCacheV.LoadAssets(m_RequestedAssets, parseContext, m_dataManager))
			{
				CachedBinaryAsset cache = assetLoader.m_Cache;
				if (cache.m_AssetState == CachedBinaryAsset.AssetState.Downloaded)
				{
					throw new AvatarException(Resources.InvalidComponentIdText);
				}
				throw new AvatarException(Resources.FailedToLoadAssetText);
			}
			CreateAvatarFromCache(parseContext);
		}
		else
		{
			if (LoadAssets() != Result.Ok)
			{
				throw new AvatarException(Resources.FailedToLoadAssetText);
			}
			if (Validate(parseContext) != Result.Ok)
			{
				throw new AvatarException(Resources.InvalidComponentIdText);
			}
			CreateAvatarFromStreams(parseContext);
		}
	}

	public AvatarComponent LoadAvatarComponent(Guid componentId, Colorb[] customColors)
	{
		m_Target = new Avatar(null);
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = AvatarComponentMasks.All;
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = m_Target;
		binaryAssetParseContext.m_BodyType = AvatarGender.Both;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		binaryAssetParseContext.m_AssetCache = m_AssetCache;
		ComponentInfo description;
		if (customColors == null)
		{
			Colorb colorb = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			description = new ComponentInfo(componentId, AvatarComponentMasks.All, colorb, colorb, colorb);
		}
		else
		{
			if (customColors.Length != 3)
			{
				throw new ArgumentException(Resources.InvalidCustomColorsText);
			}
			description = new ComponentInfo(componentId, AvatarComponentMasks.All, customColors[0], customColors[1], customColors[2]);
		}
		BinaryAssetModel assetLoader = new BinaryAssetModel(description, 4, AvatarComponentMasks.None);
		LoadAvatarComponent(assetLoader, binaryAssetParseContext);
		if (m_Target.m_Models.Count != 1)
		{
			throw new AvatarException(Resources.InvalidComponentIdText);
		}
		return m_Target.m_Models[0];
	}

	public AvatarComponent LoadAvatarComponent(Stream componentStream, Colorb[] customColors)
	{
		m_Target = new Avatar(null);
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = AvatarComponentMasks.All;
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = m_Target;
		binaryAssetParseContext.m_BodyType = AvatarGender.Both;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		binaryAssetParseContext.m_AssetCache = m_AssetCache;
		ComponentInfo description;
		if (customColors == null)
		{
			Colorb colorb = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			description = new ComponentInfo(Guid.Empty, AvatarComponentMasks.All, colorb, colorb, colorb);
		}
		else
		{
			if (customColors.Length != 3)
			{
				throw new ArgumentException(Resources.InvalidCustomColorsText);
			}
			description = new ComponentInfo(Guid.Empty, AvatarComponentMasks.All, customColors[0], customColors[1], customColors[2]);
		}
		BinaryAssetModel binaryAssetModel = new BinaryAssetModel(description, 4, AvatarComponentMasks.None);
		binaryAssetModel.Stream = componentStream;
		LoadAvatarComponent(binaryAssetModel, binaryAssetParseContext);
		if (m_Target.m_Models.Count != 1)
		{
			throw new AvatarException(Resources.InvalidComponentIdText);
		}
		return m_Target.m_Models[0];
	}

	public AvatarCarryable LoadAvatarCarryable(Guid componentId, Skeleton avatarSkeleton, Colorb[] customColors)
	{
		m_Target = new Avatar(null);
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = AvatarComponentMasks.All;
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = m_Target;
		binaryAssetParseContext.m_BodyType = AvatarGender.Both;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		binaryAssetParseContext.m_AssetCache = m_AssetCache;
		ComponentInfo description;
		if (customColors == null)
		{
			Colorb colorb = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			description = new ComponentInfo(componentId, AvatarComponentMasks.All, colorb, colorb, colorb);
		}
		else
		{
			if (customColors.Length != 3)
			{
				throw new ArgumentException(Resources.InvalidCustomColorsText);
			}
			description = new ComponentInfo(componentId, AvatarComponentMasks.All, customColors[0], customColors[1], customColors[2]);
		}
		BinaryAssetCarryable assetLoader = new BinaryAssetCarryable(description, 4, AvatarComponentMasks.None);
		LoadAvatarComponent(assetLoader, binaryAssetParseContext);
		if (m_Target.m_Carryable == null)
		{
			throw new AvatarException(Resources.InvalidComponentIdText);
		}
		if (avatarSkeleton != null)
		{
			Vector3 scale = avatarSkeleton.Joints[0].Local.scale;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.scale = scale;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.X *= scale.X;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.Y *= scale.Y;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.Z *= scale.Z;
		}
		return m_Target.m_Carryable;
	}

	public AvatarCarryable LoadAvatarCarryable(Stream componentStream, Skeleton avatarSkeleton, Colorb[] customColors)
	{
		m_Target = new Avatar(null);
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = AvatarComponentMasks.All;
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = m_Target;
		binaryAssetParseContext.m_BodyType = AvatarGender.Both;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		binaryAssetParseContext.m_AssetCache = m_AssetCache;
		ComponentInfo description;
		if (customColors == null)
		{
			Colorb colorb = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			description = new ComponentInfo(Guid.Empty, AvatarComponentMasks.All, colorb, colorb, colorb);
		}
		else
		{
			if (customColors.Length != 3)
			{
				throw new ArgumentException(Resources.InvalidCustomColorsText);
			}
			description = new ComponentInfo(Guid.Empty, AvatarComponentMasks.All, customColors[0], customColors[1], customColors[2]);
		}
		BinaryAssetCarryable binaryAssetCarryable = new BinaryAssetCarryable(description, 4, AvatarComponentMasks.None);
		binaryAssetCarryable.Stream = componentStream;
		LoadAvatarComponent(binaryAssetCarryable, binaryAssetParseContext);
		if (m_Target.m_Carryable == null)
		{
			throw new AvatarException(Resources.InvalidComponentIdText);
		}
		if (avatarSkeleton != null)
		{
			Vector3 scale = avatarSkeleton.Joints[0].Local.scale;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.scale = scale;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.X *= scale.X;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.Y *= scale.Y;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.Z *= scale.Z;
		}
		return m_Target.m_Carryable;
	}

	public ComponentColors[] LoadColorTable(Guid assetId)
	{
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = AvatarComponentMasks.All;
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = null;
		binaryAssetParseContext.m_BodyType = AvatarGender.Both;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		binaryAssetParseContext.m_AssetCache = m_AssetCache;
		Colorb colorb = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		ComponentInfo componentInfo = new ComponentInfo(assetId, AvatarComponentMasks.All, colorb, colorb, colorb);
		BinaryAssetColorTable binaryAssetColorTable = new BinaryAssetColorTable(assetId, AvatarComponentMasks.None);
		m_RequestedAssets = new List<BinaryAsset>();
		m_RequestedAssets.Add(binaryAssetColorTable);
		ComponentColorTable componentColorTable;
		if (binaryAssetParseContext.m_AssetCache != null)
		{
			AvatarAssetCacheV1 avatarAssetCacheV = binaryAssetParseContext.m_AssetCache.GetAssetCache(1) as AvatarAssetCacheV1;
			bool flag = avatarAssetCacheV.LoadAssets(m_RequestedAssets, binaryAssetParseContext, m_dataManager);
			if (!(binaryAssetColorTable.m_Cache is CachedBinaryAssetColorTable cachedBinaryAssetColorTable))
			{
				throw new AvatarException(Resources.InvalidComponentIdText);
			}
			if (!flag)
			{
				if (cachedBinaryAssetColorTable.m_AssetState == CachedBinaryAsset.AssetState.Downloaded)
				{
					throw new AvatarException(Resources.InvalidComponentIdText);
				}
				throw new AvatarException(Resources.FailedToLoadAssetText);
			}
			componentColorTable = cachedBinaryAssetColorTable.ColorTable;
		}
		else
		{
			if (LoadAssets() != Result.Ok)
			{
				throw new AvatarException(Resources.FailedToLoadAssetText);
			}
			if (Validate(binaryAssetParseContext) != Result.Ok)
			{
				throw new AvatarException(Resources.InvalidComponentIdText);
			}
			componentColorTable = binaryAssetColorTable.GetPrimaryColorTable();
		}
		return componentColorTable?.Colors;
	}

	public ComponentColors[] LoadColorTable(Stream assetStream)
	{
		BinaryAssetParseContext binaryAssetParseContext = new BinaryAssetParseContext();
		binaryAssetParseContext.m_CombinedComponentMask = AvatarComponentMasks.All;
		binaryAssetParseContext.m_CoordinateSystem = m_CoordinateSystem;
		binaryAssetParseContext.m_Target = null;
		binaryAssetParseContext.m_BodyType = AvatarGender.Both;
		binaryAssetParseContext.m_ResourceFactory = m_ResourceFactory;
		Colorb colorb = new Colorb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		ComponentInfo componentInfo = new ComponentInfo(Guid.Empty, AvatarComponentMasks.All, colorb, colorb, colorb);
		BinaryAssetColorTable binaryAssetColorTable = new BinaryAssetColorTable(Guid.Empty, AvatarComponentMasks.None);
		m_RequestedAssets = new List<BinaryAsset>();
		m_RequestedAssets.Add(binaryAssetColorTable);
		binaryAssetColorTable.Stream = assetStream;
		if (Validate(binaryAssetParseContext) != Result.Ok)
		{
			throw new AvatarException(Resources.InvalidComponentIdText);
		}
		return binaryAssetColorTable.GetPrimaryColorTable()?.Colors;
	}

	public AssetMetadataParser LoadMetadata(Guid assetId)
	{
		LoadMetaDataContext loadMetaDataContext = new LoadMetaDataContext();
		AssetsAsyncLoadContextCommon common = loadMetaDataContext.common;
		using (common.syncEvent = new AutoResetEvent(initialState: false))
		{
			m_dataManager.GetAssetAsync(assetId, LoadMetadataCompleted, loadMetaDataContext);
			common.syncEvent.WaitOne();
		}
		return loadMetaDataContext.parser;
	}

	public void LoadMetadataCompleted(object sender, DataRequestCompletedEventArgs e)
	{
		LoadMetaDataContext loadMetaDataContext = e.UserState as LoadMetaDataContext;
		lock (loadMetaDataContext.common.syncRequestLock)
		{
			if (e.Error != null)
			{
				loadMetaDataContext.parser = null;
			}
			else if (e.Cancelled)
			{
				loadMetaDataContext.parser = null;
			}
			else
			{
				loadMetaDataContext.parser = LoadMetadata(e.Result);
			}
			loadMetaDataContext.common.syncEvent.Set();
		}
	}

	public AssetMetadataParser LoadMetadata(Stream assetStream)
	{
		StructuredBinary structuredBinary = new StructuredBinary();
		if (!structuredBinary.Open(assetStream))
		{
			return null;
		}
		if (!(structuredBinary.Namespace == BinaryAsset.AvatarAssetGuid))
		{
			return null;
		}
		AssetMetadataParser assetMetadataParser = new AssetMetadataParser();
		if (!assetMetadataParser.LoadFromStrb(structuredBinary.Iterator))
		{
			return null;
		}
		return assetMetadataParser;
	}

	public void DownloadAssetCompleted(object sender, DataRequestCompletedEventArgs e)
	{
		AssetsAsyncLoadContext assetsAsyncLoadContext = e.UserState as AssetsAsyncLoadContext;
		lock (assetsAsyncLoadContext.common.syncRequestLock)
		{
			assetsAsyncLoadContext.common.numRequests--;
			if (e.Error != null)
			{
				assetsAsyncLoadContext.common.failed = true;
			}
			else if (e.Cancelled)
			{
				assetsAsyncLoadContext.common.failed = true;
			}
			else
			{
				m_RequestedAssets[assetsAsyncLoadContext.index].Stream = e.Result;
			}
			if (assetsAsyncLoadContext.common.numRequests == 0)
			{
				assetsAsyncLoadContext.common.syncEvent.Set();
			}
		}
	}

	public Result LoadAssets()
	{
		int count = m_RequestedAssets.Count;
		if (count == 0)
		{
			return Result.Ok;
		}
		AssetsAsyncLoadContextCommon assetsAsyncLoadContextCommon = new AssetsAsyncLoadContextCommon();
		assetsAsyncLoadContextCommon.syncRequestLock = new object();
		using (assetsAsyncLoadContextCommon.syncEvent = new AutoResetEvent(initialState: false))
		{
			assetsAsyncLoadContextCommon.numRequests = count;
			for (int i = 0; i < count; i++)
			{
				AssetsAsyncLoadContext assetsAsyncLoadContext = new AssetsAsyncLoadContext();
				assetsAsyncLoadContext.index = i;
				assetsAsyncLoadContext.common = assetsAsyncLoadContextCommon;
				m_dataManager.GetAssetAsync(m_RequestedAssets[i].AssetId, DownloadAssetCompleted, assetsAsyncLoadContext);
			}
			assetsAsyncLoadContextCommon.syncEvent.WaitOne();
		}
		return assetsAsyncLoadContextCommon.failed ? Result.Failed : Result.Ok;
	}

	public bool ValidateComponentMasks()
	{
		int count = m_RequestedAssets.Count;
		AvatarComponentMasks avatarComponentMasks = AvatarComponentMasks.None;
		for (int i = 0; i < count; i++)
		{
			AvatarComponentMasks componentMask = m_RequestedAssets[i].m_ComponentMask;
			if ((componentMask & avatarComponentMasks) != AvatarComponentMasks.None)
			{
				return false;
			}
			avatarComponentMasks |= componentMask;
		}
		return true;
	}

	public Result Validate(BinaryAssetParseContext ctx)
	{
		int count = m_RequestedAssets.Count;
		if (ctx.m_skeletonVersion == Skeleton.SkeletonVersion.Invalid && count > 0)
		{
			ctx.m_skeletonVersion = m_RequestedAssets[0].m_SkeletonVersion;
			if (ctx.m_skeletonVersion == Skeleton.SkeletonVersion.Invalid)
			{
				AssetMetadataParser metadata = m_RequestedAssets[0].GetMetadata();
				if (metadata != null)
				{
					ctx.m_skeletonVersion = metadata.AssetSkeletonVersion;
				}
			}
		}
		for (int i = 0; i < count; i++)
		{
			if (!m_RequestedAssets[i].Validate(ctx))
			{
				return Result.InvalidManifest;
			}
		}
		return Result.Ok;
	}

	public Result CreateAvatarFromCache(BinaryAssetParseContext ctx)
	{
		if (m_RequestedAssets.Count > 0)
		{
			if (ctx.m_skeletonVersion == Skeleton.SkeletonVersion.Invalid)
			{
				for (int i = 0; i < m_RequestedAssets.Count; i++)
				{
					CachedBinaryAsset cache = m_RequestedAssets[0].m_Cache;
					if (cache != null)
					{
						ctx.m_skeletonVersion = cache.GetAssetSkeletonVersion();
						break;
					}
				}
			}
			for (int j = 0; j < m_RequestedAssets.Count; j++)
			{
				if (!m_RequestedAssets[j].ValidateFromCache(ctx))
				{
					Logger.Log(new DebugLog(this, "Cached asset " + m_RequestedAssets[j].AssetId.ToString() + " cannot be used in current loading context."));
					return Result.Failed;
				}
			}
		}
		for (int k = 0; k < m_RequestedAssets.Count; k++)
		{
			if (!m_RequestedAssets[k].ProcessComponentsFromCache(ctx))
			{
				return Result.Failed;
			}
		}
		for (int l = 0; l < m_RequestedAssets.Count; l++)
		{
			if (!m_RequestedAssets[l].ProcessAssetsFromCache(ctx))
			{
				return Result.Failed;
			}
		}
		for (int m = 0; m < m_RequestedAssets.Count; m++)
		{
			if (!m_RequestedAssets[m].ProcessOverridesFromCache(ctx))
			{
				return Result.Failed;
			}
		}
		return Result.Ok;
	}

	public Result CreateAvatarFromStreams(BinaryAssetParseContext ctx)
	{
		for (int i = 0; i < m_RequestedAssets.Count; i++)
		{
			if (!m_RequestedAssets[i].ProcessComponentsFromStream(ctx))
			{
				return Result.Failed;
			}
		}
		for (int j = 0; j < m_RequestedAssets.Count; j++)
		{
			if (!m_RequestedAssets[j].ProcessAssetsFromStream(ctx))
			{
				return Result.Failed;
			}
		}
		for (int k = 0; k < m_RequestedAssets.Count; k++)
		{
			if (!m_RequestedAssets[k].ProcessOverridesFromStream(ctx))
			{
				return Result.Failed;
			}
		}
		return Result.Ok;
	}

	public bool CreateAssetsList(AvatarComponentMasks componentMask)
	{
		if (m_Manifest.m_Dirty)
		{
			m_Manifest.UpdateDependencies(m_dataManager);
		}
		m_RequestedAssets.Clear();
		bool flag = (componentMask & m_Manifest.BodyComponentInfo.m_ComponentInfo.m_ComponentMask) != 0;
		bool flag2 = (componentMask & m_Manifest.HeadComponentInfo.m_ComponentInfo.m_ComponentMask) != 0;
		if (flag)
		{
			BinaryAssetModel binaryAssetModel = new BinaryAssetModel(m_Manifest.BodyComponentInfo.m_ComponentInfo, 2, m_Manifest.BodyComponentInfo.m_ComponentInfo.m_ComponentMask);
			ShaderConstantOverride[] shaderConstantOverrides = binaryAssetModel.ShaderConstantOverrides;
			shaderConstantOverrides[0].m_Constant = ShaderParameterUsage.PixelConstantColorCustom0;
			shaderConstantOverrides[0].m_Value = m_Manifest.GetDynamicColor(DynamicColorType.Skin);
			shaderConstantOverrides[1].m_Constant = ShaderParameterUsage.PixelConstantColorRimLight;
			shaderConstantOverrides[1].m_Value = GetRimLight(m_Manifest.GetDynamicColor(DynamicColorType.Skin));
			m_RequestedAssets.Add(binaryAssetModel);
		}
		if (flag2)
		{
			BinaryAssetModel binaryAssetModel2 = new BinaryAssetModel(m_Manifest.HeadComponentInfo.m_ComponentInfo, 10, m_Manifest.HeadComponentInfo.m_ComponentInfo.m_ComponentMask);
			ShaderConstantOverride[] shaderConstantOverrides2 = binaryAssetModel2.ShaderConstantOverrides;
			shaderConstantOverrides2[0].m_Constant = ShaderParameterUsage.PixelConstantColorRimLight;
			shaderConstantOverrides2[0].m_Value = GetRimLight(m_Manifest.GetDynamicColor(DynamicColorType.Skin));
			int num = 9;
			while (--num >= 0)
			{
				shaderConstantOverrides2[num + 1].m_Constant = ManifestColorToParamUsageMap[num].use;
				shaderConstantOverrides2[num + 1].m_Value = m_Manifest.GetDynamicColor(ManifestColorToParamUsageMap[num].color);
			}
			m_RequestedAssets.Add(binaryAssetModel2);
			for (int i = 0; i < 6; i++)
			{
				DynamicTextureType dynamicTextureType = (DynamicTextureType)i;
				AvatarManifestV1.ReplacementTexture replacementTexture = m_Manifest.GetReplacementTexture(dynamicTextureType);
				if (replacementTexture.m_TextureAssetId != Guid.Empty)
				{
					BinaryAssetTexture item = new BinaryAssetTexture(dynamicTextureType, replacementTexture.m_TextureAssetId);
					m_RequestedAssets.Add(item);
				}
			}
			for (int j = 0; j < 3; j++)
			{
				BlendShapeType blendShapeType = (BlendShapeType)j;
				if (!(Guid.Empty == m_Manifest.GetBlendShape(blendShapeType)))
				{
					BinaryAssetBlendShape item2 = new BinaryAssetBlendShape(blendShapeType, m_Manifest.GetBlendShape(blendShapeType));
					m_RequestedAssets.Add(item2);
				}
			}
			if (m_Manifest.HeadComponentInfo.m_OverrideAsset != Guid.Empty)
			{
				AddOverride(m_Manifest.HeadComponentInfo.m_OverrideAsset);
			}
		}
		Vector4 dynamicColor = m_Manifest.GetDynamicColor(DynamicColorType.Hair);
		for (int k = 0; k < m_Manifest.m_ComponentInfo.Count; k++)
		{
			if ((m_Manifest.m_ComponentInfo[k].m_ComponentInfo.m_ComponentMask & componentMask) != AvatarComponentMasks.None)
			{
				AddModel(m_Manifest.m_ComponentInfo[k], dynamicColor);
			}
		}
		return true;
	}

	public void AddOverride(Guid guid)
	{
		BinaryAssetShapeOverride item = new BinaryAssetShapeOverride(guid, AvatarComponentMasks.None);
		m_RequestedAssets.Add(item);
	}

	public void AddModel(ComponentDescription info, Vector4 colorHair)
	{
		BinaryAssetModel binaryAssetModel = ((info.m_ComponentInfo.m_ComponentMask != AvatarComponentMasks.Carryable) ? new BinaryAssetModel(info.m_ComponentInfo, 4, info.m_ComponentInfo.m_ComponentMask) : new BinaryAssetCarryable(info.m_ComponentInfo, 4, AvatarComponentMasks.Carryable));
		ShaderConstantOverride[] shaderConstantOverrides = binaryAssetModel.ShaderConstantOverrides;
		if (info.m_ComponentInfo.m_ComponentMask == AvatarComponentMasks.Hair)
		{
			shaderConstantOverrides[0].m_Constant = ShaderParameterUsage.PixelConstantColorCustom0;
			shaderConstantOverrides[0].m_Value = colorHair;
			if (info.m_OverrideAsset != Guid.Empty && (m_Manifest.GetCombinedComponentMask() & AvatarComponentMasks.Hat) != AvatarComponentMasks.None)
			{
				binaryAssetModel.SetAssetId(info.m_OverrideAsset);
			}
		}
		else
		{
			if (info.m_OverrideAsset != Guid.Empty)
			{
				AddOverride(info.m_OverrideAsset);
			}
			shaderConstantOverrides[0].m_Constant = ShaderParameterUsage.PixelConstantColorCustom0;
			shaderConstantOverrides[0].m_Value = Utilities.ColorbToVector4(info.m_ComponentInfo.m_CustomColors0);
		}
		shaderConstantOverrides[1].m_Constant = ShaderParameterUsage.PixelConstantColorCustom1;
		shaderConstantOverrides[1].m_Value = Utilities.ColorbToVector4(info.m_ComponentInfo.m_CustomColors1);
		shaderConstantOverrides[2].m_Constant = ShaderParameterUsage.PixelConstantColorCustom2;
		shaderConstantOverrides[2].m_Value = Utilities.ColorbToVector4(info.m_ComponentInfo.m_CustomColors2);
		shaderConstantOverrides[3].m_Constant = ShaderParameterUsage.PixelConstantColorRimLight;
		shaderConstantOverrides[3].m_Value = DefaultRimLight;
		m_RequestedAssets.Add(binaryAssetModel);
	}

	public void DressDefaultClothes(AvatarComponentMasks componentMask)
	{
		if (!m_Manifest.ReplaceMissingComponents(fUseDefaultsIfNecessary: true))
		{
			throw new AvatarException(Resources.InvalidManifest1);
		}
		if (!m_Manifest.GetRequiredComponentsPresent())
		{
			throw new AvatarException(Resources.InvalidManifest2);
		}
		if ((componentMask & AvatarComponentMasks.Body) != AvatarComponentMasks.None && m_Manifest.BodyComponentInfo.m_ComponentInfo.m_ComponentMask != AvatarComponentMasks.Body)
		{
			throw new AvatarException(Resources.InvalidManifest3);
		}
		if ((componentMask & AvatarComponentMasks.Head) != AvatarComponentMasks.None)
		{
			if (m_Manifest.HeadComponentInfo.m_ComponentInfo.m_ComponentMask != AvatarComponentMasks.Head)
			{
				throw new AvatarException(Resources.InvalidManifest4);
			}
			if (!m_Manifest.GetRequiredBlendShapesPresent())
			{
				throw new AvatarException(Resources.InvalidManifest5);
			}
			if (!m_Manifest.GetRequiredReplacementTexturesPresent())
			{
				throw new AvatarException(Resources.InvalidManifest6);
			}
		}
	}

	public static Vector4 GetRimLight(Vector4 skinColour4)
	{
		Colorb colorb = Utilities.ColorbFromVector4(skinColour4);
		int num = 268435455;
		uint num2 = 0u;
		for (uint num3 = 0u; num3 < RimLightColours.Length; num3++)
		{
			ColorBlendPoint colorBlendPoint = RimLightColours[num3];
			int num4 = (colorBlendPoint.Skin.red - colorb.red) * (colorBlendPoint.Skin.red - colorb.red) + (colorBlendPoint.Skin.green - colorb.green) * (colorBlendPoint.Skin.green - colorb.green) + (colorBlendPoint.Skin.blue - colorb.blue) * (colorBlendPoint.Skin.blue - colorb.blue);
			if (num4 < num)
			{
				num2 = num3;
				num = num4;
			}
		}
		return RimLightColours[num2].Rim;
	}

	public void GenerateSkeleton(Skeleton.SkeletonVersion skeletonVersion)
	{
		m_Target.m_Skeleton = EmbeddedSkeleton.GetEmbeddedSkeleton(m_CoordinateSystem, skeletonVersion);
		switch (skeletonVersion)
		{
		case Skeleton.SkeletonVersion.Nxe:
			AvatarSkeletonScaling.ApplyTo(m_Manifest.BodyType, m_Manifest.WidthFactor, m_Manifest.HeightFactor, m_Target.m_Skeleton);
			break;
		case Skeleton.SkeletonVersion.Natal:
			AvatarSkeletonScalingV2.ApplyTo(m_Manifest.BodyType, m_Manifest.WidthFactor, m_Manifest.HeightFactor, m_Target.m_Skeleton);
			break;
		default:
			throw new AvatarException(Resources.InvalidSkeletonVersion);
		}
		if (m_Target.m_Carryable != null)
		{
			Vector3 scale = m_Target.m_Skeleton.Joints[0].Local.scale;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.scale = scale;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.X *= scale.X;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.Y *= scale.Y;
			m_Target.m_Carryable.m_Skeleton.Joints[0].Local.position.Z *= scale.Z;
		}
	}

	public void FinalizeAssets()
	{
		int num = 0;
		while (num < m_Target.m_Models.Count)
		{
			int num2 = m_Target.m_Models[num].m_Batches.Length;
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				num3 += m_Target.m_Models[num].m_Batches[i].Triangles.Length;
			}
			if (num3 > 0)
			{
				num++;
			}
			else
			{
				m_Target.m_Models.RemoveAt(num);
			}
		}
	}
}
