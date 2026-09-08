namespace System.Reflection.Metadata;

public sealed class TypeNameParseOptions
{
	private int _maxNodes = 20;

	public int MaxNodes
	{
		get
		{
			return _maxNodes;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, "value");
			_maxNodes = value;
		}
	}
}
