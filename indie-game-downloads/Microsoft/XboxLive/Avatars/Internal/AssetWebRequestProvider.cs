#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AssetWebRequestProvider : NetDataProvider, IDataProvider
{
	public class RequestState
	{
		public WebRequest request;

		public ManualResetEvent workFinished = new ManualResetEvent(initialState: false);

		public DataRequest dataRequest { get; set; }

		public DataProvider Provider { get; set; }

		public string Address { get; set; }

		public RequestState(DataRequest dataRequestObj, DataProvider dataProvider)
		{
			request = null;
			dataRequest = dataRequestObj;
			Provider = dataProvider;
		}
	}

	public int requestTimeout = -1;

	public AssetWebRequestProvider(string stockAssetAddressFormat, string nonStockAssetAddressFormat)
		: base(stockAssetAddressFormat, nonStockAssetAddressFormat, null)
	{
	}

	public AssetWebRequestProvider(string stockAssetAddressFormat, string nonStockAssetAddressFormat, int millisecondsRequestTimeout)
		: base(stockAssetAddressFormat, nonStockAssetAddressFormat, null)
	{
		requestTimeout = millisecondsRequestTimeout;
	}

	public AssetWebRequestProvider(string manifestServiceAddressFormat)
		: base(null, null, manifestServiceAddressFormat)
	{
	}

	public AssetWebRequestProvider(string manifestServiceAddressFormat, int millisecondsRequestTimeout)
		: base(null, null, manifestServiceAddressFormat)
	{
		requestTimeout = millisecondsRequestTimeout;
	}

	public void GetDataAsync(DataRequest request, DataProvider dataProvider)
	{
		try
		{
			string address = GetAddress(request);
			Debug.WriteLine("Create webrequest for {0}.", address);
			WebRequest webRequest = WebRequest.Create(address);
			RequestState requestState = new RequestState(request, dataProvider);
			requestState.request = webRequest;
			requestState.Address = address;
			IAsyncResult asyncResult = webRequest.BeginGetResponse(RespCallback, requestState);
			if (!requestState.workFinished.WaitOne(requestTimeout))
			{
				webRequest.Abort();
				Debug.WriteLine("Request aborted for {0}.", address);
			}
			lock (requestState)
			{
				requestState.workFinished.Close();
				requestState.workFinished = null;
			}
		}
		catch (WebException error)
		{
			ErrorLog errorLog = new ErrorLog(error);
			errorLog.Sender = this;
			errorLog.Message = "Failed to open a readable stream for data request.";
			Logger.Log(errorLog);
			dataProvider.RequestProcessed(new DataRequestCompletedEventArgs(error, canceled: false, request.Context), request);
		}
		catch (Exception error2)
		{
			ErrorLog errorLog2 = new ErrorLog(error2);
			errorLog2.Sender = this;
			Logger.Log(errorLog2);
			dataProvider.RequestProcessed(new DataRequestCompletedEventArgs(error2, canceled: false, request.Context), request);
		}
	}

	public static void RespCallback(IAsyncResult asynchronousResult)
	{
		RequestState requestState = (RequestState)asynchronousResult.AsyncState;
		DataRequestCompletedEventArgs e;
		try
		{
			WebResponse webResponse = null;
			HttpWebRequest httpWebRequest = (HttpWebRequest)requestState.request;
			if (httpWebRequest != null && httpWebRequest.HaveResponse)
			{
				webResponse = httpWebRequest.EndGetResponse(asynchronousResult);
			}
			if (webResponse != null)
			{
				Stream responseStream = webResponse.GetResponseStream();
				if (responseStream != null)
				{
					e = new DataRequestCompletedEventArgs(null, canceled: false, requestState.dataRequest.Context);
					e.Result = NetDataProvider.DuplicateStream(responseStream, seekToBegin: false);
					e.SourceAddress = requestState.Address;
					responseStream.Close();
				}
				else
				{
					e = new DataRequestCompletedEventArgs(new InvalidOperationException("Failed to get Response Stream."), canceled: false, requestState.dataRequest.Context);
				}
				webResponse.Close();
			}
			else
			{
				e = new DataRequestCompletedEventArgs(new InvalidOperationException("Failed to get Web Response."), canceled: false, requestState.dataRequest.Context);
			}
		}
		catch (WebException error)
		{
			e = new DataRequestCompletedEventArgs(error, canceled: false, requestState.dataRequest.Context);
		}
		catch (Exception error2)
		{
			e = new DataRequestCompletedEventArgs(error2, canceled: false, requestState.dataRequest.Context);
		}
		finally
		{
			lock (requestState)
			{
				if (requestState.workFinished != null)
				{
					requestState.workFinished.Set();
				}
			}
		}
		DataProvider provider = requestState.Provider;
		provider.RequestProcessed(e, requestState.dataRequest);
	}

	public void CancelAsync()
	{
	}
}
