using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace System.Xml;

public class XmlDocumentType : XmlLinkedNode
{
	private readonly string _name;

	private readonly string _publicId;

	private readonly string _systemId;

	private readonly string _internalSubset;

	private bool _namespaces;

	private SchemaInfo _schemaInfo;

	[CompilerGenerated]
	private XmlNamedNodeMap _003CEntities_003Ek__BackingField;

	[CompilerGenerated]
	private XmlNamedNodeMap _003CNotations_003Ek__BackingField;

	public override string Name => _name;

	public override string LocalName => _name;

	public override XmlNodeType NodeType => XmlNodeType.DocumentType;

	public override bool IsReadOnly => true;

	public XmlNamedNodeMap Entities => _003CEntities_003Ek__BackingField ?? (_003CEntities_003Ek__BackingField = new XmlNamedNodeMap(this));

	public XmlNamedNodeMap Notations => _003CNotations_003Ek__BackingField ?? (_003CNotations_003Ek__BackingField = new XmlNamedNodeMap(this));

	public string? PublicId => _publicId;

	public string? SystemId => _systemId;

	public string? InternalSubset => _internalSubset;

	internal bool ParseWithNamespaces => _namespaces;

	internal SchemaInfo? DtdSchemaInfo
	{
		get
		{
			return _schemaInfo;
		}
		set
		{
			_schemaInfo = value;
		}
	}

	protected internal XmlDocumentType(string name, string? publicId, string? systemId, string? internalSubset, XmlDocument doc)
		: base(doc)
	{
		_name = name;
		_publicId = publicId;
		_systemId = systemId;
		_namespaces = true;
		_internalSubset = internalSubset;
		if (!doc.IsLoading)
		{
			doc.IsLoading = true;
			new XmlLoader().ParseDocumentType(this);
			doc.IsLoading = false;
		}
	}

	public override XmlNode CloneNode(bool deep)
	{
		return OwnerDocument.CreateDocumentType(_name, _publicId, _systemId, _internalSubset);
	}

	public override void WriteTo(XmlWriter w)
	{
		w.WriteDocType(_name, _publicId, _systemId, _internalSubset);
	}

	public override void WriteContentTo(XmlWriter w)
	{
	}
}
