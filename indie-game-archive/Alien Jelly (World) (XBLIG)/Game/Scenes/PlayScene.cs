using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using Game.Audio;
using Game.Data;
using Game.Dialogs;
using Game.Post;
using Game.Scenes.Play;
using Game.Scenes.Play.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes;

public class PlayScene : Scene
{
	public static PlayScene instance;

	public PlayUniverse universe;

	public DialogManager dialogs;

	public PostProcess_Dialog postDialog;

	public PostProcess_Dialog_Title postDialogRays;

	public PostProcess_Rewind postRewind;

	public PostProcess_Effects postEffects;

	public PostProcess_Splat postSplat;

	public PostProcess_Gamma postGamma;

	public GameAudio audio;

	public PlayScene()
		: base("Play")
	{
		instance = this;
		renderStacks.Add(new EntityStack(this, Material.State.Solid, GameMain.RENDERSTACK_SOLID, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Alpha, GameMain.RENDERSTACK_ALPHA_HARD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Add, GameMain.RENDERSTACK_ADD_FIRST, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.AlphaNoDepthWrite, GameMain.RENDERSTACK_ALPHA_UNSORTED, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Alpha, GameMain.RENDERSTACK_ALPHA_SORTED, xSort: true));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_MANUAL, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Add, GameMain.RENDERSTACK_ADD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.AlphaNoDepthWrite, GameMain.RENDERSTACK_ALPHA_LAST, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_UI, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_DIALOGS, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_GAMMA, xSort: false));
	}

	public override void Load()
	{
		library.FileLoad("Content/Data/Library_Play.xml");
		base.Load();
		GameEngine.Graphics.GraphicsDevice.SamplerStates[GameMain.REGISTER_DISTORT] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.Textures[GameMain.REGISTER_DISTORT] = library.texture2Ds["TextureDistort"];
		Init();
	}

	public override void Init()
	{
		base.Init();
		audio = new GameAudio(this, new Base3D());
		Init_Cameras();
		Init_Lights();
		universe = new PlayUniverse(this);
		universe.Init();
		Init_Dialogs();
		Post_Init();
		dialogs.Open("Loading");
		dialogs.Close(delegate
		{
			universe.Start();
		});
	}

	private void Init_Cameras()
	{
		cameras.camera.rotation = Quaternion.Identity * Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
		cameras.camera.position = new Vector3(0f, 0f, -200f);
		cameras.Add(new Camera(PlayerCamera.CAMERA_NAME, GameEngine.Graphics.GraphicsDevice.Viewport, cameras));
	}

	private void Init_Lights()
	{
		lights.primary.position = new Vector3(10000f, 10000f, -10000f);
		lights.primary.SetColor(250, 245, 205);
		lights.secondary.position = new Vector3(-10000f, 5000f, 5000f);
		lights.secondary.SetColor(32, 32, 64);
		lights.SetAmbientColor(0, 20, 64);
	}

	private void Init_Dialogs()
	{
		postDialog = new PostProcess_Dialog(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialog.Load();
		postDialog.amount = 0f;
		postDialog.active = false;
		postDialogRays = new PostProcess_Dialog_Title(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialogRays.Load();
		postDialogRays.amount = 0f;
		postDialogRays.active = false;
		dialogs = new DialogManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS), delegate(bool xPause)
		{
			universe.paused = xPause;
		}, new PostProcess[2] { postDialog, postDialogRays }, audio);
		DialogCatalog.Make_Loading(dialogs);
		DialogCatalog.Make_Settings(dialogs);
		DataLevelHeader header = DataManager.header;
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		if (header.edit)
		{
			list.Add(new DialogMenuOption("BUILD MODE", delegate
			{
				SwitchToBuild();
			}));
		}
		list.Add(new DialogMenuOption("SETTINGS", delegate
		{
			dialogs.Show("Settings");
		}));
		list.Add(new DialogMenuOption("HELP", delegate
		{
			dialogs.Show("PlayerHelp");
		}));
		list.Add(new DialogMenuOption("RESTART LEVEL", delegate
		{
			Reload();
		}));
		list.Add(new DialogMenuOption("EXIT TO TITLE", delegate
		{
			SwitchToMenu();
		}));
		DialogMenuPlay value = new DialogMenuPlay(dialogs, "PLAY MENU", "PLEASE SELECT AN OPTION BELOW, PRESSING A TO CONFIRM", list, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("exit menu", DialogMenuButtonLable.Button.B, delegate
			{
			})
		});
		dialogs.dialogs.Add("MainMenu", value);
		list = new List<DialogMenuOption>();
		if (header.edit)
		{
			list.Add(new DialogMenuOption("BUILD MODE", delegate
			{
				SwitchToBuild();
			}));
		}
		if (header.type == 0)
		{
			DataLevelHeader dataLevelHeader = DataManager.Levels_GetNextPlay();
			if (dataLevelHeader != null)
			{
				list.Add(new DialogMenuOption("NEXT LEVEL", delegate
				{
					NextLevel();
				}));
			}
			else
			{
				list.Add(new DialogMenuOption("FINISH IT!", delegate
				{
					SwitchToEnding();
				}));
			}
		}
		list.Add(new DialogMenuOption("RESTART LEVEL", delegate
		{
			Reload();
		}));
		list.Add(new DialogMenuOption("EXIT TO TITLE", delegate
		{
			SwitchToMenu();
		}));
		value = new DialogMenuPlay(dialogs, "WELL DONE!", "PLEASE SELECT AN OPTION", list, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null)
		});
		dialogs.dialogs.Add("WonMenu", value);
		Init_Dialogs_Help();
	}

	private void Init_Dialogs_Help()
	{
		DialogHelp dialogHelp = new DialogHelp(dialogs, new string[4] { "Content/UI/Dialogs/Play/Dialog_Play_Help_0", "Content/UI/Dialogs/Play/Dialog_Play_Help_1", "Content/UI/Dialogs/Play/Dialog_Play_Help_2", "Content/UI/Dialogs/Play/Dialog_Play_Help_3" });
		dialogHelp.postIndex = 1;
		dialogs.dialogs.Add("PlayerHelp", dialogHelp);
		DialogWin value = new DialogWin(dialogs);
		dialogs.dialogs.Add("PlayerWin", value);
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		dialogs.Update(oGameTime);
		universe.Update(oGameTime);
		audio.Update(oGameTime);
	}

	public override void PreRender(GameTime oGameTime)
	{
		if (universe != null)
		{
			universe.PreRender(oGameTime);
		}
	}

	public override void Exit()
	{
		audio.Dispose();
		GameMain.instance.Exit();
	}

	public override void Unload()
	{
		universe.Dispose();
		dialogs.Dispose();
		audio.Dispose();
		audio = null;
		Post_Dispose();
		UniversalInput.InputEntity_Flush(InputEntity.Scope.Scene);
		universe = null;
		dialogs = null;
		audio = null;
		base.Unload();
	}

	private void SwitchToBuild()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			GameMain.instance.Scene_Swap(GameMain.instance.sceneBuild);
		};
	}

	private void SwitchToMenu()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			GameMain.instance.Scene_Swap(GameMain.instance.sceneMenu);
		};
	}

	private void SwitchToEnding()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			GameMain.instance.sceneStory.storyMode = StoryScene.StoryMode.Ending;
			GameMain.instance.Scene_Swap(GameMain.instance.sceneStory);
		};
	}

	private void NextLevel()
	{
		DataLevelHeader oHeader = DataManager.Levels_GetNextPlay();
		if (oHeader == null)
		{
			return;
		}
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			DataManager.Levels_Load(oHeader.index, oHeader.type, oHeader.group, delegate
			{
				GameMain.instance.Scene_Swap(GameMain.instance.scenePlay);
			}, delegate
			{
				Console.WriteLine("Loading Next Level Failed");
			});
		};
	}

	private void Reload()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			GameMain.instance.Scene_Swap(GameMain.instance.scenePlay);
		};
	}

	private void Post_Init()
	{
		postRewind = new PostProcess_Rewind(RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		postRewind.Load();
		postRewind.amount = 1f;
		postRewind.active = false;
		postEffects = new PostProcess_Effects(RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		postEffects.Load();
		postEffects.amount = 1f;
		postEffects.active = true;
		universe.depth.targetEffectParam = postEffects.effectData.textureDepth;
		postSplat = new PostProcess_Splat(RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		postSplat.Load();
		postSplat.amount = 1f;
		postSplat.active = false;
		postGamma = new PostProcess_Gamma(RenderStacks_FromName(GameMain.RENDERSTACK_GAMMA));
		postGamma.Load();
	}

	private void Post_Dispose()
	{
		postDialog.Unload();
		postDialogRays.Unload();
		postRewind.Unload();
		postEffects.Unload();
		postSplat.Unload();
		postGamma.Unload();
		postDialog = null;
		postDialogRays = null;
		postRewind = null;
		postEffects = null;
		postSplat = null;
		postGamma = null;
	}

	public override void Input_Update(GameTime oGameTime)
	{
		dialogs.Input_Update(oGameTime);
		universe.Input_Update(oGameTime);
		base.Input_Update(oGameTime);
	}
}
