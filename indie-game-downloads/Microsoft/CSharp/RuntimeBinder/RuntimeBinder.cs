using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Semantics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder;

internal readonly struct RuntimeBinder
{
	private static readonly object s_bindLock = new object();

	private readonly ExpressionBinder _binder;

	internal bool IsChecked => _binder.Context.Checked;

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public RuntimeBinder(Type contextType, bool isChecked = false)
	{
		AggregateSymbol context;
		if (contextType != null)
		{
			lock (s_bindLock)
			{
				context = ((AggregateType)SymbolTable.GetCTypeFromType(contextType)).OwningAggregate;
			}
		}
		else
		{
			context = null;
		}
		_binder = new ExpressionBinder(new BindingContext(context, isChecked));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public Expression Bind(ICSharpBinder payload, Expression[] parameters, DynamicMetaObject[] args, out DynamicMetaObject deferredBinding)
	{
		lock (s_bindLock)
		{
			return BindCore(payload, parameters, args, out deferredBinding);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expression BindCore(ICSharpBinder payload, Expression[] parameters, DynamicMetaObject[] args, out DynamicMetaObject deferredBinding)
	{
		ArgumentObject[] array = CreateArgumentArray(payload, parameters, args);
		payload.PopulateSymbolTableWithName(array[0].Type, array);
		AddConversionsForArguments(array);
		Scope pScope = SymFactory.CreateScope();
		LocalVariableSymbol[] locals = PopulateLocalScope(payload, pScope, array, parameters);
		if (DeferBinding(payload, array, args, locals, out deferredBinding))
		{
			return null;
		}
		Expr pResult = payload.DispatchPayload(this, array, locals);
		return CreateExpressionTreeFromResult(parameters, pScope, pResult);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private bool DeferBinding(ICSharpBinder payload, ArgumentObject[] arguments, DynamicMetaObject[] args, LocalVariableSymbol[] locals, out DynamicMetaObject deferredBinding)
	{
		if (payload is CSharpInvokeMemberBinder cSharpInvokeMemberBinder)
		{
			Type[] typeArguments = cSharpInvokeMemberBinder.TypeArguments;
			int arity = ((typeArguments != null) ? typeArguments.Length : 0);
			MemberLookup mem = new MemberLookup();
			Expr callingObject = CreateCallingObjectForCall(cSharpInvokeMemberBinder, arguments, locals);
			SymWithType symWithType = SymbolTable.LookupMember(cSharpInvokeMemberBinder.Name, callingObject, _binder.Context.ContextForMemberLookup, arity, mem, (cSharpInvokeMemberBinder.Flags & CSharpCallFlags.EventHookup) != 0, requireInvocable: true);
			if (symWithType != null && symWithType.Sym.getKind() != SYMKIND.SK_MethodSymbol)
			{
				CSharpGetMemberBinder cSharpGetMemberBinder = new CSharpGetMemberBinder(cSharpInvokeMemberBinder.Name, resultIndexed: false, cSharpInvokeMemberBinder.CallingContext, new CSharpArgumentInfo[1] { cSharpInvokeMemberBinder.GetArgumentInfo(0) }).TryGetExisting();
				CSharpArgumentInfo[] array = cSharpInvokeMemberBinder.ArgumentInfoArray();
				array[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
				CSharpInvokeBinder cSharpInvokeBinder = new CSharpInvokeBinder(cSharpInvokeMemberBinder.Flags, cSharpInvokeMemberBinder.CallingContext, array).TryGetExisting();
				DynamicMetaObject[] array2 = new DynamicMetaObject[args.Length - 1];
				Array.Copy(args, 1, array2, 0, args.Length - 1);
				deferredBinding = cSharpInvokeBinder.Defer(cSharpGetMemberBinder.Defer(args[0]), array2);
				return true;
			}
		}
		deferredBinding = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Expression CreateExpressionTreeFromResult(Expression[] parameters, Scope pScope, Expr pResult)
	{
		return ExpressionTreeCallRewriter.Rewrite(ExpressionTreeRewriter.Rewrite(GenerateBoundLambda(pScope, pResult)), parameters);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Type GetArgumentType(ICSharpBinder p, CSharpArgumentInfo argInfo, Expression param, DynamicMetaObject arg, int index)
	{
		Type type = (argInfo.UseCompileTimeType ? param.Type : arg.LimitType);
		if (argInfo.IsByRefOrOut)
		{
			if (index != 0 || !p.IsBinderThatCanHaveRefReceiver)
			{
				type = type.MakeByRefType();
			}
		}
		else if (!argInfo.UseCompileTimeType)
		{
			CType cTypeFromType = SymbolTable.GetCTypeFromType(type);
			type = TypeManager.GetBestAccessibleType(_binder.Context.ContextForMemberLookup, cTypeFromType).AssociatedSystemType;
		}
		return type;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ArgumentObject[] CreateArgumentArray(ICSharpBinder payload, Expression[] parameters, DynamicMetaObject[] args)
	{
		ArgumentObject[] array = new ArgumentObject[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			CSharpArgumentInfo argumentInfo = payload.GetArgumentInfo(i);
			array[i] = new ArgumentObject(args[i].Value, argumentInfo, GetArgumentType(payload, argumentInfo, parameters[i], args[i], i));
		}
		return array;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static void PopulateSymbolTableWithPayloadInformation(ICSharpInvokeOrInvokeMemberBinder callOrInvoke, Type callingType, ArgumentObject[] arguments)
	{
		Type type;
		if (callOrInvoke.StaticCall)
		{
			type = arguments[0].Value as Type;
			if (type == null)
			{
				throw Error.BindStaticRequiresType(arguments[0].Info.Name);
			}
		}
		else
		{
			type = callingType;
		}
		SymbolTable.PopulateSymbolTableWithName(callOrInvoke.Name, callOrInvoke.TypeArguments, type);
		if (callOrInvoke.Name.StartsWith("set_", StringComparison.Ordinal) || callOrInvoke.Name.StartsWith("get_", StringComparison.Ordinal))
		{
			SymbolTable.PopulateSymbolTableWithName(callOrInvoke.Name.Substring(4), callOrInvoke.TypeArguments, type);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddConversionsForArguments(ArgumentObject[] arguments)
	{
		for (int i = 0; i < arguments.Length; i++)
		{
			SymbolTable.AddConversionsForType(arguments[i].Type);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal ExprWithArgs DispatchPayload(ICSharpInvokeOrInvokeMemberBinder payload, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return BindCall(payload, CreateCallingObjectForCall(payload, arguments, locals), arguments, locals);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static LocalVariableSymbol[] PopulateLocalScope(ICSharpBinder payload, Scope pScope, ArgumentObject[] arguments, Expression[] parameterExpressions)
	{
		LocalVariableSymbol[] array = new LocalVariableSymbol[parameterExpressions.Length];
		for (int i = 0; i < parameterExpressions.Length; i++)
		{
			Expression expression = parameterExpressions[i];
			CType cType = SymbolTable.GetCTypeFromType(expression.Type);
			if ((i != 0 || !payload.IsBinderThatCanHaveRefReceiver) && expression is ParameterExpression { IsByRef: not false })
			{
				CSharpArgumentInfo info = arguments[i].Info;
				if (info.IsByRefOrOut)
				{
					cType = TypeManager.GetParameterModifier(cType, info.IsOut);
				}
			}
			LocalVariableSymbol localVariableSymbol = SymFactory.CreateLocalVar(NameManager.Add("p" + i), pScope, cType);
			array[i] = localVariableSymbol;
		}
		return array;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprBoundLambda GenerateBoundLambda(Scope pScope, Expr call)
	{
		return ExprFactory.CreateAnonymousMethod(SymbolLoader.GetPredefindType(PredefinedType.PT_FUNC), pScope, call);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr CreateLocal(Type type, bool isOut, LocalVariableSymbol local)
	{
		CType dest = ((!isOut) ? SymbolTable.GetCTypeFromType(type) : TypeManager.GetParameterModifier(SymbolTable.GetCTypeFromType(type.GetElementType()), isOut: true));
		ExprLocal expr = ExprFactory.CreateLocal(local);
		Expr obj = _binder.tryConvert(expr, dest) ?? _binder.mustCast(expr, dest);
		obj.Flags |= EXPRFLAG.EXF_LVALUE;
		return obj;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr CreateArgumentListEXPR(ArgumentObject[] arguments, LocalVariableSymbol[] locals, int startIndex, int endIndex)
	{
		Expr first = null;
		Expr last = null;
		if (arguments != null)
		{
			for (int i = startIndex; i < endIndex; i++)
			{
				ArgumentObject argument = arguments[i];
				Expr expr = CreateArgumentEXPR(argument, locals[i]);
				if (first == null)
				{
					first = expr;
					last = first;
				}
				else
				{
					ExprFactory.AppendItemToList(expr, ref first, ref last);
				}
			}
		}
		return first;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr CreateArgumentEXPR(ArgumentObject argument, LocalVariableSymbol local)
	{
		Expr expr = (argument.Info.LiteralConstant ? ((argument.Value != null) ? ExprFactory.CreateConstant(SymbolTable.GetCTypeFromType(argument.Type), ConstVal.Get(argument.Value)) : ((!argument.Info.UseCompileTimeType) ? ExprFactory.CreateNull() : ExprFactory.CreateConstant(SymbolTable.GetCTypeFromType(argument.Type), default(ConstVal)))) : ((argument.Info.UseCompileTimeType || argument.Value != null) ? CreateLocal(argument.Type, argument.Info.IsOut, local) : ExprFactory.CreateNull()));
		if (argument.Info.NamedArgument)
		{
			expr = ExprFactory.CreateNamedArgumentSpecification(NameManager.Add(argument.Info.Name), expr);
		}
		if (!argument.Info.UseCompileTimeType && argument.Value != null)
		{
			expr.RuntimeObject = argument.Value;
			expr.RuntimeObjectActualType = SymbolTable.GetCTypeFromType(argument.Value.GetType());
		}
		return expr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ExprMemberGroup CreateMemberGroupExpr(string Name, Type[] typeArguments, Expr callingObject, SYMKIND kind)
	{
		Name name = NameManager.Add(Name);
		CType type = callingObject.Type;
		AggregateType aggregateType = ((type is ArrayType) ? SymbolLoader.GetPredefindType(PredefinedType.PT_ARRAY) : ((!(type is NullableType nullableType)) ? ((AggregateType)type) : nullableType.GetAts()));
		HashSet<CType> hashSet = new HashSet<CType>();
		List<CType> list = new List<CType>();
		symbmask_t mask = symbmask_t.MASK_MethodSymbol;
		switch (kind)
		{
		case SYMKIND.SK_PropertySymbol:
		case SYMKIND.SK_IndexerSymbol:
			mask = symbmask_t.MASK_PropertySymbol;
			break;
		case SYMKIND.SK_MethodSymbol:
			mask = symbmask_t.MASK_MethodSymbol;
			break;
		}
		bool flag = name == NameManager.GetPredefinedName(PredefinedName.PN_CTOR);
		foreach (AggregateType item in aggregateType.TypeHierarchy)
		{
			if (SymbolTable.AggregateContainsMethod(item.OwningAggregate, Name, mask) && hashSet.Add(item))
			{
				list.Add(item);
			}
			if (flag)
			{
				break;
			}
		}
		EXPRFLAG eXPRFLAG = EXPRFLAG.EXF_USERCALLABLE;
		if (Name == "Invoke" && callingObject.Type.IsDelegateType)
		{
			eXPRFLAG |= EXPRFLAG.EXF_GOTONOTBLOCKED;
		}
		if (Name == ".ctor")
		{
			eXPRFLAG |= EXPRFLAG.EXF_CTOR;
		}
		if (Name == "$Item$")
		{
			eXPRFLAG |= EXPRFLAG.EXF_INDEXER;
		}
		TypeArray typeArgs = ((typeArguments != null && typeArguments.Length != 0) ? TypeArray.Allocate(SymbolTable.GetCTypeArrayFromTypes(typeArguments)) : TypeArray.Empty);
		ExprMemberGroup exprMemberGroup = ExprFactory.CreateMemGroup(eXPRFLAG, name, typeArgs, kind, aggregateType, null, new CMemberLookupResults(TypeArray.Allocate(list.ToArray()), name));
		if (!(callingObject is ExprClass))
		{
			exprMemberGroup.OptionalObject = callingObject;
		}
		return exprMemberGroup;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr CreateProperty(SymWithType swt, Expr callingObject, BindingFlag flags)
	{
		PropertySymbol propertySymbol = swt.Prop();
		AggregateType type = swt.GetType();
		PropWithType pwt = new PropWithType(propertySymbol, type);
		ExprMemberGroup pMemGroup = CreateMemberGroupExpr(propertySymbol.name.Text, null, callingObject, SYMKIND.SK_PropertySymbol);
		return _binder.BindToProperty((callingObject is ExprClass) ? null : callingObject, pwt, flags, null, pMemGroup);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprWithArgs CreateIndexer(SymWithType swt, Expr callingObject, Expr arguments, BindingFlag bindFlags)
	{
		ExprMemberGroup grp = CreateMemberGroupExpr((swt.Sym as IndexerSymbol).name.Text, null, callingObject, SYMKIND.SK_PropertySymbol);
		ExprWithArgs result = _binder.BindMethodGroupToArguments(bindFlags, grp, arguments);
		ReorderArgumentsForNamedAndOptional(callingObject, result);
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr CreateArray(Expr callingObject, Expr optionalIndexerArguments)
	{
		return _binder.BindArrayIndexCore(callingObject, optionalIndexerArguments);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr CreateField(SymWithType swt, Expr callingObject)
	{
		FieldSymbol field = swt.Field();
		AggregateType type = swt.GetType();
		FieldWithType fwt = new FieldWithType(field, type);
		return _binder.BindToField((callingObject is ExprClass) ? null : callingObject, fwt, (BindingFlag)0);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private Expr CreateCallingObjectForCall(ICSharpInvokeOrInvokeMemberBinder payload, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		Expr expr;
		if (payload.StaticCall)
		{
			expr = ExprFactory.CreateClass(SymbolTable.GetCTypeFromType(arguments[0].Value as Type));
		}
		else
		{
			if (!arguments[0].Info.UseCompileTimeType && arguments[0].Value == null)
			{
				throw Error.NullReferenceOnMemberException();
			}
			expr = _binder.mustConvert(CreateArgumentEXPR(arguments[0], locals[0]), SymbolTable.GetCTypeFromType(arguments[0].Type));
			if (arguments[0].Type.IsValueType && expr is ExprCast)
			{
				expr.Flags |= EXPRFLAG.EXF_USERCALLABLE;
			}
		}
		return expr;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private ExprWithArgs BindCall(ICSharpInvokeOrInvokeMemberBinder payload, Expr callingObject, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		if (payload is InvokeBinder && !callingObject.Type.IsDelegateType)
		{
			throw Error.BindInvokeFailedNonDelegate();
		}
		Type[] typeArguments = payload.TypeArguments;
		int arity = ((typeArguments != null) ? typeArguments.Length : 0);
		MemberLookup memberLookup = new MemberLookup();
		SymWithType symWithType = SymbolTable.LookupMember(payload.Name, callingObject, _binder.Context.ContextForMemberLookup, arity, memberLookup, (payload.Flags & CSharpCallFlags.EventHookup) != 0, requireInvocable: true);
		if (symWithType == null)
		{
			throw memberLookup.ReportErrors();
		}
		if (symWithType.Sym.getKind() != SYMKIND.SK_MethodSymbol)
		{
			throw Error.InternalCompilerError();
		}
		ExprMemberGroup exprMemberGroup = CreateMemberGroupExpr(payload.Name, payload.TypeArguments, callingObject, symWithType.Sym.getKind());
		if ((payload.Flags & CSharpCallFlags.SimpleNameCall) != CSharpCallFlags.None)
		{
			callingObject.Flags |= EXPRFLAG.EXF_UNREALIZEDGOTO;
		}
		if ((payload.Flags & CSharpCallFlags.EventHookup) != CSharpCallFlags.None)
		{
			memberLookup = new MemberLookup();
			SymWithType symWithType2 = SymbolTable.LookupMember(payload.Name.Split('_')[1], callingObject, _binder.Context.ContextForMemberLookup, arity, memberLookup, (payload.Flags & CSharpCallFlags.EventHookup) != 0, requireInvocable: true);
			if (symWithType2 == null)
			{
				throw memberLookup.ReportErrors();
			}
			CType typeSrc = null;
			if (symWithType2.Sym.getKind() == SYMKIND.SK_FieldSymbol)
			{
				typeSrc = symWithType2.Field().GetType();
			}
			else if (symWithType2.Sym.getKind() == SYMKIND.SK_EventSymbol)
			{
				typeSrc = symWithType2.Event().type;
			}
			Type associatedSystemType = TypeManager.SubstType(typeSrc, symWithType2.Ats).AssociatedSystemType;
			if (associatedSystemType != null)
			{
				BindImplicitConversion(new ArgumentObject[1] { arguments[1] }, associatedSystemType, locals, bIsArrayCreationConversion: false);
			}
			exprMemberGroup.Flags &= ~EXPRFLAG.EXF_USERCALLABLE;
		}
		if ((payload.Name.StartsWith("set_", StringComparison.Ordinal) && ((MethodSymbol)symWithType.Sym).Params.Count > 1) || (payload.Name.StartsWith("get_", StringComparison.Ordinal) && ((MethodSymbol)symWithType.Sym).Params.Count > 0))
		{
			exprMemberGroup.Flags &= ~EXPRFLAG.EXF_USERCALLABLE;
		}
		ExprCall exprCall = _binder.BindMethodGroupToArguments(BindingFlag.BIND_RVALUEREQUIRED | BindingFlag.BIND_STMTEXPRONLY, exprMemberGroup, CreateArgumentListEXPR(arguments, locals, 1, arguments.Length)) as ExprCall;
		CheckForConditionalMethodError(exprCall);
		ReorderArgumentsForNamedAndOptional(callingObject, exprCall);
		return exprCall;
	}

	private static void CheckForConditionalMethodError(ExprCall call)
	{
		MethodSymbol methodSymbol = call.MethWithInst.Meth();
		if (methodSymbol.AssociatedMemberInfo.GetCustomAttributes(typeof(ConditionalAttribute), inherit: true).Length != 0)
		{
			throw Error.BindCallToConditionalMethod(methodSymbol.name);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ReorderArgumentsForNamedAndOptional(Expr callingObject, ExprWithArgs result)
	{
		Expr optionalArguments = result.OptionalArguments;
		AggregateType ats;
		ExprMemberGroup memberGroup;
		TypeArray typeArgsMeth;
		MethodOrPropertySymbol methodOrPropertySymbol;
		if (result is ExprCall exprCall)
		{
			ats = exprCall.MethWithInst.Ats;
			methodOrPropertySymbol = exprCall.MethWithInst.Meth();
			memberGroup = exprCall.MemberGroup;
			typeArgsMeth = exprCall.MethWithInst.TypeArgs;
		}
		else
		{
			ExprProperty obj = result as ExprProperty;
			ats = obj.PropWithTypeSlot.Ats;
			methodOrPropertySymbol = obj.PropWithTypeSlot.Prop();
			memberGroup = obj.MemberGroup;
			typeArgsMeth = null;
		}
		ArgInfos argInfos = new ArgInfos
		{
			carg = ExpressionBinder.CountArguments(optionalArguments)
		};
		ExpressionBinder.FillInArgInfoFromArgList(argInfos, optionalArguments);
		TypeArray typeArray = TypeManager.SubstTypeArray(methodOrPropertySymbol.Params, ats, typeArgsMeth);
		methodOrPropertySymbol = ExpressionBinder.GroupToArgsBinder.FindMostDerivedMethod(methodOrPropertySymbol, callingObject.Type);
		ExpressionBinder.GroupToArgsBinder.ReOrderArgsForNamedArguments(methodOrPropertySymbol, typeArray, ats, memberGroup, argInfos);
		Expr expr = null;
		for (int num = argInfos.carg - 1; num >= 0; num--)
		{
			Expr pArg = argInfos.prgexpr[num];
			pArg = StripNamedArgument(pArg);
			pArg = _binder.tryConvert(pArg, typeArray[num]);
			expr = ((expr == null) ? pArg : ExprFactory.CreateList(pArg, expr));
		}
		result.OptionalArguments = expr;
	}

	private Expr StripNamedArgument(Expr pArg)
	{
		if (pArg is ExprNamedArgumentSpecification exprNamedArgumentSpecification)
		{
			pArg = exprNamedArgumentSpecification.Value;
		}
		else if (pArg is ExprArrayInit exprArrayInit)
		{
			exprArrayInit.OptionalArguments = StripNamedArguments(exprArrayInit.OptionalArguments);
		}
		return pArg;
	}

	private Expr StripNamedArguments(Expr pArg)
	{
		ExprList exprList = pArg as ExprList;
		if (exprList != null)
		{
			while (true)
			{
				exprList.OptionalElement = StripNamedArgument(exprList.OptionalElement);
				if (!(exprList.OptionalNextListNode is ExprList exprList2))
				{
					break;
				}
				exprList = exprList2;
			}
			exprList.OptionalNextListNode = StripNamedArgument(exprList.OptionalNextListNode);
		}
		return StripNamedArgument(pArg);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindUnaryOperation(CSharpUnaryOperationBinder payload, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		OperatorKind operatorKind = GetOperatorKind(payload.Operation);
		Expr expr = CreateArgumentEXPR(arguments[0], locals[0]);
		expr.ErrorString = Operators.GetDisplayName(GetOperatorKind(payload.Operation));
		if (operatorKind == OperatorKind.OP_TRUE || operatorKind == OperatorKind.OP_FALSE)
		{
			Expr expr2 = _binder.tryConvert(expr, SymbolLoader.GetPredefindType(PredefinedType.PT_BOOL));
			if (expr2 != null && operatorKind == OperatorKind.OP_FALSE)
			{
				expr2 = _binder.BindStandardUnaryOperator(OperatorKind.OP_LOGNOT, expr2);
			}
			return expr2 ?? _binder.bindUDUnop((operatorKind == OperatorKind.OP_TRUE) ? ExpressionKind.True : ExpressionKind.False, expr) ?? _binder.mustConvert(expr, SymbolLoader.GetPredefindType(PredefinedType.PT_BOOL));
		}
		return _binder.BindStandardUnaryOperator(operatorKind, expr);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindBinaryOperation(CSharpBinaryOperationBinder payload, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		ExpressionKind expressionKind = Operators.GetExpressionKind(GetOperatorKind(payload.Operation, payload.IsLogicalOperation));
		Expr expr = CreateArgumentEXPR(arguments[0], locals[0]);
		Expr expr2 = CreateArgumentEXPR(arguments[1], locals[1]);
		expr.ErrorString = Operators.GetDisplayName(GetOperatorKind(payload.Operation, payload.IsLogicalOperation));
		expr2.ErrorString = Operators.GetDisplayName(GetOperatorKind(payload.Operation, payload.IsLogicalOperation));
		if (expressionKind > ExpressionKind.MultiOffset)
		{
			expressionKind -= 71;
		}
		return _binder.BindStandardBinop(expressionKind, expr, expr2);
	}

	private static OperatorKind GetOperatorKind(ExpressionType p)
	{
		return GetOperatorKind(p, bIsLogical: false);
	}

	private static OperatorKind GetOperatorKind(ExpressionType p, bool bIsLogical)
	{
		switch (p)
		{
		default:
			throw Error.InternalCompilerError();
		case ExpressionType.Add:
			return OperatorKind.OP_ADD;
		case ExpressionType.Subtract:
			return OperatorKind.OP_SUB;
		case ExpressionType.Multiply:
			return OperatorKind.OP_MUL;
		case ExpressionType.Divide:
			return OperatorKind.OP_DIV;
		case ExpressionType.Modulo:
			return OperatorKind.OP_MOD;
		case ExpressionType.LeftShift:
			return OperatorKind.OP_LSHIFT;
		case ExpressionType.RightShift:
			return OperatorKind.OP_RSHIFT;
		case ExpressionType.LessThan:
			return OperatorKind.OP_LT;
		case ExpressionType.GreaterThan:
			return OperatorKind.OP_GT;
		case ExpressionType.LessThanOrEqual:
			return OperatorKind.OP_LE;
		case ExpressionType.GreaterThanOrEqual:
			return OperatorKind.OP_GE;
		case ExpressionType.Equal:
			return OperatorKind.OP_EQ;
		case ExpressionType.NotEqual:
			return OperatorKind.OP_NEQ;
		case ExpressionType.And:
			if (!bIsLogical)
			{
				return OperatorKind.OP_BITAND;
			}
			return OperatorKind.OP_LOGAND;
		case ExpressionType.ExclusiveOr:
			return OperatorKind.OP_BITXOR;
		case ExpressionType.Or:
			if (!bIsLogical)
			{
				return OperatorKind.OP_BITOR;
			}
			return OperatorKind.OP_LOGOR;
		case ExpressionType.AddAssign:
			return OperatorKind.OP_ADDEQ;
		case ExpressionType.SubtractAssign:
			return OperatorKind.OP_SUBEQ;
		case ExpressionType.MultiplyAssign:
			return OperatorKind.OP_MULEQ;
		case ExpressionType.DivideAssign:
			return OperatorKind.OP_DIVEQ;
		case ExpressionType.ModuloAssign:
			return OperatorKind.OP_MODEQ;
		case ExpressionType.AndAssign:
			return OperatorKind.OP_ANDEQ;
		case ExpressionType.ExclusiveOrAssign:
			return OperatorKind.OP_XOREQ;
		case ExpressionType.OrAssign:
			return OperatorKind.OP_OREQ;
		case ExpressionType.LeftShiftAssign:
			return OperatorKind.OP_LSHIFTEQ;
		case ExpressionType.RightShiftAssign:
			return OperatorKind.OP_RSHIFTEQ;
		case ExpressionType.Negate:
			return OperatorKind.OP_NEG;
		case ExpressionType.UnaryPlus:
			return OperatorKind.OP_UPLUS;
		case ExpressionType.Not:
			return OperatorKind.OP_LOGNOT;
		case ExpressionType.OnesComplement:
			return OperatorKind.OP_BITNOT;
		case ExpressionType.IsTrue:
			return OperatorKind.OP_TRUE;
		case ExpressionType.IsFalse:
			return OperatorKind.OP_FALSE;
		case ExpressionType.Increment:
			return OperatorKind.OP_PREINC;
		case ExpressionType.Decrement:
			return OperatorKind.OP_PREDEC;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindProperty(ICSharpBinder payload, ArgumentObject argument, LocalVariableSymbol local, Expr optionalIndexerArguments)
	{
		Expr expr = (argument.Info.IsStaticType ? ExprFactory.CreateClass(SymbolTable.GetCTypeFromType(argument.Value as Type)) : CreateLocal(argument.Type, argument.Info.IsOut, local));
		if (!argument.Info.UseCompileTimeType && argument.Value == null)
		{
			throw Error.NullReferenceOnMemberException();
		}
		if (argument.Type.IsValueType && expr is ExprCast)
		{
			expr.Flags |= EXPRFLAG.EXF_USERCALLABLE;
		}
		string name = payload.Name;
		BindingFlag bindingFlags = payload.BindingFlags;
		MemberLookup memberLookup = new MemberLookup();
		SymWithType symWithType = SymbolTable.LookupMember(name, expr, _binder.Context.ContextForMemberLookup, 0, memberLookup, allowSpecialNames: false, requireInvocable: false);
		if (symWithType == null)
		{
			if (optionalIndexerArguments != null)
			{
				int num = ExpressionIterator.Count(optionalIndexerArguments);
				Type type = argument.Type;
				if (type.IsArray)
				{
					if (type.IsArray && type.GetArrayRank() != num)
					{
						throw ErrorHandling.Error(ErrorCode.ERR_BadIndexCount, type.GetArrayRank());
					}
					return CreateArray(expr, optionalIndexerArguments);
				}
			}
			throw memberLookup.ReportErrors();
		}
		switch (symWithType.Sym.getKind())
		{
		case SYMKIND.SK_MethodSymbol:
			throw Error.BindPropertyFailedMethodGroup(name);
		case SYMKIND.SK_PropertySymbol:
			if (symWithType.Sym is IndexerSymbol)
			{
				return CreateIndexer(symWithType, expr, optionalIndexerArguments, bindingFlags);
			}
			expr.Flags |= EXPRFLAG.EXF_LVALUE;
			return CreateProperty(symWithType, expr, payload.BindingFlags);
		case SYMKIND.SK_FieldSymbol:
			return CreateField(symWithType, expr);
		case SYMKIND.SK_EventSymbol:
			throw Error.BindPropertyFailedEvent(name);
		default:
			throw Error.InternalCompilerError();
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindImplicitConversion(ArgumentObject[] arguments, Type returnType, LocalVariableSymbol[] locals, bool bIsArrayCreationConversion)
	{
		SymbolTable.AddConversionsForType(returnType);
		Expr expr = CreateArgumentEXPR(arguments[0], locals[0]);
		CType cTypeFromType = SymbolTable.GetCTypeFromType(returnType);
		if (bIsArrayCreationConversion)
		{
			CType dest = _binder.ChooseArrayIndexType(expr);
			return _binder.mustCast(_binder.mustConvert(expr, dest), cTypeFromType, CONVERTTYPE.NOUDC | CONVERTTYPE.CHECKOVERFLOW);
		}
		return _binder.mustConvert(expr, cTypeFromType);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindExplicitConversion(ArgumentObject[] arguments, Type returnType, LocalVariableSymbol[] locals)
	{
		SymbolTable.AddConversionsForType(returnType);
		Expr expr = CreateArgumentEXPR(arguments[0], locals[0]);
		CType cTypeFromType = SymbolTable.GetCTypeFromType(returnType);
		return _binder.mustCast(expr, cTypeFromType);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindAssignment(ICSharpBinder payload, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		string name = payload.Name;
		Expr optionalIndexerArguments;
		bool isCompoundAssignment;
		if (payload is CSharpSetIndexBinder cSharpSetIndexBinder)
		{
			optionalIndexerArguments = CreateArgumentListEXPR(arguments, locals, 1, arguments.Length - 1);
			isCompoundAssignment = cSharpSetIndexBinder.IsCompoundAssignment;
		}
		else
		{
			optionalIndexerArguments = null;
			isCompoundAssignment = (payload as CSharpSetMemberBinder).IsCompoundAssignment;
		}
		SymbolTable.PopulateSymbolTableWithName(name, null, arguments[0].Type);
		Expr op = BindProperty(payload, arguments[0], locals[0], optionalIndexerArguments);
		int num = arguments.Length - 1;
		Expr op2 = CreateArgumentEXPR(arguments[num], locals[num]);
		return _binder.BindAssignment(op, op2, isCompoundAssignment);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal Expr BindIsEvent(CSharpIsEventBinder binder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		Expr callingObject = CreateLocal(arguments[0].Type, isOut: false, locals[0]);
		MemberLookup mem = new MemberLookup();
		AggregateType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_BOOL);
		bool value = false;
		if (arguments[0].Value == null)
		{
			throw Error.NullReferenceOnMemberException();
		}
		SymWithType symWithType = SymbolTable.LookupMember(binder.Name, callingObject, _binder.Context.ContextForMemberLookup, 0, mem, allowSpecialNames: false, requireInvocable: false);
		if (symWithType != null)
		{
			if (symWithType.Sym.getKind() == SYMKIND.SK_EventSymbol)
			{
				value = true;
			}
			else if (symWithType.Sym is FieldSymbol { isEvent: not false })
			{
				value = true;
			}
		}
		return ExprFactory.CreateConstant(predefindType, ConstVal.Get(value));
	}
}
