using System;
using l;

namespace s;

internal class _0006
{
	private l.h<_7> a5h;

	private _6 a5b;

	public l.h<_7> Tree => a5h;

	public _6 Owner => a5b;

	public _0006(_6 owner)
	{
		a5b = owner;
		_7[] array = new _7[owner.a5h.a5h];
		Array.Copy(owner.a5h.Elements, array, owner.a5h.a5h);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CollisionInformation.worldTransform = owner.Shape.a5h.Elements[i].LocalTransform;
			array[i].CollisionInformation.UpdateBoundingBoxInternal(0f);
		}
		a5h = new l.h<_7>(array);
	}
}
