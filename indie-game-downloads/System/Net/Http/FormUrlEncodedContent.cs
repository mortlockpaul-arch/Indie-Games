using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>A container for name/value tuples encoded using application/x-www-form-urlencoded MIME type.</summary>
public class FormUrlEncodedContent : ByteArrayContent
{
	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.FormUrlEncodedContent" /> class with a specific collection of name/value pairs.</summary>
	/// <param name="nameValueCollection">A collection of name/value pairs.</param>
	public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
		: base(GetContentByteArray(nameValueCollection))
	{
		base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
	}

	private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
	{
		ArgumentNullException.ThrowIfNull(nameValueCollection, "nameValueCollection");
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder builder = new System.Text.ValueStringBuilder(initialBuffer);
		foreach (KeyValuePair<string, string> item in nameValueCollection)
		{
			if (builder.Length > 0)
			{
				builder.Append('&');
			}
			Encode(ref builder, item.Key);
			builder.Append('=');
			Encode(ref builder, item.Value);
		}
		byte[] array = new byte[builder.Length];
		HttpRuleParser.DefaultHttpEncoding.GetBytes(builder.AsSpan(), array);
		builder.Dispose();
		return array;
	}

	private static void Encode(ref System.Text.ValueStringBuilder builder, string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return;
		}
		int charsWritten;
		while (!Uri.TryEscapeDataString(data.AsSpan(), builder.RawChars.Slice(builder.Length), out charsWritten))
		{
			builder.EnsureCapacity(builder.Capacity + 1);
		}
		if (data.Contains(' '))
		{
			ReadOnlySpan<char> readOnlySpan = builder.RawChars.Slice(builder.Length, charsWritten);
			while (true)
			{
				int num = readOnlySpan.IndexOf("%20".AsSpan(), StringComparison.Ordinal);
				if (num < 0)
				{
					break;
				}
				builder.Append(readOnlySpan.Slice(0, num));
				builder.Append('+');
				readOnlySpan = readOnlySpan.Slice(num + 3);
			}
			builder.Append(readOnlySpan);
		}
		else
		{
			builder.Length += charsWritten;
		}
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		if (!(GetType() == typeof(FormUrlEncodedContent)))
		{
			return base.SerializeToStreamAsync(stream, context, cancellationToken);
		}
		return SerializeToStreamAsyncCore(stream, cancellationToken);
	}

	internal override Stream TryCreateContentReadStream()
	{
		if (!(GetType() == typeof(FormUrlEncodedContent)))
		{
			return null;
		}
		return CreateMemoryStreamForByteArray();
	}
}
