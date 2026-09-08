using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Tracing;

internal static class EventSourceInitHelper
{
	private static List<Func<EventSource>> s_preregisteredEventSourceFactories = new List<Func<EventSource>>();

	private static readonly Dictionary<Guid, EventSource.OverrideEventProvider> s_preregisteredEtwProviders = new Dictionary<Guid, EventSource.OverrideEventProvider>();

	private static readonly Dictionary<string, EventSource.OverrideEventProvider> s_preregisteredEventPipeProviders = new Dictionary<string, EventSource.OverrideEventProvider>();

	internal static EventSource GetMetricsEventSource()
	{
		return GetInstance(null) as EventSource;
		[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetInstance")]
		[return: UnsafeAccessorType("System.Diagnostics.Metrics.MetricsEventSource, System.Diagnostics.DiagnosticSource")]
		static extern object GetInstance([UnsafeAccessorType("System.Diagnostics.Metrics.MetricsEventSource, System.Diagnostics.DiagnosticSource")] object _);
	}

	internal unsafe static void PreregisterEventProviders(Guid id, string name, Func<EventSource> eventSourceFactory)
	{
		try
		{
			s_preregisteredEventSourceFactories.Add(eventSourceFactory);
			EventSource.OverrideEventProvider overrideEventProvider = new EventSource.OverrideEventProvider(eventSourceFactory, EventProviderType.ETW);
			overrideEventProvider.Register(id, name);
			byte[] array = Statics.MetadataForString(name, 0, 0, 0);
			fixed (byte* data = array)
			{
				overrideEventProvider.SetInformation(Interop.Advapi32.EVENT_INFO_CLASS.SetTraits, data, (uint)array.Length);
			}
			lock (s_preregisteredEtwProviders)
			{
				s_preregisteredEtwProviders[id] = overrideEventProvider;
			}
			EventSource.OverrideEventProvider overrideEventProvider2 = new EventSource.OverrideEventProvider(eventSourceFactory, EventProviderType.EventPipe);
			overrideEventProvider2.Register(id, name);
			lock (s_preregisteredEventPipeProviders)
			{
				s_preregisteredEventPipeProviders[name] = overrideEventProvider2;
			}
		}
		catch (Exception)
		{
		}
	}

	internal static void EnsurePreregisteredEventSourcesExist()
	{
		if (EventSource.IsSupported)
		{
			Func<EventSource>[] array;
			lock (s_preregisteredEventSourceFactories)
			{
				array = s_preregisteredEventSourceFactories.ToArray();
				s_preregisteredEventSourceFactories.Clear();
			}
			Func<EventSource>[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i]();
			}
		}
	}

	internal static EventSource.OverrideEventProvider TryGetPreregisteredEtwProvider(Guid id)
	{
		lock (s_preregisteredEtwProviders)
		{
			s_preregisteredEtwProviders.Remove(id, out var value);
			return value;
		}
	}

	internal static EventSource.OverrideEventProvider TryGetPreregisteredEventPipeProvider(string name)
	{
		lock (s_preregisteredEventPipeProviders)
		{
			s_preregisteredEventPipeProviders.Remove(name, out var value);
			return value;
		}
	}
}
