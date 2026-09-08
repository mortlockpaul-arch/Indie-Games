using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>Provides HTTP content based on a string.</summary>
public class StringContent : ByteArrayContent
{
	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.StringContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.StringContent" />.</param>
	public StringContent(string content)
		: this(content, HttpContent.DefaultStringEncoding, "text/plain")
	{
	}

	public StringContent(string content, MediaTypeHeaderValue? mediaType)
		: this(content, HttpContent.DefaultStringEncoding, mediaType)
	{
	}

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.StringContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.StringContent" />.</param>
	/// <param name="encoding">The encoding to use for the content.</param>
	public StringContent(string content, Encoding? encoding)
		: this(content, encoding, "text/plain")
	{
	}

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.StringContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.StringContent" />.</param>
	/// <param name="encoding">The encoding to use for the content.</param>
	/// <param name="mediaType">The media type to use for the content.</param>
	public StringContent(string content, Encoding? encoding, string? mediaType)
		: base(GetContentByteArray(content, encoding))
	{
		if (encoding == null)
		{
			encoding = HttpContent.DefaultStringEncoding;
		}
		if (mediaType == null)
		{
			mediaType = "text/plain";
		}
		if (encoding == HttpContent.DefaultStringEncoding)
		{
			string text = ((mediaType == "text/plain") ? "text/plain; charset=utf-8" : ((!(mediaType == "application/json")) ? null : "application/json; charset=utf-8"));
			if (text != null)
			{
				base.Headers.TryAddWithoutValidation(KnownHeaders.ContentType.Descriptor, text);
				return;
			}
		}
		base.Headers.ContentType = new MediaTypeHeaderValue(mediaType, encoding.WebName);
	}

	public StringContent(string content, Encoding? encoding, MediaTypeHeaderValue? mediaType)
		: base(GetContentByteArray(content, encoding))
	{
		base.Headers.ContentType = mediaType;
	}

	private static byte[] GetContentByteArray(string content, Encoding encoding)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		return (encoding ?? HttpContent.DefaultStringEncoding).GetBytes(content);
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		if (!(GetType() == typeof(StringContent)))
		{
			return base.SerializeToStreamAsync(stream, context, cancellationToken);
		}
		return SerializeToStreamAsyncCore(stream, cancellationToken);
	}

	internal override Stream TryCreateContentReadStream()
	{
		if (!(GetType() == typeof(StringContent)))
		{
			return null;
		}
		return CreateMemoryStreamForByteArray();
	}
}
