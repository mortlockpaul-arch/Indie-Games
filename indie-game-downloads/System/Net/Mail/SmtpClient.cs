using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

[UnsupportedOSPlatform("browser")]
public class SmtpClient : IDisposable
{
	private string _host;

	private int _port;

	private int _timeout = 100000;

	private bool _inCall;

	private bool _timedOut;

	private string _targetName;

	private SmtpDeliveryMethod _deliveryMethod;

	private SmtpDeliveryFormat _deliveryFormat;

	private string _pickupDirectoryLocation;

	private SmtpTransport _transport;

	internal string _clientDomain;

	private bool _disposed;

	private CancellationTokenSource _pendingSendCts;

	private ServicePoint _servicePoint;

	private bool _useDefaultCredentials;

	private ICredentialsByHost _customCredentials;

	public string? Host
	{
		get
		{
			return _host;
		}
		[param: DisallowNull]
		set
		{
			if (_inCall)
			{
				throw new InvalidOperationException(System.SR.SmtpInvalidOperationDuringSend);
			}
			ArgumentException.ThrowIfNullOrEmpty(value, "value");
			value = value.Trim();
			if (value != _host)
			{
				_host = value;
				_servicePoint = null;
			}
		}
	}

	public int Port
	{
		get
		{
			return _port;
		}
		set
		{
			if (_inCall)
			{
				throw new InvalidOperationException(System.SR.SmtpInvalidOperationDuringSend);
			}
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, "value");
			if (value != _port)
			{
				_port = value;
				_servicePoint = null;
			}
		}
	}

	public bool UseDefaultCredentials
	{
		get
		{
			return _useDefaultCredentials;
		}
		set
		{
			if (_inCall)
			{
				throw new InvalidOperationException(System.SR.SmtpInvalidOperationDuringSend);
			}
			_useDefaultCredentials = value;
			UpdateTransportCredentials();
		}
	}

	public ICredentialsByHost? Credentials
	{
		get
		{
			return _transport.Credentials;
		}
		set
		{
			if (_inCall)
			{
				throw new InvalidOperationException(System.SR.SmtpInvalidOperationDuringSend);
			}
			_customCredentials = value;
			UpdateTransportCredentials();
		}
	}

	public int Timeout
	{
		get
		{
			return _timeout;
		}
		set
		{
			if (_inCall)
			{
				throw new InvalidOperationException(System.SR.SmtpInvalidOperationDuringSend);
			}
			ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
			_timeout = value;
		}
	}

	public ServicePoint ServicePoint
	{
		get
		{
			CheckHostAndPort();
			return _servicePoint ?? (_servicePoint = ServicePointManager.FindServicePoint(new Uri($"mailto:{_host}:{_port}")));
		}
	}

	public SmtpDeliveryMethod DeliveryMethod
	{
		get
		{
			return _deliveryMethod;
		}
		set
		{
			_deliveryMethod = value;
		}
	}

	public SmtpDeliveryFormat DeliveryFormat
	{
		get
		{
			return _deliveryFormat;
		}
		set
		{
			_deliveryFormat = value;
		}
	}

	public string? PickupDirectoryLocation
	{
		get
		{
			return _pickupDirectoryLocation;
		}
		set
		{
			_pickupDirectoryLocation = value;
		}
	}

	public bool EnableSsl
	{
		get
		{
			return _transport.EnableSsl;
		}
		set
		{
			_transport.EnableSsl = value;
		}
	}

	public X509CertificateCollection ClientCertificates => _transport.ClientCertificates;

	public string? TargetName
	{
		get
		{
			return _targetName;
		}
		set
		{
			_targetName = value;
		}
	}

	private bool ServerSupportsEai => _transport.ServerSupportsEai;

	public event SendCompletedEventHandler? SendCompleted;

	public SmtpClient()
	{
		Initialize();
	}

	public SmtpClient(string? host)
	{
		_host = host;
		Initialize();
	}

	public SmtpClient(string? host, int port)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(port, "port");
		_host = host;
		_port = port;
		Initialize();
	}

	[MemberNotNull("_transport")]
	[MemberNotNull("_clientDomain")]
	[MemberNotNull("_pendingSendCts")]
	private void Initialize()
	{
		_transport = new SmtpTransport(this);
		_pendingSendCts = new CancellationTokenSource();
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Associate(this, _transport, "Initialize");
		}
		if (!string.IsNullOrEmpty(_host))
		{
			_host = _host.Trim();
		}
		if (_port == 0)
		{
			_port = 25;
		}
		if (_targetName == null)
		{
			_targetName = "SMTPSVC/" + _host;
		}
		if (_clientDomain != null)
		{
			return;
		}
		string text = IPGlobalProperties.GetIPGlobalProperties().HostName;
		IdnMapping idnMapping = new IdnMapping();
		try
		{
			text = idnMapping.GetAscii(text);
		}
		catch (ArgumentException)
		{
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char value in text)
		{
			if (Ascii.IsValid(value))
			{
				stringBuilder.Append(value);
			}
		}
		if (stringBuilder.Length > 0)
		{
			_clientDomain = stringBuilder.ToString();
		}
		else
		{
			_clientDomain = "LocalHost";
		}
	}

	private void UpdateTransportCredentials()
	{
		SmtpTransport transport = _transport;
		ICredentialsByHost credentials;
		if (!_useDefaultCredentials)
		{
			credentials = _customCredentials;
		}
		else
		{
			ICredentialsByHost defaultNetworkCredentials = CredentialCache.DefaultNetworkCredentials;
			credentials = defaultNetworkCredentials;
		}
		transport.Credentials = credentials;
	}

	private bool IsUnicodeSupported()
	{
		if (DeliveryMethod == SmtpDeliveryMethod.Network)
		{
			if (ServerSupportsEai)
			{
				return DeliveryFormat == SmtpDeliveryFormat.International;
			}
			return false;
		}
		return DeliveryFormat == SmtpDeliveryFormat.International;
	}

	internal MailWriter GetFileMailWriter(string pickupDirectory)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, FormattableStringFactory.Create("{0}={1}", "pickupDirectory", pickupDirectory), "GetFileMailWriter");
		}
		if (!Path.IsPathRooted(pickupDirectory))
		{
			throw new SmtpException(System.SR.SmtpNeedAbsolutePickupDirectory);
		}
		string path2;
		do
		{
			string path = $"{Guid.NewGuid()}.eml";
			path2 = Path.Combine(pickupDirectory, path);
		}
		while (File.Exists(path2));
		return new MailWriter(new FileStream(path2, FileMode.CreateNew), encodeForTransport: false);
	}

	protected void OnSendCompleted(AsyncCompletedEventArgs e)
	{
		SendCompleted?.Invoke(this, e);
	}

	public void Send(string from, string recipients, string? subject, string? body)
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		MailMessage message = new MailMessage(from, recipients, subject, body);
		Send(message);
	}

	public void Send(MailMessage message)
	{
		Exception item = SendAsyncInternal<System.Net.SyncReadWriteAdapter>(message, invokeSendCompleted: false, null).GetAwaiter().GetResult().ex;
		if (item != null)
		{
			ExceptionDispatchInfo.Throw(item);
		}
	}

	private async Task<(Exception ex, bool synchronous)> SendAsyncInternal<TIOAdapter>(MailMessage message, bool invokeSendCompleted, object userToken, bool forceWrapExceptions = false, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter?
	{
		if (_disposed)
		{
			return (ex: ExceptionDispatchInfo.SetCurrentStackTrace(new ObjectDisposedException(typeof(SmtpClient).FullName)), synchronous: true);
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, $"DeliveryMethod={DeliveryMethod}", "SendAsyncInternal");
			System.Net.NetEventSource.Associate(this, message, "SendAsyncInternal");
		}
		if (cancellationToken.CanBeCanceled)
		{
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_pendingSendCts.Token, cancellationToken);
			cancellationToken = cancellationTokenSource.Token;
		}
		else
		{
			cancellationToken = _pendingSendCts.Token;
		}
		if (Interlocked.Exchange(ref _inCall, value: true))
		{
			return (ex: ExceptionDispatchInfo.SetCurrentStackTrace(new InvalidOperationException(System.SR.net_inasync)), synchronous: true);
		}
		bool synchronous = true;
		bool canceled = false;
		Timer timer = null;
		Exception exception = null;
		try
		{
			ArgumentNullException.ThrowIfNull(message, "message");
			if (DeliveryMethod == SmtpDeliveryMethod.Network)
			{
				CheckHostAndPort();
			}
			MailAddressCollection recipients = new MailAddressCollection();
			if (message.From == null)
			{
				throw new InvalidOperationException(System.SR.SmtpFromRequired);
			}
			if (message.To != null)
			{
				foreach (MailAddress item in message.To)
				{
					recipients.Add(item);
				}
			}
			if (message.Bcc != null)
			{
				foreach (MailAddress item2 in message.Bcc)
				{
					recipients.Add(item2);
				}
			}
			if (message.CC != null)
			{
				foreach (MailAddress item3 in message.CC)
				{
					recipients.Add(item3);
				}
			}
			if (recipients.Count == 0)
			{
				throw new InvalidOperationException(System.SR.SmtpRecipientRequired);
			}
			forceWrapExceptions = true;
			_timedOut = false;
			timer = new Timer(TimeOutCallback, null, Timeout, Timeout);
			string pickupDirectoryLocation = PickupDirectoryLocation;
			List<SmtpFailedRecipientException> failedRecipientExceptions = null;
			bool allowUnicode;
			MailWriter writer;
			switch (DeliveryMethod)
			{
			case SmtpDeliveryMethod.PickupDirectoryFromIis:
				throw new NotSupportedException(System.SR.SmtpGetIisPickupDirectoryNotSupported);
			case SmtpDeliveryMethod.SpecifiedPickupDirectory:
				if (EnableSsl)
				{
					throw new SmtpException(System.SR.SmtpPickupDirectoryDoesnotSupportSsl);
				}
				allowUnicode = IsUnicodeSupported();
				ValidateUnicodeRequirement(message, recipients, allowUnicode);
				writer = GetFileMailWriter(pickupDirectoryLocation);
				break;
			default:
				synchronous = false;
				await EnsureConnection<TIOAdapter>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				allowUnicode = IsUnicodeSupported();
				ValidateUnicodeRequirement(message, recipients, allowUnicode);
				(writer, failedRecipientExceptions) = await _transport.SendMailAsync<TIOAdapter>(message.Sender ?? message.From, recipients, message.BuildDeliveryStatusNotificationString(), allowUnicode, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				break;
			}
			synchronous = false;
			await message.SendAsync<TIOAdapter>(writer, DeliveryMethod != SmtpDeliveryMethod.Network, allowUnicode, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			writer.Close();
			if (failedRecipientExceptions != null)
			{
				throw (failedRecipientExceptions.Count == 1) ? failedRecipientExceptions[0] : new SmtpFailedRecipientsException(failedRecipientExceptions, allFailed: false);
			}
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(this, ex, "SendAsyncInternal");
			}
			exception = ProcessException(ex, ref canceled, forceWrapExceptions, _timedOut);
		}
		finally
		{
			_inCall = false;
			timer?.Dispose();
			if (invokeSendCompleted && !synchronous)
			{
				AsyncCompletedEventArgs e = new AsyncCompletedEventArgs(canceled ? null : exception, canceled, userToken);
				OnSendCompleted(e);
			}
		}
		return (ex: exception, synchronous: synchronous);
		Exception ProcessException(Exception ex2, ref bool reference, bool flag2, bool timedOut)
		{
			if (ex2 is SmtpFailedRecipientException && !((SmtpFailedRecipientException)ex2).fatal)
			{
				return ex2;
			}
			reference = ex2 is OperationCanceledException;
			Abort();
			if (timedOut)
			{
				return ExceptionDispatchInfo.SetCurrentStackTrace(new SmtpException(System.SR.net_timeout));
			}
			bool flag = !flag2;
			if (!flag)
			{
				bool flag3 = typeof(TIOAdapter) == typeof(System.Net.SyncReadWriteAdapter);
				if (flag3)
				{
					bool flag4 = ((ex2 is SecurityException || ex2 is AuthenticationException) ? true : false);
					flag3 = flag4;
				}
				flag = flag3;
			}
			if (flag || ex2 is SmtpException || ex2 is OperationCanceledException)
			{
				return ex2;
			}
			return ExceptionDispatchInfo.SetCurrentStackTrace(new SmtpException(System.SR.SmtpSendMailFailure, ex2));
		}
	}

	public void SendAsync(string from, string recipients, string? subject, string? body, object? userToken)
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		SendAsync(new MailMessage(from, recipients, subject, body), userToken);
	}

	public void SendAsync(MailMessage message, object? userToken)
	{
		Task<(Exception, bool)> task = SendAsyncInternal<System.Net.AsyncReadWriteAdapter>(message, invokeSendCompleted: true, userToken, forceWrapExceptions: true);
		if (task.IsCompleted)
		{
			var (ex, flag) = task.GetAwaiter().GetResult();
			if ((ex != null) & flag)
			{
				ExceptionDispatchInfo.Throw(ex);
			}
		}
	}

	public void SendAsyncCancel()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
		if (_inCall)
		{
			CancellationTokenSource cancellationTokenSource = Interlocked.Exchange(ref _pendingSendCts, new CancellationTokenSource());
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
		}
	}

	public Task SendMailAsync(string from, string recipients, string? subject, string? body)
	{
		MailMessage message = new MailMessage(from, recipients, subject, body);
		return SendMailAsync(message, default(CancellationToken));
	}

	public Task SendMailAsync(MailMessage message)
	{
		return SendMailAsync(message, default(CancellationToken));
	}

	public Task SendMailAsync(string from, string recipients, string? subject, string? body, CancellationToken cancellationToken)
	{
		MailMessage message = new MailMessage(from, recipients, subject, body);
		return SendMailAsync(message, cancellationToken);
	}

	public Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		Task<(Exception, bool)> task = SendAsyncInternal<System.Net.AsyncReadWriteAdapter>(message, invokeSendCompleted: false, null, forceWrapExceptions: true, cancellationToken);
		if (task.IsCompleted)
		{
			var (ex, flag) = task.GetAwaiter().GetResult();
			if (ex != null)
			{
				if (flag)
				{
					ExceptionDispatchInfo.Throw(ex);
				}
				return Task.FromException(ex);
			}
			return Task.CompletedTask;
		}
		return WaitAndRethrowIfNeeded(task);
		static async Task WaitAndRethrowIfNeeded(Task<(Exception ex, bool _)> task2)
		{
			Exception item = (await task2.ConfigureAwait(continueOnCapturedContext: false)).Item1;
			if (item != null)
			{
				ExceptionDispatchInfo.Throw(item);
			}
		}
	}

	private void CheckHostAndPort()
	{
		if (string.IsNullOrEmpty(_host))
		{
			throw new InvalidOperationException(System.SR.UnspecifiedHost);
		}
		if (_port <= 0 || _port > 65535)
		{
			throw new InvalidOperationException(System.SR.InvalidPort);
		}
	}

	private void TimeOutCallback(object state)
	{
		if (!_timedOut)
		{
			_timedOut = true;
			Abort();
		}
	}

	private static void ValidateUnicodeRequirement(MailMessage message, MailAddressCollection recipients, bool allowUnicode)
	{
		foreach (MailAddress recipient in recipients)
		{
			recipient.GetSmtpAddress(allowUnicode);
		}
		message.Sender?.GetSmtpAddress(allowUnicode);
		message.From.GetSmtpAddress(allowUnicode);
	}

	private Task EnsureConnection<TIOAdapter>(CancellationToken cancellationToken) where TIOAdapter : System.Net.IReadWriteAdapter?
	{
		if (_transport.IsConnected)
		{
			return Task.CompletedTask;
		}
		return _transport.GetConnectionAsync<TIOAdapter>(_host, _port, cancellationToken);
	}

	private void Abort()
	{
		_transport.Abort();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
			if (_inCall)
			{
				_pendingSendCts.Cancel();
				Abort();
			}
			else
			{
				_transport?.ReleaseConnection();
			}
		}
	}
}
