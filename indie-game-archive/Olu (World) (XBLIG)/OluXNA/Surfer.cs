using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Surfer : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static Dictionary<ModelBone, int> sBones;

	public static ModelWrapper model;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public ModelOluAnimator anim;

	public AnimationController leftAnim;

	public AnimationController rightAnim;

	public float pCooldown;

	public float pMax;

	public Vector3 revVel;

	public float size;

	public float maxRot;

	public float blendAmount
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			return 0.5f + ((Vector3.Transform(getDir(), Matrix.Invert(BaseGame.MapObjectToSystem(Vector3.Zero, new Vector3(0f, -1f, 1f), new Vector3(0f, 1f, 0f)))).X >= 0f) ? (-0.5f) : 0.5f) * (1f - Vector3.Dot(Vector3.Normalize(new Vector3(0f, -1f, 1f)), Vector3.Normalize(getDir())));
		}
	}

	public Surfer()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.02f;
		maxRot = 720f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 2;
		vel = Vector3.Forward;
		revVel = -vel;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Bird01", Beats.Quarter));
			wCond.Add(1, new WaitCond("Bird01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Bird02", Beats.Eighth, Beats.Quarter));
			wCond.Add(3, new WaitCond("Bird03", Beats.Eighth, Beats.Quarter));
			wCond.Add(4, new WaitCond("Bird04", Beats.Eighth, Beats.Quarter));
			wCond.Add(5, new WaitCond("Bird05", Beats.Eighth));
			wCond.Add(6, new WaitCond("Bird06", Beats.Eighth));
			wCond.Add(7, new WaitCond("Bird07", Beats.Eighth));
			wCond.Add(8, new WaitCond("Bird08", Beats.Eighth));
		}
		_eCond = wCond;
	}

	public static void LoadModel()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Surfer\\Surfer");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DiffuseColor", Vector3.Zero);
		BaseGame.SetAllEPCs(model.epc, "TextureMix", BaseGame.T_ADD);
		sBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!sBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				sBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
	}

	public Surfer(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 2f);
	}

	public override Matrix Transformation()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			return BaseGame.MapObjectToSystem(Vector3.Zero, getDir(), new Vector3(0f, 1f, 0f)) * Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90f)) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
		}
		return Matrix.CreateScale(0f);
	}

	public override void draw(GameTime gametime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().DrawModel(ref model);
		BaseGame.Get().matStack.PopMatrix();
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 80, 0.0005f);
		base.die();
	}

	public override void act(GameTime gametime)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		float num = blendAmount;
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		BaseGame.RunController(anim, leftAnim, rightAnim, num);
		((GameComponent)anim).Update(gametime);
		pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (pCooldown <= 0f)
		{
			if (5f * (1f - num) >= 1f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, model.transforms[sBones[model.model.Bones["Armature_LeftFin"]]] * Transformation()), BaseGame.Get().skyFlow, 0.2f, 10f, Vector3.Zero, 0f, 0.1f, 0.1f, 0f, new Vector4(1f, 1f, 1f, 1f), (int)(5f * (1f - num)), pMax / (1f - num));
			}
			if (5f * num >= 1f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, model.transforms[sBones[model.model.Bones["Armature_RightFin"]]] * Transformation()), BaseGame.Get().skyFlow, 0.2f, 10f, Vector3.Zero, 0f, 0.1f, 0.1f, 0f, new Vector4(1f, 1f, 1f, 1f), (int)(5f * num), pMax / num);
			}
			pCooldown += pMax;
		}
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 2, 10);
		addCond(new NeverCondition());
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		leftAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["left"], component: false);
		rightAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["right"], component: false);
		BaseGame.RunController(anim, leftAnim, rightAnim, blendAmount);
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		((GameComponent)leftAnim).Update(BaseGame.Get().emptytime);
		((GameComponent)rightAnim).Update(BaseGame.Get().emptytime);
		base.start();
	}

	public Vector3 getDir()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return pathList.curDir();
	}

	public override string name()
	{
		return "[l_edge^~man]";
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
