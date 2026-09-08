using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.XboxLive.Avatars.Internal;

public class EmbeddedAssetDataProvider : IDataProvider
{
	public Assembly resourceAssembly;

	public string m_stockAssetAddressFormat;

	public string m_nonStockAssetAddressFormat;

	public string m_manifestServiceAddressFormat;

	public EmbeddedAssetDataProvider(Assembly assembly, string stockAssetAddressFormat, string nonStockAssetAddressFormat)
	{
		m_stockAssetAddressFormat = stockAssetAddressFormat;
		m_nonStockAssetAddressFormat = nonStockAssetAddressFormat;
		resourceAssembly = assembly;
	}

	public EmbeddedAssetDataProvider(Assembly assembly, string manifestServiceAddressFormat)
	{
		m_manifestServiceAddressFormat = manifestServiceAddressFormat;
		resourceAssembly = assembly;
	}

	public EmbeddedAssetDataProvider(Assembly assembly)
	{
		resourceAssembly = assembly;
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
		if (AssetLoader.IsStockAsset(avatarAssetGuid))
		{
			return MakeStockAssetAddress(avatarAssetGuid.ToString());
		}
		return MakeNonStockAssetAddress(avatarAssetGuid.ToString(), AssetLoader.GetTitleIdFromAssetId(avatarAssetGuid));
	}

	public void GetDataAsync(DataRequest request, DataProvider dataProvider)
	{
		string text = null;
		if (request is DataRequestGuid)
		{
			DataRequestGuid dataRequestGuid = request as DataRequestGuid;
			text = MakeAssetAddressFromGuid(dataRequestGuid.DataId);
		}
		else if (request is DataRequestString)
		{
			DataRequestString dataRequestString = request as DataRequestString;
			text = ((m_manifestServiceAddressFormat == null) ? MakeStockAssetAddress(dataRequestString.DataId) : MakeManifestAddress(dataRequestString.DataId));
		}
		DataRequestCompletedEventArgs e;
		if (text == null)
		{
			e = new DataRequestCompletedEventArgs(new ArgumentNullException("dataId", "Address cannot be null"), canceled: false, request.Context);
		}
		if (resourceAssembly == null)
		{
			e = new DataRequestCompletedEventArgs(new ArgumentNullException("resourceAssembly", "resourceAssembly cannot be null"), canceled: false, request.Context);
		}
		try
		{
			Stream manifestResourceStream = resourceAssembly.GetManifestResourceStream(text);
			if (manifestResourceStream != null)
			{
				e = new DataRequestCompletedEventArgs(null, canceled: false, request.Context);
				e.Result = manifestResourceStream;
			}
			else
			{
				e = new DataRequestCompletedEventArgs(new InvalidOperationException("Failed to obtain resource."), canceled: false, request.Context);
			}
		}
		catch (Exception error)
		{
			e = new DataRequestCompletedEventArgs(error, canceled: false, request.Context);
		}
		dataProvider.RequestProcessed(e, request);
	}

	public void CancelAsync()
	{
	}
}
