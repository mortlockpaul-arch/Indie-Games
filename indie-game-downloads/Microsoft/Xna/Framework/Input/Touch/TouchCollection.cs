using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input.Touch;

public struct TouchCollection : IList<TouchLocation>, ICollection<TouchLocation>, IEnumerable<TouchLocation>, IEnumerable
{
	public struct Enumerator : IEnumerator<TouchLocation>, IEnumerator, IDisposable
	{
		private TouchCollection collection;

		private int position;

		public TouchLocation Current => collection[position];

		object IEnumerator.Current => collection[position];

		internal Enumerator(TouchCollection collection)
		{
			this.collection = collection;
			position = -1;
		}

		public bool MoveNext()
		{
			position++;
			return position < collection.Count;
		}

		public void Dispose()
		{
		}

		void IEnumerator.Reset()
		{
			position = -1;
		}
	}

	private readonly List<TouchLocation> touches;

	public int Count
	{
		get
		{
			if (touches == null)
			{
				return 0;
			}
			return touches.Count;
		}
	}

	public bool IsConnected => TouchPanel.TouchDeviceExists;

	public bool IsReadOnly => true;

	public TouchLocation this[int index]
	{
		get
		{
			if (touches == null)
			{
				throw new ArgumentOutOfRangeException();
			}
			return touches[index];
		}
		set
		{
			touches[index] = value;
		}
	}

	public TouchCollection(TouchLocation[] touches)
	{
		this.touches = new List<TouchLocation>(touches);
	}

	public void Add(TouchLocation item)
	{
		touches.Add(item);
	}

	public void Clear()
	{
		touches.Clear();
	}

	public bool Contains(TouchLocation item)
	{
		if (touches == null)
		{
			return false;
		}
		return touches.Contains(item);
	}

	public void CopyTo(TouchLocation[] array, int arrayIndex)
	{
		if (touches != null)
		{
			touches.CopyTo(array, arrayIndex);
		}
	}

	public bool FindById(int id, out TouchLocation touchLocation)
	{
		if (touches != null)
		{
			foreach (TouchLocation touch in touches)
			{
				if (touch.Id == id)
				{
					touchLocation = touch;
					return true;
				}
			}
		}
		touchLocation = new TouchLocation(-1, TouchLocationState.Invalid, Vector2.Zero);
		return false;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public int IndexOf(TouchLocation item)
	{
		if (touches == null)
		{
			return -1;
		}
		return touches.IndexOf(item);
	}

	public void Insert(int index, TouchLocation item)
	{
		touches.Insert(index, item);
	}

	public bool Remove(TouchLocation item)
	{
		return touches.Remove(item);
	}

	public void RemoveAt(int index)
	{
		touches.RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<TouchLocation> IEnumerable<TouchLocation>.GetEnumerator()
	{
		return new Enumerator(this);
	}
}
