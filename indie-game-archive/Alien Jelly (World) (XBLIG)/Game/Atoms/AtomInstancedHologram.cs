using GKEngine.Animation;
using GKEngine.Entities;
using Game.Grids;
using Game.History;
using Game.Physics;
using Game.Scenes;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomInstancedHologram : AtomInstanced, IGridable, IReversible
{
	private const float ANIM_TIME = 400f;

	public static string PROPERTIES_DESCRIPTION = "This hologram block has the following properties that allow control over whether is visible when the level starts or not.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[1]
	{
		new AtomProperty("Visibility", "This option allows you to set the start up visibility of the block.", new string[2] { "Not Visible", "Visible" })
	};

	public static int[] PROPERTIES_DEFAULT = new int[1] { 1 };

	public static Range ANIM_LERP = new Range(0.0001f, 1f);

	public bool historyLocked;

	private bool animating;

	private float animTime;

	private int animMode;

	private int animState;

	public AtomInstancedHologram(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void InitPlay()
	{
		SetVisibility(properties[0] == 1, pHard: true);
		base.InitPlay();
	}

	public void Toggle()
	{
		SetVisibility(!visible, pHard: false);
	}

	public void SetVisibility(bool pValue, bool pHard)
	{
		PlayUniverse universe = (instancer.manager.scene as PlayScene).universe;
		if (!universe.history.reversing && play)
		{
			universe.history.Open(this, HistoryItem.Action.Property);
		}
		visible = pValue;
		if (pHard)
		{
			animating = false;
			scaleAll = (pValue ? 1f : 1E-05f);
			PopulateInstancer();
		}
		else
		{
			Anim_Start();
		}
		if (pValue)
		{
			if (!universe.history.reversing)
			{
				PhysicsItem physicsItem = universe.physics.At(X, Y, Z);
				if (physicsItem != null && !physicsItem.dead && !physicsItem.dying)
				{
					physicsItem.Death();
				}
			}
			universe.grid.Add(this);
		}
		else
		{
			universe.grid.Remove(this);
		}
		if (!universe.history.reversing && play)
		{
			universe.history.Close(this, HistoryItem.Action.Property);
		}
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		if (!animating)
		{
			return;
		}
		animTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
		if (animTime > 400f)
		{
			Anim_Lerp(1f);
			if (animState >= 1)
			{
				animating = false;
				return;
			}
			animState++;
			animTime = 0f;
		}
		else
		{
			Anim_Lerp(animTime / 400f);
		}
	}

	public override void AreaRefresh()
	{
		AreaRotate();
		if (visible)
		{
			manager.grid.Refresh(this);
		}
	}

	public override void AreaRefreshSoft()
	{
		AreaRotate();
		if (visible)
		{
			manager.grid.Add(this);
		}
	}

	public override void Event_Flip_Start()
	{
		_base.matrix = Matrix.Multiply(matrix, manager.inverse);
		base.Event_Flip_Start();
	}

	private void Anim_Start()
	{
		(manager.scene as PlayScene).audio.EventCues_Trigger("Special Event");
		animMode = ((!visible) ? 1 : 0);
		animState = 0;
		animTime = 0f;
		Anim_Lerp(0f);
		animating = true;
	}

	private void Anim_Lerp(float pRatio)
	{
		Tween.EaseInOut(pRatio);
		if (animMode == 0)
		{
			if (animState == 0)
			{
				float num = (scaleZ = ANIM_LERP.Lerp(pRatio));
				scaleX = num;
			}
			else
			{
				scaleY = ANIM_LERP.Lerp(pRatio);
			}
		}
		else if (animState == 0)
		{
			scaleY = ANIM_LERP.Lerp(1f - pRatio);
		}
		else
		{
			float num3 = (scaleZ = ANIM_LERP.Lerp(1f - pRatio));
			scaleX = num3;
		}
		PopulateInstancer();
	}

	public void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction)
	{
		if (oAction == HistoryItem.Action.Property)
		{
			oItem.value = (visible ? 1 : 0);
		}
	}

	public void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		if (oItem.action == HistoryItem.Action.Property)
		{
			SetVisibility(oItem.start.value == 1f, pHard: true);
		}
	}

	public bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		return false;
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
	}

	public void History_Event_Reverse_Start(ref HistoryItem oItem)
	{
	}

	public void History_Event_Reverse_End(ref HistoryItem oItem)
	{
	}

	public void History_Event_Resume(ref HistoryItem oItem)
	{
	}

	public void History_Event_ForceClose(ref HistoryItem oItem)
	{
	}
}
