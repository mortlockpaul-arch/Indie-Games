using B;
using Microsoft.Xna.Framework.Content;

namespace V;

internal class B
{
	internal static void H_0005(ContentReader P_0)
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
		global::B.B._0002(array);
	}
}
