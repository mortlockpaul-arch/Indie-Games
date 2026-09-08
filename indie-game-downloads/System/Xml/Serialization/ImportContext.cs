using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace System.Xml.Serialization;

public class ImportContext
{
	private readonly bool _shareTypes;

	private CodeIdentifiers _typeIdentifiers;

	[CompilerGenerated]
	private SchemaObjectCache _003CCache_003Ek__BackingField;

	[CompilerGenerated]
	private Hashtable _003CElements_003Ek__BackingField;

	[CompilerGenerated]
	private Hashtable _003CMappings_003Ek__BackingField;

	internal SchemaObjectCache Cache => _003CCache_003Ek__BackingField ?? (_003CCache_003Ek__BackingField = new SchemaObjectCache());

	internal Hashtable Elements => _003CElements_003Ek__BackingField ?? (_003CElements_003Ek__BackingField = new Hashtable());

	internal Hashtable Mappings => _003CMappings_003Ek__BackingField ?? (_003CMappings_003Ek__BackingField = new Hashtable());

	public CodeIdentifiers TypeIdentifiers => _typeIdentifiers ?? (_typeIdentifiers = new CodeIdentifiers());

	public bool ShareTypes => _shareTypes;

	public StringCollection Warnings => Cache.Warnings;

	public ImportContext(CodeIdentifiers? identifiers, bool shareTypes)
	{
		_typeIdentifiers = identifiers;
		_shareTypes = shareTypes;
	}

	internal ImportContext()
		: this(null, shareTypes: false)
	{
	}
}
