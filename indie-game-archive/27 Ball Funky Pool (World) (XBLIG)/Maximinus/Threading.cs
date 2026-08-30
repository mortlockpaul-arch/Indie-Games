using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class Threading
{
	public delegate void ThreadTaskDelegate();

	public delegate void TaskFinishedDelegate(int taskId);

	public struct ThreadTask
	{
		private ThreadTaskDelegate task;

		private TaskFinishedDelegate taskFinished;

		private int taskId;

		public ThreadTaskDelegate Task => task;

		public TaskFinishedDelegate TaskFinished => taskFinished;

		public int TaskId => taskId;

		public ThreadTask(ThreadTaskDelegate task, TaskFinishedDelegate taskFinished, int taskId)
		{
			this.task = task;
			this.taskFinished = taskFinished;
			this.taskId = taskId;
		}

		public ThreadTask(ThreadTaskDelegate task, TaskFinishedDelegate taskFinished)
			: this(task, taskFinished, -1)
		{
		}

		public ThreadTask(ThreadTaskDelegate task)
			: this(task, null, -1)
		{
		}
	}

	public class ManagedThread
	{
		private static List<ManagedThread> instances = new List<ManagedThread>();

		private Thread thread;

		private int processorAffinity;

		private bool killThread;

		private Queue<ThreadTask> tasks = new Queue<ThreadTask>();

		public static void KillAll()
		{
			foreach (ManagedThread instance in instances)
			{
				instance?.KillImmediately();
			}
		}

		public ManagedThread(int processorAffinity)
		{
			this.processorAffinity = processorAffinity;
			if (processorAffinity != -1)
			{
				Thread.CurrentThread.SetProcessorAffinity(new int[1] { processorAffinity });
			}
			ThreadStart start = taskRunner;
			thread = new Thread(start);
			thread.Start();
			instances.Add(this);
		}

		public ManagedThread()
			: this(-1)
		{
		}

		private void taskRunner()
		{
			if (processorAffinity > 0 && processorAffinity < 6 && processorAffinity != 2)
			{
				thread.SetProcessorAffinity(new int[1] { processorAffinity });
			}
			while (!killThread)
			{
				if (tasks.Count > 0)
				{
					ThreadTask threadTask;
					lock (tasks)
					{
						threadTask = tasks.Dequeue();
					}
					threadTask.Task();
					if (threadTask.TaskFinished != null)
					{
						threadTask.TaskFinished(threadTask.TaskId);
					}
				}
				else
				{
					Thread.Sleep(0);
				}
			}
			tasks.Clear();
			tasks = null;
		}

		public void Kill()
		{
			killThread = true;
		}

		public void KillImmediately()
		{
			killThread = true;
			thread.Abort();
		}

		public void AddTask(ThreadTask task)
		{
			lock (tasks)
			{
				tasks.Enqueue(task);
			}
		}
	}

	public class ManagedThreadEveryFrame : ManagedThread
	{
		private AutoResetEvent frameStart;

		private AutoResetEvent completion;

		public GameTime gameTime;

		public ManagedThreadEveryFrame(int processorAffinity)
			: base(processorAffinity)
		{
			frameStart = new AutoResetEvent(initialState: false);
			completion = new AutoResetEvent(initialState: false);
		}

		public void FrameStart(GameTime gameTime)
		{
			this.gameTime = gameTime;
			frameStart.Set();
		}

		public void WaitForFrameStart()
		{
			frameStart.WaitOne();
		}

		public void WaitForCompletion()
		{
			completion.WaitOne();
		}

		public void Completed()
		{
			completion.Set();
		}
	}
}
