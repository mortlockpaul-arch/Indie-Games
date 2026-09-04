using System;
using System.Collections.Generic;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public class BossShield : BossParts
{
	public XSIModel breakModel;

	public IEnumerator<object> ShotTaskUpdater;

	private float enemyShotNear;

	private float enemyShotFar;

	public float Rotation { get; private set; }

	public int FirstShotWait { get; set; }

	public int ShotInterval { get; set; }

	public new event Action<Vector3> Shot;

	public BossShield(Game game, Boss parent, Vector3 offset, float rotation)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, parent, offset);
		Rotation = rotation;
		ShotTaskUpdater = UpdateShot();
	}

	public override void Initialize()
	{
		ContentManager content = base.Parent.Game.Content;
		GameSettings gameSettings = content.Load<GameSettings>("GameSettings");
		enemyShotNear = gameSettings.EnemyShotNear;
		enemyShotFar = gameSettings.EnemyShotFar;
		model = new XSIModel("Models/Models/boss/boss_shield", content);
		breakMotion = new XSIModel("Models/Models/boss/boss_shield_breakmotion", content);
		breakModel = new XSIModel("Models/Models/boss/boss_shield_break", content);
		collision = new XSIModel("Models/Models/boss/boss_shield_col", content);
		model.Play();
		collision.Play();
		breakModel.Play();
		model.FixedUpdate(model.Animation.Duration);
		collision.FixedUpdate(collision.Animation.Duration);
		breakModel.FixedUpdate(breakModel.Animation.Duration);
		base.Initialize();
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (base.Parent.IsBattle)
		{
			base.UpdateTime += elapsedGameTime;
		}
		collision.FixedUpdate(base.UpdateTime);
		collision.UpdateBoundingSphere(GetWorld());
		if (!base.IsBreak)
		{
			Vector3 position = GetPosition();
			if (position.Z <= enemyShotNear && position.Z >= enemyShotFar)
			{
				ShotTaskUpdater.MoveNext();
			}
		}
		base.UpdateMain(elapsedGameTime);
	}

	public override IEnumerator<object> Task()
	{
		yield return null;
	}

	public override IEnumerator<object> UpdateShot()
	{
		while (base.Parent.Phase == Boss.StatusPhase.Appear)
		{
			yield return null;
		}
		for (int i = 0; i < FirstShotWait; i++)
		{
			yield return null;
		}
		while (!base.IsBreak && base.Parent.IsBattle)
		{
			if (Shot != null)
			{
				Matrix val = model.CrosswalkModel.Bones["shot_null"].Transform * GetWorld();
				Shot(((Matrix)(ref val)).Translation);
			}
			for (int j = 0; j < ShotInterval; j++)
			{
				yield return null;
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		DrawModel((!base.IsBreak) ? model : breakModel, update: true);
	}

	public override void Play()
	{
		model.Play();
		collision.Play();
		base.UpdateTime = TimeSpan.Zero;
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		return ((Matrix)(ref world)).Translation;
	}

	public override Matrix GetWorld()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.Parent.GetPosition();
		return Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation)) * Matrix.CreateTranslation(position);
	}

	public override bool Damage(int damage)
	{
		if (base.Parent.core.Status == BossCore.ShieldStatus.Open)
		{
			return base.Parent.core.Damage(damage);
		}
		return base.Damage(damage);
	}
}
