using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.CSharp.RuntimeBinder;

/// <summary>Represents an error that occurs when a dynamic bind in the C# runtime binder is processed.</summary>
[Serializable]
public class RuntimeBinderInternalCompilerException : Exception
{
	/// <summary>Initializes a new instance of the <see cref="T:Microsoft.CSharp.RuntimeBinder.RuntimeBinderInternalCompilerException" /> class with a system-supplied message that describes the error.</summary>
	public RuntimeBinderInternalCompilerException()
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:Microsoft.CSharp.RuntimeBinder.RuntimeBinderInternalCompilerException" /> class with a specified message that describes the error.</summary>
	/// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
	public RuntimeBinderInternalCompilerException(string? message)
		: base(message)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:Microsoft.CSharp.RuntimeBinder.RuntimeBinderInternalCompilerException" /> class that has a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
	public RuntimeBinderInternalCompilerException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:Microsoft.CSharp.RuntimeBinder.RuntimeBinderInternalCompilerException" /> class with serialized data.</summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected RuntimeBinderInternalCompilerException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
