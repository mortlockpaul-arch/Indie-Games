using System.Collections.Generic;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;
using System.Threading;

namespace System.Diagnostics.Metrics;

internal static class RuntimeMetrics
{
	[ThreadStatic]
	private static bool t_handlingFirstChanceException;

	private static readonly Meter s_meter;

	private static readonly string[] s_genNames;

	private static readonly int s_maxGenerations;

	private static readonly Counter<long> s_exceptions;

	public static void EnsureInitialized()
	{
	}

	static RuntimeMetrics()
	{
		s_meter = new Meter("System.Runtime");
		s_genNames = new string[5] { "gen0", "gen1", "gen2", "loh", "poh" };
		s_maxGenerations = Math.Min(GC.GetGCMemoryInfo().GenerationInfo.Length, s_genNames.Length);
		s_meter.CreateObservableCounter("dotnet.gc.collections", GetGarbageCollectionCounts, "{collection}", "The number of garbage collections that have occurred since the process has started.");
		s_meter.CreateObservableUpDownCounter("dotnet.process.memory.working_set", () => Environment.WorkingSet, "By", "The number of bytes of physical memory mapped to the process context.");
		s_meter.CreateObservableCounter("dotnet.gc.heap.total_allocated", () => GC.GetTotalAllocatedBytes(), "By", "The approximate number of bytes allocated on the managed GC heap since the process has started. The returned value does not include any native allocations.");
		s_meter.CreateObservableUpDownCounter("dotnet.gc.last_collection.memory.committed_size", () => GC.GetGCMemoryInfo().TotalCommittedBytes, "By", "The amount of committed virtual memory in use by the .NET GC, as observed during the latest garbage collection.");
		s_meter.CreateObservableUpDownCounter("dotnet.gc.last_collection.heap.size", GetHeapSizes, "By", "The managed GC heap size (including fragmentation), as observed during the latest garbage collection.");
		s_meter.CreateObservableUpDownCounter("dotnet.gc.last_collection.heap.fragmentation.size", GetHeapFragmentation, "By", "The heap fragmentation, as observed during the latest garbage collection.");
		s_meter.CreateObservableCounter("dotnet.gc.pause.time", () => GC.GetTotalPauseDuration().TotalSeconds, "s", "The total amount of time paused in GC since the process has started.");
		s_meter.CreateObservableCounter("dotnet.jit.compiled_il.size", () => JitInfo.GetCompiledILBytes(), "By", "Count of bytes of intermediate language that have been compiled since the process has started.");
		s_meter.CreateObservableCounter("dotnet.jit.compiled_methods", () => JitInfo.GetCompiledMethodCount(), "{method}", "The number of times the JIT compiler (re)compiled methods since the process has started.");
		s_meter.CreateObservableCounter("dotnet.jit.compilation.time", () => JitInfo.GetCompilationTime().TotalSeconds, "s", "The number of times the JIT compiler (re)compiled methods since the process has started.");
		s_meter.CreateObservableCounter("dotnet.monitor.lock_contentions", () => Monitor.LockContentionCount, "{contention}", "The number of times there was contention when trying to acquire a monitor lock since the process has started.");
		s_meter.CreateObservableCounter("dotnet.thread_pool.thread.count", (Func<long>)(() => ThreadPool.ThreadCount), "{thread}", "The number of thread pool threads that currently exist.");
		s_meter.CreateObservableCounter("dotnet.thread_pool.work_item.count", () => ThreadPool.CompletedWorkItemCount, "{work_item}", "The number of work items that the thread pool has completed since the process has started.");
		s_meter.CreateObservableCounter("dotnet.thread_pool.queue.length", () => ThreadPool.PendingWorkItemCount, "{work_item}", "The number of work items that are currently queued to be processed by the thread pool.");
		s_meter.CreateObservableUpDownCounter("dotnet.timer.count", () => Timer.ActiveCount, "{timer}", "The number of timer instances that are currently active. An active timer is registered to tick at some point in the future and has not yet been canceled.");
		s_meter.CreateObservableUpDownCounter("dotnet.assembly.count", (Func<long>)(() => AppDomain.CurrentDomain.GetAssemblies().Length), "{assembly}", "The number of .NET assemblies that are currently loaded.");
		s_exceptions = s_meter.CreateCounter<long>("dotnet.exceptions", "{exception}", "The number of exceptions that have been thrown in managed code.");
		AppDomain.CurrentDomain.FirstChanceException += delegate(object source, FirstChanceExceptionEventArgs e)
		{
			if (!t_handlingFirstChanceException)
			{
				t_handlingFirstChanceException = true;
				s_exceptions.Add(1L, new KeyValuePair<string, object>("error.type", e.Exception.GetType().Name));
				t_handlingFirstChanceException = false;
			}
		};
		s_meter.CreateObservableUpDownCounter("dotnet.process.cpu.count", (Func<long>)(() => Environment.ProcessorCount), "{cpu}", "The number of processors available to the process.");
		if (!OperatingSystem.IsBrowser() && !OperatingSystem.IsWasi() && !OperatingSystem.IsTvOS() && (!OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst()))
		{
			s_meter.CreateObservableCounter("dotnet.process.cpu.time", GetCpuTime, "s", "CPU time used by the process.");
		}
	}

	private static IEnumerable<Measurement<long>> GetGarbageCollectionCounts()
	{
		long num = 0L;
		int gen = GC.MaxGeneration;
		while (gen >= 0)
		{
			long collectionsFromThisGeneration = GC.CollectionCount(gen);
			yield return new Measurement<long>(collectionsFromThisGeneration - num, new KeyValuePair<string, object>("gc.heap.generation", s_genNames[gen]));
			num = collectionsFromThisGeneration;
			int num2 = gen - 1;
			gen = num2;
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	[UnsupportedOSPlatform("browser")]
	[SupportedOSPlatform("maccatalyst")]
	private static IEnumerable<Measurement<double>> GetCpuTime()
	{
		Environment.ProcessCpuUsage processCpuUsage = Environment.CpuUsage;
		yield return new Measurement<double>(processCpuUsage.UserTime.TotalSeconds, new KeyValuePair<string, object>("cpu.mode", "user"));
		yield return new Measurement<double>(processCpuUsage.PrivilegedTime.TotalSeconds, new KeyValuePair<string, object>("cpu.mode", "system"));
	}

	private static IEnumerable<Measurement<long>> GetHeapSizes()
	{
		GCMemoryInfo gcInfo = GC.GetGCMemoryInfo();
		int i = 0;
		while (i < s_maxGenerations)
		{
			yield return new Measurement<long>(gcInfo.GenerationInfo[i].SizeAfterBytes, new KeyValuePair<string, object>("gc.heap.generation", s_genNames[i]));
			int num = i + 1;
			i = num;
		}
	}

	private static IEnumerable<Measurement<long>> GetHeapFragmentation()
	{
		GCMemoryInfo gcInfo = GC.GetGCMemoryInfo();
		int i = 0;
		while (i < s_maxGenerations)
		{
			yield return new Measurement<long>(gcInfo.GenerationInfo[i].FragmentationAfterBytes, new KeyValuePair<string, object>("gc.heap.generation", s_genNames[i]));
			int num = i + 1;
			i = num;
		}
	}
}
