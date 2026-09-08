using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.CSharp.RuntimeBinder;

[Serializable]
[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class DynamicBindingFailedException : Exception
{
	public DynamicBindingFailedException()
	{
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	private DynamicBindingFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
