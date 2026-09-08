using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace System.Runtime.Serialization.DataContracts;

internal sealed class DateOnlyDataContract : PrimitiveDataContract
{
	internal override string WriteMethodName => "WriteDateOnly";

	internal override string ReadMethodName => "ReadElementContentAsDateOnly";

	public DateOnlyDataContract()
		: this(DictionaryGlobals.DateOnlyLocalName, DictionaryGlobals.SerializationNamespace)
	{
	}

	internal DateOnlyDataContract(XmlDictionaryString name, XmlDictionaryString ns)
		: base(typeof(DateOnly), name, ns)
	{
	}

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
	internal override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
	{
		writer.WriteDateOnly((DateOnly)obj);
	}

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
	internal override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
	{
		if (context != null)
		{
			return PrimitiveDataContract.HandleReadValue(reader.ReadElementContentAsDateOnly(), context);
		}
		return reader.ReadElementContentAsDateOnly();
	}

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[RequiresUnreferencedCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed. Make sure all of the required types are preserved.")]
	internal override void WriteXmlElement(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context, XmlDictionaryString name, XmlDictionaryString ns)
	{
		xmlWriter.WriteDateOnly((DateOnly)obj, name, ns);
	}
}
