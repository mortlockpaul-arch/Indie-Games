using Microsoft.Xna.Framework;
using y;

namespace s;

internal class X : s.B<y.v>
{
	public X()
		: base(new y.v())
	{
	}

	public X(y.v shape)
		: base(shape)
	{
	}

	public void Initialize(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{
		y.v shape = base.Shape;
		shape.collisionMargin = 0f;
		shape.a5a = y._0006.DoubleSided;
		shape.a5h = a;
		shape.a5b = b;
		shape.a56 = c;
	}

	public void CleanUp()
	{
		events.RemoveAllEvents();
	}
}
