using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace System.Runtime.Serialization.DataContracts;

internal sealed class TimeOnlyDataContract : PrimitiveDataContract
{
	internal override string WriteMethodName => "WriteTimeOnly";

	internal override string ReadMethodName => "ReadElementContentAsTimeOnly";

	public TimeOnlyDataContract()
		: this(DictionaryGlobals.TimeOnlyLocalName, DictionaryGlobals.SerializationNamespace)
	{
	}

	internal TimeOnlyDataContract(XmlDictionaryString name, XmlDictionaryString ns)
		: base(typeof(TimeOnly), name, ns)
	{
	}

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
	internal override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
	{
		writer.WriteTimeOnly((TimeOnly)obj);
	}

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
	internal override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
	{
		if (context != null)
		{
			return PrimitiveDataContract.HandleReadValue(reader.ReadElementContentAsTimeOnly(), context);
		}
		return reader.ReadElementContentAsTimeOnly();
	}

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
	internal override void WriteXmlElement(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, XmlDictionaryString name, XmlDictionaryString ns)
	{
		xmlWriter.WriteTimeOnly((TimeOnly)obj, name, ns);
	}
}
