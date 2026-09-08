using System.Runtime.InteropServices;
using System.Threading;

namespace System.Transactions.DtcProxyShim;

internal static class OletxHelper
{
	internal static void Retry(Action action)
	{
		int num = 100;
		while (true)
		{
			try
			{
				action();
				break;
			}
			catch (COMException ex) when (ex.ErrorCode == -2147168232)
			{
				if (--num == 0)
				{
					throw;
				}
				Thread.Sleep(50);
			}
		}
	}
}
