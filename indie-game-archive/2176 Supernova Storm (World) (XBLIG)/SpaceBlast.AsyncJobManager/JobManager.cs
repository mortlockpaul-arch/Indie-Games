using System.Collections.Generic;
using System.Threading;

namespace SpaceBlast.AsyncJobManager;

internal class JobManager
{
	private Thread m_Worker1;

	private Thread m_Worker2;

	private Queue<AsyncJob> m_JobQueue = new Queue<AsyncJob>();

	private ManualResetEvent m_JobsAvailableEvent = new ManualResetEvent(initialState: false);

	private ManualResetEvent m_ShutdownEvent = new ManualResetEvent(initialState: false);

	public JobManager()
	{
		m_Worker1 = new Thread(WorkerThreadLauncher1);
		m_Worker2 = new Thread(WorkerThreadLauncher2);
		m_Worker1.Start();
		m_Worker2.Start();
	}

	public void Shutdown()
	{
		m_ShutdownEvent.Set();
	}

	public void WorkerThreadLauncher1()
	{
		MainWorkerThread(3);
	}

	public void WorkerThreadLauncher2()
	{
		MainWorkerThread(4);
	}

	public void MainWorkerThread(int cpu_affinity)
	{
		int[] processorAffinity = new int[1] { cpu_affinity };
		Thread.CurrentThread.SetProcessorAffinity(processorAffinity);
		while (!m_ShutdownEvent.WaitOne(0, exitContext: false))
		{
			if (!m_JobsAvailableEvent.WaitOne(100, exitContext: false))
			{
				continue;
			}
			AsyncJob asyncJob;
			lock (m_JobQueue)
			{
				if (m_JobQueue.Count == 0)
				{
					m_JobsAvailableEvent.Reset();
					continue;
				}
				asyncJob = m_JobQueue.Dequeue();
			}
			asyncJob.ExecuteJob();
			asyncJob.IsComplete = true;
		}
	}

	public void AddJobToStack(AsyncJob job)
	{
		lock (m_JobQueue)
		{
			m_JobQueue.Enqueue(job);
			m_JobsAvailableEvent.Set();
		}
	}
}
