using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class ComEventSinksContainer : List<System.Runtime.InteropServices.ComEventsSink>, IDisposable
{
	private static readonly object s_comObjectEventSinksKey = new object();

	private ComEventSinksContainer()
	{
	}

	public static ComEventSinksContainer FromRuntimeCallableWrapper(object rcw, bool createIfNotFound)
	{
		object comObjectData = Marshal.GetComObjectData(rcw, s_comObjectEventSinksKey);
		if (comObjectData != null || !createIfNotFound)
		{
			return (ComEventSinksContainer)comObjectData;
		}
		lock (s_comObjectEventSinksKey)
		{
			comObjectData = Marshal.GetComObjectData(rcw, s_comObjectEventSinksKey);
			if (comObjectData != null)
			{
				return (ComEventSinksContainer)comObjectData;
			}
			ComEventSinksContainer comEventSinksContainer = new ComEventSinksContainer();
			if (!Marshal.SetComObjectData(rcw, s_comObjectEventSinksKey, comEventSinksContainer))
			{
				throw Error.SetComObjectDataFailed();
			}
			return comEventSinksContainer;
		}
	}

	public void Dispose()
	{
		DisposeAll();
		GC.SuppressFinalize(this);
	}

	private void DisposeAll()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			System.Runtime.InteropServices.ComEventsSink.RemoveAll(enumerator.Current);
		}
	}

	~ComEventSinksContainer()
	{
		DisposeAll();
	}
}
