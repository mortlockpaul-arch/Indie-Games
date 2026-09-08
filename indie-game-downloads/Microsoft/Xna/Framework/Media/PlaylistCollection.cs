using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media;

public sealed class PlaylistCollection : IEnumerable<Playlist>, IEnumerable, IDisposable
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

	public Playlist this[int index]
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal PlaylistCollection()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<Playlist> GetEnumerator()
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
