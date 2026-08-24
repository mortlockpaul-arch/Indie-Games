using System;
using GKEngine.Entities;
using Game.Grids;
using Game.History;
using Game.Particles;
using Game.Scenes;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomDissapearing : AtomSingle, IGridable, IRenderable, IReversible
{
	private const int SCALE_TIME_TOTAL = 500;

	public static int[] PROPERTY_TIME_VISIBLE = new int[8] { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000 };

	public static int[] PROPERTY_TIME_HIDDEN = new int[8] { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000 };

	public static string TITLE = "Dissapearing Block";

	public static string DESCRIPTION = "This is a block that dissapears and reappears. This item has properties";

	public static string PROPERTIES_DESCRIPTION = "This dissapearing block has the following properties that allow you to control its timing.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[2]
	{
		new AtomProperty("Visible Time", "This option allows you to set the time the block is visible", new string[8] { "1 Second", "2 Seconds", "3 Seconds", "4 Seconds", "5 Seconds", "6 Seconds", "7 Seconds", "8 Seconds" }),
		new AtomProperty("Invisible Time", "This option allows you to set the time the block is hidden for.", new string[8] { "1 Second", "2 Seconds", "3 Seconds", "4 Seconds", "5 Seconds", "6 Seconds", "7 Seconds", "8 Seconds" })
	};

	public static int[] PROPERTIES_DEFAULT = new int[2] { 3, 1 };

	private static Range SCALE_RANGE = new Range(0.0001f, 1f);

	private float time;

	private int index;

	private float timeTotal;

	private int visibleTime;

	private int hiddenTime;

	public ParticleEmitter emitter;

	public ParticleEmitterSchema emitterSchema;

	public override int[] properties
	{
		get
		{
			return base.properties;
		}
		set
		{
			base.properties = value;
			if (value.Length > 0)
			{
				visibleTime = PROPERTY_TIME_VISIBLE[value[0]];
				hiddenTime = PROPERTY_TIME_HIDDEN[value[1]];
			}
		}
	}

	public AtomDissapearing(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void InitPlay()
	{
		base.InitPlay();
		Particles_Set();
		time = 0f;
		index = 0;
		timeTotal = visibleTime;
		(manager.scene as PlayScene).universe.history.Open(this, HistoryItem.Action.Property);
	}

	public override void Dispose()
	{
		if (emitter != null)
		{
			emitter.Dispose();
		}
		base.Dispose();
	}

	public override void Update(GameTime oGameTime)
	{
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= timeTotal)
		{
			(manager.scene as PlayScene).universe.history.Close(this, HistoryItem.Action.Property);
			index++;
			index %= 4;
			Phase_Set();
			time = 0f;
			(manager.scene as PlayScene).universe.history.Open(this, HistoryItem.Action.Property);
		}
		Phase_Lerp(time / timeTotal);
		base.Update(oGameTime);
	}

	private void Phase_Set()
	{
		switch (index)
		{
		case 0:
			emitter.position = position;
			emitter.Start(500f, emitterSchema);
			timeTotal = visibleTime;
			manager.grid.Add(this);
			break;
		case 1:
			emitter.position = position;
			emitter.Start(500f, emitterSchema);
			timeTotal = 500f;
			break;
		case 2:
			visible = false;
			timeTotal = hiddenTime;
			manager.grid.Remove(this);
			break;
		case 3:
			visible = true;
			timeTotal = 500f;
			break;
		}
	}

	private void Phase_Lerp(float xRatio)
	{
		if (index == 1)
		{
			scaleAll = SCALE_RANGE.Lerp(1f - xRatio);
		}
		else if (index == 3)
		{
			scaleAll = SCALE_RANGE.Lerp(xRatio);
		}
	}

	private void Phase_Resume()
	{
		switch (index)
		{
		case 0:
			visible = true;
			timeTotal = visibleTime;
			manager.grid.Add(this);
			break;
		case 1:
			visible = true;
			timeTotal = 500f;
			break;
		case 2:
			visible = false;
			timeTotal = hiddenTime;
			manager.grid.Remove(this);
			break;
		case 3:
			visible = true;
			timeTotal = 500f;
			break;
		}
	}

	public void Particles_Set()
	{
		emitter = new ParticleEmitter(manager.scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ALPHA_UNSORTED);
		emitterSchema = new ParticleEmitterSchema(20);
		emitterSchema.mode = ParticleEmitter.Mode.OneShot;
		emitterSchema.rotationStart = 0f;
		emitterSchema.rotationEnd = (float)Math.PI * 2f;
		emitterSchema.rotationTween = 1;
		emitterSchema.scaleStart = 5f;
		emitterSchema.scaleEnd = 20f;
		emitterSchema.scaleTween = 1;
		emitterSchema.textureStart = manager.scene.library.texture2Ds["TextureDiffuse_Particles_Clouds_0"];
		emitterSchema.textureEnd = manager.scene.library.texture2Ds["TextureDiffuse_Particles_Clouds_1"];
		emitterSchema.textureTween = 1;
		emitterSchema.tintStart = new Color(121, 82, 13, 200);
		emitterSchema.tintEnd = new Color(50, 41, 26, 0);
		emitterSchema.tintTween = 1;
		emitterSchema.tween = 1;
		emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 0.3f);
		emitterSchema.Vector_Constant(ref emitterSchema.positions, Vector3.Zero);
		emitterSchema.Vector_Random(ref emitterSchema.deltas, Vector3.Zero, 15f, 30f);
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
			time = oItem.start.value + (oItem.end.value - oItem.start.value) * (1f - xRatio);
			float num = time / 500f;
			if (oItem.start.index == 1)
			{
				scaleAll = SCALE_RANGE.Lerp(1f - num);
			}
			else if (oItem.start.index == 3)
			{
				scaleAll = SCALE_RANGE.Lerp(num);
			}
		}
	}

	public virtual bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		return false;
	}

	public override void Event_Flip_Start()
	{
		base.Event_Flip_Start();
	}

	public virtual void History_Event_Lock()
	{
	}

	public virtual void History_Event_Unlock()
	{
	}

	public void History_Event_Resume(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Property)
		{
			index = oItem.end.index;
			Phase_Resume();
		}
	}

	public virtual void History_Event_Replayed(ref HistoryItem oItem)
	{
	}

	public virtual void History_Event_Reverse_Start(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Property)
		{
			visible = true;
		}
	}

	public virtual void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Property)
		{
			index--;
			index = ((index < 0) ? 3 : index);
		}
	}

	public virtual void History_Event_ForceClose(ref HistoryItem oItem)
	{
	}
}
