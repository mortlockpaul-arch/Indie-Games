using System;
using System.Collections.Generic;

namespace Microsoft.XboxLive.Avatars.Internal;

public class DataProvider
{
	public DataManagerBase dataManager;

	public IDataProvider dataProvider;

	public Queue<IDataProvider> providers = new Queue<IDataProvider>();

	public IDataProvider ProcessingProvider => dataProvider;

	public DataProvider()
	{
	}

	public DataProvider(Queue<IDataProvider> dataProviders)
	{
		foreach (IDataProvider dataProvider in dataProviders)
		{
			providers.Enqueue(dataProvider);
		}
	}

	public void AddDataProvider(IDataProvider dataProvider)
	{
		if (dataProvider == null)
		{
			throw new ArgumentNullException("dataProvider");
		}
		providers.Enqueue(dataProvider);
	}

	public void ProccessRequest(DataRequest request, DataManagerBase dataManager)
	{
		while (providers.Count > 0)
		{
			dataProvider = providers.Dequeue();
			if (dataProvider != null)
			{
				this.dataManager = dataManager;
				dataProvider.GetDataAsync(request, this);
				break;
			}
		}
	}

	public void RequestProcessed(DataRequestCompletedEventArgs eventArguments, DataRequest dataRequest)
	{
		if (dataManager == null)
		{
			throw new InvalidOperationException();
		}
		if (eventArguments.Error != null && providers.Count > 0)
		{
			dataManager.EnqueueRequest(dataRequest, DownloadPriority.High);
			return;
		}
		dataManager.RequestProcessed(eventArguments, dataRequest);
		if (dataRequest.Handler != null)
		{
			dataRequest.Handler(null, eventArguments);
		}
		dataProvider = null;
	}

	public void CancelRequest()
	{
		if (dataProvider != null)
		{
			dataProvider.CancelAsync();
		}
	}
}
