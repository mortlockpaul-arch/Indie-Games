using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>Provides HTTP content based on a byte array.</summary>
public class ByteArrayContent : HttpContent
{
	private readonly byte[] _content;

	private readonly int _offset;

	private readonly int _count;

	internal override bool AllowDuplex => false;

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.ByteArrayContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> parameter is <see langword="null" />.</exception>
	public ByteArrayContent(byte[] content)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		_content = content;
		_count = content.Length;
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.ByteArrayContent" /> class.</summary>
	/// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
	/// <param name="offset">The offset, in bytes, in the <paramref name="content" /> parameter used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
	/// <param name="count">The number of bytes in the <paramref name="content" /> starting from the <paramref name="offset" /> parameter used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> parameter is <see langword="null" />.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> parameter is less than zero.  
	///  -or-  
	///  The <paramref name="offset" /> parameter is greater than the length of content specified by the <paramref name="content" /> parameter.  
	///  -or-  
	///  The <paramref name="count" /> parameter is less than zero.  
	///  -or-  
	///  The <paramref name="count" /> parameter is greater than the length of content specified by the <paramref name="content" /> parameter - minus the <paramref name="offset" /> parameter.</exception>
	public ByteArrayContent(byte[] content, int offset, int count)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, content.Length, "offset");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(count, content.Length - offset, "count");
		_content = content;
		_offset = offset;
		_count = count;
	}

	protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		stream.Write(_content, _offset, _count);
	}

	/// <summary>Serialize and write the byte array provided in the constructor to an HTTP content stream as an asynchronous operation.</summary>
	/// <param name="stream">The target stream.</param>
	/// <param name="context">Information about the transport, like channel binding token. This parameter may be <see langword="null" />.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
	{
		return SerializeToStreamAsyncCore(stream, default(CancellationToken));
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		if (!(GetType() == typeof(ByteArrayContent)))
		{
			return base.SerializeToStreamAsync(stream, context, cancellationToken);
		}
		return SerializeToStreamAsyncCore(stream, cancellationToken);
	}

	private protected Task SerializeToStreamAsyncCore(Stream stream, CancellationToken cancellationToken)
	{
		return stream.WriteAsync(_content, _offset, _count, cancellationToken);
	}

	/// <summary>Determines whether a byte array has a valid length in bytes.</summary>
	/// <param name="length">The length in bytes of the byte array.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="length" /> is a valid length; otherwise, <see langword="false" />.</returns>
	protected internal override bool TryComputeLength(out long length)
	{
		length = _count;
		return true;
	}

	protected override Stream CreateContentReadStream(CancellationToken cancellationToken)
	{
		return CreateMemoryStreamForByteArray();
	}

	/// <summary>Creates an HTTP content stream as an asynchronous operation for reading whose backing store is memory from the <see cref="T:System.Net.Http.ByteArrayContent" />.</summary>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected override Task<Stream> CreateContentReadStreamAsync()
	{
		return Task.FromResult((Stream)CreateMemoryStreamForByteArray());
	}

	internal override Stream TryCreateContentReadStream()
	{
		if (!(GetType() == typeof(ByteArrayContent)))
		{
			return null;
		}
		return CreateMemoryStreamForByteArray();
	}

	internal MemoryStream CreateMemoryStreamForByteArray()
	{
		return new MemoryStream(_content, _offset, _count, writable: false);
	}
}
