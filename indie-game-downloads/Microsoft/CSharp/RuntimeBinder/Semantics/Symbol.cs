using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class Symbol
{
	private SYMKIND _kind;

	private ACCESS _access;

	public Name name;

	public ParentSymbol parent;

	public Symbol nextChild;

	public Symbol nextSameName;

	public bool isStatic
	{
		get
		{
			if (this is FieldSymbol fieldSymbol)
			{
				return fieldSymbol.isStatic;
			}
			if (this is EventSymbol eventSymbol)
			{
				return eventSymbol.isStatic;
			}
			if (this is MethodOrPropertySymbol methodOrPropertySymbol)
			{
				return methodOrPropertySymbol.isStatic;
			}
			return this is AggregateSymbol;
		}
	}

	public Symbol LookupNext(symbmask_t kindmask)
	{
		for (Symbol symbol = nextSameName; symbol != null; symbol = symbol.nextSameName)
		{
			if ((kindmask & symbol.mask()) != ~symbmask_t.MASK_ALL)
			{
				return symbol;
			}
		}
		return null;
	}

	public ACCESS GetAccess()
	{
		return _access;
	}

	public void SetAccess(ACCESS access)
	{
		_access = access;
	}

	public SYMKIND getKind()
	{
		return _kind;
	}

	public void setKind(SYMKIND kind)
	{
		_kind = kind;
	}

	public symbmask_t mask()
	{
		return (symbmask_t)(1 << (int)_kind);
	}

	public CType getType()
	{
		if (this is MethodOrPropertySymbol methodOrPropertySymbol)
		{
			return methodOrPropertySymbol.RetType;
		}
		if (this is FieldSymbol fieldSymbol)
		{
			return fieldSymbol.GetType();
		}
		if (this is EventSymbol eventSymbol)
		{
			return eventSymbol.type;
		}
		return null;
	}

	private Assembly GetAssembly()
	{
		switch (_kind)
		{
		case SYMKIND.SK_TypeParameterSymbol:
		case SYMKIND.SK_FieldSymbol:
		case SYMKIND.SK_MethodSymbol:
		case SYMKIND.SK_PropertySymbol:
		case SYMKIND.SK_EventSymbol:
			return ((AggregateSymbol)parent).AssociatedAssembly;
		case SYMKIND.SK_AggregateSymbol:
			return ((AggregateSymbol)this).AssociatedAssembly;
		default:
			return null;
		}
	}

	private bool InternalsVisibleTo(Assembly assembly)
	{
		switch (_kind)
		{
		case SYMKIND.SK_TypeParameterSymbol:
		case SYMKIND.SK_FieldSymbol:
		case SYMKIND.SK_MethodSymbol:
		case SYMKIND.SK_PropertySymbol:
		case SYMKIND.SK_EventSymbol:
			return ((AggregateSymbol)parent).InternalsVisibleTo(assembly);
		case SYMKIND.SK_AggregateSymbol:
			return ((AggregateSymbol)this).InternalsVisibleTo(assembly);
		default:
			return false;
		}
	}

	public bool SameAssemOrFriend(Symbol sym)
	{
		Assembly assembly = GetAssembly();
		if (!(assembly == sym.GetAssembly()))
		{
			return sym.InternalsVisibleTo(assembly);
		}
		return true;
	}

	public bool IsOverride()
	{
		switch (_kind)
		{
		case SYMKIND.SK_MethodSymbol:
		case SYMKIND.SK_PropertySymbol:
			return ((MethodOrPropertySymbol)this).isOverride;
		case SYMKIND.SK_EventSymbol:
			return ((EventSymbol)this).isOverride;
		default:
			return false;
		}
	}

	public bool IsHideByName()
	{
		switch (_kind)
		{
		case SYMKIND.SK_MethodSymbol:
		case SYMKIND.SK_PropertySymbol:
			return ((MethodOrPropertySymbol)this).isHideByName;
		case SYMKIND.SK_EventSymbol:
			return ((EventSymbol)this).methAdd?.isHideByName ?? false;
		default:
			return true;
		}
	}

	public bool isUserCallable()
	{
		if (this is MethodSymbol methodSymbol)
		{
			return methodSymbol.isUserCallable();
		}
		return true;
	}
}
