using System;
using System.Collections.Generic;
using GKEngine.Entities;
using Game.Atoms;
using Game.Audio;
using Game.Data;
using Game.Dialogs;
using Game.Environment;
using Game.Grids;
using Game.Scenes.Build.Players;
using Game.Scenes.Build.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Game.Scenes.Build;

public class BuildUniverse
{
	public enum Modes
	{
		Edit,
		Add,
		Camera,
		Cinimatic,
		Focus
	}

	private static Point LOADING_SPRITE_OFFSET = new Point(180, 101);

	private DataManager.DataCallback __save;

	public BuildScene scene;

	public Sky sky;

	public Player player;

	public Grid grid;

	public BuildAtomManager atoms;

	public AtomPainter painter;

	public IntroManager intro;

	public SpriteManager spriteManager;

	public BuildUI ui;

	public BuildGrid buildGrid;

	public bool paused;

	public bool preloaded;

	public Modes mode;

	private bool levelPassed;

	public BuildUniverseDepth depth;

	public BuildUniverse(BuildScene oScene)
	{
		scene = oScene;
		Init();
	}

	public void Init()
	{
		sky = new Sky(scene);
		grid = new Grid(-30, 30, -30, 30, -30, 30);
		buildGrid = new BuildGrid(this);
		atoms = new BuildAtomManager(scene, grid, this);
		painter = new AtomPainter(scene, atoms);
		intro = new IntroManager(scene);
		scene.cameras.camera.Y = 500f;
		scene.cameras.camera.Update_View();
		levelPassed = DataManager.header.passed;
		Levels_Set(DataManager.level);
		player = new Player(this);
		ui = new BuildUI(this);
		depth = new BuildUniverseDepth(this);
		Modes_SetEdit();
		Update(new GameTime());
		Pause();
	}

	public void Update(GameTime elapsed)
	{
		if (paused)
		{
			return;
		}
		if (!atoms.processing)
		{
			if (player != null)
			{
				player.Update(elapsed);
			}
			sky.Update(elapsed);
			atoms.Update(elapsed);
			ui.Update(elapsed);
			intro.Update(elapsed);
		}
		else
		{
			atoms.UpdateProcessing(elapsed);
		}
	}

	public void PreRender(GameTime oGameTime)
	{
		depth.Render(oGameTime);
	}

	public void Input_Update(GameTime oGameTime)
	{
		if (player != null && !paused && !atoms.processing)
		{
			player.Input_Update(oGameTime);
		}
	}

	public void Dispose()
	{
		depth.Dispose();
		ui.Dispose();
		intro.Dispose();
		buildGrid.Dispose();
		player.Dispose();
		painter.Dispose();
		sky.Dispose();
		atoms.Dispose();
	}

	public void Save(DataManager.DataCallback oCallback)
	{
		__save = oCallback;
		atoms.Atoms_ToData(DataManager.level);
		intro.ToData(DataManager.level);
		DataManager.Levels_Save(delegate
		{
			if (DataManager.header.passed != levelPassed)
			{
				DataManager.header.passed = levelPassed;
			}
			DataManager.PlayerData_Save(__save, scene.dialogs.Message_Saving_Show, scene.dialogs.Message_Saving_Hide);
		}, scene.dialogs.Message_Saving_Show, scene.dialogs.Message_Saving_Hide);
	}

	public void Pause()
	{
		paused = true;
		player.inputPaused = true;
	}

	public void Resume()
	{
		paused = false;
		player.inputPaused = false;
	}

	public void Start()
	{
		Resume();
		if (!DataManager.local.settings.seenHelp)
		{
			scene.dialogs.Show("BuildHelp", 1000f);
			DataManager.local.settings.seenHelp = true;
		}
	}

	public void CenterAll()
	{
		atoms.Atoms_CenterAll();
	}

	public void Levels_Set(DataLevel oLevel)
	{
		sky.FromName(oLevel.sky, oLevel.particles);
		atoms.Atoms_FromData(oLevel);
		intro.FromData(oLevel);
	}

	public void Levels_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.local.levels.Count; i++)
		{
			if (DataManager.local.levels[i].type == 1)
			{
				string text = DataManager.local.levels[i].name;
				if (DataManager.local.levels[i].index == DataManager.levelIndex && DataManager.local.levels[i].type == DataManager.levelType)
				{
					text = "[ " + text + " ]";
				}
				list.Add(new DialogMenuOption(text, delegate
				{
					uint[] array = (uint[])oMenu.options[oMenu.selectedIndex].data;
					scene.ChangeLevel(array[0], array[1]);
				}, new uint[2]
				{
					DataManager.local.levels[i].index,
					DataManager.local.levels[i].type
				}));
			}
		}
		oMenu.Options_Set(list);
	}

	public void Levels_Rename_Start(Dialog dialog)
	{
		DialogMenu dialogMenu = dialog as DialogMenu;
		uint[] array = (uint[])dialogMenu.options[dialogMenu.selectedIndex].data;
		DataLevelHeader dataLevelHeader = DataManager.Levels_FromIndex(array[0], array[1], -1);
		Guide.BeginShowKeyboardInput(PlayerIndex.One, "Rename Level", "Please enter the new level name below:", dataLevelHeader.name, Levels_Rename_End, dialog);
	}

	private void Levels_Rename_End(IAsyncResult result)
	{
		string text = Guide.EndShowKeyboardInput(result);
		DialogMenu dialogMenu = result.AsyncState as DialogMenu;
		uint[] array = (uint[])dialogMenu.options[dialogMenu.selectedIndex].data;
		DataLevelHeader dataLevelHeader = DataManager.Levels_FromIndex(array[0], array[1], -1);
		if (dataLevelHeader != null && text != null && text.Replace(" ", "").Length > 0)
		{
			dataLevelHeader.name = text;
			dialogMenu.options[dialogMenu.selectedIndex].stringTitle.SetText(text);
		}
		dialogMenu.Options_Show();
		dialogMenu.paused = false;
	}

	public void Levels_MarkAsEdited()
	{
		levelPassed = false;
	}

	public void Modes_SetAdd()
	{
		mode = Modes.Add;
		atoms.Select_Deselect();
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		painter.Show(player);
		player.shapeCursor.visible = true;
		ui.Render();
	}

	public void Modes_SetEdit()
	{
		mode = Modes.Edit;
		player.camera.mode = PlayerCamera.Mode.Rotate;
		atoms.Select_Deselect();
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		painter.Hide();
		player.shapeCursor.visible = false;
		ui.Render();
	}

	public void Modes_SetCamera_Start()
	{
		mode = Modes.Camera;
		player.camera.mode = PlayerCamera.Mode.Free;
		if (intro.index < intro.stack.Count)
		{
			player.camera.camera.position = intro.current.position;
			player.camera.camera.rotation = intro.current.rotation;
			player.camera.SetFromPos(intro.current.rotation);
		}
		atoms.Select_Deselect();
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		painter.Hide();
		player.shapeCursor.visible = false;
		buildGrid.visible = false;
		ui.Render();
	}

	public void Modes_SetCamera_End(bool xSuccess)
	{
		intro.Recording_End(xSuccess);
		buildGrid.visible = true;
		Modes_SetEdit();
		scene.dialogs.data = 0;
		scene.dialogs.Show("Build_Environment_Intro");
	}

	public void Modes_SetCinimatic()
	{
		mode = Modes.Cinimatic;
		player.camera.mode = PlayerCamera.Mode.Free;
		player.inputPaused = true;
		atoms.Select_Deselect();
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		painter.Hide();
		player.shapeCursor.visible = false;
		ui.Render();
		buildGrid.visible = false;
		intro.Start(delegate
		{
			buildGrid.visible = true;
			player.inputPaused = false;
			Modes_SetEdit();
			scene.dialogs.data = 2;
			scene.dialogs.Show("Build_Environment_Intro");
		});
	}

	public void Modes_SetFocus_Start()
	{
		AtomSwitch atomSwitch = atoms.over as AtomSwitch;
		mode = Modes.Focus;
		player.camera.mode = PlayerCamera.Mode.Free;
		if (atomSwitch.focus != null)
		{
			player.camera.camera.position = atomSwitch.focus.position;
			player.camera.camera.rotation = atomSwitch.focus.rotation;
		}
		atoms.Select_Deselect();
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		painter.Hide();
		player.shapeCursor.visible = false;
		buildGrid.visible = false;
		ui.Render();
	}

	public void Modes_SetFocus_End()
	{
		AtomSwitch atomSwitch = atoms.over as AtomSwitch;
		scene.postWhiteOut.active = true;
		scene.postWhiteOut.Anim_Out();
		atomSwitch.focus = new Base3D(scene.cameras.camera.position, scene.cameras.camera.rotation, new Vector3(1f));
		buildGrid.visible = true;
		Modes_SetEdit();
		scene.dialogs.data = 0;
		scene.dialogs.Show("AtomPropertyMenu");
	}

	public void Modes_SetFocus_Clear()
	{
		AtomSwitch atomSwitch = atoms.over as AtomSwitch;
		atomSwitch.focus = null;
		buildGrid.visible = true;
		Modes_SetEdit();
		scene.dialogs.data = 0;
		scene.dialogs.Show("AtomPropertyMenu");
	}

	public void Dialog_Menu_Tracks(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		string text = "No Music";
		if (DataManager.level.music == -1)
		{
			text = "[ " + text + " ]";
		}
		list.Add(new DialogMenuOption(text, delegate(Dialog dialog)
		{
			DataManager.level.music = -1;
			dialog.manager.data = oMenu.selectedIndex;
			dialog.manager.Show("Build_Environment_Music");
		}));
		for (int num = 0; num < scene.audio.music.tracks.Length; num++)
		{
			text = scene.audio.music.tracks[num].title.ToUpper();
			if (num == DataManager.level.music)
			{
				text = "[ " + text + " ]";
			}
			list.Add(new DialogMenuOption(text, delegate(Dialog dialog)
			{
				MusicTrack oTrack = oMenu.options[oMenu.selectedIndex].data as MusicTrack;
				DataManager.level.music = scene.audio.music.IndexOf(oTrack);
				dialog.manager.data = oMenu.selectedIndex;
				dialog.manager.Show("Build_Environment_Music");
			}, scene.audio.music.tracks[num]));
		}
		oMenu.Options_Set(list);
	}

	public void Dialog_Menu_Sky(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < DataManager.global.skys.Count; i++)
		{
			string text = DataManager.global.skys[i].name.ToUpper();
			if (DataManager.global.skys[i].name.ToLower() == DataManager.level.sky.ToLower())
			{
				text = "[ " + text + " ]";
			}
			list.Add(new DialogMenuOption(text, delegate(Dialog dialog)
			{
				DataSky dataSky = oMenu.options[oMenu.selectedIndex].data as DataSky;
				DataManager.level.sky = dataSky.name;
				scene.universe.sky.FromName(dataSky.name, DataManager.level.particles);
				dialog.manager.data = oMenu.selectedIndex;
				dialog.manager.Show("Build_Environment_Sky");
			}, DataManager.global.skys[i]));
		}
		oMenu.Options_Set(list);
	}

	public void Dialog_Menu_Particles(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		for (int i = 0; i < Sky.PARTICLES.Length; i++)
		{
			string text = Sky.PARTICLES[i].ToUpper();
			if (i == DataManager.level.particles)
			{
				text = "[ " + text + " ]";
			}
			list.Add(new DialogMenuOption(text, delegate(Dialog dialog)
			{
				DataManager.level.particles = oMenu.selectedIndex;
				scene.universe.sky.FromName(DataManager.level.sky, DataManager.level.particles);
				dialog.manager.data = oMenu.selectedIndex;
				dialog.manager.Show("Build_Environment_Particles");
			}, i));
		}
		oMenu.Options_Set(list);
	}
}
