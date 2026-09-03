using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BulletPlaneCollection : Enemy
{
	public VertexBuffer vBuffer;

	public ModelWrapper parent;

	public List<BulletPlane> enemies;

	public List<BulletPlane> detached;

	public int maxSize;

	public int curIndex;

	public Random r;

	public BulletPlaneCollection(ref ModelWrapper _parent)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		maxSize = 36864;
		base._002Ector();
		enemies = new List<BulletPlane>();
		detached = new List<BulletPlane>();
		vBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, maxSize * VertexPositionNormalTexture.SizeInBytes, (BufferUsage)8);
		parent = _parent;
		r = new Random();
		hitPoints = 1;
	}

	public void AddPlane(Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, Vector3 difColor, Vector3 emisColor, PathList pList, int part, float accel, bool followPath)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		AddPlane(_enem, ref _parent, _mesh, _planeIndex, boneIndex, difColor, emisColor, pList, part, accel, followPath, (FillMode)2);
	}

	public void AddPlane(Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, Vector3 difColor, Vector3 emisColor, PathList pList, int part, float accel, bool followPath, FillMode fMode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		enemies.Add(new BulletPlane(ref _enem, ref _parent, _mesh, _planeIndex, boneIndex, this, difColor, emisColor, pList, part, accel, followPath, fMode));
		enemies[enemies.Count - 1].start();
		BaseGame.Get().enems.Add(enemies[enemies.Count - 1]);
	}

	public void AddAttachedPlane(Enemy _enem, ref ModelWrapper _parent, int _mesh, int _planeIndex, int boneIndex, Vector3 difColor, Vector3 emisColor, PathList pList, int part, float accel, bool followPath)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		enemies.Add(new AttachedPlane(ref _enem, ref _parent, _mesh, _planeIndex, boneIndex, this, difColor, emisColor, pList, part, accel, followPath));
		enemies[enemies.Count - 1].start();
		BaseGame.Get().enems.Add(enemies[enemies.Count - 1]);
	}

	public override void act(GameTime gametime)
	{
		if (exists && hitPoints <= 0)
		{
			leave();
		}
	}

	public override void draw(GameTime gametime)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().SetUpEffect(ref parent.epc[0], clearEpc: false);
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().fogEffect.Parameters["usePalette"].SetValue(false);
		BaseGame.Get().fogEffect.Parameters["xDoubleSided"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["xEnableLighting"].SetValue(true);
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VPNTDec;
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (BulletPlane enemy in enemies)
		{
			enemy.DrawPlane();
		}
		foreach (BulletPlane item in detached)
		{
			item.DrawPlane();
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
	}

	public int AllocateSpace()
	{
		int result = curIndex;
		curIndex += 3;
		return result;
	}

	public void Launch()
	{
		if (enemies.Count > 0)
		{
			int index = r.Next(0, enemies.Count);
			enemies[index].Launch();
			detached.Add(enemies[index]);
			enemies.RemoveAt(index);
		}
	}

	public override void start()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.start();
		BaseGame.Get().actualEnem--;
		pos = Vector3.Zero;
	}

	public override Enemy attack()
	{
		return new ECube();
	}

	public override string name()
	{
		return "[plane_col]";
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}

	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.Identity;
	}

	public override TargetEffectCol lockOn(int targetsLeft)
	{
		return new TargetEffectCol();
	}
}
