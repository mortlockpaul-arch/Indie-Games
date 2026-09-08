using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Microsoft.CSharp.RuntimeBinder.Semantics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal struct UserStringBuilder
{
	private StringBuilder _strBuilder;

	private void BeginString()
	{
		if (_strBuilder == null)
		{
			_strBuilder = new StringBuilder();
		}
	}

	private string EndString()
	{
		string result = _strBuilder.ToString();
		_strBuilder.Clear();
		return result;
	}

	private static string ErrSK(SYMKIND sk)
	{
		return ErrId(sk switch
		{
			SYMKIND.SK_MethodSymbol => MessageID.SK_METHOD, 
			SYMKIND.SK_AggregateSymbol => MessageID.SK_CLASS, 
			SYMKIND.SK_NamespaceSymbol => MessageID.SK_NAMESPACE, 
			SYMKIND.SK_FieldSymbol => MessageID.SK_FIELD, 
			SYMKIND.SK_LocalVariableSymbol => MessageID.SK_VARIABLE, 
			SYMKIND.SK_PropertySymbol => MessageID.SK_PROPERTY, 
			SYMKIND.SK_EventSymbol => MessageID.SK_EVENT, 
			SYMKIND.SK_TypeParameterSymbol => MessageID.SK_TYVAR, 
			_ => MessageID.SK_UNKNOWN, 
		});
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendParamList(TypeArray @params, bool isParamArray)
	{
		if (@params == null)
		{
			return;
		}
		for (int i = 0; i < @params.Count; i++)
		{
			if (i > 0)
			{
				ErrAppendString(", ");
			}
			if (isParamArray && i == @params.Count - 1)
			{
				ErrAppendString("params ");
			}
			ErrAppendType(@params[i], null);
		}
	}

	private void ErrAppendString(string str)
	{
		_strBuilder.Append(str);
	}

	private void ErrAppendChar(char ch)
	{
		_strBuilder.Append(ch);
	}

	private void ErrAppendPrintf(string format, params object[] args)
	{
		ErrAppendString(string.Format(CultureInfo.InvariantCulture, format, args));
	}

	private void ErrAppendName(Name name)
	{
		if (name == NameManager.GetPredefinedName(PredefinedName.PN_INDEXERINTERNAL))
		{
			ErrAppendString("this");
		}
		else
		{
			ErrAppendString(name.Text);
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendParentSym(Symbol sym, SubstContext pctx)
	{
		ErrAppendParentCore(sym.parent, pctx);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendParentCore(Symbol parent, SubstContext pctx)
	{
		if (parent != null && parent != NamespaceSymbol.Root)
		{
			if (pctx != null && !pctx.IsNop && parent is AggregateSymbol aggregateSymbol && aggregateSymbol.GetTypeVarsAll().Count != 0)
			{
				CType pType = TypeManager.SubstType(aggregateSymbol.getThisType(), pctx);
				ErrAppendType(pType, null);
			}
			else
			{
				ErrAppendSym(parent, null);
			}
			ErrAppendChar('.');
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendTypeParameters(TypeArray @params, SubstContext pctx)
	{
		if (@params != null && @params.Count != 0)
		{
			ErrAppendChar('<');
			ErrAppendType(@params[0], pctx);
			for (int i = 1; i < @params.Count; i++)
			{
				ErrAppendString(",");
				ErrAppendType(@params[i], pctx);
			}
			ErrAppendChar('>');
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendMethod(MethodSymbol meth, SubstContext pctx, bool fArgs)
	{
		if (meth.IsExpImpl() && (bool)meth.swtSlot)
		{
			ErrAppendParentSym(meth, pctx);
			SubstContext pctx2 = new SubstContext(TypeManager.SubstType(meth.swtSlot.GetType(), pctx));
			ErrAppendSym(meth.swtSlot.Sym, pctx2, fArgs);
			return;
		}
		MethodKindEnum methKind = meth.MethKind;
		switch (methKind)
		{
		case MethodKindEnum.PropAccessor:
		{
			PropertySymbol property = meth.getProperty();
			ErrAppendSym(property, pctx);
			if (property.GetterMethod == meth)
			{
				ErrAppendString(".get");
			}
			else
			{
				ErrAppendString(".set");
			}
			return;
		}
		case MethodKindEnum.EventAccessor:
		{
			EventSymbol eventSymbol = meth.getEvent();
			ErrAppendSym(eventSymbol, pctx);
			if (eventSymbol.methAdd == meth)
			{
				ErrAppendString(".add");
			}
			else
			{
				ErrAppendString(".remove");
			}
			return;
		}
		}
		ErrAppendParentSym(meth, pctx);
		switch (methKind)
		{
		case MethodKindEnum.Constructor:
			ErrAppendName(meth.getClass().name);
			break;
		case MethodKindEnum.Destructor:
			ErrAppendChar('~');
			goto case MethodKindEnum.Constructor;
		case MethodKindEnum.ExplicitConv:
			ErrAppendString("explicit");
			goto IL_0118;
		case MethodKindEnum.ImplicitConv:
			ErrAppendString("implicit");
			goto IL_0118;
		default:
			{
				if (meth.isOperator)
				{
					ErrAppendString("operator ");
					ErrAppendString(Operators.OperatorOfMethodName(meth.name));
				}
				else if (!meth.IsExpImpl())
				{
					ErrAppendName(meth.name);
				}
				break;
			}
			IL_0118:
			ErrAppendString(" operator ");
			ErrAppendType(meth.RetType, pctx);
			break;
		}
		ErrAppendTypeParameters(meth.typeVars, pctx);
		if (fArgs)
		{
			ErrAppendChar('(');
			ErrAppendParamList(TypeManager.SubstTypeArray(meth.Params, pctx), meth.isParamArray);
			ErrAppendChar(')');
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendIndexer(IndexerSymbol indexer, SubstContext pctx)
	{
		ErrAppendString("this[");
		ErrAppendParamList(TypeManager.SubstTypeArray(indexer.Params, pctx), indexer.isParamArray);
		ErrAppendChar(']');
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendProperty(PropertySymbol prop, SubstContext pctx)
	{
		ErrAppendParentSym(prop, pctx);
		if (prop.IsExpImpl())
		{
			if (prop.swtSlot.Sym != null)
			{
				SubstContext pctx2 = new SubstContext(TypeManager.SubstType(prop.swtSlot.GetType(), pctx));
				ErrAppendSym(prop.swtSlot.Sym, pctx2);
			}
			else if (prop is IndexerSymbol indexer)
			{
				ErrAppendChar('.');
				ErrAppendIndexer(indexer, pctx);
			}
		}
		else if (prop is IndexerSymbol indexer2)
		{
			ErrAppendIndexer(indexer2, pctx);
		}
		else
		{
			ErrAppendName(prop.name);
		}
	}

	private void ErrAppendId(MessageID id)
	{
		ErrAppendString(ErrId(id));
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendSym(Symbol sym, SubstContext pctx)
	{
		ErrAppendSym(sym, pctx, fArgs: true);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendSym(Symbol sym, SubstContext pctx, bool fArgs)
	{
		switch (sym.getKind())
		{
		case SYMKIND.SK_AggregateSymbol:
		{
			string niceName = PredefinedTypes.GetNiceName(sym as AggregateSymbol);
			if (niceName != null)
			{
				ErrAppendString(niceName);
				break;
			}
			ErrAppendParentSym(sym, pctx);
			ErrAppendName(sym.name);
			ErrAppendTypeParameters(((AggregateSymbol)sym).GetTypeVars(), pctx);
			break;
		}
		case SYMKIND.SK_MethodSymbol:
			ErrAppendMethod((MethodSymbol)sym, pctx, fArgs);
			break;
		case SYMKIND.SK_PropertySymbol:
			ErrAppendProperty((PropertySymbol)sym, pctx);
			break;
		case SYMKIND.SK_NamespaceSymbol:
			if (sym == NamespaceSymbol.Root)
			{
				ErrAppendId(MessageID.GlobalNamespace);
				break;
			}
			ErrAppendParentSym(sym, null);
			ErrAppendName(sym.name);
			break;
		case SYMKIND.SK_FieldSymbol:
			ErrAppendParentSym(sym, pctx);
			ErrAppendName(sym.name);
			break;
		case SYMKIND.SK_TypeParameterSymbol:
			if (sym.name == null)
			{
				TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)sym;
				if (typeParameterSymbol.IsMethodTypeParameter())
				{
					ErrAppendChar('!');
				}
				ErrAppendChar('!');
				ErrAppendPrintf("{0}", typeParameterSymbol.GetIndexInTotalParameters());
			}
			else
			{
				ErrAppendName(sym.name);
			}
			break;
		case SYMKIND.SK_LocalVariableSymbol:
			ErrAppendName(sym.name);
			break;
		case SYMKIND.SK_EventSymbol:
			break;
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private void ErrAppendType(CType pType, SubstContext pctx)
	{
		if (pctx != null && !pctx.IsNop)
		{
			pType = TypeManager.SubstType(pType, pctx);
		}
		switch (pType.TypeKind)
		{
		case TypeKind.TK_AggregateType:
		{
			AggregateType aggregateType = (AggregateType)pType;
			string niceName = PredefinedTypes.GetNiceName(aggregateType.OwningAggregate);
			if (niceName != null)
			{
				ErrAppendString(niceName);
			}
			else
			{
				if (aggregateType.OuterType != null)
				{
					ErrAppendType(aggregateType.OuterType, null);
					ErrAppendChar('.');
				}
				else
				{
					ErrAppendParentSym(aggregateType.OwningAggregate, null);
				}
				ErrAppendName(aggregateType.OwningAggregate.name);
			}
			ErrAppendTypeParameters(aggregateType.TypeArgsThis, null);
			break;
		}
		case TypeKind.TK_TypeParameterType:
		{
			TypeParameterType typeParameterType = (TypeParameterType)pType;
			if (typeParameterType.Name == null)
			{
				if (typeParameterType.IsMethodTypeParameter)
				{
					ErrAppendChar('!');
				}
				ErrAppendChar('!');
				ErrAppendPrintf("{0}", typeParameterType.IndexInTotalParameters);
			}
			else
			{
				ErrAppendName(typeParameterType.Name);
			}
			break;
		}
		case TypeKind.TK_NullType:
			ErrAppendId(MessageID.NULL);
			break;
		case TypeKind.TK_MethodGroupType:
			ErrAppendId(MessageID.MethodGroup);
			break;
		case TypeKind.TK_ArgumentListType:
			ErrAppendString(TokenFacts.GetText(TokenKind.ArgList));
			break;
		case TypeKind.TK_ArrayType:
		{
			CType baseElementType = ((ArrayType)pType).BaseElementType;
			ErrAppendType(baseElementType, null);
			for (baseElementType = pType; baseElementType is ArrayType { Rank: var rank } arrayType; baseElementType = arrayType.ElementType)
			{
				ErrAppendChar('[');
				if (rank == 1)
				{
					if (!arrayType.IsSZArray)
					{
						ErrAppendChar('*');
					}
				}
				else
				{
					for (int num = rank; num > 1; num--)
					{
						ErrAppendChar(',');
					}
				}
				ErrAppendChar(']');
			}
			break;
		}
		case TypeKind.TK_VoidType:
			ErrAppendName(NameManager.GetPredefinedName(PredefinedName.PN_VOID));
			break;
		case TypeKind.TK_ParameterModifierType:
		{
			ParameterModifierType parameterModifierType = (ParameterModifierType)pType;
			ErrAppendString(parameterModifierType.IsOut ? "out " : "ref ");
			ErrAppendType(parameterModifierType.ParameterType, null);
			break;
		}
		case TypeKind.TK_PointerType:
			ErrAppendType(((PointerType)pType).ReferentType, null);
			ErrAppendChar('*');
			break;
		case TypeKind.TK_NullableType:
			ErrAppendType(((NullableType)pType).UnderlyingType, null);
			ErrAppendChar('?');
			break;
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public bool ErrArgToString(out string psz, ErrArg parg, out bool fUserStrings)
	{
		fUserStrings = false;
		psz = null;
		bool result = true;
		switch (parg.eak)
		{
		case ErrArgKind.SymKind:
			psz = ErrSK(parg.sk);
			break;
		case ErrArgKind.Type:
			BeginString();
			ErrAppendType(parg.pType, null);
			psz = EndString();
			fUserStrings = true;
			break;
		case ErrArgKind.Sym:
			BeginString();
			ErrAppendSym(parg.sym, null);
			psz = EndString();
			fUserStrings = true;
			break;
		case ErrArgKind.Name:
			if (parg.name == NameManager.GetPredefinedName(PredefinedName.PN_INDEXERINTERNAL))
			{
				psz = "this";
			}
			else
			{
				psz = parg.name.Text;
			}
			break;
		case ErrArgKind.Str:
			psz = parg.psz;
			break;
		case ErrArgKind.SymWithType:
		{
			SubstContext pctx2 = new SubstContext(parg.swtMemo.ats, null);
			BeginString();
			ErrAppendSym(parg.swtMemo.sym, pctx2, fArgs: true);
			psz = EndString();
			fUserStrings = true;
			break;
		}
		case ErrArgKind.MethWithInst:
		{
			SubstContext pctx = new SubstContext(parg.mpwiMemo.ats, parg.mpwiMemo.typeArgs);
			BeginString();
			ErrAppendSym(parg.mpwiMemo.sym, pctx, fArgs: true);
			psz = EndString();
			fUserStrings = true;
			break;
		}
		default:
			result = false;
			break;
		}
		return result;
	}

	private static string ErrId(MessageID id)
	{
		return ErrorFacts.GetMessage(id);
	}
}
