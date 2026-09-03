using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class AttachedPlane : BulletPlane
{
	protected Vector3 targetPos;

	public AttachedPlane(ref Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, BulletPlaneCollection _col, Vector3 _difColor, Vector3 _emisColor, PathList pList, int _part, float _accel, bool _followPath)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(ref _enem, ref _parent, _mesh, _planeIndex, boneIndex, _col, _difColor, _emisColor, pList, _part, _accel, _followPath, (FillMode)2);
	}

	public AttachedPlane(ref Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, BulletPlaneCollection _col, Vector3 _difColor, Vector3 _emisColor, PathList pList, int _part, float _accel, bool _followPath, FillMode _fillMode)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		VertexNormalTex[] array = new VertexNormalTex[3];
		VertexPositionNormalTexture[] array2 = (VertexPositionNormalTexture[])(object)new VertexPositionNormalTexture[3];
		collection = _col;
		targetPos = Vector3.Zero;
		Vector3 val = Vector3.Zero;
		for (int i = 0; i < 3; i++)
		{
			ref VertexNormalTex reference = ref array[i];
			reference = new VertexNormalTex(_parent.vertices[_mesh][_parent.indices[_mesh][_planeIndex + i]]);
			array[i].position = Vector3.Transform(array[i].position, _parent.palette[0][boneIndex]);
			array[i].normal = Vector3.TransformNormal(array[i].normal, _parent.palette[0][boneIndex]);
			targetPos += array[i].position;
			val += array[i].normal;
		}
		targetPos /= 3f;
		val /= 3f;
		for (int j = 0; j < 3; j++)
		{
			array2[j].Position = array[j].position;
			array2[j].Normal = array[j].normal;
			array2[j].TextureCoordinate = array[j].tex;
		}
		difColor = _difColor;
		emisColor = _emisColor;
		part = _part;
		followPath = _followPath;
		strName = "000" + (_planeIndex * 83 + 5).ToString("X");
		strName = strName.Substring(strName.Length - 4);
		strName = strName.Substring(2) + strName.Substring(0, 2);
		strName = "[self 0x" + strName + "]";
		if (pList != null)
		{
			pathList = pList.Clone();
			if (followPath)
			{
				((PLine)pathList.publicPaths[0]).Initialize(Vector3.Zero, ((PLine)pathList.publicPaths[0]).end, ((PLine)pathList.publicPaths[0]).speed);
			}
		}
		partCooldown = 0f;
		maxCooldown = 0.05f;
		numParticles = 2;
		genLength = maxCooldown / (float)numParticles;
		setPos(Vector3.Zero);
		pos = Vector3.Zero;
		vel = Vector3.Normalize(val);
		up = Vector3.Normalize(Vector3.Cross(vel, Vector3.Right));
		drawIndex = collection.AllocateSpace();
		drawIndex = drawIndex * 3 * VertexPositionNormalTexture.SizeInBytes;
		collection.vBuffer.SetData<VertexPositionNormalTexture>(drawIndex, array2, 0, 3, VertexPositionNormalTexture.SizeInBytes);
		speed = 25f;
		accel = _accel;
		maxspeed = 5f;
		state = 0;
		hitPoints = 1;
		launched = false;
		active = true;
		dirty = true;
		enem = _enem;
		fillMode = (FillMode)2;
	}

	public override Matrix Transformation()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsLaunched)
		{
			return base.Transformation();
		}
		return enem.Transformation() * base.Transformation();
	}

	public override void BossPartLeave()
	{
		((Pythagoras)enem).bosshp[part]--;
	}

	public override void TargetStart()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		addTarget(targetPos, 1, 5);
	}

	public override void p_act(GameTime gametime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (!exists)
		{
			return;
		}
		oldvel = vel;
		if (followPath)
		{
			Vector3 val = pos - oldPos;
			if ((double)((Vector3)(ref val)).LengthSquared() > 0.02)
			{
				vel = Vector3.Normalize(val) * pathList.maxSpeed();
				partVel = -vel * 0.05f;
			}
		}
		if (vel != oldvel)
		{
			up = BaseGame.GetUpVector(vel, up, oldvel);
		}
		rot += BulletPlane.rotInc * (float)gametime.ElapsedGameTime.TotalSeconds;
		partCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (partCooldown <= 0f)
		{
			BaseGame.Get().ps.AddParticles(getPos(), partVel, 0.2f, 45f, Vector3.Zero, 0f, 1f, 2f, 0f, new Vector4(emisColor, 1f), numParticles, genLength);
			partCooldown += maxCooldown;
		}
	}
}
