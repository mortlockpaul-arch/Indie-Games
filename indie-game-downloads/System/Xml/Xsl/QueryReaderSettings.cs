using System.IO;

namespace System.Xml.Xsl;

internal sealed class QueryReaderSettings
{
	private readonly bool _validatingReader;

	private readonly XmlReaderSettings _xmlReaderSettings;

	private readonly XmlNameTable _xmlNameTable;

	private readonly EntityHandling _entityHandling;

	private readonly bool _namespaces;

	private readonly bool _normalization;

	private readonly bool _prohibitDtd;

	private readonly WhitespaceHandling _whitespaceHandling;

	private readonly XmlResolver _xmlResolver;

	public XmlNameTable NameTable
	{
		get
		{
			if (_xmlReaderSettings == null)
			{
				return _xmlNameTable;
			}
			return _xmlReaderSettings.NameTable;
		}
	}

	public QueryReaderSettings(XmlNameTable xmlNameTable)
	{
		_xmlReaderSettings = new XmlReaderSettings();
		_xmlReaderSettings.NameTable = xmlNameTable;
		_xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
		_xmlReaderSettings.XmlResolver = null;
		_xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
		_xmlReaderSettings.CloseInput = true;
	}

	public QueryReaderSettings(XmlReader reader)
	{
		if (reader is XmlValidatingReader xmlValidatingReader)
		{
			_validatingReader = true;
			reader = xmlValidatingReader.Impl.Reader;
		}
		_xmlReaderSettings = reader.Settings;
		if (_xmlReaderSettings != null)
		{
			_xmlReaderSettings = _xmlReaderSettings.Clone();
			_xmlReaderSettings.NameTable = reader.NameTable;
			_xmlReaderSettings.CloseInput = true;
			_xmlReaderSettings.LineNumberOffset = 0;
			_xmlReaderSettings.LinePositionOffset = 0;
			if (reader is XmlTextReaderImpl xmlTextReaderImpl)
			{
				_xmlReaderSettings.XmlResolver = xmlTextReaderImpl.GetResolver();
			}
			return;
		}
		_xmlNameTable = reader.NameTable;
		if (reader is XmlTextReader xmlTextReader)
		{
			XmlTextReaderImpl impl = xmlTextReader.Impl;
			_entityHandling = impl.EntityHandling;
			_namespaces = impl.Namespaces;
			_normalization = impl.Normalization;
			_prohibitDtd = impl.DtdProcessing == DtdProcessing.Prohibit;
			_whitespaceHandling = impl.WhitespaceHandling;
			_xmlResolver = impl.GetResolver();
		}
		else
		{
			_entityHandling = EntityHandling.ExpandEntities;
			_namespaces = true;
			_normalization = true;
			_prohibitDtd = true;
			_whitespaceHandling = WhitespaceHandling.All;
			_xmlResolver = null;
		}
	}

	public XmlReader CreateReader(Stream stream, string baseUri)
	{
		XmlReader xmlReader = ((_xmlReaderSettings == null) ? new XmlTextReaderImpl(baseUri, stream, _xmlNameTable)
		{
			EntityHandling = _entityHandling,
			Namespaces = _namespaces,
			Normalization = _normalization,
			DtdProcessing = ((!_prohibitDtd) ? DtdProcessing.Parse : DtdProcessing.Prohibit),
			WhitespaceHandling = _whitespaceHandling,
			XmlResolver = _xmlResolver
		} : XmlReader.Create(stream, _xmlReaderSettings, baseUri));
		if (_validatingReader)
		{
			xmlReader = new XmlValidatingReader(xmlReader);
		}
		return xmlReader;
	}
}
