using System;
using System.Threading;
using Microsoft.Xna.Framework;

namespace RenegadeEngine.Threading;

public abstract class UpdateAsyncTask
{
	private EventWaitHandle beginAsyncHandle;

	private EventWaitHandle endAsyncHandle;

	private bool isAsyncCanceled;

	protected GameTime gameTimeAsync;

	protected object parameters;

	private Thread workThread;

	private int processorAffinity;

	public int WorkerThreadId => workThread.ManagedThreadId;

	public string Name
	{
		get
		{
			return workThread.Name;
		}
		protected set
		{
			workThread.Name = value;
		}
	}

	public virtual void Initialize(int processorAffinity)
	{
		beginAsyncHandle = new AutoResetEvent(initialState: false);
		endAsyncHandle = new AutoResetEvent(initialState: false);
		isAsyncCanceled = false;
		gameTimeAsync = new GameTime();
		parameters = new object();
		workThread = new Thread(UpdateLoopAsync);
		workThread.IsBackground = true;
		if (processorAffinity >= 0 && processorAffinity <= 5)
		{
			this.processorAffinity = processorAffinity;
		}
		else
		{
			this.processorAffinity = 0;
		}
		workThread.Start();
	}

	public void BeginUpdateAsync(GameTime gameTime)
	{
		gameTimeAsync = gameTime;
		beginAsyncHandle.Set();
	}

	public virtual void BeginUpdateAsync(GameTime gameTime, object parameters)
	{
		gameTimeAsync = gameTime;
		beginAsyncHandle.Set();
	}

	public void EndUpdateAsync()
	{
		endAsyncHandle.WaitOne();
	}

	public void CancelUpdateAsync()
	{
		isAsyncCanceled = true;
		beginAsyncHandle.Set();
	}

	protected void UpdateLoopAsync()
	{
		//Thread.CurrentThread.SetProcessorAffinity(new int[1] { processorAffinity });
		while (true)
		{
			beginAsyncHandle.WaitOne();
			try
			{
				DoWork(isAsyncCanceled);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				endAsyncHandle.Set();
			}
		}
	}

	protected abstract void DoWork(bool isCancelled);

	public virtual void Dispose()
	{
		CancelUpdateAsync();
		workThread.Abort();
	}
}
