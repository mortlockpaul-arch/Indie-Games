using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class ExpressionTreeCallRewriter : ExprVisitorBase
{
	private sealed class ExpressionExpr : Expr
	{
		public readonly Expression Expression;

		public ExpressionExpr(Expression e)
			: base(ExpressionKind.NoOp)
		{
			Expression = e;
		}
	}

	private readonly Dictionary<ExprCall, Expression> _DictionaryOfParameters;

	private readonly Expression[] _ListOfParameters;

	private int _currentParameterIndex;

	private ExpressionTreeCallRewriter(Expression[] listOfParameters)
	{
		_DictionaryOfParameters = new Dictionary<ExprCall, Expression>();
		_ListOfParameters = listOfParameters;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public static Expression Rewrite(ExprBinOp binOp, Expression[] listOfParameters)
	{
		ExpressionTreeCallRewriter expressionTreeCallRewriter = new ExpressionTreeCallRewriter(listOfParameters);
		expressionTreeCallRewriter.Visit(binOp.OptionalLeftChild);
		ExprCall pExpr = (ExprCall)binOp.OptionalRightChild;
		return (expressionTreeCallRewriter.Visit(pExpr) as ExpressionExpr).Expression;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitSAVE(ExprBinOp pExpr)
	{
		ExprCall key = (ExprCall)pExpr.OptionalLeftChild;
		Expression value = _ListOfParameters[_currentParameterIndex++];
		_DictionaryOfParameters.Add(key, value);
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitCALL(ExprCall pExpr)
	{
		if (pExpr.PredefinedMethod == PREDEFMETH.PM_COUNT)
		{
			return pExpr;
		}
		Expression e;
		switch (pExpr.PredefinedMethod)
		{
		case PREDEFMETH.PM_EXPRESSION_LAMBDA:
			return GenerateLambda(pExpr);
		case PREDEFMETH.PM_EXPRESSION_CALL:
			e = GenerateCall(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_ARRAYINDEX:
		case PREDEFMETH.PM_EXPRESSION_ARRAYINDEX2:
			e = GenerateArrayIndex(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_CONVERT:
		case PREDEFMETH.PM_EXPRESSION_CONVERT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED:
		case PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED_USER_DEFINED:
			e = GenerateConvert(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_PROPERTY:
			e = GenerateProperty(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_FIELD:
			e = GenerateField(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_INVOKE:
			e = GenerateInvoke(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_NEW:
			e = GenerateNew(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_ADD:
		case PREDEFMETH.PM_EXPRESSION_ADDCHECKED:
		case PREDEFMETH.PM_EXPRESSION_AND:
		case PREDEFMETH.PM_EXPRESSION_ANDALSO:
		case PREDEFMETH.PM_EXPRESSION_DIVIDE:
		case PREDEFMETH.PM_EXPRESSION_EQUAL:
		case PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHAN:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL:
		case PREDEFMETH.PM_EXPRESSION_LEFTSHIFT:
		case PREDEFMETH.PM_EXPRESSION_LESSTHAN:
		case PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL:
		case PREDEFMETH.PM_EXPRESSION_MODULO:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLY:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED:
		case PREDEFMETH.PM_EXPRESSION_NOTEQUAL:
		case PREDEFMETH.PM_EXPRESSION_OR:
		case PREDEFMETH.PM_EXPRESSION_ORELSE:
		case PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACT:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED:
			e = GenerateBinaryOperator(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_ADDCHECKED_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_AND_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_ANDALSO_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_DIVIDE_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_EQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHAN_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_LEFTSHIFT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_LESSTHAN_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_MODULO_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLY_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NOTEQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_OR_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_ORELSE_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED_USER_DEFINED:
			e = GenerateUserDefinedBinaryOperator(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_NEGATE:
		case PREDEFMETH.PM_EXPRESSION_NEGATECHECKED:
		case PREDEFMETH.PM_EXPRESSION_NOT:
			e = GenerateUnaryOperator(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_UNARYPLUS_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NEGATE_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NEGATECHECKED_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED:
			e = GenerateUserDefinedUnaryOperator(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_CONSTANT_OBJECT_TYPE:
			e = GenerateConstantType(pExpr);
			break;
		case PREDEFMETH.PM_EXPRESSION_ASSIGN:
			e = GenerateAssignment(pExpr);
			break;
		default:
			throw Error.InternalCompilerError();
		}
		return new ExpressionExpr(e);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	protected override Expr VisitWRAP(ExprWrap pExpr)
	{
		return new ExpressionExpr(GetExpression(pExpr));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expr GenerateLambda(ExprCall pExpr)
	{
		return Visit(((ExprList)pExpr.OptionalArguments).OptionalElement);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateCall(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		ExprMethodInfo exprMethodInfo;
		ExprArrayInit arrinit;
		if (exprList.OptionalNextListNode is ExprList exprList2)
		{
			exprMethodInfo = (ExprMethodInfo)exprList2.OptionalElement;
			arrinit = (ExprArrayInit)exprList2.OptionalNextListNode;
		}
		else
		{
			exprMethodInfo = (ExprMethodInfo)exprList.OptionalNextListNode;
			arrinit = null;
		}
		Expression instance = null;
		MethodInfo methodInfo = exprMethodInfo.MethodInfo;
		Expression[] argumentsFromArrayInit = GetArgumentsFromArrayInit(arrinit);
		if (methodInfo == null)
		{
			throw Error.InternalCompilerError();
		}
		if (!methodInfo.IsStatic)
		{
			instance = GetExpression(((ExprList)pExpr.OptionalArguments).OptionalElement);
		}
		return Expression.Call(instance, methodInfo, argumentsFromArrayInit);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateArrayIndex(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		Expression expression = GetExpression(exprList.OptionalElement);
		Expression[] indexes = ((pExpr.PredefinedMethod != PREDEFMETH.PM_EXPRESSION_ARRAYINDEX) ? GetArgumentsFromArrayInit((ExprArrayInit)exprList.OptionalNextListNode) : new Expression[1] { GetExpression(exprList.OptionalNextListNode) });
		return Expression.ArrayAccess(expression, indexes);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateConvert(ExprCall pExpr)
	{
		PREDEFMETH predefinedMethod = pExpr.PredefinedMethod;
		Expression expression;
		Type associatedSystemType;
		if (predefinedMethod == PREDEFMETH.PM_EXPRESSION_CONVERT_USER_DEFINED || predefinedMethod == PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED_USER_DEFINED)
		{
			ExprList exprList = (ExprList)pExpr.OptionalArguments;
			ExprList exprList2 = (ExprList)exprList.OptionalNextListNode;
			expression = GetExpression(exprList.OptionalElement);
			associatedSystemType = ((ExprTypeOf)exprList2.OptionalElement).SourceType.AssociatedSystemType;
			if (expression.Type.MakeByRefType() == associatedSystemType)
			{
				return expression;
			}
			MethodInfo methodInfo = ((ExprMethodInfo)exprList2.OptionalNextListNode).MethodInfo;
			if (predefinedMethod == PREDEFMETH.PM_EXPRESSION_CONVERT_USER_DEFINED)
			{
				return Expression.Convert(expression, associatedSystemType, methodInfo);
			}
			return Expression.ConvertChecked(expression, associatedSystemType, methodInfo);
		}
		ExprList exprList3 = (ExprList)pExpr.OptionalArguments;
		expression = GetExpression(exprList3.OptionalElement);
		associatedSystemType = ((ExprTypeOf)exprList3.OptionalNextListNode).SourceType.AssociatedSystemType;
		if (expression.Type.MakeByRefType() == associatedSystemType)
		{
			return expression;
		}
		if ((pExpr.Flags & EXPRFLAG.EXF_USERCALLABLE) != 0)
		{
			return Expression.Unbox(expression, associatedSystemType);
		}
		if (predefinedMethod == PREDEFMETH.PM_EXPRESSION_CONVERT)
		{
			return Expression.Convert(expression, associatedSystemType);
		}
		return Expression.ConvertChecked(expression, associatedSystemType);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateProperty(ExprCall pExpr)
	{
		ExprList obj = (ExprList)pExpr.OptionalArguments;
		Expr optionalElement = obj.OptionalElement;
		Expr optionalNextListNode = obj.OptionalNextListNode;
		ExprPropertyInfo exprPropertyInfo;
		ExprArrayInit exprArrayInit;
		if (optionalNextListNode is ExprList exprList)
		{
			exprPropertyInfo = exprList.OptionalElement as ExprPropertyInfo;
			exprArrayInit = exprList.OptionalNextListNode as ExprArrayInit;
		}
		else
		{
			exprPropertyInfo = optionalNextListNode as ExprPropertyInfo;
			exprArrayInit = null;
		}
		PropertyInfo propertyInfo = exprPropertyInfo.PropertyInfo;
		if (propertyInfo == null)
		{
			throw Error.InternalCompilerError();
		}
		if (exprArrayInit == null)
		{
			return Expression.Property(GetExpression(optionalElement), propertyInfo);
		}
		return Expression.Property(GetExpression(optionalElement), propertyInfo, GetArgumentsFromArrayInit(exprArrayInit));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateField(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		ExprFieldInfo obj = (ExprFieldInfo)exprList.OptionalNextListNode;
		Type type = obj.FieldType.AssociatedSystemType;
		FieldInfo fieldInfo = obj.Field.AssociatedFieldInfo;
		if (!type.IsGenericType && !type.IsNested)
		{
			type = fieldInfo.DeclaringType;
		}
		if (type.IsGenericType)
		{
			fieldInfo = type.GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
		return Expression.Field(GetExpression(exprList.OptionalElement), fieldInfo);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateInvoke(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		return Expression.Invoke(GetExpression(exprList.OptionalElement), GetArgumentsFromArrayInit(exprList.OptionalNextListNode as ExprArrayInit));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateNew(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		ConstructorInfo constructorInfo = ((ExprMethodInfo)exprList.OptionalElement).ConstructorInfo;
		Expression[] argumentsFromArrayInit = GetArgumentsFromArrayInit(exprList.OptionalNextListNode as ExprArrayInit);
		return Expression.New(constructorInfo, argumentsFromArrayInit);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private static Expression GenerateConstantType(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		return Expression.Constant(exprList.OptionalElement.Object, ((ExprTypeOf)exprList.OptionalNextListNode).SourceType.AssociatedSystemType);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateAssignment(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		return Expression.Assign(GetExpression(exprList.OptionalElement), GetExpression(exprList.OptionalNextListNode));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateBinaryOperator(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		Expression expression = GetExpression(exprList.OptionalElement);
		Expression expression2 = GetExpression(exprList.OptionalNextListNode);
		return pExpr.PredefinedMethod switch
		{
			PREDEFMETH.PM_EXPRESSION_ADD => Expression.Add(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_AND => Expression.And(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_DIVIDE => Expression.Divide(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_EQUAL => Expression.Equal(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR => Expression.ExclusiveOr(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_GREATERTHAN => Expression.GreaterThan(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL => Expression.GreaterThanOrEqual(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_LEFTSHIFT => Expression.LeftShift(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_LESSTHAN => Expression.LessThan(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL => Expression.LessThanOrEqual(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_MODULO => Expression.Modulo(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_MULTIPLY => Expression.Multiply(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_NOTEQUAL => Expression.NotEqual(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_OR => Expression.Or(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT => Expression.RightShift(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_SUBTRACT => Expression.Subtract(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_ORELSE => Expression.OrElse(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_ANDALSO => Expression.AndAlso(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_ADDCHECKED => Expression.AddChecked(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED => Expression.MultiplyChecked(expression, expression2), 
			PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED => Expression.SubtractChecked(expression, expression2), 
			_ => throw Error.InternalCompilerError(), 
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateUserDefinedBinaryOperator(ExprCall pExpr)
	{
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		Expression expression = GetExpression(exprList.OptionalElement);
		Expression expression2 = GetExpression(((ExprList)exprList.OptionalNextListNode).OptionalElement);
		exprList = (ExprList)exprList.OptionalNextListNode;
		bool liftToNull = false;
		MethodInfo methodInfo;
		if (exprList.OptionalNextListNode is ExprList exprList2)
		{
			liftToNull = ((ExprConstant)exprList2.OptionalElement).Val.Int32Val == 1;
			methodInfo = ((ExprMethodInfo)exprList2.OptionalNextListNode).MethodInfo;
		}
		else
		{
			methodInfo = ((ExprMethodInfo)exprList.OptionalNextListNode).MethodInfo;
		}
		return pExpr.PredefinedMethod switch
		{
			PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED => Expression.Add(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_AND_USER_DEFINED => Expression.And(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_DIVIDE_USER_DEFINED => Expression.Divide(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_EQUAL_USER_DEFINED => Expression.Equal(expression, expression2, liftToNull, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR_USER_DEFINED => Expression.ExclusiveOr(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_GREATERTHAN_USER_DEFINED => Expression.GreaterThan(expression, expression2, liftToNull, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL_USER_DEFINED => Expression.GreaterThanOrEqual(expression, expression2, liftToNull, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_LEFTSHIFT_USER_DEFINED => Expression.LeftShift(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_LESSTHAN_USER_DEFINED => Expression.LessThan(expression, expression2, liftToNull, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL_USER_DEFINED => Expression.LessThanOrEqual(expression, expression2, liftToNull, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_MODULO_USER_DEFINED => Expression.Modulo(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_MULTIPLY_USER_DEFINED => Expression.Multiply(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_NOTEQUAL_USER_DEFINED => Expression.NotEqual(expression, expression2, liftToNull, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_OR_USER_DEFINED => Expression.Or(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT_USER_DEFINED => Expression.RightShift(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_SUBTRACT_USER_DEFINED => Expression.Subtract(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_ORELSE_USER_DEFINED => Expression.OrElse(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_ANDALSO_USER_DEFINED => Expression.AndAlso(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_ADDCHECKED_USER_DEFINED => Expression.AddChecked(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED_USER_DEFINED => Expression.MultiplyChecked(expression, expression2, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED_USER_DEFINED => Expression.SubtractChecked(expression, expression2, methodInfo), 
			_ => throw Error.InternalCompilerError(), 
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateUnaryOperator(ExprCall pExpr)
	{
		PREDEFMETH predefinedMethod = pExpr.PredefinedMethod;
		Expression expression = GetExpression(pExpr.OptionalArguments);
		return predefinedMethod switch
		{
			PREDEFMETH.PM_EXPRESSION_NOT => Expression.Not(expression), 
			PREDEFMETH.PM_EXPRESSION_NEGATE => Expression.Negate(expression), 
			PREDEFMETH.PM_EXPRESSION_NEGATECHECKED => Expression.NegateChecked(expression), 
			_ => throw Error.InternalCompilerError(), 
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GenerateUserDefinedUnaryOperator(ExprCall pExpr)
	{
		PREDEFMETH predefinedMethod = pExpr.PredefinedMethod;
		ExprList exprList = (ExprList)pExpr.OptionalArguments;
		Expression expression = GetExpression(exprList.OptionalElement);
		MethodInfo methodInfo = ((ExprMethodInfo)exprList.OptionalNextListNode).MethodInfo;
		return predefinedMethod switch
		{
			PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED => Expression.Not(expression, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_NEGATE_USER_DEFINED => Expression.Negate(expression, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_UNARYPLUS_USER_DEFINED => Expression.UnaryPlus(expression, methodInfo), 
			PREDEFMETH.PM_EXPRESSION_NEGATECHECKED_USER_DEFINED => Expression.NegateChecked(expression, methodInfo), 
			_ => throw Error.InternalCompilerError(), 
		};
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression GetExpression(Expr pExpr)
	{
		if (pExpr is ExprWrap exprWrap)
		{
			return _DictionaryOfParameters[(ExprCall)exprWrap.OptionalExpression];
		}
		if (pExpr is ExprConstant)
		{
			return null;
		}
		ExprCall exprCall = (ExprCall)pExpr;
		switch (exprCall.PredefinedMethod)
		{
		case PREDEFMETH.PM_EXPRESSION_CALL:
			return GenerateCall(exprCall);
		case PREDEFMETH.PM_EXPRESSION_CONVERT:
		case PREDEFMETH.PM_EXPRESSION_CONVERT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED:
		case PREDEFMETH.PM_EXPRESSION_CONVERTCHECKED_USER_DEFINED:
			return GenerateConvert(exprCall);
		case PREDEFMETH.PM_EXPRESSION_NEWARRAYINIT:
		{
			ExprList exprList = (ExprList)exprCall.OptionalArguments;
			return Expression.NewArrayInit(((ExprTypeOf)exprList.OptionalElement).SourceType.AssociatedSystemType, GetArgumentsFromArrayInit((ExprArrayInit)exprList.OptionalNextListNode));
		}
		case PREDEFMETH.PM_EXPRESSION_ARRAYINDEX:
		case PREDEFMETH.PM_EXPRESSION_ARRAYINDEX2:
			return GenerateArrayIndex(exprCall);
		case PREDEFMETH.PM_EXPRESSION_NEW:
			return GenerateNew(exprCall);
		case PREDEFMETH.PM_EXPRESSION_PROPERTY:
			return GenerateProperty(exprCall);
		case PREDEFMETH.PM_EXPRESSION_FIELD:
			return GenerateField(exprCall);
		case PREDEFMETH.PM_EXPRESSION_CONSTANT_OBJECT_TYPE:
			return GenerateConstantType(exprCall);
		case PREDEFMETH.PM_EXPRESSION_ASSIGN:
			return GenerateAssignment(exprCall);
		case PREDEFMETH.PM_EXPRESSION_ADD:
		case PREDEFMETH.PM_EXPRESSION_ADDCHECKED:
		case PREDEFMETH.PM_EXPRESSION_AND:
		case PREDEFMETH.PM_EXPRESSION_ANDALSO:
		case PREDEFMETH.PM_EXPRESSION_DIVIDE:
		case PREDEFMETH.PM_EXPRESSION_EQUAL:
		case PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHAN:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL:
		case PREDEFMETH.PM_EXPRESSION_LEFTSHIFT:
		case PREDEFMETH.PM_EXPRESSION_LESSTHAN:
		case PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL:
		case PREDEFMETH.PM_EXPRESSION_MODULO:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLY:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED:
		case PREDEFMETH.PM_EXPRESSION_NOTEQUAL:
		case PREDEFMETH.PM_EXPRESSION_OR:
		case PREDEFMETH.PM_EXPRESSION_ORELSE:
		case PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACT:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED:
			return GenerateBinaryOperator(exprCall);
		case PREDEFMETH.PM_EXPRESSION_ADD_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_ADDCHECKED_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_AND_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_ANDALSO_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_DIVIDE_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_EQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_EXCLUSIVEOR_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHAN_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_GREATERTHANOREQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_LEFTSHIFT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_LESSTHAN_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_LESSTHANOREQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_MODULO_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLY_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_MULTIPLYCHECKED_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NOTEQUAL_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_OR_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_ORELSE_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_RIGHTSHIFT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACT_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_SUBTRACTCHECKED_USER_DEFINED:
			return GenerateUserDefinedBinaryOperator(exprCall);
		case PREDEFMETH.PM_EXPRESSION_NEGATE:
		case PREDEFMETH.PM_EXPRESSION_NEGATECHECKED:
		case PREDEFMETH.PM_EXPRESSION_NOT:
			return GenerateUnaryOperator(exprCall);
		case PREDEFMETH.PM_EXPRESSION_UNARYPLUS_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NEGATE_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NEGATECHECKED_USER_DEFINED:
		case PREDEFMETH.PM_EXPRESSION_NOT_USER_DEFINED:
			return GenerateUserDefinedUnaryOperator(exprCall);
		default:
			throw Error.InternalCompilerError();
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	private Expression[] GetArgumentsFromArrayInit(ExprArrayInit arrinit)
	{
		List<Expression> list = new List<Expression>();
		if (arrinit != null)
		{
			Expr expr = arrinit.OptionalArguments;
			while (expr != null)
			{
				Expr pExpr;
				if (expr is ExprList exprList)
				{
					pExpr = exprList.OptionalElement;
					expr = exprList.OptionalNextListNode;
				}
				else
				{
					pExpr = expr;
					expr = null;
				}
				list.Add(GetExpression(pExpr));
			}
		}
		return list.ToArray();
	}
}
