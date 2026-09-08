using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Collections.Generic;

[Serializable]
public class NonRandomizedStringEqualityComparer : IEqualityComparer<string?>, IInternalStringEqualityComparer, ISerializable
{
	private sealed class OrdinalComparer : NonRandomizedStringEqualityComparer, IAlternateEqualityComparer<ReadOnlySpan<char>, string>
	{
		internal OrdinalComparer(IEqualityComparer<string> wrappedComparer)
			: base(wrappedComparer)
		{
		}

		public override bool Equals(string x, string y)
		{
			return string.Equals(x, y);
		}

		public override int GetHashCode(string obj)
		{
			return obj.GetNonRandomizedHashCode();
		}

		int IAlternateEqualityComparer<ReadOnlySpan<char>, string>.GetHashCode(ReadOnlySpan<char> span)
		{
			return string.GetNonRandomizedHashCode(span);
		}

		bool IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Equals(ReadOnlySpan<char> span, string target)
		{
			if (span.IsEmpty && target == null)
			{
				return false;
			}
			return span.SequenceEqual(target.AsSpan());
		}

		string IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Create(ReadOnlySpan<char> span)
		{
			return span.ToString();
		}
	}

	private sealed class OrdinalIgnoreCaseComparer : NonRandomizedStringEqualityComparer, IAlternateEqualityComparer<ReadOnlySpan<char>, string>
	{
		internal OrdinalIgnoreCaseComparer(IEqualityComparer<string> wrappedComparer)
			: base(wrappedComparer)
		{
		}

		public override bool Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode(string obj)
		{
			return obj.GetNonRandomizedHashCodeOrdinalIgnoreCase();
		}

		int IAlternateEqualityComparer<ReadOnlySpan<char>, string>.GetHashCode(ReadOnlySpan<char> span)
		{
			return string.GetNonRandomizedHashCodeOrdinalIgnoreCase(span);
		}

		bool IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Equals(ReadOnlySpan<char> span, string target)
		{
			if (span.IsEmpty && target == null)
			{
				return false;
			}
			return span.EqualsOrdinalIgnoreCase(target.AsSpan());
		}

		string IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Create(ReadOnlySpan<char> span)
		{
			return span.ToString();
		}

		internal override RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
		{
			return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: true);
		}
	}

	private static readonly NonRandomizedStringEqualityComparer WrappedAroundDefaultComparer = new OrdinalComparer(EqualityComparer<string>.Default);

	private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinal = new OrdinalComparer(StringComparer.Ordinal);

	private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinalIgnoreCase = new OrdinalIgnoreCaseComparer(StringComparer.OrdinalIgnoreCase);

	private readonly IEqualityComparer<string> _underlyingComparer;

	private NonRandomizedStringEqualityComparer(IEqualityComparer<string> underlyingComparer)
	{
		_underlyingComparer = underlyingComparer;
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected NonRandomizedStringEqualityComparer(SerializationInfo information, StreamingContext context)
		: this(EqualityComparer<string>.Default)
	{
	}

	public virtual bool Equals(string? x, string? y)
	{
		return string.Equals(x, y);
	}

	public virtual int GetHashCode(string? obj)
	{
		return obj?.GetNonRandomizedHashCode() ?? 0;
	}

	internal virtual RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
	{
		return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: false);
	}

	public virtual IEqualityComparer<string?> GetUnderlyingEqualityComparer()
	{
		return _underlyingComparer;
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.SetType(typeof(GenericEqualityComparer<string>));
	}

	public static IEqualityComparer<string>? GetStringComparer(object comparer)
	{
		if (comparer == EqualityComparer<string>.Default)
		{
			return WrappedAroundDefaultComparer;
		}
		if (comparer == StringComparer.Ordinal)
		{
			return WrappedAroundStringComparerOrdinal;
		}
		if (comparer == StringComparer.OrdinalIgnoreCase)
		{
			return WrappedAroundStringComparerOrdinalIgnoreCase;
		}
		return null;
	}
}
