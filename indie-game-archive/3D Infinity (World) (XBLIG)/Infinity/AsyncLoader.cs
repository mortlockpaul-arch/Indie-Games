using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity;

public class AsyncLoader
{
	private Stopwatch stopwatch;

	public ContentManager Content { get; private set; }

	public bool IsAlive { get; set; }

	public Thread LoadThread { get; private set; }

	public AsyncLoader(ContentManager content)
	{
		Content = content;
		stopwatch = new Stopwatch();
	}

	public void AsyncLoad(IEnumerable<string> assets, Action finished)
	{
		LoadThread = new Thread((ThreadStart)delegate
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			Thread.CurrentThread.Name = "AssetLoader";
			stopwatch.Reset();
			stopwatch.Start();
			foreach (string asset in assets)
			{
				Content.Load<Model>(asset);
			}
			stopwatch.Stop();
			IsAlive = false;
			if (finished != null)
			{
				finished();
			}
		});
		IsAlive = true;
		LoadThread.Start();
	}

	private void Chache<T>(string asset)
	{
		Content.Load<T>(asset);
	}
}
