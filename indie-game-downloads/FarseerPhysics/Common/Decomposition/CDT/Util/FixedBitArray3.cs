using System;
using System.Collections;
using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Util;

public struct FixedBitArray3 : IEnumerable<bool>, IEnumerable
{
	public bool _0;

	public bool _1;

	public bool _2;

	public bool this[int index]
	{
		get
		{
			return index switch
			{
				0 => _0, 
				1 => _1, 
				2 => _2, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				_0 = value;
				break;
			case 1:
				_1 = value;
				break;
			case 2:
				_2 = value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	public IEnumerator<bool> GetEnumerator()
	{
		return Enumerate().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Clear()
	{
		_0 = (_1 = (_2 = false));
	}

	private IEnumerable<bool> Enumerate()
	{
		for (int i = 0; i < 3; i++)
		{
			yield return this[i];
		}
	}
}
