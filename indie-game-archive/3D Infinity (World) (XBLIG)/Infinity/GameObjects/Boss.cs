using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public class Boss : IDisposable
{
	public enum StatusPhase
	{
		Appear,
		Battle,
		Dead
	}

	public BossCore core;

	public BossShield[] shields;

	public BossHand[] hands;

	private XSIModel motionAppear;

	private XSIModel motionBattle;

	public Vector3 Position;

	public Vector3 Wave;

	public Vector3 Shake;

	public Vector3 BattlePosition;

	public Vector3 BattlePositionVelocity;

	public TimeSpan BattleTime;

	private List<IEnumerator<object>> taskList;

	private IEnumerator<object> updateWave;

	private IEnumerator<object> updateBreakout;

	private Random random = new Random();

	private float ShakeRange;

	private BossSettings settings;

	private int frame;

	[CompilerGenerated]
	private Vector3 _003CTarget_003Ek__BackingField;

	public Game Game { get; private set; }

	public StatusPhase Phase { get; private set; }

	public TimeSpan UpdateTime { get; set; }

	public Vector3 Target
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CTarget_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CTarget_003Ek__BackingField = value;
		}
	}

	public bool IsBattle
	{
		get
		{
			if (Phase != StatusPhase.Appear)
			{
				return Phase != StatusPhase.Dead;
			}
			return false;
		}
	}

	public event Action<Vector3> Explosion;

	public event Action<Vector3> Destruction;

	public event Action BattleFinished;

	public event Action BreakMotionFinished;

	public event Action<Vector3> Shot;

	public Boss(Game game, BossSettings settings, ContentManager motionContent)
	{
		Game = game;
		this.settings = settings;
		Initialize(motionContent);
	}

	public void Initialize(ContentManager motionContent)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		_ = Game.Content;
		motionAppear = new XSIModel(settings.MotionAppearAsset, motionContent);
		motionBattle = new XSIModel(settings.MotionBattleAsset, motionContent);
		motionAppear.Play();
		motionBattle.Play(isLoop: true);
		motionAppear.Finished += delegate
		{
			Phase = StatusPhase.Battle;
		};
		core = new BossCore(Game, this, Vector3.Zero);
		shields = new BossShield[3]
		{
			new BossShield(Game, this, Vector3.Zero, 0f),
			new BossShield(Game, this, Vector3.Zero, 120f),
			new BossShield(Game, this, Vector3.Zero, -120f)
		};
		hands = new BossHand[2]
		{
			new BossHand(Game, this, settings.HandDistance),
			new BossHand(Game, this, settings.HandDistance * Vector3.Left)
		};
		taskList = new List<IEnumerator<object>>();
		taskList.Add(core.Task());
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			taskList.Add(bossShield.Task());
			taskList.Add(bossShield.UpdateShot());
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			taskList.Add(bossHand.Task());
		}
		updateWave = UpdateWave();
		updateBreakout = UpdateBreakout();
		core.Enable = true;
		core.Visible = true;
		core.Use = true;
		core.IsLockOnEnabled = true;
		core.Vitality = settings.CoreSettings.Vitality;
		core.OpenInterval = settings.ShieldOpenInterval;
		core.Score = settings.CoreSettings.Score;
		BossCore bossCore = core;
		bossCore.Destruction = (Action<int>)Delegate.Combine(bossCore.Destruction, (Action<int>)delegate
		{
			Phase = StatusPhase.Dead;
			if (BattleFinished != null)
			{
				BattleFinished();
			}
		});
		for (int num3 = 0; num3 < hands.Length; num3++)
		{
			hands[num3].Enable = true;
			hands[num3].Visible = true;
			hands[num3].Use = true;
			hands[num3].IsLockOnEnabled = true;
			hands[num3].Vitality = settings.HandSettings.Vitality;
			hands[num3].Score = settings.HandSettings.Score;
			hands[num3].StartWait = settings.HandAttackFirstWaitStep * (num3 + 1);
			hands[num3].AttackInterval = settings.HandAttackInterval;
		}
		for (int num4 = 0; num4 < shields.Length; num4++)
		{
			shields[num4].Enable = true;
			shields[num4].Visible = true;
			shields[num4].Use = true;
			shields[num4].Vitality = settings.ShieldSettings.Vitality;
			shields[num4].Score = settings.ShieldSettings.Score;
			shields[num4].FirstShotWait = settings.ShieldShotFirstWaitStep + num4 * 20;
			shields[num4].ShotInterval = settings.ShieldShotInterval;
			shields[num4].Shot += delegate(Vector3 position)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Shot(position);
			};
		}
		Phase = StatusPhase.Appear;
		frame = 0;
	}

	public virtual void Dispose()
	{
		core.Dispose();
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.Dispose();
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			bossHand.Dispose();
		}
		frame = 0;
	}

	public void Update(TimeSpan elapsedGameTime, Vector3 target)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		UpdateTime += elapsedGameTime;
		int num = (int)((UpdateTime.Ticks > 0) ? (UpdateTime.Ticks / 166666) : 0);
		UpdatePosition(elapsedGameTime);
		core.Update(elapsedGameTime);
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.Update(elapsedGameTime);
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			bossHand.Update(elapsedGameTime);
		}
		while (frame <= num)
		{
			if (Phase != StatusPhase.Dead)
			{
				updateWave.MoveNext();
				if (IsBattle)
				{
					BattleTime += elapsedGameTime;
				}
				foreach (IEnumerator<object> task in taskList)
				{
					task.MoveNext();
				}
				SetTarget(target);
			}
			else
			{
				updateBreakout.MoveNext();
			}
			SetShake();
			ShakeRange *= 0.9f;
			frame++;
		}
	}

	private void UpdatePosition(TimeSpan elapsedGameTime)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		XSIModel xSIModel = null;
		if (Phase == StatusPhase.Appear)
		{
			xSIModel = motionAppear;
		}
		else if (Phase == StatusPhase.Battle)
		{
			xSIModel = motionBattle;
		}
		if (xSIModel != null)
		{
			xSIModel.Update(elapsedGameTime);
			xSIModel.UpdateBoundingSphere();
			Position = xSIModel.Spheres[0].Center;
		}
	}

	private IEnumerator<object> UpdateWave()
	{
		while (true)
		{
			Wave.X = (float)Math.Sin(UpdateTime.TotalSeconds) * 1f;
			Wave.Y = (float)Math.Cos(UpdateTime.TotalSeconds) * 1f;
			yield return null;
		}
	}

	private void ExplodeAction(float shake)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Explosion(GetPosition());
		BossHand[] array = hands;
		foreach (BossHand bossHand in array)
		{
			Explosion(bossHand.GetDockPosition());
		}
		SetShake(shake);
	}

	private IEnumerator<object> UpdateBreakout()
	{
		ExplodeAction(2f);
		for (int wait = 0; wait < 60; wait++)
		{
			yield return null;
		}
		for (int i = 0; i < 2; i++)
		{
			ExplodeAction(5f);
			for (int j = 0; j < 10; j++)
			{
				yield return null;
			}
		}
		for (int k = 0; k < 60; k++)
		{
			yield return null;
		}
		for (int l = 0; l < 10; l++)
		{
			ExplodeAction(5f);
			for (int m = 0; m < 15; m++)
			{
				yield return null;
			}
		}
		for (int n = 0; n < 10; n++)
		{
			ExplodeAction(n >> 1);
			for (int num = 0; num < 10; num++)
			{
				yield return null;
			}
		}
		PlayBreakMotion();
		do
		{
			yield return null;
		}
		while (!core.IsBreakMotionFinished);
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			_ = bossShield.IsBreakMotionFinished;
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			_ = bossHand.IsBreakMotionFinished;
		}
		BreakMotionFinished();
	}

	public void Draw(GameTime gameTime)
	{
		core.Draw(gameTime);
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.Draw(gameTime);
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			bossHand.Draw(gameTime);
		}
	}

	private void SetTarget(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Target = position;
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.Target = position;
		}
	}

	public void SetShake(float range)
	{
		ShakeRange = range;
		SetShake();
	}

	private void SetShake()
	{
		Shake.X = ((float)random.NextDouble() - 0.5f) * 2f * ShakeRange;
		Shake.Y = ((float)random.NextDouble() - 0.5f) * 2f * ShakeRange;
		Shake.Z = ((float)random.NextDouble() - 0.5f) * 2f * ShakeRange;
	}

	public Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return Position + BattlePosition + BattlePositionVelocity + Wave + Shake;
	}

	protected Matrix GetWorld()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateTranslation(GetPosition());
	}

	public IEnumerator<EnemyData> GetEnemys()
	{
		yield return core;
		try
		{
			BossShield[] array = shields;
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}
		finally
		{
		}
		try
		{
			BossHand[] array2 = hands;
			for (int j = 0; j < array2.Length; j++)
			{
				yield return array2[j];
			}
		}
		finally
		{
		}
	}

	private void PlayBreakMotion()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Destruction(GetPosition());
		core.PlayBreakMotion();
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.PlayBreakMotion();
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			bossHand.PlayBreakMotion();
		}
	}

	public void OpenShields()
	{
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.Play();
		}
	}

	public void Unlock(MissileManager missileManager)
	{
		core.Unlock(missileManager);
		BossShield[] array = shields;
		foreach (BossShield bossShield in array)
		{
			bossShield.Unlock(missileManager);
		}
		BossHand[] array2 = hands;
		foreach (BossHand bossHand in array2)
		{
			bossHand.Unlock(missileManager);
		}
	}
}
