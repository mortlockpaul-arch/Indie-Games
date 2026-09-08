using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mime;

internal sealed class MimePart : MimeBasePart, IDisposable
{
	private Stream _stream;

	private bool _streamSet;

	private bool _streamUsedOnce;

	internal Stream Stream => _stream;

	internal ContentDisposition ContentDisposition
	{
		get
		{
			return _contentDisposition;
		}
		set
		{
			_contentDisposition = value;
			if (value == null)
			{
				((HeaderCollection)base.Headers).InternalRemove(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition));
			}
			else
			{
				_contentDisposition.PersistIfNeeded((HeaderCollection)base.Headers, forcePersist: true);
			}
		}
	}

	internal TransferEncoding TransferEncoding
	{
		get
		{
			string text = base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)];
			if (text.Equals("base64", StringComparison.OrdinalIgnoreCase))
			{
				return TransferEncoding.Base64;
			}
			if (text.Equals("quoted-printable", StringComparison.OrdinalIgnoreCase))
			{
				return TransferEncoding.QuotedPrintable;
			}
			if (text.Equals("7bit", StringComparison.OrdinalIgnoreCase))
			{
				return TransferEncoding.SevenBit;
			}
			if (text.Equals("8bit", StringComparison.OrdinalIgnoreCase))
			{
				return TransferEncoding.EightBit;
			}
			return TransferEncoding.Unknown;
		}
		set
		{
			switch (value)
			{
			case TransferEncoding.Base64:
				base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "base64";
				break;
			case TransferEncoding.QuotedPrintable:
				base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "quoted-printable";
				break;
			case TransferEncoding.SevenBit:
				base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "7bit";
				break;
			case TransferEncoding.EightBit:
				base.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentTransferEncoding)] = "8bit";
				break;
			default:
				throw new NotSupportedException(System.SR.Format(System.SR.MimeTransferEncodingNotSupported, value));
			}
		}
	}

	internal MimePart()
	{
	}

	public void Dispose()
	{
		_stream?.Close();
	}

	internal void SetContent(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		if (_streamSet)
		{
			_stream.Close();
		}
		_stream = stream;
		_streamSet = true;
		_streamUsedOnce = false;
		TransferEncoding = TransferEncoding.Base64;
	}

	internal void SetContent(Stream stream, string name, string mimeType)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		if (mimeType != null && mimeType != string.Empty)
		{
			_contentType = new ContentType(mimeType);
		}
		if (name != null && name != string.Empty)
		{
			base.ContentType.Name = name;
		}
		SetContent(stream);
	}

	internal void SetContent(Stream stream, ContentType contentType)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		_contentType = contentType;
		SetContent(stream);
	}

	internal Stream GetEncodedStream(Stream stream)
	{
		Stream stream2 = stream;
		if (TransferEncoding == TransferEncoding.Base64)
		{
			stream2 = new Base64Stream(stream2, new Base64WriteStateInfo());
		}
		else if (TransferEncoding == TransferEncoding.QuotedPrintable)
		{
			stream2 = new QuotedPrintableStream(stream2, encodeCRLF: true);
		}
		else if (TransferEncoding == TransferEncoding.SevenBit || TransferEncoding == TransferEncoding.EightBit)
		{
			stream2 = new EightBitStream(stream2);
		}
		return stream2;
	}

	internal override async Task SendAsync<TIOAdapter>(BaseWriter writer, bool allowUnicode, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (Stream != null)
		{
			byte[] buffer = new byte[17408];
			PrepareHeaders(allowUnicode);
			writer.WriteHeaders(base.Headers, allowUnicode);
			Stream outputStream = writer.GetContentStream();
			outputStream = GetEncodedStream(outputStream);
			ResetStream();
			_streamUsedOnce = true;
			int length;
			while ((length = await TIOAdapter.ReadAsync(Stream, buffer.AsMemory(0, 17408), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) > 0)
			{
				await TIOAdapter.WriteAsync(outputStream, buffer.AsMemory(0, length), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			outputStream.Close();
		}
	}

	internal void ResetStream()
	{
		if (_streamUsedOnce)
		{
			if (!Stream.CanSeek)
			{
				throw new InvalidOperationException(System.SR.MimePartCantResetStream);
			}
			Stream.Seek(0L, SeekOrigin.Begin);
			_streamUsedOnce = false;
		}
	}
}
