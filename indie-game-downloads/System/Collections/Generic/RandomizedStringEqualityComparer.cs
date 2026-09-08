using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Collections.Generic;

internal abstract class RandomizedStringEqualityComparer : EqualityComparer<string>, IInternalStringEqualityComparer, IEqualityComparer<string>
{
	private struct MarvinSeed
	{
		internal uint p0;

		internal uint p1;
	}

	private sealed class OrdinalComparer : RandomizedStringEqualityComparer, IAlternateEqualityComparer<ReadOnlySpan<char>, string>
	{
		internal OrdinalComparer(IEqualityComparer<string> wrappedComparer)
			: base(wrappedComparer)
		{
		}

		string IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Create(ReadOnlySpan<char> span)
		{
			return span.ToString();
		}

		public override bool Equals(string x, string y)
		{
			return string.Equals(x, y);
		}

		bool IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Equals(ReadOnlySpan<char> alternate, string other)
		{
			if (alternate.IsEmpty && other == null)
			{
				return false;
			}
			return alternate.SequenceEqual(other.AsSpan());
		}

		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return Marvin.ComputeHash32(ref obj.GetRawStringDataAsUInt8(), (uint)(obj.Length * 2), _seed.p0, _seed.p1);
		}

		int IAlternateEqualityComparer<ReadOnlySpan<char>, string>.GetHashCode(ReadOnlySpan<char> alternate)
		{
			return Marvin.ComputeHash32(ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(alternate)), (uint)(alternate.Length * 2), _seed.p0, _seed.p1);
		}
	}

	private sealed class OrdinalIgnoreCaseComparer : RandomizedStringEqualityComparer, IAlternateEqualityComparer<ReadOnlySpan<char>, string>
	{
		internal OrdinalIgnoreCaseComparer(IEqualityComparer<string> wrappedComparer)
			: base(wrappedComparer)
		{
		}

		string IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Create(ReadOnlySpan<char> span)
		{
			return span.ToString();
		}

		public override bool Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
		}

		bool IAlternateEqualityComparer<ReadOnlySpan<char>, string>.Equals(ReadOnlySpan<char> alternate, string other)
		{
			if (alternate.IsEmpty && other == null)
			{
				return false;
			}
			return alternate.EqualsOrdinalIgnoreCase(other.AsSpan());
		}

		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return Marvin.ComputeHash32OrdinalIgnoreCase(ref obj.GetRawStringData(), obj.Length, _seed.p0, _seed.p1);
		}

		int IAlternateEqualityComparer<ReadOnlySpan<char>, string>.GetHashCode(ReadOnlySpan<char> alternate)
		{
			return Marvin.ComputeHash32OrdinalIgnoreCase(ref MemoryMarshal.GetReference(alternate), alternate.Length, _seed.p0, _seed.p1);
		}
	}

	private readonly MarvinSeed _seed;

	private readonly IEqualityComparer<string> _underlyingComparer;

	private unsafe RandomizedStringEqualityComparer(IEqualityComparer<string> underlyingComparer)
	{
		_underlyingComparer = underlyingComparer;
		fixed (MarvinSeed* seed = &_seed)
		{
			Interop.GetRandomBytes((byte*)seed, sizeof(MarvinSeed));
		}
	}

	internal static RandomizedStringEqualityComparer Create(IEqualityComparer<string> underlyingComparer, bool ignoreCase)
	{
		if (!ignoreCase)
		{
			return new OrdinalComparer(underlyingComparer);
		}
		return new OrdinalIgnoreCaseComparer(underlyingComparer);
	}

	public IEqualityComparer<string> GetUnderlyingEqualityComparer()
	{
		return _underlyingComparer;
	}
}
