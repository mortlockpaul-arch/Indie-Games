using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using GKEngine.Utils;
using Game.Audio;
using Game.Data;
using Game.Dialogs;
using Game.Environment;
using Game.Post;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace Game.Scenes;

public class MenuScene : Scene
{
	public enum ShareMode
	{
		Share,
		Download
	}

	private const float SKY_RADIUS = 3000f;

	private const float SKY_SPEED = 0.5f;

	private const int SHARING_MAX_LOCAL_GAMERS = 4;

	private const int SHARING_MAX_LEECHERS = 16;

	public static MenuScene instance;

	public DialogManager dialogs;

	public PostProcess_Dialog postDialog;

	public PostProcess_Dialog_Title postDialogTitle;

	public PostProcess_Gamma postGamma;

	private Sky sky;

	private bool skyActive;

	private Vector3 skyFocus;

	private Vector3 skyPositionFrom;

	private Vector3 skyPositionTo;

	private Quaternion skyRotation;

	private float skyTime;

	private float skyTimeTotal;

	private Vector3 skyAxis = new Vector3(0f, 1f, 0f);

	public GameAudio audio;

	public Base3D audioPoint;

	public bool ready;

	public bool showTitle = true;

	private bool sharingActive;

	private bool sharingIsLoaded;

	private bool sharingSigningIn;

	private uint sharingLevelIndex;

	private List<byte> sharingLevelData;

	private ShareMode sharingMode;

	private NetworkSession sharingSession;

	private DialogMenuPlay sharingDialog;

	private AvailableNetworkSessionCollection sharingSessions;

	private PacketWriter sharingPacketWriter = new PacketWriter();

	private PacketReader sharingPacketReader = new PacketReader();

	private DataShareReciever sharingDataReciever;

	public MenuScene()
		: base("Menu")
	{
		instance = this;
		renderStacks.Add(new EntityStack(this, Material.State.Solid, GameMain.RENDERSTACK_SOLID, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Alpha, GameMain.RENDERSTACK_ALPHA_HARD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Add, GameMain.RENDERSTACK_ADD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_DIALOGS, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_GAMMA, xSort: false));
	}

	public override void Load()
	{
		library.FileLoad("Content/Data/Library_Menu.xml");
		base.Load();
		GameEngine.Graphics.GraphicsDevice.SamplerStates[GameMain.REGISTER_DISTORT] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.Textures[GameMain.REGISTER_DISTORT] = library.texture2Ds["TextureDistort"];
		Init();
		ready = true;
	}

	public override void Init()
	{
		base.Init();
		audioPoint = new Base3D();
		audio = new GameAudio(this, audioPoint);
		Post_Init();
		Dialogs_Init();
		dialogs.Open("Loading");
		audio.music.Set(0);
		Init_Sky();
		dialogs.Close(delegate
		{
			if (DataManager.local.settings.screen.Width <= 0 || DataManager.local.settings.resolution.X != GameEngine.Graphics.GraphicsDevice.DisplayMode.Width)
			{
				dialogs.Show("Screen");
				(dialogs.dialog as DialogScreen).__completed = delegate
				{
					dialogs.Close(delegate
					{
						dialogs.Show("MainMenu");
					});
				};
			}
			else
			{
				dialogs.Show("MainMenu");
			}
		});
	}

	private void Init_Sky()
	{
		sky = new Sky(this);
		sky.FromName("Alpha Prime", 1);
		Sky_Start();
	}

	public override void Update(GameTime pGameTime)
	{
		base.Update(pGameTime);
		if (ready)
		{
			dialogs.Update(pGameTime);
			audio.Update(pGameTime);
			if (skyActive)
			{
				Sky_Update(pGameTime);
			}
			Share_Update(pGameTime);
		}
	}

	public override void Exit()
	{
		audio.music.Stop();
		audio.Dispose();
		GameMain.instance.Exit();
	}

	public override void Unload()
	{
		dialogs.Dispose();
		sky.Dispose();
		audio.Dispose();
		UniversalInput.InputEntity_Flush(InputEntity.Scope.Scene);
		base.Unload();
	}

	private void SwapToPlay(uint xIndex, uint xType, int xGroup)
	{
		if (DataManager.Levels_FromIndex(xIndex, xType, xGroup) != null && DataManager.Levels_FromIndex(xIndex, xType, xGroup).passed)
		{
			audio.music.Stop();
			dialogs.Show("Loading");
			(dialogs.dialog as DialogLoading).__opened = delegate
			{
				DataManager.Progression_GetNextPlayable(out var _, out var _);
				if (xIndex == 0 && xGroup == 0 && !Guide.IsTrialMode)
				{
					GameMain.instance.sceneStory.storyMode = StoryScene.StoryMode.Intro;
					GameMain.instance.Scene_Swap(GameMain.instance.sceneStory);
				}
				else
				{
					DataManager.Levels_Load(xIndex, xType, xGroup, delegate
					{
						GameMain.instance.Scene_Swap(GameMain.instance.scenePlay);
					}, delegate
					{
						Console.WriteLine("Loading Play Level Failed From Main Menu");
					});
				}
			};
		}
		else
		{
			dialogs.Show("MainMenu_CantPlay");
		}
	}

	private void SwapToBuild(uint xIndex, uint xType, int xGroup)
	{
		audio.music.Stop();
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			DataManager.Levels_Load(xIndex, xType, xGroup, delegate
			{
				GameMain.instance.Scene_Swap(GameMain.instance.sceneBuild);
			}, delegate
			{
				Console.WriteLine("Loading Build Level Failed From Main Menu");
			});
		};
	}

	private void Post_Init()
	{
		postDialog = new PostProcess_Dialog(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialog.Load();
		postDialog.amount = 0f;
		postDialog.active = false;
		postDialogTitle = new PostProcess_Dialog_Title(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialogTitle.Load();
		postDialogTitle.amount = 0f;
		postDialogTitle.active = false;
		postGamma = new PostProcess_Gamma(RenderStacks_FromName(GameMain.RENDERSTACK_GAMMA));
		postDialogTitle.amount = 1f;
		postGamma.Load();
	}

	private void Post_Dispose()
	{
		postDialog.Unload();
		postDialogTitle.Unload();
		postGamma.Unload();
		postDialog = null;
		postDialogTitle = null;
		postGamma = null;
	}

	private void Dialogs_Init()
	{
		dialogs = new DialogManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS), delegate
		{
		}, new PostProcess[2] { postDialog, postDialogTitle }, audio);
		DialogCatalog.Make_Loading(dialogs);
		DialogCatalog.Make_Screen(dialogs);
		DialogCatalog.Make_Error_Loading(dialogs);
		DialogMenu value = new DialogMenuTitle(dialogs, new List<DialogMenuOption>
		{
			new DialogMenuOption("PLAY", delegate
			{
				dialogs.Show("MainMenu_Play");
			}),
			new DialogMenuOption("BUILD", delegate
			{
				dialogs.Show("BuildLevelMenu");
			}),
			new DialogMenuOption("SHARE", delegate
			{
				dialogs.Show("ShareLevelMenu");
			}),
			new DialogMenuOption("CREDITS", delegate
			{
				dialogs.Show("Credits");
				(dialogs.dialog as DialogHelp).close = delegate
				{
					dialogs.Show("MainMenu");
				};
			}),
			new DialogMenuOption("SETTINGS", delegate
			{
				dialogs.Show("Settings");
			}),
			new DialogMenuOption("EXIT", delegate
			{
				GameMain.instance.Exit();
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select", DialogMenuButtonLable.Button.A, null)
		});
		dialogs.dialogs.Add("MainMenu", value);
		dialogs.dialogs.Add("Credits", new DialogHelp(dialogs, new string[1] { "Content/UI/Dialogs/Menu/Dialog_Menu_Credits" }));
		DialogCatalog.Make_Menu_Play(dialogs, this);
		DialogCatalog.Make_Menu_Play_Levels(dialogs, this);
		value = new DialogMenuPlay(dialogs, "BUILD ALIEN JELLY!", "PLEASE SELECT A LEVEL TO BUILD\nFROM THE LIST ON THE RIGHT", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("play", DialogMenuButtonLable.Button.X, delegate(Dialog dialog)
			{
				DialogMenu dialogMenu = dialog as DialogMenu;
				uint[] array = dialogMenu.options[dialogMenu.selectedIndex].data as uint[];
				SwapToPlay(array[0], array[1], -1);
			}),
			new DialogMenuButtonLable("build", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				dialogs.Show("MainMenu");
			})
		});
		value.show = delegate(DialogMenu oMenu)
		{
			Levels_Build_PopulateMenu(oMenu);
		};
		dialogs.dialogs.Add("BuildLevelMenu", value);
		value = new DialogMenuPlay(dialogs, "SHARE YOUR ALIEN JELLY!", "SEND AND RECIEVE LEVEL FILES TO AND FROM YOUR FRIENDS.", new List<DialogMenuOption>
		{
			new DialogMenuOption("SHARE A LEVEL", delegate
			{
				dialogs.Show("ShareLevelSelect");
			}),
			new DialogMenuOption("DOWNLOAD LEVELS", delegate
			{
				Share_Download_Start();
			}),
			new DialogMenuOption("PLAY DOWNLOADED", delegate
			{
				dialogs.Show("MainMenu_Share_Play");
			})
		}, new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("select option", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				dialogs.Show("MainMenu");
			})
		});
		dialogs.dialogs.Add("ShareLevelMenu", value);
		value = new DialogMenuPlay(dialogs, "SELECT A LEVEL", "please select one of your levels to share.", new List<DialogMenuOption>(), new List<DialogMenuButtonLable>
		{
			new DialogMenuButtonLable("share", DialogMenuButtonLable.Button.A, null),
			new DialogMenuButtonLable("where are my levels?", DialogMenuButtonLable.Button.X, delegate
			{
				dialogs.Show("MainMenu_Share_WhereAreMyLevels");
			}),
			new DialogMenuButtonLable("back", DialogMenuButtonLable.Button.B, delegate
			{
				dialogs.Show("ShareLevelMenu");
			})
		});
		value.show = delegate(DialogMenu oMenu)
		{
			Levels_Sharable_PopulateMenu(oMenu);
		};
		dialogs.dialogs.Add("ShareLevelSelect", value);
		DialogCatalog.Make_Menu_Sharing_Where(dialogs);
		DialogCatalog.Make_Menu_Sharing_Play(dialogs, this);
		DialogCatalog.Make_Menu_Sharing_DeleteConfirm(dialogs, this);
		DialogCatalog.Make_Menu_Sharing_PermissionError(dialogs);
		DialogCatalog.Make_Menu_Sharing_Error(dialogs);
		DialogCatalog.Make_Settings(dialogs);
		DialogCatalog.Make_Menu_CantPlay(dialogs);
		DialogCatalog.Make_Menu_Sharing_Share(dialogs);
	}

	public void Levels_Build_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.local.levels.Count; i++)
		{
			if (DataManager.local.levels[i].edit)
			{
				string text = DataManager.local.levels[i].name;
				if (DataManager.local.levels[i].index == DataManager.levelIndex && DataManager.local.levels[i].type == DataManager.levelType)
				{
					text = "[ " + text + " ]";
				}
				list.Add(new DialogMenuOption(text, delegate
				{
					uint[] array = (uint[])oMenu.options[oMenu.selectedIndex].data;
					SwapToBuild(array[0], array[1], -1);
				}, new uint[2]
				{
					DataManager.local.levels[i].index,
					DataManager.local.levels[i].type
				}));
			}
		}
		oMenu.Options_Set(list);
	}

	public void Levels_Play_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.global.groups.Count; i++)
		{
			DataManager.Progression_GetNextPlayable(out var _, out var xGroup);
			string text = DataManager.global.groups[i].title;
			if (DataManager.global.groups[i].index == DataManager.levelGroupIndex)
			{
				text = "[ " + text + " ]";
			}
			DialogMenuOption dialogMenuOption = new DialogMenuOption(text, delegate
			{
				int levelGroupIndex = (int)oMenu.options[oMenu.selectedIndex].data;
				DataManager.levelGroupIndex = levelGroupIndex;
				oMenu.manager.Show("MainMenu_Play_Levels");
			}, DataManager.global.groups[i].index);
			if (xGroup < i)
			{
				dialogMenuOption.deactivated = true;
			}
			list.Add(dialogMenuOption);
		}
		oMenu.Options_Set(list);
	}

	public void Levels_Play_Levels_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.global.levels.Count; i++)
		{
			if (DataManager.global.levels[i].group == DataManager.levelGroupIndex)
			{
				DataManager.Progression_GetNextPlayable(out var xIndex, out var xGroup);
				DataProgression dataProgression = DataManager.Progression_Get(i, DataManager.global.levels[i].group);
				if (dataProgression != null)
				{
					_ = "    (" + MathUtils.Commas(dataProgression.score, 3u) + ")";
				}
				string text = DataManager.global.levels[i].name;
				if (DataManager.global.levels[i].index == DataManager.levelIndex && DataManager.global.levels[i].type == DataManager.levelType)
				{
					text = "[ " + text + " ]";
				}
				DialogMenuOption dialogMenuOption = new DialogMenuOption(text, delegate
				{
					uint[] array = (uint[])oMenu.options[oMenu.selectedIndex].data;
					SwapToPlay(array[0], array[1], DataManager.levelGroupIndex);
				}, new uint[2]
				{
					DataManager.global.levels[i].index,
					DataManager.global.levels[i].type
				});
				dialogMenuOption.deactivated = xGroup <= DataManager.global.levels[i].group && xIndex < DataManager.global.levels[i].index;
				list.Add(dialogMenuOption);
			}
		}
		oMenu.Options_Set(list);
	}

	public void Levels_Downloaded_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.local.levels.Count; i++)
		{
			if (DataManager.local.levels[i].type == 2)
			{
				string text = DataManager.local.levels[i].author + "'s " + DataManager.local.levels[i].name;
				if (DataManager.local.levels[i].index == DataManager.levelIndex && DataManager.local.levels[i].type == DataManager.levelType)
				{
					text = "[ " + text + " ]";
				}
				list.Add(new DialogMenuOption(text, delegate
				{
					uint[] array = (uint[])oMenu.options[oMenu.selectedIndex].data;
					SwapToPlay(array[0], array[1], -1);
				}, new uint[2]
				{
					DataManager.local.levels[i].index,
					DataManager.local.levels[i].type
				}));
			}
		}
		oMenu.Options_Set(list);
	}

	public void Levels_Sharable_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.local.levels.Count; i++)
		{
			if (DataManager.local.levels[i].type == 1 && DataManager.local.levels[i].passed)
			{
				string text = DataManager.local.levels[i].author + "'s " + DataManager.local.levels[i].name;
				if (DataManager.local.levels[i].index == DataManager.levelIndex && DataManager.local.levels[i].type == DataManager.levelType)
				{
					text = "[ " + text + " ]";
				}
				list.Add(new DialogMenuOption(text, delegate
				{
					uint[] array = (uint[])oMenu.options[oMenu.selectedIndex].data;
					Share_ShareLevel_Start(array[0]);
				}, new uint[1] { DataManager.local.levels[i].index }));
			}
		}
		oMenu.Options_Set(list);
	}

	private void Share_ShareLevel_Start(uint xIndex)
	{
		Share_Halt();
		dialogs.Show("MainMenu_Share_Sharing");
		sharingDialog = dialogs.dialog as DialogMenuPlay;
		sharingDialog.title = "SHARING A LEVEL";
		sharingDialog.desc = "signing in...";
		sharingDialog.Lables_Dispose();
		sharingDialog.Lables_Refresh();
		sharingDialog.Options_Dispose();
		sharingDialog.Options_Refresh();
		sharingActive = true;
		sharingIsLoaded = false;
		sharingLevelIndex = xIndex;
		sharingMode = ShareMode.Share;
		sharingSigningIn = Share_CheckPermissions(sharingMode);
	}

	private void Share_ShareLevel_Start_Do()
	{
		DataManager.Levels_LoadAsData(sharingLevelIndex, 1u, delegate(byte[] pData)
		{
			sharingLevelData = new List<byte>(pData);
			Share_ShareLevel_SetSession();
		}, delegate
		{
			Share_ShareLevel_Halt(null, null);
		});
	}

	private void Share_ShareLevel_SetSession()
	{
		try
		{
			sharingDialog.desc = "making session";
			NetworkSessionProperties networkSessionProperties = new NetworkSessionProperties();
			networkSessionProperties[0] = (int)sharingLevelIndex;
			sharingSession = NetworkSession.Create(NetworkSessionType.PlayerMatch, 4, 16);
			Share_ShareLevel_SetEvents();
		}
		catch (Exception pError)
		{
			Share_ShareLevel_SetError(pError);
		}
	}

	private void Share_ShareLevel_RenderList()
	{
		sharingDialog.Options_Dispose();
		for (int i = 0; i < sharingSession.AllGamers.Count; i++)
		{
			if (!sharingSession.AllGamers[i].IsHost)
			{
				string text = "0";
				if (sharingSession.AllGamers[i].Tag is DataShareSender)
				{
					text = Math.Floor((float)(sharingSession.AllGamers[i].Tag as DataShareSender).byteIndex / (float)sharingLevelData.Count * 100f).ToString();
				}
				text += "%";
				string text2 = ((sharingSession.AllGamers[i].Gamertag.Length > 20) ? (sharingSession.AllGamers[i].Gamertag.Substring(0, 17) + "...") : sharingSession.AllGamers[i].Gamertag);
				sharingDialog.options.Add(new DialogMenuOption(text2 + " - " + text, null));
				sharingDialog.options[sharingDialog.options.Count - 1].autoCloseDialog = false;
			}
		}
		sharingDialog.Options_Refresh();
	}

	private void Share_ShareLevel_Update(GameTime pGameTime)
	{
		bool flag = false;
		LocalNetworkGamer localNetworkGamer = sharingSession.LocalGamers[0];
		while (localNetworkGamer.IsDataAvailable)
		{
			localNetworkGamer.ReceiveData(sharingPacketReader, out var sender);
			if (!sender.IsLocal)
			{
				DataShareSender dataShareSender = sender.Tag as DataShareSender;
				dataShareSender.ready = sharingPacketReader.ReadBoolean();
				flag = true;
			}
		}
		foreach (NetworkGamer allGamer in sharingSession.AllGamers)
		{
			if (allGamer.IsHost || allGamer.IsLocal)
			{
				continue;
			}
			DataShareSender dataShareSender = allGamer.Tag as DataShareSender;
			if (dataShareSender.ready)
			{
				if (dataShareSender.step == DataShareSender.Step.Header)
				{
					sharingPacketWriter.Write(DataManager.header.name);
					sharingPacketWriter.Write(sharingLevelData.Count);
					dataShareSender.ready = false;
					dataShareSender.step = DataShareSender.Step.Data;
					localNetworkGamer.SendData(sharingPacketWriter, SendDataOptions.ReliableInOrder, allGamer);
				}
				else if (dataShareSender.step == DataShareSender.Step.Data && dataShareSender.byteIndex < sharingLevelData.Count)
				{
					byte[] array = sharingLevelData.GetRange(dataShareSender.byteIndex, Math.Min(256, sharingLevelData.Count - dataShareSender.byteIndex)).ToArray();
					sharingPacketWriter.Write(array.Length);
					sharingPacketWriter.Write(array);
					dataShareSender.ready = false;
					dataShareSender.byteIndex += array.Length;
					dataShareSender.step = DataShareSender.Step.Data;
					localNetworkGamer.SendData(sharingPacketWriter, SendDataOptions.ReliableInOrder, allGamer);
				}
				else if (dataShareSender.step == DataShareSender.Step.Data && dataShareSender.byteIndex >= sharingLevelData.Count)
				{
					dataShareSender.step = DataShareSender.Step.Done;
				}
			}
		}
		if (flag)
		{
			Share_ShareLevel_RenderList();
		}
	}

	private void Share_ShareLevel_Halt(string xDialogName, object oDialogData)
	{
		if (xDialogName == null)
		{
			xDialogName = "ShareLevelMenu";
			oDialogData = 1;
		}
		if (sharingSession != null)
		{
			sharingSession.GamerJoined -= Event_Share_GamerJoined;
			sharingSession.GamerLeft -= Event_Share_GamerLeft;
			sharingSession.SessionEnded -= Event_Share_SessionEnded;
		}
		Share_Halt();
		sharingActive = false;
		sharingSigningIn = false;
		sharingIsLoaded = false;
		if (dialogs.dialog != null)
		{
			dialogs.Close(delegate
			{
				dialogs.Show(xDialogName);
				dialogs.data = oDialogData;
			});
		}
		else
		{
			dialogs.Show(xDialogName);
			dialogs.data = oDialogData;
		}
	}

	private void Share_ShareLevel_SetError(Exception pError)
	{
		sharingDialog.desc = "ERROR :( [" + pError.Message + "]";
		sharingDialog.Options_Dispose();
		sharingDialog.Options_Refresh();
		sharingDialog.lables.Clear();
		sharingDialog.lables.Add(new DialogMenuButtonLable("Close", DialogMenuButtonLable.Button.B, delegate
		{
			Share_ShareLevel_Halt(null, null);
		}));
		sharingDialog.Lables_Refresh();
	}

	private void Share_ShareLevel_SetEvents()
	{
		sharingDialog.desc = "ready, waiting for leechers";
		sharingDialog.lables.Clear();
		sharingDialog.lables.Add(new DialogMenuButtonLable("Stop Sharing", DialogMenuButtonLable.Button.B, delegate
		{
			Share_ShareLevel_Halt(null, null);
		}));
		sharingDialog.Lables_Refresh();
		sharingSession.GamerJoined += Event_Share_GamerJoined;
		sharingSession.GamerLeft += Event_Share_GamerLeft;
		sharingSession.SessionEnded += Event_Share_SessionEnded;
	}

	private void Share_Download_Start()
	{
		Share_Halt();
		dialogs.Show("MainMenu_Share_Sharing");
		sharingDialog = dialogs.dialog as DialogMenuPlay;
		sharingDialog.title = "FIND A LEVEL";
		sharingDialog.desc = "signing in...";
		sharingDialog.Lables_Dispose();
		sharingDialog.Lables_Refresh();
		sharingDialog.Options_Dispose();
		sharingDialog.Options_Refresh();
		sharingActive = true;
		sharingMode = ShareMode.Download;
		sharingSigningIn = Share_CheckPermissions(sharingMode);
	}

	private void Share_Download_Halt(string xDialogName, object oDialogData)
	{
		if (xDialogName == null)
		{
			xDialogName = "ShareLevelMenu";
			oDialogData = 1;
		}
		Share_Halt();
		sharingActive = false;
		sharingSigningIn = false;
		if (dialogs.dialog != null)
		{
			dialogs.Close(delegate
			{
				dialogs.Show(xDialogName);
				dialogs.data = oDialogData;
			});
		}
		else
		{
			dialogs.Show(xDialogName);
			dialogs.data = oDialogData;
		}
	}

	private void Share_Download_ShowSessions()
	{
		try
		{
			sharingDialog.desc = "finding people sharing levels";
			sharingSessions = NetworkSession.Find(NetworkSessionType.PlayerMatch, 4, null);
			Share_Download_RenderList();
		}
		catch (Exception ex)
		{
			sharingDialog.desc = "ERROR :( [" + ex.Message + "]";
		}
	}

	private void Share_Download_RenderList()
	{
		sharingDialog.Options_Dispose();
		sharingDialog.Lables_Dispose();
		if (sharingSessions == null || sharingSessions.Count == 0)
		{
			sharingDialog.desc = "No level shares found. Try Refreshing";
		}
		else
		{
			for (int i = 0; i < sharingSessions.Count; i++)
			{
				sharingDialog.options.Add(new DialogMenuOption(sharingSessions[i].HostGamertag, delegate(Dialog pDialog)
				{
					DialogMenuOption dialogMenuOption = (pDialog as DialogMenu).options[(pDialog as DialogMenu).selectedIndex];
					Share_Download_Connect(dialogMenuOption.data as AvailableNetworkSession);
				}, sharingSessions[i]));
				sharingDialog.options[sharingDialog.options.Count - 1].autoCloseDialog = false;
			}
			sharingDialog.lables.Add(new DialogMenuButtonLable("Download", DialogMenuButtonLable.Button.A, null));
		}
		sharingDialog.Options_Refresh();
		sharingDialog.lables.Add(new DialogMenuButtonLable("Back", DialogMenuButtonLable.Button.B, delegate
		{
			Share_Download_Halt(null, null);
		}));
		sharingDialog.lables.Add(new DialogMenuButtonLable("Refresh", DialogMenuButtonLable.Button.X, delegate(Dialog oDialog)
		{
			sharingDialog.desc = "Searching...";
			oDialog.manager.__oop = delegate
			{
				Share_Download_ShowSessions();
			};
		}, pImmediate: true));
		sharingDialog.paused = false;
		sharingDialog.Lables_Refresh();
	}

	private void Share_Download_RenderProgress_Start()
	{
		sharingDialog.Options_Dispose();
		sharingDialog.Lables_Dispose();
		sharingDialog.title = "Downloading Level...";
		sharingDialog.desc = "Now downloading level from " + sharingSession.Host.Gamertag;
		sharingDialog.options.Add(new DialogMenuOption("Starting...", null, null, dialogs.fontKA_60));
		sharingDialog.options[0].autoCloseDialog = false;
		sharingDialog.Options_Refresh();
		sharingDialog.lables.Add(new DialogMenuButtonLable("Cancel", DialogMenuButtonLable.Button.B, delegate
		{
			Share_Download_Halt(null, null);
		}));
		sharingDialog.Lables_Refresh();
	}

	private void Share_Download_RenderProgress_Update()
	{
		if (sharingSession != null)
		{
			string text = "";
			text = ((sharingDataReciever.header != null) ? (Math.Floor((float)sharingDataReciever.byteIndex / (float)sharingDataReciever.byteTotal * 100f) + "%") : "Initialising...");
			sharingDialog.options[0].SetTitle(text);
		}
	}

	private void Share_Download_Connect(AvailableNetworkSession pSession)
	{
		try
		{
			sharingSession = NetworkSession.Join(pSession);
			sharingDataReciever = new DataShareReciever();
			Share_Download_SetEvents();
			Share_Download_RenderProgress_Start();
		}
		catch (Exception)
		{
			Share_Download_ShowSessions();
		}
	}

	private void Share_Download_SetEvents()
	{
		sharingSession.SessionEnded += Event_Share_SessionEnded;
	}

	private void Share_Download_Update(GameTime pGameTime)
	{
		LocalNetworkGamer localNetworkGamer = sharingSession.LocalGamers[0];
		while (localNetworkGamer.IsDataAvailable)
		{
			localNetworkGamer.ReceiveData(sharingPacketReader, out var sender);
			if (!sender.IsHost)
			{
				continue;
			}
			if (sharingDataReciever.header == null)
			{
				sharingDataReciever.header = new DataLevelHeader(sharingPacketReader.ReadString(), sender.Gamertag, (uint)DataManager.Levels_Count(2u), 2u, xEdit: false, xPassed: true, -1);
				sharingDataReciever.byteIndex = 0;
				sharingDataReciever.byteTotal = sharingPacketReader.ReadInt32();
				sharingDataReciever.data = new byte[sharingDataReciever.byteTotal];
			}
			else if (sharingDataReciever.header != null)
			{
				int count = sharingPacketReader.ReadInt32();
				byte[] array = sharingPacketReader.ReadBytes(count);
				array.CopyTo(sharingDataReciever.data, sharingDataReciever.byteIndex);
				sharingDataReciever.byteIndex += array.Length;
				if (sharingDataReciever.byteIndex >= sharingDataReciever.byteTotal)
				{
					DataManager.Levels_SaveAsData(sharingDataReciever.data, sharingDataReciever.header.index, 2u, delegate
					{
						DataManager.local.levels.Add(sharingDataReciever.header);
						DataManager.PlayerData_Save(delegate
						{
							Share_Download_Halt(null, null);
						}, dialogs.Message_Saving_Show, dialogs.Message_Saving_Hide);
					});
				}
			}
			sharingPacketWriter.Write(value: true);
			localNetworkGamer?.SendData(sharingPacketWriter, SendDataOptions.ReliableInOrder, sender);
			Share_Download_RenderProgress_Update();
		}
	}

	public void Share_Play_Delete(uint xIndex)
	{
		sharingLevelIndex = xIndex;
		dialogs.Show("MainMenu_Share_DeleteConfirm");
	}

	public void Share_Play_Delete_Do()
	{
		DataLevelHeader item = DataManager.Levels_FromIndex(sharingLevelIndex, 2u, -1);
		DataManager.local.levels.Remove(item);
		DataManager.PlayerData_Save(delegate
		{
			DataManager.Levels_Delete(sharingLevelIndex, 2u, delegate
			{
				dialogs.Show("MainMenu_Share_Play");
			}, delegate
			{
				dialogs.Show("MainMenu_Share_Play");
			});
		}, dialogs.Message_Saving_Show, dialogs.Message_Saving_Hide);
	}

	private void Share_Halt()
	{
		if (sharingSession != null)
		{
			sharingSession.Dispose();
			sharingSession = null;
		}
	}

	private void Share_Update(GameTime pGameTime)
	{
		if (!sharingActive)
		{
			return;
		}
		if (!sharingSigningIn && !Share_CheckPermissions(sharingMode) && !Guide.IsVisible)
		{
			Share_Halt();
			sharingSigningIn = true;
			Guide.ShowSignIn(4, onlineOnly: true);
		}
		else if (sharingSigningIn && !Share_CheckPermissions(sharingMode) && !Guide.IsVisible)
		{
			Share_ShareLevel_Halt("MainMenu_Share_PermissionError", null);
		}
		else if (sharingSigningIn && Share_CheckPermissions(sharingMode) && !Guide.IsVisible)
		{
			sharingSigningIn = false;
			switch (sharingMode)
			{
			case ShareMode.Share:
				Share_ShareLevel_Start_Do();
				break;
			case ShareMode.Download:
				Share_Download_ShowSessions();
				break;
			}
		}
		else
		{
			if (sharingSession == null)
			{
				return;
			}
			try
			{
				sharingSession.Update();
				switch (sharingMode)
				{
				case ShareMode.Share:
					Share_ShareLevel_Update(pGameTime);
					break;
				case ShareMode.Download:
					Share_Download_Update(pGameTime);
					break;
				}
			}
			catch
			{
				Share_Halt();
				Share_ShareLevel_Halt("MainMenu_Share_Error", null);
			}
		}
	}

	private bool Share_CheckPermissions(ShareMode oMode)
	{
		bool result = false;
		for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
		{
			if (Gamer.SignedInGamers[i].PlayerIndex == (PlayerIndex)UniversalInput.gamePadPrimaryIndex && Gamer.SignedInGamers[i].Privileges.AllowOnlineSessions && ((oMode == ShareMode.Download && Gamer.SignedInGamers[i].Privileges.AllowUserCreatedContent != GamerPrivilegeSetting.Blocked) || oMode == ShareMode.Share))
			{
				result = true;
			}
		}
		return result;
	}

	private void Sky_Start()
	{
		skyFocus = cameras.camera._position;
		Sky_Set();
	}

	private void Sky_Set()
	{
		skyPositionFrom = skyFocus;
		skyPositionTo = GameMain.instance.GetRandUnitVecor() * 3000f;
		skyTime = 0f;
		skyTimeTotal = Vector3.Distance(skyPositionFrom, skyPositionTo) / 0.5f;
		skyActive = true;
	}

	private void Sky_Update(GameTime oGameTime)
	{
		skyTime += oGameTime.ElapsedGameTime.Milliseconds;
		sky.Update(oGameTime);
		if (skyTime >= skyTimeTotal)
		{
			Sky_Set();
		}
		else
		{
			Sky_Lerp(skyTime / skyTimeTotal, oGameTime.ElapsedGameTime.Milliseconds);
		}
	}

	private void Sky_Lerp(float xRatio, float oElapsed)
	{
		skyFocus = Vector3.Lerp(skyPositionFrom, skyPositionTo, xRatio);
		float amount = Math.Min(oElapsed / 10000f, 1f);
		cameras.camera.position = Vector3.Lerp(cameras.camera.position, skyFocus, amount);
		skyRotation = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(cameras.camera.position, skyFocus, Vector3.Up, Vector3.Forward)));
		cameras.camera.rotation = Quaternion.Lerp(cameras.camera.rotation, skyRotation, amount);
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (dialogs != null)
		{
			dialogs.Input_Update(oGameTime);
		}
		base.Input_Update(oGameTime);
	}

	private void Event_Share_GamerJoined(object sender, GamerJoinedEventArgs e)
	{
		if (sharingActive && sharingSession != null && sharingMode == ShareMode.Share)
		{
			e.Gamer.Tag = new DataShareSender();
			Share_ShareLevel_RenderList();
		}
	}

	private void Event_Share_GamerLeft(object sender, GamerLeftEventArgs e)
	{
		if (sharingActive && sharingSession != null && sharingMode == ShareMode.Share)
		{
			Share_ShareLevel_RenderList();
		}
	}

	private void Event_Share_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
	{
	}

	public override void Event_SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
	{
	}

	public override void Event_SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
	{
		if (sharingActive && !Share_CheckPermissions(sharingMode))
		{
			switch (sharingMode)
			{
			case ShareMode.Share:
				Share_ShareLevel_Halt("MainMenu_Share_PermissionError", null);
				break;
			case ShareMode.Download:
				Share_Download_Halt("MainMenu_Share_PermissionError", null);
				break;
			}
		}
	}
}
