#define DEBUG
using System;
using System.Diagnostics;
using System.Net;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetUrlDataProvider : NetDataProvider, IDataProvider
{
	public class AssetUrlDataProviderContext
	{
		public DataRequest Request { get; set; }

		public DataProvider Provider { get; set; }

		public WebClient Client { get; set; }

		public string Address { get; set; }

		public AssetUrlDataProviderContext(DataRequest request, WebClient client, DataProvider dataProvider)
		{
			Request = request;
			Client = client;
			Provider = dataProvider;
		}
	}

	public const int numWebClients = 10;

	public static WebClientsManager m_webClients;

	public static int MaxSimultaneousDownloads
	{
		get
		{
			if (m_webClients == null)
			{
				InitializeWebClientsManager(10);
			}
			return m_webClients.MaxNumWebClients;
		}
		set
		{
			if (m_webClients == null)
			{
				InitializeWebClientsManager(value);
			}
			else
			{
				m_webClients.MaxNumWebClients = value;
			}
		}
	}

	public AssetUrlDataProvider(string stockAssetAddressFormat, string nonStockAssetAddressFormat)
		: base(stockAssetAddressFormat, nonStockAssetAddressFormat, null)
	{
		InitializeWebClientsManager(10);
	}

	public AssetUrlDataProvider(string manifestServiceAddressFormat)
		: base(null, null, manifestServiceAddressFormat)
	{
		InitializeWebClientsManager(10);
	}

	public static void InitializeWebClientsManager(int maxNumWebClients)
	{
		if (m_webClients == null)
		{
			m_webClients = new WebClientsManager(maxNumWebClients, DownloadOpenReadCompleted, null);
		}
	}

	public static void DownloadOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
	{
		AssetUrlDataProviderContext assetUrlDataProviderContext = e.UserState as AssetUrlDataProviderContext;
		DataRequest request = assetUrlDataProviderContext.Request;
		DebugLog debugLog = new DebugLog();
		debugLog.Sender = assetUrlDataProviderContext.Provider.ProcessingProvider;
		debugLog.Message = "Download completed for " + assetUrlDataProviderContext.Address + ".";
		Logger.Log(debugLog);
		DataRequestCompletedEventArgs e2;
		if (e.Error != null)
		{
			e2 = new DataRequestCompletedEventArgs(e.Error, canceled: false, request.Context);
			ErrorLog errorLog = new ErrorLog(e.Error);
			errorLog.Sender = assetUrlDataProviderContext.Provider.ProcessingProvider;
			errorLog.Message = "Download failed for " + assetUrlDataProviderContext.Address + ".";
			Logger.Log(errorLog);
		}
		else if (e.Cancelled)
		{
			e2 = new DataRequestCompletedEventArgs(null, canceled: true, request.Context);
			DebugLog debugLog2 = new DebugLog();
			debugLog2.Sender = assetUrlDataProviderContext.Provider.ProcessingProvider;
			debugLog2.Message = "Download cancelled for " + assetUrlDataProviderContext.Address + ".";
			Logger.Log(debugLog2);
		}
		else
		{
			e2 = new DataRequestCompletedEventArgs(null, canceled: false, request.Context);
			if (!e.Result.CanSeek)
			{
				e2.Result = NetDataProvider.DuplicateStream(e.Result, seekToBegin: false);
			}
			else
			{
				e2.Result = e.Result;
			}
			e2.SourceAddress = assetUrlDataProviderContext.Address;
		}
		m_webClients.ReleaseWebClient((WebClient)sender);
		DataProvider provider = assetUrlDataProviderContext.Provider;
		provider.RequestProcessed(e2, request);
	}

	public void ReleaseWebClient(AssetUrlDataProviderContext urlProviderContext)
	{
		WebClient client = urlProviderContext.Client;
		m_webClients.ReleaseWebClient(client);
	}

	public void GetDataAsync(DataRequest request, DataProvider dataProvider)
	{
		WebClient webClient = m_webClients.GetWebClient(waitForFreeClient: true);
		if (webClient == null)
		{
			dataProvider.RequestProcessed(new DataRequestCompletedEventArgs(new InvalidOperationException(), canceled: false, request.Context), request);
		}
		AssetUrlDataProviderContext assetUrlDataProviderContext = new AssetUrlDataProviderContext(request, webClient, dataProvider);
		try
		{
			assetUrlDataProviderContext.Address = GetAddress(request);
			Debug.WriteLine(assetUrlDataProviderContext.Address);
			webClient.OpenReadAsync(new Uri(assetUrlDataProviderContext.Address), assetUrlDataProviderContext);
		}
		catch (Exception error)
		{
			ReleaseWebClient(assetUrlDataProviderContext);
			ErrorLog errorLog = new ErrorLog(error);
			errorLog.Sender = this;
			errorLog.Message = "Failed to open a readable stream for data request.";
			Logger.Log(errorLog);
			dataProvider.RequestProcessed(new DataRequestCompletedEventArgs(error, canceled: false, request.Context), request);
		}
	}

	public void CancelAsync()
	{
		m_webClients.StopDownloading();
	}
}
