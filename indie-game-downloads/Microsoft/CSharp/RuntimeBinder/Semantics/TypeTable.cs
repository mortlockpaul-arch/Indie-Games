using System;
using System.Collections.Generic;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class TypeTable
{
	private readonly struct KeyPair<TKey1, TKey2>(TKey1 pKey1, TKey2 pKey2) : IEquatable<KeyPair<TKey1, TKey2>>
	{
		private readonly TKey1 _pKey1 = pKey1;

		private readonly TKey2 _pKey2 = pKey2;

		public bool Equals(KeyPair<TKey1, TKey2> other)
		{
			if (EqualityComparer<TKey1>.Default.Equals(_pKey1, other._pKey1))
			{
				return EqualityComparer<TKey2>.Default.Equals(_pKey2, other._pKey2);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is KeyPair<TKey1, TKey2>))
			{
				return false;
			}
			return Equals((KeyPair<TKey1, TKey2>)obj);
		}

		public override int GetHashCode()
		{
			int num = ((_pKey1 != null) ? _pKey1.GetHashCode() : 0);
			return (num << 5) - num + ((_pKey2 != null) ? _pKey2.GetHashCode() : 0);
		}
	}

	private static readonly Dictionary<KeyPair<AggregateSymbol, KeyPair<AggregateType, TypeArray>>, AggregateType> s_aggregateTable = new Dictionary<KeyPair<AggregateSymbol, KeyPair<AggregateType, TypeArray>>, AggregateType>();

	private static readonly Dictionary<KeyPair<CType, int>, ArrayType> s_arrayTable = new Dictionary<KeyPair<CType, int>, ArrayType>();

	private static readonly Dictionary<KeyPair<CType, bool>, ParameterModifierType> s_parameterModifierTable = new Dictionary<KeyPair<CType, bool>, ParameterModifierType>();

	private static readonly Dictionary<CType, PointerType> s_pointerTable = new Dictionary<CType, PointerType>();

	private static readonly Dictionary<CType, NullableType> s_nullableTable = new Dictionary<CType, NullableType>();

	private static KeyPair<TKey1, TKey2> MakeKey<TKey1, TKey2>(TKey1 key1, TKey2 key2)
	{
		return new KeyPair<TKey1, TKey2>(key1, key2);
	}

	public static AggregateType LookupAggregate(AggregateSymbol aggregate, AggregateType outer, TypeArray args)
	{
		s_aggregateTable.TryGetValue(MakeKey(aggregate, MakeKey(outer, args)), out var value);
		return value;
	}

	public static void InsertAggregate(AggregateSymbol aggregate, AggregateType outer, TypeArray args, AggregateType ats)
	{
		s_aggregateTable.Add(MakeKey(aggregate, MakeKey(outer, args)), ats);
	}

	public static ArrayType LookupArray(CType elementType, int rankNum)
	{
		s_arrayTable.TryGetValue(new KeyPair<CType, int>(elementType, rankNum), out var value);
		return value;
	}

	public static void InsertArray(CType elementType, int rankNum, ArrayType pArray)
	{
		s_arrayTable.Add(new KeyPair<CType, int>(elementType, rankNum), pArray);
	}

	public static ParameterModifierType LookupParameterModifier(CType elementType, bool isOut)
	{
		s_parameterModifierTable.TryGetValue(new KeyPair<CType, bool>(elementType, isOut), out var value);
		return value;
	}

	public static void InsertParameterModifier(CType elementType, bool isOut, ParameterModifierType parameterModifier)
	{
		s_parameterModifierTable.Add(new KeyPair<CType, bool>(elementType, isOut), parameterModifier);
	}

	public static PointerType LookupPointer(CType elementType)
	{
		s_pointerTable.TryGetValue(elementType, out var value);
		return value;
	}

	public static void InsertPointer(CType elementType, PointerType pointer)
	{
		s_pointerTable.Add(elementType, pointer);
	}

	public static NullableType LookupNullable(CType underlyingType)
	{
		s_nullableTable.TryGetValue(underlyingType, out var value);
		return value;
	}

	public static void InsertNullable(CType underlyingType, NullableType nullable)
	{
		s_nullableTable.Add(underlyingType, nullable);
	}
}
