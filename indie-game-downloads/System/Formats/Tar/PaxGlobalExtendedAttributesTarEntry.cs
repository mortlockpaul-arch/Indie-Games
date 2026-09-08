using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Formats.Tar;

public sealed class PaxGlobalExtendedAttributesTarEntry : PosixTarEntry
{
	[CompilerGenerated]
	private IReadOnlyDictionary<string, string> _003CGlobalExtendedAttributes_003Ek__BackingField;

	public IReadOnlyDictionary<string, string> GlobalExtendedAttributes => _003CGlobalExtendedAttributes_003Ek__BackingField ?? (_003CGlobalExtendedAttributes_003Ek__BackingField = _header.ExtendedAttributes.AsReadOnly());

	internal PaxGlobalExtendedAttributesTarEntry(TarHeader header, TarReader readerOfOrigin)
		: base(header, readerOfOrigin, TarEntryFormat.Pax)
	{
	}

	public PaxGlobalExtendedAttributesTarEntry(IEnumerable<KeyValuePair<string, string>> globalExtendedAttributes)
		: base(TarEntryType.GlobalExtendedAttributes, "PaxGlobalExtendedAttributesTarEntry", TarEntryFormat.Pax, isGea: true)
	{
		ArgumentNullException.ThrowIfNull(globalExtendedAttributes, "globalExtendedAttributes");
		_header.AddExtendedAttributes(globalExtendedAttributes);
	}

	internal override bool IsDataStreamSetterSupported()
	{
		return false;
	}
}
