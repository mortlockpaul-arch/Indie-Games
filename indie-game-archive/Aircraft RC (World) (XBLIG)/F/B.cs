using System;

namespace F;

internal class B
{
	internal enum _0001CB
	{
		Note,
		Warning,
		Error
	}

	internal static void _79(string P_0, _0001CB P_1)
	{
		switch (P_1)
		{
		}
	}

	internal static void _7_0004<T>(ref T P_0) where T : IDisposable
	{
		if (P_0 != null)
		{
			P_0.Dispose();
			P_0 = default(T);
		}
	}
}
