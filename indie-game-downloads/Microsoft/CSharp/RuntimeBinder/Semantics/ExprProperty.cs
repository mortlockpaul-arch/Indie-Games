using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprProperty : ExprWithArgs
{
	public Expr OptionalObjectThrough { get; }

	public PropWithType PropWithTypeSlot { get; }

	public MethWithType MethWithTypeSet { get; }

	public ExprProperty(CType type, Expr pOptionalObjectThrough, Expr pOptionalArguments, ExprMemberGroup pMemberGroup, PropWithType pwtSlot, MethWithType mwtSet)
		: base(ExpressionKind.Property, type)
	{
		OptionalObjectThrough = pOptionalObjectThrough;
		base.OptionalArguments = pOptionalArguments;
		base.MemberGroup = pMemberGroup;
		if (pwtSlot != null)
		{
			PropWithTypeSlot = pwtSlot;
		}
		if (mwtSet != null)
		{
			MethWithTypeSet = mwtSet;
			if (!HasIsExternalInitModifier(mwtSet))
			{
				base.Flags = EXPRFLAG.EXF_LVALUE;
			}
		}
	}

	public override SymWithType GetSymWithType()
	{
		return PropWithTypeSlot;
	}

	internal static bool HasIsExternalInitModifier(MethWithType mwtSet)
	{
		return ((mwtSet.Meth()?.AssociatedMemberInfo as MethodInfo)?.ReturnParameter.GetRequiredCustomModifiers())?.Any((Type type) => type.Name == "IsExternalInit" && !type.IsNested && type.Namespace == "System.Runtime.CompilerServices") ?? false;
	}
}
