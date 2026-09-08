using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class SplatCallSite
{
	public delegate object InvokeDelegate(object[] args);

	internal readonly object _callable;

	private CallSite<Func<CallSite, object, object[], object>> _site;

	internal SplatCallSite(object callable)
	{
		_callable = callable;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal object Invoke(object[] args)
	{
		if (_site == null)
		{
			_site = CallSite<Func<CallSite, object, object[], object>>.Create(SplatInvokeBinder.Instance);
		}
		return _site.Target(_site, _callable, args);
	}
}
