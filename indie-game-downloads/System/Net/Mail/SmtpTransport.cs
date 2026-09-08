using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal sealed class SmtpTransport
{
	private readonly ISmtpAuthenticationModule[] _authenticationModules;

	private SmtpConnection _connection;

	private readonly SmtpClient _client;

	private ICredentialsByHost _credentials;

	private bool _shouldAbort;

	private bool _enableSsl;

	[CompilerGenerated]
	private X509CertificateCollection _003CClientCertificates_003Ek__BackingField;

	internal ICredentialsByHost Credentials
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

	internal bool IsConnected
	{
		get
		{
			if (_connection != null)
			{
				return _connection.IsConnected;
			}
			return false;
		}
	}

	internal bool EnableSsl
	{
		get
		{
			return _enableSsl;
		}
		set
		{
			_enableSsl = value;
		}
	}

	internal X509CertificateCollection ClientCertificates => _003CClientCertificates_003Ek__BackingField ?? (_003CClientCertificates_003Ek__BackingField = new X509CertificateCollection());

	internal bool ServerSupportsEai
	{
		get
		{
			if (_connection != null)
			{
				return _connection.ServerSupportsEai;
			}
			return false;
		}
	}

	internal SmtpTransport(SmtpClient client)
		: this(client, SmtpAuthenticationManager.GetModules())
	{
	}

	internal SmtpTransport(SmtpClient client, ISmtpAuthenticationModule[] authenticationModules)
	{
		ArgumentNullException.ThrowIfNull(authenticationModules, "authenticationModules");
		_client = client;
		_authenticationModules = authenticationModules;
	}

	internal Task GetConnectionAsync<TIOAdapter>(string host, int port, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		lock (this)
		{
			_connection = new SmtpConnection(this, _client, _credentials, _authenticationModules);
			if (_shouldAbort)
			{
				_connection.Abort();
			}
			_shouldAbort = false;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Associate(this, _connection, "GetConnectionAsync");
		}
		if (EnableSsl)
		{
			_connection.EnableSsl = true;
			_connection.ClientCertificates = ClientCertificates;
		}
		return _connection.GetConnectionAsync<TIOAdapter>(host, port, cancellationToken);
	}

	internal async Task<(MailWriter, List<SmtpFailedRecipientException>)> SendMailAsync<TIOAdapter>(MailAddress sender, MailAddressCollection recipients, string deliveryNotify, bool allowUnicode, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		ArgumentNullException.ThrowIfNull(sender, "sender");
		ArgumentNullException.ThrowIfNull(recipients, "recipients");
		await MailCommand.SendAsync<TIOAdapter>(_connection, SmtpCommands.Mail, sender, allowUnicode, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		List<SmtpFailedRecipientException> failedRecipientExceptions = null;
		foreach (MailAddress recipient in recipients)
		{
			string smtpAddress = recipient.GetSmtpAddress(allowUnicode);
			string to = smtpAddress + (_connection.DSNEnabled ? deliveryNotify : string.Empty);
			var (flag, serverResponse) = await RecipientCommand.SendAsync<TIOAdapter>(_connection, to, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (!flag)
			{
				List<SmtpFailedRecipientException> list = failedRecipientExceptions;
				if (list == null)
				{
					List<SmtpFailedRecipientException> list2;
					failedRecipientExceptions = (list2 = new List<SmtpFailedRecipientException>());
					list = list2;
				}
				list.Add(new SmtpFailedRecipientException(_connection.Reader.StatusCode, smtpAddress, serverResponse));
			}
		}
		List<SmtpFailedRecipientException> list3 = failedRecipientExceptions;
		if (list3 != null && list3.Count > 0 && failedRecipientExceptions.Count == recipients.Count)
		{
			SmtpFailedRecipientException obj = ((failedRecipientExceptions.Count == 1) ? failedRecipientExceptions[0] : new SmtpFailedRecipientsException(failedRecipientExceptions, allFailed: true));
			obj.fatal = true;
			throw obj;
		}
		await DataCommand.SendAsync<TIOAdapter>(_connection, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return (new MailWriter(_connection.GetClosableStream(), encodeForTransport: true), failedRecipientExceptions);
	}

	internal void ReleaseConnection()
	{
		_connection?.ReleaseConnection();
	}

	internal void Abort()
	{
		lock (this)
		{
			if (_connection != null)
			{
				_connection.Abort();
			}
			else
			{
				_shouldAbort = true;
			}
		}
	}
}
