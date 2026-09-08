using System.Runtime.CompilerServices;
using _000E;
using Microsoft.Xna.Framework;
using T;
using Y;

namespace s;

internal class _7 : Y._6
{
	private _000E.a a5h;

	internal int a5b;

	private b a56;

	[CompilerGenerated]
	private T._6 a5a;

	public int ShapeIndex => a5b;

	public b CollisionInformation => a56;

	public T._6 Material
	{
		[CompilerGenerated]
		get
		{
			return a5a;
		}
		[CompilerGenerated]
		set
		{
			a5a = value;
		}
	}

	public _000E._6 Entry => a5h.a5h.Elements[a5b];

	public BoundingBox BoundingBox => a56.boundingBox;

	internal _7(_000E.a P_0, b P_1, T._6 P_2, int P_3)
	{
		a5h = P_0;
		a56 = P_1;
		Material = P_2;
		a5b = P_3;
	}

	internal _7(_000E.a P_0, b P_1, int P_2)
	{
		a5h = P_0;
		a56 = P_1;
		a5b = P_2;
	}
}
