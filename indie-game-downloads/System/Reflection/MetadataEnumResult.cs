using System.Runtime.CompilerServices;

namespace System.Reflection;

internal ref struct MetadataEnumResult
{
	[InlineArray(16)]
	internal struct SmallIntArray
	{
		public int e;
	}

	internal int _length;

	internal SmallIntArray _smallResult;

	internal int[] _largeResult;

	public int Length => _length;

	public int this[int index]
	{
		get
		{
			if (_largeResult != null)
			{
				return _largeResult[index];
			}
			return _smallResult[index];
		}
	}
}
