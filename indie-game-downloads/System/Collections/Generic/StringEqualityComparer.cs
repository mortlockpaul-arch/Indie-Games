using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Collections.Generic;

[Serializable]
internal sealed class StringEqualityComparer : EqualityComparer<string>, IAlternateEqualityComparer<ReadOnlySpan<char>, string>, ISerializable
{
	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.SetType(typeof(GenericEqualityComparer<string>));
	}

	public override bool Equals(string x, string y)
	{
		return string.Equals(x, y);
	}

	public override int GetHashCode([DisallowNull] string obj)
	{
		return obj?.GetHashCode() ?? 0;
	}

	public bool Equals(ReadOnlySpan<char> span, string target)
	{
		if (span.IsEmpty && target == null)
		{
			return false;
		}
		return span.SequenceEqual(target.AsSpan());
	}

	public int GetHashCode(ReadOnlySpan<char> span)
	{
		return string.GetHashCode(span);
	}

	public string Create(ReadOnlySpan<char> span)
	{
		return span.ToString();
	}

	public override bool Equals(object obj)
	{
		return obj is StringEqualityComparer;
	}

	public override int GetHashCode()
	{
		return GetType().GetHashCode();
	}
}
