using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

internal class Background3D : GameScreen
{
	private ContentManager content;

	private SpriteFont gameFont;

	private string namescene;

	private Scene scene;

	public SceneEnvironment environment;

	public CustomCamera mCamera;

	private Model skysphere;

	private Matrix skySphereWorld;

	private bool exterior;

	public Background3D(string name, bool ext)
	{
		exterior = ext;
		namescene = name;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		skySphereWorld = Matrix.CreateRotationY((float)Math.PI * -17f / 20f) * Matrix.CreateTranslation(new Vector3(0f, -30f, 0f));
		base.ScreenManager.init_managers();
		if (base.ScreenManager.contentRepository == null)
		{
			base.ScreenManager.contentRepository = content.Load<ContentRepository>("Background/" + namescene + "/Content");
		}
		else
		{
			base.ScreenManager.contentRepository.Dispose();
			base.ScreenManager.contentRepository = content.Load<ContentRepository>("Background/" + namescene + "/Content");
		}
		scene = content.Load<Scene>("Background/" + namescene + "/scene");
		environment = content.Load<SceneEnvironment>("Background/" + namescene + "/Environment");
		if (exterior)
		{
			skysphere = content.Load<Model>("Background/" + namescene + "/SkySphere");
		}
		mCamera = new CustomCamera(environment.VisibleDistance, 1f);
		base.ScreenManager.sceneInterface.Submit(scene);
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (SplashScreenGameComponent.DisplayComplete)
		{
			base.ScreenManager.sceneInterface.Update(gameTime);
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
		new Rectangle(0, 0, viewport.Width, viewport.Height);
		base.ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
		base.ScreenManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		if (SplashScreenGameComponent.DisplayComplete)
		{
			base.ScreenManager.sceneState.BeginFrameRendering(mCamera.View, mCamera.Projection, gameTime, environment, base.ScreenManager.frameBuffers, renderingtoscreen: true);
			base.ScreenManager.sceneInterface.BeginFrameRendering(base.ScreenManager.sceneState);
			if (exterior)
			{
				RenderSky(base.ScreenManager.sceneState, base.ScreenManager.GraphicsDevice);
			}
			base.ScreenManager.sceneInterface.RenderManager.Render();
			base.ScreenManager.sceneInterface.EndFrameRendering();
			base.ScreenManager.sceneState.EndFrameRendering();
		}
		spriteBatch.Begin();
		spriteBatch.End();
	}

	public void RenderSky(ISceneState scenestate, GraphicsDevice gd)
	{
		if (skysphere == null)
		{
			gd.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 0);
			return;
		}
		gd.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 0);
		gd.BlendState = BlendState.Opaque;
		gd.DepthStencilState = DepthStencilState.None;
		gd.RasterizerState = RasterizerState.CullCounterClockwise;
		gd.SamplerStates[0] = SamplerState.AnisotropicWrap;
		for (int i = 1; i < 8; i++)
		{
			gd.SamplerStates[i] = SamplerState.PointClamp;
		}
		Matrix view = scenestate.View;
		view.Translation = Vector3.Zero;
		foreach (ModelMesh mesh in skysphere.Meshes)
		{
			foreach (Effect effect in mesh.Effects)
			{
				if (effect is BasicEffect)
				{
					BasicEffect basicEffect = effect as BasicEffect;
					basicEffect.LightingEnabled = false;
					basicEffect.DiffuseColor = new Vector3(1f, 1f, 1f);
					basicEffect.EmissiveColor = new Vector3(0.5f, 0.5f, 0.5f);
					basicEffect.View = view;
					basicEffect.World = skySphereWorld;
					basicEffect.Projection = scenestate.Projection;
				}
			}
			mesh.Draw();
		}
		gd.DepthStencilState = DepthStencilState.Default;
	}
}
