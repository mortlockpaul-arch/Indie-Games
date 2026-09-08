#define DEBUG
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetStorageDataProvider : IDataProvider
{
	public static AssetLocalCache s_assetLocalCache;

	public string m_stockAssetAddressFormat;

	public string m_nonStockAssetAddressFormat;

	public string m_manifestServiceAddressFormat;

	public AssetLocalCache LocalCache => s_assetLocalCache;

	public AssetStorageDataProvider()
	{
		if (s_assetLocalCache == null)
		{
			s_assetLocalCache = new AssetLocalCache();
		}
	}

	public AssetStorageDataProvider(string manifestServiceAddressFormat)
	{
		m_manifestServiceAddressFormat = manifestServiceAddressFormat;
		if (s_assetLocalCache == null)
		{
			s_assetLocalCache = new AssetLocalCache();
		}
	}

	public AssetStorageDataProvider(string stockAssetAddressFormat, string nonStockAssetAddressFormat)
	{
		m_stockAssetAddressFormat = stockAssetAddressFormat;
		m_nonStockAssetAddressFormat = nonStockAssetAddressFormat;
		if (s_assetLocalCache == null)
		{
			s_assetLocalCache = new AssetLocalCache();
		}
	}

	public static string GetFullIdAddress(string addressFormat, string assetId)
	{
		string result = null;
		try
		{
			string extension = ((uint)addressFormat.ToLower(CultureInfo.InvariantCulture).GetHashCode()).ToString(CultureInfo.InvariantCulture);
			result = Path.ChangeExtension(Path.GetFileName(assetId), extension);
		}
		catch (ArgumentException)
		{
			Debug.WriteLine("Invalid addressFormat {0} or assetId {1}, addressFormat, assetId");
		}
		return result;
	}

	public static bool IncreaseCacheQuotaTo(long size)
	{
		if (s_assetLocalCache == null)
		{
			return false;
		}
		return s_assetLocalCache.IncreaseQuotaTo(size);
	}

	public static void CleanCache()
	{
		AssetLocalCache.Clean();
	}

	public string GetAddressFormat(DataRequest request)
	{
		string result = null;
		if (request is DataRequestGuid)
		{
			DataRequestGuid dataRequestGuid = (DataRequestGuid)request;
			result = GetAssetAddressFromGuid(dataRequestGuid.DataId);
		}
		else if (request is DataRequestString)
		{
			result = ((m_manifestServiceAddressFormat == null) ? m_stockAssetAddressFormat : m_manifestServiceAddressFormat);
		}
		return result;
	}

	public string GetAssetAddressFromGuid(Guid avatarAssetGuid)
	{
		if (AssetLoader.IsStockAsset(avatarAssetGuid))
		{
			return m_stockAssetAddressFormat;
		}
		return m_nonStockAssetAddressFormat;
	}

	public void GetDataAsync(DataRequest request, DataProvider dataProvider)
	{
		string assetId = null;
		if (request is DataRequestGuid)
		{
			assetId = ((DataRequestGuid)request).DataId.ToString();
		}
		else if (request is DataRequestString)
		{
			assetId = ((DataRequestString)request).DataId.ToString();
		}
		string addressFormat = GetAddressFormat(request);
		string fullIdAddress = GetFullIdAddress(addressFormat, assetId);
		DataRequestCompletedEventArgs e;
		if (fullIdAddress == null)
		{
			e = new DataRequestCompletedEventArgs(new ArgumentException("Invalid data request", "request"), canceled: false, request.Context);
		}
		if (s_assetLocalCache == null)
		{
			e = new DataRequestCompletedEventArgs(new InvalidOperationException("Local cache is not initialized"), canceled: false, request.Context);
		}
		long ticks = DateTime.Now.Ticks;
		Stream stream = s_assetLocalCache.TryOpen(fullIdAddress);
		if (stream != null)
		{
			e = new DataRequestCompletedEventArgs(null, canceled: false, request.Context);
			e.Result = stream;
			Debug.WriteLine("Reading from Cache : {0} in {1} ms", fullIdAddress, (float)(DateTime.Now.Ticks - ticks) / 10000f);
		}
		else
		{
			e = new DataRequestCompletedEventArgs(new InvalidOperationException("Failed to obtain resource."), canceled: false, request.Context);
		}
		dataProvider.RequestProcessed(e, request);
	}

	public void CancelAsync()
	{
	}
}
