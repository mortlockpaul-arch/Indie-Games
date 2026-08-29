using System;
using System.Collections.Generic;
using GKEngine.Core;
using GKEngine.Input;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GKEngine;

public class GameEngine : Microsoft.Xna.Framework.Game
{
	public static GameEngine instance;

	public static Random random = new Random();

	public static GraphicsDeviceManager Graphics;

	public new static ContentManager Content;

	public static ContentManager SceneContent;

	public static Scene scene;

	public Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

	public Renderer renderer;

	public UpdateStack updateStack = new UpdateStack();

	public Texture2D defaultLoading;

	public bool update = true;

	public GameEngine(Point oScreenSize)
	{
		instance = this;
		Graphics = new GraphicsDeviceManager(this);
		Content = new ContentManager(base.Services);
		SceneContent = new ContentManager(base.Services);
		Graphics.PreferredBackBufferWidth = oScreenSize.X;
		Graphics.PreferredBackBufferHeight = oScreenSize.Y;
		renderer = new Renderer();
		UniversalInput.Init();
		Input_Set();
	}

	protected override void Initialize()
	{
		base.GraphicsDevice.Reset();
		renderer.Init();
		base.Initialize();
	}

	protected override void LoadContent()
	{
		LoadLoadingScreen();
		base.LoadContent();
	}

	protected virtual void LoadLoadingScreen()
	{
		defaultLoading = Content.Load<Texture2D>("Content/_protected/Defaults/Loading");
	}

	protected override void UnloadContent()
	{
		Content.Unload();
		SceneContent.Unload();
		base.UnloadContent();
	}

	protected override void Update(GameTime oGameTime)
	{
		if (!update)
		{
			return;
		}
		UniversalInput.Update(oGameTime);
		Input_Update(oGameTime);
		if (scene != null)
		{
			if (scene.mode == Scene.SceneModes.Normal)
			{
				scene.Update(oGameTime);
			}
			else
			{
				scene.Edit_Update(oGameTime);
			}
		}
		updateStack.Update(oGameTime);
		base.Update(oGameTime);
	}

	protected override void Draw(GameTime oGameTime)
	{
		renderer.Render(oGameTime);
		base.Draw(oGameTime);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public void Scene_Set(Scene oScene)
	{
		scene = oScene;
		renderer.scene = oScene;
	}

	public void Scene_Add(Scene oScene)
	{
		scenes.Add(oScene.name, oScene);
	}

	public void Scene_Swap(Scene oScene)
	{
		updateStack.done = delegate
		{
			try
			{
				updateStack.Stop(null);
				updateStack.Clear();
				renderer.active = false;
				scene.Unload();
				Graphics.GraphicsDevice.SetVertexBuffers();
				Graphics.GraphicsDevice.Indices = null;
				renderer.RenderLoading();
				Graphics.GraphicsDevice.Present();
				Scene_Set(oScene);
				scene.Load();
				renderer.active = true;
				updateStack.Start();
			}
			catch (Exception error)
			{
				Event_SceneLoadError(error);
			}
		};
	}

	public virtual void Input_Set()
	{
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "Exit", InputEntity.Scope.Game));
		UniversalInput.inputEntities["Exit"].Add(new InputButton(Keys.Escape));
		UniversalInput.inputEntities["Exit"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "Fullscreen", InputEntity.Scope.Game));
		UniversalInput.inputEntities["Fullscreen"].Add(new InputButton(Keys.Tab));
	}

	public virtual void Input_Update(GameTime oGameTime)
	{
		if (UniversalInput.inputEntities["Exit"].pressed)
		{
			Exit();
		}
		if (UniversalInput.inputEntities["Fullscreen"].pressed)
		{
			Graphics.ToggleFullScreen();
		}
	}

	public void Video_SetRes(Point oScreenSize)
	{
		Graphics.PreferredBackBufferWidth = oScreenSize.X;
		Graphics.PreferredBackBufferHeight = oScreenSize.Y;
		Graphics.ApplyChanges();
		renderer.Reset();
	}

	public Vector3 GetRandUnitVecor()
	{
		return Vector3.Normalize(new Vector3((float)(random.NextDouble() * 2.0) - 1f, (float)(random.NextDouble() * 2.0) - 1f, (float)(random.NextDouble() * 2.0) - 1f));
	}

	public Texture2D GetSolidColorTexture(Color oColor)
	{
		Color[] data = new Color[1] { oColor };
		Texture2D texture2D = new Texture2D(Graphics.GraphicsDevice, 1, 1);
		texture2D.SetData(data);
		return texture2D;
	}

	public Texture2D SetTextureColor(Texture2D xTexColor, Color oColor, int xVariance)
	{
		Color[] array = new Color[xTexColor.Width * xTexColor.Height];
		xTexColor.GetData(array);
		for (int i = 0; i < array.Length; i++)
		{
			int num = random.Next(-xVariance, xVariance);
			ref Color reference = ref array[i];
			reference = new Color((byte)(oColor.R + num), (byte)(oColor.G + num), (byte)(oColor.B + num), array[i].A);
		}
		Texture2D texture2D = new Texture2D(Graphics.GraphicsDevice, xTexColor.Width, xTexColor.Height);
		texture2D.SetData(array);
		return texture2D;
	}

	protected virtual void Event_SceneLoadError(Exception error)
	{
	}
}
