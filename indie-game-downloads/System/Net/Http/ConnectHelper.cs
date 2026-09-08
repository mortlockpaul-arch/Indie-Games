using System.IO;
using System.Net.Quic;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

internal static class ConnectHelper
{
	internal sealed class CertificateCallbackMapper
	{
		public readonly Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> FromHttpClientHandler;

		public readonly RemoteCertificateValidationCallback ForSocketsHttpHandler;

		public CertificateCallbackMapper(Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> fromHttpClientHandler)
		{
			FromHttpClientHandler = fromHttpClientHandler;
			ForSocketsHttpHandler = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => FromHttpClientHandler((HttpRequestMessage)sender, certificate as X509Certificate2, chain, sslPolicyErrors);
		}
	}

	private static SslClientAuthenticationOptions SetUpRemoteCertificateValidationCallback(SslClientAuthenticationOptions sslOptions, HttpRequestMessage request)
	{
		RemoteCertificateValidationCallback remoteCertificateValidationCallback = sslOptions.RemoteCertificateValidationCallback;
		if (remoteCertificateValidationCallback != null && remoteCertificateValidationCallback.Target is CertificateCallbackMapper certificateCallbackMapper)
		{
			sslOptions = sslOptions.ShallowClone();
			Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> localFromHttpClientHandler = certificateCallbackMapper.FromHttpClientHandler;
			HttpRequestMessage localRequest = request;
			sslOptions.RemoteCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				bool result = localFromHttpClientHandler(localRequest, certificate as X509Certificate2, chain, sslPolicyErrors);
				localRequest = null;
				return result;
			};
		}
		return sslOptions;
	}

	public static async ValueTask<SslStream> EstablishSslConnectionAsync(SslClientAuthenticationOptions sslOptions, HttpRequestMessage request, bool async, Stream stream, CancellationToken cancellationToken)
	{
		sslOptions = SetUpRemoteCertificateValidationCallback(sslOptions, request);
		SslStream sslStream = new SslStream(stream);
		try
		{
			if (async)
			{
				await sslStream.AuthenticateAsClientAsync(sslOptions, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				using (cancellationToken.UnsafeRegister(delegate(object s)
				{
					((Stream)s).Dispose();
				}, stream))
				{
					sslStream.AuthenticateAsClient(sslOptions);
				}
			}
		}
		catch (Exception ex)
		{
			sslStream.Dispose();
			if (ex is OperationCanceledException)
			{
				throw;
			}
			if (CancellationHelper.ShouldWrapInOperationCanceledException(ex, cancellationToken))
			{
				throw CancellationHelper.CreateOperationCanceledException(ex, cancellationToken);
			}
			HttpRequestException ex2 = new HttpRequestException(HttpRequestError.SecureConnectionError, System.SR.net_http_ssl_connection_failed, ex);
			if (request.IsExtendedConnectRequest)
			{
				ex2.Data["HTTP2_ENABLED"] = false;
			}
			throw ex2;
		}
		if (cancellationToken.IsCancellationRequested)
		{
			sslStream.Dispose();
			throw CancellationHelper.CreateOperationCanceledException(null, cancellationToken);
		}
		return sslStream;
	}

	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	[SupportedOSPlatform("windows")]
	public static async ValueTask<QuicConnection> ConnectQuicAsync(HttpRequestMessage request, DnsEndPoint endPoint, TimeSpan idleTimeout, SslClientAuthenticationOptions clientAuthenticationOptions, Action<QuicConnection, QuicStreamCapacityChangedArgs> streamCapacityCallback, CancellationToken cancellationToken)
	{
		clientAuthenticationOptions = SetUpRemoteCertificateValidationCallback(clientAuthenticationOptions, request);
		try
		{
			return await QuicConnection.ConnectAsync(new QuicClientConnectionOptions
			{
				MaxInboundBidirectionalStreams = 0,
				MaxInboundUnidirectionalStreams = 5,
				IdleTimeout = idleTimeout,
				DefaultStreamErrorCode = 268L,
				DefaultCloseErrorCode = 256L,
				RemoteEndPoint = endPoint,
				ClientAuthenticationOptions = clientAuthenticationOptions,
				StreamCapacityCallback = streamCapacityCallback
			}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex) when (!(ex is OperationCanceledException))
		{
			throw CreateWrappedException(ex, endPoint.Host, endPoint.Port, cancellationToken);
		}
	}

	internal static Exception CreateWrappedException(Exception exception, string host, int port, CancellationToken cancellationToken)
	{
		if (!CancellationHelper.ShouldWrapInOperationCanceledException(exception, cancellationToken))
		{
			return new HttpRequestException(DeduceError(exception), $"{exception.Message} ({host}:{port})", exception, RequestRetryType.RetryOnNextProxy);
		}
		return CancellationHelper.CreateOperationCanceledException(exception, cancellationToken);
		static HttpRequestError DeduceError(Exception ex)
		{
			if (ex is AuthenticationException)
			{
				return HttpRequestError.SecureConnectionError;
			}
			SocketException ex2 = ex as SocketException;
			bool flag = ex2 != null;
			if (flag)
			{
				SocketError socketErrorCode = ex2.SocketErrorCode;
				bool flag2 = (uint)(socketErrorCode - 11001) <= 1u;
				flag = flag2;
			}
			if (flag)
			{
				return HttpRequestError.NameResolutionError;
			}
			return HttpRequestError.ConnectionError;
		}
	}
}
