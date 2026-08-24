using GKEngine;
using GKEngine.Entities;
using Game.Atoms;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Build.UI;

public class BuildUI
{
	private const float ICON_WAIT_TIME = 2000f;

	private const float ICON_FADE_TIME = 500f;

	public static Color COLOR_TEXT_DEFAULT = new Color(249, 180, 6, 255);

	public BuildUniverse universe;

	public SpriteManager spriteManager;

	public SpriteFont fontKA_25;

	public SpriteFont fontKA_18;

	private Sprite spriteVingette;

	private Sprite spriteRays;

	private Sprite spriteTitle;

	private Sprite spriteHelp;

	private Sprite spriteMenu;

	private Sprite spriteIcon;

	private Sprite spriteButtonBackground;

	private BuildUIStatus status;

	private BuildUIButtonA buttonA;

	private BuildUIButtonB buttonB;

	private BuildUIButtonX buttonX;

	private BuildUIButtonY buttonY;

	private BuildUIButtonLeftStick buttonLeftStick;

	private BuildUIButtonRightStick buttonRightStick;

	private BuildUIButtonLeftShoulder buttonLeftShoulder;

	private BuildUIButtonRightShoulder buttonRightShoulder;

	private float iconTime;

	private bool iconWaiting;

	private bool iconFading;

	public BuildUI(BuildUniverse oUniverse)
	{
		universe = oUniverse;
		Init();
	}

	private void Init()
	{
		spriteManager = new SpriteManager(universe.scene, universe.scene.RenderStacks_FromName("UI"));
		spriteManager.effect = null;
		Load();
	}

	private void Load()
	{
		spriteManager.Load();
		Fonts_Load();
		spriteVingette = new Sprite(spriteManager);
		spriteVingette.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Vingette");
		spriteVingette.tint = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		spriteRays = new Sprite(spriteManager);
		spriteRays.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Rays");
		spriteButtonBackground = new Sprite(spriteManager);
		spriteButtonBackground.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Buttons_Background");
		spriteTitle = new Sprite(spriteManager);
		spriteTitle.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Title");
		spriteHelp = new Sprite(spriteManager);
		spriteHelp.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Button_Help");
		spriteMenu = new Sprite(spriteManager);
		spriteMenu.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Button_Menu");
		status = new BuildUIStatus(this);
		status.text = "Terror Firma";
		buttonA = new BuildUIButtonA(this);
		buttonB = new BuildUIButtonB(this);
		buttonX = new BuildUIButtonX(this);
		buttonY = new BuildUIButtonY(this);
		buttonLeftStick = new BuildUIButtonLeftStick(this);
		buttonRightStick = new BuildUIButtonRightStick(this);
		buttonLeftShoulder = new BuildUIButtonLeftShoulder(this);
		buttonRightShoulder = new BuildUIButtonRightShoulder(this);
		spriteIcon = new Sprite(spriteManager);
	}

	public void Update(GameTime oGameTime)
	{
		status.Update(oGameTime);
		buttonA.Update(oGameTime);
		buttonB.Update(oGameTime);
		buttonX.Update(oGameTime);
		buttonY.Update(oGameTime);
		buttonLeftStick.Update(oGameTime);
		buttonRightStick.Update(oGameTime);
		buttonLeftShoulder.Update(oGameTime);
		buttonRightShoulder.Update(oGameTime);
		Icon_Update(oGameTime);
	}

	public void Dispose()
	{
		spriteHelp.Dispose();
		spriteMenu.Dispose();
		spriteVingette.Dispose();
		spriteButtonBackground.Dispose();
		spriteTitle.Dispose();
		spriteRays.Dispose();
		status.Dispose();
		buttonA.Dispose();
		buttonB.Dispose();
		buttonX.Dispose();
		buttonY.Dispose();
		buttonLeftStick.Dispose();
		buttonRightStick.Dispose();
		buttonLeftShoulder.Dispose();
		buttonRightShoulder.Dispose();
		spriteManager.Dispose();
		Fonts_Dispose();
	}

	public void HideSprites()
	{
		spriteVingette.visible = false;
		spriteTitle.visible = false;
		spriteHelp.visible = false;
		spriteMenu.visible = false;
		spriteRays.visible = false;
		spriteIcon.visible = false;
	}

	public void Render()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = (int)((float)height * 0.1f);
		int num2 = (int)((float)height * 0.8f);
		HideSprites();
		spriteVingette.visible = true;
		spriteVingette.scale.X = (float)width / spriteVingette.size.X;
		spriteVingette.scale.Y = (float)height / spriteVingette.size.Y;
		spriteButtonBackground.visible = DataManager.local.settings.showBuildHelpBar;
		spriteButtonBackground.position.X = ((float)width - spriteButtonBackground.size.X) * 0.5f;
		spriteButtonBackground.position.Y = (float)(num + num2) - spriteButtonBackground.size.Y - 8f;
		spriteRays.visible = true;
		spriteRays.position.X = (float)(width - spriteRays.texture.Width) * 0.5f;
		spriteRays.position.Y = 0f;
		spriteTitle.visible = true;
		spriteTitle.position.X = (float)width * 0.5f - 306f;
		spriteTitle.position.Y = -11f;
		spriteHelp.visible = true;
		spriteHelp.position.X = (float)width * 0.5f - 418f;
		spriteHelp.position.Y = num - 10;
		spriteMenu.visible = true;
		spriteMenu.position.X = (float)width * 0.5f + 318f;
		spriteMenu.position.Y = num - 11;
		status.Render();
		buttonA.Render();
		buttonB.Render();
		buttonX.Render();
		buttonY.Render();
		buttonLeftStick.Render();
		buttonRightStick.Render();
		buttonLeftShoulder.Render();
		buttonRightShoulder.Render();
		if (universe.mode != BuildUniverse.Modes.Edit)
		{
			if (universe.mode == BuildUniverse.Modes.Add)
			{
				RenderIcon(universe.painter.selected);
				return;
			}
			_ = universe.mode;
			_ = 2;
		}
	}

	public void RenderIcon(AtomDefinition oDef)
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = (int)((float)height * 0.1f);
		spriteIcon.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Atoms/Icons/" + oDef.name);
		spriteIcon.visible = true;
		spriteIcon.position.X = ((float)width - spriteIcon.size.X) * 0.5f;
		spriteIcon.position.Y = num + 68;
		Icon_Wait();
		status.SetState(status.state);
	}

	private void Icon_Wait()
	{
		iconFading = false;
		iconWaiting = true;
		iconTime = 0f;
		spriteIcon.tint = Color.White;
	}

	private void Icon_Update(GameTime oGameTime)
	{
		if (iconWaiting)
		{
			iconTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (iconTime >= 2000f)
			{
				iconTime = 0f;
				iconFading = true;
				iconWaiting = false;
			}
		}
		else if (iconFading)
		{
			iconTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (iconTime >= 500f)
			{
				iconFading = false;
				iconWaiting = false;
				spriteIcon.tint = Color.White;
				spriteIcon.visible = false;
			}
			else
			{
				spriteIcon.tint = Color.White * (1f - iconTime / 500f);
			}
		}
	}

	private void Fonts_Load()
	{
		fontKA_25 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_25");
		fontKA_18 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_18");
	}

	private void Fonts_Dispose()
	{
		fontKA_25 = null;
		fontKA_18 = null;
	}
}
