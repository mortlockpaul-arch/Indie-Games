using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mail;

internal sealed class Message
{
	private MailAddress _from;

	private MailAddress _sender;

	private MailAddress _replyTo;

	private MailAddressCollection _to;

	private MimeBasePart _content;

	private HeaderCollection _headers;

	private HeaderCollection _envelopeHeaders;

	private string _subject;

	private Encoding _subjectEncoding;

	private Encoding _headersEncoding;

	private MailPriority _priority = (MailPriority)(-1);

	[CompilerGenerated]
	private MailAddressCollection _003CReplyToList_003Ek__BackingField;

	[CompilerGenerated]
	private MailAddressCollection _003CBcc_003Ek__BackingField;

	[CompilerGenerated]
	private MailAddressCollection _003CCC_003Ek__BackingField;

	public MailPriority Priority
	{
		get
		{
			if (_priority != (MailPriority)(-1))
			{
				return _priority;
			}
			return MailPriority.Normal;
		}
		set
		{
			_priority = value;
		}
	}

	internal MailAddress From
	{
		get
		{
			return _from;
		}
		[param: DisallowNull]
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			_from = value;
		}
	}

	internal MailAddress Sender
	{
		get
		{
			return _sender;
		}
		set
		{
			_sender = value;
		}
	}

	internal MailAddress ReplyTo
	{
		get
		{
			return _replyTo;
		}
		set
		{
			_replyTo = value;
		}
	}

	internal MailAddressCollection ReplyToList => _003CReplyToList_003Ek__BackingField ?? (_003CReplyToList_003Ek__BackingField = new MailAddressCollection());

	internal MailAddressCollection To => _to ?? (_to = new MailAddressCollection());

	internal MailAddressCollection Bcc => _003CBcc_003Ek__BackingField ?? (_003CBcc_003Ek__BackingField = new MailAddressCollection());

	internal MailAddressCollection CC => _003CCC_003Ek__BackingField ?? (_003CCC_003Ek__BackingField = new MailAddressCollection());

	internal string Subject
	{
		get
		{
			return _subject;
		}
		set
		{
			Encoding encoding = null;
			try
			{
				encoding = MimeBasePart.DecodeEncoding(value);
			}
			catch (ArgumentException)
			{
			}
			if (encoding != null && value != null)
			{
				try
				{
					value = MimeBasePart.DecodeHeaderValue(value);
					if (_subjectEncoding == null)
					{
						_subjectEncoding = encoding;
					}
				}
				catch (FormatException)
				{
				}
			}
			if (value != null && MailBnfHelper.HasCROrLF(value))
			{
				throw new ArgumentException(System.SR.MailSubjectInvalidFormat);
			}
			_subject = value;
			if (_subject != null)
			{
				_subject = _subject.Normalize(NormalizationForm.FormC);
				if (_subjectEncoding == null && !MimeBasePart.IsAscii(_subject, permitCROrLF: false))
				{
					_subjectEncoding = Encoding.GetEncoding("utf-8");
				}
			}
		}
	}

	internal Encoding SubjectEncoding
	{
		get
		{
			return _subjectEncoding;
		}
		set
		{
			_subjectEncoding = value;
		}
	}

	internal HeaderCollection Headers
	{
		get
		{
			if (_headers == null)
			{
				_headers = new HeaderCollection();
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Associate(this, _headers, "Headers");
				}
			}
			return _headers;
		}
	}

	internal Encoding HeadersEncoding
	{
		get
		{
			return _headersEncoding;
		}
		set
		{
			_headersEncoding = value;
		}
	}

	internal HeaderCollection EnvelopeHeaders
	{
		get
		{
			if (_envelopeHeaders == null)
			{
				_envelopeHeaders = new HeaderCollection();
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Associate(this, _envelopeHeaders, "EnvelopeHeaders");
				}
			}
			return _envelopeHeaders;
		}
	}

	internal MimeBasePart Content
	{
		get
		{
			return _content;
		}
		[param: DisallowNull]
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			_content = value;
		}
	}

	internal Message()
	{
	}

	internal Message(string from, string to)
		: this()
	{
		ArgumentException.ThrowIfNullOrEmpty(from, "from");
		ArgumentException.ThrowIfNullOrEmpty(to, "to");
		_from = new MailAddress(from);
		_to = new MailAddressCollection { to };
	}

	internal Message(MailAddress from, MailAddress to)
		: this()
	{
		_from = from;
		To.Add(to);
	}

	internal async Task SendAsync<TIOAdapter>(BaseWriter writer, bool sendEnvelope, bool allowUnicode, CancellationToken cancellationToken = default(CancellationToken)) where TIOAdapter : System.Net.IReadWriteAdapter
	{
		if (sendEnvelope)
		{
			PrepareEnvelopeHeaders(allowUnicode);
			writer.WriteHeaders(EnvelopeHeaders, allowUnicode);
		}
		PrepareHeaders(allowUnicode);
		writer.WriteHeaders(Headers, allowUnicode);
		if (Content != null)
		{
			await Content.SendAsync<TIOAdapter>(writer, allowUnicode, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			writer.GetContentStream().Close();
		}
	}

	internal void PrepareEnvelopeHeaders(bool allowUnicode)
	{
		if (_headersEncoding == null)
		{
			_headersEncoding = Encoding.GetEncoding("utf-8");
		}
		EncodeHeaders(EnvelopeHeaders, allowUnicode);
		string text = MailHeaderInfo.GetString(MailHeaderID.XSender);
		if (!IsHeaderSet(text))
		{
			MailAddress mailAddress = Sender ?? From;
			EnvelopeHeaders.InternalSet(text, mailAddress.Encode(text.Length, allowUnicode));
		}
		string text2 = MailHeaderInfo.GetString(MailHeaderID.XReceiver);
		EnvelopeHeaders.Remove(text2);
		foreach (MailAddress item in To)
		{
			EnvelopeHeaders.InternalAdd(text2, item.Encode(text2.Length, allowUnicode));
		}
		foreach (MailAddress item2 in CC)
		{
			EnvelopeHeaders.InternalAdd(text2, item2.Encode(text2.Length, allowUnicode));
		}
		foreach (MailAddress item3 in Bcc)
		{
			EnvelopeHeaders.InternalAdd(text2, item3.Encode(text2.Length, allowUnicode));
		}
	}

	internal void PrepareHeaders(bool allowUnicode)
	{
		if (_headersEncoding == null)
		{
			_headersEncoding = Encoding.GetEncoding("utf-8");
		}
		Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentType));
		Headers[MailHeaderInfo.GetString(MailHeaderID.MimeVersion)] = "1.0";
		string text = MailHeaderInfo.GetString(MailHeaderID.Sender);
		if (Sender != null)
		{
			Headers.InternalAdd(text, Sender.Encode(text.Length, allowUnicode));
		}
		else
		{
			Headers.Remove(text);
		}
		text = MailHeaderInfo.GetString(MailHeaderID.From);
		Headers.InternalAdd(text, From.Encode(text.Length, allowUnicode));
		text = MailHeaderInfo.GetString(MailHeaderID.To);
		if (To.Count > 0)
		{
			Headers.InternalAdd(text, To.Encode(text.Length, allowUnicode));
		}
		else
		{
			Headers.Remove(text);
		}
		text = MailHeaderInfo.GetString(MailHeaderID.Cc);
		if (CC.Count > 0)
		{
			Headers.InternalAdd(text, CC.Encode(text.Length, allowUnicode));
		}
		else
		{
			Headers.Remove(text);
		}
		text = MailHeaderInfo.GetString(MailHeaderID.ReplyTo);
		if (ReplyTo != null)
		{
			Headers.InternalAdd(text, ReplyTo.Encode(text.Length, allowUnicode));
		}
		else if (ReplyToList.Count > 0)
		{
			Headers.InternalAdd(text, ReplyToList.Encode(text.Length, allowUnicode));
		}
		else
		{
			Headers.Remove(text);
		}
		Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Bcc));
		if (_priority == MailPriority.High)
		{
			Headers[MailHeaderInfo.GetString(MailHeaderID.XPriority)] = "1";
			Headers[MailHeaderInfo.GetString(MailHeaderID.Priority)] = "urgent";
			Headers[MailHeaderInfo.GetString(MailHeaderID.Importance)] = "high";
		}
		else if (_priority == MailPriority.Low)
		{
			Headers[MailHeaderInfo.GetString(MailHeaderID.XPriority)] = "5";
			Headers[MailHeaderInfo.GetString(MailHeaderID.Priority)] = "non-urgent";
			Headers[MailHeaderInfo.GetString(MailHeaderID.Importance)] = "low";
		}
		else if (_priority != (MailPriority)(-1))
		{
			Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.XPriority));
			Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Priority));
			Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Importance));
		}
		Headers.InternalAdd(MailHeaderInfo.GetString(MailHeaderID.Date), MailBnfHelper.GetDateTimeString(DateTime.Now, null));
		text = MailHeaderInfo.GetString(MailHeaderID.Subject);
		if (!string.IsNullOrEmpty(_subject))
		{
			if (allowUnicode)
			{
				Headers.InternalAdd(text, _subject);
			}
			else
			{
				Headers.InternalAdd(text, MimeBasePart.EncodeHeaderValue(_subject, _subjectEncoding, MimeBasePart.ShouldUseBase64Encoding(_subjectEncoding), text.Length));
			}
		}
		else
		{
			Headers.Remove(text);
		}
		EncodeHeaders(_headers, allowUnicode);
	}

	internal void EncodeHeaders(HeaderCollection headers, bool allowUnicode)
	{
		if (_headersEncoding == null)
		{
			_headersEncoding = Encoding.GetEncoding("utf-8");
		}
		for (int i = 0; i < headers.Count; i++)
		{
			string key = headers.GetKey(i);
			if (!MailHeaderInfo.IsUserSettable(key))
			{
				continue;
			}
			string[] values = headers.GetValues(key);
			for (int j = 0; j < values.Length; j++)
			{
				string value = ((!MimeBasePart.IsAscii(values[j], permitCROrLF: false) && (!allowUnicode || !MailHeaderInfo.AllowsUnicode(key) || MailBnfHelper.HasCROrLF(values[j]))) ? MimeBasePart.EncodeHeaderValue(values[j], _headersEncoding, MimeBasePart.ShouldUseBase64Encoding(_headersEncoding), key.Length) : values[j]);
				if (j == 0)
				{
					headers.Set(key, value);
				}
				else
				{
					headers.Add(key, value);
				}
			}
		}
	}

	private bool IsHeaderSet(string headerName)
	{
		for (int i = 0; i < Headers.Count; i++)
		{
			if (string.Equals(Headers.GetKey(i), headerName, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
