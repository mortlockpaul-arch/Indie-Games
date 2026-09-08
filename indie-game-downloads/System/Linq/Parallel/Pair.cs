namespace System.Linq.Parallel;

internal struct Pair<T, U>(T first, U second)
{
	internal T _first = first;

	internal U _second = second;

	public T First
	{
		get
		{
			return _first;
		}
		set
		{
			_first = value;
		}
	}

	public U Second
	{
		get
		{
			return _second;
		}
		set
		{
			_second = value;
		}
	}
}
