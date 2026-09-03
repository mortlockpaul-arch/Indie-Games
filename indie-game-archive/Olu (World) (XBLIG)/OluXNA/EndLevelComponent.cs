using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class EndLevelComponent : DrawableGameComponent
{
	public Vector2[] namePos;

	public Vector2[] scorePos;

	public Vector2 titlePos;

	public int level;

	public int scoreNumber;

	public float scoreCountdown;

	public float maxCountdown;

	public EndLevelComponent(Game game, int _level, int _scoreHighlight)
		: base(game)
	{
		namePos = (Vector2[])(object)new Vector2[10];
		scorePos = (Vector2[])(object)new Vector2[10];
		level = _level;
		scoreNumber = _scoreHighlight;
		maxCountdown = (scoreCountdown = 0.8f);
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (level >= 0)
		{
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector((float)BaseGame.WIDTH * 0.1f, (float)BaseGame.HEIGHT * 0.2f);
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector((float)BaseGame.WIDTH * 0.8f, 0f);
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))._002Ector(0f, (float)BaseGame.HEIGHT * 0.075f);
			for (int i = 0; i < 10; i++)
			{
				ref Vector2 reference = ref namePos[i];
				reference = val + (float)i * val3;
				ref Vector2 reference2 = ref scorePos[i];
				reference2 = val + val2 + (float)i * val3;
				ref Vector2 reference3 = ref scorePos[i];
				reference3.X -= BaseGame.Get().hud.HUDfont.MeasureString(BaseGame.Get().hiScores.topScores[level - 1][i].ToString()).X;
			}
			titlePos = new Vector2((float)BaseGame.WIDTH * 0.5f, (float)BaseGame.HEIGHT * 0.15f);
			titlePos -= BaseGame.Get().hud.HUDfont.MeasureString("HIGH SCORES") / 2f;
		}
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		BaseGame.Get().input.Update();
		BaseGame.Get().CheckAndResetRumble();
		if (BaseGame.Get().input.KeyPressed((Keys)13) || BaseGame.Get().input.PadPressed((Buttons)4096) || BaseGame.Get().input.PadPressed((Buttons)16) || level < 0)
		{
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new MainMenuComponent(((GameComponent)this).Game, 6));
		}
		scoreCountdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (scoreCountdown <= 0f)
		{
			scoreCountdown += maxCountdown;
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.Clear(Color.Black);
		if (level >= 0)
		{
			((Effect)BaseGame.Get().flatEffect).Begin();
			((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].Begin();
			BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
			Color darkGreen = default(Color);
			for (int i = 0; i < 10; i++)
			{
				if (i != scoreNumber)
				{
					darkGreen = Color.DarkGreen;
				}
				else
				{
					((Color)(ref darkGreen))._002Ector(Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.95f, 0.95f, 0.95f), 1f - maxCountdown + scoreCountdown));
				}
				BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, BaseGame.Get().hiScores.topNames[level - 1][i], namePos[i], darkGreen);
				BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, BaseGame.Get().hiScores.topScores[level - 1][i].ToString(), scorePos[i], darkGreen);
			}
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "HIGH SCORES", titlePos, Color.White);
			BaseGame.Get().spriteBatch.End();
			((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].End();
			((Effect)BaseGame.Get().flatEffect).End();
		}
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
