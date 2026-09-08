using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media;

public sealed class AlbumCollection : IEnumerable<Album>, IEnumerable, IDisposable
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

	public Album this[int index]
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal AlbumCollection()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<Album> GetEnumerator()
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
