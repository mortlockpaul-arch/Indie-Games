using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mime;

internal abstract class MimeBasePart
{
	protected ContentType _contentType;

	protected ContentDisposition _contentDisposition;

	private HeaderCollection _headers;

	private static readonly char[] s_headerValueSplitChars = new char[3] { '\r', '\n', ' ' };

	internal string ContentID
	{
		get
		{
			return Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)];
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentID));
			}
			else
			{
				Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)] = value;
			}
		}
	}

	internal string ContentLocation
	{
		get
		{
			return Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)];
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentLocation));
			}
			else
			{
				Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)] = value;
			}
		}
	}

	internal NameValueCollection Headers
	{
		get
		{
			if (_headers == null)
			{
				_headers = new HeaderCollection();
			}
			if (_contentType == null)
			{
				_contentType = new ContentType();
			}
			_contentType.PersistIfNeeded(_headers, forcePersist: false);
			_contentDisposition?.PersistIfNeeded(_headers, forcePersist: false);
			return _headers;
		}
	}

	internal ContentType ContentType
	{
		get
		{
			return _contentType ?? (_contentType = new ContentType());
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			_contentType = value;
			_contentType.PersistIfNeeded((HeaderCollection)Headers, forcePersist: true);
		}
	}

	internal MimeBasePart()
	{
	}

	internal static bool ShouldUseBase64Encoding(Encoding encoding)
	{
		if (encoding != Encoding.Unicode && encoding != Encoding.UTF8 && encoding != Encoding.UTF32)
		{
			return encoding == Encoding.BigEndianUnicode;
		}
		return true;
	}

	internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding)
	{
		return EncodeHeaderValue(value, encoding, base64Encoding, 0);
	}

	internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding, int headerLength)
	{
		if (IsAscii(value, permitCROrLF: false))
		{
			return value;
		}
		if (encoding == null)
		{
			encoding = Encoding.GetEncoding("utf-8");
		}
		IEncodableStream encoderForHeader = EncodedStreamFactory.GetEncoderForHeader(encoding, base64Encoding, headerLength);
		encoderForHeader.EncodeString(value, encoding);
		return encoderForHeader.GetEncodedString();
	}

	internal static string DecodeHeaderValue(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		string text = string.Empty;
		string[] array = value.Split(s_headerValueSplitChars, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('?');
			if (array2.Length != 5 || array2[0] != "=" || array2[4] != "=")
			{
				return value;
			}
			string name = array2[1];
			bool useBase64Encoding = array2[2] == "B";
			byte[] bytes = Encoding.ASCII.GetBytes(array2[3]);
			int count = EncodedStreamFactory.GetEncoderForHeader(Encoding.GetEncoding(name), useBase64Encoding, 0).DecodeBytes(bytes);
			Encoding encoding = Encoding.GetEncoding(name);
			text += encoding.GetString(bytes, 0, count);
		}
		return text;
	}

	internal static Encoding DecodeEncoding(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		ReadOnlySpan<char> source = value.AsSpan();
		Span<Range> destination = stackalloc Range[6];
		if (source.SplitAny(destination, "?\r\n".AsSpan()) >= 5)
		{
			Range range = destination[0];
			if (source[range.Start..range.End].SequenceEqual("=".AsSpan()))
			{
				range = destination[4];
				if (source[range.Start..range.End].SequenceEqual("=".AsSpan()))
				{
					range = destination[1];
					return Encoding.GetEncoding(value[range.Start..range.End]);
				}
			}
		}
		return null;
	}

	internal static bool IsAscii(string value, bool permitCROrLF)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		if (Ascii.IsValid(value.AsSpan()))
		{
			if (!permitCROrLF)
			{
				return !value.AsSpan().ContainsAny('\r', '\n');
			}
			return true;
		}
		return false;
	}

	internal void PrepareHeaders(bool allowUnicode)
	{
		_contentType.PersistIfNeeded((HeaderCollection)Headers, forcePersist: false);
		_headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentType), _contentType.Encode(allowUnicode));
		if (_contentDisposition != null)
		{
			_contentDisposition.PersistIfNeeded((HeaderCollection)Headers, forcePersist: false);
			_headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), _contentDisposition.Encode(allowUnicode));
		}
	}

	internal abstract Task SendAsync<TIOAdapter>(BaseWriter writer, bool allowUnicode, CancellationToken cancellationToken) where TIOAdapter : System.Net.IReadWriteAdapter;
}
