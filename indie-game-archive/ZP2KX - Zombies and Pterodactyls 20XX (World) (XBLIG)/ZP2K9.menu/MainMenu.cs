using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.menu;

public class MainMenu
{
	private float frame;

	private float scroll;

	private float alpha;

	private float inAlpha;

	public bool active;

	private RenderTarget2D sceneTarg;

	private Effect sceneEffect;

	private float sat;

	private float brite;

	public MainMenu(GraphicsDevice dev, ContentManager Content)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		frame = 1024f;
		scroll = 1024f;
		alpha = 1f;
		active = true;
		base._002Ector();
		sceneEffect = Content.Load<Effect>("fx/scene");
		sceneTarg = new RenderTarget2D(dev, 1280, 720, 1, (SurfaceFormat)1);
	}

	public bool IsSolid()
	{
		if (alpha >= 1f)
		{
			return true;
		}
		return false;
	}

	public void Update()
	{
		if (inAlpha < 1f)
		{
			inAlpha += Game1.frameTime;
			if (inAlpha > 1f)
			{
				inAlpha = 1f;
			}
		}
		if (active)
		{
			if (alpha < 1f)
			{
				alpha += Game1.frameTime * 2f;
			}
			if (alpha >= 1f)
			{
				alpha = 1f;
			}
		}
		else
		{
			if (alpha > 0f)
			{
				alpha -= Game1.frameTime * 2f;
			}
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
		if (brite < 1f)
		{
			brite += Game1.frameTime;
			if (brite > 1f)
			{
				brite = 1f;
			}
		}
		if (!(alpha > 0f))
		{
			return;
		}
		float num = 0f;
		num = ((!Game1.menu.menuLevel[13].active) ? 1f : 0f);
		if (sat > num)
		{
			sat -= Game1.frameTime;
			if (sat < num)
			{
				sat = num;
			}
		}
		if (sat < num)
		{
			sat += Game1.frameTime;
			if (sat > num)
			{
				sat = num;
			}
		}
		Game1.sceneMgr.Update();
		frame += Game1.frameTime;
		scroll += Game1.frameTime * 10f;
		if (scroll > 1536f)
		{
			scroll -= 1024f;
		}
	}

	public void Prepare(SpriteBatch sprite, GraphicsDevice dev)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!(alpha <= 0f))
		{
			dev.SetRenderTarget(0, sceneTarg);
			dev.Clear(Color.Black);
			sprite.Begin((SpriteBlendMode)1);
			Game1.sceneMgr.Draw(sprite);
			sprite.End();
			dev.SetRenderTarget(0, (RenderTarget2D)null);
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		if (alpha <= 0f)
		{
			return;
		}
		sceneEffect.Parameters["alpha"].SetValue(alpha);
		sceneEffect.Parameters["burn"].SetValue(1.5f + sat * 1f);
		sceneEffect.Parameters["add"].SetValue(sat * 0.4f - 1f + brite);
		sceneEffect.Parameters["sat"].SetValue(1f + sat * 0.99f);
		sceneEffect.Begin();
		sprite.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
		sceneEffect.CurrentTechnique.Passes[0].Begin();
		sprite.Draw(sceneTarg.GetTexture(), new Rectangle(0, 0, 1280, 720), Color.White);
		sceneEffect.CurrentTechnique.Passes[0].End();
		sprite.End();
		sceneEffect.End();
		sprite.Begin((SpriteBlendMode)2);
		Game1.text.size = 1f;
		Game1.text.color = new Color(1f, 1f, 1f, alpha);
		Game1.text.DrawString(new Vector2(1150f, 560f), Game1.netSession.version, 2, -1f, Game1.impact, sprite);
		if (Game1.netSession.newVersAvailable)
		{
			Game1.text.color = new Color(1f, 0.4f, 0.4f, alpha);
			for (int i = 0; i < Game1.netSession.newAvail.Length; i++)
			{
				Game1.text.DrawString(new Vector2(1150f, 420f + (float)i * 20f), Game1.netSession.newAvail[i], 2, -1f, Game1.impact, sprite);
			}
		}
		sprite.End();
		if (GameState.mode == 2 && alpha > 0f)
		{
			sprite.Begin((SpriteBlendMode)1);
			Game1.ticker.Draw(sprite, alpha);
			sprite.End();
		}
	}
}
