using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

internal class BackgroundMenu3D : GameScreen
{
	private BlendState TrueAdditiveBlend = new BlendState
	{
		ColorSourceBlend = Blend.One,
		ColorDestinationBlend = Blend.One,
		AlphaSourceBlend = Blend.One,
		AlphaDestinationBlend = Blend.One
	};

	private ContentManager content;

	private SpriteFont gameFont;

	private string namescene;

	private Scene scene;

	public SceneEnvironment environment;

	public CustomCamera mCamera;

	private Model skysphere;

	private Matrix skySphereWorld;

	private bool exterior;

	private Plane waterWorldPlane;

	private SasEffect waterEffect;

	private int reflectionRefractionTargetSize = 512;

	private int reflectionRefractionTargetMultiSampleAmount = 2;

	private FrameBuffers reflectionRefractionFrameBuffers;

	private RenderTargetHelper refractionTarget;

	private RenderTargetHelper waterReflectionTarget;

	private SystemPreferences refractionPreferences;

	private DetailPreference renderQuality;

	private SceneObject water;

	private SceneObject WaterPlane;

	private SceneObject skybox;

	private float totalTime;

	public BackgroundMenu3D(string name, bool ext)
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
		skySphereWorld = Matrix.CreateRotationY(MathHelper.ToRadians(270f)) * Matrix.CreateTranslation(new Vector3(0f, 0f, 0f));
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
		graphics = base.ScreenManager.graphics;
		graphics.SynchronizeWithVerticalRetrace = false;
		graphics.PreparingDeviceSettings += PrepareDeviceSettings;
		graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
		renderQuality = DetailPreference.Medium;
		reflectionRefractionTargetSize = 512;
		reflectionRefractionTargetMultiSampleAmount = 2;
		graphics.PreferMultiSampling = true;
		graphics.SynchronizeWithVerticalRetrace = true;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		reflectionRefractionFrameBuffers = new FrameBuffers(reflectionRefractionTargetSize, reflectionRefractionTargetSize, DetailPreference.Medium, DetailPreference.Medium);
		refractionTarget = new RenderTargetHelper(RenderTargetHelper.TargetType.Standard, reflectionRefractionTargetSize, reflectionRefractionTargetSize, mipmapped: false, SurfaceFormat.Color, reflectionRefractionTargetMultiSampleAmount, RenderTargetUsage.PlatformContents);
		waterReflectionTarget = new RenderTargetHelper(RenderTargetHelper.TargetType.Reflection, reflectionRefractionTargetSize, reflectionRefractionTargetSize, mipmapped: false, SurfaceFormat.Color, reflectionRefractionTargetMultiSampleAmount, RenderTargetUsage.PlatformContents);
		base.ScreenManager.sceneInterface.ResourceManager.AssignOwnership(base.ScreenManager.frameBuffers);
		base.ScreenManager.sceneInterface.ResourceManager.AssignOwnership(reflectionRefractionFrameBuffers);
		base.ScreenManager.sceneInterface.ResourceManager.AssignOwnership(refractionTarget);
		base.ScreenManager.sceneInterface.ResourceManager.AssignOwnership(waterReflectionTarget);
		scene = content.Load<Scene>("Background/" + namescene + "/scene");
		environment = content.Load<SceneEnvironment>("Background/" + namescene + "/Environment");
		if (exterior)
		{
			skysphere = content.Load<Model>("Background/" + namescene + "/SkySphere");
		}
		skybox = new SceneObject(skysphere);
		mCamera = new CustomCamera(environment.VisibleDistance, 1f);
		base.ScreenManager.sceneInterface.Submit(scene);
		refractionPreferences = new SystemPreferences();
		refractionPreferences.EffectDetail = DetailPreference.Low;
		refractionPreferences.MaxAnisotropy = 0;
		refractionPreferences.PostProcessingDetail = DetailPreference.Low;
		refractionPreferences.ShadowDetail = DetailPreference.Low;
		refractionPreferences.ShadowQuality = 0.25f;
		refractionPreferences.TextureSampling = SamplingPreference.Trilinear;
		refractionTarget.ApplyPreferences(refractionPreferences);
		waterReflectionTarget.ApplyPreferences(refractionPreferences);
		waterEffect = content.Load<SasEffect>("Scenes/Scene3/WaterRef");
		waterWorldPlane = new Plane(Vector3.Down, 1.2f);
		water = new SceneObject(content.Load<Model>("Scenes/Scene3/OceanMesh"));
		water.RenderableMeshes[0].Effect = content.Load<SasEffect>("Scenes/Scene3/water");
		water.World = Matrix.CreateScale(new Vector3(25f, 0.5f, 25f)) * Matrix.CreateTranslation(new Vector3(0f, 1f, 0f));
		WaterPlane = water;
		base.ScreenManager.sceneInterface.ObjectManager.Submit(water);
		base.ScreenManager.sceneInterface.ObjectManager.Submit(skybox);
	}

	private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
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
		totalTime += (float)gameTime.ElapsedGameTime.Milliseconds * 0.0003f;
		waterEffect.Parameters["time"].SetValue(totalTime / 2f);
		waterWorldPlane = new Plane(Vector3.Down, 20f + (float)Math.Sin(totalTime * 0.0001f));
		if (WaterPlane != null)
		{
			WaterPlane.World = Matrix.CreateScale(new Vector3(50f, 1f, 50f)) * Matrix.CreateTranslation(new Vector3(0f, 20.5f, 0f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["LightDirection"].SetValue(new Vector3(-1f, -1f, 0f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["EyePosition"].SetValue(base.ScreenManager.sceneState.ViewToWorld.Translation);
			WaterPlane.RenderableMeshes[0].Effect.Parameters["AmbientColor"].SetValue(new Vector4(0.1f, 0.1f, 0.3f, 1f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["SpecularColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["time"].SetValue(totalTime / 2f);
		}
		mCamera.position.Y = 50f;
		mCamera.lookat.Y = 80f;
		mCamera.update(gameTime);
		skySphereWorld = Matrix.CreateScale(new Vector3(10f, 10f, 10f)) * Matrix.CreateRotationY(MathHelper.ToRadians(297f)) * Matrix.CreateTranslation(new Vector3(0f, -50f, 0f));
		skybox.World = skySphereWorld;
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
			base.ScreenManager.sceneInterface.ObjectManager.Remove(water);
			base.ScreenManager.sceneState.BeginFrameRendering(mCamera.View, mCamera.Projection, gameTime, environment, reflectionRefractionFrameBuffers, renderingtoscreen: false);
			refractionTarget.BeginFrameRendering(base.ScreenManager.sceneState);
			base.ScreenManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Gray, 1f, 0);
			RenderTarget(refractionTarget);
			refractionTarget.EndFrameRendering();
			base.ScreenManager.sceneState.EndFrameRendering();
			base.ScreenManager.sceneState.BeginFrameRendering(mCamera.View, mCamera.Projection, gameTime, environment, reflectionRefractionFrameBuffers, renderingtoscreen: false);
			waterReflectionTarget.BeginFrameRendering(base.ScreenManager.sceneState, waterWorldPlane);
			base.ScreenManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Gray, 1f, 0);
			RenderTarget(waterReflectionTarget);
			waterReflectionTarget.EndFrameRendering();
			base.ScreenManager.sceneState.EndFrameRendering();
			base.ScreenManager.sceneInterface.ObjectManager.Submit(water);
			base.ScreenManager.sceneState.BeginFrameRendering(mCamera.View, mCamera.Projection, gameTime, environment, base.ScreenManager.frameBuffers, renderingtoscreen: true);
			base.ScreenManager.sceneInterface.BeginFrameRendering(base.ScreenManager.sceneState);
			base.ScreenManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);
			base.ScreenManager.sceneInterface.RenderManager.Render();
			base.ScreenManager.GraphicsDevice.BlendState = TrueAdditiveBlend;
			base.ScreenManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			RenderMesh(water, base.ScreenManager.sceneState, waterEffect, waterReflectionTarget.GetTexture(), refractionTarget.GetTexture());
			base.ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
			base.ScreenManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			base.ScreenManager.sceneInterface.EndFrameRendering();
			base.ScreenManager.sceneState.EndFrameRendering();
		}
		spriteBatch.Begin();
		spriteBatch.End();
	}

	private void RenderMesh(SceneObject sceneObject, SceneState sceneState, SasEffect effect, Texture2D reflecttexture, Texture2D refracttexture)
	{
		effect.View = sceneState.View;
		effect.Projection = sceneState.Projection;
		effect.Parameters["ReflectTexture"].SetValue(reflecttexture);
		effect.Parameters["RefractTexture"].SetValue(refracttexture);
		EffectPassCollection passes = effect.CurrentTechnique.Passes;
		for (int i = 0; i < passes.Count; i++)
		{
			EffectPass effectPass = passes[i];
			for (int j = 0; j < sceneObject.RenderableMeshes.Count; j++)
			{
				RenderableMesh renderableMesh = sceneObject.RenderableMeshes[j];
				effect.World = renderableMesh.World;
				Effect effect2 = renderableMesh.Effect;
				if (effect2 is BaseMaterialEffect)
				{
					effect.Parameters["BumpTexture"].SetValue((effect2 as BaseMaterialEffect).NormalMapTexture);
				}
				else
				{
					effect.Parameters["BumpTexture"].SetValue(effect2.Parameters["NormalMap"].GetValueTexture2D());
				}
				effectPass.Apply();
				base.ScreenManager.GraphicsDevice.SetVertexBuffer(renderableMesh.VertexBuffer, renderableMesh.VertexStreamOffset);
				base.ScreenManager.GraphicsDevice.Indices = renderableMesh.IndexBuffer;
				base.ScreenManager.GraphicsDevice.DrawIndexedPrimitives(renderableMesh.PrimitiveType, renderableMesh.VertexBase, 0, renderableMesh.VertexCount, renderableMesh.ElementStart, renderableMesh.PrimitiveCount);
			}
		}
	}

	private void RenderTarget(RenderTargetHelper refractionTarget)
	{
		base.ScreenManager.sceneInterface.ApplyPreferences(refractionTarget.Preferences);
		base.ScreenManager.sceneInterface.BeginFrameRendering(refractionTarget.SceneState);
		base.ScreenManager.sceneInterface.RenderManager.Render();
		base.ScreenManager.sceneInterface.EndFrameRendering();
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
					basicEffect.EmissiveColor = new Vector3(1f, 1f, 1f);
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
