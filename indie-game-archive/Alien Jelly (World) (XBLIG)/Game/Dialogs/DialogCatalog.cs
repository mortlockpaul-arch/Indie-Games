using System.Collections.Generic;
using GKEngine;
using Game.Data;
using Game.Scenes;
using Microsoft.Xna.Framework;

namespace Game.Dialogs;

public class DialogCatalog
{
	public const string NAME_SETTINGS = "Settings";

	public const string NAME_LOADING = "Loading";

	public const string NAME_CM = "CM";

	public const string NAME_START = "Start";

	public const string NAME_SCREEN = "Screen";

	public const string NAME_GAMMA = "Gamma";

	public const string NAME_ERROR_LOAD = "Error_Load";

	public const string NAME_ERROR_LOADPLAYERDATA = "Error_LoadPlayerData";

	public const string NAME_PLAY_HELP = "PlayerHelp";

	public const string NAME_MENU = "MainMenu";

	public const string NAME_MENU_CANTPLAY = "MainMenu_CantPlay";

	public const string NAME_MENU_SHARE = "MainMenu_Share";

	public const string NAME_MENU_SHARE_SHARING = "MainMenu_Share_Sharing";

	public const string NAME_MENU_SHARE_DELETECONFIRM = "MainMenu_Share_DeleteConfirm";

	public const string NAME_MENU_SHARE_PLAY = "MainMenu_Share_Play";

	public const string NAME_MENU_PLAY = "MainMenu_Play";

	public const string NAME_MENU_PLAY_LEVELS = "MainMenu_Play_Levels";

	public const string NAME_MENU_SHARE_WHERE = "MainMenu_Share_WhereAreMyLevels";

	public const string NAME_MENU_SHARE_PERMISSION_ERROR = "MainMenu_Share_PermissionError";

	public const string NAME_MENU_SHARE_ERROR = "MainMenu_Share_Error";

	public const string NAME_BUILD = "Build";

	public const string NAME_BUILD_ENVIRONMENT = "Build_Environment";

	public const string NAME_BUILD_CANTPLAY = "Build_CantPlay";

	public const string NAME_BUILD_ENVIRONMENT_INTRO = "Build_Environment_Intro";

	public const string NAME_BUILD_ENVIRONMENT_MUSIC = "Build_Environment_Music";

	public const string NAME_BUILD_ENVIRONMENT_SKY = "Build_Environment_Sky";

	public const string NAME_BUILD_ENVIRONMENT_PARTICLES = "Build_Environment_Particles";

	public const string NAME_BUILD_ENVIRONMENT_INTRO_SAVE = "Build_Environment_Intro_Save";

	public const string NAME_BUILD_ENVIRONMENT_INTRO_CLEAR = "Build_Environment_Intro_Clear";

	public static void Make_Loading(DialogManager oDialogManager)
	{
		DialogLoading value = new DialogLoading(oDialogManager);
		oDialogManager.dialogs.Add("Loading", value);
	}

	public static void Make_CM(DialogManager oDialogManager)
	{
		DialogCM value = new DialogCM(oDialogManager);
		oDialogManager.dialogs.Add("CM", value);
	}

	public static void Make_Start(DialogManager oDialogManager)
	{
		DialogStart value = new DialogStart(oDialogManager);
		oDialogManager.dialogs.Add("Start", value);
	}

	public static void Make_Screen(DialogManager oDialogManager)
	{
		DialogScreen value = new DialogScreen(oDialogManager);
		oDialogManager.dialogs.Add("Screen", value);
	}

	public static void Make_Settings(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("Music Volume < 0 >", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			int num = (int)dialogMenuOption10.data;
			DataManager.local.settings.volumeMusic = (int)MathHelper.Clamp(DataManager.local.settings.volumeMusic + num, 0f, 10f);
			dialog.manager.Utils_AudioUpdate();
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		}, 1);
		dialogMenuOption.hasHorizontal = true;
		dialogMenuOption.autoCloseDialog = false;
		dialogMenuOption.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			string text = "Music Volume: ";
			if (DataManager.local.settings.volumeMusic > 0 && DataManager.local.settings.volumeMusic < 10)
			{
				text = text + "< " + DataManager.local.settings.volumeMusic + " >";
			}
			else if (DataManager.local.settings.volumeMusic == 0)
			{
				text += "OFF >";
			}
			else if (DataManager.local.settings.volumeMusic == 10)
			{
				text += "< MAX";
			}
			me.SetTitle(text);
		};
		DialogMenuOption dialogMenuOption2 = new DialogMenuOption("Effects Volume  < 0 >", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			int num = (int)dialogMenuOption10.data;
			DataManager.local.settings.volumeFX = (int)MathHelper.Clamp(DataManager.local.settings.volumeFX + num, 0f, 10f);
			dialog.manager.Utils_AudioUpdate();
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		}, 1);
		dialogMenuOption2.hasHorizontal = true;
		dialogMenuOption2.autoCloseDialog = false;
		dialogMenuOption2.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			string text = "Effects Volume: ";
			if (DataManager.local.settings.volumeFX > 0 && DataManager.local.settings.volumeFX < 10)
			{
				text = text + "< " + DataManager.local.settings.volumeFX + " >";
			}
			else if (DataManager.local.settings.volumeFX == 0)
			{
				text += "OFF >";
			}
			else if (DataManager.local.settings.volumeFX == 10)
			{
				text += "< MAX";
			}
			me.SetTitle(text);
		};
		DialogMenuOption dialogMenuOption3 = new DialogMenuOption("Gamma  < 0 >", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			int num = (int)dialogMenuOption10.data;
			DataManager.local.settings.gamma = (int)MathHelper.Clamp(DataManager.local.settings.gamma + num, 0f, 10f);
			if (dialog.manager.scene is MenuScene)
			{
				(dialog.manager.scene as MenuScene).postGamma.UpdateGamma();
			}
			else if (dialog.manager.scene is PlayScene)
			{
				(dialog.manager.scene as PlayScene).postGamma.UpdateGamma();
			}
			else if (dialog.manager.scene is BuildScene)
			{
				(dialog.manager.scene as BuildScene).postGamma.UpdateGamma();
			}
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		}, 1);
		dialogMenuOption3.hasHorizontal = true;
		dialogMenuOption3.autoCloseDialog = false;
		dialogMenuOption3.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			string text = "Gamma : ";
			if (DataManager.local.settings.gamma > 0 && DataManager.local.settings.gamma < 10)
			{
				text = text + "< " + DataManager.local.settings.gamma + " >";
			}
			else if (DataManager.local.settings.gamma == 0)
			{
				text += "OFF >";
			}
			else if (DataManager.local.settings.gamma == 10)
			{
				text += "< MAX";
			}
			me.SetTitle(text);
		};
		DialogMenuOption dialogMenuOption4 = new DialogMenuOption("Move X: [ Inverted ]", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			DataManager.local.settings.moveInvertX = !DataManager.local.settings.moveInvertX;
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		});
		dialogMenuOption4.autoCloseDialog = false;
		dialogMenuOption4.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			me.SetTitle("Move X: [ " + (DataManager.local.settings.moveInvertX ? "Inverted" : "Normal") + " ]");
		};
		DialogMenuOption dialogMenuOption5 = new DialogMenuOption("Move Y: [ Inverted ]", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			DataManager.local.settings.moveInvertY = !DataManager.local.settings.moveInvertY;
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		});
		dialogMenuOption5.autoCloseDialog = false;
		dialogMenuOption5.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			me.SetTitle("Move Y: [ " + (DataManager.local.settings.moveInvertY ? "Inverted" : "Normal") + " ]");
		};
		DialogMenuOption dialogMenuOption6 = new DialogMenuOption("Play Camera Snapping: [ On ]", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			DataManager.local.settings.cameraSnapping = !DataManager.local.settings.cameraSnapping;
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		});
		dialogMenuOption6.autoCloseDialog = false;
		dialogMenuOption6.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			me.SetTitle("Play Camera Snapping: [ " + (DataManager.local.settings.cameraSnapping ? "On" : "Off") + " ]");
		};
		DialogMenuOption dialogMenuOption7 = new DialogMenuOption("Camera X: [ Inverted ]", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			DataManager.local.settings.cameraInvertX = !DataManager.local.settings.cameraInvertX;
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		});
		dialogMenuOption7.autoCloseDialog = false;
		dialogMenuOption7.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			me.SetTitle("Camera X: [ " + (DataManager.local.settings.cameraInvertX ? "Inverted" : "Normal") + " ]");
		};
		DialogMenuOption dialogMenuOption8 = new DialogMenuOption("Camera Y: [ Inverted ]", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			DataManager.local.settings.cameraInvertY = !DataManager.local.settings.cameraInvertY;
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		});
		dialogMenuOption8.autoCloseDialog = false;
		dialogMenuOption8.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			me.SetTitle("Camera Y: [ " + (DataManager.local.settings.cameraInvertY ? "Inverted" : "Normal") + " ]");
		};
		DialogMenuOption dialogMenuOption9 = new DialogMenuOption("Show Build Help Bar: [ Yes ]", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			DialogMenuOption dialogMenuOption10 = dialogMenu.options[dialogMenu.selectedIndex];
			DataManager.local.settings.showBuildHelpBar = !DataManager.local.settings.showBuildHelpBar;
			dialogMenuOption10.show(dialogMenu, dialogMenuOption10);
		});
		dialogMenuOption9.autoCloseDialog = false;
		dialogMenuOption9.show = delegate(DialogMenu menu, DialogMenuOption me)
		{
			me.SetTitle("Show Build Help Bar: [ " + (DataManager.local.settings.showBuildHelpBar ? "Yes" : "No") + " ]");
		};
		DialogMenuPlay value = new DialogMenuPlay(oDialogManager, "SETTINGS", "You can change your game settings here. Move the left stick side to side to change teh volumes", new List<DialogMenuOption> { dialogMenuOption, dialogMenuOption2, dialogMenuOption3, dialogMenuOption4, dialogMenuOption5, dialogMenuOption6, dialogMenuOption7, dialogMenuOption8, dialogMenuOption9 }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				DataManager.PlayerData_Save(delegate
				{
					if (dialog.manager.scene is MenuScene)
					{
						dialog.manager.data = 3;
						dialog.manager.Show("MainMenu");
					}
					else if (dialog.manager.scene is PlayScene)
					{
						dialog.manager.data = 2;
						dialog.manager.Show("MainMenu");
					}
				}, dialog.manager.Message_Saving_Show, dialog.manager.Message_Saving_Hide);
			})
		});
		oDialogManager.dialogs.Add("Settings", value);
	}

	public static void Make_Error_Loading(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("", delegate(Dialog dialog)
		{
			DialogMenu dialogMenu = dialog as DialogMenu;
			_ = dialogMenu.options[dialogMenu.selectedIndex];
			GameMain.instance.Exit();
		});
		dialogMenuOption.autoCloseDialog = false;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "LOADING ERROR", "Sorry but there was an error loading the main game data. Please make sure you have the appropriate storage devices plugged in and try again. Alien Jelly will now close.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("Gosh! Okay", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = -1;
		dialogMenuGeneric.optionsRender = false;
		oDialogManager.dialogs.Add("Error_Load", dialogMenuGeneric);
	}

	public static void Make_Error_LoadingPlayerData(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("", delegate
		{
			((IntroScene)GameEngine.scene).dialogs.Show("Start");
		});
		dialogMenuOption.autoCloseDialog = true;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "LOCAL DATA ERROR", "Sorry, but there was an error loading your player data. Please make sure you have the appropriate storage devices plugged in and try again.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("Okay I guess...", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = -1;
		dialogMenuGeneric.optionsRender = false;
		oDialogManager.dialogs.Add("Error_LoadPlayerData", dialogMenuGeneric);
	}

	public static void Make_Menu_CantPlay(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("OK", delegate(Dialog dialog)
		{
			dialog.manager.Show("MainMenu");
		});
		dialogMenuOption.autoCloseDialog = true;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "Level Not Ready", "To finish creating a level, make sure you have all the needed parts in it and can play it from the Build-o-Matic.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("okay", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = 1;
		dialogMenuGeneric.optionsRender = false;
		oDialogManager.dialogs.Add("MainMenu_CantPlay", dialogMenuGeneric);
	}

	public static void Make_Menu_Sharing_Share(DialogManager oDialogManager)
	{
		DialogMenuPlay value = new DialogMenuPlay(oDialogManager, "", "connecting......", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>());
		oDialogManager.dialogs.Add("MainMenu_Share_Sharing", value);
	}

	public static void Make_Menu_Sharing_Play(DialogManager oDialogManager, MenuScene oMenuScene)
	{
		DialogMenuPlay dialogMenuPlay = new DialogMenuPlay(oDialogManager, "DOWNLOADED LEVELS", "please select a downoaded level\nto play from the list below.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("play", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				oDialogManager.Show("ShareLevelMenu");
			}),
			new DialogMenuButtonLable("delete", DialogMenuButtonLable.Button.Y, delegate(Dialog dialog)
			{
				DialogMenu dialogMenu = dialog as DialogMenu;
				if (dialogMenu.options.Count > 0)
				{
					uint[] array = (uint[])dialogMenu.options[dialogMenu.selectedIndex].data;
					oMenuScene.Share_Play_Delete(array[0]);
				}
				else
				{
					oDialogManager.Show("MainMenu_Share_Play");
				}
			})
		});
		dialogMenuPlay.show = delegate(DialogMenu oMenu)
		{
			oMenuScene.Levels_Downloaded_PopulateMenu(oMenu);
		};
		oDialogManager.dialogs.Add("MainMenu_Share_Play", dialogMenuPlay);
	}

	public static void Make_Menu_Sharing_Where(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("OK", delegate(Dialog dialog)
		{
			dialog.manager.Show("ShareLevelSelect");
		});
		dialogMenuOption.autoCloseDialog = true;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "Where are my levels?", "Levels you have made will only appear here if they have been successfully played through. You will need to play through your level every time you modify it.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("okay", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = 1;
		dialogMenuGeneric.optionsRender = false;
		oDialogManager.dialogs.Add("MainMenu_Share_WhereAreMyLevels", dialogMenuGeneric);
	}

	public static void Make_Menu_Sharing_DeleteConfirm(DialogManager oDialogManager, MenuScene oMenuScene)
	{
		DialogMenuPlay value = new DialogMenuPlay(oDialogManager, "DELETE LEVEL?", "ARE YOU SURE YOU WANT TO DELETE THE SELECTED LEVEL?", new List<DialogMenuOption>
		{
			new DialogMenuOption("YES", delegate
			{
				oMenuScene.Share_Play_Delete_Do();
			}),
			new DialogMenuOption("NO", delegate
			{
				oDialogManager.Show("MainMenu_Share_Play");
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null)
		});
		oDialogManager.dialogs.Add("MainMenu_Share_DeleteConfirm", value);
	}

	public static void Make_Menu_Sharing_PermissionError(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("OK", delegate(Dialog dialog)
		{
			dialog.manager.Show("ShareLevelMenu");
		});
		dialogMenuOption.autoCloseDialog = true;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "XBox Live Profile Problem", "Unfortunately there was an error with your XBox Live gamer profile. You are either not signed in with a XBox Live Gold account or do not have the permission to access content from other gamers over the network. Levels can only be downloaded or shared between XBox Live Gold members.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("okay", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = 1;
		dialogMenuGeneric.optionsRender = false;
		oDialogManager.dialogs.Add("MainMenu_Share_PermissionError", dialogMenuGeneric);
	}

	public static void Make_Menu_Sharing_Error(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("OK", delegate(Dialog dialog)
		{
			dialog.manager.Show("ShareLevelMenu");
		});
		dialogMenuOption.autoCloseDialog = true;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "Sharing Problem", "Unfortunately there was a connection error while trying to download or share a level. This may be due to the host disconnecting or a network connection problem on either end. Please try again.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("okay", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = 1;
		dialogMenuGeneric.optionsRender = false;
		oDialogManager.dialogs.Add("MainMenu_Share_Error", dialogMenuGeneric);
	}

	public static void Make_Build_Environment(DialogManager oDialogManager)
	{
		DialogMenuBuild value = new DialogMenuBuild(oDialogManager, "Environment Options", "Please select an environment option.", new List<DialogMenuOption>
		{
			new DialogMenuOption("INTRO CINIMATIC", delegate
			{
				oDialogManager.Show("Build_Environment_Intro");
			}),
			new DialogMenuOption("MUSIC", delegate
			{
				oDialogManager.Show("Build_Environment_Music");
			}),
			new DialogMenuOption("SKY", delegate
			{
				oDialogManager.Show("Build_Environment_Sky");
			}),
			new DialogMenuOption("PARTICLES", delegate
			{
				oDialogManager.Show("Build_Environment_Particles");
			}),
			new DialogMenuOption("Center All", delegate(Dialog dialog)
			{
				(dialog.manager.scene as BuildScene).universe.CenterAll();
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				dialog.manager.data = 4;
				dialog.manager.Show("Build");
			})
		});
		oDialogManager.dialogs.Add("Build_Environment", value);
	}

	public static void Make_Build_Environment_Intro(DialogManager oDialogManager, DialogMenu.MenuDialogShowDelegate oShowDelegate)
	{
		DialogMenuBuild dialogMenuBuild = new DialogMenuBuild(oDialogManager, "CAMERA INTRO", "Please select a camera intro option.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				dialog.manager.data = 0;
				dialog.manager.Show("Build_Environment");
			})
		});
		dialogMenuBuild.show = oShowDelegate;
		oDialogManager.dialogs.Add("Build_Environment_Intro", dialogMenuBuild);
		Make_Build_Environment_Intro_Save(oDialogManager);
		Make_Build_Environment_Intro_Clear(oDialogManager);
	}

	public static void Make_Build_Environment_Intro_Save(DialogManager oDialogManager)
	{
		DialogMenuBuild value = new DialogMenuBuild(oDialogManager, "Save New Cinimatic?", "Do you want to save your new intro cinimatic?", new List<DialogMenuOption>
		{
			new DialogMenuOption("YES", delegate
			{
				(oDialogManager.scene as BuildScene).universe.Modes_SetCamera_End(xSuccess: true);
			}),
			new DialogMenuOption("No", delegate
			{
				(oDialogManager.scene as BuildScene).universe.Modes_SetCamera_End(xSuccess: false);
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null)
		});
		oDialogManager.dialogs.Add("Build_Environment_Intro_Save", value);
	}

	public static void Make_Build_Environment_Intro_Clear(DialogManager oDialogManager)
	{
		DialogMenuBuild value = new DialogMenuBuild(oDialogManager, "Clear Cinimatic?", "Are you sure you want to clear your intro cinimatic?", new List<DialogMenuOption>
		{
			new DialogMenuOption("YES", delegate(Dialog dialog)
			{
				(oDialogManager.scene as BuildScene).universe.intro.Clear();
				dialog.manager.data = 1;
				dialog.manager.Show("Build_Environment_Intro");
			}),
			new DialogMenuOption("No", delegate(Dialog dialog)
			{
				dialog.manager.data = 1;
				dialog.manager.Show("Build_Environment_Intro");
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null)
		});
		oDialogManager.dialogs.Add("Build_Environment_Intro_Clear", value);
	}

	public static void Make_Build_Environment_Music(DialogManager oDialogManager, DialogMenu.MenuDialogShowDelegate oShowDelegate)
	{
		DialogMenuBuild dialogMenuBuild = new DialogMenuBuild(oDialogManager, "MUSIC TRACK", "Please select a music track you would like to play for this level", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				dialog.manager.data = 1;
				dialog.manager.Show("Build_Environment");
			})
		});
		dialogMenuBuild.show = oShowDelegate;
		oDialogManager.dialogs.Add("Build_Environment_Music", dialogMenuBuild);
	}

	public static void Make_Build_Environment_Sky(DialogManager oDialogManager, DialogMenu.MenuDialogShowDelegate oShowDelegate)
	{
		DialogMenuBuild dialogMenuBuild = new DialogMenuBuild(oDialogManager, "SKY & LIGHTING", "Please select a sky and lighting scheme for this level.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				dialog.manager.data = 2;
				dialog.manager.Show("Build_Environment");
			})
		});
		dialogMenuBuild.show = oShowDelegate;
		oDialogManager.dialogs.Add("Build_Environment_Sky", dialogMenuBuild);
	}

	public static void Make_Build_Environment_Particles(DialogManager oDialogManager, DialogMenu.MenuDialogShowDelegate oShowDelegate)
	{
		DialogMenuBuild dialogMenuBuild = new DialogMenuBuild(oDialogManager, "SKY PARTICLES", "Please select an environment particle scheme for this level.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate(Dialog dialog)
			{
				dialog.manager.data = 3;
				dialog.manager.Show("Build_Environment");
			})
		});
		dialogMenuBuild.show = oShowDelegate;
		oDialogManager.dialogs.Add("Build_Environment_Particles", dialogMenuBuild);
	}

	public static void Make_Build_CantPlay(DialogManager oDialogManager)
	{
		DialogMenuOption dialogMenuOption = new DialogMenuOption("OK", delegate(Dialog dialog)
		{
			dialog.manager.Show("Build");
		});
		dialogMenuOption.autoCloseDialog = true;
		DialogMenuGeneric dialogMenuGeneric = new DialogMenuGeneric(oDialogManager, "Level Is Not Ready", "Your level needs to have an Alien Jelly and an exit before you can play it. Put them in and try again.", new List<DialogMenuOption> { dialogMenuOption }, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select", DialogMenuButtonLable.Button.A, null)
		});
		dialogMenuGeneric.postIndex = 0;
		oDialogManager.dialogs.Add("Build_CantPlay", dialogMenuGeneric);
	}

	public static void Make_Menu_Play(DialogManager oDialogManager, MenuScene oMenuScene)
	{
		DialogMenuPlay dialogMenuPlay = new DialogMenuPlay(oDialogManager, "CHAPTERS", "please select a level chapter to play\nfrom the list below.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				oDialogManager.Show("MainMenu");
			})
		});
		dialogMenuPlay.show = delegate(DialogMenu oMenu)
		{
			oMenuScene.Levels_Play_PopulateMenu(oMenu);
		};
		oDialogManager.dialogs.Add("MainMenu_Play", dialogMenuPlay);
	}

	public static void Make_Menu_Play_Levels(DialogManager oDialogManager, MenuScene oMenuScene)
	{
		DialogMenuPlay dialogMenuPlay = new DialogMenuPlay(oDialogManager, "PLAY ALIEN JELLY!", "please select a level to play\nfrom the list below.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("play", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				oDialogManager.Show("MainMenu_Play");
			})
		});
		dialogMenuPlay.show = delegate(DialogMenu oMenu)
		{
			oMenuScene.Levels_Play_Levels_PopulateMenu(oMenu);
		};
		oDialogManager.dialogs.Add("MainMenu_Play_Levels", dialogMenuPlay);
	}
}
