using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class WebClientsManager : IDisposable
{
	public const int defaultMaxClients = 1;

	public Queue<WebClient> m_qWebClientsStorage = new Queue<WebClient>();

	public List<WebClient> m_usedWebClientsStorage = new List<WebClient>();

	public int m_maxWebClients;

	public bool m_stopDownloading;

	public readonly object m_mgrLock = new object();

	public AutoResetEvent m_mgrWaitEvent = new AutoResetEvent(initialState: false);

	public OpenReadCompletedEventHandler m_downloadOpenReadCompleted;

	public DownloadProgressChangedEventHandler m_downloadProgressEventHandler;

	public bool m_disposed;

	public int MaxNumWebClients
	{
		get
		{
			return m_maxWebClients;
		}
		set
		{
			lock (m_mgrLock)
			{
				m_maxWebClients = value;
				while (m_usedWebClientsStorage.Count + m_qWebClientsStorage.Count > m_maxWebClients)
				{
					WebClient webClient = m_qWebClientsStorage.Dequeue();
					if (m_downloadOpenReadCompleted != null)
					{
						webClient.OpenReadCompleted -= m_downloadOpenReadCompleted;
					}
				}
			}
		}
	}

	public int NumDownloadingFiles => m_usedWebClientsStorage.Count;

	public WebClientsManager()
	{
		MaxNumWebClients = 1;
	}

	public WebClientsManager(int maxNumWebClients, OpenReadCompletedEventHandler downloadOpenReadCompleted)
	{
		MaxNumWebClients = maxNumWebClients;
		m_downloadOpenReadCompleted = downloadOpenReadCompleted;
	}

	public WebClientsManager(int maxNumWebClients, OpenReadCompletedEventHandler downloadOpenReadCompleted, DownloadProgressChangedEventHandler downloadProgressEventHandler)
	{
		MaxNumWebClients = maxNumWebClients;
		m_downloadOpenReadCompleted = downloadOpenReadCompleted;
		m_downloadProgressEventHandler = downloadProgressEventHandler;
	}

	public WebClient GetWebClient(bool waitForFreeClient)
	{
		bool lockTaken = false;
		object obj = null;
		try
		{
			Monitor.Enter(obj = m_mgrLock, ref lockTaken);
			if (m_qWebClientsStorage.Count > 0)
			{
				WebClient webClient = m_qWebClientsStorage.Dequeue();
				m_usedWebClientsStorage.Add(webClient);
				return webClient;
			}
			if (m_usedWebClientsStorage.Count < m_maxWebClients)
			{
				WebClient webClient2 = new WebClient();
				if (webClient2 != null)
				{
					m_usedWebClientsStorage.Add(webClient2);
					if (m_downloadOpenReadCompleted != null)
					{
						webClient2.OpenReadCompleted += m_downloadOpenReadCompleted;
					}
					if (m_downloadProgressEventHandler != null)
					{
						webClient2.DownloadProgressChanged += m_downloadProgressEventHandler;
					}
				}
				return webClient2;
			}
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(obj);
			}
		}
		if (!waitForFreeClient)
		{
			return null;
		}
		while (!m_stopDownloading)
		{
			while (m_qWebClientsStorage.Count == 0)
			{
				m_mgrWaitEvent.WaitOne();
				if (m_stopDownloading)
				{
					m_stopDownloading = false;
					return null;
				}
			}
			bool lockTaken2 = false;
			try
			{
				Monitor.Enter(obj = m_mgrLock, ref lockTaken2);
				if (m_qWebClientsStorage.Count > 0)
				{
					WebClient webClient3 = m_qWebClientsStorage.Dequeue();
					m_usedWebClientsStorage.Add(webClient3);
					return webClient3;
				}
			}
			finally
			{
				if (lockTaken2)
				{
					Monitor.Exit(obj);
				}
			}
		}
		return null;
	}

	public bool ReleaseWebClient(WebClient webClient)
	{
		lock (m_mgrLock)
		{
			if (webClient == null)
			{
				throw new ArgumentNullException("webClient");
			}
			bool result = m_usedWebClientsStorage.Remove(webClient);
			if (m_usedWebClientsStorage.Count + m_qWebClientsStorage.Count > m_maxWebClients)
			{
				if (m_downloadOpenReadCompleted != null)
				{
					webClient.OpenReadCompleted -= m_downloadOpenReadCompleted;
				}
				if (m_downloadProgressEventHandler != null)
				{
					webClient.DownloadProgressChanged -= m_downloadProgressEventHandler;
				}
				if (m_usedWebClientsStorage.Count + m_qWebClientsStorage.Count == 0)
				{
					m_stopDownloading = true;
					m_mgrWaitEvent.Set();
				}
			}
			else
			{
				m_qWebClientsStorage.Enqueue(webClient);
				if (m_mgrWaitEvent != null)
				{
					m_mgrWaitEvent.Set();
				}
			}
			return result;
		}
	}

	public void CancelDownloading()
	{
		lock (m_mgrWaitEvent)
		{
			foreach (WebClient item in m_usedWebClientsStorage)
			{
				item.CancelAsync();
			}
		}
	}

	public void StopDownloading()
	{
		CancelDownloading();
		m_stopDownloading = true;
		m_mgrWaitEvent.Set();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Dispose(bool disposing)
	{
		if (m_disposed)
		{
			return;
		}
		StopDownloading();
		if (!disposing)
		{
			return;
		}
		foreach (WebClient item in m_qWebClientsStorage)
		{
			if (m_downloadOpenReadCompleted != null)
			{
				item.OpenReadCompleted -= m_downloadOpenReadCompleted;
			}
			if (m_downloadProgressEventHandler != null)
			{
				item.DownloadProgressChanged -= m_downloadProgressEventHandler;
			}
		}
		m_qWebClientsStorage.Clear();
		foreach (WebClient item2 in m_usedWebClientsStorage)
		{
			if (m_downloadOpenReadCompleted != null)
			{
				item2.OpenReadCompleted -= m_downloadOpenReadCompleted;
			}
			if (m_downloadProgressEventHandler != null)
			{
				item2.DownloadProgressChanged -= m_downloadProgressEventHandler;
			}
		}
		m_usedWebClientsStorage.Clear();
		m_mgrWaitEvent.Close();
		m_mgrWaitEvent = null;
		m_disposed = true;
	}
}
