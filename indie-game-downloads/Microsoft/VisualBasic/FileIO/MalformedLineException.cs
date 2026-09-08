using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualBasic.FileIO;

[Serializable]
public class MalformedLineException : Exception
{
	private long m_LineNumber;

	[EditorBrowsable(EditorBrowsableState.Always)]
	public long LineNumber
	{
		get
		{
			return m_LineNumber;
		}
		set
		{
			m_LineNumber = value;
		}
	}

	public MalformedLineException()
	{
	}

	public MalformedLineException(string message, long lineNumber)
		: base(message)
	{
		m_LineNumber = lineNumber;
	}

	public MalformedLineException(string message)
		: base(message)
	{
	}

	public MalformedLineException(string message, long lineNumber, Exception innerException)
		: base(message, innerException)
	{
		m_LineNumber = lineNumber;
	}

	public MalformedLineException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected MalformedLineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			m_LineNumber = info.GetInt32("LineNumber");
		}
		else
		{
			m_LineNumber = -1L;
		}
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info?.AddValue("LineNumber", m_LineNumber, typeof(long));
		base.GetObjectData(info, context);
	}

	public override string ToString()
	{
		return base.ToString() + " " + System.SR.Format(System.SR.TextFieldParser_MalformedExtraData, LineNumber.ToString(CultureInfo.InvariantCulture));
	}
}
