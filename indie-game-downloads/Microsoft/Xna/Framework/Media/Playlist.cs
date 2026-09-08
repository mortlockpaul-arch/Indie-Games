using System;

namespace Microsoft.Xna.Framework.Media;

public sealed class Playlist : IEquatable<Playlist>, IDisposable
{
	public TimeSpan Duration
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

	public string Name
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public SongCollection Songs
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal Playlist()
	{
		throw new NotImplementedException();
	}

	public bool Equals(Playlist other)
	{
		throw new NotImplementedException();
	}

	public override bool Equals(object obj)
	{
		throw new NotImplementedException();
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

	public static bool operator ==(Playlist first, Playlist second)
	{
		return first?.Equals(second) ?? ((object)first == second);
	}

	public static bool operator !=(Playlist first, Playlist second)
	{
		return !(first == second);
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		throw new NotImplementedException();
	}
}
