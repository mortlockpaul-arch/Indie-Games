using System;

namespace FarseerGames.FarseerPhysics.Interfaces;

public interface IIsDisposable : IDisposable
{
	bool IsDisposed { get; set; }
}
