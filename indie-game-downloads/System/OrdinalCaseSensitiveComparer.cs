using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System;

[Serializable]
internal sealed class OrdinalCaseSensitiveComparer : OrdinalComparer, ISerializable, IAlternateEqualityComparer<ReadOnlySpan<char>, string>
{
	internal static readonly OrdinalCaseSensitiveComparer Instance = new OrdinalCaseSensitiveComparer();

	private OrdinalCaseSensitiveComparer()
		: base(ignoreCase: false)
	{
	}

	public override int Compare(string x, string y)
	{
		return string.CompareOrdinal(x, y);
	}

	public override bool Equals(string x, string y)
	{
		return string.Equals(x, y);
	}

	public override int GetHashCode(string obj)
	{
		if (obj == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.obj);
		}
		return obj.GetHashCode();
	}

	bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string target)
	{
		if (span.IsEmpty && target == null)
		{
			return false;
		}
		return span.SequenceEqual(target.AsSpan());
	}

	int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span)
	{
		return string.GetHashCode(span);
	}

	string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span)
	{
		return span.ToString();
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.SetType(typeof(OrdinalComparer));
		info.AddValue("_ignoreCase", value: false);
	}
}
