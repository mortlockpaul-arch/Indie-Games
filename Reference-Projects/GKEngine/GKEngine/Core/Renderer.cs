using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Core;

public class Renderer
{
	public static Color COLOR_BLANK = new Color(0, 0, 0, 1);

	public bool active = true;

	public Scene scene;

	public SpriteBatch spriteBatch;

	public int targetIndex;

	public RenderTarget2D[] target = new RenderTarget2D[2];

	public Renderer()
	{
	}

	public Renderer(Scene oScene)
	{
		scene = oScene;
		active = true;
	}

	public virtual void Init()
	{
		spriteBatch = new SpriteBatch(GameEngine.instance.GraphicsDevice);
		target[0] = new RenderTarget2D(GameEngine.instance.GraphicsDevice, GameEngine.Graphics.PreferredBackBufferWidth, GameEngine.Graphics.PreferredBackBufferHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		target[1] = new RenderTarget2D(GameEngine.instance.GraphicsDevice, GameEngine.Graphics.PreferredBackBufferWidth, GameEngine.Graphics.PreferredBackBufferHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		targetIndex = 0;
	}

	public virtual void Reset()
	{
		if (spriteBatch != null)
		{
			spriteBatch.Dispose();
			target[0].Dispose();
			target[1].Dispose();
		}
		Init();
	}

	public virtual void Render(GameTime oGameTime)
	{
		if (active && scene != null && scene.isLoaded)
		{
			scene.PreRender(oGameTime);
			targetIndex = 0;
			GameEngine.instance.GraphicsDevice.SetRenderTarget(target[targetIndex]);
			GameEngine.instance.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, COLOR_BLANK, 1f, 0);
			scene.Render(oGameTime);
			GameEngine.instance.GraphicsDevice.SetRenderTarget(null);
			GameEngine.instance.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, COLOR_BLANK, 1f, 0);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			spriteBatch.Draw(target[targetIndex], Vector2.Zero, Color.White);
			spriteBatch.End();
		}
		else
		{
			RenderLoading();
		}
	}

	public void RenderLoading()
	{
		GameEngine.instance.GraphicsDevice.SetRenderTarget(null);
		GameEngine.instance.GraphicsDevice.Clear(ClearOptions.Target, COLOR_BLANK, 1f, 0);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		spriteBatch.Draw(GameEngine.instance.defaultLoading, new Vector2((float)(GameEngine.Graphics.GraphicsDevice.Viewport.Width - GameEngine.instance.defaultLoading.Width) * 0.5f, (float)(GameEngine.Graphics.GraphicsDevice.Viewport.Height - GameEngine.instance.defaultLoading.Height) * 0.5f), Color.White);
		spriteBatch.End();
	}

	public void ToggleTarget()
	{
		targetIndex = ((targetIndex == 0) ? 1 : 0);
		GameEngine.instance.GraphicsDevice.SetRenderTarget(target[targetIndex]);
		GameEngine.instance.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, COLOR_BLANK, 1f, 0);
	}

	public void SetStartTarget()
	{
		targetIndex = 0;
		GameEngine.instance.GraphicsDevice.SetRenderTarget(target[targetIndex]);
	}
}
