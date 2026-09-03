using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Fish02 : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	private static ModelWrapper[] _fishModel;

	private static ModelWrapper[] _fishWireModel;

	public ModelWrapper[] fishModel;

	public ModelWrapper[] fishWireModel;

	public List<ModelOluAnimator> fishAnim;

	public List<ModelOluAnimator> fishWireAnim;

	public List<AnimationController> swimming;

	public List<AnimationController> swimmingWire;

	public bool[] active;

	public Vector3 actualPos;

	public Vector3 vel;

	public Vector3 oldvel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public float waterHeight;

	public bool launched;

	public bool launchDone;

	public float jetScale;

	public Fish02()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 4;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		launched = (launchDone = false);
		jetScale = 1f;
		active = new bool[5] { true, true, true, true, true };
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
	}

	public static void LoadModel()
	{
		_fishModel = new ModelWrapper[5];
		_fishWireModel = new ModelWrapper[4];
		_fishModel[0] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Head");
		_fishModel[1] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Body01");
		_fishModel[2] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Body02");
		_fishModel[3] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Tail");
		_fishModel[4] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Jet");
		_fishWireModel[0] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02HeadWire");
		_fishWireModel[1] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Body01Wire");
		_fishWireModel[2] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02Body02Wire");
		_fishWireModel[3] = BaseGame.Get().models.GetModel("Content\\Fish02\\Fish02TailWire");
		for (int i = 0; i < _fishModel.Length; i++)
		{
			BaseGame.SetAllEPCs(_fishModel[i].epc, "xEnableLighting", false);
			BaseGame.Get().LinkEffect(_fishModel[i].model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		}
		for (int j = 0; j < _fishWireModel.Length; j++)
		{
			BaseGame.SetAllEPCs(_fishWireModel[j].epc, "xEnableLighting", false);
			BaseGame.Get().LinkEffect(_fishWireModel[j].model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		}
	}

	public Fish02(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		waterHeight = LevelLoader.GetFloatFromAtt(attributes, "waterheight", 0f);
	}

	public override void draw(GameTime gametime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Invalid comparison between Unknown and I4
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		if ((int)fillMode == 3)
		{
			for (int i = 0; i < 5; i++)
			{
				if (active[i])
				{
					BaseGame.Get().DrawModel(ref fishModel[i]);
				}
			}
		}
		if ((int)fillMode == 2)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
			for (int j = 0; j < 4; j++)
			{
				if (active[j])
				{
					BaseGame.Get().DrawModel(ref fishWireModel[j]);
				}
			}
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
		BaseGame.Get().matStack.PopMatrix();
	}

	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		return BaseGame.MapObjectToSystem(Vector3.Zero, (vel == Vector3.Zero) ? Vector3.Forward : vel, up) * Matrix.CreateScale(new Vector3(15f, 15f, 15f)) * Matrix.CreateTranslation(getPos());
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
			BaseGame.Get().ps.AddParticles(toHit.eTarget.absolutePos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 1f, 0f, 1f), 80, 0.0005f);
		}
		active[((BoneTarget)toHit.eTarget).id] = false;
		if (((BoneTarget)toHit.eTarget).id == 3)
		{
			active[4] = false;
		}
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		oldvel = vel;
		vel += (pos - actualPos) * accel * (float)gametime.ElapsedGameTime.TotalSeconds;
		if (((Vector3)(ref vel)).Length() > 2f * pathList.maxSpeed())
		{
			vel = Vector3.Normalize(vel) * 2f * pathList.maxSpeed();
		}
		actualPos += vel * (float)gametime.ElapsedGameTime.TotalSeconds;
		if (vel != oldvel)
		{
			up = BaseGame.GetUpVector(vel, up, oldvel);
		}
		for (int i = 0; i < 5; i++)
		{
			((GameComponent)swimming[i]).Update(gametime);
		}
		for (int j = 0; j < 4; j++)
		{
			((GameComponent)swimmingWire[j]).Update(gametime);
		}
		for (int k = 0; k < 5; k++)
		{
			if (active[k])
			{
				((GameComponent)fishAnim[k]).Update(gametime);
			}
		}
		for (int l = 0; l < 4; l++)
		{
			if (active[l])
			{
				((GameComponent)fishWireAnim[l]).Update(gametime);
			}
		}
	}

	public override void start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		base.start();
		vel = BaseGame.FaceUpward(pathList);
		actualPos = pos;
		fishModel = new ModelWrapper[5];
		fishWireModel = new ModelWrapper[4];
		fishAnim = new List<ModelOluAnimator>();
		fishWireAnim = new List<ModelOluAnimator>();
		swimming = new List<AnimationController>();
		swimmingWire = new List<AnimationController>();
		for (int i = 0; i < fishModel.Length; i++)
		{
			fishModel[i] = new ModelWrapper(_fishModel[i]);
			fishAnim.Add(new ModelOluAnimator(BaseGame.Get().CoreGame, fishModel[i], BaseGame.GetFogEffect()));
			swimming.Add(new AnimationController(BaseGame.Get().CoreGame, fishAnim[i].Animations["swim"]));
			BaseGame.RunController(fishAnim[i], swimming[i]);
		}
		for (int j = 0; j < fishWireModel.Length; j++)
		{
			fishWireModel[j] = new ModelWrapper(_fishWireModel[j]);
			fishWireAnim.Add(new ModelOluAnimator(BaseGame.Get().CoreGame, fishWireModel[j], BaseGame.GetFogEffect()));
			swimmingWire.Add(new AnimationController(BaseGame.Get().CoreGame, fishWireAnim[j].Animations[0]));
			BaseGame.RunController(fishWireAnim[j], swimmingWire[j]);
		}
		for (int k = 0; k < 4; k++)
		{
			if (k == 0)
			{
				addTarget(new Vector3(0f, -0.3f, 0.3f), 1, 10, ref fishModel[k], k);
			}
			else
			{
				addTarget(new Vector3(0f, -0.1f, 0.5f), 1, 10, ref fishModel[k], k);
			}
		}
		addCond(new NeverCondition());
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return actualPos;
	}

	public override string name()
	{
		return "[qualifier_0xF053]";
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
