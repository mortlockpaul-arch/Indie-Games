using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class SymFactory
{
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static Symbol NewBasicSymbol(SYMKIND kind, Name name, ParentSymbol parent)
	{
		Symbol symbol;
		switch (kind)
		{
		case SYMKIND.SK_NamespaceSymbol:
			symbol = new NamespaceSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_AggregateSymbol:
			symbol = new AggregateSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_TypeParameterSymbol:
			symbol = new TypeParameterSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_FieldSymbol:
			symbol = new FieldSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_LocalVariableSymbol:
			symbol = new LocalVariableSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_MethodSymbol:
			symbol = new MethodSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_PropertySymbol:
			symbol = new PropertySymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_EventSymbol:
			symbol = new EventSymbol();
			symbol.name = name;
			break;
		case SYMKIND.SK_Scope:
			symbol = new Scope();
			symbol.name = name;
			break;
		case SYMKIND.SK_IndexerSymbol:
			symbol = new IndexerSymbol();
			symbol.name = name;
			break;
		default:
			throw Error.InternalCompilerError();
		}
		symbol.setKind(kind);
		if (parent != null)
		{
			parent.AddToChildList(symbol);
			SymbolStore.InsertChild(parent, symbol);
		}
		return symbol;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static NamespaceSymbol CreateNamespace(Name name, NamespaceSymbol parent)
	{
		NamespaceSymbol obj = (NamespaceSymbol)NewBasicSymbol(SYMKIND.SK_NamespaceSymbol, name, parent);
		obj.SetAccess(ACCESS.ACC_PUBLIC);
		return obj;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateSymbol CreateAggregate(Name name, NamespaceOrAggregateSymbol parent)
	{
		AggregateSymbol obj = (AggregateSymbol)NewBasicSymbol(SYMKIND.SK_AggregateSymbol, name, parent);
		obj.name = name;
		obj.SetSealed(@sealed: false);
		obj.SetAccess(ACCESS.ACC_UNKNOWN);
		obj.SetIfaces(null);
		obj.SetIfacesAll(null);
		obj.SetTypeVars(null);
		return obj;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static FieldSymbol CreateMemberVar(Name name, AggregateSymbol parent)
	{
		return NewBasicSymbol(SYMKIND.SK_FieldSymbol, name, parent) as FieldSymbol;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static LocalVariableSymbol CreateLocalVar(Name name, Scope parent, CType type)
	{
		LocalVariableSymbol obj = (LocalVariableSymbol)NewBasicSymbol(SYMKIND.SK_LocalVariableSymbol, name, parent);
		obj.SetType(type);
		obj.SetAccess(ACCESS.ACC_UNKNOWN);
		obj.wrap = null;
		return obj;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static MethodSymbol CreateMethod(Name name, AggregateSymbol parent)
	{
		return NewBasicSymbol(SYMKIND.SK_MethodSymbol, name, parent) as MethodSymbol;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static PropertySymbol CreateProperty(Name name, AggregateSymbol parent)
	{
		return NewBasicSymbol(SYMKIND.SK_PropertySymbol, name, parent) as PropertySymbol;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static EventSymbol CreateEvent(Name name, AggregateSymbol parent)
	{
		return NewBasicSymbol(SYMKIND.SK_EventSymbol, name, parent) as EventSymbol;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeParameterSymbol CreateMethodTypeParameter(Name pName, MethodSymbol pParent, int index, int indexTotal)
	{
		TypeParameterSymbol obj = (TypeParameterSymbol)NewBasicSymbol(SYMKIND.SK_TypeParameterSymbol, pName, pParent);
		obj.SetIndexInOwnParameters(index);
		obj.SetIndexInTotalParameters(indexTotal);
		obj.SetIsMethodTypeParameter(b: true);
		obj.SetAccess(ACCESS.ACC_PRIVATE);
		return obj;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeParameterSymbol CreateClassTypeParameter(Name pName, AggregateSymbol pParent, int index, int indexTotal)
	{
		TypeParameterSymbol obj = (TypeParameterSymbol)NewBasicSymbol(SYMKIND.SK_TypeParameterSymbol, pName, pParent);
		obj.SetIndexInOwnParameters(index);
		obj.SetIndexInTotalParameters(indexTotal);
		obj.SetIsMethodTypeParameter(b: false);
		obj.SetAccess(ACCESS.ACC_PRIVATE);
		return obj;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static Scope CreateScope()
	{
		return (Scope)NewBasicSymbol(SYMKIND.SK_Scope, null, null);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static IndexerSymbol CreateIndexer(Name name, ParentSymbol parent)
	{
		IndexerSymbol obj = (IndexerSymbol)NewBasicSymbol(SYMKIND.SK_IndexerSymbol, name, parent);
		obj.setKind(SYMKIND.SK_PropertySymbol);
		obj.isOperator = true;
		return obj;
	}
}
