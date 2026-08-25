using System;

namespace BunnyOfWar.AI;

public static class aiZombie
{
	public static void doSomething(FighterObject cpu, FighterObject human)
	{
		bool flag = false;
		float layerDepth = cpu.getLayerDepth();
		if (layerDepth < Definitions.LayerDepthForGround)
		{
			cpu.onDeath();
			return;
		}
		if (cpu.Y > 600)
		{
			cpu.onDeath();
			return;
		}
		switch (cpu.PROPERTIES.AImode)
		{
		case AI.modes.X:
			if (!attackIfPossible(cpu, human, (int)cpu.PROPERTIES.DamageFromAttack))
			{
				moveCloser(cpu, human, cpu.PROPERTIES.moveSpeed * 2);
			}
			break;
		case AI.modes.Y:
			if (!attackIfPossible(cpu, human, (int)cpu.PROPERTIES.DamageFromAttack))
			{
				moveCloser(cpu, human, cpu.PROPERTIES.moveSpeed * 2);
			}
			break;
		}
	}

	private static bool attackIfPossible(FighterObject cpu, FighterObject human, int amount)
	{
		if (cpu.PROPERTIES.CpuAttackCooldown < DateTime.Now && cpu.getWhereFistIs().Intersects(human.getWhereBodyIs()))
		{
			if (cpu.PROPERTIES.CpuAttackCooldown == DateTime.MinValue)
			{
				if (!RandomStaticGlobals.isHardMode)
				{
					cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(500.0);
				}
				else
				{
					cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(100.0);
				}
				return true;
			}
			if (cpu.X < human.X)
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.right;
			}
			else
			{
				cpu.PROPERTIES.isFacing = Definitions.facing.left;
			}
			cpu.PlayAnimation(FighterObjectProperties.AnimationName.Punching, broadcastThis: true);
			cpu.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddMilliseconds(200.0);
			human.healthChange(-amount);
			return true;
		}
		return false;
	}

	private static void moveCloser(FighterObject cpu, FighterObject human, int distance)
	{
		if (RandomStaticGlobals.isHardMode)
		{
			distance = (int)((float)distance * 1.5f);
		}
		if (cpu.getWhereFistIs().Intersects(human.getWhereBodyIs()))
		{
			return;
		}
		if (cpu.PROPERTIES.AIMemory2 == "" && cpu.getPersonalSpace().Intersects(human.getPersonalSpace()))
		{
			int num = 0;
			num = ((cpu.X > human.X) ? 1 : (-1));
			cpu.PROPERTIES.AIMemory2 = "x";
			if (cpu.PROPERTIES.name.Contains("dog"))
			{
				SoundManager.playNextZombieMoan(isDog: true, num);
			}
			else
			{
				SoundManager.playNextZombieMoan(isDog: false, num);
			}
		}
		if (cpu.X > human.X)
		{
			cpu.X -= distance;
			cpu.PROPERTIES.isFacing = Definitions.facing.left;
			cpu.PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		}
		else
		{
			cpu.X += distance;
			cpu.PROPERTIES.isFacing = Definitions.facing.right;
			cpu.PlayAnimation(FighterObjectProperties.AnimationName.Walking, broadcastThis: true);
		}
	}
}
