using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Edit;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GKEngine.Scenes;

public class Scene
{
	public enum SceneModes
	{
		Normal,
		Edit
	}

	private int _i;

	public CameraManager cameras = new CameraManager();

	public LightManager lights;

	public string name;

	public GameTime gameTime;

	public List<EntityStack> renderStacks = new List<EntityStack>();

	public int renderStackIndex;

	public bool isLoaded;

	public SceneModes mode;

	public Editor edit;

	public SceneLibrary library;

	public EntityStack renderStackFirst => renderStacks[0];

	public EntityStack renderStackLast => renderStacks[renderStacks.Count - 1];

	public Scene(string xName)
	{
		name = xName;
		Edit_Set();
		lights = new LightManager(this);
		lights.primary.position = new Vector3(0f, 0f, 0f);
		library = new SceneLibrary(this);
	}

	public virtual void Init()
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		cameras.Add(new Camera("Default", graphicsDevice.Viewport, cameras));
		cameras.Add(new Camera("Edit", graphicsDevice.Viewport, cameras));
		cameras.SetActive("Default");
	}

	public virtual void Exit()
	{
	}

	public virtual void PreRender(GameTime oGameTime)
	{
	}

	public virtual void Render(GameTime oGameTime)
	{
		int count = renderStacks.Count;
		for (renderStackIndex = 0; renderStackIndex < count; renderStackIndex++)
		{
			renderStacks[renderStackIndex].Render(oGameTime);
		}
	}

	public EntityStack RenderStacks_FromName(string xName)
	{
		EntityStack result = null;
		for (int i = 0; i < renderStacks.Count; i++)
		{
			if (renderStacks[i].name == xName)
			{
				result = renderStacks[i];
				break;
			}
		}
		return result;
	}

	public int RenderStacks_IndexFromName(string xName)
	{
		int result = -1;
		for (int i = 0; i < renderStacks.Count; i++)
		{
			if (renderStacks[i].name == xName)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public EntityStack RenderStacks_FromState(Material.State oState)
	{
		EntityStack result = null;
		for (int i = 0; i < renderStacks.Count; i++)
		{
			if (renderStacks[i].renderState == oState)
			{
				result = renderStacks[i];
				break;
			}
		}
		return result;
	}

	public void RenderStacks_Flush()
	{
		for (_i = 0; _i < renderStacks.Count; _i++)
		{
			renderStacks[_i].Clear();
		}
	}

	public virtual void Load()
	{
		_ = GameEngine.instance.GraphicsDevice;
		library.Load();
		Edit_Load();
		Input_Set();
		isLoaded = true;
	}

	public virtual void Unload()
	{
		for (_i = 0; _i < renderStacks.Count; _i++)
		{
			renderStacks[_i].Clear();
		}
		library.Unload();
		GameEngine.SceneContent.Unload();
		isLoaded = false;
	}

	public virtual void Update(GameTime oGameTime)
	{
		gameTime = oGameTime;
		Input_Update(oGameTime);
	}

	public virtual void Input_Set()
	{
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "Mode", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["Mode"].Add(new InputButton(Keys.OemTilde));
		UniversalInput.inputEntities["Mode"].active = true;
	}

	public virtual void Input_Update(GameTime oGameTime)
	{
		if (UniversalInput.inputEntities.ContainsKey("Mode") && UniversalInput.inputEntities["Mode"].pressed)
		{
			Mode_Toggle();
		}
	}

	public void Mode_Toggle()
	{
		if (mode == SceneModes.Normal)
		{
			mode = SceneModes.Edit;
			Edit_Activate();
		}
		else
		{
			mode = SceneModes.Normal;
			Edit_Deactivate();
		}
	}

	public void Mode_Set(SceneModes xMode)
	{
		if (xMode == SceneModes.Edit && mode == SceneModes.Normal)
		{
			Edit_Activate();
		}
		else if (xMode == SceneModes.Normal && mode == SceneModes.Edit)
		{
			Edit_Deactivate();
		}
		mode = xMode;
	}

	public virtual void Event_SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
	{
	}

	public virtual void Event_SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
	{
	}

	public virtual void Edit_Set()
	{
		edit = new Editor(this);
	}

	public virtual void Edit_Load()
	{
		edit.Load();
	}

	public virtual void Edit_Render(GameTime oGameTime)
	{
		edit.Render(oGameTime);
	}

	public virtual void Edit_Activate()
	{
		edit.Activate();
	}

	public virtual void Edit_Deactivate()
	{
		edit.Deactivate();
	}

	public virtual void Edit_Update(GameTime oGameTime)
	{
		edit.Update(oGameTime);
	}
}
