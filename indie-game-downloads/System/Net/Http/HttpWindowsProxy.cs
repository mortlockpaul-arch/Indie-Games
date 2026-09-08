using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System.Net.Http;

internal sealed class HttpWindowsProxy : IMultiWebProxy, IWebProxy, IDisposable
{
	private readonly RegistryKey _internetSettingsRegistry = Registry.CurrentUser?.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");

	private MultiProxy _insecureProxy;

	private MultiProxy _secureProxy;

	private FailedProxyCache _failedProxies = new FailedProxyCache();

	private List<string> _bypass;

	private List<IPAddress> _localIp;

	private ICredentials _credentials;

	private WinInetProxyHelper _proxyHelper;

	private global::Interop.WinHttp.SafeWinHttpHandle _sessionHandle;

	private bool _disposed;

	private EventWaitHandle _waitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);

	private RegisteredWaitHandle _registeredWaitHandle;

	public ICredentials Credentials
	{
		get
		{
			return _credentials;
		}
		set
		{
			_credentials = value;
		}
	}

	public HttpWindowsProxy(WinInetProxyHelper proxy = null)
	{
		if (_internetSettingsRegistry != null && proxy == null && global::Interop.Advapi32.RegNotifyChangeKeyValue(_internetSettingsRegistry.Handle, watchSubtree: true, 268435463u, _waitHandle.SafeWaitHandle, asynchronous: true) == 0)
		{
			_registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle, RegistryChangeNotificationCallback, this, -1, executeOnlyOnce: false);
		}
		UpdateConfiguration(proxy);
	}

	private static void RegistryChangeNotificationCallback(object state, bool timedOut)
	{
		HttpWindowsProxy httpWindowsProxy = (HttpWindowsProxy)state;
		if (httpWindowsProxy._disposed)
		{
			return;
		}
		try
		{
			global::Interop.Advapi32.RegNotifyChangeKeyValue(httpWindowsProxy._internetSettingsRegistry.Handle, watchSubtree: true, 268435463u, httpWindowsProxy._waitHandle.SafeWaitHandle, asynchronous: true);
			lock (httpWindowsProxy)
			{
				httpWindowsProxy.UpdateConfiguration();
			}
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(httpWindowsProxy, $"Failed to refresh proxy configuration: {ex.Message}", "RegistryChangeNotificationCallback");
			}
		}
	}

	[MemberNotNull("_proxyHelper")]
	private void UpdateConfiguration(WinInetProxyHelper proxyHelper = null)
	{
		if (proxyHelper == null)
		{
			proxyHelper = new WinInetProxyHelper();
		}
		if (proxyHelper.AutoSettingsUsed)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(proxyHelper, FormattableStringFactory.Create("AutoSettingsUsed, calling {0}", "WinHttpOpen"), "UpdateConfiguration");
			}
			global::Interop.WinHttp.SafeWinHttpHandle safeWinHttpHandle = global::Interop.WinHttp.WinHttpOpen(IntPtr.Zero, 1u, null, null, 268435456);
			if (safeWinHttpHandle.IsInvalid)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(proxyHelper, FormattableStringFactory.Create("{0} returned invalid handle", "WinHttpOpen"), "UpdateConfiguration");
				}
				safeWinHttpHandle.Dispose();
			}
			_sessionHandle = safeWinHttpHandle;
		}
		if (proxyHelper.ManualSettingsUsed)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(proxyHelper, $"ManualSettingsUsed, {proxyHelper.Proxy}", "UpdateConfiguration");
			}
			_secureProxy = MultiProxy.ParseManualSettings(_failedProxies, proxyHelper.Proxy, secure: true);
			_insecureProxy = MultiProxy.ParseManualSettings(_failedProxies, proxyHelper.Proxy, secure: false);
			if (!string.IsNullOrWhiteSpace(proxyHelper.ProxyBypass))
			{
				int i = 0;
				bool flag = false;
				List<IPAddress> list = null;
				List<string> list2 = new List<string>(proxyHelper.ProxyBypass.Length / 5);
				while (i < proxyHelper.ProxyBypass.Length)
				{
					for (; i < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[i] == ' '; i++)
					{
					}
					if (string.Compare(proxyHelper.ProxyBypass, i, "http://", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
					{
						i += 7;
					}
					else if (string.Compare(proxyHelper.ProxyBypass, i, "https://", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
					{
						i += 8;
					}
					if (i < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[i] == '[')
					{
						i++;
					}
					int num = i;
					for (; i < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[i] != ' ' && proxyHelper.ProxyBypass[i] != ';' && proxyHelper.ProxyBypass[i] != ']'; i++)
					{
					}
					string text;
					if (i == num)
					{
						text = null;
					}
					else if (string.Compare(proxyHelper.ProxyBypass, num, "<local>", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						text = null;
					}
					else
					{
						text = proxyHelper.ProxyBypass.Substring(num, i - num);
					}
					if (i < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[i] != ';')
					{
						for (; i < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[i] != ';'; i++)
						{
						}
					}
					if (i < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[i] == ';')
					{
						i++;
					}
					if (text != null)
					{
						list2.Add(text);
					}
				}
				_bypass = ((list2.Count > 0) ? list2 : null);
				if (flag)
				{
					list = new List<IPAddress>();
					NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					for (int j = 0; j < allNetworkInterfaces.Length; j++)
					{
						foreach (UnicastIPAddressInformation unicastAddress in allNetworkInterfaces[j].GetIPProperties().UnicastAddresses)
						{
							list.Add(unicastAddress.Address);
						}
					}
				}
				_localIp = ((list != null && list.Count > 0) ? list : null);
			}
		}
		_proxyHelper = proxyHelper;
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			if (_sessionHandle != null && !_sessionHandle.IsInvalid)
			{
				global::Interop.WinHttp.SafeWinHttpHandle.DisposeAndClearHandle(ref _sessionHandle);
			}
			_waitHandle?.Dispose();
			_internetSettingsRegistry?.Dispose();
			_registeredWaitHandle?.Unregister(null);
		}
	}

	public Uri GetProxy(Uri uri)
	{
		if (!_proxyHelper.AutoSettingsUsed && !_proxyHelper.ManualSettingsOnly)
		{
			return null;
		}
		GetMultiProxy(uri).ReadNext(out var uri2, out var _);
		return uri2;
	}

	public MultiProxy GetMultiProxy(Uri uri)
	{
		if (_proxyHelper.AutoSettingsUsed && !_proxyHelper.RecentAutoDetectionFailure)
		{
			global::Interop.WinHttp.WINHTTP_PROXY_INFO proxyInfo = default(global::Interop.WinHttp.WINHTTP_PROXY_INFO);
			try
			{
				if (!_proxyHelper.GetProxyForUrl(_sessionHandle, uri, out proxyInfo))
				{
					return MultiProxy.Empty;
				}
				if (proxyInfo.ProxyBypass == IntPtr.Zero)
				{
					if (proxyInfo.Proxy != IntPtr.Zero)
					{
						string proxyConfig = Marshal.PtrToStringUni(proxyInfo.Proxy);
						return MultiProxy.CreateLazy(_failedProxies, proxyConfig, IsSecureUri(uri));
					}
					return MultiProxy.Empty;
				}
			}
			finally
			{
				Marshal.FreeHGlobal(proxyInfo.Proxy);
				Marshal.FreeHGlobal(proxyInfo.ProxyBypass);
			}
		}
		if (_proxyHelper.ManualSettingsUsed)
		{
			if (_localIp != null)
			{
				if (uri.IsLoopback)
				{
					return MultiProxy.Empty;
				}
				if ((uri.HostNameType == UriHostNameType.IPv6 || uri.HostNameType == UriHostNameType.IPv4) && IPAddress.TryParse(uri.IdnHost, out IPAddress address))
				{
					foreach (IPAddress item in _localIp)
					{
						if (item.Equals(address))
						{
							return MultiProxy.Empty;
						}
					}
				}
				if (uri.HostNameType != UriHostNameType.IPv6 && !uri.IdnHost.Contains('.'))
				{
					return MultiProxy.Empty;
				}
			}
			if (_bypass != null)
			{
				foreach (string item2 in _bypass)
				{
					if (SimpleRegex.IsMatchWithStarWildcard(uri.IdnHost.AsSpan(), item2.AsSpan()))
					{
						return MultiProxy.Empty;
					}
				}
			}
			if (!IsSecureUri(uri))
			{
				return _insecureProxy;
			}
			return _secureProxy;
		}
		return MultiProxy.Empty;
	}

	private static bool IsSecureUri(Uri uri)
	{
		if (!(uri.Scheme == "https"))
		{
			return uri.Scheme == "wss";
		}
		return true;
	}

	public bool IsBypassed(Uri uri)
	{
		return false;
	}
}
