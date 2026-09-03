using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class PythDigit : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static Dictionary<ModelBone, int> dBones;

	public static Dictionary<Vector3, int> sides;

	public static ModelWrapper model;

	public static ModelWrapper wire;

	public ModelOluAnimator anim;

	public AnimationController still;

	public float pCooldown;

	public float pMax;

	public string curSide;

	public Enemy enem;

	public float size;

	public Vector3 posVert;

	public Vector3 posHoriz;

	public Vector3 faceDir;

	public float rotAmount;

	public float rotRate;

	public float rotEnd;

	public float horizScale;

	public int part;

	protected string strName;

	public Matrix posTransform;

	public PythDigit()
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.05f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 2;
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
		fillMode = (FillMode)2;
	}

	public static void LoadModel()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Digit\\Digit");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		wire = BaseGame.Get().models.GetModel("Content\\Digit\\DigitWire");
		BaseGame.SetAllEPCs(wire.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(wire.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		dBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!dBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				dBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
		sides = new Dictionary<Vector3, int>();
		sides.Add(Vector3.Down, dBones[model.model.Bones["Armature_Top"]]);
		sides.Add(Vector3.Up, dBones[model.model.Bones["Armature_Bottom"]]);
		sides.Add(Vector3.Forward, dBones[model.model.Bones["Armature_Front"]]);
		sides.Add(Vector3.Backward, dBones[model.model.Bones["Armature_Back"]]);
		sides.Add(Vector3.Left, dBones[model.model.Bones["Armature_Right"]]);
		sides.Add(Vector3.Right, dBones[model.model.Bones["Armature_Left"]]);
	}

	public void Initialize(float _size)
	{
		size = _size;
	}

	public PythDigit(float _size)
		: this()
	{
		Initialize(_size);
	}

	public PythDigit(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		Initialize(LevelLoader.GetFloatFromAtt(attributes, "size", 5f));
	}

	public void AttachToPyth(Enemy _enem, ref ModelWrapper _parent, int _mesh, int _vIndex, int boneIndex, int _part, float _rotRate)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		VertexNormalTex vertexNormalTex = new VertexNormalTex(_parent.vertices[_mesh][_vIndex]);
		vertexNormalTex.position = Vector3.Transform(vertexNormalTex.position, _parent.palette[0][boneIndex]);
		vertexNormalTex.normal = Vector3.TransformNormal(vertexNormalTex.normal, _parent.palette[0][boneIndex]);
		horizScale = 1f;
		posVert = new Vector3(0f, vertexNormalTex.position.Y, 0f);
		posHoriz = new Vector3(vertexNormalTex.position.X, 0f, vertexNormalTex.position.Z);
		faceDir = Vector3.Normalize(vertexNormalTex.normal);
		part = _part;
		rotRate = _rotRate;
		strName = "000" + (_vIndex * 83 + 5).ToString("X");
		strName = strName.Substring(strName.Length - 4);
		strName = strName.Substring(2) + strName.Substring(0, 2);
		strName = "[d1g1t 0x" + strName + "]";
		pCooldown = 0f;
		pMax = 0.05f;
		state = 0;
		hitPoints = 1;
		enem = _enem;
	}

	public override Matrix Transformation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		CheckLaunch();
		return BaseGame.MapObjectToSystem(Vector3.Zero, getDir(), Vector3.Up) * Matrix.CreateScale(size / Pythagoras.size) * Matrix.CreateTranslation(getPos()) * Matrix.CreateRotationY(MathHelper.ToRadians(rotAmount)) * enem.Transformation();
	}

	public override void draw(GameTime gametime)
	{
	}

	public void DrawModel(GameTime gametime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			BaseGame.Get().matStack.ApplyRawMatrix(posTransform);
			BaseGame.Get().fogEffect.Parameters["MatrixPalette"].SetValue(model.palette[0]);
			BaseGame.Get().DrawModelEffectStarted(ref model);
		}
	}

	public void DrawWire(GameTime gametime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			posTransform = Transformation();
			BaseGame.Get().matStack.ApplyRawMatrix(posTransform);
			BaseGame.Get().DrawModelEffectStarted(ref wire);
		}
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, Transformation()), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 10, 0.0005f);
			((Pythagoras)enem).bosshp[part]--;
			((Pythagoras)enem).dColl.Remove(this);
			BaseGame.Get().pythdigit01.enemies.Remove(this);
			base.die();
		}
	}

	public override void leave()
	{
		if (exists)
		{
			((Pythagoras)enem).bosshp[part]--;
			((Pythagoras)enem).dColl.Remove(this);
			BaseGame.Get().pythdigit01.enemies.Remove(this);
			base.leave();
		}
	}

	public void CheckLaunch()
	{
		if (state == 0 && !enem.exists)
		{
			Launch();
		}
	}

	public void Launch()
	{
		state = 1;
		attackCond.set.Clear();
		addCond(new TimeCondition(0.6000000238418579));
		attackCond.Start();
	}

	public override void act(GameTime gametime)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		CheckLaunch();
		if (!exists)
		{
			return;
		}
		if (state == 1)
		{
			horizScale += 0.5f * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (horizScale >= 1.7f)
			{
				state = 2;
				rotAmount = 360f;
				rotEnd = 0f;
			}
		}
		else if (state == 2)
		{
			rotAmount -= 2f * rotRate * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (rotAmount <= rotEnd)
			{
				rotAmount = 0f;
				state = 3;
			}
		}
		else if (state == 3)
		{
			horizScale -= 0.5f * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (horizScale <= 1f)
			{
				state = 0;
				attackCond.set.Clear();
				addCond(new NeverCondition());
				horizScale = 1f;
			}
		}
		if ((float)state > 0f)
		{
			pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (pCooldown <= 0f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, Transformation()), Vector3.TransformNormal(getDir(), Transformation()), 0.5f, 20f, Vector3.Zero, 0f, 0.2f, 0.1f, 0f, new Vector4(1f, 0.3f, 0f, 1f), 5, 0.0025f);
				pCooldown += pMax;
			}
		}
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 2, 10);
		addCond(new NeverCondition());
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		still = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["still"]);
		anim.bonePoses[dBones[model.model.Bones["Armature_Top"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Bottom"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Left"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Right"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Front"]]].enabled = false;
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		base.start();
		curSide = "Armature_Back";
		BaseGame.Get().pythdigit01.enemies.Add(this);
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return posVert + Vector3.Transform(horizScale * posHoriz, Matrix.CreateRotationY(MathHelper.ToRadians(rotAmount)));
	}

	public Vector3 getDir()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Transform(faceDir, Matrix.CreateRotationY(MathHelper.ToRadians(rotAmount)));
	}

	public Vector3 getParticleDir()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Zero;
		if (state == 1)
		{
			val = Vector3.Forward;
		}
		else if (state == 2)
		{
			val = Vector3.Right;
		}
		else if (state == 3)
		{
			val = Vector3.Backward;
		}
		return Vector3.Transform(val, Transformation());
	}

	public override string name()
	{
		return strName;
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
