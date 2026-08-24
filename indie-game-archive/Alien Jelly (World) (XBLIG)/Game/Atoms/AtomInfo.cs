using GKEngine.Entities;
using Game.Grids;
using Game.QBits;
using Game.Scenes;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomInfo : AtomSingle, IGridable, IRenderable
{
	private const float TIME = 2000f;

	private const float WAIT = 100f;

	public static string TITLE = "Conversation Trigger";

	public static string DESCRIPTION = "This object allows you to set the conversation the jelly has whne it steps on this. Dev use Only!";

	public static string PROPERTIES_DESCRIPTION = "Dev use only!";

	public static AtomProperty[] PROPERTIES = new AtomProperty[1]
	{
		new AtomProperty("Conversation Index", "Set teh conversation Index.", new string[11]
		{
			"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
			"10"
		})
	};

	public static int[] PROPERTIES_DEFAULT;

	private bool waiting;

	private float waitTime;

	private QBit waitQBit;

	public bool collected;

	public AtomInfo(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void Load()
	{
		base.Load();
		trigger = new AtomTrigger(this, Triggered);
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		float num = (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
		rotation *= Quaternion.CreateFromYawPitchRoll(0.001f * num, 0f, 0f);
		if (waiting)
		{
			waitTime += num;
			if (waitTime >= 100f)
			{
				waiting = false;
				Show();
			}
		}
	}

	public override void InitPlay()
	{
		base.InitPlay();
	}

	public override void InitBuild()
	{
		base.InitBuild();
	}

	public bool Triggered(object oTriggerer)
	{
		bool flag = false;
		QBit qBit = oTriggerer as QBit;
		return oTriggerer != null && qBit.player != null && !collected;
	}

	private void Show()
	{
		if (waitQBit != null && (waitQBit.position - position).Length() < 1f)
		{
			(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Button");
			(manager.scene as PlayScene).universe.players.Input_Deactivate();
			waitQBit.Particles_Switch_Start();
			waitQBit.manager.conversation.Show(properties[0], delegate
			{
				(manager.scene as PlayScene).universe.players.Input_Activate();
			}, waitQBit.manager.qbits.IndexOf(waitQBit));
			collected = true;
			scaleAll = 0.0001f;
			visible = false;
		}
	}

	public override void StateSet(int xState)
	{
		base.StateSet(xState);
	}

	public override void Event_Triggered_Start(object oTriggerer)
	{
		waitQBit = oTriggerer as QBit;
		Show();
		visible = false;
	}

	public override void Event_Flip_End()
	{
		base.Event_Flip_End();
	}

	public override void Event_Flip_Start()
	{
		base.Event_Flip_Start();
	}

	static AtomInfo()
	{
		int[] pROPERTIES_DEFAULT = new int[1];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
	}
}
