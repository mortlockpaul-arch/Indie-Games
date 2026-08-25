using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceBlast;

internal class IntroScreenComponent : DrawableGameComponent
{
	private enum TextAlign
	{
		textLeft,
		textCentered,
		textRight
	}

	public IntroStage Stage;

	private byte m_BackgroundFade = byte.MaxValue;

	private byte m_SupernovaStormFade;

	private byte m_ByBenSleatFade;

	private SpriteBatch m_SpriteBatch;

	private Effect m_FaderFX;

	private EffectParameter m_FaderAlphaParam;

	private MainGame m_Game;

	private Rectangle m_FullscreenRect;

	private Texture2D m_TexBackground;

	private Texture2D m_TexSupernovaStorm;

	private Texture2D m_TexByBenSleat;

	private Vector2 m_PosSupernovaStorm;

	private Vector2 m_PosByBenSleat;

	private double m_NextActionTime;

	public IntroScreenComponent(MainGame game)
		: base(game)
	{
		m_Game = game;
		Stage = IntroStage.SupernovaStormFadingIn;
	}

	protected override void LoadContent()
	{
		m_SpriteBatch = new SpriteBatch(m_Game.GraphicsDevice);
		m_FaderFX = MainGame.ContentMan.Load<Effect>("Effects/Fader");
		m_FaderAlphaParam = m_FaderFX.Parameters["alpha"];
		m_TexBackground = MainGame.ContentMan.Load<Texture2D>("Textures/Black");
		m_TexSupernovaStorm = MainGame.ContentMan.Load<Texture2D>("Textures/Intro_SupernovaStorm");
		m_TexByBenSleat = MainGame.ContentMan.Load<Texture2D>("Textures/Intro_ByBenSleat");
		m_FullscreenRect = new Rectangle(0, 0, m_Game.GraphicsDevice.Viewport.Width, m_Game.GraphicsDevice.Viewport.Height);
		m_PosSupernovaStorm = new Vector2(m_FullscreenRect.Width / 2 - m_TexSupernovaStorm.Width / 2, 200f);
		m_PosByBenSleat = new Vector2(m_FullscreenRect.Width / 2 - m_TexByBenSleat.Width / 2, 500f);
		base.LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		bool flag = InputManager.ListenForPlayer1Controller();
		KeyboardState state = Keyboard.GetState();
		if (flag || state.IsKeyDown(Keys.Space))
		{
			m_Game.IntroFinished();
			return;
		}
		switch (Stage)
		{
		case IntroStage.SupernovaStormFadingIn:
			if (m_SupernovaStormFade++ == 254)
			{
				Stage = IntroStage.Holding1;
				m_NextActionTime = gameTime.TotalGameTime.TotalSeconds + 3.0;
			}
			break;
		case IntroStage.Holding1:
			if (gameTime.TotalGameTime.TotalSeconds >= m_NextActionTime)
			{
				Stage = IntroStage.NameFadeIn;
			}
			break;
		case IntroStage.NameFadeIn:
			if (m_ByBenSleatFade++ == 10)
			{
				Stage = IntroStage.Holding2;
				m_NextActionTime = gameTime.TotalGameTime.TotalSeconds + 3.0;
			}
			break;
		case IntroStage.Holding2:
			if (gameTime.TotalGameTime.TotalSeconds >= m_NextActionTime)
			{
				Stage = IntroStage.FadingOut;
			}
			break;
		case IntroStage.FadingOut:
			if (m_BackgroundFade-- == 1)
			{
				m_Game.IntroFinished();
			}
			break;
		}
		base.Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		m_FaderFX.Begin();
		m_FaderFX.CurrentTechnique.Passes[0].Begin();
		m_FaderAlphaParam.SetValue((float)(int)m_BackgroundFade / 255f);
		m_SpriteBatch.Draw(m_TexBackground, m_FullscreenRect, Color.White);
		m_FaderFX.CurrentTechnique.Passes[0].End();
		m_FaderFX.CurrentTechnique.Passes[0].Begin();
		m_FaderAlphaParam.SetValue(MathHelper.Min((float)(int)m_BackgroundFade / 255f, (float)(int)m_SupernovaStormFade / 255f));
		m_SpriteBatch.Draw(m_TexSupernovaStorm, m_PosSupernovaStorm, Color.White);
		m_FaderFX.CurrentTechnique.Passes[0].End();
		m_FaderFX.CurrentTechnique.Passes[0].Begin();
		m_FaderAlphaParam.SetValue(MathHelper.Min((float)(int)m_BackgroundFade / 255f, (float)(int)m_ByBenSleatFade / 255f));
		m_SpriteBatch.Draw(m_TexByBenSleat, m_PosByBenSleat, Color.White);
		m_FaderFX.CurrentTechnique.Passes[0].End();
		m_FaderFX.End();
		m_SpriteBatch.End();
		base.Draw(gameTime);
	}
}
