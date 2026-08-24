using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomValue
{
	public Vector3 _position;

	public int X;

	public int Y;

	public int Z;

	public int state;

	public AtomValue(int xX, int xY, int xZ, int xState)
	{
		X = xX;
		Y = xY;
		Z = xZ;
		state = xState;
		_position = GridPoint.ToPosition(X, Y, Z);
	}

	public void Set(Atom oAtom)
	{
		X = oAtom.point.X;
		Y = oAtom.point.Y;
		Z = oAtom.point.Z;
		state = oAtom.state;
		_position = GridPoint.ToPosition(X, Y, Z);
	}

	public bool Compair(AtomValue oValue)
	{
		if (X == oValue.X && Y == oValue.Y && Z == oValue.Z)
		{
			return state == oValue.state;
		}
		return false;
	}

	public static AtomValue FromAtom(Atom oAtom)
	{
		return new AtomValue(oAtom.point.X, oAtom.point.Y, oAtom.point.Z, oAtom.state);
	}
}
