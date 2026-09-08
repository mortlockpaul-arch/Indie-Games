using System;
using System.Collections.Generic;
using System.Threading;
using ProjectMercury.Emitters;

namespace ProjectMercury;

public class AsyncUpdateManager : IDisposable
{
	private volatile float DeltaSeconds;

	private volatile bool RunWorkerThread;

	private Thread WorkerThread { get; set; }

	private ManualResetEvent WorkAvailable { get; set; }

	private ManualResetEvent WorkDone { get; set; }

	private Queue<ParticleEffect> WorkQueue { get; set; }

	public AsyncUpdateManager(int processorAffinity)
	{
		WorkerThread = new Thread(WorkerThread_Body)
		{
			Name = "AsyncUpdateManager",
			IsBackground = true
		};
		WorkerThread.SetProcessorAffinity(new int[1] { processorAffinity });
		WorkAvailable = new ManualResetEvent(initialState: false);
		WorkDone = new ManualResetEvent(initialState: true);
		WorkQueue = new Queue<ParticleEffect>();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (WorkAvailable != null)
			{
				WorkAvailable.Dispose();
			}
			if (WorkDone != null)
			{
				WorkDone.Dispose();
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~AsyncUpdateManager()
	{
		Dispose(disposing: false);
	}

	public void Start()
	{
		RunWorkerThread = true;
		WorkerThread.Start();
	}

	public void Stop()
	{
		RunWorkerThread = false;
		WorkAvailable.Set();
		WorkerThread.Join(-1);
		WorkAvailable.Close();
		WorkDone.Close();
	}

	public void BeginUpdate(float deltaSeconds, params ParticleEffect[] effects)
	{
		DeltaSeconds = deltaSeconds;
		foreach (ParticleEffect item in effects)
		{
			WorkQueue.Enqueue(item);
		}
		WorkDone.Reset();
		WorkAvailable.Set();
	}

	public void EndUpdate()
	{
		WorkDone.WaitOne(-1);
		WorkAvailable.Reset();
	}

	private void WorkerThread_Body()
	{
		while (RunWorkerThread)
		{
			WorkAvailable.WaitOne(-1);
			lock (WorkQueue)
			{
				while (WorkQueue.Count > 0)
				{
					ParticleEffect particleEffect = WorkQueue.Dequeue();
					lock (particleEffect)
					{
						foreach (Emitter item in particleEffect)
						{
							item.Update(DeltaSeconds);
						}
					}
				}
				WorkDone.Set();
			}
		}
	}
}
