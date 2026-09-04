using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;

namespace Infinity.Scenes;

public abstract class InitializeScene : AnaglyphScene
{
	private XSIModel model;

	protected TimeSpan loadingTime;

	protected TimeSpan workFadeTime;

	public event EventHandler InitializeFinished;

	public InitializeScene(Game game)
		: base(game)
	{
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		model = new XSIModel("Models/Models/screen/screen_loading", base.Content);
		model.Play(isLoop: true);
		InitializeFinished = (EventHandler)Delegate.Combine(InitializeFinished, (EventHandler)delegate
		{
			base.Fade.FadeTime = workFadeTime;
			FadeOut();
		});
		Thread thread = new Thread((ThreadStart)delegate
		{
			LoadContents();
			if (InitializeFinished != null)
			{
				InitializeFinished(this, EventArgs.Empty);
			}
		});
		thread.Start();
		workFadeTime = base.Fade.FadeTime;
		base.Fade.FadeTime = TimeSpan.Zero;
		loadingTime = TimeSpan.Zero;
		base.Initialize();
	}

	protected abstract void LoadContents();

	private void SceneUpdate(object sender, GameTime gameTime)
	{
		if (fadePhase != FadePhase.In && fadePhase != FadePhase.Main)
		{
			_ = fadePhase;
			_ = 2;
		}
		model.Update(gameTime);
		loadingTime += gameTime.ElapsedGameTime;
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		anaglyphRender.Draw(gameTime, base.SASData);
	}

	protected override void DrawScene(GameTime gameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		model.Draw(base.SASData, Matrix.Identity);
		base.DrawScene(gameTime);
	}
}
