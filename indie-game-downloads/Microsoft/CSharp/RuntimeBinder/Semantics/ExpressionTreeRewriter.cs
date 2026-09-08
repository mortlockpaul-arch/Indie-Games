using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class ExpressionTreeRewriter : ExprVisitorBase
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public static ExprBinOp Rewrite(ExprBoundLambda expr)
	{
		return new ExpressionTreeRewriter().VisitBoundLambda(expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr Dispatch(Expr expr)
	{
		Expr expr2 = base.Dispatch(expr);
		if (expr2 == expr)
		{
			throw Error.InternalCompilerError();
		}
		return expr2;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitASSIGNMENT(ExprAssignment assignment)
	{
		Expr arg;
		if (assignment.LHS is ExprProperty exprProperty)
		{
			if (exprProperty.OptionalArguments == null)
			{
				arg = Visit(exprProperty);
			}
			else
			{
				Expr arg2 = Visit(exprProperty.MemberGroup.OptionalObject);
				Expr arg3 = ExprFactory.CreatePropertyInfo(exprProperty.PropWithTypeSlot.Prop(), exprProperty.PropWithTypeSlot.Ats);
				Expr arg4 = GenerateParamsArray(GenerateArgsList(exprProperty.OptionalArguments), PredefinedType.PT_EXPRESSION);
				arg = GenerateCall(PREDEFMETH.PM_EXPRESSION_PROPERTY, arg2, arg3, arg4);
			}
		}
		else
		{
			arg = Visit(assignment.LHS);
		}
		Expr arg5 = Visit(assignment.RHS);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_ASSIGN, arg, arg5);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitMULTIGET(ExprMultiGet pExpr)
	{
		return Visit(pExpr.OptionalMulti.Left);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitMULTI(ExprMulti pExpr)
	{
		Expr arg = Visit(pExpr.Operator);
		Expr arg2 = Visit(pExpr.Left);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_ASSIGN, arg2, arg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private ExprBinOp VisitBoundLambda(ExprBoundLambda anonmeth)
	{
		MethodSymbol preDefMethod = GetPreDefMethod(PREDEFMETH.PM_EXPRESSION_LAMBDA);
		AggregateType delegateType = anonmeth.DelegateType;
		TypeArray typeArgs = TypeArray.Allocate(delegateType);
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_EXPRESSION);
		MethWithInst methWithInst = new MethWithInst(preDefMethod, predefindType, typeArgs);
		Expr first = CreateWraps(anonmeth);
		Expr op = Visit(anonmeth.Expression);
		Expr op2 = GenerateParamsArray(null, PredefinedType.PT_PARAMETEREXPRESSION);
		Expr arguments = ExprFactory.CreateList(op, op2);
		CType type = TypeManager.SubstType(methWithInst.Meth().RetType, methWithInst.GetType(), methWithInst.TypeArgs);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, methWithInst);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, type, arguments, memberGroup, methWithInst);
		exprCall.PredefinedMethod = PREDEFMETH.PM_EXPRESSION_LAMBDA;
		return ExprFactory.CreateSequence(first, exprCall);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitCONSTANT(ExprConstant expr)
	{
		return GenerateConstant(expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitLOCAL(ExprLocal local)
	{
		return local.Local.wrap;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitFIELD(ExprField expr)
	{
		Expr arg = ((expr.OptionalObject != null) ? Visit(expr.OptionalObject) : ExprFactory.CreateNull());
		ExprFieldInfo arg2 = ExprFactory.CreateFieldInfo(expr.FieldWithType.Field(), expr.FieldWithType.GetType());
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_FIELD, arg, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitUSERDEFINEDCONVERSION(ExprUserDefinedConversion expr)
	{
		return GenerateUserDefinedConversion(expr, expr.Argument);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitCAST(ExprCast pExpr)
	{
		Expr argument = pExpr.Argument;
		if (argument.Type == pExpr.Type || SymbolLoader.IsBaseClassOfClass(argument.Type, pExpr.Type) || CConversions.FImpRefConv(argument.Type, pExpr.Type))
		{
			return Visit(argument);
		}
		if (pExpr.Type != null && pExpr.Type.IsPredefType(PredefinedType.PT_G_EXPRESSION) && argument is ExprBoundLambda)
		{
			return Visit(argument);
		}
		Expr expr = GenerateConversion(argument, pExpr.Type, pExpr.isChecked());
		if ((pExpr.Flags & EXPRFLAG.EXF_USERCALLABLE) != 0)
		{
			expr.Flags |= EXPRFLAG.EXF_USERCALLABLE;
		}
		return expr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitCONCAT(ExprConcat expr)
	{
		PREDEFMETH pdm = ((!expr.FirstArgument.Type.IsPredefType(PredefinedType.PT_STRING) || !expr.SecondArgument.Type.IsPredefType(PredefinedType.PT_STRING)) ? PREDEFMETH.PM_STRING_CONCAT_OBJECT_2 : PREDEFMETH.PM_STRING_CONCAT_STRING_2);
		Expr arg = Visit(expr.FirstArgument);
		Expr arg2 = Visit(expr.SecondArgument);
		Expr arg3 = ExprFactory.CreateMethodInfo(GetPreDefMethod(pdm), SymbolLoader.GetPredefindType(PredefinedType.PT_STRING), null);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED, arg, arg2, arg3);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitBINOP(ExprBinOp expr)
	{
		if (expr.UserDefinedCallMethod != null)
		{
			return GenerateUserDefinedBinaryOperator(expr);
		}
		return GenerateBuiltInBinaryOperator(expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitUNARYOP(ExprUnaryOp pExpr)
	{
		if (pExpr.UserDefinedCallMethod != null)
		{
			return GenerateUserDefinedUnaryOperator(pExpr);
		}
		return GenerateBuiltInUnaryOperator(pExpr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitARRAYINDEX(ExprArrayIndex pExpr)
	{
		Expr arg = Visit(pExpr.Array);
		Expr expr = GenerateIndexList(pExpr.Index);
		if (expr is ExprList)
		{
			Expr arg2 = GenerateParamsArray(expr, PredefinedType.PT_EXPRESSION);
			return GenerateCall(PREDEFMETH.PM_EXPRESSION_ARRAYINDEX2, arg, arg2);
		}
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_ARRAYINDEX, arg, expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitCALL(ExprCall expr)
	{
		switch (expr.NullableCallLiftKind)
		{
		case NullableCallLiftKind.NullableConversion:
		case NullableCallLiftKind.NullableConversionConstructor:
		case NullableCallLiftKind.NullableIntermediateConversion:
			return GenerateConversion(expr.OptionalArguments, expr.Type, expr.isChecked());
		case NullableCallLiftKind.UserDefinedConversion:
		case NullableCallLiftKind.NotLiftedIntermediateConversion:
			return GenerateUserDefinedConversion(expr.OptionalArguments, expr.Type, expr.MethWithInst);
		default:
		{
			if (expr.MethWithInst.Meth().IsConstructor())
			{
				return GenerateConstructor(expr);
			}
			if (expr.MemberGroup.IsDelegate)
			{
				return GenerateDelegateInvoke(expr);
			}
			Expr arg;
			if (expr.MethWithInst.Meth().isStatic || expr.MemberGroup.OptionalObject == null)
			{
				arg = ExprFactory.CreateNull();
			}
			else
			{
				arg = expr.MemberGroup.OptionalObject;
				if (arg != null && arg is ExprCast { IsBoxingCast: not false } exprCast)
				{
					arg = exprCast.Argument;
				}
				arg = Visit(arg);
			}
			Expr arg2 = ExprFactory.CreateMethodInfo(expr.MethWithInst);
			Expr arg3 = GenerateParamsArray(GenerateArgsList(expr.OptionalArguments), PredefinedType.PT_EXPRESSION);
			return GenerateCall(PREDEFMETH.PM_EXPRESSION_CALL, arg, arg2, arg3);
		}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitPROP(ExprProperty expr)
	{
		Expr arg = ((!expr.PropWithTypeSlot.Prop().isStatic && expr.MemberGroup.OptionalObject != null) ? Visit(expr.MemberGroup.OptionalObject) : ExprFactory.CreateNull());
		Expr arg2 = ExprFactory.CreatePropertyInfo(expr.PropWithTypeSlot.Prop(), expr.PropWithTypeSlot.GetType());
		if (expr.OptionalArguments != null)
		{
			Expr arg3 = GenerateParamsArray(GenerateArgsList(expr.OptionalArguments), PredefinedType.PT_EXPRESSION);
			return GenerateCall(PREDEFMETH.PM_EXPRESSION_PROPERTY, arg, arg2, arg3);
		}
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_PROPERTY, arg, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitARRINIT(ExprArrayInit expr)
	{
		Expr arg = CreateTypeOf(((ArrayType)expr.Type).ElementType);
		Expr arg2 = GenerateParamsArray(GenerateArgsList(expr.OptionalArguments), PredefinedType.PT_EXPRESSION);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_NEWARRAYINIT, arg, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitZEROINIT(ExprZeroInit expr)
	{
		return GenerateConstant(expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitTYPEOF(ExprTypeOf expr)
	{
		return GenerateConstant(expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateDelegateInvoke(ExprCall expr)
	{
		Expr optionalObject = expr.MemberGroup.OptionalObject;
		Expr arg = Visit(optionalObject);
		Expr arg2 = GenerateParamsArray(GenerateArgsList(expr.OptionalArguments), PredefinedType.PT_EXPRESSION);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_INVOKE, arg, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateBuiltInBinaryOperator(ExprBinOp expr)
	{
		int pdm = expr.Kind switch
		{
			ExpressionKind.LeftShirt => 35, 
			ExpressionKind.RightShift => 54, 
			ExpressionKind.BitwiseExclusiveOr => 27, 
			ExpressionKind.BitwiseOr => 49, 
			ExpressionKind.BitwiseAnd => 11, 
			ExpressionKind.LogicalAnd => 13, 
			ExpressionKind.LogicalOr => 51, 
			ExpressionKind.StringEq => 25, 
			ExpressionKind.Eq => 25, 
			ExpressionKind.StringNotEq => 47, 
			ExpressionKind.NotEq => 47, 
			ExpressionKind.GreaterThanOrEqual => 32, 
			ExpressionKind.LessThanOrEqual => 39, 
			ExpressionKind.LessThan => 37, 
			ExpressionKind.GreaterThan => 30, 
			ExpressionKind.Modulo => 41, 
			ExpressionKind.Divide => 23, 
			ExpressionKind.Multiply => expr.isChecked() ? 45 : 43, 
			ExpressionKind.Subtract => expr.isChecked() ? 58 : 56, 
			ExpressionKind.Add => expr.isChecked() ? 9 : 7, 
			_ => throw Error.InternalCompilerError(), 
		};
		Expr optionalLeftChild = expr.OptionalLeftChild;
		Expr optionalRightChild = expr.OptionalRightChild;
		CType cType = optionalLeftChild.Type;
		CType cType2 = optionalRightChild.Type;
		Expr arg = Visit(optionalLeftChild);
		Expr expr2 = Visit(optionalRightChild);
		bool flag = false;
		CType cType3 = null;
		CType cType4 = null;
		if (cType.IsEnumType)
		{
			cType3 = TypeManager.GetNullable(cType.UnderlyingEnumType);
			cType = cType3;
			flag = true;
		}
		else if (cType is NullableType nullableType && nullableType.UnderlyingType.IsEnumType)
		{
			cType3 = TypeManager.GetNullable(nullableType.UnderlyingType.UnderlyingEnumType);
			cType = cType3;
			flag = true;
		}
		if (cType2.IsEnumType)
		{
			cType4 = TypeManager.GetNullable(cType2.UnderlyingEnumType);
			cType2 = cType4;
			flag = true;
		}
		else if (cType2 is NullableType nullableType2 && nullableType2.UnderlyingType.IsEnumType)
		{
			cType4 = TypeManager.GetNullable(nullableType2.UnderlyingType.UnderlyingEnumType);
			cType2 = cType4;
			flag = true;
		}
		if (cType is NullableType nullableType3 && nullableType3.UnderlyingType == cType2)
		{
			cType4 = cType;
		}
		if (cType2 is NullableType nullableType4 && nullableType4.UnderlyingType == cType)
		{
			cType3 = cType2;
		}
		if (cType3 != null)
		{
			arg = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, arg, CreateTypeOf(cType3));
		}
		if (cType4 != null)
		{
			expr2 = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, expr2, CreateTypeOf(cType4));
		}
		Expr expr3 = GenerateCall((PREDEFMETH)pdm, arg, expr2);
		if (flag && expr.Type.StripNubs().IsEnumType)
		{
			expr3 = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, expr3, CreateTypeOf(expr.Type));
		}
		return expr3;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateBuiltInUnaryOperator(ExprUnaryOp expr)
	{
		PREDEFMETH pdm;
		switch (expr.Kind)
		{
		case ExpressionKind.UnaryPlus:
			return Visit(expr.Child);
		case ExpressionKind.BitwiseNot:
			pdm = PREDEFMETH.PM_EXPRESSION_NOT;
			break;
		case ExpressionKind.LogicalNot:
			pdm = PREDEFMETH.PM_EXPRESSION_NOT;
			break;
		case ExpressionKind.Negate:
			pdm = (expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_NEGATECHECKED : PREDEFMETH.PM_EXPRESSION_NEGATE);
			break;
		default:
			throw Error.InternalCompilerError();
		}
		Expr child = expr.Child;
		return GenerateCall(pdm, Visit(child));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateUserDefinedBinaryOperator(ExprBinOp expr)
	{
		PREDEFMETH pdm;
		switch (expr.Kind)
		{
		case ExpressionKind.LogicalOr:
			pdm = PREDEFMETH.PM_EXPRESSION_ORELSE_USER_DEFINED;
			break;
		case ExpressionKind.LogicalAnd:
			pdm = PREDEFMETH.PM_EXPRESSION_ANDALSO_USER_DEFINED;
			break;
		case ExpressionKind.LeftShirt:
			pdm = PREDEFMETH.PM_EXPRESSION_LEFTSHIFT_USER_DEFINED;
			break;
		case ExpressionKind.RightShift:
			pdm = PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT_USER_DEFINED;
			break;
		case ExpressionKind.BitwiseExclusiveOr:
			pdm = PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR_USER_DEFINED;
			break;
		case ExpressionKind.BitwiseOr:
			pdm = PREDEFMETH.PM_EXPRESSION_OR_USER_DEFINED;
			break;
		case ExpressionKind.BitwiseAnd:
			pdm = PREDEFMETH.PM_EXPRESSION_AND_USER_DEFINED;
			break;
		case ExpressionKind.Modulo:
			pdm = PREDEFMETH.PM_EXPRESSION_MODULO_USER_DEFINED;
			break;
		case ExpressionKind.Divide:
			pdm = PREDEFMETH.PM_EXPRESSION_DIVIDE_USER_DEFINED;
			break;
		case ExpressionKind.Eq:
		case ExpressionKind.NotEq:
		case ExpressionKind.LessThan:
		case ExpressionKind.LessThanOrEqual:
		case ExpressionKind.GreaterThan:
		case ExpressionKind.GreaterThanOrEqual:
		case ExpressionKind.StringEq:
		case ExpressionKind.StringNotEq:
		case ExpressionKind.DelegateEq:
		case ExpressionKind.DelegateNotEq:
			return GenerateUserDefinedComparisonOperator(expr);
		case ExpressionKind.Subtract:
		case ExpressionKind.DelegateSubtract:
			pdm = (expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_SUBTRACT_USER_DEFINED);
			break;
		case ExpressionKind.Add:
		case ExpressionKind.DelegateAdd:
			pdm = (expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_ADDCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED);
			break;
		case ExpressionKind.Multiply:
			pdm = (expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_MULTIPLY_USER_DEFINED);
			break;
		default:
			throw Error.InternalCompilerError();
		}
		Expr pExpr = expr.OptionalLeftChild;
		Expr pExpr2 = expr.OptionalRightChild;
		Expr optionalUserDefinedCall = expr.OptionalUserDefinedCall;
		if (optionalUserDefinedCall != null)
		{
			if (optionalUserDefinedCall is ExprCall exprCall)
			{
				ExprList obj = (ExprList)exprCall.OptionalArguments;
				pExpr = obj.OptionalElement;
				pExpr2 = obj.OptionalNextListNode;
			}
			else
			{
				ExprList obj2 = (ExprList)(optionalUserDefinedCall as ExprUserLogicalOp).OperatorCall.OptionalArguments;
				pExpr = ((ExprWrap)obj2.OptionalElement).OptionalExpression;
				pExpr2 = obj2.OptionalNextListNode;
			}
		}
		pExpr = Visit(pExpr);
		pExpr2 = Visit(pExpr2);
		FixLiftedUserDefinedBinaryOperators(expr, ref pExpr, ref pExpr2);
		Expr arg = ExprFactory.CreateMethodInfo(expr.UserDefinedCallMethod);
		Expr expr2 = GenerateCall(pdm, pExpr, pExpr2, arg);
		if (expr.Kind == ExpressionKind.DelegateSubtract || expr.Kind == ExpressionKind.DelegateAdd)
		{
			Expr arg2 = CreateTypeOf(expr.Type);
			return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, expr2, arg2);
		}
		return expr2;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateUserDefinedUnaryOperator(ExprUnaryOp expr)
	{
		Expr pExpr = expr.Child;
		ExprCall exprCall = (ExprCall)expr.OptionalUserDefinedCall;
		if (exprCall != null)
		{
			pExpr = exprCall.OptionalArguments;
		}
		PREDEFMETH pdm;
		switch (expr.Kind)
		{
		case ExpressionKind.True:
		case ExpressionKind.False:
			return Visit(exprCall);
		case ExpressionKind.UnaryPlus:
			pdm = PREDEFMETH.PM_EXPRESSION_UNARYPLUS_USER_DEFINED;
			break;
		case ExpressionKind.BitwiseNot:
			pdm = PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED;
			break;
		case ExpressionKind.LogicalNot:
			pdm = PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED;
			break;
		case ExpressionKind.Negate:
		case ExpressionKind.DecimalNegate:
			pdm = (expr.isChecked() ? PREDEFMETH.PM_EXPRESSION_NEGATECHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_NEGATE_USER_DEFINED);
			break;
		case ExpressionKind.Inc:
		case ExpressionKind.Dec:
		case ExpressionKind.DecimalInc:
		case ExpressionKind.DecimalDec:
			pdm = PREDEFMETH.PM_EXPRESSION_CALL;
			break;
		default:
			throw Error.InternalCompilerError();
		}
		Expr expr2 = Visit(pExpr);
		Expr arg = ExprFactory.CreateMethodInfo(expr.UserDefinedCallMethod);
		if (expr.Kind == ExpressionKind.Inc || expr.Kind == ExpressionKind.Dec || expr.Kind == ExpressionKind.DecimalInc || expr.Kind == ExpressionKind.DecimalDec)
		{
			return GenerateCall(pdm, null, arg, GenerateParamsArray(expr2, PredefinedType.PT_EXPRESSION));
		}
		return GenerateCall(pdm, expr2, arg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateUserDefinedComparisonOperator(ExprBinOp expr)
	{
		int pdm = expr.Kind switch
		{
			ExpressionKind.StringEq => 26, 
			ExpressionKind.StringNotEq => 48, 
			ExpressionKind.DelegateEq => 26, 
			ExpressionKind.DelegateNotEq => 48, 
			ExpressionKind.Eq => 26, 
			ExpressionKind.NotEq => 48, 
			ExpressionKind.LessThanOrEqual => 40, 
			ExpressionKind.LessThan => 38, 
			ExpressionKind.GreaterThanOrEqual => 33, 
			ExpressionKind.GreaterThan => 31, 
			_ => throw Error.InternalCompilerError(), 
		};
		Expr pExpr = expr.OptionalLeftChild;
		Expr pExpr2 = expr.OptionalRightChild;
		if (expr.OptionalUserDefinedCall != null)
		{
			ExprList obj = (ExprList)((ExprCall)expr.OptionalUserDefinedCall).OptionalArguments;
			pExpr = obj.OptionalElement;
			pExpr2 = obj.OptionalNextListNode;
		}
		pExpr = Visit(pExpr);
		pExpr2 = Visit(pExpr2);
		FixLiftedUserDefinedBinaryOperators(expr, ref pExpr, ref pExpr2);
		return GenerateCall(arg3: ExprFactory.CreateBoolConstant(b: false), arg4: ExprFactory.CreateMethodInfo(expr.UserDefinedCallMethod), pdm: (PREDEFMETH)pdm, arg1: pExpr, arg2: pExpr2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateConversion(Expr arg, CType CType, bool bChecked)
	{
		return GenerateConversionWithSource(Visit(arg), CType, bChecked || arg.isChecked());
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static Expr GenerateConversionWithSource(Expr pTarget, CType pType, bool bChecked)
	{
		int pdm = (bChecked ? 21 : 19);
		Expr arg = CreateTypeOf(pType);
		return GenerateCall((PREDEFMETH)pdm, pTarget, arg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateValueAccessConversion(Expr pArgument)
	{
		Expr arg = CreateTypeOf(pArgument.Type.StripNubs());
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, Visit(pArgument), arg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateUserDefinedConversion(Expr arg, CType type, MethWithInst method)
	{
		Expr target = Visit(arg);
		return GenerateUserDefinedConversion(arg, type, target, method);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static Expr GenerateUserDefinedConversion(Expr arg, CType CType, Expr target, MethWithInst method)
	{
		if (isEnumToDecimalConversion(arg.Type, CType))
		{
			Expr arg2 = CreateTypeOf(TypeManager.GetNullable(arg.Type.StripNubs().UnderlyingEnumType));
			target = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, target, arg2);
		}
		CType cType = TypeManager.SubstType(method.Meth().RetType, method.GetType(), method.TypeArgs);
		int num;
		CType type;
		if (cType != CType)
		{
			if (IsNullableValueType(arg.Type))
			{
				num = (IsNullableValueType(CType) ? 1 : 0);
				if (num != 0)
				{
					goto IL_0076;
				}
			}
			else
			{
				num = 0;
			}
			type = cType;
			goto IL_0077;
		}
		num = 1;
		goto IL_0076;
		IL_0077:
		Expr expr = GenerateCall(arg2: CreateTypeOf(type), arg3: ExprFactory.CreateMethodInfo(method), pdm: arg.isChecked() ? PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED_USER_DEFINED : PREDEFMETH.PM_EXPRESSION_CONVERT_USER_DEFINED, arg1: target);
		if (num != 0)
		{
			return expr;
		}
		int pdm = (arg.isChecked() ? 21 : 19);
		Expr arg3 = CreateTypeOf(CType);
		return GenerateCall((PREDEFMETH)pdm, expr, arg3);
		IL_0076:
		type = CType;
		goto IL_0077;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateUserDefinedConversion(ExprUserDefinedConversion pExpr, Expr pArgument)
	{
		Expr userDefinedCall = pExpr.UserDefinedCall;
		Expr argument = pExpr.Argument;
		Expr target;
		if (!isEnumToDecimalConversion(pArgument.Type, pExpr.Type) && IsNullableValueAccess(argument, pArgument))
		{
			target = GenerateValueAccessConversion(pArgument);
		}
		else
		{
			ExprCall exprCall = userDefinedCall as ExprCall;
			Expr expr = exprCall?.PConversions;
			if (expr != null)
			{
				if (expr is ExprCall { OptionalArguments: var optionalArguments })
				{
					target = ((!IsNullableValueAccess(optionalArguments, pArgument)) ? Visit(optionalArguments) : GenerateValueAccessConversion(pArgument));
					return GenerateConversionWithSource(target, userDefinedCall.Type, exprCall.isChecked());
				}
				return GenerateUserDefinedConversion((ExprUserDefinedConversion)expr, pArgument);
			}
			target = Visit(argument);
		}
		return GenerateUserDefinedConversion(argument, pExpr.Type, target, pExpr.UserDefinedCallMethod);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static Expr GenerateParameter(string name, CType CType)
	{
		SymbolLoader.GetPredefindType(PredefinedType.PT_STRING);
		ExprConstant arg = ExprFactory.CreateStringConstant(name);
		ExprTypeOf arg2 = CreateTypeOf(CType);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_PARAMETER, arg2, arg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static MethodSymbol GetPreDefMethod(PREDEFMETH pdm)
	{
		return PredefinedMembers.GetMethod(pdm);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ExprTypeOf CreateTypeOf(CType type)
	{
		return ExprFactory.CreateTypeOf(type);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static Expr CreateWraps(ExprBoundLambda anonmeth)
	{
		Expr expr = null;
		for (Symbol symbol = anonmeth.ArgumentScope.firstChild; symbol != null; symbol = symbol.nextChild)
		{
			if (symbol is LocalVariableSymbol localVariableSymbol)
			{
				Expr expression = GenerateParameter(localVariableSymbol.name.Text, localVariableSymbol.GetType());
				localVariableSymbol.wrap = ExprFactory.CreateWrap(expression);
				Expr expr2 = ExprFactory.CreateSave(localVariableSymbol.wrap);
				expr = ((expr != null) ? ExprFactory.CreateSequence(expr, expr2) : expr2);
			}
		}
		return expr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateConstructor(ExprCall expr)
	{
		Expr arg = ExprFactory.CreateMethodInfo(expr.MethWithInst);
		Expr arg2 = GenerateParamsArray(GenerateArgsList(expr.OptionalArguments), PredefinedType.PT_EXPRESSION);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_NEW, arg, arg2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateArgsList(Expr oldArgs)
	{
		Expr first = null;
		Expr last = first;
		ExpressionIterator expressionIterator = new ExpressionIterator(oldArgs);
		while (!expressionIterator.AtEnd())
		{
			Expr pExpr = expressionIterator.Current();
			ExprFactory.AppendItemToList(Visit(pExpr), ref first, ref last);
			expressionIterator.MoveNext();
		}
		return first;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateIndexList(Expr oldIndices)
	{
		CType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_INT);
		Expr first = null;
		Expr last = first;
		ExpressionIterator expressionIterator = new ExpressionIterator(oldIndices);
		while (!expressionIterator.AtEnd())
		{
			Expr expr = expressionIterator.Current();
			if (expr.Type != predefindType)
			{
				expr = ExprFactory.CreateCast(EXPRFLAG.EXF_LITERALCONST, predefindType, expr);
				expr.Flags |= EXPRFLAG.EXF_CHECKOVERFLOW;
			}
			ExprFactory.AppendItemToList(Visit(expr), ref first, ref last);
			expressionIterator.MoveNext();
		}
		return first;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static Expr GenerateConstant(Expr expr)
	{
		EXPRFLAG flags = (EXPRFLAG)0;
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_OBJECT);
		if (expr.Type is NullType)
		{
			ExprTypeOf arg = CreateTypeOf(predefindType);
			return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONSTANT_OBJECT_TYPE, expr, arg);
		}
		AggregateType predefindType2 = SymbolLoader.GetPredefindType(PredefinedType.PT_STRING);
		if (expr.Type != predefindType2)
		{
			flags = EXPRFLAG.EXF_CTOR;
		}
		ExprCast arg2 = ExprFactory.CreateCast(flags, predefindType, expr);
		ExprTypeOf arg3 = CreateTypeOf(expr.Type);
		return GenerateCall(PREDEFMETH.PM_EXPRESSION_CONSTANT_OBJECT_TYPE, arg2, arg3);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1)
	{
		MethodSymbol preDefMethod = GetPreDefMethod(pdm);
		if (preDefMethod == null)
		{
			return null;
		}
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_EXPRESSION);
		MethWithInst methWithInst = new MethWithInst(preDefMethod, predefindType);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, methWithInst);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, methWithInst.Meth().RetType, arg1, memberGroup, methWithInst);
		exprCall.PredefinedMethod = pdm;
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1, Expr arg2)
	{
		MethodSymbol preDefMethod = GetPreDefMethod(pdm);
		if (preDefMethod == null)
		{
			return null;
		}
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_EXPRESSION);
		Expr arguments = ExprFactory.CreateList(arg1, arg2);
		MethWithInst methWithInst = new MethWithInst(preDefMethod, predefindType);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, methWithInst);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, methWithInst.Meth().RetType, arguments, memberGroup, methWithInst);
		exprCall.PredefinedMethod = pdm;
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1, Expr arg2, Expr arg3)
	{
		MethodSymbol preDefMethod = GetPreDefMethod(pdm);
		if (preDefMethod == null)
		{
			return null;
		}
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_EXPRESSION);
		Expr arguments = ExprFactory.CreateList(arg1, arg2, arg3);
		MethWithInst methWithInst = new MethWithInst(preDefMethod, predefindType);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, methWithInst);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, methWithInst.Meth().RetType, arguments, memberGroup, methWithInst);
		exprCall.PredefinedMethod = pdm;
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ExprCall GenerateCall(PREDEFMETH pdm, Expr arg1, Expr arg2, Expr arg3, Expr arg4)
	{
		MethodSymbol preDefMethod = GetPreDefMethod(pdm);
		if (preDefMethod == null)
		{
			return null;
		}
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_EXPRESSION);
		Expr arguments = ExprFactory.CreateList(arg1, arg2, arg3, arg4);
		MethWithInst methWithInst = new MethWithInst(preDefMethod, predefindType);
		ExprMemberGroup memberGroup = ExprFactory.CreateMemGroup(null, methWithInst);
		ExprCall exprCall = ExprFactory.CreateCall((EXPRFLAG)0, methWithInst.Meth().RetType, arguments, memberGroup, methWithInst);
		exprCall.PredefinedMethod = pdm;
		return exprCall;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static ExprArrayInit GenerateParamsArray(Expr args, PredefinedType pt)
	{
		int num = ExpressionIterator.Count(args);
		ArrayType array = TypeManager.GetArray(SymbolLoader.GetPredefindType(pt), 1, isSZArray: true);
		ExprConstant argumentDimensions = ExprFactory.CreateIntegerConstant(num);
		return ExprFactory.CreateArrayInit(array, args, argumentDimensions, new int[1] { num });
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static void FixLiftedUserDefinedBinaryOperators(ExprBinOp expr, ref Expr pp1, ref Expr pp2)
	{
		MethodSymbol methodSymbol = expr.UserDefinedCallMethod.Meth();
		Expr optionalLeftChild = expr.OptionalLeftChild;
		Expr optionalRightChild = expr.OptionalRightChild;
		Expr expr2 = pp1;
		Expr expr3 = pp2;
		CType cType = methodSymbol.Params[0];
		CType cType2 = methodSymbol.Params[1];
		CType type = optionalLeftChild.Type;
		CType type2 = optionalRightChild.Type;
		if (cType is AggregateType aggregateType && aggregateType.OwningAggregate.IsValueType() && cType2 is AggregateType aggregateType2 && aggregateType2.OwningAggregate.IsValueType())
		{
			CType nullable = TypeManager.GetNullable(cType);
			CType nullable2 = TypeManager.GetNullable(cType2);
			if (type is NullType || (type == cType && (type2 == nullable2 || type2 is NullType)))
			{
				expr2 = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, expr2, CreateTypeOf(nullable));
			}
			if (type2 is NullType || (type2 == cType2 && (type == nullable || type is NullType)))
			{
				expr3 = GenerateCall(PREDEFMETH.PM_EXPRESSION_CONVERT, expr3, CreateTypeOf(nullable2));
			}
			pp1 = expr2;
			pp2 = expr3;
		}
	}

	private static bool IsNullableValueType(CType pType)
	{
		if (pType is NullableType && pType.StripNubs() is AggregateType aggregateType)
		{
			return aggregateType.OwningAggregate.IsValueType();
		}
		return false;
	}

	private static bool IsNullableValueAccess(Expr pExpr, Expr pObject)
	{
		if (pExpr is ExprProperty exprProperty && exprProperty.MemberGroup.OptionalObject == pObject)
		{
			return pObject.Type is NullableType;
		}
		return false;
	}

	private static bool isEnumToDecimalConversion(CType argtype, CType desttype)
	{
		if (argtype.StripNubs().IsEnumType)
		{
			return desttype.StripNubs().IsPredefType(PredefinedType.PT_DECIMAL);
		}
		return false;
	}
}
