using System;
using System.Globalization;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class NetDataProvider
{
	public string m_stockAssetAddressFormat;

	public string m_nonStockAssetAddressFormat;

	public string m_manifestServiceAddressFormat;

	public NetDataProvider(string stockAssetAddressFormat, string nonStockAssetAddressFormat, string manifestServiceAddressFormat)
	{
		m_stockAssetAddressFormat = stockAssetAddressFormat;
		m_nonStockAssetAddressFormat = nonStockAssetAddressFormat;
		m_manifestServiceAddressFormat = manifestServiceAddressFormat;
	}

	public static Stream DuplicateStream(Stream source, bool seekToBegin)
	{
		lock (source)
		{
			Stream stream = new MemoryStream();
			byte[] array = new byte[32768];
			if (seekToBegin)
			{
				source.Seek(0L, SeekOrigin.Begin);
			}
			while (true)
			{
				bool flag = true;
				int num = source.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				stream.Write(array, 0, num);
			}
			stream.Seek(0L, SeekOrigin.Begin);
			return stream;
		}
	}

	public string MakeStockAssetAddress(string asset)
	{
		return string.Format(CultureInfo.InvariantCulture, m_stockAssetAddressFormat, new object[1] { asset });
	}

	public string MakeNonStockAssetAddress(string asset, string titleId)
	{
		return string.Format(CultureInfo.InvariantCulture, m_nonStockAssetAddressFormat, new object[2] { titleId, asset });
	}

	public string MakeManifestAddress(string gamertag)
	{
		return string.Format(CultureInfo.InvariantCulture, m_manifestServiceAddressFormat, new object[1] { gamertag });
	}

	public string MakeAssetAddressFromGuid(Guid avatarAssetGuid)
	{
		string text = ((!AssetLoader.IsStockAsset(avatarAssetGuid)) ? MakeNonStockAssetAddress(avatarAssetGuid.ToString(), AssetLoader.GetTitleIdFromAssetId(avatarAssetGuid)) : MakeStockAssetAddress(avatarAssetGuid.ToString()));
		return text.ToLower(CultureInfo.InvariantCulture);
	}

	public string GetAddress(DataRequest request)
	{
		string result = null;
		if (request is DataRequestGuid)
		{
			DataRequestGuid dataRequestGuid = request as DataRequestGuid;
			result = MakeAssetAddressFromGuid(dataRequestGuid.DataId);
		}
		else if (request is DataRequestString)
		{
			DataRequestString dataRequestString = request as DataRequestString;
			result = ((m_manifestServiceAddressFormat == null) ? MakeStockAssetAddress(dataRequestString.DataId) : MakeManifestAddress(dataRequestString.DataId));
		}
		return result;
	}

	public string GetAddressFormat(DataRequest request)
	{
		string result = null;
		if (request is DataRequestGuid)
		{
			DataRequestGuid dataRequestGuid = request as DataRequestGuid;
			result = GetAssetAddressFromGuid(dataRequestGuid.DataId);
		}
		else if (request is DataRequestString)
		{
			DataRequestString dataRequestString = request as DataRequestString;
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
}
