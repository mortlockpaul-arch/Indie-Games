using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public class BossCore : BossParts
{
	public enum ShieldStatus
	{
		Open,
		Close
	}

	private const int ShieldActionFrame = 40;

	public ShieldStatus Status { get; set; }

	public int OpenInterval { get; set; }

	public BossCore(Game game, Boss parent, Vector3 offset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, parent, offset);
		Status = ShieldStatus.Close;
	}

	public override void Initialize()
	{
		ContentManager content = base.Parent.Game.Content;
		model = new XSIModel("Models/Models/boss/boss_core", content);
		breakMotion = new XSIModel("Models/Models/boss/boss_core_breakmotion", content);
		collision = new XSIModel("Models/Models/boss/boss_core_col", content);
		model.Play(isLoop: true);
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
		while (true)
		{
			base.Parent.OpenShields();
			PlaySE("SE07");
			for (int i = 0; i < 40; i++)
			{
				yield return null;
			}
			Status = ShieldStatus.Open;
			for (int j = 0; j < 40; j++)
			{
				yield return null;
			}
			for (int k = 0; k < 40; k++)
			{
				yield return null;
			}
			Status = ShieldStatus.Close;
			for (int l = 0; l < OpenInterval; l++)
			{
				yield return null;
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		DrawModel(model, update: false);
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.Parent.GetPosition();
		return Matrix.CreateTranslation(position);
	}

	public override bool Damage(int damage)
	{
		if (Status == ShieldStatus.Open)
		{
			return base.Damage(damage);
		}
		return false;
	}
}
