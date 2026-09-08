namespace System.Xml.Schema;

internal struct Position(int symbol, object particle)
{
	public int symbol = symbol;

	public object particle = particle;
}
