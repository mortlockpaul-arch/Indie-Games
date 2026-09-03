using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

public class Game1 : Game
{
	public Thread gamerThread;

	public List<Vector3> someList;

	public Game1()
	{
		((Game)this).IsFixedTimeStep = true;
		((Game)this).TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000f * BaseGame.frameRat));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)new LoadComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)new RumbleComponent((Game)(object)this));
		ThreadStart start = delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		};
		gamerThread = new Thread(start);
		gamerThread.Start();
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 1 });
	}

	protected override void Initialize()
	{
		((Game)this).Initialize();
	}

	protected override void LoadContent()
	{
	}

	protected override void UnloadContent()
	{
		BaseGame.Get().content.Unload();
	}

	protected override void Update(GameTime gameTime)
	{
		if (((Collection<IGameComponent>)(object)((Game)this).Components).Count == 0)
		{
			((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)new MainMenuComponent((Game)(object)this, 2));
		}
		((Game)this).Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		((Game)this).Draw(gameTime);
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		if (BaseGame.Get().loadThread != null)
		{
			BaseGame.Get().loadThread.Join();
			BaseGame.Get().loadThread = null;
		}
		((Game)this).OnExiting(sender, args);
	}

	public VertexPositionColor ConvertVect3ToVertPosColor(Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return new VertexPositionColor(v, Color.White);
	}
}
