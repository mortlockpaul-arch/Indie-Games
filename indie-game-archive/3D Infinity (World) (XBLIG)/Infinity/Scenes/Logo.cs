using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;

namespace Infinity.Scenes;

public class Logo : AnaglyphScene
{
	private XSIModel screenModel;

	public Logo(Game game)
		: base(game)
	{
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		screenModel = new XSIModel("Models/Models/screen/screen_smilelogo", base.Content);
		screenModel.Finished += delegate
		{
			base.SceneManager.AddScene(new Title(base.Game));
			FadeOut();
		};
		screenModel.Play();
		base.Sound.PlaySE("SE15");
		base.Initialize();
	}

	public override void Dispose()
	{
		base.Content.Unload();
		base.Dispose();
	}

	private void SceneUpdate(object sender, GameTime gameTime)
	{
		if (fadePhase != FadePhase.In)
		{
			if (fadePhase == FadePhase.Main)
			{
				UpdateMain(gameTime);
			}
			else
			{
				_ = fadePhase;
				_ = 2;
			}
		}
		UpdateModels(gameTime);
	}

	private void UpdateModels(GameTime gameTime)
	{
		screenModel.Update(gameTime);
	}

	private void UpdateMain(GameTime gameTime)
	{
	}

	protected override void FadeInFinished(object sender, EventArgs e)
	{
		base.FadeInFinished(sender, e);
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		anaglyphRender.Draw(gameTime, base.SASData);
	}

	protected override void DrawScene(GameTime gameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		screenModel.Draw(base.SASData, Matrix.Identity);
		base.DrawScene(gameTime);
	}
}
