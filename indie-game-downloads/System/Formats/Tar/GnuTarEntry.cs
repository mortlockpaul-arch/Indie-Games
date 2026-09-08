namespace System.Formats.Tar;

public sealed class GnuTarEntry : PosixTarEntry
{
	public DateTimeOffset AccessTime
	{
		get
		{
			return _header._aTime;
		}
		set
		{
			_header._aTime = value;
		}
	}

	public DateTimeOffset ChangeTime
	{
		get
		{
			return _header._cTime;
		}
		set
		{
			_header._cTime = value;
		}
	}

	internal GnuTarEntry(TarHeader header, TarReader readerOfOrigin)
		: base(header, readerOfOrigin, TarEntryFormat.Gnu)
	{
	}

	public GnuTarEntry(TarEntryType entryType, string entryName)
		: base(entryType, entryName, TarEntryFormat.Gnu, isGea: false)
	{
	}

	public GnuTarEntry(TarEntry other)
		: base(other, TarEntryFormat.Gnu)
	{
		if (other is GnuTarEntry gnuTarEntry)
		{
			_header._aTime = gnuTarEntry.AccessTime;
			_header._cTime = gnuTarEntry.ChangeTime;
		}
	}

	internal override bool IsDataStreamSetterSupported()
	{
		return base.EntryType == TarEntryType.RegularFile;
	}
}
