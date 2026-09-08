using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>Provides a container for content encoded using multipart/form-data MIME type.</summary>
public class MultipartFormDataContent : MultipartContent
{
	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartFormDataContent" /> class.</summary>
	public MultipartFormDataContent()
		: base("form-data")
	{
	}

	/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartFormDataContent" /> class.</summary>
	/// <param name="boundary">The boundary string for the multipart form data content.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="boundary" /> was <see langword="null" /> or contains only white space characters.  
	///  -or-  
	///  The <paramref name="boundary" /> ends with a space character.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The length of the <paramref name="boundary" /> was greater than 70.</exception>
	public MultipartFormDataContent(string boundary)
		: base("form-data", boundary)
	{
	}

	/// <summary>Add HTTP content to a collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized to multipart/form-data MIME type.</summary>
	/// <param name="content">The HTTP content to add to the collection.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was <see langword="null" />.</exception>
	public override void Add(HttpContent content)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		HttpContentHeaders headers = content.Headers;
		if (headers.ContentDisposition == null)
		{
			ContentDispositionHeaderValue contentDispositionHeaderValue = (headers.ContentDisposition = new ContentDispositionHeaderValue("form-data"));
		}
		base.Add(content);
	}

	/// <summary>Add HTTP content to a collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized to multipart/form-data MIME type.</summary>
	/// <param name="content">The HTTP content to add to the collection.</param>
	/// <param name="name">The name for the HTTP content to add.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="name" /> was <see langword="null" /> or contains only white space characters.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was <see langword="null" />.</exception>
	public void Add(HttpContent content, string name)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		ArgumentException.ThrowIfNullOrWhiteSpace(name, "name");
		AddInternal(content, name, null);
	}

	/// <summary>Add HTTP content to a collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized to multipart/form-data MIME type.</summary>
	/// <param name="content">The HTTP content to add to the collection.</param>
	/// <param name="name">The name for the HTTP content to add.</param>
	/// <param name="fileName">The file name for the HTTP content to add to the collection.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="name" /> was <see langword="null" /> or contains only white space characters.  
	///  -or-  
	///  The <paramref name="fileName" /> was <see langword="null" /> or contains only white space characters.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was <see langword="null" />.</exception>
	public void Add(HttpContent content, string name, string fileName)
	{
		ArgumentNullException.ThrowIfNull(content, "content");
		ArgumentException.ThrowIfNullOrWhiteSpace(name, "name");
		ArgumentException.ThrowIfNullOrWhiteSpace(fileName, "fileName");
		AddInternal(content, name, fileName);
	}

	private void AddInternal(HttpContent content, string name, string fileName)
	{
		if (content.Headers.ContentDisposition == null)
		{
			ContentDispositionHeaderValue contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
			contentDispositionHeaderValue.Name = name;
			contentDispositionHeaderValue.FileName = fileName;
			contentDispositionHeaderValue.FileNameStar = fileName;
			content.Headers.ContentDisposition = contentDispositionHeaderValue;
		}
		base.Add(content);
	}

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
	{
		if (!(GetType() == typeof(MultipartFormDataContent)))
		{
			return base.SerializeToStreamAsync(stream, context, cancellationToken);
		}
		return SerializeToStreamAsyncCore(stream, context, cancellationToken);
	}
}
