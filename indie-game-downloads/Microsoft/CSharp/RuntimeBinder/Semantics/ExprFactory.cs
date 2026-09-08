using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class ExprFactory
{
	public static ExprCall CreateCall(EXPRFLAG flags, CType type, Expr arguments, ExprMemberGroup memberGroup, MethWithInst method)
	{
		return new ExprCall(type, flags, arguments, memberGroup, method);
	}

	public static ExprField CreateField(CType type, Expr optionalObject, FieldWithType field)
	{
		return new ExprField(type, optionalObject, field);
	}

	public static ExprArrayInit CreateArrayInit(CType type, Expr arguments, Expr argumentDimensions, int[] dimSizes)
	{
		return new ExprArrayInit(type, arguments, argumentDimensions, dimSizes);
	}

	public static ExprProperty CreateProperty(CType type, Expr optionalObjectThrough, Expr arguments, ExprMemberGroup memberGroup, PropWithType property, MethWithType setMethod)
	{
		return new ExprProperty(type, optionalObjectThrough, arguments, memberGroup, property, setMethod);
	}

	public static ExprMemberGroup CreateMemGroup(EXPRFLAG flags, Name name, TypeArray typeArgs, SYMKIND symKind, CType parentType, Expr obj, CMemberLookupResults memberLookupResults)
	{
		return new ExprMemberGroup(flags, name, typeArgs, symKind, parentType, obj, memberLookupResults);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprMemberGroup CreateMemGroup(Expr obj, MethPropWithInst method)
	{
		Name name = method.Sym?.name;
		return CreateMemGroup((EXPRFLAG)0, name, method.TypeArgs, method.MethProp()?.getKind() ?? SYMKIND.SK_MethodSymbol, method.GetType(), obj, new CMemberLookupResults(TypeArray.Allocate(method.GetType()), name));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprUserDefinedConversion CreateUserDefinedConversion(Expr arg, Expr call, MethWithInst method)
	{
		return new ExprUserDefinedConversion(arg, call, method);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprCast CreateCast(CType type, Expr argument)
	{
		return CreateCast((EXPRFLAG)0, type, argument);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprCast CreateCast(EXPRFLAG flags, CType type, Expr argument)
	{
		return new ExprCast(flags, type, argument);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprLocal CreateLocal(LocalVariableSymbol local)
	{
		return new ExprLocal(local);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprBoundLambda CreateAnonymousMethod(AggregateType delegateType, Scope argumentScope, Expr expression)
	{
		return new ExprBoundLambda(delegateType, argumentScope, expression);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprMethodInfo CreateMethodInfo(MethPropWithInst mwi)
	{
		return CreateMethodInfo(mwi.Meth(), mwi.GetType(), mwi.TypeArgs);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprMethodInfo CreateMethodInfo(MethodSymbol method, AggregateType methodType, TypeArray methodParameters)
	{
		return new ExprMethodInfo(TypeManager.GetPredefAgg(method.IsConstructor() ? PredefinedType.PT_CONSTRUCTORINFO : PredefinedType.PT_METHODINFO).getThisType(), method, methodType, methodParameters);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprPropertyInfo CreatePropertyInfo(PropertySymbol prop, AggregateType propertyType)
	{
		return new ExprPropertyInfo(TypeManager.GetPredefAgg(PredefinedType.PT_PROPERTYINFO).getThisType(), prop, propertyType);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprFieldInfo CreateFieldInfo(FieldSymbol field, AggregateType fieldType)
	{
		return new ExprFieldInfo(field, fieldType, TypeManager.GetPredefAgg(PredefinedType.PT_FIELDINFO).getThisType());
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprTypeOf CreateTypeOf(CType sourceType)
	{
		return new ExprTypeOf(TypeManager.GetPredefAgg(PredefinedType.PT_TYPE).getThisType(), sourceType);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprUserLogicalOp CreateUserLogOp(CType type, Expr trueFalseCall, ExprCall operatorCall)
	{
		return new ExprUserLogicalOp(type, trueFalseCall, operatorCall);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprConcat CreateConcat(Expr first, Expr second)
	{
		return new ExprConcat(first, second);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprConstant CreateStringConstant(string str)
	{
		return CreateConstant(TypeManager.GetPredefAgg(PredefinedType.PT_STRING).getThisType(), ConstVal.Get(str));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprMultiGet CreateMultiGet(EXPRFLAG flags, CType type, ExprMulti multi)
	{
		return new ExprMultiGet(type, flags, multi);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprMulti CreateMulti(EXPRFLAG flags, CType type, Expr left, Expr op)
	{
		return new ExprMulti(type, flags, left, op);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static Expr CreateZeroInit(CType type)
	{
		if (type.IsEnumType)
		{
			return CreateConstant(type, ConstVal.Get(Activator.CreateInstance(type.AssociatedSystemType)));
		}
		switch (type.FundamentalType)
		{
		case FUNDTYPE.FT_PTR:
			return CreateCast((EXPRFLAG)0, type, CreateNull());
		case FUNDTYPE.FT_STRUCT:
			if (type.IsPredefType(PredefinedType.PT_DECIMAL))
			{
				break;
			}
			goto case FUNDTYPE.FT_VAR;
		case FUNDTYPE.FT_VAR:
			return new ExprZeroInit(type);
		}
		return CreateConstant(type, ConstVal.GetDefaultValue(type.ConstValKind));
	}

	public static ExprConstant CreateConstant(CType type, ConstVal constVal)
	{
		return new ExprConstant(type, constVal);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprConstant CreateIntegerConstant(int x)
	{
		return CreateConstant(TypeManager.GetPredefAgg(PredefinedType.PT_INT).getThisType(), ConstVal.Get(x));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ExprConstant CreateBoolConstant(bool b)
	{
		return CreateConstant(TypeManager.GetPredefAgg(PredefinedType.PT_BOOL).getThisType(), ConstVal.Get(b));
	}

	public static ExprArrayIndex CreateArrayIndex(CType type, Expr array, Expr index)
	{
		return new ExprArrayIndex(type, array, index);
	}

	public static ExprBinOp CreateBinop(ExpressionKind exprKind, CType type, Expr left, Expr right)
	{
		return new ExprBinOp(exprKind, type, left, right);
	}

	public static ExprUnaryOp CreateUnaryOp(ExpressionKind exprKind, CType type, Expr operand)
	{
		return new ExprUnaryOp(exprKind, type, operand);
	}

	public static ExprOperator CreateOperator(ExpressionKind exprKind, CType type, Expr arg1, Expr arg2)
	{
		if (!exprKind.IsUnaryOperator())
		{
			return CreateBinop(exprKind, type, arg1, arg2);
		}
		return CreateUnaryOp(exprKind, type, arg1);
	}

	public static ExprBinOp CreateUserDefinedBinop(ExpressionKind exprKind, CType type, Expr left, Expr right, Expr call, MethPropWithInst userMethod)
	{
		return new ExprBinOp(exprKind, type, left, right, call, userMethod);
	}

	public static ExprUnaryOp CreateUserDefinedUnaryOperator(ExpressionKind exprKind, CType type, Expr operand, ExprCall call, MethPropWithInst userMethod)
	{
		return new ExprUnaryOp(exprKind, type, operand, call, userMethod);
	}

	public static ExprUnaryOp CreateNeg(EXPRFLAG flags, Expr operand)
	{
		ExprUnaryOp exprUnaryOp = CreateUnaryOp(ExpressionKind.Negate, operand.Type, operand);
		exprUnaryOp.Flags |= flags;
		return exprUnaryOp;
	}

	public static ExprBinOp CreateSequence(Expr first, Expr second)
	{
		return CreateBinop(ExpressionKind.Sequence, second.Type, first, second);
	}

	public static ExprAssignment CreateAssignment(Expr left, Expr right)
	{
		return new ExprAssignment(left, right);
	}

	public static ExprNamedArgumentSpecification CreateNamedArgumentSpecification(Name name, Expr value)
	{
		return new ExprNamedArgumentSpecification(name, value);
	}

	public static ExprWrap CreateWrap(Expr expression)
	{
		return new ExprWrap(expression);
	}

	public static ExprBinOp CreateSave(ExprWrap wrap)
	{
		ExprBinOp exprBinOp = CreateBinop(ExpressionKind.Save, wrap.Type, wrap.OptionalExpression, wrap);
		exprBinOp.SetAssignment();
		return exprBinOp;
	}

	public static ExprConstant CreateNull()
	{
		return CreateConstant(NullType.Instance, default(ConstVal));
	}

	public static void AppendItemToList(Expr newItem, ref Expr first, ref Expr last)
	{
		if (newItem != null)
		{
			if (first == null)
			{
				first = newItem;
				last = newItem;
			}
			else if (first.Kind != ExpressionKind.List)
			{
				first = CreateList(first, newItem);
				last = first;
			}
			else
			{
				ExprList exprList = (ExprList)last;
				exprList.OptionalNextListNode = CreateList(exprList.OptionalNextListNode, newItem);
				last = exprList.OptionalNextListNode;
			}
		}
	}

	public static ExprList CreateList(Expr op1, Expr op2)
	{
		return new ExprList(op1, op2);
	}

	public static ExprList CreateList(Expr op1, Expr op2, Expr op3)
	{
		return CreateList(op1, CreateList(op2, op3));
	}

	public static ExprList CreateList(Expr op1, Expr op2, Expr op3, Expr op4)
	{
		return CreateList(op1, CreateList(op2, CreateList(op3, op4)));
	}

	public static ExprClass CreateClass(CType type)
	{
		return new ExprClass(type);
	}
}
