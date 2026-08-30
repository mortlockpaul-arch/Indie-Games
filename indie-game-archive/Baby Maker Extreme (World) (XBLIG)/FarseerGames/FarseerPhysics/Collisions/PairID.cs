using System.Runtime.InteropServices;

namespace FarseerGames.FarseerPhysics.Collisions;

[StructLayout(LayoutKind.Explicit)]
public struct PairID
{
	[FieldOffset(0)]
	private long ID;

	[FieldOffset(0)]
	private int lowID;

	[FieldOffset(4)]
	private int highID;

	public static long GetId(int id1, int id2)
	{
		PairID pairID = default(PairID);
		pairID.ID = 0L;
		if (id1 > id2)
		{
			pairID.lowID = id2;
			pairID.highID = id1;
		}
		else
		{
			pairID.lowID = id1;
			pairID.highID = id2;
		}
		return pairID.ID;
	}

	public static long GetHash(int value1, int value2)
	{
		PairID pairID = default(PairID);
		pairID.ID = 0L;
		pairID.lowID = value1;
		pairID.highID = value2;
		return pairID.ID;
	}

	public static void GetIds(long id, out int id1, out int id2)
	{
		PairID pairID = default(PairID);
		pairID.lowID = 0;
		pairID.highID = 0;
		pairID.ID = id;
		id1 = pairID.lowID;
		id2 = pairID.highID;
	}
}
