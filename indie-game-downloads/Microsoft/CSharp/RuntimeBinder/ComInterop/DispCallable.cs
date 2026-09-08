using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class DispCallable : IPseudoComObject
{
	public IDispatchComObject DispatchComObject { get; }

	public IDispatch DispatchObject => DispatchComObject.DispatchObject;

	public string MemberName { get; }

	public int DispId { get; }

	internal DispCallable(IDispatchComObject dispatch, string memberName, int dispId)
	{
		DispatchComObject = dispatch;
		MemberName = memberName;
		DispId = dispId;
	}

	public override string ToString()
	{
		return "<bound dispmethod " + MemberName + ">";
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	public DynamicMetaObject GetMetaObject(Expression parameter)
	{
		return new DispCallableMetaObject(parameter, this);
	}

	public override bool Equals(object obj)
	{
		if (obj is DispCallable dispCallable && dispCallable.DispatchComObject == DispatchComObject)
		{
			return dispCallable.DispId == DispId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return DispatchComObject.GetHashCode() ^ DispId;
	}
}
