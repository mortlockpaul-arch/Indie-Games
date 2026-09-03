using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BulletPlane : Enemy
{
	protected Vector3 oldvel;

	protected Vector3 vel;

	protected Vector3 up;

	protected Vector3 oldPos;

	protected float speed;

	protected float maxspeed;

	protected float accel;

	protected float rot;

	protected static float rotInc = (float)Math.PI * 2f;

	protected Enemy enem;

	protected int part;

	protected Vector3 actualPos;

	protected bool followPath;

	private Matrix _transformation;

	protected bool dirty;

	protected bool launched;

	public bool active;

	protected BulletPlaneCollection collection;

	protected int drawIndex;

	protected float partCooldown;

	protected float maxCooldown;

	protected Vector3 partVel;

	protected int numParticles;

	protected float genLength;

	protected string strName;

	protected Vector3 difColor;

	protected Vector3 emisColor;

	protected float size;

	public bool IsLaunched => launched;

	public BulletPlane()
	{
		size = 1f;
	}

	public BulletPlane(ref Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, BulletPlaneCollection _col, Vector3 _difColor, Vector3 _emisColor, PathList pList, int _part, float _accel, bool _followPath)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(ref _enem, ref _parent, _mesh, _planeIndex, boneIndex, _col, _difColor, _emisColor, pList, _part, _accel, _followPath, (FillMode)2);
	}

	public BulletPlane(ref Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, BulletPlaneCollection _col, Vector3 _difColor, Vector3 _emisColor, PathList pList, int _part, float _accel, bool _followPath, FillMode _fillMode)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		VertexNormalTex[] array = new VertexNormalTex[3];
		VertexPositionNormalTexture[] array2 = (VertexPositionNormalTexture[])(object)new VertexPositionNormalTexture[3];
		collection = _col;
		Vector3 val = Vector3.Zero;
		for (int i = 0; i < 3; i++)
		{
			ref VertexNormalTex reference = ref array[i];
			reference = new VertexNormalTex(_parent.vertices[_mesh][_parent.indices[_mesh][_planeIndex + i]]);
			array[i].position = BaseGame.GetVertexPos(ref _parent, _mesh, _planeIndex + i, ref _enem);
			array[i].normal = BaseGame.GetVertexNorm(ref _parent, _mesh, _planeIndex + i, ref _enem);
			val += array[i].position;
		}
		val /= 3f;
		for (int j = 0; j < 3; j++)
		{
			ref VertexNormalTex reference2 = ref array[j];
			reference2.position -= val;
		}
		for (int k = 0; k < 3; k++)
		{
			array2[k].Position = array[k].position;
			array2[k].Normal = array[k].normal;
			array2[k].TextureCoordinate = array[k].tex;
		}
		difColor = _difColor;
		emisColor = _emisColor;
		part = _part;
		followPath = _followPath;
		strName = "000" + (_planeIndex * 83 + 5).ToString("X");
		strName = strName.Substring(strName.Length - 4);
		strName = strName.Substring(2) + strName.Substring(0, 2);
		strName = "[self 0x" + strName + "]";
		pathList = pList.Clone();
		if (followPath)
		{
			((PLine)pathList.publicPaths[0]).Initialize(val, ((PLine)pathList.publicPaths[0]).end, ((PLine)pathList.publicPaths[0]).speed);
		}
		partCooldown = 0f;
		maxCooldown = 0.05f;
		numParticles = 2;
		genLength = maxCooldown / (float)numParticles;
		setPos(val);
		vel = Vector3.Normalize(BaseGame.Get().playerPos - val);
		up = Vector3.Normalize(Vector3.Cross(vel, Vector3.Right));
		drawIndex = collection.AllocateSpace();
		drawIndex = drawIndex * 3 * VertexPositionNormalTexture.SizeInBytes;
		collection.vBuffer.SetData<VertexPositionNormalTexture>(drawIndex, array2, 0, 3, VertexPositionNormalTexture.SizeInBytes);
		size = 1f;
		speed = 25f;
		accel = _accel;
		maxspeed = 5f;
		state = 0;
		hitPoints = 1;
		launched = false;
		active = true;
		dirty = true;
		enem = _enem;
		fillMode = _fillMode;
	}

	public void DrawPlane()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (active)
		{
			BaseGame.Get().fogEffect.Parameters["xEnableLighting"].SetValue(true);
			BaseGame.Get().fogEffect.Parameters["DirLight0Direction"].SetValue(Vector3.Normalize(new Vector3(-0.5f, -0.5f, 0.5f)));
			BaseGame.Get().fogEffect.Parameters["DiffuseColor"].SetValue(difColor);
			BaseGame.Get().fogEffect.Parameters["EmissiveColor"].SetValue(emisColor);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = fillMode;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(collection.vBuffer, drawIndex, VertexPositionNormalTexture.SizeInBytes);
			BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)4, 0, 1);
			BaseGame.Get().matStack.PopMatrix();
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		}
	}

	public override void draw(GameTime gametime)
	{
	}

	public override Matrix Transformation()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void act(GameTime gametime)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		dirty = true;
		oldPos = pos;
		base.act(gametime);
		p_act(gametime);
	}

	public virtual void p_act(GameTime gametime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Invalid comparison between Unknown and I4
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		if (!exists)
		{
			return;
		}
		oldvel = vel;
		if (!followPath)
		{
			vel += (pos - actualPos) * accel * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (((Vector3)(ref vel)).Length() > pathList.maxSpeed() && pathList.maxSpeed() > 0f)
			{
				vel = Vector3.Normalize(vel) * pathList.maxSpeed();
			}
			actualPos += vel * (float)gametime.ElapsedGameTime.TotalSeconds;
			partVel = -vel * 0.2f;
		}
		else
		{
			vel = pos - oldPos;
			vel = Vector3.Normalize(vel) * pathList.maxSpeed();
			partVel = -vel * 0.05f;
		}
		if (launched)
		{
			PlayerHit();
		}
		if (vel != oldvel)
		{
			up = BaseGame.GetUpVector(vel, up, oldvel);
		}
		rot += rotInc * (float)gametime.ElapsedGameTime.TotalSeconds;
		if ((int)fillMode == 2)
		{
			partCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (partCooldown <= 0f)
			{
				BaseGame.Get().ps.AddParticles(getPos(), partVel, 0.2f, 45f, Vector3.Zero, 0f, 1f, 0.2f, 0f, new Vector4(emisColor, 1f), numParticles, genLength);
				partCooldown += maxCooldown;
			}
		}
	}

	public override Vector3 getPos()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (followPath)
		{
			return pos;
		}
		return actualPos;
	}

	public void Launch()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		launched = true;
		pathList = new PathList();
		pathList.Add(new PLine(getPos(), BaseGame.Get().playerPos, 5f));
		pathList.Add(new PLine(BaseGame.Get().playerPos, BaseGame.Get().playerPos + new Vector3(1f, 1f, 1f), 0f));
		pathList.ResetCurrent();
		pathList.SetLoop(-1);
		setPos(pathList.curLocation());
		emisColor = new Vector3(0.5f, 0f, 0f);
		difColor = new Vector3(1f, 0.3f, 0.3f);
	}

	public void LaunchOlu()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		launched = true;
		pathList = new PathList();
		Vector3 end = getPos() + BaseGame.GetRandVect(new Vector3(0f, 0f, 1f), 90f) * 6f;
		pathList.Add(new PLine(getPos(), end, 10f));
		pathList.Add(new PLine(end, BaseGame.Get().playerPos, 5f));
		pathList.Add(new PLine(BaseGame.Get().playerPos, BaseGame.Get().playerPos + new Vector3(1f, 1f, 1f), 0f));
		pathList.ResetCurrent();
		pathList.SetLoop(-1);
		setPos(pathList.curLocation());
		emisColor = new Vector3(0f, 0.35f, 0f);
		difColor = new Vector3(0.2f, 0.9f, 0.2f);
		size = 0.7f;
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos;
		TargetStart();
		addCond(new NeverCondition());
		base.start();
		vel = BaseGame.FaceUpward(pathList);
		BaseGame.Get().actualEnem--;
		actualPos = val;
	}

	public virtual void TargetStart()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		addTarget(new Vector3(0f, 0f, 0f), 1, 10);
	}

	public override Enemy attack()
	{
		return new ECube();
	}

	public override string name()
	{
		return strName;
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue("muteCrash", volume);
		}
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		exists = false;
		BossPartDie();
		BaseGame.Get().actualEnem++;
		if (!launched)
		{
			collection.enemies.Remove(this);
		}
		else
		{
			collection.detached.Remove(this);
		}
		base.die();
	}

	public override void leave()
	{
		exists = false;
		BossPartLeave();
		if (!launched)
		{
			collection.enemies.Remove(this);
		}
		else
		{
			collection.detached.Remove(this);
		}
		BaseGame.Get().actualEnem++;
		base.leave();
	}

	public virtual void BossPartLeave()
	{
		if (enem is Hypatia)
		{
			((Hypatia)enem).bosshp[part]--;
		}
		if (enem is Pythagoras)
		{
			((Pythagoras)enem).bosshp[part]--;
		}
	}

	public virtual void BossPartDie()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (enem is Hypatia)
		{
			((Hypatia)enem).bosshp[part]--;
		}
		if (enem is Pythagoras)
		{
			((Pythagoras)enem).bosshp[part]--;
		}
		if (enem is Olu)
		{
			((Olu)enem).NoteHit(part, (BaseGame.Get().fillMode == fillMode) ? 1 : 2);
		}
	}

	public override TargetEffectCol lockOn(int targetsLeft)
	{
		if (active)
		{
			return base.lockOn(targetsLeft);
		}
		return new TargetEffectCol();
	}
}
