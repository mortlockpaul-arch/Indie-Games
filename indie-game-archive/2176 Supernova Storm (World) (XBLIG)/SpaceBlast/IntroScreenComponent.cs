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
		: base((Game)(object)game)
	{
		m_Game = game;
		Stage = IntroStage.SupernovaStormFadingIn;
	}

	protected override void LoadContent()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		m_SpriteBatch = new SpriteBatch(((Game)m_Game).GraphicsDevice);
		m_FaderFX = MainGame.ContentMan.Load<Effect>("Effects/Fader");
		m_FaderAlphaParam = m_FaderFX.Parameters["alpha"];
		m_TexBackground = MainGame.ContentMan.Load<Texture2D>("Textures/Black");
		m_TexSupernovaStorm = MainGame.ContentMan.Load<Texture2D>("Textures/Intro_SupernovaStorm");
		m_TexByBenSleat = MainGame.ContentMan.Load<Texture2D>("Textures/Intro_ByBenSleat");
		Viewport viewport = ((Game)m_Game).GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = ((Game)m_Game).GraphicsDevice.Viewport;
		m_FullscreenRect = new Rectangle(0, 0, width, ((Viewport)(ref viewport2)).Height);
		m_PosSupernovaStorm = new Vector2((float)(m_FullscreenRect.Width / 2 - m_TexSupernovaStorm.Width / 2), 200f);
		m_PosByBenSleat = new Vector2((float)(m_FullscreenRect.Width / 2 - m_TexByBenSleat.Width / 2), 500f);
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = InputManager.ListenForPlayer1Controller();
		KeyboardState state = Keyboard.GetState();
		if (flag || ((KeyboardState)(ref state)).IsKeyDown((Keys)32))
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
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		m_SpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
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
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
