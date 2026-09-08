using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers;

internal struct ProbabilisticMapState
{
	public ProbabilisticMap Map;

	private readonly uint _multiplier;

	private readonly char[] _hashEntries;

	private unsafe readonly ReadOnlySpan<char>* _slowContainsValuesPtr;

	public unsafe ProbabilisticMapState(ReadOnlySpan<char> values, int maxInclusive)
	{
		_slowContainsValuesPtr = default(ReadOnlySpan<char>*);
		Map = new ProbabilisticMap(values);
		uint num = FindModulus(values, maxInclusive);
		_multiplier = GetFastModMultiplier(num);
		_hashEntries = new char[num];
		_hashEntries.AsSpan().Fill(values[0]);
		ReadOnlySpan<char> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			_hashEntries[FastMod(c, num, _multiplier)] = c;
		}
	}

	public unsafe ProbabilisticMapState(ReadOnlySpan<char>* valuesPtr)
	{
		_multiplier = 0u;
		_hashEntries = null;
		Map = new ProbabilisticMap(*valuesPtr);
		_slowContainsValuesPtr = valuesPtr;
	}

	public char[] GetValues()
	{
		HashSet<char> hashSet = new HashSet<char>(_hashEntries);
		char[] array = new char[hashSet.Count];
		hashSet.CopyTo(array);
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool FastContains(char value)
	{
		return FastContains(_hashEntries, _multiplier, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool FastContains(char[] hashEntries, uint multiplier, char value)
	{
		ulong num = FastMod(value, (uint)hashEntries.Length, multiplier);
		return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(hashEntries), (nuint)num) == value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe bool SlowProbabilisticContains(char value)
	{
		return ProbabilisticMap.Contains(ref Unsafe.As<ProbabilisticMap, uint>(ref Map), *_slowContainsValuesPtr, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe bool SlowContains(char value)
	{
		return ProbabilisticMap.Contains(*_slowContainsValuesPtr, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ConfirmProbabilisticMatch<TUseFastContains>(char value) where TUseFastContains : struct, SearchValues.IRuntimeConst
	{
		if (TUseFastContains.Value)
		{
			return FastContains(value);
		}
		return SlowContains(value);
	}

	private static uint FindModulus(ReadOnlySpan<char> values, int maxInclusive)
	{
		int prime = HashHelpers.GetPrime(values.Length);
		bool flag = false;
		if (prime >= maxInclusive)
		{
			return (uint)(maxInclusive + 1);
		}
		while (true)
		{
			if (prime >= maxInclusive)
			{
				if (flag || !TryRemoveDuplicates(values, out var deduplicated))
				{
					return (uint)(maxInclusive + 1);
				}
				flag = true;
				values = deduplicated;
				prime = HashHelpers.GetPrime(values.Length);
			}
			else
			{
				if (TestModulus(values, prime))
				{
					break;
				}
				prime = HashHelpers.GetPrime(prime + 1);
			}
		}
		return (uint)prime;
		static bool TestModulus(ReadOnlySpan<char> readOnlySpan2, int modulus)
		{
			bool[] array = ArrayPool<bool>.Shared.Rent(modulus);
			array.AsSpan(0, modulus).Clear();
			uint fastModMultiplier = GetFastModMultiplier((uint)modulus);
			ReadOnlySpan<char> readOnlySpan = readOnlySpan2;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ulong num = FastMod(readOnlySpan[i], (uint)modulus, fastModMultiplier);
				if (array[num])
				{
					ArrayPool<bool>.Shared.Return(array);
					return false;
				}
				array[num] = true;
			}
			ArrayPool<bool>.Shared.Return(array);
			return true;
		}
		static bool TryRemoveDuplicates(ReadOnlySpan<char> readOnlySpan, [NotNullWhen(true)] out char[] reference)
		{
			HashSet<char> hashSet = new HashSet<char>();
			ReadOnlySpan<char>.Enumerator enumerator = readOnlySpan.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					char current = enumerator.Current;
					hashSet.Add(current);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			HashSet<char> hashSet2 = hashSet;
			if (hashSet2.Count == readOnlySpan.Length)
			{
				reference = null;
				return false;
			}
			reference = new char[hashSet2.Count];
			hashSet2.CopyTo(reference);
			return true;
		}
	}

	private static uint GetFastModMultiplier(uint divisor)
	{
		return uint.MaxValue / divisor + 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong FastMod(char value, uint divisor, uint multiplier)
	{
		return (ulong)((long)(multiplier * value) * (long)divisor) >> 32;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnySimpleLoop<TUseFastContains, TNegator>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		ref char right = ref Unsafe.Add(ref searchSpace, searchSpaceLength);
		ref char reference = ref searchSpace;
		if (TUseFastContains.Value)
		{
			char[] hashEntries = state._hashEntries;
			uint multiplier = state._multiplier;
			while (!Unsafe.AreSame(in reference, in right))
			{
				char value = reference;
				if (TNegator.NegateIfNeeded(FastContains(hashEntries, multiplier, value)))
				{
					return (int)((nuint)Unsafe.ByteOffset(in searchSpace, in reference) / (nuint)2u);
				}
				reference = ref Unsafe.Add(ref reference, 1);
			}
		}
		else
		{
			while (!Unsafe.AreSame(in reference, in right))
			{
				char value2 = reference;
				if (TNegator.NegateIfNeeded(state.SlowProbabilisticContains(value2)))
				{
					return (int)((nuint)Unsafe.ByteOffset(in searchSpace, in reference) / (nuint)2u);
				}
				reference = ref Unsafe.Add(ref reference, 1);
			}
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnySimpleLoop<TUseFastContains, TNegator>(ref char searchSpace, int searchSpaceLength, ref ProbabilisticMapState state) where TUseFastContains : struct, SearchValues.IRuntimeConst where TNegator : struct, IndexOfAnyAsciiSearcher.INegator
	{
		if (TUseFastContains.Value)
		{
			char[] hashEntries = state._hashEntries;
			uint multiplier = state._multiplier;
			while (--searchSpaceLength >= 0)
			{
				char value = Unsafe.Add(ref searchSpace, searchSpaceLength);
				if (TNegator.NegateIfNeeded(FastContains(hashEntries, multiplier, value)))
				{
					break;
				}
			}
		}
		else
		{
			while (--searchSpaceLength >= 0)
			{
				char value2 = Unsafe.Add(ref searchSpace, searchSpaceLength);
				if (TNegator.NegateIfNeeded(state.SlowProbabilisticContains(value2)))
				{
					break;
				}
			}
		}
		return searchSpaceLength;
	}
}
