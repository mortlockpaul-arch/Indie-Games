using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

public static class Dns
{
	private static readonly Dictionary<object, Task> s_tasks = new Dictionary<object, Task>();

	public static string GetHostName()
	{
		NameResolutionActivity activity = NameResolutionTelemetry.Log.BeforeResolution(string.Empty, 0L);
		string hostName;
		try
		{
			hostName = NameResolutionPal.GetHostName();
		}
		catch (Exception exception) when (LogFailure(string.Empty, in activity, exception))
		{
			throw;
		}
		NameResolutionTelemetry.Log.AfterResolution(string.Empty, in activity, hostName);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(null, hostName, "GetHostName");
		}
		return hostName;
	}

	public static IPHostEntry GetHostEntry(IPAddress address)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(address, "address");
		if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(address, $"Invalid address '{address}'", "GetHostEntry");
			}
			throw new ArgumentException(System.SR.net_invalid_ip_addr, "address");
		}
		IPHostEntry hostEntryCore = GetHostEntryCore(address, AddressFamily.Unspecified);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(address, $"{hostEntryCore} with {hostEntryCore.AddressList.Length} entries", "GetHostEntry");
		}
		return hostEntryCore;
	}

	public static IPHostEntry GetHostEntry(string hostNameOrAddress)
	{
		return GetHostEntry(hostNameOrAddress, AddressFamily.Unspecified);
	}

	public static IPHostEntry GetHostEntry(string hostNameOrAddress, AddressFamily family)
	{
		ArgumentNullException.ThrowIfNull(hostNameOrAddress, "hostNameOrAddress");
		IPHostEntry hostEntryCore;
		if (IPAddress.TryParse(hostNameOrAddress, out IPAddress address))
		{
			if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(address, $"Invalid address '{address}'", "GetHostEntry");
				}
				throw new ArgumentException(System.SR.net_invalid_ip_addr, "hostNameOrAddress");
			}
			hostEntryCore = GetHostEntryCore(address, family);
		}
		else
		{
			hostEntryCore = GetHostEntryCore(hostNameOrAddress, family);
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(hostNameOrAddress, $"{hostEntryCore} with {hostEntryCore.AddressList.Length} entries", "GetHostEntry");
		}
		return hostEntryCore;
	}

	public static Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress)
	{
		return GetHostEntryAsync(hostNameOrAddress, AddressFamily.Unspecified, CancellationToken.None);
	}

	public static Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress, CancellationToken cancellationToken)
	{
		return GetHostEntryAsync(hostNameOrAddress, AddressFamily.Unspecified, cancellationToken);
	}

	public static Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress, AddressFamily family, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			Task<IPHostEntry> hostEntryCoreAsync = GetHostEntryCoreAsync(hostNameOrAddress, justReturnParsedIp: false, throwOnIIPAny: true, family, cancellationToken);
			hostEntryCoreAsync.ContinueWith(delegate(Task<IPHostEntry> t, object s)
			{
				string text = (string)s;
				if (t.Status == TaskStatus.RanToCompletion)
				{
					System.Net.NetEventSource.Info(text, $"{t.Result} with {t.Result.AddressList.Length} entries", "GetHostEntryAsync");
				}
				Exception ex = t.Exception?.InnerException;
				if (ex is SocketException ex2)
				{
					System.Net.NetEventSource.Error(text, $"{text} DNS lookup failed with {ex2.ErrorCode}", "GetHostEntryAsync");
				}
				else if (ex is OperationCanceledException)
				{
					System.Net.NetEventSource.Error(text, $"{text} DNS lookup was canceled", "GetHostEntryAsync");
				}
			}, hostNameOrAddress, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			return hostEntryCoreAsync;
		}
		return GetHostEntryCoreAsync(hostNameOrAddress, justReturnParsedIp: false, throwOnIIPAny: true, family, cancellationToken);
	}

	public static Task<IPHostEntry> GetHostEntryAsync(IPAddress address)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(address, "address");
		if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(address, $"Invalid address '{address}'", "GetHostEntryAsync");
			}
			throw new ArgumentException(System.SR.net_invalid_ip_addr, "address");
		}
		return RunAsync(delegate(object s, NameResolutionActivity activity)
		{
			if (OperatingSystem.IsWasi())
			{
				throw new PlatformNotSupportedException();
			}
			IPHostEntry hostEntryCore = GetHostEntryCore((IPAddress)s, AddressFamily.Unspecified, activity);
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info((IPAddress)s, $"{hostEntryCore} with {hostEntryCore.AddressList.Length} entries", "GetHostEntryAsync");
			}
			return hostEntryCore;
		}, address, CancellationToken.None);
	}

	public static IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback? requestCallback, object? stateObject)
	{
		return TaskToAsyncResult.Begin(GetHostEntryAsync(address), requestCallback, stateObject);
	}

	public static IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback? requestCallback, object? stateObject)
	{
		return TaskToAsyncResult.Begin(GetHostEntryAsync(hostNameOrAddress), requestCallback, stateObject);
	}

	public static IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(asyncResult, "asyncResult");
		return TaskToAsyncResult.End<IPHostEntry>(asyncResult);
	}

	public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
	{
		return GetHostAddresses(hostNameOrAddress, AddressFamily.Unspecified);
	}

	public static IPAddress[] GetHostAddresses(string hostNameOrAddress, AddressFamily family)
	{
		ArgumentNullException.ThrowIfNull(hostNameOrAddress, "hostNameOrAddress");
		IPAddress[] array;
		if (IPAddress.TryParse(hostNameOrAddress, out IPAddress address))
		{
			if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(address, $"Invalid address '{address}'", "GetHostAddresses");
				}
				throw new ArgumentException(System.SR.net_invalid_ip_addr, "hostNameOrAddress");
			}
			array = ((family != AddressFamily.Unspecified && address.AddressFamily != family) ? Array.Empty<IPAddress>() : new IPAddress[1] { address });
		}
		else
		{
			array = GetHostAddressesCore(hostNameOrAddress, family);
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(hostNameOrAddress, array, "GetHostAddresses");
		}
		return array;
	}

	public static Task<IPAddress[]> GetHostAddressesAsync(string hostNameOrAddress)
	{
		return (Task<IPAddress[]>)GetHostEntryOrAddressesCoreAsync(hostNameOrAddress, justReturnParsedIp: true, throwOnIIPAny: true, justAddresses: true, AddressFamily.Unspecified, CancellationToken.None);
	}

	public static Task<IPAddress[]> GetHostAddressesAsync(string hostNameOrAddress, CancellationToken cancellationToken)
	{
		return (Task<IPAddress[]>)GetHostEntryOrAddressesCoreAsync(hostNameOrAddress, justReturnParsedIp: true, throwOnIIPAny: true, justAddresses: true, AddressFamily.Unspecified, cancellationToken);
	}

	public static Task<IPAddress[]> GetHostAddressesAsync(string hostNameOrAddress, AddressFamily family, CancellationToken cancellationToken = default(CancellationToken))
	{
		return (Task<IPAddress[]>)GetHostEntryOrAddressesCoreAsync(hostNameOrAddress, justReturnParsedIp: true, throwOnIIPAny: true, justAddresses: true, family, cancellationToken);
	}

	public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback? requestCallback, object? state)
	{
		return TaskToAsyncResult.Begin(GetHostAddressesAsync(hostNameOrAddress), requestCallback, state);
	}

	public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(asyncResult, "asyncResult");
		return TaskToAsyncResult.End<IPAddress[]>(asyncResult);
	}

	[Obsolete("GetHostByName has been deprecated. Use GetHostEntry instead.")]
	public static IPHostEntry GetHostByName(string hostName)
	{
		ArgumentNullException.ThrowIfNull(hostName, "hostName");
		if (IPAddress.TryParse(hostName, out IPAddress address))
		{
			return CreateHostEntryForAddress(address);
		}
		return GetHostEntryCore(hostName, AddressFamily.Unspecified);
	}

	[Obsolete("BeginGetHostByName has been deprecated. Use BeginGetHostEntry instead.")]
	public static IAsyncResult BeginGetHostByName(string hostName, AsyncCallback? requestCallback, object? stateObject)
	{
		return TaskToAsyncResult.Begin(GetHostEntryCoreAsync(hostName, justReturnParsedIp: true, throwOnIIPAny: true, AddressFamily.Unspecified, CancellationToken.None), requestCallback, stateObject);
	}

	[Obsolete("EndGetHostByName has been deprecated. Use EndGetHostEntry instead.")]
	public static IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(asyncResult, "asyncResult");
		return TaskToAsyncResult.End<IPHostEntry>(asyncResult);
	}

	[Obsolete("GetHostByAddress has been deprecated. Use GetHostEntry instead.")]
	public static IPHostEntry GetHostByAddress(string address)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(address, "address");
		IPHostEntry hostEntryCore = GetHostEntryCore(IPAddress.Parse(address), AddressFamily.Unspecified);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(address, hostEntryCore, "GetHostByAddress");
		}
		return hostEntryCore;
	}

	[Obsolete("GetHostByAddress has been deprecated. Use GetHostEntry instead.")]
	public static IPHostEntry GetHostByAddress(IPAddress address)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		ArgumentNullException.ThrowIfNull(address, "address");
		IPHostEntry hostEntryCore = GetHostEntryCore(address, AddressFamily.Unspecified);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(address, hostEntryCore, "GetHostByAddress");
		}
		return hostEntryCore;
	}

	[Obsolete("Resolve has been deprecated. Use GetHostEntry instead.")]
	public static IPHostEntry Resolve(string hostName)
	{
		ArgumentNullException.ThrowIfNull(hostName, "hostName");
		IPHostEntry iPHostEntry;
		if (IPAddress.TryParse(hostName, out IPAddress address) && (address.AddressFamily != AddressFamily.InterNetworkV6 || SocketProtocolSupportPal.OSSupportsIPv6))
		{
			try
			{
				iPHostEntry = GetHostEntryCore(address, AddressFamily.Unspecified);
			}
			catch (SocketException message)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(hostName, message, "Resolve");
				}
				iPHostEntry = CreateHostEntryForAddress(address);
			}
		}
		else
		{
			iPHostEntry = GetHostEntryCore(hostName, AddressFamily.Unspecified);
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(hostName, iPHostEntry, "Resolve");
		}
		return iPHostEntry;
	}

	[Obsolete("BeginResolve has been deprecated. Use BeginGetHostEntry instead.")]
	public static IAsyncResult BeginResolve(string hostName, AsyncCallback? requestCallback, object? stateObject)
	{
		return TaskToAsyncResult.Begin(GetHostEntryCoreAsync(hostName, justReturnParsedIp: false, throwOnIIPAny: false, AddressFamily.Unspecified, CancellationToken.None), requestCallback, stateObject);
	}

	[Obsolete("EndResolve has been deprecated. Use EndGetHostEntry instead.")]
	public static IPHostEntry EndResolve(IAsyncResult asyncResult)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		IPHostEntry iPHostEntry;
		try
		{
			iPHostEntry = TaskToAsyncResult.End<IPHostEntry>(asyncResult);
		}
		catch (SocketException message)
		{
			object asyncState = TaskToAsyncResult.Unwrap(asyncResult).AsyncState;
			IPAddress iPAddress = ((asyncState is IPAddress iPAddress2) ? iPAddress2 : ((!(asyncState is KeyValuePair<IPAddress, AddressFamily> keyValuePair)) ? null : keyValuePair.Key));
			if (iPAddress == null)
			{
				throw;
			}
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, message, "EndResolve");
			}
			iPHostEntry = CreateHostEntryForAddress(iPAddress);
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(null, iPHostEntry, "EndResolve");
		}
		return iPHostEntry;
	}

	private static IPHostEntry GetHostEntryCore(string hostName, AddressFamily addressFamily, NameResolutionActivity? activityOrDefault = null)
	{
		return (IPHostEntry)GetHostEntryOrAddressesCore(hostName, justAddresses: false, addressFamily, activityOrDefault);
	}

	private static IPAddress[] GetHostAddressesCore(string hostName, AddressFamily addressFamily, NameResolutionActivity? activityOrDefault = null)
	{
		return (IPAddress[])GetHostEntryOrAddressesCore(hostName, justAddresses: true, addressFamily, activityOrDefault);
	}

	private static bool ValidateAddressFamily(ref AddressFamily addressFamily, string hostName, bool justAddresses, [NotNullWhen(false)] out object resultOnFailure)
	{
		if (!SocketProtocolSupportPal.OSSupportsIPv6)
		{
			if (addressFamily == AddressFamily.InterNetworkV6)
			{
				IPAddress[] array = Array.Empty<IPAddress>();
				resultOnFailure = (justAddresses ? ((object)array) : ((object)new IPHostEntry
				{
					AddressList = array,
					HostName = hostName,
					Aliases = Array.Empty<string>()
				}));
				return false;
			}
			if (addressFamily == AddressFamily.Unspecified)
			{
				addressFamily = AddressFamily.InterNetwork;
			}
		}
		resultOnFailure = null;
		return true;
	}

	private static object GetHostEntryOrAddressesCore(string hostName, bool justAddresses, AddressFamily addressFamily, NameResolutionActivity? activityOrDefault = null)
	{
		ValidateHostName(hostName);
		if (!ValidateAddressFamily(ref addressFamily, hostName, justAddresses, out var resultOnFailure))
		{
			return resultOnFailure;
		}
		NameResolutionActivity activity = activityOrDefault ?? NameResolutionTelemetry.Log.BeforeResolution(hostName, 0L);
		object obj;
		try
		{
			SocketError socketError = NameResolutionPal.TryGetAddrInfo(hostName, justAddresses, addressFamily, out var hostName2, out var aliases, out var addresses, out var nativeErrorCode);
			if (socketError != SocketError.Success)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(hostName, $"{hostName} DNS lookup failed with {socketError}", "GetHostEntryOrAddressesCore");
				}
				throw CreateException(socketError, nativeErrorCode);
			}
			obj = (justAddresses ? ((object)addresses) : ((object)new IPHostEntry
			{
				AddressList = addresses,
				HostName = hostName2,
				Aliases = aliases
			}));
		}
		catch (Exception exception) when (LogFailure(hostName, in activity, exception))
		{
			throw;
		}
		NameResolutionTelemetry.Log.AfterResolution(hostName, in activity, obj);
		return obj;
	}

	private static IPHostEntry GetHostEntryCore(IPAddress address, AddressFamily addressFamily, NameResolutionActivity? activityOrDefault = null)
	{
		return (IPHostEntry)GetHostEntryOrAddressesCore(address, justAddresses: false, addressFamily, activityOrDefault);
	}

	private static IPAddress[] GetHostAddressesCore(IPAddress address, AddressFamily addressFamily, NameResolutionActivity? activityOrDefault = null)
	{
		return (IPAddress[])GetHostEntryOrAddressesCore(address, justAddresses: true, addressFamily, activityOrDefault);
	}

	private static object GetHostEntryOrAddressesCore(IPAddress address, bool justAddresses, AddressFamily addressFamily, NameResolutionActivity? activityOrDefault = null)
	{
		if (OperatingSystem.IsWasi())
		{
			throw new PlatformNotSupportedException();
		}
		NameResolutionActivity activity = activityOrDefault ?? NameResolutionTelemetry.Log.BeforeResolution(address, 0L);
		string text;
		try
		{
			text = NameResolutionPal.TryGetNameInfo(address, out var errorCode, out var nativeErrorCode);
			if (errorCode != SocketError.Success)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(address, $"{address} DNS lookup failed with {errorCode}", "GetHostEntryOrAddressesCore");
				}
				throw CreateException(errorCode, nativeErrorCode);
			}
		}
		catch (Exception exception) when (LogFailure(address, in activity, exception))
		{
			throw;
		}
		NameResolutionTelemetry.Log.AfterResolution(address, in activity, text);
		if (!ValidateAddressFamily(ref addressFamily, text, justAddresses, out var resultOnFailure))
		{
			return resultOnFailure;
		}
		activity = NameResolutionTelemetry.Log.BeforeResolution(text, 0L);
		object obj;
		try
		{
			SocketError errorCode = NameResolutionPal.TryGetAddrInfo(text, justAddresses, addressFamily, out var hostName, out var aliases, out var addresses, out var _);
			if (errorCode != SocketError.Success && System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(address, $"forward lookup for '{text}' failed with {errorCode}", "GetHostEntryOrAddressesCore");
			}
			obj = (justAddresses ? ((object)addresses) : ((object)new IPHostEntry
			{
				HostName = hostName,
				Aliases = aliases,
				AddressList = addresses
			}));
		}
		catch (Exception exception2) when (LogFailure(text, in activity, exception2))
		{
			throw;
		}
		NameResolutionTelemetry.Log.AfterResolution(text, in activity, obj);
		return obj;
	}

	private static Task<IPHostEntry> GetHostEntryCoreAsync(string hostName, bool justReturnParsedIp, bool throwOnIIPAny, AddressFamily family, CancellationToken cancellationToken)
	{
		return (Task<IPHostEntry>)GetHostEntryOrAddressesCoreAsync(hostName, justReturnParsedIp, throwOnIIPAny, justAddresses: false, family, cancellationToken);
	}

	private static Task GetHostEntryOrAddressesCoreAsync(string hostName, bool justReturnParsedIp, bool throwOnIIPAny, bool justAddresses, AddressFamily family, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(hostName, "hostName");
		if (cancellationToken.IsCancellationRequested)
		{
			if (!justAddresses)
			{
				return Task.FromCanceled<IPHostEntry>(cancellationToken);
			}
			return Task.FromCanceled<IPAddress[]>(cancellationToken);
		}
		if (!ValidateAddressFamily(ref family, hostName, justAddresses, out var resultOnFailure))
		{
			if (!justAddresses)
			{
				return Task.FromResult((IPHostEntry)resultOnFailure);
			}
			return Task.FromResult((IPAddress[])resultOnFailure);
		}
		object key;
		if (IPAddress.TryParse(hostName, out IPAddress address))
		{
			if (throwOnIIPAny && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any)))
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(hostName, $"Invalid address '{address}'", "GetHostEntryOrAddressesCoreAsync");
				}
				throw new ArgumentException(System.SR.net_invalid_ip_addr, "hostName");
			}
			if (justReturnParsedIp)
			{
				if (!justAddresses)
				{
					return Task.FromResult(CreateHostEntryForAddress(address));
				}
				return Task.FromResult((family != AddressFamily.Unspecified && address.AddressFamily != family) ? Array.Empty<IPAddress>() : new IPAddress[1] { address });
			}
			key = ((family == AddressFamily.Unspecified) ? address : ((object)new KeyValuePair<IPAddress, AddressFamily>(address, family)));
		}
		else
		{
			if (NameResolutionPal.SupportsGetAddrInfoAsync)
			{
				ValidateHostName(hostName);
				Task task = ((!NameResolutionTelemetry.AnyDiagnosticsEnabled()) ? NameResolutionPal.GetAddrInfoAsync(hostName, justAddresses, family, cancellationToken) : (justAddresses ? ((Task)GetAddrInfoWithTelemetryAsync<IPAddress[]>(hostName, justAddresses, family, cancellationToken)) : ((Task)GetAddrInfoWithTelemetryAsync<IPHostEntry>(hostName, justAddresses, family, cancellationToken))));
				if (task != null)
				{
					return task;
				}
			}
			key = ((family == AddressFamily.Unspecified) ? hostName : ((object)new KeyValuePair<string, AddressFamily>(hostName, family)));
		}
		if (justAddresses)
		{
			return RunAsync(delegate(object s, NameResolutionActivity activity)
			{
				if (s is string hostName2)
				{
					return GetHostAddressesCore(hostName2, AddressFamily.Unspecified, activity);
				}
				if (s is KeyValuePair<string, AddressFamily> keyValuePair)
				{
					return GetHostAddressesCore(keyValuePair.Key, keyValuePair.Value, activity);
				}
				if (s is IPAddress address2)
				{
					return GetHostAddressesCore(address2, AddressFamily.Unspecified, activity);
				}
				return (s is KeyValuePair<IPAddress, AddressFamily> keyValuePair2) ? GetHostAddressesCore(keyValuePair2.Key, keyValuePair2.Value, activity) : null;
			}, key, cancellationToken);
		}
		return RunAsync(delegate(object s, NameResolutionActivity activity)
		{
			if (s is string hostName2)
			{
				return GetHostEntryCore(hostName2, AddressFamily.Unspecified, activity);
			}
			if (s is KeyValuePair<string, AddressFamily> keyValuePair)
			{
				return GetHostEntryCore(keyValuePair.Key, keyValuePair.Value, activity);
			}
			if (s is IPAddress address2)
			{
				return GetHostEntryCore(address2, AddressFamily.Unspecified, activity);
			}
			return (s is KeyValuePair<IPAddress, AddressFamily> keyValuePair2) ? GetHostEntryCore(keyValuePair2.Key, keyValuePair2.Value, activity) : null;
		}, key, cancellationToken);
	}

	private static Task<T> GetAddrInfoWithTelemetryAsync<T>(string hostName, bool justAddresses, AddressFamily addressFamily, CancellationToken cancellationToken) where T : class
	{
		long timestamp = Stopwatch.GetTimestamp();
		Task addrInfoAsync = NameResolutionPal.GetAddrInfoAsync(hostName, justAddresses, addressFamily, cancellationToken);
		if (addrInfoAsync != null)
		{
			return CompleteAsync(addrInfoAsync, hostName, timestamp);
		}
		return null;
		static async Task<T> CompleteAsync(Task task, string hostNameOrAddress, long startingTimeStamp)
		{
			NameResolutionActivity activity = NameResolutionTelemetry.Log.BeforeResolution(hostNameOrAddress, startingTimeStamp);
			Exception exception = null;
			T result = null;
			try
			{
				result = await ((Task<T>)task).ConfigureAwait(continueOnCapturedContext: false);
				return result;
			}
			catch (Exception ex)
			{
				exception = ex;
				throw;
			}
			finally
			{
				NameResolutionTelemetry.Log.AfterResolution(hostNameOrAddress, in activity, result, exception);
			}
		}
	}

	private static IPHostEntry CreateHostEntryForAddress(IPAddress address)
	{
		IPHostEntry iPHostEntry = new IPHostEntry();
		iPHostEntry.HostName = address.ToString();
		iPHostEntry.Aliases = Array.Empty<string>();
		iPHostEntry.AddressList = new IPAddress[1] { address };
		return iPHostEntry;
	}

	private static void ValidateHostName(string hostName)
	{
		if (hostName.Length > 255 || (hostName.Length == 255 && hostName[254] != '.'))
		{
			throw new ArgumentOutOfRangeException("hostName", System.SR.Format(System.SR.net_toolong, "hostName", 255.ToString(NumberFormatInfo.CurrentInfo)));
		}
	}

	private static bool LogFailure(object hostNameOrAddress, in NameResolutionActivity activity, Exception exception)
	{
		NameResolutionTelemetry.Log.AfterResolution(hostNameOrAddress, in activity, null, exception);
		return false;
	}

	private static Task<TResult> RunAsync<TResult>(Func<object, NameResolutionActivity, TResult> func, object key, CancellationToken cancellationToken)
	{
		bool num = NameResolutionActivity.IsTracingEnabled();
		Activity current = (num ? Activity.Current : null);
		NameResolutionActivity activity = NameResolutionTelemetry.Log.BeforeResolution(key, 0L);
		if (num)
		{
			Activity.Current = current;
		}
		Task<TResult> task = null;
		lock (s_tasks)
		{
			s_tasks.TryGetValue(key, out var value);
			if (value == null)
			{
				value = Task.CompletedTask;
			}
			task = value.ContinueWith(delegate
			{
				try
				{
					return func(key, activity);
				}
				finally
				{
					lock (s_tasks)
					{
						((ICollection<KeyValuePair<object, Task>>)s_tasks).Remove(new KeyValuePair<object, Task>(key, task));
					}
				}
			}, key, cancellationToken, TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
			if (cancellationToken.CanBeCanceled)
			{
				task.ContinueWith(delegate(Task<TResult> value2, object obj)
				{
					lock (s_tasks)
					{
						((ICollection<KeyValuePair<object, Task>>)s_tasks).Remove(new KeyValuePair<object, Task>(obj, value2));
					}
					NameResolutionTelemetry.Log.AfterResolution(obj, in activity, new OperationCanceledException());
				}, key, CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			}
			s_tasks[key] = task;
		}
		return task;
	}

	private static SocketException CreateException(SocketError error, int nativeError)
	{
		return new SocketException((int)error)
		{
			HResult = nativeError
		};
	}
}
