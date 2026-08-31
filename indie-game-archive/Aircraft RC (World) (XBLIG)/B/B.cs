using System;
using System.Diagnostics;

namespace B;

internal class B
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static uint HCB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool HC_0002 = false;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static H HC_0012;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static string HCH = "";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool HC7 = false;

	[DebuggerHidden]
	private static void B()
	{
		if (HC_0012 == null)
		{
			HCB = global::B._0002._0001();
			HC_0012 = new H();
		}
	}

	[DebuggerHidden]
	internal static void _0002(byte[] P_0)
	{
		B();
		if (!HC_0012.D(P_0))
		{
			throw new Exception("Product not activated, please run activation tool.");
		}
	}

	[DebuggerHidden]
	internal static void _0012()
	{
		B();
		if (!HC7)
		{
			HC7 = true;
			HC_0002 = false;
			HC_0002 = HC_0012.K(global::B._0012.HC_0002[0].FileName, global::B._0012.HC_0002[0].DRMProductName, HCB);
		}
		if (!HC_0002)
		{
			throw new Exception("Product not activated, please run activation tool.");
		}
	}

	[DebuggerHidden]
	internal static void H()
	{
		B();
		if (!HC7)
		{
			HC7 = true;
			HC_0002 = false;
			HC_0002 = HC_0012.K(global::B._0012.HC_0002[0].FileName, global::B._0012.HC_0002[0].DRMProductName, HCB);
		}
		if (!HC_0002)
		{
			throw new Exception("Product not activated, please run activation tool.");
		}
	}

	[DebuggerHidden]
	internal static string _7()
	{
		return global::B._0012.ActivationPath + global::B._0012.HC_0002[0].FileName;
	}
}
