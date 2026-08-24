using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Grids;
using Game.History;
using Game.QBits;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomFilter : AtomSingle, IGridable, IRenderable, IReversible
{
	private const float EFFECT_TIME_TOTAL = 10000f;

	public static Color[] COLOR = new Color[4]
	{
		new Color(32, 0, 8),
		new Color(0, 32, 8),
		new Color(0, 8, 32),
		new Color(32, 24, 0)
	};

	public static Color[] COLOR_PULSE = new Color[4]
	{
		new Color(255, 0, 64),
		new Color(0, 255, 64),
		new Color(0, 102, 255),
		new Color(255, 190, 0)
	};

	public static int[] PROPERTY_TIME_WAIT = new int[8] { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000 };

	public static string TITLE = "Jelly Filter";

	public static string DESCRIPTION = "This is a block that allows only a single color of Alien Jelly to land on it. If the Jelly's color does not match the block, it will get destroyed. This item has properties.";

	public static string PROPERTIES_DESCRIPTION = "This filter block has the following properties that allow you to control its color and its timing.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[4]
	{
		new AtomProperty("Second Collor", "This option allows you to set the type of Alein Jelly allowed on this block.", new string[5] { "None", "Red", "Green", "Blue", "Yellow" }),
		new AtomProperty("Third Collor", "This option allows you to set the type of Alein Jelly allowed on this block.", new string[5] { "None", "Red", "Green", "Blue", "Yellow" }),
		new AtomProperty("Forth Collor", "This option allows you to set the type of Alein Jelly allowed on this block.", new string[5] { "None", "Red", "Green", "Blue", "Yellow" }),
		new AtomProperty("Change Time", "This option allows you to set the time in between color changes.", new string[8] { "1 Second", "2 Seconds", "3 Seconds", "4 Seconds", "5 Seconds", "6 Seconds", "7 Seconds", "8 Seconds" })
	};

	public static int[] PROPERTIES_DEFAULT;

	private QBit.QBitType qbitBase;

	private QBit.QBitType[] qbits;

	private MaxModelPart partModel;

	private MaxModelPartRenderable partPlanes;

	private float effectTime;

	private EffectParameter effectColor;

	private EffectParameter effectPulseColor;

	private EffectParameter effectTimeValue;

	private EffectParameter effectDataModel;

	private EffectParameter effectDataPlanes;

	private int time;

	private int timeTotal;

	private int index;

	private Vector4 dataOld = default(Vector4);

	private PlayScene scenePlay;

	public override int[] properties
	{
		get
		{
			return base.properties;
		}
		set
		{
			base.properties = value;
			if (value.Length <= 0)
			{
				return;
			}
			int num = 1;
			for (int i = 0; i < 3; i++)
			{
				if (value[i] > 0)
				{
					num++;
				}
			}
			qbits = new QBit.QBitType[num];
			num = 1;
			qbits[0] = qbitBase;
			for (int i = 0; i < 3; i++)
			{
				if (value[i] > 0)
				{
					qbits[num] = (QBit.QBitType)(value[i] - 1);
					num++;
				}
			}
			timeTotal = PROPERTY_TIME_WAIT[value[3]];
		}
	}

	public QBit.QBitType qbit => qbits[index];

	public AtomFilter(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
		effectTime = (float)GameEngine.random.NextDouble() * 10000f;
	}

	public override void InitPlay()
	{
		base.InitPlay();
		time = 0;
		index = 0;
		scenePlay = manager.scene as PlayScene;
		if (qbits.Length > 1)
		{
			scenePlay.universe.history.Open(this, HistoryItem.Action.Property);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(partPlanes.guid.value, partPlanes);
		effectColor = null;
		effectPulseColor = null;
		effectTimeValue = null;
		partModel.Dispose();
		partModel = null;
		partPlanes.Dispose();
		partPlanes = null;
	}

	public override void Load()
	{
		useMaterials = false;
		base.Load();
		partModel = model.PartFromName("Model");
		partPlanes = new MaxModelPartRenderable(manager.scene, this, model.PartFromName("Planes"));
		qbitBase = (definition as AtomFilterDefinition).qbit;
		effectColor = partPlanes.part.material.effect.Parameters["Color"];
		effectPulseColor = partPlanes.part.material.effect.Parameters["PulseColor"];
		effectTimeValue = partPlanes.part.material.effect.Parameters["Time"];
		effectDataModel = partModel.material.effect.Parameters["data"];
		effectDataPlanes = partPlanes.part.material.effect.Parameters["data"];
		effectColor.SetValue(COLOR[(int)qbitBase].ToVector3());
		effectPulseColor.SetValue(COLOR_PULSE[(int)qbitBase].ToVector3());
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(partPlanes.guid.value, partPlanes);
	}

	public override void Update(GameTime oGameTime)
	{
		if (visible && dataOld != data)
		{
			effectDataModel.SetValue(data);
			effectDataPlanes.SetValue(data);
			dataOld = data;
		}
		if (effectTimeValue != null)
		{
			effectTimeValue.SetValue((float)oGameTime.TotalGameTime.TotalMilliseconds);
		}
		if (qbits.Length > 1)
		{
			time += oGameTime.ElapsedGameTime.Milliseconds;
			if (time >= timeTotal)
			{
				scenePlay.universe.history.Close(this, HistoryItem.Action.Property);
				index++;
				index %= qbits.Length;
				time %= timeTotal;
				KillCheck();
				Color_Set();
				scenePlay.universe.history.Open(this, HistoryItem.Action.Property);
			}
		}
		base.Update(oGameTime);
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			Camera camera = manager.scene.cameras.camera;
			partModel.Render(ref _matrix, camera);
		}
	}

	private void KillCheck()
	{
		if (manager.scene is PlayScene playScene)
		{
			QBit qBit = playScene.universe.qbits.At(X, Y + Grid.SPACING.Y, Z, null);
			if (qBit != null && qBit.type != qbit && !qBit.home && !qBit.dead && !qBit.busy)
			{
				qBit.Death();
			}
		}
	}

	private void Color_Set()
	{
		effectColor.SetValue(COLOR[(int)qbits[index]].ToVector3());
		effectPulseColor.SetValue(COLOR_PULSE[(int)qbits[index]].ToVector3());
	}

	public void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction)
	{
		if (oAction == HistoryItem.Action.Property)
		{
			oItem.index = index;
			oItem.value = time;
		}
	}

	public virtual void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		if (oItem.action == HistoryItem.Action.Property)
		{
			time = (int)(oItem.start.value + (oItem.end.value - oItem.start.value) * (1f - xRatio));
		}
	}

	public virtual bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		return false;
	}

	public virtual void History_Event_Lock()
	{
	}

	public virtual void History_Event_Unlock()
	{
	}

	public void History_Event_Resume(ref HistoryItem oItem)
	{
	}

	public virtual void History_Event_Replayed(ref HistoryItem oItem)
	{
	}

	public virtual void History_Event_Reverse_Start(ref HistoryItem oItem)
	{
	}

	public virtual void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Property)
		{
			index--;
			index = ((index < 0) ? (qbits.Length - 1) : index);
			Color_Set();
		}
	}

	public virtual void History_Event_ForceClose(ref HistoryItem oItem)
	{
	}

	static AtomFilter()
	{
		int[] pROPERTIES_DEFAULT = new int[4];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
	}
}
