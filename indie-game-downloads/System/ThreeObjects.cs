using System.Runtime.CompilerServices;

namespace System;

[InlineArray(3)]
internal struct ThreeObjects
{
	private object _arg0;

	public ThreeObjects(object arg0, object arg1, object arg2)
	{
		this = default(ThreeObjects);
		this[0] = arg0;
		this[1] = arg1;
		this[2] = arg2;
	}
}
