using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Metrics;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

internal abstract class HttpConnectionBase : IDisposable, IHttpTrace
{
	protected readonly HttpConnectionPool _pool;

	private static long s_connectionCounter = -1L;

	private ConnectionMetrics _connectionMetrics;

	private bool _httpTelemetryMarkedConnectionAsOpened;

	private readonly long _creationTickCount = Environment.TickCount64;

	private long? _idleSinceTickCount;

	private string _lastDateHeaderValue;

	private string _lastServerHeaderValue;

	public long Id { get; } = Interlocked.Increment(ref s_connectionCounter);

	public Activity ConnectionSetupActivity { get; private set; }

	public HttpConnectionBase(HttpConnectionPool pool)
	{
		_pool = pool;
	}

	public HttpConnectionBase(HttpConnectionPool pool, Activity connectionSetupActivity, IPEndPoint remoteEndPoint)
		: this(pool)
	{
		MarkConnectionAsEstablished(connectionSetupActivity, remoteEndPoint);
	}

	protected void MarkConnectionAsEstablished(Activity connectionSetupActivity, IPEndPoint remoteEndPoint)
	{
		ConnectionSetupActivity = connectionSetupActivity;
		if (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
		{
			SocketsHttpHandlerMetrics metrics = _pool.Settings._metrics;
			if (metrics.OpenConnections.Enabled || metrics.ConnectionDuration.Enabled)
			{
				string protocolVersion = ((this is HttpConnection) ? "1.1" : ((this is Http2Connection) ? "2" : "3"));
				_connectionMetrics = new ConnectionMetrics(metrics, protocolVersion, _pool.IsSecure ? "https" : "http", _pool.TelemetryServerAddress, _pool.OriginAuthority.Port, remoteEndPoint?.Address?.ToString());
				_connectionMetrics.ConnectionEstablished();
			}
		}
		_idleSinceTickCount = _creationTickCount;
		if (HttpTelemetry.Log.IsEnabled())
		{
			_httpTelemetryMarkedConnectionAsOpened = true;
			string scheme = (_pool.IsSecure ? "https" : "http");
			string hostValue = _pool.OriginAuthority.HostValue;
			int port = _pool.OriginAuthority.Port;
			if (this is HttpConnection)
			{
				HttpTelemetry.Log.Http11ConnectionEstablished(Id, scheme, hostValue, port, remoteEndPoint);
			}
			else if (this is Http2Connection)
			{
				HttpTelemetry.Log.Http20ConnectionEstablished(Id, scheme, hostValue, port, remoteEndPoint);
			}
			else
			{
				HttpTelemetry.Log.Http30ConnectionEstablished(Id, scheme, hostValue, port, remoteEndPoint);
			}
		}
	}

	public void MarkConnectionAsClosed()
	{
		if (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
		{
			_connectionMetrics?.ConnectionClosed(Environment.TickCount64 - _creationTickCount);
		}
		if (HttpTelemetry.Log.IsEnabled() && _httpTelemetryMarkedConnectionAsOpened)
		{
			if (this is HttpConnection)
			{
				HttpTelemetry.Log.Http11ConnectionClosed(Id);
			}
			else if (this is Http2Connection)
			{
				HttpTelemetry.Log.Http20ConnectionClosed(Id);
			}
			else
			{
				HttpTelemetry.Log.Http30ConnectionClosed(Id);
			}
		}
	}

	public void MarkConnectionAsIdle()
	{
		_idleSinceTickCount = Environment.TickCount64;
		if (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
		{
			_connectionMetrics?.IdleStateChanged(idle: true);
		}
	}

	public void MarkConnectionAsNotIdle()
	{
		_idleSinceTickCount = null;
		if (GlobalHttpSettings.MetricsHandler.IsGloballyEnabled)
		{
			_connectionMetrics?.IdleStateChanged(idle: false);
		}
	}

	public string GetResponseHeaderValueWithCaching(HeaderDescriptor descriptor, ReadOnlySpan<byte> value, Encoding valueEncoding)
	{
		if (!descriptor.Equals(KnownHeaders.Date))
		{
			if (!descriptor.Equals(KnownHeaders.Server))
			{
				return descriptor.GetHeaderValue(value, valueEncoding);
			}
			return GetOrAddCachedValue(ref _lastServerHeaderValue, descriptor, value, valueEncoding);
		}
		return GetOrAddCachedValue(ref _lastDateHeaderValue, descriptor, value, valueEncoding);
		static string GetOrAddCachedValue([NotNull] ref string cache, HeaderDescriptor headerDescriptor, ReadOnlySpan<byte> readOnlySpan, Encoding encoding)
		{
			string text = cache;
			if (text == null || !Ascii.Equals(readOnlySpan, text.AsSpan()))
			{
				text = (cache = headerDescriptor.GetHeaderValue(readOnlySpan, encoding));
			}
			return text;
		}
	}

	public abstract void Trace(string message, [CallerMemberName] string memberName = null);

	protected void TraceConnection(Stream stream)
	{
		if (stream is SslStream sslStream)
		{
			Trace($"{this}. Id:{Id}, SslProtocol:{sslStream.SslProtocol}, NegotiatedApplicationProtocol:{sslStream.NegotiatedApplicationProtocol}, NegotiatedCipherSuite:{sslStream.NegotiatedCipherSuite}, CipherAlgorithm:{sslStream.CipherAlgorithm}, CipherStrength:{sslStream.CipherStrength}, HashAlgorithm:{sslStream.HashAlgorithm}, HashStrength:{sslStream.HashStrength}, KeyExchangeAlgorithm:{sslStream.KeyExchangeAlgorithm}, KeyExchangeStrength:{sslStream.KeyExchangeStrength}, LocalCertificate:{sslStream.LocalCertificate}, RemoteCertificate:{sslStream.RemoteCertificate}", "TraceConnection");
		}
		else
		{
			Trace($"{this}. Id:{Id}", "TraceConnection");
		}
	}

	public long GetLifetimeTicks(long nowTicks)
	{
		return nowTicks - _creationTickCount;
	}

	public long GetIdleTicks(long nowTicks)
	{
		long? idleSinceTickCount = _idleSinceTickCount;
		if (idleSinceTickCount.HasValue)
		{
			long valueOrDefault = idleSinceTickCount.GetValueOrDefault();
			return nowTicks - valueOrDefault;
		}
		return 0L;
	}

	public virtual bool CheckUsabilityOnScavenge()
	{
		return true;
	}

	internal static bool IsDigit(byte c)
	{
		return (uint)(c - 48) <= 9u;
	}

	internal static int ParseStatusCode(ReadOnlySpan<byte> value)
	{
		byte b;
		byte b2;
		byte b3;
		if (value.Length != 3 || !IsDigit(b = value[0]) || !IsDigit(b2 = value[1]) || !IsDigit(b3 = value[2]))
		{
			throw new HttpRequestException(HttpRequestError.InvalidResponse, System.SR.Format(System.SR.net_http_invalid_response_status_code, Encoding.ASCII.GetString(value)));
		}
		return 100 * (b - 48) + 10 * (b2 - 48) + (b3 - 48);
	}

	internal void LogExceptions(Task task)
	{
		if (task.IsCompleted)
		{
			if (task.IsFaulted)
			{
				LogFaulted(this, task);
			}
		}
		else
		{
			task.ContinueWith(delegate(Task t, object state)
			{
				LogFaulted((HttpConnectionBase)state, t);
			}, this, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		static void LogFaulted(HttpConnectionBase connection, Task task2)
		{
			Exception innerException = task2.Exception.InnerException;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				connection.Trace($"Exception from asynchronous processing: {innerException}", "LogExceptions");
			}
		}
	}

	public abstract void Dispose();

	public bool IsUsable(long nowTicks, TimeSpan pooledConnectionLifetime, TimeSpan pooledConnectionIdleTimeout)
	{
		if (pooledConnectionIdleTimeout != Timeout.InfiniteTimeSpan)
		{
			long idleTicks = GetIdleTicks(nowTicks);
			if ((double)idleTicks > pooledConnectionIdleTimeout.TotalMilliseconds)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Scavenging connection. Idle {TimeSpan.FromMilliseconds(idleTicks)} > {pooledConnectionIdleTimeout}.", "IsUsable");
				}
				return false;
			}
		}
		if (pooledConnectionLifetime != Timeout.InfiniteTimeSpan)
		{
			long lifetimeTicks = GetLifetimeTicks(nowTicks);
			if ((double)lifetimeTicks > pooledConnectionLifetime.TotalMilliseconds)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					Trace($"Scavenging connection. Lifetime {TimeSpan.FromMilliseconds(lifetimeTicks)} > {pooledConnectionLifetime}.", "IsUsable");
				}
				return false;
			}
		}
		if (!CheckUsabilityOnScavenge())
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				Trace("Scavenging connection. Keep-Alive timeout exceeded, unexpected data or EOF received.", "IsUsable");
			}
			return false;
		}
		return true;
	}
}
