namespace System.Diagnostics.Metrics;

internal struct StringSequenceMany(string[] values) : IEquatable<StringSequenceMany>, IStringSequence
{
	private readonly string[] _values = values;

	public Span<string> AsSpan()
	{
		return _values.AsSpan();
	}

	public bool Equals(StringSequenceMany other)
	{
		return ((ReadOnlySpan<string>)_values.AsSpan()).SequenceEqual((ReadOnlySpan<string>)other._values.AsSpan());
	}

	public override bool Equals(object obj)
	{
		if (obj is StringSequenceMany other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		for (int i = 0; i < _values.Length; i++)
		{
			hashCode.Add(_values[i]);
		}
		return hashCode.ToHashCode();
	}
}
