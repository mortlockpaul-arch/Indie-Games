using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary.Diagnostics;

namespace XnaLibrary;

public class SceneManagerComponent : DrawableGameComponent
{
	private SpriteBatch spriteBatch;

	private TimeWatcher updateWatcher;

	private TimeWatcher drawWatcher;

	private LinkedList<GameScene> Scenes { get; set; }

	public int Count => Scenes.Count;

	public SceneManagerComponent(Game game)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		((DrawableGameComponent)this)._002Ector(game);
		Scenes = new LinkedList<GameScene>();
		updateWatcher = new TimeWatcher
		{
			Name = "updateWatcher",
			Color = Color.Yellow
		};
		drawWatcher = new TimeWatcher
		{
			Name = "drawWatcher",
			Color = Color.Red
		};
	}

	protected override void LoadContent()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((GameComponent)this).Game.GraphicsDevice);
		((DrawableGameComponent)this).LoadContent();
	}

	protected override void UnloadContent()
	{
		((DrawableGameComponent)this).UnloadContent();
	}

	public override void Update(GameTime gameTime)
	{
		if (Guide.IsVisible)
		{
			return;
		}
		if (Scenes.Count != 0)
		{
			if (Scenes.First.Value != null && !Scenes.First.Value.Initialized)
			{
				Scenes.First.Value.Initialize();
			}
			else if (Scenes.First.Value != null && !Scenes.First.Value.Disposed)
			{
				Scenes.First.Value.Update(gameTime);
			}
			else if (Scenes.First.Value != null && Scenes.First.Value.Disposed)
			{
				Scenes.RemoveFirst();
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		if (Scenes.First != null && Scenes.First.Value != null && Scenes.First.Value.Initialized && !Scenes.First.Value.Disposed)
		{
			Scenes.First.Value.Draw(gameTime, spriteBatch);
		}
		((DrawableGameComponent)this).Draw(gameTime);
	}

	public void AddScene(GameScene scene)
	{
		Scenes.AddLast(scene);
	}
}
