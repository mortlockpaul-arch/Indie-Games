using System;
using System.Collections.Generic;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class EXPRExtensions
{
	public static Expr Map(this Expr expr, Func<Expr, Expr> f)
	{
		if (expr == null)
		{
			return f(null);
		}
		Expr first = null;
		Expr last = null;
		foreach (Expr item in expr.ToEnumerable())
		{
			ExprFactory.AppendItemToList(f(item), ref first, ref last);
		}
		return first;
	}

	public static IEnumerable<Expr> ToEnumerable(this Expr expr)
	{
		Expr expr2 = expr;
		while (expr2 != null)
		{
			if (expr2 is ExprList list)
			{
				yield return list.OptionalElement;
				expr2 = list.OptionalNextListNode;
				continue;
			}
			yield return expr2;
			break;
		}
	}

	public static bool isLvalue(this Expr expr)
	{
		if (expr != null)
		{
			return (expr.Flags & EXPRFLAG.EXF_LVALUE) != 0;
		}
		return false;
	}

	public static bool isChecked(this Expr expr)
	{
		if (expr != null)
		{
			return (expr.Flags & EXPRFLAG.EXF_CHECKOVERFLOW) != 0;
		}
		return false;
	}

	public static bool isNull(this Expr expr)
	{
		if (expr is ExprConstant exprConstant && expr.Type.FundamentalType == FUNDTYPE.FT_REF)
		{
			return exprConstant.Val.IsNullRef;
		}
		return false;
	}

	public static bool IsZero(this Expr expr)
	{
		if (expr is ExprConstant exprConstant)
		{
			return exprConstant.IsZero;
		}
		return false;
	}

	private static Expr GetSeqVal(this Expr expr)
	{
		if (expr == null)
		{
			return null;
		}
		Expr expr2 = expr;
		while (expr2.Kind == ExpressionKind.Sequence)
		{
			expr2 = ((ExprBinOp)expr2).OptionalRightChild;
		}
		return expr2;
	}

	public static Expr GetConst(this Expr expr)
	{
		Expr seqVal = expr.GetSeqVal();
		switch (seqVal?.Kind)
		{
		case ExpressionKind.Constant:
		case ExpressionKind.ZeroInit:
			return seqVal;
		default:
			return null;
		}
	}
}
