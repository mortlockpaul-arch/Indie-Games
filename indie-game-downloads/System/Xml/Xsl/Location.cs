using System.Diagnostics;

namespace System.Xml.Xsl;

[DebuggerDisplay("({Line},{Pos})")]
internal readonly struct Location
{
	private readonly ulong _value = (ulong)(((long)line << 32) | (uint)pos);

	public int Line => (int)(_value >> 32);

	public int Pos => (int)_value;

	public Location(int line, int pos)
	{
	}

	public bool LessOrEqual(Location that)
	{
		return _value <= that._value;
	}
}
