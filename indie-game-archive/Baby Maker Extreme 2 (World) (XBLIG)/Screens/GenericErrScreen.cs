using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

internal class GenericErrScreen : Screen
{
	private SpriteInstance m_bg;

	private SpriteInstance m_bgbehind;

	private string text;

	private int index;

	private RenderLight m_light;

	public GenericErrScreen(int controllerIndex, string s)
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		index = controllerIndex;
		text = s;
		m_bg = TextureContainer.GetSprite("images/score", SceneRenderer.GetCameraPosition(), 1000f);
		m_bg.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/scoreNorm");
		m_bg.SurfaceScale = SceneRenderer.GetScreenDim();
		m_bgbehind = TextureContainer.GetSprite("images/whitesquare", SceneRenderer.GetScreenDim() / 2f, 0f);
		m_bgbehind.Color = Color.Black;
		m_bgbehind.SurfaceScale = new Vector2(1280f, 1280f);
		m_light = new RenderLight(new Vector3(1500f, 400f, 1000f), 0f, 2000, Color.White);
		text += "\n\nPress A to Continue";
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_light.Draw(gameTime);
		m_bgbehind.Position = SceneRenderer.GetCameraPosition();
		m_bgbehind.Draw(gameTime);
		m_bg.Position = SceneRenderer.GetCameraPosition();
		m_bg.Draw(gameTime);
		SceneRenderer.DrawString(fonts.BASE_FONT, text, m_bg.Position - new Vector2(m_bg.SurfaceScale.X / 2f - 200f, 150f), Color.White, m_bg.Depth + DepthConsts.TEXT_DEPTH);
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedActivate(index) || ControlManager.PressedStart(index))
		{
			ScreenStorage.PopScreen("");
		}
	}

	public override void Update(TimeTracker gameTime)
	{
	}
}
