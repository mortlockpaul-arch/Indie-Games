using BabyMaker;
using Microsoft.Xna.Framework;
using Renderer;
using Skeleton;

namespace Screens;

public class SplashScreen : Screen
{
	private RenderSprite m_bg;

	private CharacterManager m_dino;

	private int timer;

	private TransitionHelper m_transition;

	private bool m_bStarted;

	public SplashScreen(int timeShown)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_transition = new TransitionHelper();
		m_bg = SpriteManager.GetSprite("images/backdrops/stegersaurusLogo", SceneRenderer.GetScreenDim() / 2f, 1f);
		m_bg.Position = SceneRenderer.GetScreenDim() / 2f;
		m_dino = new CharacterManager("outfits/others/stegersaurus/outfit");
		m_dino.SetAnimation(1, 0);
		m_dino.WorldPosition = new Vector2(-400f, 412f);
		m_dino.Depth = 1.2f;
		timer = timeShown;
		m_bStarted = false;
	}

	public override void Draw(TimeTracker gameTime)
	{
		SceneRenderer.MoveCamera(SceneRenderer.GetScreenDim() / 2f, 0f, 1f);
		SceneRenderer.DrawRenderSprite(m_bg);
		m_dino.DrawUpdater(gameTime);
		m_dino.Draw(gameTime);
		if (timer <= 0)
		{
			m_transition.Draw(gameTime);
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		m_bStarted = true;
		m_dino.WorldPosition += new Vector2(gameTime.FractionOfSecond * 300f, 0f);
		if (m_dino.WorldPosition.X >= SceneRenderer.GetScreenDim().X / 2f)
		{
			m_dino.WorldPosition = new Vector2(SceneRenderer.GetScreenDim().X / 2f, 412f);
			m_dino.AnimSpeed -= gameTime.FractionOfSecond * 0.6f;
			if (m_dino.AnimSpeed < 0.01f)
			{
				m_dino.AnimSpeed = 0f;
			}
		}
		m_dino.Update(gameTime);
		if (timer > 0)
		{
			timer -= gameTime.GameTime.ElapsedGameTime.Milliseconds;
			if (timer <= 0)
			{
				m_transition.TransitionIn();
			}
			return;
		}
		m_transition.Update(gameTime);
		if (m_transition.IsTransitionedIn)
		{
			Game1.PopScreen("");
			Game1.LoadGlobalOptions();
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
	}
}
