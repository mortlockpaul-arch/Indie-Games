using System;
using System.Collections;
using System.Collections.Generic;
using H;
using Z;
using l;

namespace E;

internal class _6 : IEnumerable<H.h>, IEnumerable
{
	public struct _00065h(l._7<Z._7> connections) : IEnumerator<H.h>, IDisposable, IEnumerator
	{
		private l._7<Z._7> a5h = connections;

		private int a5b = -1;

		private H.h a56 = null;

		public H.h Current => a56;

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			while (++a5b < a5h.Count)
			{
				if (!a5h.Elements[a5b].SlatedForRemoval)
				{
					a56 = a5h.Elements[a5b].Owner as H.h;
					if (a56 != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Reset()
		{
			a5b = -1;
			a56 = null;
		}
	}

	private l._7<Z._7> a5h;

	public _6(l._7<Z._7> connections)
	{
		a5h = connections;
	}

	public _00065h GetEnumerator()
	{
		return new _00065h(a5h);
	}

	IEnumerator<H.h> IEnumerable<H.h>.GetEnumerator()
	{
		return new _00065h(a5h);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new _00065h(a5h);
	}
}
