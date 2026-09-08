using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class DataManagerBase
{
	public int m_numProcessingRequests = 0;

	public Thread m_workerThread = null;

	public bool m_quitWorkerThread = false;

	public AutoResetEvent m_newRequestEvent = new AutoResetEvent(initialState: false);

	public AutoResetEvent m_workerWaitExitEvent = new AutoResetEvent(initialState: false);

	public readonly object m_requestQueueLock = new object();

	public Queue<DataRequest> m_qNormalPriorityRequests = new Queue<DataRequest>();

	public Queue<DataRequest> m_qHighPriorityRequests = new Queue<DataRequest>();

	public List<DataRequest> m_processingRequests = new List<DataRequest>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public DownloadProgressEventHandler DownloadProgress;

	public int NumberFilesToDownload => m_numProcessingRequests;

	public event DownloadProgressEventHandler DownloadProgress
	{
		[CompilerGenerated]
		add
		{
			DownloadProgressEventHandler downloadProgressEventHandler = this.DownloadProgress;
			DownloadProgressEventHandler downloadProgressEventHandler2;
			do
			{
				downloadProgressEventHandler2 = downloadProgressEventHandler;
				DownloadProgressEventHandler value2 = (DownloadProgressEventHandler)Delegate.Combine(downloadProgressEventHandler2, value);
				downloadProgressEventHandler = Interlocked.CompareExchange(ref this.DownloadProgress, value2, downloadProgressEventHandler2);
			}
			while ((object)downloadProgressEventHandler != downloadProgressEventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			DownloadProgressEventHandler downloadProgressEventHandler = this.DownloadProgress;
			DownloadProgressEventHandler downloadProgressEventHandler2;
			do
			{
				downloadProgressEventHandler2 = downloadProgressEventHandler;
				DownloadProgressEventHandler value2 = (DownloadProgressEventHandler)Delegate.Remove(downloadProgressEventHandler2, value);
				downloadProgressEventHandler = Interlocked.CompareExchange(ref this.DownloadProgress, value2, downloadProgressEventHandler2);
			}
			while ((object)downloadProgressEventHandler != downloadProgressEventHandler2);
		}
	}

	public void InitializeThread()
	{
		CreateWorkerThread();
	}

	public void EnqueueRequest(DataRequest request, DownloadPriority priority)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		if (request.DataSource == null || request.Handler == null)
		{
			throw new ArgumentException("request", "Invalid request parameter");
		}
		lock (m_requestQueueLock)
		{
			if (priority == DownloadPriority.High)
			{
				m_qHighPriorityRequests.Enqueue(request);
			}
			else
			{
				m_qNormalPriorityRequests.Enqueue(request);
			}
		}
		PropagateProgressEvent();
		m_newRequestEvent.Set();
	}

	public DataRequest DequeueRequest()
	{
		DataRequest result = null;
		lock (m_requestQueueLock)
		{
			if (m_qHighPriorityRequests.Count > 0)
			{
				result = m_qHighPriorityRequests.Dequeue();
			}
			else if (m_qNormalPriorityRequests.Count > 0)
			{
				result = m_qNormalPriorityRequests.Dequeue();
			}
		}
		return result;
	}

	public bool IsRequest()
	{
		bool result = false;
		lock (m_requestQueueLock)
		{
			if (m_qHighPriorityRequests.Count > 0)
			{
				result = true;
			}
			else if (m_qNormalPriorityRequests.Count > 0)
			{
				result = true;
			}
		}
		return result;
	}

	public bool IsRequestProcessing(DataRequest dataRequest)
	{
		bool result = false;
		lock (m_requestQueueLock)
		{
			for (int i = 0; i < m_processingRequests.Count; i++)
			{
				if (m_processingRequests[i].Equals(dataRequest))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public void ProcessRequest(DataRequest request)
	{
		lock (m_requestQueueLock)
		{
			if (!IsRequestProcessing(request))
			{
				m_processingRequests.Add(request);
			}
		}
		DataProvider dataSource = request.DataSource;
		dataSource.ProccessRequest(request, this);
	}

	public void RequestProcessed(DataRequestCompletedEventArgs eventArguments, DataRequest dataRequest)
	{
		lock (m_requestQueueLock)
		{
			for (int i = 0; i < m_processingRequests.Count; i++)
			{
				if (m_processingRequests[i].Equals(dataRequest))
				{
					m_processingRequests.RemoveAt(i);
					break;
				}
			}
		}
		OnFinishRequest(eventArguments, dataRequest);
		PropagateProgressEvent();
	}

	public abstract void OnFinishRequest(DataRequestCompletedEventArgs eventArguments, DataRequest dataRequest);

	public void DataManagerThreadProc()
	{
		while (!m_quitWorkerThread)
		{
			m_newRequestEvent.WaitOne();
			if (m_quitWorkerThread)
			{
				break;
			}
			DataRequest dataRequest = DequeueRequest();
			if (dataRequest != null)
			{
				ProcessRequest(dataRequest);
			}
			lock (m_requestQueueLock)
			{
				if (IsRequest())
				{
					m_newRequestEvent.Set();
				}
			}
		}
		CancelAllRequests();
	}

	public void CreateWorkerThread()
	{
		if (m_workerThread == null)
		{
			m_quitWorkerThread = false;
			m_workerThread = new Thread(DataManagerThreadProc);
			m_workerThread.IsBackground = true;
			m_workerThread.Name = "DataManagerWorker";
			m_workerThread.Start();
		}
	}

	public void PropagateProgressEvent()
	{
		DownloadProgressEventHandler downloadProgressEventHandler = this.DownloadProgress;
		if (downloadProgressEventHandler != null)
		{
			DownloadProgressEventArgs e = new DownloadProgressEventArgs();
			lock (m_requestQueueLock)
			{
				e.NumberOfDownloadingFiles = m_processingRequests.Count;
				e.TotalNumberOfFilesToDownload = m_qHighPriorityRequests.Count + m_qNormalPriorityRequests.Count;
			}
			downloadProgressEventHandler(this, e);
		}
	}

	public void CancelAllRequests()
	{
		lock (m_requestQueueLock)
		{
			foreach (DataRequest qHighPriorityRequest in m_qHighPriorityRequests)
			{
				if (qHighPriorityRequest.Handler != null)
				{
					qHighPriorityRequest.Handler(null, new DataRequestCompletedEventArgs(null, canceled: true, qHighPriorityRequest.Context));
				}
			}
			m_qHighPriorityRequests.Clear();
			foreach (DataRequest qNormalPriorityRequest in m_qNormalPriorityRequests)
			{
				if (qNormalPriorityRequest.Handler != null)
				{
					qNormalPriorityRequest.Handler(null, new DataRequestCompletedEventArgs(null, canceled: true, qNormalPriorityRequest.Context));
				}
			}
			m_qNormalPriorityRequests.Clear();
			foreach (DataRequest processingRequest in m_processingRequests)
			{
				DataProvider dataSource = processingRequest.DataSource;
				dataSource.CancelRequest();
				if (processingRequest.Handler != null)
				{
					processingRequest.Handler(null, new DataRequestCompletedEventArgs(null, canceled: true, processingRequest.Context));
				}
			}
			m_processingRequests.Clear();
		}
	}

	public void QuitWorkerThread()
	{
		m_quitWorkerThread = true;
		m_newRequestEvent.Set();
		if (m_workerThread != null && m_workerThread.IsAlive)
		{
			m_workerThread.Join();
		}
		m_workerThread = null;
	}

	public void Cleanup()
	{
		QuitWorkerThread();
	}

	public DataManagerBase()
	{
	}
}
