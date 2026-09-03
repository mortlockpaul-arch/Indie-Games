using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Sled01 : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	private static ModelWrapper _sledModel;

	public static float size = 10f;

	public ConditionSet launchCond;

	public int launchNum;

	public ModelWrapper sledModel;

	public Vector3 actualPos;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public bool launched;

	public bool launchDone;

	private int launchChan;

	private float launchDelay;

	public Digit[] shots;

	private Matrix _transformation;

	private bool dirty;

	public Sled01()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 2;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		launched = (launchDone = false);
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Fish01", Beats.Eighth));
			wCond.Add(1, new WaitCond("Fish01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Fish02", Beats.Eighth));
			wCond.Add(3, new WaitCond("Fish03", Beats.Eighth));
			wCond.Add(4, new WaitCond("Fish04", Beats.Eighth));
			wCond.Add(5, new WaitCond("Fish05", Beats.Eighth));
			wCond.Add(6, new WaitCond("Fish06", Beats.Eighth));
			wCond.Add(7, new WaitCond("Fish07", Beats.Eighth));
			wCond.Add(8, new WaitCond("Fish08", Beats.Eighth));
		}
		_eCond = wCond;
		launchCond = new ConditionSet();
		launchNum = 0;
		dirty = true;
	}

	public static void LoadModel()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		_sledModel = BaseGame.Get().models.GetModel("Content\\Sled\\Sled");
		BaseGame.SetAllEPCs(_sledModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(_sledModel.epc, "DiffuseColor", (object)new Vector3(1f, 1f, 0f));
		BaseGame.Get().LinkEffect(_sledModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
	}

	public Sled01(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		launchChan = LevelLoader.GetIntFromAtt(attributes, "channel", 0);
		launchDelay = LevelLoader.GetFloatFromAtt(attributes, "launchdelay", 1.5f);
		shots = new Digit[2];
		for (int i = 0; i < shots.Length; i++)
		{
			shots[i] = new Digit(LevelLoader.GetAttributeDictionary(node.SelectSingleNode("digitpath")), node.SelectSingleNode("digitpath"));
			shots[i].enem = this;
			shots[i].launched = false;
		}
	}

	public override void draw(GameTime gametime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.SetAllEPCs(sledModel.epc, "DirLight0Direction", pos - BaseGame.Get().playerPos);
		BaseGame.Get().DrawModel(ref sledModel);
		BaseGame.Get().matStack.PopMatrix();
	}

	public override Matrix Transformation()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = BaseGame.MapObjectToSystem(Vector3.Zero, (vel == Vector3.Zero) ? Vector3.Forward : vel, up) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(toHit.eTarget.absolutePos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 0f, 0f, 1f), 320, 0.0005f);
		}
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		launchCond.Update();
		if (state == 0 && launchCond.ConditionsMet())
		{
			state++;
		}
		if (state == 1 && launchCond.ConditionsMet())
		{
			shots[launchNum].Launch();
			launchNum++;
			launchCond.set.Clear();
			if (launchNum >= shots.Length)
			{
				launchCond.set.Add(new NeverCondition());
				launchCond.Start();
			}
			else
			{
				launchCond.set.Add(new ChanCondition(launchChan, 0.95f));
				launchCond.Start();
			}
		}
		dirty = true;
	}

	public override void start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		base.start();
		vel = Vector3.Backward;
		addTarget(new Vector3(0f, 0f, 0f), 2, 20);
		actualPos = pos;
		sledModel = new ModelWrapper(_sledModel);
		addCond(new NeverCondition());
		launchCond.set.Add(new TimeCondition(launchDelay));
		launchCond.set.Add(new ChanCondition(launchChan, 0.95f));
		launchCond.Start();
		for (int i = 0; i < shots.Length; i++)
		{
			shots[i].start();
			shots[0].pos = new Vector3(0f, 0.2f, 0.4f);
			shots[1].pos = new Vector3(0f, 0.2f, -0.2f);
			shots[0].actualPos = new Vector3(0f, 0.2f, 0.4f);
			shots[1].actualPos = new Vector3(0f, 0.2f, -0.2f);
			BaseGame.Get().enems.Add(shots[i]);
		}
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pos;
	}

	public override string name()
	{
		return "[clef_carryover_v.01]";
	}

	public override bool Check(int numEnem)
	{
		return wCond[numEnem].Check(BaseGame.Get().curBeat);
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue(wCond[lockNum].cueName, volume);
		}
	}
}
