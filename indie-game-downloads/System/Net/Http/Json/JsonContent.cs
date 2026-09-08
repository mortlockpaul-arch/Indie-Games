using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Json;

public sealed class JsonContent : HttpContent
{
	private readonly JsonTypeInfo _typeInfo;

	public Type ObjectType => _typeInfo.Type;

	public object? Value { get; }

	private JsonContent(object inputValue, JsonTypeInfo jsonTypeInfo, MediaTypeHeaderValue mediaType)
	{
		Value = inputValue;
		_typeInfo = jsonTypeInfo;
		if (mediaType != null)
		{
			base.Headers.ContentType = mediaType;
		}
		else
		{
			base.Headers.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static JsonContent Create<T>(T inputValue, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
	{
		return Create(inputValue, JsonHelpers.GetJsonTypeInfo(typeof(T), options), mediaType);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static JsonContent Create(object? inputValue, Type inputType, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(inputType, "inputType");
		EnsureTypeCompatibility(inputValue, inputType);
		return new JsonContent(inputValue, JsonHelpers.GetJsonTypeInfo(inputType, options), mediaType);
	}

	public static JsonContent Create<T>(T? inputValue, JsonTypeInfo<T> jsonTypeInfo, MediaTypeHeaderValue? mediaType = null)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		return new JsonContent(inputValue, jsonTypeInfo, mediaType);
	}

	public static JsonContent Create(object? inputValue, JsonTypeInfo jsonTypeInfo, MediaTypeHeaderValue? mediaType = null)
	{
		ArgumentNullException.ThrowIfNull(jsonTypeInfo, "jsonTypeInfo");
		EnsureTypeCompatibility(inputValue, jsonTypeInfo.Type);
		return new JsonContent(inputValue, jsonTypeInfo, mediaType);
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
	{
		return SerializeToStreamAsyncCore(stream, CancellationToken.None);
	}

	protected override bool TryComputeLength(out long length)
	{
		length = 0L;
		return false;
	}

	private Task SerializeToStreamAsyncCore(Stream targetStream, CancellationToken cancellationToken)
	{
		Encoding encoding = JsonHelpers.GetEncoding(this);
		if (encoding == null || encoding == Encoding.UTF8)
		{
			return JsonSerializer.SerializeAsync(targetStream, Value, _typeInfo, cancellationToken);
		}
		return SerializeToStreamAsyncTranscoding(targetStream, async: true, encoding, cancellationToken);
	}

	private async Task SerializeToStreamAsyncTranscoding(Stream targetStream, bool async, Encoding targetEncoding, CancellationToken cancellationToken)
	{
		Stream transcodingStream = Encoding.CreateTranscodingStream(targetStream, targetEncoding, Encoding.UTF8, leaveOpen: true);
		try
		{
			if (async)
			{
				await JsonSerializer.SerializeAsync(transcodingStream, Value, _typeInfo, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				JsonSerializer.Serialize(transcodingStream, Value, _typeInfo);
			}
		}
		finally
		{
			if (async)
			{
				await transcodingStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				transcodingStream.Dispose();
			}
		}
	}

	private static void EnsureTypeCompatibility(object inputValue, Type inputType)
	{
		if (inputValue != null && !inputType.IsAssignableFrom(inputValue.GetType()))
		{
			throw new ArgumentException(System.SR.Format(System.SR.SerializeWrongType, inputType, inputValue.GetType()));
		}
	}

	protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		Encoding encoding = JsonHelpers.GetEncoding(this);
		if (encoding != null && encoding != Encoding.UTF8)
		{
			SerializeToStreamAsyncTranscoding(stream, async: false, encoding, cancellationToken).GetAwaiter().GetResult();
		}
		else
		{
			JsonSerializer.Serialize(stream, Value, _typeInfo);
		}
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		return SerializeToStreamAsyncCore(stream, cancellationToken);
	}
}
