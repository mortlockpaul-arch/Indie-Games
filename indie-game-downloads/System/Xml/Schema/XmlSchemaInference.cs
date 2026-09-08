using System.Collections;

namespace System.Xml.Schema;

public sealed class XmlSchemaInference
{
	public enum InferenceOption
	{
		Restricted,
		Relaxed
	}

	internal static readonly XmlQualifiedName ST_boolean = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_byte = new XmlQualifiedName("byte", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_unsignedByte = new XmlQualifiedName("unsignedByte", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_short = new XmlQualifiedName("short", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_unsignedShort = new XmlQualifiedName("unsignedShort", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_int = new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_unsignedInt = new XmlQualifiedName("unsignedInt", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_long = new XmlQualifiedName("long", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_unsignedLong = new XmlQualifiedName("unsignedLong", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_integer = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_decimal = new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_float = new XmlQualifiedName("float", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_double = new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_duration = new XmlQualifiedName("duration", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_dateTime = new XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_time = new XmlQualifiedName("time", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_date = new XmlQualifiedName("date", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_gYearMonth = new XmlQualifiedName("gYearMonth", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_string = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

	internal static readonly XmlQualifiedName ST_anySimpleType = new XmlQualifiedName("anySimpleType", "http://www.w3.org/2001/XMLSchema");

	internal static XmlQualifiedName[] SimpleTypes = new XmlQualifiedName[19]
	{
		ST_boolean, ST_byte, ST_unsignedByte, ST_short, ST_unsignedShort, ST_int, ST_unsignedInt, ST_long, ST_unsignedLong, ST_integer,
		ST_decimal, ST_float, ST_double, ST_duration, ST_dateTime, ST_time, ST_date, ST_gYearMonth, ST_string
	};

	private XmlSchema _rootSchema;

	private XmlSchemaSet _schemaSet;

	private XmlReader _xtr;

	private readonly NameTable _nametable;

	private string _targetNamespace;

	private readonly XmlNamespaceManager _namespaceManager;

	private InferenceOption _occurrence;

	private InferenceOption _typeInference;

	public InferenceOption Occurrence
	{
		get
		{
			return _occurrence;
		}
		set
		{
			_occurrence = value;
		}
	}

	public InferenceOption TypeInference
	{
		get
		{
			return _typeInference;
		}
		set
		{
			_typeInference = value;
		}
	}

	public XmlSchemaInference()
	{
		_nametable = new NameTable();
		_namespaceManager = new XmlNamespaceManager(_nametable);
		_namespaceManager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
	}

	public XmlSchemaSet InferSchema(XmlReader instanceDocument)
	{
		return InferSchema1(instanceDocument, new XmlSchemaSet(_nametable));
	}

	public XmlSchemaSet InferSchema(XmlReader instanceDocument, XmlSchemaSet schemas)
	{
		if (schemas == null)
		{
			schemas = new XmlSchemaSet(_nametable);
		}
		return InferSchema1(instanceDocument, schemas);
	}

	internal XmlSchemaSet InferSchema1(XmlReader instanceDocument, XmlSchemaSet schemas)
	{
		ArgumentNullException.ThrowIfNull(instanceDocument, "instanceDocument");
		_rootSchema = null;
		_xtr = instanceDocument;
		schemas.Compile();
		_schemaSet = schemas;
		while (_xtr.NodeType != XmlNodeType.Element && _xtr.Read())
		{
		}
		if (_xtr.NodeType == XmlNodeType.Element)
		{
			_targetNamespace = _xtr.NamespaceURI;
			if (_xtr.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				throw new XmlSchemaInferenceException(System.SR.SchInf_schema, 0, 0);
			}
			XmlSchemaElement xse = null;
			foreach (XmlSchemaElement value in schemas.GlobalElements.Values)
			{
				if (value.Name == _xtr.LocalName && value.QualifiedName.Namespace == _xtr.NamespaceURI)
				{
					_rootSchema = value.Parent as XmlSchema;
					xse = value;
					break;
				}
			}
			if (_rootSchema == null)
			{
				AddElement(_xtr.LocalName, _xtr.Prefix, _xtr.NamespaceURI, null, null, -1);
			}
			else
			{
				InferElement(xse, bCreatingNewType: false, _rootSchema);
			}
			foreach (string item in _namespaceManager)
			{
				if (!item.Equals("xml") && !item.Equals("xmlns"))
				{
					string text2 = _namespaceManager.LookupNamespace(_nametable.Get(item));
					if (text2.Length != 0)
					{
						_rootSchema.Namespaces.Add(item, text2);
					}
				}
			}
			schemas.Reprocess(_rootSchema);
			schemas.Compile();
			return schemas;
		}
		throw new XmlSchemaInferenceException(System.SR.SchInf_NoElement, 0, 0);
	}

	private XmlSchemaAttribute AddAttribute(string localName, string prefix, string childURI, string attrValue, bool bCreatingNewType, XmlSchema parentSchema, XmlSchemaObjectCollection addLocation, XmlSchemaObjectTable compiledAttributes)
	{
		if (childURI == "http://www.w3.org/2001/XMLSchema")
		{
			throw new XmlSchemaInferenceException(System.SR.SchInf_schema, 0, 0);
		}
		int iTypeFlags = -1;
		XmlSchema xmlSchema = null;
		bool flag = true;
		ICollection attributes;
		ICollection collection;
		if (compiledAttributes.Count > 0)
		{
			attributes = compiledAttributes.Values;
			collection = addLocation;
		}
		else
		{
			attributes = addLocation;
			collection = null;
		}
		XmlSchemaAttribute result;
		if (childURI == "http://www.w3.org/XML/1998/namespace")
		{
			XmlSchemaAttribute xmlSchemaAttribute = FindAttributeRef(attributes, localName, childURI);
			if (xmlSchemaAttribute == null && collection != null)
			{
				xmlSchemaAttribute = FindAttributeRef(collection, localName, childURI);
			}
			if (xmlSchemaAttribute == null)
			{
				xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.RefName = new XmlQualifiedName(localName, childURI);
				if (bCreatingNewType && Occurrence == InferenceOption.Restricted)
				{
					xmlSchemaAttribute.Use = XmlSchemaUse.Required;
				}
				else
				{
					xmlSchemaAttribute.Use = XmlSchemaUse.Optional;
				}
				addLocation.Add(xmlSchemaAttribute);
			}
			result = xmlSchemaAttribute;
		}
		else
		{
			if (childURI.Length == 0)
			{
				xmlSchema = parentSchema;
				flag = false;
			}
			else if (!_schemaSet.Contains(childURI))
			{
				xmlSchema = new XmlSchema();
				xmlSchema.AttributeFormDefault = XmlSchemaForm.Unqualified;
				xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
				if (childURI.Length != 0)
				{
					xmlSchema.TargetNamespace = childURI;
				}
				_schemaSet.Add(xmlSchema);
				if (prefix.Length != 0 && !string.Equals(prefix, "xml", StringComparison.OrdinalIgnoreCase))
				{
					_namespaceManager.AddNamespace(prefix, childURI);
				}
			}
			else if (_schemaSet.Schemas(childURI) is ArrayList { Count: >0 } arrayList)
			{
				xmlSchema = arrayList[0] as XmlSchema;
			}
			if (childURI.Length != 0)
			{
				XmlSchemaAttribute xmlSchemaAttribute2 = FindAttributeRef(attributes, localName, childURI);
				if ((xmlSchemaAttribute2 == null) & (collection != null))
				{
					xmlSchemaAttribute2 = FindAttributeRef(collection, localName, childURI);
				}
				if (xmlSchemaAttribute2 == null)
				{
					xmlSchemaAttribute2 = new XmlSchemaAttribute();
					xmlSchemaAttribute2.RefName = new XmlQualifiedName(localName, childURI);
					if (bCreatingNewType && Occurrence == InferenceOption.Restricted)
					{
						xmlSchemaAttribute2.Use = XmlSchemaUse.Required;
					}
					else
					{
						xmlSchemaAttribute2.Use = XmlSchemaUse.Optional;
					}
					addLocation.Add(xmlSchemaAttribute2);
				}
				result = xmlSchemaAttribute2;
				XmlSchemaAttribute xmlSchemaAttribute3 = FindAttribute(xmlSchema.Items, localName);
				if (xmlSchemaAttribute3 == null)
				{
					xmlSchemaAttribute3 = new XmlSchemaAttribute();
					xmlSchemaAttribute3.Name = localName;
					xmlSchemaAttribute3.SchemaTypeName = RefineSimpleType(attrValue, ref iTypeFlags);
					xmlSchemaAttribute3.LineNumber = iTypeFlags;
					xmlSchema.Items.Add(xmlSchemaAttribute3);
				}
				else
				{
					if (xmlSchemaAttribute3.Parent == null)
					{
						iTypeFlags = xmlSchemaAttribute3.LineNumber;
					}
					else
					{
						iTypeFlags = GetSchemaType(xmlSchemaAttribute3.SchemaTypeName);
						xmlSchemaAttribute3.Parent = null;
					}
					xmlSchemaAttribute3.SchemaTypeName = RefineSimpleType(attrValue, ref iTypeFlags);
					xmlSchemaAttribute3.LineNumber = iTypeFlags;
				}
			}
			else
			{
				XmlSchemaAttribute xmlSchemaAttribute3 = FindAttribute(attributes, localName);
				if (xmlSchemaAttribute3 == null && collection != null)
				{
					xmlSchemaAttribute3 = FindAttribute(collection, localName);
				}
				if (xmlSchemaAttribute3 == null)
				{
					xmlSchemaAttribute3 = new XmlSchemaAttribute();
					xmlSchemaAttribute3.Name = localName;
					xmlSchemaAttribute3.SchemaTypeName = RefineSimpleType(attrValue, ref iTypeFlags);
					xmlSchemaAttribute3.LineNumber = iTypeFlags;
					if (bCreatingNewType && Occurrence == InferenceOption.Restricted)
					{
						xmlSchemaAttribute3.Use = XmlSchemaUse.Required;
					}
					else
					{
						xmlSchemaAttribute3.Use = XmlSchemaUse.Optional;
					}
					addLocation.Add(xmlSchemaAttribute3);
					if (xmlSchema.AttributeFormDefault != XmlSchemaForm.Unqualified)
					{
						xmlSchemaAttribute3.Form = XmlSchemaForm.Unqualified;
					}
				}
				else
				{
					if (xmlSchemaAttribute3.Parent == null)
					{
						iTypeFlags = xmlSchemaAttribute3.LineNumber;
					}
					else
					{
						iTypeFlags = GetSchemaType(xmlSchemaAttribute3.SchemaTypeName);
						xmlSchemaAttribute3.Parent = null;
					}
					xmlSchemaAttribute3.SchemaTypeName = RefineSimpleType(attrValue, ref iTypeFlags);
					xmlSchemaAttribute3.LineNumber = iTypeFlags;
				}
				result = xmlSchemaAttribute3;
			}
		}
		string text = null;
		if (flag && childURI != parentSchema.TargetNamespace)
		{
			for (int i = 0; i < parentSchema.Includes.Count; i++)
			{
				if (parentSchema.Includes[i] is XmlSchemaImport xmlSchemaImport && xmlSchemaImport.Namespace == childURI)
				{
					flag = false;
				}
			}
			if (flag)
			{
				XmlSchemaImport xmlSchemaImport2 = new XmlSchemaImport();
				xmlSchemaImport2.Schema = xmlSchema;
				if (childURI.Length != 0)
				{
					text = childURI;
				}
				xmlSchemaImport2.Namespace = text;
				parentSchema.Includes.Add(xmlSchemaImport2);
			}
		}
		return result;
	}

	private XmlSchema CreateXmlSchema(string targetNS)
	{
		XmlSchema xmlSchema = new XmlSchema();
		xmlSchema.AttributeFormDefault = XmlSchemaForm.Unqualified;
		xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
		xmlSchema.TargetNamespace = targetNS;
		_schemaSet.Add(xmlSchema);
		return xmlSchema;
	}

	private XmlSchemaElement AddElement(string localName, string prefix, string childURI, XmlSchema parentSchema, XmlSchemaObjectCollection addLocation, int positionWithinCollection)
	{
		if (childURI == "http://www.w3.org/2001/XMLSchema")
		{
			throw new XmlSchemaInferenceException(System.SR.SchInf_schema, 0, 0);
		}
		bool bCreatingNewType = true;
		if (childURI == string.Empty)
		{
			childURI = null;
		}
		XmlSchemaElement xmlSchemaElement;
		XmlSchema parentSchema2;
		if (parentSchema != null && childURI == parentSchema.TargetNamespace)
		{
			xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = localName;
			parentSchema2 = parentSchema;
			if (parentSchema2.ElementFormDefault != XmlSchemaForm.Qualified && addLocation != null)
			{
				xmlSchemaElement.Form = XmlSchemaForm.Qualified;
			}
		}
		else if (_schemaSet.Contains(childURI))
		{
			xmlSchemaElement = FindGlobalElement(childURI, localName, out parentSchema2);
			if (xmlSchemaElement == null)
			{
				if (_schemaSet.Schemas(childURI) is ArrayList { Count: >0 } arrayList)
				{
					parentSchema2 = arrayList[0] as XmlSchema;
				}
				xmlSchemaElement = new XmlSchemaElement();
				xmlSchemaElement.Name = localName;
				parentSchema2.Items.Add(xmlSchemaElement);
			}
			else
			{
				bCreatingNewType = false;
			}
		}
		else
		{
			parentSchema2 = CreateXmlSchema(childURI);
			if (prefix.Length != 0)
			{
				_namespaceManager.AddNamespace(prefix, childURI);
			}
			xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = localName;
			parentSchema2.Items.Add(xmlSchemaElement);
		}
		if (parentSchema == null)
		{
			parentSchema = parentSchema2;
			_rootSchema = parentSchema;
		}
		if (childURI != parentSchema.TargetNamespace)
		{
			bool flag = true;
			for (int i = 0; i < parentSchema.Includes.Count; i++)
			{
				if (parentSchema.Includes[i] is XmlSchemaImport xmlSchemaImport && xmlSchemaImport.Namespace == childURI)
				{
					flag = false;
				}
			}
			if (flag)
			{
				XmlSchemaImport xmlSchemaImport2 = new XmlSchemaImport();
				xmlSchemaImport2.Schema = parentSchema2;
				xmlSchemaImport2.Namespace = childURI;
				parentSchema.Includes.Add(xmlSchemaImport2);
			}
		}
		XmlSchemaElement result = xmlSchemaElement;
		if (addLocation != null)
		{
			if (childURI == parentSchema.TargetNamespace)
			{
				if (Occurrence == InferenceOption.Relaxed)
				{
					xmlSchemaElement.MinOccurs = 0m;
				}
				if (positionWithinCollection == -1)
				{
					addLocation.Add(xmlSchemaElement);
				}
				else
				{
					addLocation.Insert(positionWithinCollection, xmlSchemaElement);
				}
			}
			else
			{
				XmlSchemaElement xmlSchemaElement2 = new XmlSchemaElement();
				xmlSchemaElement2.RefName = new XmlQualifiedName(localName, childURI);
				if (Occurrence == InferenceOption.Relaxed)
				{
					xmlSchemaElement2.MinOccurs = 0m;
				}
				if (positionWithinCollection == -1)
				{
					addLocation.Add(xmlSchemaElement2);
				}
				else
				{
					addLocation.Insert(positionWithinCollection, xmlSchemaElement2);
				}
				result = xmlSchemaElement2;
			}
		}
		InferElement(xmlSchemaElement, bCreatingNewType, parentSchema2);
		return result;
	}

	internal void InferElement(XmlSchemaElement xse, bool bCreatingNewType, XmlSchema parentSchema)
	{
		bool isEmptyElement = _xtr.IsEmptyElement;
		int lastUsedSeqItem = -1;
		Hashtable hashtable = new Hashtable();
		XmlSchemaType effectiveSchemaType = GetEffectiveSchemaType(xse, bCreatingNewType);
		XmlSchemaComplexType xmlSchemaComplexType = effectiveSchemaType as XmlSchemaComplexType;
		if (_xtr.MoveToFirstAttribute())
		{
			ProcessAttributes(ref xse, effectiveSchemaType, bCreatingNewType, parentSchema);
		}
		else if (!bCreatingNewType && xmlSchemaComplexType != null)
		{
			MakeExistingAttributesOptional(xmlSchemaComplexType, null);
		}
		if (xmlSchemaComplexType == null || xmlSchemaComplexType == XmlSchemaComplexType.AnyType)
		{
			xmlSchemaComplexType = xse.SchemaType as XmlSchemaComplexType;
		}
		if (isEmptyElement)
		{
			if (!bCreatingNewType)
			{
				if (xmlSchemaComplexType != null)
				{
					if (xmlSchemaComplexType.Particle != null)
					{
						xmlSchemaComplexType.Particle.MinOccurs = 0m;
					}
					else if (xmlSchemaComplexType.ContentModel != null)
					{
						XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = CheckSimpleContentExtension(xmlSchemaComplexType);
						xmlSchemaSimpleContentExtension.BaseTypeName = ST_string;
						xmlSchemaSimpleContentExtension.LineNumber = 262144;
					}
				}
				else if (!xse.SchemaTypeName.IsEmpty)
				{
					xse.LineNumber = 262144;
					xse.SchemaTypeName = ST_string;
				}
			}
			else
			{
				xse.LineNumber = 262144;
			}
			return;
		}
		bool flag = false;
		do
		{
			_xtr.Read();
			if (_xtr.NodeType == XmlNodeType.Whitespace)
			{
				flag = true;
			}
			if (_xtr.NodeType == XmlNodeType.EntityReference)
			{
				throw new XmlSchemaInferenceException(System.SR.SchInf_entity, 0, 0);
			}
		}
		while (!_xtr.EOF && _xtr.NodeType != XmlNodeType.EndElement && _xtr.NodeType != XmlNodeType.CDATA && _xtr.NodeType != XmlNodeType.Element && _xtr.NodeType != XmlNodeType.Text);
		if (_xtr.NodeType == XmlNodeType.EndElement)
		{
			if (flag)
			{
				if (xmlSchemaComplexType != null)
				{
					if (xmlSchemaComplexType.ContentModel != null)
					{
						XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension2 = CheckSimpleContentExtension(xmlSchemaComplexType);
						xmlSchemaSimpleContentExtension2.BaseTypeName = ST_string;
						xmlSchemaSimpleContentExtension2.LineNumber = 262144;
					}
					else if (bCreatingNewType)
					{
						XmlSchemaSimpleContent xmlSchemaSimpleContent = (XmlSchemaSimpleContent)(xmlSchemaComplexType.ContentModel = new XmlSchemaSimpleContent());
						XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension3 = (XmlSchemaSimpleContentExtension)(xmlSchemaSimpleContent.Content = new XmlSchemaSimpleContentExtension());
						MoveAttributes(xmlSchemaComplexType, xmlSchemaSimpleContentExtension3, bCreatingNewType);
						xmlSchemaSimpleContentExtension3.BaseTypeName = ST_string;
						xmlSchemaSimpleContentExtension3.LineNumber = 262144;
					}
					else
					{
						xmlSchemaComplexType.IsMixed = true;
					}
				}
				else
				{
					xse.SchemaTypeName = ST_string;
					xse.LineNumber = 262144;
				}
			}
			if (bCreatingNewType)
			{
				xse.LineNumber = 262144;
			}
			else if (xmlSchemaComplexType != null)
			{
				if (xmlSchemaComplexType.Particle != null)
				{
					xmlSchemaComplexType.Particle.MinOccurs = 0m;
				}
				else if (xmlSchemaComplexType.ContentModel != null)
				{
					XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension4 = CheckSimpleContentExtension(xmlSchemaComplexType);
					xmlSchemaSimpleContentExtension4.BaseTypeName = ST_string;
					xmlSchemaSimpleContentExtension4.LineNumber = 262144;
				}
			}
			else if (!xse.SchemaTypeName.IsEmpty)
			{
				xse.LineNumber = 262144;
				xse.SchemaTypeName = ST_string;
			}
			return;
		}
		int num = 0;
		bool flag2 = false;
		while (!_xtr.EOF && _xtr.NodeType != XmlNodeType.EndElement)
		{
			bool flag3 = false;
			num++;
			if (_xtr.NodeType == XmlNodeType.Text || _xtr.NodeType == XmlNodeType.CDATA)
			{
				if (xmlSchemaComplexType != null)
				{
					if (xmlSchemaComplexType.Particle != null)
					{
						xmlSchemaComplexType.IsMixed = true;
						if (num == 1)
						{
							do
							{
								_xtr.Read();
							}
							while (!_xtr.EOF && (_xtr.NodeType == XmlNodeType.CDATA || _xtr.NodeType == XmlNodeType.Text || _xtr.NodeType == XmlNodeType.Comment || _xtr.NodeType == XmlNodeType.ProcessingInstruction || _xtr.NodeType == XmlNodeType.Whitespace || _xtr.NodeType == XmlNodeType.SignificantWhitespace || _xtr.NodeType == XmlNodeType.XmlDeclaration));
							flag3 = true;
							if (_xtr.NodeType == XmlNodeType.EndElement)
							{
								xmlSchemaComplexType.Particle.MinOccurs = 0m;
							}
						}
					}
					else if (xmlSchemaComplexType.ContentModel != null)
					{
						XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension5 = CheckSimpleContentExtension(xmlSchemaComplexType);
						if (_xtr.NodeType == XmlNodeType.Text && num == 1)
						{
							int num2 = -1;
							if (xse.Parent == null)
							{
								num2 = xmlSchemaSimpleContentExtension5.LineNumber;
							}
							else
							{
								num2 = GetSchemaType(xmlSchemaSimpleContentExtension5.BaseTypeName);
								xse.Parent = null;
							}
							xmlSchemaSimpleContentExtension5.BaseTypeName = RefineSimpleType(_xtr.Value, ref num2);
							xmlSchemaSimpleContentExtension5.LineNumber = num2;
						}
						else
						{
							xmlSchemaSimpleContentExtension5.BaseTypeName = ST_string;
							xmlSchemaSimpleContentExtension5.LineNumber = 262144;
						}
					}
					else
					{
						XmlSchemaSimpleContent xmlSchemaSimpleContent2 = (XmlSchemaSimpleContent)(xmlSchemaComplexType.ContentModel = new XmlSchemaSimpleContent());
						XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension6 = (XmlSchemaSimpleContentExtension)(xmlSchemaSimpleContent2.Content = new XmlSchemaSimpleContentExtension());
						MoveAttributes(xmlSchemaComplexType, xmlSchemaSimpleContentExtension6, bCreatingNewType);
						if (_xtr.NodeType == XmlNodeType.Text)
						{
							int iTypeFlags = (bCreatingNewType ? (-1) : 262144);
							xmlSchemaSimpleContentExtension6.BaseTypeName = RefineSimpleType(_xtr.Value, ref iTypeFlags);
							xmlSchemaSimpleContentExtension6.LineNumber = iTypeFlags;
						}
						else
						{
							xmlSchemaSimpleContentExtension6.BaseTypeName = ST_string;
							xmlSchemaSimpleContentExtension6.LineNumber = 262144;
						}
					}
				}
				else if (num > 1)
				{
					xse.SchemaTypeName = ST_string;
					xse.LineNumber = 262144;
				}
				else
				{
					int iTypeFlags2 = -1;
					if (bCreatingNewType)
					{
						if (_xtr.NodeType == XmlNodeType.Text)
						{
							xse.SchemaTypeName = RefineSimpleType(_xtr.Value, ref iTypeFlags2);
							xse.LineNumber = iTypeFlags2;
						}
						else
						{
							xse.SchemaTypeName = ST_string;
							xse.LineNumber = 262144;
						}
					}
					else if (_xtr.NodeType == XmlNodeType.Text)
					{
						if (xse.Parent == null)
						{
							iTypeFlags2 = xse.LineNumber;
						}
						else
						{
							iTypeFlags2 = GetSchemaType(xse.SchemaTypeName);
							if (iTypeFlags2 == -1 && xse.LineNumber == 262144)
							{
								iTypeFlags2 = 262144;
							}
							xse.Parent = null;
						}
						xse.SchemaTypeName = RefineSimpleType(_xtr.Value, ref iTypeFlags2);
						xse.LineNumber = iTypeFlags2;
					}
					else
					{
						xse.SchemaTypeName = ST_string;
						xse.LineNumber = 262144;
					}
				}
			}
			else if (_xtr.NodeType == XmlNodeType.Element)
			{
				XmlQualifiedName key = new XmlQualifiedName(_xtr.LocalName, _xtr.NamespaceURI);
				bool setMaxoccurs = false;
				if (hashtable.Contains(key))
				{
					setMaxoccurs = true;
				}
				else
				{
					hashtable.Add(key, null);
				}
				if (xmlSchemaComplexType == null)
				{
					xmlSchemaComplexType = (XmlSchemaComplexType)(xse.SchemaType = new XmlSchemaComplexType());
					if (!xse.SchemaTypeName.IsEmpty)
					{
						xmlSchemaComplexType.IsMixed = true;
						xse.SchemaTypeName = XmlQualifiedName.Empty;
					}
				}
				if (xmlSchemaComplexType.ContentModel != null)
				{
					MoveAttributes(CheckSimpleContentExtension(xmlSchemaComplexType), xmlSchemaComplexType);
					xmlSchemaComplexType.ContentModel = null;
					xmlSchemaComplexType.IsMixed = true;
					if (xmlSchemaComplexType.Particle != null)
					{
						throw new XmlSchemaInferenceException(System.SR.SchInf_particle, 0, 0);
					}
					xmlSchemaComplexType.Particle = new XmlSchemaSequence();
					flag2 = true;
					AddElement(_xtr.LocalName, _xtr.Prefix, _xtr.NamespaceURI, parentSchema, ((XmlSchemaSequence)xmlSchemaComplexType.Particle).Items, -1);
					lastUsedSeqItem = 0;
					if (!bCreatingNewType)
					{
						xmlSchemaComplexType.Particle.MinOccurs = 0m;
					}
				}
				else if (xmlSchemaComplexType.Particle == null)
				{
					xmlSchemaComplexType.Particle = new XmlSchemaSequence();
					flag2 = true;
					AddElement(_xtr.LocalName, _xtr.Prefix, _xtr.NamespaceURI, parentSchema, ((XmlSchemaSequence)xmlSchemaComplexType.Particle).Items, -1);
					if (!bCreatingNewType)
					{
						((XmlSchemaSequence)xmlSchemaComplexType.Particle).MinOccurs = 0m;
					}
					lastUsedSeqItem = 0;
				}
				else
				{
					FindMatchingElement(bCreatingNewType | flag2, _xtr, xmlSchemaComplexType, ref lastUsedSeqItem, parentSchema, setMaxoccurs);
				}
			}
			else if (_xtr.NodeType == XmlNodeType.Text)
			{
				if (xmlSchemaComplexType == null)
				{
					throw new XmlSchemaInferenceException(System.SR.SchInf_ct, 0, 0);
				}
				xmlSchemaComplexType.IsMixed = true;
			}
			do
			{
				if (_xtr.NodeType == XmlNodeType.EntityReference)
				{
					throw new XmlSchemaInferenceException(System.SR.SchInf_entity, 0, 0);
				}
				if (!flag3)
				{
					_xtr.Read();
				}
				else
				{
					flag3 = false;
				}
			}
			while (!_xtr.EOF && _xtr.NodeType != XmlNodeType.EndElement && _xtr.NodeType != XmlNodeType.CDATA && _xtr.NodeType != XmlNodeType.Element && _xtr.NodeType != XmlNodeType.Text);
		}
		if (lastUsedSeqItem == -1)
		{
			return;
		}
		while (++lastUsedSeqItem < ((XmlSchemaSequence)xmlSchemaComplexType.Particle).Items.Count)
		{
			if (((XmlSchemaSequence)xmlSchemaComplexType.Particle).Items[lastUsedSeqItem].GetType() != typeof(XmlSchemaElement))
			{
				throw new XmlSchemaInferenceException(System.SR.SchInf_seq, 0, 0);
			}
			((XmlSchemaElement)((XmlSchemaSequence)xmlSchemaComplexType.Particle).Items[lastUsedSeqItem]).MinOccurs = 0m;
		}
	}

	private static XmlSchemaSimpleContentExtension CheckSimpleContentExtension(XmlSchemaComplexType ct)
	{
		return (((ct.ContentModel as XmlSchemaSimpleContent) ?? throw new XmlSchemaInferenceException(System.SR.SchInf_simplecontent, 0, 0)).Content as XmlSchemaSimpleContentExtension) ?? throw new XmlSchemaInferenceException(System.SR.SchInf_extension, 0, 0);
	}

	private XmlSchemaType GetEffectiveSchemaType(XmlSchemaElement elem, bool bCreatingNewType)
	{
		XmlSchemaType result = null;
		if (!bCreatingNewType && elem.ElementSchemaType != null)
		{
			result = elem.ElementSchemaType;
		}
		else if (elem.SchemaType != null)
		{
			result = elem.SchemaType;
		}
		else if (elem.SchemaTypeName != XmlQualifiedName.Empty)
		{
			result = (XmlSchemaType)((_schemaSet.GlobalTypes[elem.SchemaTypeName] as XmlSchemaType) ?? ((object)XmlSchemaType.GetBuiltInSimpleType(elem.SchemaTypeName)) ?? ((object)XmlSchemaType.GetBuiltInComplexType(elem.SchemaTypeName)));
		}
		return result;
	}

	internal XmlSchemaElement FindMatchingElement(bool bCreatingNewType, XmlReader xtr, XmlSchemaComplexType ct, ref int lastUsedSeqItem, XmlSchema parentSchema, bool setMaxoccurs)
	{
		if (xtr.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
		{
			throw new XmlSchemaInferenceException(System.SR.SchInf_schema, 0, 0);
		}
		bool flag = lastUsedSeqItem == -1;
		XmlSchemaObjectCollection xmlSchemaObjectCollection = new XmlSchemaObjectCollection();
		if (ct.Particle.GetType() == typeof(XmlSchemaSequence))
		{
			string text = xtr.NamespaceURI;
			if (text.Length == 0)
			{
				text = null;
			}
			XmlSchemaSequence xmlSchemaSequence = (XmlSchemaSequence)ct.Particle;
			if (xmlSchemaSequence.Items.Count < 1 && !bCreatingNewType)
			{
				lastUsedSeqItem = 0;
				XmlSchemaElement xmlSchemaElement = AddElement(xtr.LocalName, xtr.Prefix, xtr.NamespaceURI, parentSchema, xmlSchemaSequence.Items, -1);
				xmlSchemaElement.MinOccurs = 0m;
				return xmlSchemaElement;
			}
			if (xmlSchemaSequence.Items[0].GetType() == typeof(XmlSchemaChoice))
			{
				XmlSchemaChoice xmlSchemaChoice = (XmlSchemaChoice)xmlSchemaSequence.Items[0];
				for (int i = 0; i < xmlSchemaChoice.Items.Count; i++)
				{
					if (!(xmlSchemaChoice.Items[i] is XmlSchemaElement xmlSchemaElement2))
					{
						throw new XmlSchemaInferenceException(System.SR.SchInf_UnknownParticle, 0, 0);
					}
					if (xmlSchemaElement2.Name == xtr.LocalName && parentSchema.TargetNamespace == text)
					{
						InferElement(xmlSchemaElement2, bCreatingNewType: false, parentSchema);
						SetMinMaxOccurs(xmlSchemaElement2, setMaxoccurs);
						return xmlSchemaElement2;
					}
					if (xmlSchemaElement2.RefName.Name == xtr.LocalName && xmlSchemaElement2.RefName.Namespace == xtr.NamespaceURI)
					{
						XmlSchemaElement xmlSchemaElement3 = FindGlobalElement(text, xtr.LocalName, out parentSchema);
						InferElement(xmlSchemaElement3, bCreatingNewType: false, parentSchema);
						SetMinMaxOccurs(xmlSchemaElement2, setMaxoccurs);
						return xmlSchemaElement3;
					}
				}
				return AddElement(xtr.LocalName, xtr.Prefix, xtr.NamespaceURI, parentSchema, xmlSchemaChoice.Items, -1);
			}
			int num = 0;
			if (lastUsedSeqItem >= 0)
			{
				num = lastUsedSeqItem;
			}
			if (!(xmlSchemaSequence.Items[num] as XmlSchemaParticle is XmlSchemaElement xmlSchemaElement4))
			{
				throw new XmlSchemaInferenceException(System.SR.SchInf_UnknownParticle, 0, 0);
			}
			if (xmlSchemaElement4.Name == xtr.LocalName && parentSchema.TargetNamespace == text)
			{
				if (!flag)
				{
					xmlSchemaElement4.MaxOccurs = decimal.MaxValue;
				}
				lastUsedSeqItem = num;
				InferElement(xmlSchemaElement4, bCreatingNewType: false, parentSchema);
				SetMinMaxOccurs(xmlSchemaElement4, setMaxOccurs: false);
				return xmlSchemaElement4;
			}
			if (xmlSchemaElement4.RefName.Name == xtr.LocalName && xmlSchemaElement4.RefName.Namespace == xtr.NamespaceURI)
			{
				if (!flag)
				{
					xmlSchemaElement4.MaxOccurs = decimal.MaxValue;
				}
				lastUsedSeqItem = num;
				XmlSchemaElement xse = FindGlobalElement(text, xtr.LocalName, out parentSchema);
				InferElement(xse, bCreatingNewType: false, parentSchema);
				SetMinMaxOccurs(xmlSchemaElement4, setMaxOccurs: false);
				return xmlSchemaElement4;
			}
			if (flag && xmlSchemaElement4.MinOccurs != 0m)
			{
				xmlSchemaObjectCollection.Add(xmlSchemaElement4);
			}
			for (num++; num < xmlSchemaSequence.Items.Count; num++)
			{
				if (!(xmlSchemaSequence.Items[num] as XmlSchemaParticle is XmlSchemaElement xmlSchemaElement5))
				{
					throw new XmlSchemaInferenceException(System.SR.SchInf_UnknownParticle, 0, 0);
				}
				if (xmlSchemaElement5.Name == xtr.LocalName && parentSchema.TargetNamespace == text)
				{
					lastUsedSeqItem = num;
					for (int j = 0; j < xmlSchemaObjectCollection.Count; j++)
					{
						((XmlSchemaElement)xmlSchemaObjectCollection[j]).MinOccurs = 0m;
					}
					InferElement(xmlSchemaElement5, bCreatingNewType: false, parentSchema);
					SetMinMaxOccurs(xmlSchemaElement5, setMaxoccurs);
					return xmlSchemaElement5;
				}
				if (xmlSchemaElement5.RefName.Name == xtr.LocalName && xmlSchemaElement5.RefName.Namespace == xtr.NamespaceURI)
				{
					lastUsedSeqItem = num;
					for (int k = 0; k < xmlSchemaObjectCollection.Count; k++)
					{
						((XmlSchemaElement)xmlSchemaObjectCollection[k]).MinOccurs = 0m;
					}
					XmlSchemaElement xmlSchemaElement6 = FindGlobalElement(text, xtr.LocalName, out parentSchema);
					InferElement(xmlSchemaElement6, bCreatingNewType: false, parentSchema);
					SetMinMaxOccurs(xmlSchemaElement5, setMaxoccurs);
					return xmlSchemaElement6;
				}
				xmlSchemaObjectCollection.Add(xmlSchemaElement5);
			}
			XmlSchemaElement xse2 = null;
			XmlSchemaElement xmlSchemaElement7;
			if (parentSchema.TargetNamespace == text)
			{
				xmlSchemaElement7 = FindElement(xmlSchemaSequence.Items, xtr.LocalName);
				xse2 = xmlSchemaElement7;
			}
			else
			{
				xmlSchemaElement7 = FindElementRef(xmlSchemaSequence.Items, xtr.LocalName, xtr.NamespaceURI);
				if (xmlSchemaElement7 != null)
				{
					xse2 = FindGlobalElement(text, xtr.LocalName, out parentSchema);
				}
			}
			if (xmlSchemaElement7 != null)
			{
				XmlSchemaChoice xmlSchemaChoice2 = new XmlSchemaChoice();
				xmlSchemaChoice2.MaxOccurs = decimal.MaxValue;
				SetMinMaxOccurs(xmlSchemaElement7, setMaxoccurs);
				InferElement(xse2, bCreatingNewType: false, parentSchema);
				for (int l = 0; l < xmlSchemaSequence.Items.Count; l++)
				{
					xmlSchemaChoice2.Items.Add(CreateNewElementforChoice((XmlSchemaElement)xmlSchemaSequence.Items[l]));
				}
				xmlSchemaSequence.Items.Clear();
				xmlSchemaSequence.Items.Add(xmlSchemaChoice2);
				return xmlSchemaElement7;
			}
			xmlSchemaElement7 = AddElement(xtr.LocalName, xtr.Prefix, xtr.NamespaceURI, parentSchema, xmlSchemaSequence.Items, ++lastUsedSeqItem);
			if (!bCreatingNewType)
			{
				xmlSchemaElement7.MinOccurs = 0m;
			}
			return xmlSchemaElement7;
		}
		throw new XmlSchemaInferenceException(System.SR.SchInf_noseq, 0, 0);
	}

	internal void ProcessAttributes(ref XmlSchemaElement xse, XmlSchemaType effectiveSchemaType, bool bCreatingNewType, XmlSchema parentSchema)
	{
		XmlSchemaObjectCollection xmlSchemaObjectCollection = new XmlSchemaObjectCollection();
		XmlSchemaComplexType xmlSchemaComplexType = effectiveSchemaType as XmlSchemaComplexType;
		do
		{
			if (_xtr.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				throw new XmlSchemaInferenceException(System.SR.SchInf_schema, 0, 0);
			}
			if (_xtr.NamespaceURI == "http://www.w3.org/2000/xmlns/")
			{
				if (_xtr.Prefix == "xmlns")
				{
					_namespaceManager.AddNamespace(_xtr.LocalName, _xtr.Value);
				}
				continue;
			}
			if (_xtr.NamespaceURI == "http://www.w3.org/2001/XMLSchema-instance")
			{
				string localName = _xtr.LocalName;
				if (localName == "nil")
				{
					xse.IsNillable = true;
				}
				else if (localName != "type" && localName != "schemaLocation" && localName != "noNamespaceSchemaLocation")
				{
					throw new XmlSchemaInferenceException(System.SR.Sch_NotXsiAttribute, localName);
				}
				continue;
			}
			if (xmlSchemaComplexType == null || xmlSchemaComplexType == XmlSchemaComplexType.AnyType)
			{
				xmlSchemaComplexType = new XmlSchemaComplexType();
				xse.SchemaType = xmlSchemaComplexType;
			}
			if (effectiveSchemaType != null && effectiveSchemaType.Datatype != null && !xse.SchemaTypeName.IsEmpty)
			{
				XmlSchemaSimpleContent xmlSchemaSimpleContent = (XmlSchemaSimpleContent)(xmlSchemaComplexType.ContentModel = new XmlSchemaSimpleContent());
				XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = (XmlSchemaSimpleContentExtension)(xmlSchemaSimpleContent.Content = new XmlSchemaSimpleContentExtension());
				xmlSchemaSimpleContentExtension.BaseTypeName = xse.SchemaTypeName;
				xmlSchemaSimpleContentExtension.LineNumber = xse.LineNumber;
				xse.LineNumber = 0;
				xse.SchemaTypeName = XmlQualifiedName.Empty;
			}
			XmlSchemaAttribute xmlSchemaAttribute;
			if (xmlSchemaComplexType.ContentModel != null)
			{
				XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension2 = CheckSimpleContentExtension(xmlSchemaComplexType);
				xmlSchemaAttribute = AddAttribute(_xtr.LocalName, _xtr.Prefix, _xtr.NamespaceURI, _xtr.Value, bCreatingNewType, parentSchema, xmlSchemaSimpleContentExtension2.Attributes, xmlSchemaComplexType.AttributeUses);
			}
			else
			{
				xmlSchemaAttribute = AddAttribute(_xtr.LocalName, _xtr.Prefix, _xtr.NamespaceURI, _xtr.Value, bCreatingNewType, parentSchema, xmlSchemaComplexType.Attributes, xmlSchemaComplexType.AttributeUses);
			}
			if (xmlSchemaAttribute != null)
			{
				xmlSchemaObjectCollection.Add(xmlSchemaAttribute);
			}
		}
		while (_xtr.MoveToNextAttribute());
		if (!bCreatingNewType && xmlSchemaComplexType != null)
		{
			MakeExistingAttributesOptional(xmlSchemaComplexType, xmlSchemaObjectCollection);
		}
	}

	private static void MoveAttributes(XmlSchemaSimpleContentExtension scExtension, XmlSchemaComplexType ct)
	{
		for (int i = 0; i < scExtension.Attributes.Count; i++)
		{
			ct.Attributes.Add(scExtension.Attributes[i]);
		}
	}

	private static void MoveAttributes(XmlSchemaComplexType ct, XmlSchemaSimpleContentExtension simpleContentExtension, bool bCreatingNewType)
	{
		ICollection collection = ((bCreatingNewType || ct.AttributeUses.Count <= 0) ? ct.Attributes : ct.AttributeUses.Values);
		foreach (XmlSchemaAttribute item in collection)
		{
			simpleContentExtension.Attributes.Add(item);
		}
		ct.Attributes.Clear();
	}

	internal static XmlSchemaAttribute FindAttribute(ICollection attributes, string attrName)
	{
		foreach (XmlSchemaObject attribute in attributes)
		{
			if (attribute is XmlSchemaAttribute xmlSchemaAttribute && xmlSchemaAttribute.Name == attrName)
			{
				return xmlSchemaAttribute;
			}
		}
		return null;
	}

	internal XmlSchemaElement FindGlobalElement(string namespaceURI, string localName, out XmlSchema parentSchema)
	{
		ICollection collection = _schemaSet.Schemas(namespaceURI);
		parentSchema = null;
		foreach (XmlSchema item in collection)
		{
			XmlSchemaElement xmlSchemaElement = FindElement(item.Items, localName);
			if (xmlSchemaElement != null)
			{
				parentSchema = item;
				return xmlSchemaElement;
			}
		}
		return null;
	}

	internal static XmlSchemaElement FindElement(XmlSchemaObjectCollection elements, string elementName)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i] is XmlSchemaElement xmlSchemaElement && xmlSchemaElement.RefName != null && xmlSchemaElement.Name == elementName)
			{
				return xmlSchemaElement;
			}
		}
		return null;
	}

	internal static XmlSchemaAttribute FindAttributeRef(ICollection attributes, string attributeName, string nsURI)
	{
		foreach (XmlSchemaObject attribute in attributes)
		{
			if (attribute is XmlSchemaAttribute xmlSchemaAttribute && xmlSchemaAttribute.RefName.Name == attributeName && xmlSchemaAttribute.RefName.Namespace == nsURI)
			{
				return xmlSchemaAttribute;
			}
		}
		return null;
	}

	internal static XmlSchemaElement FindElementRef(XmlSchemaObjectCollection elements, string elementName, string nsURI)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i] is XmlSchemaElement xmlSchemaElement && xmlSchemaElement.RefName != null && xmlSchemaElement.RefName.Name == elementName && xmlSchemaElement.RefName.Namespace == nsURI)
			{
				return xmlSchemaElement;
			}
		}
		return null;
	}

	internal static void MakeExistingAttributesOptional(XmlSchemaComplexType ct, XmlSchemaObjectCollection attributesInInstance)
	{
		if (ct == null)
		{
			throw new XmlSchemaInferenceException(System.SR.SchInf_noct, 0, 0);
		}
		if (ct.ContentModel != null)
		{
			SwitchUseToOptional(CheckSimpleContentExtension(ct).Attributes, attributesInInstance);
		}
		else
		{
			SwitchUseToOptional(ct.Attributes, attributesInInstance);
		}
	}

	private static void SwitchUseToOptional(XmlSchemaObjectCollection attributes, XmlSchemaObjectCollection attributesInInstance)
	{
		for (int i = 0; i < attributes.Count; i++)
		{
			if (!(attributes[i] is XmlSchemaAttribute xmlSchemaAttribute))
			{
				continue;
			}
			if (attributesInInstance != null)
			{
				if (xmlSchemaAttribute.RefName.Name.Length == 0)
				{
					if (FindAttribute(attributesInInstance, xmlSchemaAttribute.Name) == null)
					{
						xmlSchemaAttribute.Use = XmlSchemaUse.Optional;
					}
				}
				else if (FindAttributeRef(attributesInInstance, xmlSchemaAttribute.RefName.Name, xmlSchemaAttribute.RefName.Namespace) == null)
				{
					xmlSchemaAttribute.Use = XmlSchemaUse.Optional;
				}
			}
			else
			{
				xmlSchemaAttribute.Use = XmlSchemaUse.Optional;
			}
		}
	}

	internal XmlQualifiedName RefineSimpleType(string s, ref int iTypeFlags)
	{
		bool bNeedsRangeCheck = false;
		s = s.Trim();
		if (iTypeFlags == 262144 || _typeInference == InferenceOption.Relaxed)
		{
			return ST_string;
		}
		iTypeFlags &= InferSimpleType(s, ref bNeedsRangeCheck);
		if (iTypeFlags == 262144)
		{
			return ST_string;
		}
		if (bNeedsRangeCheck)
		{
			if ((iTypeFlags & 2) != 0)
			{
				try
				{
					XmlConvert.ToSByte(s);
					if ((iTypeFlags & 4) != 0)
					{
						return ST_unsignedByte;
					}
					return ST_byte;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -3;
			}
			if ((iTypeFlags & 4) != 0)
			{
				try
				{
					XmlConvert.ToByte(s);
					return ST_unsignedByte;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -5;
			}
			if ((iTypeFlags & 8) != 0)
			{
				try
				{
					XmlConvert.ToInt16(s);
					if ((iTypeFlags & 0x10) != 0)
					{
						return ST_unsignedShort;
					}
					return ST_short;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -9;
			}
			if ((iTypeFlags & 0x10) != 0)
			{
				try
				{
					XmlConvert.ToUInt16(s);
					return ST_unsignedShort;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -17;
			}
			if ((iTypeFlags & 0x20) != 0)
			{
				try
				{
					XmlConvert.ToInt32(s);
					if ((iTypeFlags & 0x40) != 0)
					{
						return ST_unsignedInt;
					}
					return ST_int;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -33;
			}
			if ((iTypeFlags & 0x40) != 0)
			{
				try
				{
					XmlConvert.ToUInt32(s);
					return ST_unsignedInt;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -65;
			}
			if ((iTypeFlags & 0x80) != 0)
			{
				try
				{
					XmlConvert.ToInt64(s);
					if ((iTypeFlags & 0x100) != 0)
					{
						return ST_unsignedLong;
					}
					return ST_long;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -129;
			}
			if ((iTypeFlags & 0x100) != 0)
			{
				try
				{
					XmlConvert.ToUInt64(s);
					return ST_unsignedLong;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -257;
			}
			if ((iTypeFlags & 0x1000) != 0)
			{
				try
				{
					double num = XmlConvert.ToDouble(s);
					if ((iTypeFlags & 0x200) != 0)
					{
						return ST_integer;
					}
					if ((iTypeFlags & 0x400) != 0)
					{
						return ST_decimal;
					}
					if ((iTypeFlags & 0x800) != 0)
					{
						try
						{
							if (string.Equals(XmlConvert.ToString(XmlConvert.ToSingle(s)), XmlConvert.ToString(num), StringComparison.OrdinalIgnoreCase))
							{
								return ST_float;
							}
						}
						catch (FormatException)
						{
						}
						catch (OverflowException)
						{
						}
					}
					iTypeFlags &= -2049;
					return ST_double;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -4097;
			}
			if ((iTypeFlags & 0x800) != 0)
			{
				try
				{
					XmlConvert.ToSingle(s);
					if ((iTypeFlags & 0x200) != 0)
					{
						return ST_integer;
					}
					if ((iTypeFlags & 0x400) != 0)
					{
						return ST_decimal;
					}
					return ST_float;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags &= -2049;
			}
			if ((iTypeFlags & 0x200) != 0)
			{
				return ST_integer;
			}
			if ((iTypeFlags & 0x400) != 0)
			{
				return ST_decimal;
			}
			if (iTypeFlags == 393216)
			{
				try
				{
					XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
					return ST_gYearMonth;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags = 262144;
				return ST_string;
			}
			if (iTypeFlags == 270336)
			{
				try
				{
					XmlConvert.ToTimeSpan(s);
					return ST_duration;
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
				iTypeFlags = 262144;
				return ST_string;
			}
			if (iTypeFlags == 262145)
			{
				return ST_boolean;
			}
		}
		return iTypeFlags switch
		{
			262144 => ST_string, 
			1 => ST_boolean, 
			2 => ST_byte, 
			4 => ST_unsignedByte, 
			8 => ST_short, 
			16 => ST_unsignedShort, 
			32 => ST_int, 
			64 => ST_unsignedInt, 
			128 => ST_long, 
			256 => ST_unsignedLong, 
			512 => ST_integer, 
			1024 => ST_decimal, 
			2048 => ST_float, 
			4096 => ST_double, 
			8192 => ST_duration, 
			16384 => ST_dateTime, 
			32768 => ST_time, 
			65536 => ST_date, 
			131072 => ST_gYearMonth, 
			262145 => ST_boolean, 
			278528 => ST_dateTime, 
			327680 => ST_date, 
			294912 => ST_time, 
			268288 => ST_float, 
			266240 => ST_double, 
			_ => ST_string, 
		};
	}

	internal static int InferSimpleType(string s, ref bool bNeedsRangeCheck)
	{
		bool flag = false;
		bool flag2 = false;
		bool bDate = false;
		bool bTime = false;
		bool flag3 = false;
		if (s.Length == 0)
		{
			return 262144;
		}
		int num = 0;
		char c;
		switch (s[num])
		{
		case 'f':
		case 't':
			if (s == "true")
			{
				return 262145;
			}
			if (s == "false")
			{
				return 262145;
			}
			return 262144;
		case 'N':
			if (s == "NaN")
			{
				return 268288;
			}
			return 262144;
		case 'I':
			if (s.AsSpan(num).SequenceEqual("INF".AsSpan()))
			{
				return 268288;
			}
			return 262144;
		case '.':
			bNeedsRangeCheck = true;
			num++;
			if (num == s.Length)
			{
				if (num == 1 || (num == 2 && (flag2 | flag)))
				{
					return 262144;
				}
				return 269312;
			}
			c = s[num];
			if (c != 'E' && c != 'e')
			{
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				while (true)
				{
					num++;
					if (num == s.Length)
					{
						return 269312;
					}
					c = s[num];
					if (c == 'E' || c == 'e')
					{
						break;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
				}
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			c = s[num];
			if (c != '+' && c != '-')
			{
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
			}
			else
			{
				num++;
				if (num == s.Length)
				{
					return 262144;
				}
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
			}
			do
			{
				num++;
				if (num == s.Length)
				{
					return 268288;
				}
			}
			while (char.IsAsciiDigit(s[num]));
			return 262144;
		case '-':
			flag = true;
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			c = s[num];
			if (c == '.')
			{
				goto case '.';
			}
			if (c == 'I')
			{
				goto case 'I';
			}
			if (c == 'P')
			{
				goto case 'P';
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			goto case '0';
		case '+':
			flag2 = true;
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			c = s[num];
			if (c == '.')
			{
				goto case '.';
			}
			if (c == 'P')
			{
				goto case 'P';
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			goto case '0';
		case 'P':
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (s[num] != 'T')
			{
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				while (true)
				{
					num++;
					if (num == s.Length)
					{
						return 262144;
					}
					c = s[num];
					if (c != 'D')
					{
						if (c == 'M')
						{
							goto IL_03d9;
						}
						if (c != 'Y')
						{
							if (!char.IsAsciiDigit(s[num]))
							{
								return 262144;
							}
							continue;
						}
						num++;
						if (num == s.Length)
						{
							bNeedsRangeCheck = true;
							return 270336;
						}
						if (s[num] == 'T')
						{
							break;
						}
						if (!char.IsAsciiDigit(s[num]))
						{
							return 262144;
						}
						while (true)
						{
							num++;
							if (num == s.Length)
							{
								return 262144;
							}
							c = s[num];
							if (c == 'D')
							{
								break;
							}
							if (c != 'M')
							{
								if (!char.IsAsciiDigit(s[num]))
								{
									return 262144;
								}
								continue;
							}
							goto IL_03d9;
						}
					}
					goto IL_044a;
					IL_044a:
					num++;
					if (num == s.Length)
					{
						bNeedsRangeCheck = true;
						return 270336;
					}
					if (s[num] == 'T')
					{
						break;
					}
					return 262144;
					IL_03d9:
					num++;
					if (num == s.Length)
					{
						bNeedsRangeCheck = true;
						return 270336;
					}
					if (s[num] == 'T')
					{
						break;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
					while (true)
					{
						num++;
						if (num == s.Length)
						{
							return 262144;
						}
						if (s[num] == 'D')
						{
							break;
						}
						if (!char.IsAsciiDigit(s[num]))
						{
							return 262144;
						}
					}
					goto IL_044a;
				}
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			while (true)
			{
				num++;
				if (num == s.Length)
				{
					return 262144;
				}
				c = s[num];
				if ((uint)c <= 72u)
				{
					if (c != '.')
					{
						if (c != 'H')
						{
							goto IL_04e9;
						}
						num++;
						if (num == s.Length)
						{
							bNeedsRangeCheck = true;
							return 270336;
						}
						if (!char.IsAsciiDigit(s[num]))
						{
							return 262144;
						}
						while (true)
						{
							num++;
							if (num == s.Length)
							{
								return 262144;
							}
							c = s[num];
							if (c == '.')
							{
								break;
							}
							if (c != 'M')
							{
								if (c == 'S')
								{
									goto end_IL_04a0;
								}
								if (!char.IsAsciiDigit(s[num]))
								{
									return 262144;
								}
								continue;
							}
							goto IL_0579;
						}
					}
					goto IL_05e8;
				}
				if (c != 'M')
				{
					if (c == 'S')
					{
						break;
					}
					goto IL_04e9;
				}
				goto IL_0579;
				IL_04e9:
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				continue;
				IL_05e8:
				num++;
				if (num == s.Length)
				{
					bNeedsRangeCheck = true;
					return 270336;
				}
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				while (true)
				{
					num++;
					if (num == s.Length)
					{
						return 262144;
					}
					if (s[num] == 'S')
					{
						break;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
				}
				break;
				IL_0579:
				num++;
				if (num == s.Length)
				{
					bNeedsRangeCheck = true;
					return 270336;
				}
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				while (true)
				{
					num++;
					if (num == s.Length)
					{
						return 262144;
					}
					c = s[num];
					if (c == '.')
					{
						break;
					}
					if (c == 'S')
					{
						goto end_IL_04a0;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
				}
				goto IL_05e8;
				continue;
				end_IL_04a0:
				break;
			}
			num++;
			if (num == s.Length)
			{
				bNeedsRangeCheck = true;
				return 270336;
			}
			return 262144;
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			num++;
			if (num == s.Length)
			{
				bNeedsRangeCheck = true;
				if (flag | flag2)
				{
					return 269994;
				}
				if (s == "0" || s == "1")
				{
					return 270335;
				}
				return 270334;
			}
			c = s[num];
			if (c != '.')
			{
				if (c == 'E' || c == 'e')
				{
					bNeedsRangeCheck = true;
					return 268288;
				}
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				num++;
				if (num == s.Length)
				{
					bNeedsRangeCheck = true;
					if (flag | flag2)
					{
						return 269994;
					}
					return 270334;
				}
				c = s[num];
				if ((uint)c <= 58u)
				{
					if (c == '.')
					{
						goto case '.';
					}
					if (c == ':')
					{
						bTime = true;
						goto IL_0b50;
					}
				}
				else if (c == 'E' || c == 'e')
				{
					bNeedsRangeCheck = true;
					return 268288;
				}
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
				num++;
				if (num == s.Length)
				{
					bNeedsRangeCheck = true;
					if (flag | flag2)
					{
						return 269994;
					}
					return 270334;
				}
				c = s[num];
				if (c != '.')
				{
					if (c == 'E' || c == 'e')
					{
						bNeedsRangeCheck = true;
						return 268288;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
					while (true)
					{
						num++;
						if (num == s.Length)
						{
							break;
						}
						c = s[num];
						if ((uint)c <= 46u)
						{
							if (c == '-')
							{
								goto IL_0819;
							}
							if (c == '.')
							{
								goto case '.';
							}
						}
						else if (c == 'E' || c == 'e')
						{
							bNeedsRangeCheck = true;
							return 268288;
						}
						if (!char.IsAsciiDigit(s[num]))
						{
							return 262144;
						}
					}
					bNeedsRangeCheck = true;
					if (flag | flag2)
					{
						return 269994;
					}
					return 270334;
				}
			}
			goto case '.';
		default:
			{
				return 262144;
			}
			IL_0819:
			bDate = true;
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				bNeedsRangeCheck = true;
				return 393216;
			}
			c = s[num];
			if ((uint)c <= 45u)
			{
				if (c == '+')
				{
					flag3 = true;
					goto IL_09d2;
				}
				if (c == '-')
				{
					num++;
					if (num == s.Length)
					{
						return 262144;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
					num++;
					if (num == s.Length)
					{
						return 262144;
					}
					if (!char.IsAsciiDigit(s[num]))
					{
						return 262144;
					}
					num++;
					if (num == s.Length)
					{
						return DateTime(s, bDate, bTime);
					}
					c = s[num];
					if ((uint)c <= 58u)
					{
						if (c == '+' || c == '-')
						{
							goto IL_09d2;
						}
						if (c == ':')
						{
							flag3 = true;
							goto IL_0a50;
						}
					}
					else
					{
						if (c == 'T')
						{
							bTime = true;
							num++;
							if (num == s.Length)
							{
								return 262144;
							}
							if (!char.IsAsciiDigit(s[num]))
							{
								return 262144;
							}
							num++;
							if (num == s.Length)
							{
								return 262144;
							}
							if (!char.IsAsciiDigit(s[num]))
							{
								return 262144;
							}
							num++;
							if (num == s.Length)
							{
								return 262144;
							}
							if (s[num] != ':')
							{
								return 262144;
							}
							goto IL_0b50;
						}
						if (c == 'Z' || c == 'z')
						{
							goto IL_09a6;
						}
					}
					return 262144;
				}
			}
			else if (c == 'Z' || c == 'z')
			{
				flag3 = true;
				goto IL_09a6;
			}
			return 262144;
			IL_09d2:
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (s[num] != ':')
			{
				return 262144;
			}
			goto IL_0a50;
			IL_0a50:
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				if (flag3)
				{
					bNeedsRangeCheck = true;
					return 393216;
				}
				return DateTime(s, bDate, bTime);
			}
			return 262144;
			IL_09a6:
			num++;
			if (num == s.Length)
			{
				if (flag3)
				{
					bNeedsRangeCheck = true;
					return 393216;
				}
				return DateTime(s, bDate, bTime);
			}
			return 262144;
			IL_0c79:
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			while (true)
			{
				num++;
				if (num == s.Length)
				{
					return DateTime(s, bDate, bTime);
				}
				c = s[num];
				if ((uint)c <= 45u)
				{
					if (c == '+' || c == '-')
					{
						break;
					}
				}
				else if (c == 'Z' || c == 'z')
				{
					goto IL_09a6;
				}
				if (!char.IsAsciiDigit(s[num]))
				{
					return 262144;
				}
			}
			goto IL_09d2;
			IL_0b50:
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (s[num] != ':')
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return 262144;
			}
			if (!char.IsAsciiDigit(s[num]))
			{
				return 262144;
			}
			num++;
			if (num == s.Length)
			{
				return DateTime(s, bDate, bTime);
			}
			switch (s[num])
			{
			case 'Z':
			case 'z':
				break;
			case '+':
			case '-':
				goto IL_09d2;
			default:
				return 262144;
			case '.':
				goto IL_0c79;
			}
			goto IL_09a6;
		}
	}

	internal static int DateTime(string s, bool bDate, bool bTime)
	{
		try
		{
			XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
		}
		catch (FormatException)
		{
			return 262144;
		}
		if (bDate & bTime)
		{
			return 278528;
		}
		if (bDate)
		{
			return 327680;
		}
		if (bTime)
		{
			return 294912;
		}
		return 262144;
	}

	private XmlSchemaElement CreateNewElementforChoice(XmlSchemaElement copyElement)
	{
		XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
		xmlSchemaElement.Annotation = copyElement.Annotation;
		xmlSchemaElement.Block = copyElement.Block;
		xmlSchemaElement.DefaultValue = copyElement.DefaultValue;
		xmlSchemaElement.Final = copyElement.Final;
		xmlSchemaElement.FixedValue = copyElement.FixedValue;
		xmlSchemaElement.Form = copyElement.Form;
		xmlSchemaElement.Id = copyElement.Id;
		if (copyElement.IsNillable)
		{
			xmlSchemaElement.IsNillable = copyElement.IsNillable;
		}
		xmlSchemaElement.LineNumber = copyElement.LineNumber;
		xmlSchemaElement.LinePosition = copyElement.LinePosition;
		xmlSchemaElement.Name = copyElement.Name;
		xmlSchemaElement.Namespaces = copyElement.Namespaces;
		xmlSchemaElement.RefName = copyElement.RefName;
		xmlSchemaElement.SchemaType = copyElement.SchemaType;
		xmlSchemaElement.SchemaTypeName = copyElement.SchemaTypeName;
		xmlSchemaElement.SourceUri = copyElement.SourceUri;
		xmlSchemaElement.SubstitutionGroup = copyElement.SubstitutionGroup;
		xmlSchemaElement.UnhandledAttributes = copyElement.UnhandledAttributes;
		if (copyElement.MinOccurs != 1m && Occurrence == InferenceOption.Relaxed)
		{
			xmlSchemaElement.MinOccurs = copyElement.MinOccurs;
		}
		if (copyElement.MaxOccurs != 1m)
		{
			xmlSchemaElement.MaxOccurs = copyElement.MaxOccurs;
		}
		return xmlSchemaElement;
	}

	private static int GetSchemaType(XmlQualifiedName qname)
	{
		if (qname == SimpleTypes[0])
		{
			return 262145;
		}
		if (qname == SimpleTypes[1])
		{
			return 269994;
		}
		if (qname == SimpleTypes[2])
		{
			return 270334;
		}
		if (qname == SimpleTypes[3])
		{
			return 269992;
		}
		if (qname == SimpleTypes[4])
		{
			return 270328;
		}
		if (qname == SimpleTypes[5])
		{
			return 269984;
		}
		if (qname == SimpleTypes[6])
		{
			return 270304;
		}
		if (qname == SimpleTypes[7])
		{
			return 269952;
		}
		if (qname == SimpleTypes[8])
		{
			return 270208;
		}
		if (qname == SimpleTypes[9])
		{
			return 269824;
		}
		if (qname == SimpleTypes[10])
		{
			return 269312;
		}
		if (qname == SimpleTypes[11])
		{
			return 268288;
		}
		if (qname == SimpleTypes[12])
		{
			return 266240;
		}
		if (qname == SimpleTypes[13])
		{
			return 270336;
		}
		if (qname == SimpleTypes[14])
		{
			return 278528;
		}
		if (qname == SimpleTypes[15])
		{
			return 294912;
		}
		if (qname == SimpleTypes[16])
		{
			return 65536;
		}
		if (qname == SimpleTypes[17])
		{
			return 131072;
		}
		if (qname == SimpleTypes[18])
		{
			return 262144;
		}
		if (qname == null || qname.IsEmpty)
		{
			return -1;
		}
		throw new XmlSchemaInferenceException(System.SR.SchInf_schematype, 0, 0);
	}

	internal void SetMinMaxOccurs(XmlSchemaElement el, bool setMaxOccurs)
	{
		if (Occurrence == InferenceOption.Relaxed)
		{
			if (setMaxOccurs || el.MaxOccurs > 1m)
			{
				el.MaxOccurs = decimal.MaxValue;
			}
			el.MinOccurs = 0m;
		}
		else if (el.MinOccurs > 1m)
		{
			el.MinOccurs = 1m;
		}
	}
}
