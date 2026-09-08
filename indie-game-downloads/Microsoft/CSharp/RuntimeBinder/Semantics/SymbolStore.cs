using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class SymbolStore
{
	private readonly struct Key(Name name, ParentSymbol parent) : IEquatable<Key>
	{
		private readonly Name _name = name;

		private readonly ParentSymbol _parent = parent;

		public bool Equals(Key other)
		{
			if (_name == other._name)
			{
				return _parent == other._parent;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is Key other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode() ^ _parent.GetHashCode();
		}
	}

	private static readonly Dictionary<Key, Symbol> s_dictionary = new Dictionary<Key, Symbol>();

	public static Symbol LookupSym(Name name, ParentSymbol parent, symbmask_t kindmask)
	{
		if (!s_dictionary.TryGetValue(new Key(name, parent), out var value))
		{
			return null;
		}
		return FindCorrectKind(value, kindmask);
	}

	public static void InsertChild(ParentSymbol parent, Symbol child)
	{
		child.parent = parent;
		InsertChildNoGrow(child);
	}

	private static void InsertChildNoGrow(Symbol child)
	{
		SYMKIND kind = child.getKind();
		if (kind == SYMKIND.SK_LocalVariableSymbol || kind == SYMKIND.SK_Scope)
		{
			return;
		}
		if (s_dictionary.TryGetValue(new Key(child.name, child.parent), out var value))
		{
			while (value?.nextSameName != null)
			{
				value = value.nextSameName;
			}
			value.nextSameName = child;
		}
		else
		{
			s_dictionary.Add(new Key(child.name, child.parent), child);
		}
	}

	private static Symbol FindCorrectKind(Symbol sym, symbmask_t kindmask)
	{
		do
		{
			if ((kindmask & sym.mask()) != ~symbmask_t.MASK_ALL)
			{
				return sym;
			}
			sym = sym.nextSameName;
		}
		while (sym != null);
		return null;
	}
}
