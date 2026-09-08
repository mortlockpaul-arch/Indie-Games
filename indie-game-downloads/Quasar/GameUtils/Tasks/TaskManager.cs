using System;
using System.Collections.Generic;
using System.Threading;
using Quasar.GameUtils.Logic;
using Quasar.Global;
using XnaToFna;

namespace Quasar.GameUtils.Tasks;

public static class TaskManager
{
	private struct PendingTask(int taskId, Action<object> action, object parameters, Action<object> finishedCallback)
	{
		public int TaskId = taskId;

		public Action<object> Action = action;

		public object Parameters = parameters;

		public Action<object> FinishedCallback = finishedCallback;
	}

	private class TaskData
	{
		public Action<object> Action;

		public object parameters;

		public Action<object> finishedCallback;

		public volatile int TaskId;

		public volatile bool Done;

		public volatile bool Released;

		public TaskData()
		{
			TaskId = 0;
			Done = true;
			Released = true;
		}

		public void Init(Action<object> action, object parameters, int taskId)
		{
			Action = action;
			this.parameters = parameters;
			TaskId = taskId;
			Done = false;
			Released = false;
		}

		public void Init(Action<object> action, object parameters, int taskId, Action<object> finishedCallback)
		{
			Init(action, parameters, taskId);
			this.finishedCallback = finishedCallback;
		}

		public void Init(PendingTask task)
		{
			Init(task.Action, task.Parameters, task.TaskId, task.FinishedCallback);
		}

		public void Release()
		{
			Action = null;
			parameters = null;
			finishedCallback = null;
			TaskId = 0;
			Done = false;
			Released = true;
		}
	}

	public const int WORKER_THREADS = 4;

	private const int TASK_ARRAY_SIZE = 32;

	private const int MAX_WAIT_TIME = 60000;

	private static List<Thread> workerThreads;

	private static volatile TaskData[] tasks;

	private static Queue<PendingTask> pendingTasks;

	private static AutoResetEvent newItemEvent;

	private static volatile int NextTaskId;

	private static Thread mainThread;

	private static int currentTaskIndex;

	static TaskManager()
	{
		workerThreads = new List<Thread>(4);
		tasks = new TaskData[32];
		pendingTasks = new Queue<PendingTask>();
		NextTaskId = 1;
		currentTaskIndex = 1;
		newItemEvent = new AutoResetEvent(initialState: false);
		for (int i = 0; i < 32; i++)
		{
			tasks[i] = new TaskData();
		}
		mainThread = Thread.CurrentThread;
		Pool<TaskData>.SetCapacity(16);
		for (int j = 0; j < 4; j++)
		{
			Thread thread = new Thread(work)
			{
				IsBackground = true
			};
			workerThreads.Add(thread);
			thread.Start();
		}
	}

	public static void Exec(Action<object> action, object parameters)
	{
		if (workerThreads.Contains(Thread.CurrentThread))
		{
			action(parameters);
			return;
		}
		int taskId = AddData(action, parameters, null);
		newItemEvent.Set();
		WaitForTask(taskId);
	}

	private static int AddData(Action<object> action, object parameters, Action<object> finishedCallback)
	{
		if (pendingTasks.Count == 0)
		{
			for (int i = 0; i < 32; i++)
			{
				if (tasks[i].Released)
				{
					int num = NextTaskId++;
					tasks[i].Init(action, parameters, num, finishedCallback);
					return num;
				}
			}
		}
		int num2 = NextTaskId++;
		pendingTasks.Enqueue(new PendingTask(num2, action, parameters, finishedCallback));
		return num2;
	}

	public static int Post(Action<object> action, object parameters, Action<object> finishedCallback)
	{
		int result = AddData(action, parameters, finishedCallback);
		newItemEvent.Set();
		return result;
	}

	public static int Post(Action<object> action, object parameters)
	{
		return Post(action, parameters, null);
	}

	public static bool IsTaskPending(int taskId)
	{
		TaskData task = GetTask(taskId);
		if (task != null)
		{
			return true;
		}
		return false;
	}

	public static void WaitForTask(int taskId)
	{
		WaitForTask(taskId, 60000);
	}

	public static void WaitForTask(int taskId, int timeOut)
	{
		TaskData task = GetTask(taskId);
		if (task == null)
		{
			return;
		}
		long currentTotalTime = Quasar.Global.Timer.DefaultTimer.CurrentTotalTime;
		while (task.TaskId == taskId && !task.Done && !task.Released)
		{
			Thread.Sleep(0);
			if (Thread.CurrentThread == mainThread)
			{
				Update();
			}
			if (Quasar.Global.Timer.DefaultTimer.CurrentTotalTime - currentTotalTime > timeOut)
			{
				throw new Exception("Excessive task wait time");
			}
		}
	}

	private static TaskData GetTask(int taskId)
	{
		for (int i = 0; i < 32; i++)
		{
			TaskData taskData = tasks[i];
			if (!taskData.Released && taskData.TaskId == taskId)
			{
				return taskData;
			}
		}
		return null;
	}

	public static void Update()
	{
		for (int i = 0; i < 32; i++)
		{
			TaskData taskData = tasks[i];
			if (taskData.Done && !taskData.Released)
			{
				if (taskData.finishedCallback != null)
				{
					taskData.finishedCallback(taskData.parameters);
				}
				taskData.Release();
			}
			if (taskData.Released && pendingTasks.Count > 0)
			{
				taskData.Init(pendingTasks.Dequeue());
				newItemEvent.Set();
			}
		}
	}

	private static void work()
	{
		Thread.CurrentThread.SetProcessorAffinity(4);
		for (int i = 0; i < 4 && workerThreads[i] != Thread.CurrentThread; i++)
		{
		}
		while (true)
		{
			newItemEvent.WaitOne();
			PumpTasks();
		}
	}

	public static void EndAllTasks()
	{
		bool flag = false;
		do
		{
			Thread.Sleep(0);
			if (Thread.CurrentThread == mainThread)
			{
				Update();
			}
			flag = false;
			if (pendingTasks.Count > 0)
			{
				flag = true;
				continue;
			}
			for (int i = 0; i < 32; i++)
			{
				TaskData taskData = tasks[i];
				if (!taskData.Released)
				{
					flag = true;
				}
			}
		}
		while (flag);
	}

	private static void PumpTasks()
	{
		int nextTaskId = NextTaskId;
		while (currentTaskIndex < nextTaskId)
		{
			int num = currentTaskIndex;
			if (num == Interlocked.CompareExchange(ref currentTaskIndex, num + 1, num) && num < nextTaskId)
			{
				bool flag = false;
				while (!flag)
				{
					for (int i = 0; i < 32; i++)
					{
						if (tasks[i].TaskId == num)
						{
							performOperation(tasks[i]);
							flag = true;
						}
					}
					if (!flag)
					{
						Thread.Sleep(0);
					}
				}
			}
			nextTaskId = NextTaskId;
		}
	}

	private static void performOperation(TaskData taskData)
	{
		try
		{
			taskData.Action(taskData.parameters);
		}
		catch (Exception)
		{
		}
		taskData.Done = true;
	}
}
