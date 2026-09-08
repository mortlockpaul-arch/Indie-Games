using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Json;

public static class HttpClientJsonExtensions
{
	private static readonly Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> s_deleteAsync = (HttpClient client, Uri uri, CancellationToken cancellation) => client.DeleteAsync(uri, cancellation);

	private static readonly Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> s_getAsync = (HttpClient client, Uri uri, CancellationToken cancellation) => client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellation);

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static IAsyncEnumerable<TValue?> GetFromJsonAsAsyncEnumerable<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsAsyncEnumerable<TValue>(CreateUri(requestUri), options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static IAsyncEnumerable<TValue?> GetFromJsonAsAsyncEnumerable<TValue>(this HttpClient client, Uri? requestUri, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonStreamAsyncCore<TValue>(client, requestUri, options, cancellationToken);
	}

	public static IAsyncEnumerable<TValue?> GetFromJsonAsAsyncEnumerable<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsAsyncEnumerable(CreateUri(requestUri), jsonTypeInfo, cancellationToken);
	}

	public static IAsyncEnumerable<TValue?> GetFromJsonAsAsyncEnumerable<TValue>(this HttpClient client, Uri? requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonStreamAsyncCore(client, requestUri, jsonTypeInfo, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static IAsyncEnumerable<TValue?> GetFromJsonAsAsyncEnumerable<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsAsyncEnumerable<TValue>(requestUri, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static IAsyncEnumerable<TValue?> GetFromJsonAsAsyncEnumerable<TValue>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsAsyncEnumerable<TValue>(requestUri, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	private static IAsyncEnumerable<TValue> FromJsonStreamAsyncCore<TValue>(HttpClient client, Uri requestUri, JsonSerializerOptions options, CancellationToken cancellationToken)
	{
		JsonTypeInfo<TValue> jsonTypeInfo = (JsonTypeInfo<TValue>)JsonHelpers.GetJsonTypeInfo(typeof(TValue), options);
		return FromJsonStreamAsyncCore(client, requestUri, jsonTypeInfo, cancellationToken);
	}

	private static IAsyncEnumerable<TValue> FromJsonStreamAsyncCore<TValue>(HttpClient client, Uri requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		return Core(client, requestUri, jsonTypeInfo, cancellationToken);
		static async IAsyncEnumerable<TValue> Core(HttpClient httpClient, Uri requestUri2, JsonTypeInfo<TValue> jsonTypeInfo2, [EnumeratorCancellation] CancellationToken cancellationToken2)
		{
			using HttpResponseMessage response = await httpClient.GetAsync(requestUri2, HttpCompletionOption.ResponseHeadersRead, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
			response.EnsureSuccessStatusCode();
			using Stream readStream = await GetHttpResponseStreamAsync(httpClient, response, usingResponseHeadersRead: false, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
			await foreach (TValue item in JsonSerializer.DeserializeAsyncEnumerable(readStream, jsonTypeInfo2, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false))
			{
				yield return item;
			}
		}
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	private static Task<object> FromJsonAsyncCore(Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> getMethod, HttpClient client, Uri requestUri, Type type, JsonSerializerOptions options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(getMethod, client, requestUri, (Stream stream, (Type type, JsonSerializerOptions options) tuple, CancellationToken cancellation) => JsonSerializer.DeserializeAsync(stream, tuple.type, tuple.options ?? JsonSerializerOptions.Web, cancellation), (type, options), cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	private static Task<TValue> FromJsonAsyncCore<TValue>(Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> getMethod, HttpClient client, Uri requestUri, JsonSerializerOptions options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(getMethod, client, requestUri, (Stream stream, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellation) => JsonSerializer.DeserializeAsync<TValue>(stream, jsonSerializerOptions ?? JsonSerializerOptions.Web, cancellation), options, cancellationToken);
	}

	private static Task<object> FromJsonAsyncCore(Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> getMethod, HttpClient client, Uri requestUri, Type type, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(getMethod, client, requestUri, (Stream stream, (Type type, JsonSerializerContext context) options, CancellationToken cancellation) => JsonSerializer.DeserializeAsync(stream, options.type, options.context, cancellation), (type, context), cancellationToken);
	}

	private static Task<TValue> FromJsonAsyncCore<TValue>(Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> getMethod, HttpClient client, Uri requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken)
	{
		return FromJsonAsyncCore(getMethod, client, requestUri, (Stream stream, JsonTypeInfo<TValue> options, CancellationToken cancellation) => JsonSerializer.DeserializeAsync(stream, options, cancellation), jsonTypeInfo, cancellationToken);
	}

	private static Task<TValue> FromJsonAsyncCore<TValue, TJsonOptions>(Func<HttpClient, Uri, CancellationToken, Task<HttpResponseMessage>> getMethod, HttpClient client, Uri requestUri, Func<Stream, TJsonOptions, CancellationToken, ValueTask<TValue>> deserializeMethod, TJsonOptions jsonOptions, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		TimeSpan timeout = client.Timeout;
		CancellationTokenSource cancellationTokenSource = null;
		if (timeout != Timeout.InfiniteTimeSpan)
		{
			cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			cancellationTokenSource.CancelAfter(timeout);
		}
		Task<HttpResponseMessage> responseTask;
		try
		{
			responseTask = getMethod(client, requestUri, cancellationToken);
		}
		catch
		{
			cancellationTokenSource?.Dispose();
			throw;
		}
		bool usingResponseHeadersRead = (object)getMethod != s_deleteAsync;
		return Core(client, responseTask, usingResponseHeadersRead, cancellationTokenSource, deserializeMethod, jsonOptions, cancellationToken);
		static async Task<TValue> Core(HttpClient httpClient, Task<HttpResponseMessage> task, bool usingResponseHeadersRead2, CancellationTokenSource linkedCTS, Func<Stream, TJsonOptions, CancellationToken, ValueTask<TValue>> func, TJsonOptions arg, CancellationToken cancellationToken2)
		{
			_ = 2;
			try
			{
				using HttpResponseMessage response = await task.ConfigureAwait(continueOnCapturedContext: false);
				response.EnsureSuccessStatusCode();
				try
				{
					using Stream readStream = await GetHttpResponseStreamAsync(httpClient, response, usingResponseHeadersRead2, cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
					return await func(readStream, arg, linkedCTS?.Token ?? cancellationToken2).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException ex) when ((linkedCTS?.Token.IsCancellationRequested ?? false) && !cancellationToken2.IsCancellationRequested)
				{
					throw new TaskCanceledException(System.SR.Format(System.SR.net_http_request_timedout, httpClient.Timeout.TotalSeconds), new TimeoutException(ex.Message, ex), ex.CancellationToken);
				}
			}
			finally
			{
				linkedCTS?.Dispose();
			}
		}
	}

	private static Uri CreateUri(string uri)
	{
		if (!string.IsNullOrEmpty(uri))
		{
			return new Uri(uri, UriKind.RelativeOrAbsolute);
		}
		return null;
	}

	private static ValueTask<Stream> GetHttpResponseStreamAsync(HttpClient client, HttpResponseMessage response, bool usingResponseHeadersRead, CancellationToken cancellationToken)
	{
		int num = (int)client.MaxResponseContentBufferSize;
		long? contentLength = response.Content.Headers.ContentLength;
		if (contentLength.HasValue)
		{
			long valueOrDefault = contentLength.GetValueOrDefault();
			if (valueOrDefault > num)
			{
				LengthLimitReadStream.ThrowExceededBufferLimit(num);
			}
		}
		ValueTask<Stream> contentStreamAsync = HttpContentJsonExtensions.GetContentStreamAsync(response.Content, cancellationToken);
		if (!usingResponseHeadersRead)
		{
			return contentStreamAsync;
		}
		return GetLengthLimitReadStreamAsync(client, contentStreamAsync);
	}

	private static async ValueTask<Stream> GetLengthLimitReadStreamAsync(HttpClient client, ValueTask<Stream> task)
	{
		return new LengthLimitReadStream(await task.ConfigureAwait(continueOnCapturedContext: false), (int)client.MaxResponseContentBufferSize);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> DeleteFromJsonAsync(this HttpClient client, [StringSyntax("Uri")] string? requestUri, Type type, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync(CreateUri(requestUri), type, options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> DeleteFromJsonAsync(this HttpClient client, Uri? requestUri, Type type, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(s_deleteAsync, client, requestUri, type, options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> DeleteFromJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync<TValue>(CreateUri(requestUri), options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> DeleteFromJsonAsync<TValue>(this HttpClient client, Uri? requestUri, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore<TValue>(s_deleteAsync, client, requestUri, options, cancellationToken);
	}

	public static Task<object?> DeleteFromJsonAsync(this HttpClient client, [StringSyntax("Uri")] string? requestUri, Type type, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync(CreateUri(requestUri), type, context, cancellationToken);
	}

	public static Task<object?> DeleteFromJsonAsync(this HttpClient client, Uri? requestUri, Type type, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(s_deleteAsync, client, requestUri, type, context, cancellationToken);
	}

	public static Task<TValue?> DeleteFromJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync(CreateUri(requestUri), jsonTypeInfo, cancellationToken);
	}

	public static Task<TValue?> DeleteFromJsonAsync<TValue>(this HttpClient client, Uri? requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(s_deleteAsync, client, requestUri, jsonTypeInfo, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> DeleteFromJsonAsync(this HttpClient client, [StringSyntax("Uri")] string? requestUri, Type type, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync(requestUri, type, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> DeleteFromJsonAsync(this HttpClient client, Uri? requestUri, Type type, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync(requestUri, type, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> DeleteFromJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync<TValue>(requestUri, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> DeleteFromJsonAsync<TValue>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.DeleteFromJsonAsync<TValue>(requestUri, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> GetFromJsonAsync(this HttpClient client, [StringSyntax("Uri")] string? requestUri, Type type, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync(CreateUri(requestUri), type, options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> GetFromJsonAsync(this HttpClient client, Uri? requestUri, Type type, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore((HttpClient httpClient, Uri uri, CancellationToken cancellation) => httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellation), client, requestUri, type, options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> GetFromJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync<TValue>(CreateUri(requestUri), options, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> GetFromJsonAsync<TValue>(this HttpClient client, Uri? requestUri, JsonSerializerOptions? options, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore<TValue>(s_getAsync, client, requestUri, options, cancellationToken);
	}

	public static Task<object?> GetFromJsonAsync(this HttpClient client, [StringSyntax("Uri")] string? requestUri, Type type, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync(CreateUri(requestUri), type, context, cancellationToken);
	}

	public static Task<object?> GetFromJsonAsync(this HttpClient client, Uri? requestUri, Type type, JsonSerializerContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(s_getAsync, client, requestUri, type, context, cancellationToken);
	}

	public static Task<TValue?> GetFromJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync(CreateUri(requestUri), jsonTypeInfo, cancellationToken);
	}

	public static Task<TValue?> GetFromJsonAsync<TValue>(this HttpClient client, Uri? requestUri, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FromJsonAsyncCore(s_getAsync, client, requestUri, jsonTypeInfo, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> GetFromJsonAsync(this HttpClient client, [StringSyntax("Uri")] string? requestUri, Type type, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync(requestUri, type, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<object?> GetFromJsonAsync(this HttpClient client, Uri? requestUri, Type type, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync(requestUri, type, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> GetFromJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync<TValue>(requestUri, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<TValue?> GetFromJsonAsync<TValue>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		return client.GetFromJsonAsync<TValue>(requestUri, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, null, options);
		return client.PostAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, null, options);
		return client.PostAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, CancellationToken cancellationToken)
	{
		return client.PostAsJsonAsync(requestUri, value, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, CancellationToken cancellationToken)
	{
		return client.PostAsJsonAsync(requestUri, value, (JsonSerializerOptions?)null, cancellationToken);
	}

	public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, jsonTypeInfo);
		return client.PostAsync(requestUri, content, cancellationToken);
	}

	public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, jsonTypeInfo);
		return client.PostAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, null, options);
		return client.PutAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, null, options);
		return client.PutAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, CancellationToken cancellationToken)
	{
		return client.PutAsJsonAsync(requestUri, value, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, CancellationToken cancellationToken)
	{
		return client.PutAsJsonAsync(requestUri, value, (JsonSerializerOptions?)null, cancellationToken);
	}

	public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, jsonTypeInfo);
		return client.PutAsync(requestUri, content, cancellationToken);
	}

	public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, jsonTypeInfo);
		return client.PutAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, null, options);
		return client.PatchAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, null, options);
		return client.PatchAsync(requestUri, content, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, CancellationToken cancellationToken)
	{
		return client.PatchAsJsonAsync(requestUri, value, (JsonSerializerOptions?)null, cancellationToken);
	}

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext.")]
	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, CancellationToken cancellationToken)
	{
		return client.PatchAsJsonAsync(requestUri, value, (JsonSerializerOptions?)null, cancellationToken);
	}

	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, jsonTypeInfo);
		return client.PatchAsync(requestUri, content, cancellationToken);
	}

	public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(client, "client");
		JsonContent content = JsonContent.Create(value, jsonTypeInfo);
		return client.PatchAsync(requestUri, content, cancellationToken);
	}
}
