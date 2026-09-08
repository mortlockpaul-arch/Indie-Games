using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace System.Runtime.Serialization.Formatters.Binary;

[Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.", DiagnosticId = "SYSLIB0011", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public sealed class BinaryFormatter : IFormatter
{
	internal ISurrogateSelector _surrogates;

	internal StreamingContext _context;

	internal SerializationBinder _binder;

	internal FormatterTypeStyle _typeFormat = FormatterTypeStyle.TypesAlways;

	internal FormatterAssemblyStyle _assemblyFormat;

	internal TypeFilterLevel _securityLevel = TypeFilterLevel.Full;

	public FormatterTypeStyle TypeFormat
	{
		get
		{
			return _typeFormat;
		}
		set
		{
			_typeFormat = value;
		}
	}

	public FormatterAssemblyStyle AssemblyFormat
	{
		get
		{
			return _assemblyFormat;
		}
		set
		{
			_assemblyFormat = value;
		}
	}

	public TypeFilterLevel FilterLevel
	{
		get
		{
			return _securityLevel;
		}
		set
		{
			_securityLevel = value;
		}
	}

	public ISurrogateSelector? SurrogateSelector
	{
		get
		{
			return _surrogates;
		}
		set
		{
			_surrogates = value;
		}
	}

	public SerializationBinder? Binder
	{
		get
		{
			return _binder;
		}
		set
		{
			_binder = value;
		}
	}

	public StreamingContext Context
	{
		get
		{
			return _context;
		}
		set
		{
			_context = value;
		}
	}

	public BinaryFormatter()
		: this(null, new StreamingContext(StreamingContextStates.All))
	{
	}

	public BinaryFormatter(ISurrogateSelector? selector, StreamingContext context)
	{
		_surrogates = selector;
		_context = context;
	}

	[RequiresDynamicCode("BinaryFormatter serialization uses dynamic code generation, the type of objects being processed cannot be statically discovered.")]
	[RequiresUnreferencedCode("BinaryFormatter serialization is not trim compatible because the type of objects being processed cannot be statically discovered.")]
	public object Deserialize(Stream serializationStream)
	{
		throw new PlatformNotSupportedException(System.SR.BinaryFormatter_Removed);
	}

	[RequiresUnreferencedCode("BinaryFormatter serialization is not trim compatible because the type of objects being processed cannot be statically discovered.")]
	public void Serialize(Stream serializationStream, object graph)
	{
		throw new PlatformNotSupportedException(System.SR.BinaryFormatter_Removed);
	}
}
