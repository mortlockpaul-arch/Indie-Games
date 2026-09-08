using _0018;
using Microsoft.Xna.Framework.Content;

namespace _7;

internal class _0018
{
	internal static void _3_0013(ContentReader P_0)
	{
		byte[] array = null;
		try
		{
			int count = P_0.ReadInt32();
			array = P_0.ReadBytes(count);
		}
		catch
		{
		}
		global::_0018._0018.L(array);
	}
}
