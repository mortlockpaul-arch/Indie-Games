using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Formats.Tar;

public sealed class PaxTarEntry : PosixTarEntry
{
	[CompilerGenerated]
	private IReadOnlyDictionary<string, string> _003CExtendedAttributes_003Ek__BackingField;

	public IReadOnlyDictionary<string, string> ExtendedAttributes => _003CExtendedAttributes_003Ek__BackingField ?? (_003CExtendedAttributes_003Ek__BackingField = _header.ExtendedAttributes.AsReadOnly());

	internal PaxTarEntry(TarHeader header, TarReader readerOfOrigin)
		: base(header, readerOfOrigin, TarEntryFormat.Pax)
	{
	}

	public PaxTarEntry(TarEntryType entryType, string entryName)
		: base(entryType, entryName, TarEntryFormat.Pax, isGea: false)
	{
		_header._prefix = string.Empty;
	}

	public PaxTarEntry(TarEntryType entryType, string entryName, IEnumerable<KeyValuePair<string, string>> extendedAttributes)
		: base(entryType, entryName, TarEntryFormat.Pax, isGea: false)
	{
		ArgumentNullException.ThrowIfNull(extendedAttributes, "extendedAttributes");
		_header._prefix = string.Empty;
		_header.AddExtendedAttributes(extendedAttributes);
	}

	public PaxTarEntry(TarEntry other)
		: base(other, TarEntryFormat.Pax)
	{
		TarEntryFormat format = other._header._format;
		if ((uint)(format - 2) <= 1u)
		{
			_header._prefix = other._header._prefix;
		}
		if (other is PaxTarEntry paxTarEntry)
		{
			_header.AddExtendedAttributes(paxTarEntry.ExtendedAttributes);
		}
		else if (other is GnuTarEntry gnuTarEntry)
		{
			if (gnuTarEntry.AccessTime != default(DateTimeOffset))
			{
				_header.ExtendedAttributes["atime"] = TarHelpers.GetTimestampStringFromDateTimeOffset(gnuTarEntry.AccessTime);
			}
			if (gnuTarEntry.ChangeTime != default(DateTimeOffset))
			{
				_header.ExtendedAttributes["ctime"] = TarHelpers.GetTimestampStringFromDateTimeOffset(gnuTarEntry.ChangeTime);
			}
		}
	}

	internal override bool IsDataStreamSetterSupported()
	{
		return base.EntryType == TarEntryType.RegularFile;
	}
}
