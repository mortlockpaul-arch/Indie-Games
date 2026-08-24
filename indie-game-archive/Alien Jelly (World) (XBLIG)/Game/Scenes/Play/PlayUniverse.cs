using System;
using GKEngine.Animation;
using GKEngine.Entities;
using GKEngine.Utils;
using Game.Atoms;
using Game.Data;
using Game.Dialogs;
using Game.Environment;
using Game.Grids;
using Game.History;
using Game.Interactable;
using Game.Particles;
using Game.Physics;
using Game.QBits;
using Game.Robots;
using Game.Scenes.Play.Players;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Play;

public class PlayUniverse : IReversible
{
	private const int DEATHS_MAX = 8;

	public static float FLIPPING_TIME = 1000f;

	public static float UPDATE_SLOW_INTERVAL = 5f;

	public PlayScene scene;

	public Sky sky;

	public PlayerManager players;

	public Grid grid;

	public PlayAtomManager atoms;

	public QBitManager qbits;

	public RobotManager robots;

	public InteractableManager interactables;

	public PhysicsManager physics;

	public HistoryManager history;

	public IntroManager intro;

	public PlayTitles titles;

	public bool paused;

	public bool preloaded;

	public PlayUniverseShadows shadows;

	public PlayUniverseDepth depth;

	private int updateSlowCount;

	private float updateSlowElapsed;

	public bool flipping;

	private float flippingTime;

	public Vector3 flippingAxis = default(Vector3);

	public int flippingAmount;

	public Matrix flipMatrix = Matrix.Identity;

	public bool historyLocked;

	public bool historyWasReversing;

	private Base3D focus;

	public uint jems;

	public uint deaths;

	public PlayUniverse(PlayScene oScene)
	{
		scene = oScene;
	}

	public void Init()
	{
		ParticleEmitter.Initialize();
		sky = new Sky(scene);
		grid = new Grid(-30, 30, -30, 30, -30, 30);
		intro = new IntroManager(scene);
		history = new HistoryManager(this);
		atoms = new PlayAtomManager(this);
		physics = new PhysicsManager(this);
		qbits = new QBitManager(this);
		robots = new RobotManager(this);
		interactables = new InteractableManager(this);
		scene.cameras.camera.Y = 500f;
		scene.cameras.camera.Update_View();
		sky.FromName(DataManager.level.sky, DataManager.level.particles);
		atoms.Atoms_FromData(DataManager.level);
		intro.FromData(DataManager.level);
		players = new PlayerManager(this);
		shadows = new PlayUniverseShadows(this);
		depth = new PlayUniverseDepth(this);
		scene.audio.music.Set(DataManager.level.music);
		Tracking_Reset();
		players.Input_Deactivate();
		titles = new PlayTitles(this);
		Update(new GameTime());
		players.camera.Refresh();
		players.camera.Set();
		paused = true;
		intro.SetStartCamera(players.camera);
		if (DataManager.header.type == 0)
		{
			titles.Start("Chapter " + (DataManager.header.group + 1) + ",  Level " + (DataManager.header.index + 1), DataManager.header.name);
		}
	}

	public void Update(GameTime elapsed)
	{
		history.Update(elapsed);
		if (history.reversing)
		{
			Reverse(elapsed);
			players.Update(elapsed);
		}
		else if (historyWasReversing)
		{
			historyWasReversing = false;
			Event_Reverse_End();
		}
		if (!paused)
		{
			players.Update(elapsed);
			sky.Update(elapsed);
			atoms.Update(elapsed);
			qbits.Update(elapsed);
			robots.Update(elapsed);
			interactables.Update(elapsed);
			physics.Update(elapsed);
			UpdateSlow(elapsed);
		}
		if (flipping)
		{
			Flip_Update(elapsed.ElapsedGameTime.Milliseconds);
			players.Update(elapsed);
		}
		intro.Update(elapsed);
		titles.Update(elapsed);
	}

	public void UpdateSlow(GameTime elapsed)
	{
		updateSlowCount++;
		updateSlowElapsed += (float)elapsed.ElapsedGameTime.TotalMilliseconds;
		if ((float)updateSlowCount >= UPDATE_SLOW_INTERVAL)
		{
			physics.UpdateSlow(updateSlowElapsed);
			updateSlowCount = 0;
			updateSlowElapsed = 0f;
		}
	}

	public void PreRender(GameTime oGameTime)
	{
		shadows.Render(oGameTime);
		depth.Render(oGameTime);
	}

	public void Dispose()
	{
		qbits.Dispose();
		robots.Dispose();
		interactables.Dispose();
		players.Dispose();
		sky.Dispose();
		physics.Dispose();
		atoms.Dispose();
		shadows.Dispose();
		depth.Dispose();
		history.Dispose();
		intro.Dispose();
		titles.Dispose();
	}

	public void Start()
	{
		players.camera.active = false;
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		players.Hint();
		intro.Start(delegate
		{
			players.Hint_Halt();
			players.Input_Activate();
			players.camera.active = true;
			qbits.Fall_Start();
			physics.Start();
			paused = false;
			if (DataManager.level.conversations != null && DataManager.level.conversations.Count > 0)
			{
				players.Input_Deactivate();
				qbits.conversation.Show(0, delegate
				{
					players.Input_Activate();
				}, -1);
			}
		});
	}

	public void Reverse(GameTime elapsed)
	{
		historyWasReversing = true;
		robots.Reverse(elapsed);
	}

	public void Flip(Vector3 vAxis, int xAmount, AtomSwitch oSwitch)
	{
		flippingTime = 0f;
		flippingAxis = vAxis;
		flippingAmount = xAmount;
		if (oSwitch.focus != null)
		{
			paused = true;
			focus = oSwitch.focus;
			intro.OneShot_Start(600f, delegate
			{
				Flip_Start();
			}, scene.cameras.camera.position, scene.cameras.camera.rotation, focus.position, focus.rotation);
		}
		else
		{
			Flip_Start();
		}
	}

	private void Flip_Start()
	{
		scene.audio.EventCues_Trigger("Special Event");
		Event_Flip_Start();
		Flip_Lerp(0f);
		flipping = true;
	}

	public void Flip_Update(int elapsed)
	{
		flippingTime += elapsed;
		if (flippingTime >= FLIPPING_TIME)
		{
			Flip_Lerp(1f);
			flipping = false;
			if (focus != null)
			{
				intro.OneShot_Start(600f, delegate
				{
					scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
					Event_Flip_End();
				}, scene.cameras.camera.position, scene.cameras.camera.rotation, players.camera.camera.position, players.camera.camera.rotation);
				focus = null;
			}
			else
			{
				Event_Flip_End();
			}
		}
		else
		{
			Flip_Lerp(flippingTime / FLIPPING_TIME);
		}
	}

	public void Flip_Lerp(float ratio)
	{
		float num = Tween.EaseInOut(ratio);
		atoms.Flip(flippingAxis, (float)flippingAmount * num);
		physics.Flip(flippingAxis, (float)flippingAmount * num);
		intro.Flip(flippingAxis, (float)flippingAmount * num);
	}

	public void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction)
	{
		if (oAction == HistoryItem.Action.Flip)
		{
			oItem.position = flippingAxis;
			oItem.value = flippingAmount;
		}
	}

	public void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		if (oItem.action == HistoryItem.Action.Flip)
		{
			flippingAxis = oItem.start.position;
			flippingAmount = (int)oItem.start.value * -1;
			Flip_Lerp(xRatio);
		}
	}

	public void History_Event_Lock()
	{
		historyLocked = true;
	}

	public void History_Event_Unlock()
	{
		historyLocked = false;
	}

	public void History_Event_Replayed(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Flip)
		{
			Event_Flip_End();
		}
	}

	public void History_Event_Reverse_Start(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Flip)
		{
			atoms.Event_Flip_Start();
			physics.Event_Flip_Start();
		}
	}

	public void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Flip)
		{
			atoms.Event_Flip_End();
			physics.Event_Flip_End();
		}
	}

	public bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		if (oAction != HistoryItem.Action.Flip)
		{
			return false;
		}
		return true;
	}

	public void History_Event_Resume(ref HistoryItem oItem)
	{
	}

	public virtual void History_Event_ForceClose(ref HistoryItem oItem)
	{
	}

	private void Tracking_Reset()
	{
		jems = 0u;
		deaths = 0u;
	}

	public bool Level_EndCheck(QBit oQBit)
	{
		bool result = false;
		if (qbits.DeadCount() > DataManager.level.expendable)
		{
			deaths++;
			scene.audio.EventCues_Trigger("Sound_Fail");
			history.Reverse(null);
			result = true;
		}
		else if (qbits.ActiveCount() <= 0)
		{
			Level_Win();
			result = true;
		}
		return result;
	}

	private void Level_Win()
	{
		scene.audio.EventCues_Trigger("Sound_Success");
		scene.dialogs.Show("PlayerWin");
		float num = Math.Max(8 - deaths, 0u);
		float num2 = (float)jems / (float)atoms.Jems_Total() * (num / 8f) * 5f;
		float num3 = (float)Math.Floor((float)players.ui.score * Math.Max(num, 1f));
		if (!DataManager.header.passed)
		{
			DataManager.header.passed = true;
		}
		DataProgression dataProgression = DataManager.Progression_Get((int)DataManager.levelIndex, DataManager.levelGroupIndex);
		if (dataProgression == null)
		{
			dataProgression = new DataProgression((int)DataManager.levelIndex, DataManager.levelGroupIndex, 0);
			DataManager.local.progression.Add(dataProgression);
		}
		if (num3 > (float)dataProgression.score)
		{
			dataProgression.score = (int)num3;
		}
		DataManager.PlayerData_Save(null, scene.dialogs.Message_Saving_Show, scene.dialogs.Message_Saving_Hide);
		DialogWin dialogWin = scene.dialogs.dialog as DialogWin;
		dialogWin.rewinds = history.count.ToString();
		dialogWin.jems = jems.ToString() + "/" + atoms.Jems_Total();
		dialogWin.deaths = deaths.ToString();
		dialogWin.score = MathUtils.Commas(num3, 3u);
		dialogWin.rating = (uint)num2;
		dialogWin.SetText();
	}

	public void Input_Update(GameTime oGameTime)
	{
		if (!historyLocked && !paused)
		{
			players.Input_Update(oGameTime);
		}
	}

	public void Event_Flip_Start()
	{
		paused = true;
		atoms.Event_Flip_Start();
		physics.Event_Flip_Start();
		intro.Flip_Start(atoms.inverse);
		history.Open(this, HistoryItem.Action.Flip);
	}

	public void Event_Flip_End()
	{
		history.Close(this, HistoryItem.Action.Flip);
		paused = false;
		atoms.Event_Flip_End();
		physics.Event_Flip_End();
		intro.Flip_End();
		qbits.Event_Flip_End();
	}

	public void Event_Reverse_End()
	{
		for (int i = 0; i < atoms.atoms.Count; i++)
		{
			if (atoms.atoms[i] is AtomSwitch)
			{
				(atoms.atoms[i] as AtomSwitch).StateCheckIfOn();
			}
		}
	}

	public void Event_Reverse_Start()
	{
		qbits.conversation.Halt();
	}
}
