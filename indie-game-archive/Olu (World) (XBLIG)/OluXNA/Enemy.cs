using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Enemy
{
	public ConditionSet attackCond;

	public Vector3 pos;

	public Vector3 rotAxis;

	public float rotAngle;

	public float deltaRot;

	public List<Target> targets;

	public int hitPoints;

	public int state;

	public float attackCooldown;

	public bool exists;

	public FillMode fillMode;

	public bool near;

	public PathList pathList;

	public Dictionary<int, WaitCond> _eCond;

	public Enemy()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		targets = new List<Target>();
		attackCond = new ConditionSet();
		pathList = new PathList();
		_eCond = new Dictionary<int, WaitCond>();
		fillMode = (FillMode)3;
		rotAxis = Vector3.Right;
		rotAngle = 0f;
		deltaRot = 0f;
	}

	public void Dispose()
	{
		attackCond.Dispose();
		targets.Clear();
		pathList.Dispose();
		_eCond.Clear();
	}

	public virtual void draw(GameTime gametime)
	{
	}

	public virtual Matrix Transformation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateTranslation(getPos());
	}

	public virtual Enemy attack()
	{
		return new Enemy();
	}

	public virtual void hit(TargetEffectBase toHit)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == toHit.eTarget)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			targets[num].hp--;
			if (toHit.eTarget.fillMode != toHit.fillMode)
			{
				targets[num].hp--;
			}
			targets[num].selected--;
			if (toHit.eTarget.fillMode != toHit.fillMode)
			{
				targets[num].selected--;
			}
			if (targets[num].hp <= 0)
			{
				targets.RemoveAt(num);
			}
		}
		if (exists)
		{
			hitPoints--;
			if (toHit.eTarget.fillMode != toHit.fillMode)
			{
				hitPoints--;
			}
			if (hitPoints <= 0)
			{
				die();
			}
		}
	}

	public virtual void act(GameTime gametime)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = pathList.Update(gametime);
		if (!exists)
		{
			return;
		}
		if (flag)
		{
			leave();
			return;
		}
		pos = pathList.curLocation();
		if (attackCooldown > 0f)
		{
			attackCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (attackCond.ConditionsMet() && attackCooldown <= 0f)
		{
			BaseGame.Get().enems.Add(attack());
			attackCooldown = 2f;
		}
	}

	public virtual void start()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		pathList.ResetCurrent();
		exists = true;
		pos = pathList.curLocation();
		BaseGame.Get().actualEnem++;
	}

	public virtual void die()
	{
		exists = false;
		BaseGame.Get().enems.Remove(this);
		BaseGame.Get().actualEnem--;
	}

	public virtual void leave()
	{
		exists = false;
		BaseGame.Get().enems.Remove(this);
		BaseGame.Get().actualEnem--;
	}

	public virtual void addPath(IPath _path)
	{
		pathList.Add(_path);
	}

	public void addPathComboList(List<IPath> _paths, IPath _comboPart)
	{
		pathList.addPathComboList(_paths, _comboPart);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Target target = new Target();
		target.pos = _pos;
		target.score = _score;
		target.selected = 0;
		target.enem = this;
		target.hp = _hp;
		target.fillMode = fillMode;
		targets.Add(target);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, ref ModelWrapper _model, int _id)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		BoneTarget boneTarget = new BoneTarget();
		boneTarget.pos = _pos;
		boneTarget.score = _score;
		boneTarget.selected = 0;
		boneTarget.enem = this;
		boneTarget.hp = _hp;
		boneTarget.model = _model;
		boneTarget.id = _id;
		boneTarget.fillMode = fillMode;
		targets.Add(boneTarget);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, ref ModelWrapper _model, int _id, string _boneName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		addTarget(_pos, _hp, _score, ref _model, _id, _boneName, null);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, ref ModelWrapper _model, int _id, string _boneName, object obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		addTarget(_pos, _hp, _score, ref _model, _id, ((ReadOnlyCollection<ModelBone>)(object)_model.model.Bones).IndexOf(_model.model.Bones[_boneName]), obj, fillMode);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, ref ModelWrapper _model, int _id, string _boneName, object obj, FillMode _fillMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		addTarget(_pos, _hp, _score, ref _model, _id, ((ReadOnlyCollection<ModelBone>)(object)_model.model.Bones).IndexOf(_model.model.Bones[_boneName]), obj, _fillMode);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, ref ModelWrapper _model, int _id, int _boneID, object obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		addTarget(_pos, _hp, _score, ref _model, _id, _boneID, obj, fillMode);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, ref ModelWrapper _model, int _id, int _boneID, object obj, FillMode _fillMode)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		BoneModelTarget boneModelTarget = ((obj == null) ? new BoneModelTarget() : new BoneModelObjectTarget());
		boneModelTarget.pos = _pos;
		boneModelTarget.score = _score;
		boneModelTarget.selected = 0;
		boneModelTarget.enem = this;
		boneModelTarget.hp = _hp;
		boneModelTarget.model = _model;
		boneModelTarget.id = _id;
		boneModelTarget.boneName = _boneID;
		boneModelTarget.fillMode = _fillMode;
		if (obj != null)
		{
			((BoneModelObjectTarget)boneModelTarget).obj = obj;
		}
		targets.Add(boneModelTarget);
	}

	public virtual void addTarget(int _hp, int _score, ref ModelWrapper _model, int _mesh, int _index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		addTarget(_hp, _score, ref _model, _mesh, _index, Matrix.Identity);
	}

	public virtual void addTarget(int _hp, int _score, ref ModelWrapper _model, int _mesh, int _index, Matrix targetMod)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		FaceTarget faceTarget = new FaceTarget();
		faceTarget.hp = _hp;
		faceTarget.score = _score;
		faceTarget.model = _model;
		faceTarget.meshNum = _mesh;
		faceTarget.indexNum = _index;
		faceTarget.modMatrix = targetMod;
		faceTarget.enem = this;
		faceTarget.fillMode = fillMode;
		targets.Add(faceTarget);
	}

	public virtual void addTarget(Vector3 _pos, int _hp, int _score, float waterHeight)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		WaterTarget waterTarget = new WaterTarget();
		waterTarget.pos = _pos;
		waterTarget.score = _score;
		waterTarget.selected = 0;
		waterTarget.enem = this;
		waterTarget.hp = _hp;
		waterTarget.fillMode = fillMode;
		waterTarget.waterHeight = waterHeight;
		targets.Add(waterTarget);
	}

	public virtual void addCond(ICondition _cond)
	{
		attackCond.set.Add(_cond);
	}

	public virtual Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pos;
	}

	public virtual void setPos(Vector3 _pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		pos = _pos;
	}

	public virtual string name()
	{
		return "undefined";
	}

	public virtual void PlayerHit()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(BaseGame.Get().playerPos, getPos()) < 5f)
		{
			if (BaseGame.Get().EasyMode)
			{
				leave();
				return;
			}
			BaseGame.Get().PlayerHit();
			leave();
		}
		else if (Vector3.Distance(BaseGame.Get().playerPos, getPos()) < 25f && !near)
		{
			if (targets.Count > 0)
			{
				TargetEffect targetEffect = new TargetEffectNear();
				targetEffect.enem = this;
				targetEffect.activated = true;
				targetEffect.countDown = 3f;
				targetEffect.eTarget = targets[0];
				BaseGame.Get().targetFX.Insert(0, targetEffect);
			}
			near = true;
		}
	}

	public virtual TargetEffectCol lockOn(int targetsLeft)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		TargetEffectCol targetEffectCol = new TargetEffectCol();
		List<Vector2> window = BaseGame.Get().window;
		_ = (float)BaseGame.S_HEIGHT / 2f / (float)Math.Tan(Math.PI / 4.0);
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i].selected >= targets[i].hp)
			{
				continue;
			}
			Vector3 val = targets[i].absolutePos();
			Vector3 val2 = Vector3.Transform(val, BaseGame.Get().targetTransform);
			if (!(val2.Z > 0f) || !(val2.Z < (float)BaseGame.CAN_TARGET))
			{
				continue;
			}
			Viewport viewport = BaseGame.Get().graphics.GraphicsDevice.Viewport;
			val2 = ((Viewport)(ref viewport)).Project(val, BaseGame.Get().worldViewProjTransform, Matrix.Identity, Matrix.Identity);
			if ((!(window[0].X <= val2.X) || !(window[1].X >= val2.X) || !(window[0].Y <= val2.Y) || !(window[1].Y >= val2.Y)) && !BaseGame.Get().MEGA_ON)
			{
				continue;
			}
			targets[i].selected++;
			if (targets[i].fillMode != BaseGame.Get().fillMode)
			{
				targets[i].selected++;
			}
			TargetEffectBase targetEffectBase = new TargetEffectBase();
			targetEffectBase.enem = targets[i].enem;
			targetEffectBase.fillMode = BaseGame.Get().fillMode;
			targetEffectBase.eTarget = targets[i];
			targetEffectBase.countDown = 1.6304f;
			if (BaseGame.Get().MEGA_ON || BaseGame.Get().FREEZE_ON)
			{
				targetEffectBase.disablePowerScore = true;
				if (BaseGame.Get().MEGA_ON)
				{
					targetEffectBase.skipSquare = true;
				}
				else
				{
					targetEffectBase.ignoreBeat = true;
				}
			}
			targetEffectCol.fx.Add(targetEffectBase);
			if (!BaseGame.Get().MEGA_ON || targetEffectCol.fx.Count == targetsLeft)
			{
				break;
			}
		}
		return targetEffectCol;
	}

	public virtual void ClearLock(Target toClear)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == toClear)
			{
				targets[i].selected--;
				if (targets[i].fillMode != BaseGame.Get().fillMode)
				{
					targets[i].selected--;
				}
			}
		}
	}

	public virtual int destroy()
	{
		return 0;
	}

	public Enemy(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
	}

	public virtual bool Check(int numEnem)
	{
		return BaseGame.Get().OnBeat(BaseGame.Beats.Eighth);
	}

	public virtual void HitSound(int lockNum, float volume)
	{
	}

	public virtual WaitCond GetSoundCue(int lockNum)
	{
		if (lockNum > 8)
		{
			lockNum = 8;
		}
		if (_eCond.ContainsKey(lockNum))
		{
			return _eCond[lockNum];
		}
		return new WaitCond("snglHat", Beats.Eighth);
	}
}
