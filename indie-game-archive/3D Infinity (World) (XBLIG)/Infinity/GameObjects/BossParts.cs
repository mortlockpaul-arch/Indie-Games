using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public abstract class BossParts : EnemyData
{
	public Vector3 Offset;

	public XSIModel breakMotion;

	protected GameSettings gameSettings;

	[CompilerGenerated]
	private Vector3 _003CTarget_003Ek__BackingField;

	public TimeSpan UpdateTime { get; set; }

	public bool IsBreakMotion { get; private set; }

	public bool IsBreakMotionFinished { get; private set; }

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

	public Boss Parent { get; private set; }

	public BossParts(Game game, Boss parent, Vector3 offset)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game);
		Parent = parent;
		_ = Parent.Game.Content;
		base.IsBreak = false;
		Offset = offset;
		Initialize();
		UpdateTime = model.Animation.Duration;
		Action<int> destruction = Destruction;
		Action<int> b = delegate
		{
			Parent.SetShake(5f);
		};
		Destruction = (Action<int>)Delegate.Combine(destruction, b);
	}

	public override void Initialize()
	{
		ContentManager content = game.Content;
		gameSettings = content.Load<GameSettings>("GameSettings");
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		if (!IsBreakMotion)
		{
			model.Update(elapsedGameTime);
		}
		else
		{
			breakMotion.Update(elapsedGameTime);
		}
		base.UpdateMain(elapsedGameTime);
	}

	public abstract IEnumerator<object> Task();

	protected void DrawModel(XSIModel drawModel, bool update)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		if (!IsBreakMotion)
		{
			if (update)
			{
				drawModel.FixedUpdate(UpdateTime);
			}
			drawModel.AmbientLightColor = Vector3.Lerp(Vector3.Zero, gameSettings.DamageColor, base.FlashAmount);
			drawModel.Draw(Global.SASData, world);
		}
		else
		{
			breakMotion.Draw(Global.SASData, world);
		}
	}

	public abstract void Play();

	public virtual void PlayBreakMotion()
	{
		IsBreakMotion = true;
		IsBreakMotionFinished = false;
		breakMotion.Finished += delegate
		{
			IsBreakMotionFinished = true;
		};
		breakMotion.Play();
	}
}
