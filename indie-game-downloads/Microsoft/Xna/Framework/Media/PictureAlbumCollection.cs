using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media;

public sealed class PictureAlbumCollection : IEnumerable<PictureAlbum>, IEnumerable, IDisposable
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

	public PictureAlbum this[int index]
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal PictureAlbumCollection()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<PictureAlbum> GetEnumerator()
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
