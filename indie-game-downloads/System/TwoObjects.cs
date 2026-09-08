using System.Runtime.CompilerServices;

namespace System;

[InlineArray(2)]
internal struct TwoObjects
{
	private object _arg0;

	public TwoObjects(object arg0, object arg1)
	{
		this = default(TwoObjects);
		this[0] = arg0;
		this[1] = arg1;
	}
}
