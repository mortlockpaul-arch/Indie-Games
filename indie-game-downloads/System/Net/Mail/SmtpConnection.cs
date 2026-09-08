using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal sealed class SmtpConnection
{
	private readonly BufferBuilder _bufferBuilder = new BufferBuilder();

	private bool _isConnected;

	private bool _isClosed;

	private bool _isStreamOpen;

	private readonly EventHandler _onCloseHandler;

	internal SmtpTransport _parent;

	private readonly SmtpClient _client;

	private Stream _stream;

	internal TcpClient _tcpClient;

	private SmtpReplyReaderFactory _responseReader;

	private readonly ICredentialsByHost _credentials;

	private string[] _extensions;

	private bool _enableSsl;

	private X509CertificateCollection _clientCertificates;

	private bool _serverSupportsEai;

	private bool _dsnEnabled;

	private bool _serverSupportsStartTls;

	private bool _sawNegotiate;

	private SupportedAuth _supportedAuth;

	private readonly ISmtpAuthenticationModule[] _authenticationModules;

	private static readonly char[] s_authExtensionSplitters = new char[2] { ' ', '=' };

	internal BufferBuilder BufferBuilder => _bufferBuilder;

	internal bool IsConnected => _isConnected;

	internal bool IsStreamOpen => _isStreamOpen;

	internal SmtpReplyReaderFactory Reader => _responseReader;

	internal bool EnableSsl
	{
		set
		{
			_enableSsl = value;
		}
	}

	internal X509CertificateCollection ClientCertificates
	{
		set
		{
			_clientCertificates = value;
		}
	}

	internal bool DSNEnabled => _dsnEnabled;

	internal bool ServerSupportsEai => _serverSupportsEai;

	internal SmtpConnection(SmtpTransport parent, SmtpClient client, ICredentialsByHost credentials, ISmtpAuthenticationModule[] authenticationModules)
	{
		_client = client;
		_credentials = credentials;
		_authenticationModules = authenticationModules;
		_parent = parent;
		_tcpClient = new TcpClient();
		_onCloseHandler = OnClose;
	}

	internal void InitializeConnection(string host, int port)
	{
		_tcpClient.Connect(host, port);
		_stream = _tcpClient.GetStream();
	}

	internal async Task InitializeConnectionAsync(string host, int port)
	{
		await _tcpClient.ConnectAsync(host, port).ConfigureAwait(continueOnCapturedContext: false);
		_stream = _tcpClient.GetStream();
	}

	internal async Task GetConnectionAsync<TIOAdapter>(string host, int port, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		if (_isConnected)
		{
			throw new InvalidOperationException(System.SR.SmtpAlreadyConnected);
		}
		bool isAsync = typeof(TIOAdapter) == typeof(System.Net.AsyncReadWriteAdapter);
		if (isAsync)
		{
			await InitializeConnectionAsync(host, port).ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			InitializeConnection(host, port);
		}
		_responseReader = new SmtpReplyReaderFactory(_stream);
		LineInfo lineInfo = await _responseReader.GetNextReplyReader().ReadLineAsync<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (lineInfo.StatusCode != SmtpStatusCode.ServiceReady)
		{
			throw new SmtpException(lineInfo.StatusCode, lineInfo.Line, _: true);
		}
		try
		{
			_extensions = await EHelloCommand.SendAsync<TIOAdapter>(this, _client._clientDomain, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ParseExtensions(_extensions);
		}
		catch (SmtpException ex)
		{
			if (ex.StatusCode != SmtpStatusCode.CommandUnrecognized && ex.StatusCode != SmtpStatusCode.CommandNotImplemented)
			{
				throw;
			}
			await HelloCommand.SendAsync<TIOAdapter>(this, _client._clientDomain, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			_supportedAuth = SupportedAuth.Login;
		}
		if (_enableSsl)
		{
			if (!_serverSupportsStartTls && !(_stream is SslStream))
			{
				throw new SmtpException(System.SR.MailServerDoesNotSupportStartTls);
			}
			await StartTlsCommand.SendAsync<TIOAdapter>(this, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			SslStream sslStream = new SslStream(_stream, leaveInnerStreamOpen: false, ServicePointManager.ServerCertificateValidationCallback);
			if (isAsync)
			{
				await sslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
				{
					TargetHost = host,
					ClientCertificates = _clientCertificates,
					EnabledSslProtocols = (SslProtocols)ServicePointManager.SecurityProtocol,
					CertificateRevocationCheckMode = (ServicePointManager.CheckCertificateRevocationList ? X509RevocationMode.Online : X509RevocationMode.NoCheck)
				}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				sslStream.AuthenticateAsClient(host, _clientCertificates, (SslProtocols)ServicePointManager.SecurityProtocol, ServicePointManager.CheckCertificateRevocationList);
			}
			_stream = sslStream;
			_responseReader = new SmtpReplyReaderFactory(_stream);
			_extensions = await EHelloCommand.SendAsync<TIOAdapter>(this, _client._clientDomain, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ParseExtensions(_extensions);
		}
		if (_credentials != null)
		{
			for (int i = 0; i < _authenticationModules.Length; i++)
			{
				if (!AuthSupported(_authenticationModules[i]))
				{
					continue;
				}
				NetworkCredential credential = _credentials.GetCredential(host, port, _authenticationModules[i].AuthenticationType);
				if (credential == null)
				{
					continue;
				}
				Authorization authorization = SetContextAndTryAuthenticate(_authenticationModules[i], credential);
				if (authorization == null || authorization.Message == null)
				{
					continue;
				}
				lineInfo = await AuthCommand.SendAsync<TIOAdapter>(this, _authenticationModules[i].AuthenticationType, authorization.Message, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (lineInfo.StatusCode == SmtpStatusCode.CommandParameterNotImplemented)
				{
					continue;
				}
				while (lineInfo.StatusCode == (SmtpStatusCode)334)
				{
					authorization = _authenticationModules[i].Authenticate(lineInfo.Line, null, this, _client.TargetName, null);
					if (authorization == null)
					{
						throw new SmtpException(System.SR.SmtpAuthenticationFailed);
					}
					lineInfo = await AuthCommand.SendAsync<TIOAdapter>(this, authorization.Message, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (lineInfo.StatusCode == (SmtpStatusCode)235)
					{
						_authenticationModules[i].CloseContext(this);
						_isConnected = true;
						return;
					}
				}
			}
		}
		_isConnected = true;
	}

	internal async Task FlushAsync<TIOAdapter>(CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		await TIOAdapter.WriteAsync(_stream, _bufferBuilder.GetBuffer().AsMemory(0, _bufferBuilder.Length), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		_bufferBuilder.Reset();
	}

	private void ShutdownConnection(bool isAbort)
	{
		if (!_isClosed)
		{
			lock (this)
			{
				if (!_isClosed && _tcpClient != null)
				{
					try
					{
						try
						{
							if (isAbort)
							{
								_tcpClient.LingerState = new LingerOption(enable: true, 0);
							}
							else
							{
								_tcpClient.Client.Blocking = false;
								QuitCommand.SendAsync<System.Net.SyncReadWriteAdapter>(this).GetAwaiter().GetResult();
							}
						}
						finally
						{
							_stream?.Close();
							_tcpClient.Dispose();
						}
					}
					catch (IOException)
					{
					}
					catch (ObjectDisposedException)
					{
					}
				}
				_isClosed = true;
			}
		}
		_isConnected = false;
	}

	internal void ReleaseConnection()
	{
		ShutdownConnection(isAbort: false);
	}

	internal void Abort()
	{
		ShutdownConnection(isAbort: true);
	}

	private Authorization SetContextAndTryAuthenticate(ISmtpAuthenticationModule module, NetworkCredential credential)
	{
		return module.Authenticate(null, credential, this, _client.TargetName, null);
	}

	internal Stream GetClosableStream()
	{
		ClosableStream result = new ClosableStream(_stream, _onCloseHandler);
		_isStreamOpen = true;
		return result;
	}

	private void OnClose(object sender, EventArgs args)
	{
		_isStreamOpen = false;
		DataStopCommand.SendAsync<System.Net.SyncReadWriteAdapter>(this).GetAwaiter().GetResult();
	}

	internal void ParseExtensions(string[] extensions)
	{
		_supportedAuth = SupportedAuth.None;
		foreach (string text in extensions)
		{
			if (string.Compare(text, 0, "auth", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
			{
				string[] array = text.Remove(0, 4).Split(s_authExtensionSplitters, StringSplitOptions.RemoveEmptyEntries);
				foreach (string a in array)
				{
					if (string.Equals(a, "login", StringComparison.OrdinalIgnoreCase))
					{
						_supportedAuth |= SupportedAuth.Login;
					}
					else if (string.Equals(a, "ntlm", StringComparison.OrdinalIgnoreCase))
					{
						_supportedAuth |= SupportedAuth.NTLM;
					}
					else if (string.Equals(a, "gssapi", StringComparison.OrdinalIgnoreCase))
					{
						_supportedAuth |= SupportedAuth.GSSAPI;
					}
				}
			}
			else if (string.Compare(text, 0, "dsn ", 0, 3, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_dsnEnabled = true;
			}
			else if (string.Compare(text, 0, "STARTTLS", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_serverSupportsStartTls = true;
			}
			else if (string.Compare(text, 0, "SMTPUTF8", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
			{
				_serverSupportsEai = true;
			}
		}
	}

	internal bool AuthSupported(ISmtpAuthenticationModule module)
	{
		if (module is SmtpLoginAuthenticationModule)
		{
			if ((_supportedAuth & SupportedAuth.Login) > SupportedAuth.None)
			{
				return true;
			}
		}
		else if (module is SmtpNegotiateAuthenticationModule)
		{
			if ((_supportedAuth & SupportedAuth.GSSAPI) > SupportedAuth.None)
			{
				_sawNegotiate = true;
				return true;
			}
		}
		else if (module is SmtpNtlmAuthenticationModule && !_sawNegotiate && (_supportedAuth & SupportedAuth.NTLM) > SupportedAuth.None)
		{
			return true;
		}
		return false;
	}
}
