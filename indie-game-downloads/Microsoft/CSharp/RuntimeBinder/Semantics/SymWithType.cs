using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal class SymWithType
{
	private AggregateType _ats;

	private Symbol _sym;

	public AggregateType Ats => _ats;

	public Symbol Sym => _sym;

	public SymWithType()
	{
	}

	public SymWithType(Symbol sym, AggregateType ats)
	{
		Set(sym, ats);
	}

	public virtual void Clear()
	{
		_sym = null;
		_ats = null;
	}

	public new AggregateType GetType()
	{
		return Ats;
	}

	public static bool operator ==(SymWithType swt1, SymWithType swt2)
	{
		if ((object)swt1 == swt2)
		{
			return true;
		}
		if ((object)swt1 == null)
		{
			return swt2._sym == null;
		}
		if ((object)swt2 == null)
		{
			return swt1._sym == null;
		}
		if (swt1.Sym == swt2.Sym)
		{
			return swt1.Ats == swt2.Ats;
		}
		return false;
	}

	public static bool operator !=(SymWithType swt1, SymWithType swt2)
	{
		return !(swt1 == swt2);
	}

	[ExcludeFromCodeCoverage(Justification = "== overload should always be the method called")]
	public override bool Equals(object obj)
	{
		SymWithType symWithType = obj as SymWithType;
		if (symWithType == null)
		{
			return false;
		}
		if (Sym == symWithType.Sym)
		{
			return Ats == symWithType.Ats;
		}
		return false;
	}

	[ExcludeFromCodeCoverage(Justification = "Never used as a key")]
	public override int GetHashCode()
	{
		return (Sym?.GetHashCode() ?? 0) + (Ats?.GetHashCode() ?? 0);
	}

	public static implicit operator bool(SymWithType swt)
	{
		return swt != null;
	}

	public MethodOrPropertySymbol MethProp()
	{
		return Sym as MethodOrPropertySymbol;
	}

	public MethodSymbol Meth()
	{
		return Sym as MethodSymbol;
	}

	public PropertySymbol Prop()
	{
		return Sym as PropertySymbol;
	}

	public FieldSymbol Field()
	{
		return Sym as FieldSymbol;
	}

	public EventSymbol Event()
	{
		return Sym as EventSymbol;
	}

	public void Set(Symbol sym, AggregateType ats)
	{
		if (sym == null)
		{
			ats = null;
		}
		_sym = sym;
		_ats = ats;
	}
}
