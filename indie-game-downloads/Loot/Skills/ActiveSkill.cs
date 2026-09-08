using System;
using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Effects;
using Microsoft.Xna.Framework;

namespace Loot.Skills;

public abstract class ActiveSkill : Skill
{
	protected TimeSpan CoolDownTimer = TimeSpan.Zero;

	protected TimeSpan CoolDownDuration = TimeSpan.FromSeconds(3.0);

	protected TimeSpan ActiveTimer = TimeSpan.Zero;

	protected int ChainLevel;

	private bool chainActivated;

	private bool chainBroken;

	public bool CanCast => Level > 0 && !Active && CoolDownTimer == TimeSpan.Zero;

	public bool CanChain => Active && !chainBroken && ChainLevel <= Level && ActiveTimer <= ChainWindow;

	public bool ChainBroken => chainBroken;

	public float ChainTimer
	{
		get
		{
			float num = (float)(ChainWindow.TotalSeconds / 2.0);
			float num2 = (float)((ActiveTimer.TotalSeconds - (double)num) / (ActiveTimerDuration.TotalSeconds - (double)num));
			if ((double)num2 < 0.0)
			{
				num2 = 0f;
			}
			return num2;
		}
	}

	public virtual bool Active => ActiveTimer > TimeSpan.Zero;

	public float CoolDown => (float)(CoolDownTimer.TotalSeconds / CoolDownDuration.TotalSeconds);

	protected abstract TimeSpan ActiveTimerDuration { get; }

	protected virtual TimeSpan ChainWindow => TimeSpan.FromMilliseconds(200.0);

	public ActiveSkill()
	{
	}

	public ActiveSkill(BinaryReader reader)
		: base(reader)
	{
	}

	public void Cast()
	{
		if (Level == 0)
		{
			return;
		}
		if (CanCast)
		{
			ChainLevel = 1;
			activateSkill();
		}
		else if (Active && !chainBroken)
		{
			if (ActiveTimer > ChainWindow || ChainLevel > Level)
			{
				PlaySound.Fail();
				chainBroken = true;
			}
			else
			{
				if (chainActivated)
				{
					return;
				}
				if (ChainLevel < Level)
				{
					chainActivated = true;
				}
				else if (ChainLevel == Level)
				{
					chainActivated = true;
					PlaySound.MaxChain();
					DM.AddEffect(FXText.GetFX(DM.Player.Location, "MAX CHAIN!", new Color(0, 192, 192), Color.Black));
					if (Level == 10)
					{
						Profile.Awardments.Unlock(Awardment.SuperChain);
					}
				}
			}
		}
		else if (!chainBroken)
		{
			PlaySound.Fail();
			chainBroken = true;
		}
	}

	private void activateSkill()
	{
		Activate();
		ActiveTimer = ActiveTimerDuration;
		CoolDownTimer = CoolDownDuration;
		chainBroken = false;
		chainActivated = false;
	}

	protected abstract void Activate();

	public override void Update(GameTime gameTime)
	{
		if (ActiveTimer > TimeSpan.Zero)
		{
			ActiveTimer -= gameTime.ElapsedGameTime;
			if (ActiveTimer <= TimeSpan.Zero)
			{
				ActiveTimer = TimeSpan.Zero;
				if (chainActivated)
				{
					ChainLevel++;
					activateSkill();
				}
			}
		}
		else if (CoolDownTimer > TimeSpan.Zero)
		{
			CoolDownTimer -= gameTime.ElapsedGameTime;
			if (CoolDownTimer < TimeSpan.Zero)
			{
				CoolDownTimer = TimeSpan.Zero;
				chainBroken = false;
			}
		}
	}

	protected override void Read(BinaryReader reader)
	{
		base.Read(reader);
		CoolDownTimer = TimeSpan.FromMilliseconds((double)reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write((int)CoolDownTimer.TotalMilliseconds);
	}
}
