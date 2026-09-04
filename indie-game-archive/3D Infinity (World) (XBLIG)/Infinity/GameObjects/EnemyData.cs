using System;
using System.Collections.Generic;
using InfinityLibrary;
using Microsoft.Xna.Framework;

namespace Infinity.GameObjects;

public class EnemyData : ModelObject
{
	public IEnumerator<object> ShotTask;

	private int frame;

	private int shotIndex;

	public bool IsBreak { get; set; }

	public int Score { get; set; }

	public int Move { get; set; }

	public TimeSpan AnimationTime { get; set; }

	public bool IsLockOnEnabled { get; set; }

	public int LockOnIndex { get; private set; }

	public TimeSpan SightLockTime { get; set; }

	public float FlashAmount { get; set; }

	public EnemyShotSettings ShotSettings { get; set; }

	public GameSettings GameSettings { get; private set; }

	public event Action<ExplosionType, Vector3> Explosion;

	public event Action<Vector3, Vector3?, float> Shot;

	public EnemyData(Game game)
		: base(game)
	{
		GameSettings = game.Content.Load<GameSettings>("GameSettings");
		Unlock(null);
	}

	public override void Initialize()
	{
		throw new NotImplementedException();
	}

	public override void Dispose()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		IsBreak = false;
		Position = Vector3.Zero;
		AnimationTime = TimeSpan.Zero;
		Unlock(null);
		SightLockTime = TimeSpan.Zero;
		ShotSettings = null;
		Shot = null;
		ShotTask = null;
		frame = 0;
		base.Dispose();
	}

	public void Dispose(MissileManager missileManager)
	{
		Unlock(missileManager);
		Dispose();
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		AnimationTime += elapsedGameTime;
		int num = (int)((AnimationTime.Ticks > 0) ? (AnimationTime.Ticks / 166666) : 0);
		while (frame <= num)
		{
			FlashAmount *= 0.5f;
			SightLockTime = ((LockOnIndex >= 0) ? (SightLockTime + elapsedGameTime) : TimeSpan.Zero);
			if (ShotSettings != null)
			{
				if (ShotTask == null)
				{
					ShotTask = UpdateShot();
				}
				Vector3 position = GetPosition();
				if (position.Z <= GameSettings.EnemyShotNear && position.Z >= GameSettings.EnemyShotFar)
				{
					ShotTask.MoveNext();
				}
			}
			frame++;
		}
	}

	public virtual IEnumerator<object> UpdateShot()
	{
		if (ShotSettings == null)
		{
			yield break;
		}
		shotIndex = 0;
		for (int i = 0; i < ShotSettings.Frame; i++)
		{
			yield return null;
		}
		ShotAction();
		yield return null;
		while (ShotSettings.Interval >= 0)
		{
			for (int j = 0; j < ShotSettings.Interval; j++)
			{
				yield return null;
			}
			ShotAction();
			yield return null;
		}
	}

	private void ShotAction()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (ShotSettings.Normals.Length == 0)
		{
			Shot(Position, null, ShotSettings.Speed);
			return;
		}
		Shot(Position, ShotSettings.Normals[shotIndex] * ShotSettings.Speed, ShotSettings.Speed);
		shotIndex = (shotIndex + 1) % ShotSettings.Normals.Length;
	}

	public override void Draw(GameTime gameTime)
	{
		throw new NotImplementedException();
	}

	public override Matrix GetWorld()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateTranslation(Position);
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		return ((Matrix)(ref world)).Translation;
	}

	public override bool Damage(int damage)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!IsBreak)
		{
			base.Vitality = Math.Max(base.Vitality - damage, 0);
			FlashAmount = 1f;
			IsBreak = base.Vitality <= 0;
			if (IsBreak)
			{
				if (Explosion != null)
				{
					Action<ExplosionType, Vector3> explosion = Explosion;
					Matrix world = GetWorld();
					explosion(ExplosionType.Normal, ((Matrix)(ref world)).Translation);
				}
				if (Destruction != null)
				{
					Destruction(Score);
				}
			}
			return true;
		}
		return false;
	}

	public void LockOn(int missileIndex)
	{
		if (missileIndex >= 0)
		{
			LockOnIndex = missileIndex;
		}
	}

	public void Unlock(MissileManager missileManager)
	{
		missileManager?.DisableReserb(LockOnIndex);
		LockOnIndex = -1;
	}
}
