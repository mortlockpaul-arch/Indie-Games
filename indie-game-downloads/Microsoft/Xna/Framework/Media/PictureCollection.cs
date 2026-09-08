using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media;

public sealed class PictureCollection : IEnumerable<Picture>, IEnumerable, IDisposable
{
	public int Count
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsDisposed
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public Picture this[int index]
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal PictureCollection()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<Picture> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
