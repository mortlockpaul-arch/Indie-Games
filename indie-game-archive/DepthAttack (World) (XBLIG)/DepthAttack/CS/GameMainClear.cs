using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DepthAttack.CS;

public class GameMainClear(Game game) : DrawableGameComponent(game)
{
	public enum enuGameScene
	{
		cintNone,
		cintLastComment
	}

	private const string cstrLastComment = "PNG\\System\\Comment\\Otuge00";

	private const string cstrGameClear = "PNG\\System\\Comment\\GameClear00";

	private const string cstrWhite = "PNG\\System\\White00";

	public bool pflgEnable;

	public bool pflgGameMainClearEnd = false;

	private enuGameScene enuGameMainScene = enuGameScene.cintNone;

	private Texture2D imgLastComment;

	private Texture2D imgGameClear;

	private SpriteFont font1;

	private Texture2D imgWhite;

	private float fltWhiteScale = 0.5f;

	public PlayerIndex playerIndex;

	private GamePadState gamePadState;

	private GamePadState gamePadMaeState;

	public int intOneScan;

	public override void Initialize()
	{
		pflgEnable = false;
		enuGameMainScene = enuGameScene.cintNone;
		pflgGameMainClearEnd = false;
		intOneScan = 0;
		fltWhiteScale = 0.5f;
		base.Initialize();
	}

	private void StageClearEndMov()
	{
		gamePadState = GamePad.GetState(playerIndex);
		if (intOneScan < 30)
		{
			intOneScan++;
			gamePadMaeState = gamePadState;
			return;
		}
		if (gamePadState.IsButtonDown(Buttons.Start) && !gamePadMaeState.IsButtonDown(Buttons.Start))
		{
			pflgGameMainClearEnd = true;
		}
		gamePadMaeState = gamePadState;
	}

	public void GameMainClearMov()
	{
		if (!pflgEnable)
		{
			return;
		}
		switch (enuGameMainScene)
		{
		case enuGameScene.cintNone:
			enuGameMainScene = enuGameScene.cintLastComment;
			break;
		case enuGameScene.cintLastComment:
			if (fltWhiteScale < 70f)
			{
				fltWhiteScale *= 1.6f;
			}
			StageClearEndMov();
			break;
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		imgLastComment = base.Game.Content.Load<Texture2D>("PNG\\System\\Comment\\Otuge00");
		imgGameClear = base.Game.Content.Load<Texture2D>("PNG\\System\\Comment\\GameClear00");
		font1 = base.Game.Content.Load<SpriteFont>("SpriteFont1");
		imgWhite = base.Game.Content.Load<Texture2D>("PNG\\System\\White00");
		base.LoadContent();
	}

	public void GameMainClearDraw(SpriteBatch aspritesBatch)
	{
		if (pflgEnable)
		{
			switch (enuGameMainScene)
			{
			case enuGameScene.cintNone:
				break;
			case enuGameScene.cintLastComment:
				aspritesBatch.Draw(imgWhite, new Vector2(640f - (float)imgWhite.Width * fltWhiteScale / 2f, 360f - (float)imgWhite.Width * fltWhiteScale / 2f + 20f), null, new Color(127, 127, 127, 180), MathHelper.ToRadians(0f), new Vector2(0f, 0f), new Vector2(fltWhiteScale, fltWhiteScale), SpriteEffects.None, 0f);
				aspritesBatch.Draw(imgGameClear, new Vector2(640 - imgGameClear.Width / 2, 360 - imgLastComment.Height / 2 - 80), Color.White);
				aspritesBatch.Draw(imgLastComment, new Vector2(640 - imgLastComment.Width / 2, 360 - imgLastComment.Height / 2), Color.White);
				aspritesBatch.DrawString(font1, "Push Start Button", new Vector2(490f, 510f), Color.White);
				break;
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
