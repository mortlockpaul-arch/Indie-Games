using Microsoft.CSharp.RuntimeBinder.Semantics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal class ErrArg
{
	public ErrArgKind eak;

	public ErrArgFlags eaf;

	internal int n;

	internal SYMKIND sk;

	internal Name name;

	internal Symbol sym;

	internal string psz;

	internal CType pType;

	internal MethPropWithInstMemo mpwiMemo;

	internal SymWithTypeMemo swtMemo;

	public ErrArg()
	{
	}

	public ErrArg(int n)
	{
		eak = ErrArgKind.Int;
		eaf = ErrArgFlags.None;
		this.n = n;
	}

	public ErrArg(Name name)
	{
		eak = ErrArgKind.Name;
		eaf = ErrArgFlags.None;
		this.name = name;
	}

	public ErrArg(string psz)
	{
		eak = ErrArgKind.Str;
		eaf = ErrArgFlags.None;
		this.psz = psz;
	}

	public ErrArg(CType pType)
		: this(pType, ErrArgFlags.None)
	{
	}

	public ErrArg(CType pType, ErrArgFlags eaf)
	{
		eak = ErrArgKind.Type;
		this.eaf = eaf;
		this.pType = pType;
	}

	public ErrArg(Symbol pSym)
		: this(pSym, ErrArgFlags.None)
	{
	}

	private ErrArg(Symbol pSym, ErrArgFlags eaf)
	{
		eak = ErrArgKind.Sym;
		this.eaf = eaf;
		sym = pSym;
	}

	public ErrArg(SymWithType swt)
	{
		eak = ErrArgKind.SymWithType;
		eaf = ErrArgFlags.None;
		swtMemo = new SymWithTypeMemo();
		swtMemo.sym = swt.Sym;
		swtMemo.ats = swt.Ats;
	}

	public ErrArg(MethPropWithInst mpwi)
	{
		eak = ErrArgKind.MethWithInst;
		eaf = ErrArgFlags.None;
		mpwiMemo = new MethPropWithInstMemo();
		mpwiMemo.sym = mpwi.Sym;
		mpwiMemo.ats = mpwi.Ats;
		mpwiMemo.typeArgs = mpwi.TypeArgs;
	}

	public static implicit operator ErrArg(int n)
	{
		return new ErrArg(n);
	}

	public static implicit operator ErrArg(CType type)
	{
		return new ErrArg(type);
	}

	public static implicit operator ErrArg(string psz)
	{
		return new ErrArg(psz);
	}

	public static implicit operator ErrArg(Name name)
	{
		return new ErrArg(name);
	}

	public static implicit operator ErrArg(Symbol pSym)
	{
		return new ErrArg(pSym);
	}

	public static implicit operator ErrArg(SymWithType swt)
	{
		return new ErrArg(swt);
	}

	public static implicit operator ErrArg(MethPropWithInst mpwi)
	{
		return new ErrArg(mpwi);
	}
}
