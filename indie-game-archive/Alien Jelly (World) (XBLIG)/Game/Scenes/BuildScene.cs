using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using Game.Atoms;
using Game.Audio;
using Game.Data;
using Game.Dialogs;
using Game.Post;
using Game.Scenes.Build;
using Game.Scenes.Build.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes;

public class BuildScene : Scene
{
	public static BuildScene instance;

	private float frameRate;

	private int frameCounter;

	private TimeSpan elapsedTime = TimeSpan.Zero;

	public BuildUniverse universe;

	public DialogManager dialogs;

	public PostProcess_Dialog postDialog;

	public PostProcess_Dialog_Rings postDialogRings;

	public PostProcess_Effects postEffects;

	public PostProcess_WhiteOut postWhiteOut;

	public PostProcess_Gamma postGamma;

	public GameAudio audio;

	public BuildScene()
		: base("Build")
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
		library.FileLoad("Content/Data/Library_Build.xml");
		base.Load();
		GameEngine.Graphics.GraphicsDevice.SamplerStates[GameMain.REGISTER_DISTORT] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.Textures[GameMain.REGISTER_DISTORT] = library.texture2Ds["TextureDistort"];
		GameEngine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicWrap;
		Init();
	}

	public override void Init()
	{
		base.Init();
		audio = new GameAudio(this, new Base3D());
		Init_Dialogs();
		Init_Cameras();
		Init_Lights();
		universe = new BuildUniverse(this);
		Post_Init();
		dialogs.Open("Loading");
		dialogs.Close(delegate
		{
			universe.Start();
		});
	}

	private void Init_Dialogs()
	{
		postDialog = new PostProcess_Dialog(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialog.Load();
		postDialog.amount = 0f;
		postDialog.active = false;
		postDialogRings = new PostProcess_Dialog_Rings(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialogRings.Load();
		postDialogRings.amount = 0f;
		postDialogRings.active = false;
		dialogs = new DialogManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS), delegate(bool xPause)
		{
			universe.paused = xPause;
		}, new PostProcess[2] { postDialog, postDialogRings }, audio);
		DialogMenuBuild dialogMenuBuild = new DialogMenuBuild(dialogs, "BUILD MENU", "PLEASE SELECT AN OPTION", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("exit menu", DialogMenuButtonLable.Button.B, delegate
			{
			})
		});
		dialogMenuBuild.show = delegate(DialogMenu oMenu)
		{
			Dialog_BuildMenu_Make(oMenu);
		};
		dialogs.dialogs.Add("Build", dialogMenuBuild);
		dialogs.dialogs.Add("DeleteConfirm", new DialogMenuBuild(dialogs, "DELETE SELECTED?", "ARE YOU SURE YOU WANT TO DELETE THE SELECTED ATOMS?", new List<DialogMenuOption>
		{
			new DialogMenuOption("YES", delegate
			{
				universe.atoms.Atoms_Delete();
			}),
			new DialogMenuOption("NO", delegate
			{
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("cancel", DialogMenuButtonLable.Button.B, delegate
			{
			})
		}));
		DialogCatalog.Make_Build_CantPlay(dialogs);
		DialogMenuButtonLable dialogMenuButtonLable = new DialogMenuButtonLable("rename", DialogMenuButtonLable.Button.Y, delegate(Dialog dialog)
		{
			universe.Levels_Rename_Start(dialog);
		});
		dialogMenuButtonLable.actionImmediate = true;
		dialogMenuBuild = new DialogMenuBuild(dialogs, "LEVEL MENU", "Please select a level from the list on the right", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			dialogMenuButtonLable,
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				dialogs.data = 3;
				dialogs.Show("Build");
			})
		});
		dialogMenuBuild.show = delegate(DialogMenu oMenu)
		{
			universe.Levels_PopulateMenu(oMenu);
		};
		dialogs.dialogs.Add("LevelMenu", dialogMenuBuild);
		dialogMenuBuild = new DialogMenuBuild(dialogs, "", "", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("close", DialogMenuButtonLable.Button.B, delegate
			{
			})
		});
		dialogMenuBuild.show = delegate(DialogMenu oMenu)
		{
			AtomDefinition.Properties_PopulateMenu(oMenu as DialogMenuBuild, universe.atoms.over);
		};
		dialogs.dialogs.Add("AtomPropertyMenu", dialogMenuBuild);
		dialogMenuBuild = new DialogMenuBuild(dialogs, "", "", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				dialog.manager.data = dialog.data;
				dialog.manager.Show("AtomPropertyMenu");
			})
		});
		dialogMenuBuild.show = delegate(DialogMenu oMenu)
		{
			AtomDefinition.Properties_PopulateValueMenu(oMenu as DialogMenuBuild, universe.atoms.over);
		};
		dialogs.dialogs.Add("AtomPropertyValueMenu", dialogMenuBuild);
		dialogMenuBuild = new DialogMenuBuild(dialogs, "PART GROUP MENU", "Select a group of parts from the list below, \"A\" to view parts or \"B\" to go back", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("view parts", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				dialogs.data = 1;
				dialogs.Show("Build");
			})
		});
		dialogMenuBuild.show = delegate(DialogMenu oMenu)
		{
			universe.painter.Brushes_Sets_PopulateMenu(oMenu);
		};
		dialogs.dialogs.Add("BrushMenu", dialogMenuBuild);
		DialogIconMenu dialogIconMenu = new DialogIconMenu(dialogs, 6, 4, new List<DialogIconMenuOption>());
		dialogIconMenu.show = delegate(DialogIconMenu oMenu)
		{
			universe.painter.Brushes_PopulateMenu(oMenu);
		};
		dialogs.dialogs.Add("BrushSelectMenu", dialogIconMenu);
		DialogCatalog.Make_Loading(dialogs);
		Init_Dialogs_Help();
		DialogCatalog.Make_Build_Environment(dialogs);
		DialogCatalog.Make_Build_Environment_Intro(dialogs, delegate(DialogMenu oMenu)
		{
			universe.intro.Menu_PopulateOptions(oMenu);
		});
		DialogCatalog.Make_Build_Environment_Music(dialogs, delegate(DialogMenu oMenu)
		{
			universe.Dialog_Menu_Tracks(oMenu);
		});
		DialogCatalog.Make_Build_Environment_Sky(dialogs, delegate(DialogMenu oMenu)
		{
			universe.Dialog_Menu_Sky(oMenu);
		});
		DialogCatalog.Make_Build_Environment_Particles(dialogs, delegate(DialogMenu oMenu)
		{
			universe.Dialog_Menu_Particles(oMenu);
		});
	}

	private void Init_Dialogs_Help()
	{
		DialogHelp dialogHelp = new DialogHelp(dialogs, new string[11]
		{
			"Content/UI/Dialogs/Build/Dialog_Build_Help_0", "Content/UI/Dialogs/Build/Dialog_Build_Help_1", "Content/UI/Dialogs/Build/Dialog_Build_Help_2", "Content/UI/Dialogs/Build/Dialog_Build_Help_3", "Content/UI/Dialogs/Build/Dialog_Build_Help_4", "Content/UI/Dialogs/Build/Dialog_Build_Help_5", "Content/UI/Dialogs/Build/Dialog_Build_Help_6", "Content/UI/Dialogs/Build/Dialog_Build_Help_7", "Content/UI/Dialogs/Build/Dialog_Build_Help_8", "Content/UI/Dialogs/Build/Dialog_Build_Help_9",
			"Content/UI/Dialogs/Build/Dialog_Build_Help_10"
		});
		dialogHelp.postIndex = 1;
		dialogs.dialogs.Add("BuildHelp", dialogHelp);
	}

	private void Init_Cameras()
	{
		cameras.camera.rotation = Quaternion.Identity * Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
		cameras.camera.position = new Vector3(0f, 0f, -200f);
		cameras.Add(new Camera(PlayerCamera.CAMERA_NAME, GameEngine.Graphics.GraphicsDevice.Viewport, cameras));
	}

	private void Init_Lights()
	{
		lights.primary.position = new Vector3(5000f, 10000f, -5000f);
		lights.primary.SetColor(250, 245, 205);
		lights.secondary.position = new Vector3(-10000f, 5000f, 5000f);
		lights.secondary.SetColor(32, 32, 64);
		lights.SetAmbientColor(40, 20, 0);
	}

	public override void Update(GameTime oGameTime)
	{
		if (universe != null && dialogs != null && audio != null)
		{
			base.Update(oGameTime);
			elapsedTime += oGameTime.ElapsedGameTime;
			if (elapsedTime > TimeSpan.FromSeconds(1.0))
			{
				elapsedTime -= TimeSpan.FromSeconds(1.0);
				frameRate = frameCounter;
				frameCounter = 0;
			}
			dialogs.Update(oGameTime);
			universe.Update(oGameTime);
			audio.Update(oGameTime);
		}
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
		Post_Dispose();
		UniversalInput.InputEntity_Flush(InputEntity.Scope.Scene);
		base.Unload();
	}

	private void Dialog_BuildMenu_Make(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		if (universe.mode == BuildUniverse.Modes.Edit)
		{
			list.Add(new DialogMenuOption("ADD MODE", delegate
			{
				universe.Modes_SetAdd();
			}));
			list.Add(new DialogMenuOption("PLAY", delegate
			{
				SwitchToPlay();
			}));
			list.Add(new DialogMenuOption("SAVE", delegate
			{
				universe.Save(null);
			}));
			list.Add(new DialogMenuOption("LEVELS", delegate
			{
				dialogs.data = DataManager.levelIndex;
				dialogs.Show("LevelMenu");
			}));
			list.Add(new DialogMenuOption("ENVIRONMENT", delegate
			{
				dialogs.Show("Build_Environment");
			}));
			list.Add(new DialogMenuOption("EXIT TO TITLE", delegate
			{
				SwitchToMenu();
			}));
		}
		else if (universe.mode == BuildUniverse.Modes.Add)
		{
			list.Add(new DialogMenuOption("EDIT MODE", delegate
			{
				universe.Modes_SetEdit();
			}));
			list.Add(new DialogMenuOption("BUILD PARTS", delegate
			{
				dialogs.Show("BrushMenu");
			}));
		}
		oMenu.Options_Set(list);
	}

	private void Post_Init()
	{
		postEffects = new PostProcess_Effects(RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		postEffects.Load();
		postEffects.amount = 1f;
		postEffects.active = true;
		universe.depth.targetEffectParam = postEffects.effectData.textureDepth;
		postWhiteOut = new PostProcess_WhiteOut(RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		postWhiteOut.Load();
		postWhiteOut.amount = 1f;
		postWhiteOut.active = false;
		postGamma = new PostProcess_Gamma(RenderStacks_FromName(GameMain.RENDERSTACK_GAMMA));
		postGamma.Load();
	}

	private void Post_Dispose()
	{
		postEffects.Unload();
		postWhiteOut.Unload();
		postGamma.Unload();
		postDialog.Unload();
		postDialogRings.Unload();
		postEffects = null;
		postWhiteOut = null;
		postGamma = null;
		postDialog = null;
		postDialogRings = null;
	}

	private void SwitchToPlay()
	{
		if (universe.atoms.Atoms_Count_Type(AtomDefinition.Type.QBit) != 0 && universe.atoms.Atoms_Count_Type(AtomDefinition.Type.Exit) != 0)
		{
			dialogs.Show("Loading");
			(dialogs.dialog as DialogLoading).__opened = delegate
			{
				universe.Save(delegate
				{
					GameMain.instance.Scene_Swap(GameMain.instance.scenePlay);
				});
			};
		}
		else
		{
			dialogs.Show("Build_CantPlay");
		}
	}

	private void SwitchToMenu()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			universe.Save(delegate
			{
				GameMain.instance.Scene_Swap(GameMain.instance.sceneMenu);
			});
		};
	}

	public void ChangeLevel(uint xIndex, uint xType)
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			universe.Save(delegate
			{
				DataManager.Levels_Load(xIndex, xType, -1, delegate
				{
					GameMain.instance.Scene_Swap(GameMain.instance.sceneBuild);
				}, delegate
				{
					Console.WriteLine("Loading Build Level Failed: Build Scene");
				});
			});
		};
	}

	public override void Input_Set()
	{
		base.Input_Set();
		Player.Input_Set();
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (dialogs != null)
		{
			dialogs.Input_Update(oGameTime);
		}
		if (universe != null)
		{
			universe.Input_Update(oGameTime);
		}
		base.Input_Update(oGameTime);
	}
}
