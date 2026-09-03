using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

public class LoadComponent : DrawableGameComponent
{
	private Texture2D btnBW;

	private Texture2D btnCol;

	private Texture2D btnPressed;

	private Texture2D btnPressedCol;

	private Texture2D buttonBW;

	private Texture2D buttonCol;

	private Texture2D games;

	private Texture2D red;

	private Texture2D nameSlide;

	private float rotateTime;

	private float curRotate;

	private int curIndex;

	private bool loadStarted;

	private int state;

	private float phaseCountdown;

	private float phaseMax;

	private Vector2 frameScale;

	private Vector2 frameCenter;

	private Vector2 frameOrigin;

	private Vector2 pressOffset;

	public LoadComponent(Game game)
		: base(game)
	{
		curRotate = (rotateTime = 4f);
		curIndex = 0;
		BaseGame.Get().MakeObj_Major(game);
		if (!BaseGame.PROFILE)
		{
			BaseGame.Get().loadThread = new Thread(BaseGame.Get().MakeObj_Minor);
			BaseGame.Get().loadThread.Start();
		}
		else
		{
			BaseGame.Get().MakeObj_Minor();
		}
		loadStarted = false;
		state = 0;
	}

	public override void Initialize()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().PrepareGraphicsObj_Major();
		games = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_games_alpha");
		nameSlide = BaseGame.Get().content.Load<Texture2D>("Content\\name_slide");
		btnBW = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_btnImage_alpha_bw");
		btnCol = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_btnImage_alpha");
		btnPressed = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_btnImage_pressed_bw");
		btnPressedCol = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_btnImage_pressed_alpha");
		buttonBW = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_button_alpha_bw");
		buttonCol = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_button_alpha");
		red = BaseGame.Get().content.Load<Texture2D>("Content\\rbLogo_red_alpha");
		frameScale = new Vector2((float)BaseGame.WIDTH / (float)red.Width, (float)BaseGame.HEIGHT / (float)red.Height);
		frameCenter = new Vector2((float)BaseGame.WIDTH / 2f, (float)BaseGame.HEIGHT * 0.46f);
		frameOrigin = new Vector2((float)red.Width / 2f, (float)red.Height * 0.46f);
		pressOffset = new Vector2(0f, 15f);
		((DrawableGameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		BaseGame.Get().input.Update();
		if (BaseGame.Get().input.PadPressed((Buttons)32) || BaseGame.Get().input.KeyPressed((Keys)27))
		{
			((GameComponent)this).Game.Exit();
		}
		phaseCountdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (state == 0 && phaseCountdown < 0f)
		{
			state = 10;
			phaseCountdown = 1.5f;
		}
		if (state == 10 && phaseCountdown < 0f)
		{
			BaseGame.Get().OpenPlay("Opening01");
			state = 15;
			phaseCountdown = (phaseMax = 0.25f);
		}
		if (state == 15 && phaseCountdown < 0f)
		{
			state = 20;
			phaseCountdown = 0.5f;
		}
		if (state == 20 && phaseCountdown < 0f)
		{
			BaseGame.Get().OpenPlay("Opening01");
			state = 25;
			phaseCountdown = (phaseMax = 0.25f);
		}
		if (state == 25 && phaseCountdown < 0f)
		{
			state = 30;
			phaseCountdown = 0.5f;
		}
		if (state == 30 && phaseCountdown < 0f)
		{
			BaseGame.Get().OpenPlay("Opening02");
			state = 35;
			phaseCountdown = (phaseMax = 0.25f);
		}
		if (state == 35 && phaseCountdown < 0f)
		{
			state = 50;
			phaseCountdown = 2.5f;
		}
		if (state == 50 && phaseCountdown < 0f)
		{
			state = 100;
			phaseCountdown = 4f;
		}
		if (state == 100)
		{
			if (phaseCountdown < 0f)
			{
				state = 110;
			}
		}
		else if (state == 110)
		{
			if (!BaseGame.quickload)
			{
				((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new MainMenuComponent(((GameComponent)this).Game));
				((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new TrialModeCheck(((GameComponent)this).Game));
			}
			else if (BaseGame.credits)
			{
				((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new CreditComponent(((GameComponent)this).Game, 4, 3));
			}
			else
			{
				((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new BaseComponent(((GameComponent)this).Game));
			}
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
		}
		if (BaseGame.Get().loadThread == null && !loadStarted)
		{
			if (!BaseGame.PROFILE)
			{
				BaseGame.Get().loadThread = new Thread(BaseGame.Get().PrepareGraphicsObj_Finish);
				BaseGame.Get().loadThread.Start();
			}
			else
			{
				BaseGame.Get().PrepareGraphicsObj_Finish();
			}
			loadStarted = true;
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.Clear(new Color(new Vector4(0.086f, 0.086f, 0.086f, 1f)));
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		if (state == 100)
		{
			BaseGame.Get().spriteBatch.Draw(nameSlide, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		if (state == 10 || state == 20)
		{
			BaseGame.Get().spriteBatch.Draw(btnBW, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		if (state == 15)
		{
			BaseGame.Get().spriteBatch.Draw(btnPressed, frameCenter + pressOffset, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(buttonBW, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale * new Vector2(1f, (phaseMax - phaseCountdown) / phaseMax), (SpriteEffects)0, 0f);
		}
		if (state == 20)
		{
			BaseGame.Get().spriteBatch.Draw(buttonBW, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		if (state == 25)
		{
			BaseGame.Get().spriteBatch.Draw(red, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale * new Vector2(1f, (phaseMax - phaseCountdown) / phaseMax), (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(btnPressed, frameCenter + pressOffset, (Rectangle?)null, new Color(Vector4.Lerp(new Vector4(1f, 1f, 1f, 1f), new Vector4(1f, 1f, 1f, 0f), (phaseMax - phaseCountdown) / phaseMax)), 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(buttonBW, frameCenter, (Rectangle?)null, new Color(Vector4.Lerp(new Vector4(1f, 1f, 1f, 1f), new Vector4(1f, 1f, 1f, 0f), (phaseMax - phaseCountdown) / phaseMax)), 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(btnPressedCol, frameCenter + pressOffset, (Rectangle?)null, new Color(Vector4.Lerp(new Vector4(1f, 1f, 1f, 0f), new Vector4(1f, 1f, 1f, 1f), (phaseMax - phaseCountdown) / phaseMax)), 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(buttonCol, frameCenter, (Rectangle?)null, new Color(Vector4.Lerp(new Vector4(1f, 1f, 1f, 0f), new Vector4(1f, 1f, 1f, 1f), (phaseMax - phaseCountdown) / phaseMax)), 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		if (state == 30 || state == 50)
		{
			BaseGame.Get().spriteBatch.Draw(btnCol, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(buttonCol, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(red, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		if (state == 35)
		{
			BaseGame.Get().spriteBatch.Draw(btnPressedCol, frameCenter + pressOffset, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(buttonCol, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(red, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.Draw(games, frameCenter + new Vector2(-1000f, 0f) * (phaseCountdown / phaseMax), (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		if (state == 50)
		{
			BaseGame.Get().spriteBatch.Draw(games, frameCenter, (Rectangle?)null, Color.White, 0f, frameOrigin, frameScale, (SpriteEffects)0, 0f);
		}
		BaseGame.Get().spriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
