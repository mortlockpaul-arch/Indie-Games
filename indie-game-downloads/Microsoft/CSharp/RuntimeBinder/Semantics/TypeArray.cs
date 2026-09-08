using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class TypeArray
{
	private readonly struct TypeArrayKey : IEquatable<TypeArrayKey>
	{
		private readonly CType[] _types;

		private readonly int _hashCode;

		public TypeArrayKey(CType[] types)
		{
			_types = types;
			int num = 371857150;
			foreach (CType cType in types)
			{
				num = (num << 5) - num;
				if (cType != null)
				{
					num ^= cType.GetHashCode();
				}
			}
			_hashCode = num;
		}

		public bool Equals(TypeArrayKey other)
		{
			CType[] types = _types;
			CType[] types2 = other._types;
			if (types2 == types)
			{
				return true;
			}
			if (other._hashCode != _hashCode || types2.Length != types.Length)
			{
				return false;
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] != types2[i])
				{
					return false;
				}
			}
			return true;
		}

		[ExcludeFromCodeCoverage(Justification = "Typed overload should always be the method called")]
		public override bool Equals(object obj)
		{
			if (obj is TypeArrayKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}
	}

	private static readonly Dictionary<TypeArrayKey, TypeArray> s_tableTypeArrays = new Dictionary<TypeArrayKey, TypeArray>();

	public static readonly TypeArray Empty = new TypeArray(Array.Empty<CType>());

	public int Count => Items.Length;

	public CType[] Items { get; }

	public CType this[int i] => Items[i];

	private TypeArray(CType[] types)
	{
		Items = types;
	}

	public void CopyItems(int i, int c, CType[] dest)
	{
		Array.Copy(Items, i, dest, 0, c);
	}

	public static TypeArray Allocate(int ctype, TypeArray array, int offset)
	{
		if (ctype == 0)
		{
			return Empty;
		}
		if (ctype == array.Count)
		{
			return array;
		}
		CType[] items = array.Items;
		CType[] array2 = new CType[ctype];
		Array.ConstrainedCopy(items, offset, array2, 0, ctype);
		return Allocate(array2);
	}

	public static TypeArray Allocate(params CType[] types)
	{
		if (types != null && types.Length != 0)
		{
			TypeArrayKey key = new TypeArrayKey(types);
			if (!s_tableTypeArrays.TryGetValue(key, out var value))
			{
				value = new TypeArray(types);
				s_tableTypeArrays.Add(key, value);
			}
			return value;
		}
		return Empty;
	}

	public static TypeArray Concat(TypeArray pta1, TypeArray pta2)
	{
		CType[] items = pta1.Items;
		if (items.Length == 0)
		{
			return pta2;
		}
		CType[] items2 = pta2.Items;
		if (items2.Length == 0)
		{
			return pta1;
		}
		CType[] array = new CType[items.Length + items2.Length];
		Array.Copy(items, array, items.Length);
		Array.Copy(items2, 0, array, items.Length, items2.Length);
		return Allocate(array);
	}
}
