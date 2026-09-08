using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Microsoft.XboxLive.Avatars.Internal;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

public class AvatarResources
{
	private const int defaultRequestTimeout = 120000;

	private static object assetAccessLock;

	private static AssetDataManager dataManager;

	private static AssetLoader assetLoader;

	private static GraphicsDevice defaultGraphicsDevice;

	internal static AvatarRenderableModelCache AvatarRenderableModelCache { get; private set; }

	internal static int UIThreadId { get; private set; }

	internal static string ManifestServiceUrlFormat { get; private set; }

	public static Assembly AnimationResourceAssembly { get; private set; }

	public static string AnimationResourcePath { get; private set; }

	public static Assembly AssetResourceAssembly { get; private set; }

	public static string AssetResourcePath { get; private set; }

	public static int AssetVersion { get; private set; }

	public static int WebRequestTimeout { get; private set; }

	internal static GraphicsDevice DefaultGraphicsDevice
	{
		get
		{
			if (defaultGraphicsDevice == null)
			{
				throw new Exception("No default GraphicsDevice set on AvatarResources.  In order to use the default GraphicsDevice you must call AvatarResource.SetDefaultGraphicsDevice.");
			}
			return defaultGraphicsDevice;
		}
	}

	internal static AssetDataManager DataMangager
	{
		get
		{
			if (dataManager == null)
			{
				lock (assetAccessLock)
				{
					if (dataManager == null)
					{
						InitializeDataManager();
					}
				}
			}
			return dataManager;
		}
	}

	internal static AssetLoader AssetLoader
	{
		get
		{
			if (assetLoader == null)
			{
				lock (assetAccessLock)
				{
					if (assetLoader == null)
					{
						assetLoader = new AssetLoader(DataMangager, new ResourceFactory(), CoordinateSystem.RightHanded);
						AssetLoader.EnableAssetCaching();
					}
				}
			}
			return assetLoader;
		}
	}

	public static void SetAnimationResourceLocation(Assembly assembly, string path)
	{
		AnimationResourceAssembly = assembly;
		AnimationResourcePath = path;
	}

	public static void SetAssetResourceLocation(Assembly assembly, string path)
	{
		AssetResourceAssembly = assembly;
		AssetResourcePath = path;
		Cleanup();
	}

	public static void SetDefaultGraphicsDevice(GraphicsDevice defaultGraphicsDevice)
	{
		UIThreadId = Thread.CurrentThread.ManagedThreadId;
		AvatarResources.defaultGraphicsDevice = defaultGraphicsDevice;
		if (AvatarRenderableModelCache != null)
		{
			AvatarRenderableModelCache.CleanCache();
		}
	}

	private static void SetAssetVersion(int assetVersion)
	{
		if (assetVersion < 1 || assetVersion > 2)
		{
			throw new ArgumentException("AssetVersion must be 1 or 2.", "AssetVersion");
		}
		if (assetVersion != AssetVersion)
		{
			AssetVersion = assetVersion;
			AssetLoader.ClearCaches();
			if (AvatarRenderableModelCache != null)
			{
				AvatarRenderableModelCache.CleanCache();
			}
			Cleanup();
		}
	}

	public static void SetRequestTimeout(int timeout)
	{
		if (timeout < -1)
		{
			throw new ArgumentException("Invalid timeout value");
		}
		WebRequestTimeout = timeout;
		Cleanup();
	}

	static AvatarResources()
	{
		assetAccessLock = new object();
		AssetVersion = 2;
		WebRequestTimeout = 120000;
	}

	public static void ClearCache()
	{
		AssetStorageDataProvider.CleanCache();
		AssetLoader.ClearCaches();
		if (AvatarRenderableModelCache != null)
		{
			AvatarRenderableModelCache.CleanCache();
		}
	}

	public static void Cleanup()
	{
		lock (assetAccessLock)
		{
			if (dataManager != null)
			{
				dataManager.Cleanup();
			}
			dataManager = null;
			assetLoader = null;
		}
	}

	public static void EnableRenderableModelCaching()
	{
		if (AvatarRenderableModelCache != null)
		{
			return;
		}
		lock (assetAccessLock)
		{
			if (AvatarRenderableModelCache == null)
			{
				AvatarRenderableModelCache = new AvatarRenderableModelCache(DefaultGraphicsDevice);
			}
		}
	}

	public static void DisableRenderableModelCaching()
	{
		if (AvatarRenderableModelCache == null)
		{
			return;
		}
		lock (assetAccessLock)
		{
			if (AvatarRenderableModelCache != null)
			{
				AvatarRenderableModelCache.Dispose();
			}
			AvatarRenderableModelCache = null;
		}
	}

	private static void InitializeDataManager()
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Stream stream = TitleContainer.OpenStream("XboxLIVESettings.xml");
			XElement xElement = XElement.Load(stream);
			IEnumerable<KeyValuePair<string, string>> enumerable = from element in xElement.Descendants("Item")
				select new KeyValuePair<string, string>(element.Element("Key").Value, element.Element("Value").Value);
			foreach (KeyValuePair<string, string> item in enumerable)
			{
				dictionary.Add(item.Key, item.Value);
			}
			if (dictionary.ContainsKey("3DAvatarAssetVersion"))
			{
				SetAssetVersion(int.Parse(dictionary["3DAvatarAssetVersion"]));
			}
			ManifestServiceUrlFormat = dictionary["3DAvatarReadURL"] + "/?gt={0}";
		}
		catch (Exception innerException)
		{
			throw new Exception("Invalid or no XboxLIVESettings.xml file was found in the root of the application.", innerException);
		}
		string stockAssetAddressFormat = ((AssetVersion == 1) ? "http://download.xboxlive.com/content/584d07d1/{0}.bin" : "http://download.xboxlive.com/content/584d07d1/v2/{0}.bin");
		string nonStockAssetAddressFormat = ((AssetVersion == 1) ? "http://download.xboxlive.com/content/{0}/avataritems/{1}.bin" : "http://download.xboxlive.com/content/{0}/avataritems/v2/{1}.bin");
		AssetDataManager assetDataManager = new AssetDataManager();
		assetDataManager.AddManifestProvider(new AssetStorageDataProvider(ManifestServiceUrlFormat));
		assetDataManager.AddManifestProvider(new AssetUrlDataProvider(ManifestServiceUrlFormat));
		if (AssetResourceAssembly != null && AssetResourcePath != null)
		{
			assetDataManager.AddAssetProvider(new EmbeddedAssetDataProvider(AssetResourceAssembly, AssetResourcePath + ".{0}.bin", AssetResourcePath + ".{0}.bin"));
		}
		assetDataManager.AddAssetProvider(new AssetWebRequestProvider(stockAssetAddressFormat, nonStockAssetAddressFormat, WebRequestTimeout));
		dataManager = assetDataManager;
	}

	public static AvatarAnimation LoadAnimation(Stream stream)
	{
		return AvatarAnimation.Load(stream);
	}
}
