using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.Render;

namespace Quasar;

public abstract class Game : IDisposable
{
	protected bool finished;

	protected List<RenderProcess> renderProcesses = new List<RenderProcess>();

	private Microsoft.Xna.Framework.Game xnaGame;

	public bool Finished => finished;

	public List<RenderProcess> RenderProcesses => renderProcesses;

	protected Microsoft.Xna.Framework.Game XNAGame => xnaGame;

	public Game()
	{
	}

	public Game(Microsoft.Xna.Framework.Game g)
	{
		xnaGame = g;
	}

	public void SetXNAGame(Microsoft.Xna.Framework.Game g)
	{
		xnaGame = g;
	}

	public abstract void MainLoop();

	public abstract void InitGame();

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public abstract void ErrorFound(Exception ex);

	~Game()
	{
		Dispose();
	}
}
