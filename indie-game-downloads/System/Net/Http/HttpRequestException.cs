namespace System.Net.Http;

/// <summary>A base class for exceptions thrown by the <see cref="T:System.Net.Http.HttpClient" /> and <see cref="T:System.Net.Http.HttpMessageHandler" /> classes.</summary>
public class HttpRequestException : Exception
{
	internal RequestRetryType AllowRetry { get; }

	public HttpRequestError HttpRequestError { get; }

	public HttpStatusCode? StatusCode { get; }

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestException" /> class.</summary>
	public HttpRequestException()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestException" /> class with a specific message that describes the current exception.</summary>
	/// <param name="message">A message that describes the current exception.</param>
	public HttpRequestException(string? message)
		: base(message)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestException" /> class with a specific message that describes the current exception and an inner exception.</summary>
	/// <param name="message">A message that describes the current exception.</param>
	/// <param name="inner">The inner exception.</param>
	public HttpRequestException(string? message, Exception? inner)
		: base(message, inner)
	{
		if (inner != null)
		{
			base.HResult = inner.HResult;
		}
	}

	public HttpRequestException(string? message, Exception? inner, HttpStatusCode? statusCode)
		: this(message, inner)
	{
		StatusCode = statusCode;
	}

	public HttpRequestException(HttpRequestError httpRequestError, string? message = null, Exception? inner = null, HttpStatusCode? statusCode = null)
		: this(message, inner, statusCode)
	{
		HttpRequestError = httpRequestError;
	}

	internal HttpRequestException(HttpRequestError httpRequestError, string message, Exception inner, RequestRetryType allowRetry)
		: this(httpRequestError, message, inner)
	{
		AllowRetry = allowRetry;
	}
}
