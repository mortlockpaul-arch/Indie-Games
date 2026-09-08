using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

internal interface ICSharpBinder
{
	bool IsBinderThatCanHaveRefReceiver { get; }

	BindingFlag BindingFlags { get; }

	string Name { get; }

	Type ReturnType { get; }

	CSharpArgumentInfo GetArgumentInfo(int index);

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments);

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals);

	int GetGetBinderEquivalenceHash();

	bool IsEquivalentTo(ICSharpBinder other);
}
