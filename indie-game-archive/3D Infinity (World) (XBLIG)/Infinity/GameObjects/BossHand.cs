using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public class BossHand : BossParts
{
	private const float AmountFrameValue = 1f / 60f;

	public float AttackAmount { get; set; }

	public int StartWait { get; set; }

	public int AttackInterval { get; set; }

	public BossHand(Game game, Boss parent, Vector3 offset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, parent, offset);
	}

	public override void Initialize()
	{
		ContentManager content = base.Parent.Game.Content;
		model = new XSIModel("Models/Models/boss/boss_hand", content);
		breakMotion = new XSIModel("Models/Models/boss/boss_hand_breakmotion", content);
		collision = new XSIModel("Models/Models/boss/boss_hand_col", content);
		model.Play();
		collision.Play();
		model.FixedUpdate(model.Animation.Duration);
		collision.FixedUpdate(collision.Animation.Duration);
		base.Initialize();
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (base.Parent.IsBattle)
		{
			base.UpdateTime += elapsedGameTime;
		}
		collision.FixedUpdate(base.UpdateTime);
		collision.UpdateBoundingSphere(GetWorld());
		base.UpdateMain(elapsedGameTime);
	}

	public override IEnumerator<object> Task()
	{
		while (base.Parent.Phase == Boss.StatusPhase.Appear)
		{
			yield return null;
		}
		for (int i = 0; i < StartWait; i++)
		{
			yield return null;
		}
		while (!base.IsBreak)
		{
			Play();
			for (int j = 0; j < 60; j++)
			{
				AttackAmount += 1f / 60f;
				yield return null;
			}
			for (int k = 0; k < 60; k++)
			{
				AttackAmount -= 1f / 60f;
				yield return null;
			}
			AttackAmount = 0f;
			for (int l = 0; l < AttackInterval; l++)
			{
				yield return null;
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		DrawModel(model, update: true);
	}

	public override void Play()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		model.Play();
		collision.Play();
		base.UpdateTime = TimeSpan.Zero;
		base.Target = base.Parent.Target;
		AttackAmount = 0f;
		PlaySE("SE08");
	}

	public override Matrix GetWorld()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateTranslation(GetPosition());
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Lerp(GetDockPosition(), base.Target, AttackAmount);
	}

	public Vector3 GetDockPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return base.Parent.GetPosition() + Offset;
	}
}
