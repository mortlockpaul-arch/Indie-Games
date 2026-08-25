using System;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class FighterObjectProperties
{
	public enum AnimationName
	{
		Idle,
		Blocking,
		Walking,
		Dying,
		Punching,
		QuickPunching,
		RangedSpecialMove,
		Exploding,
		AirborneSlowAttack,
		AirborneQuickAttack,
		BeingCarried,
		CarryingIdle,
		CarryingWalking,
		Whirlwind,
		HammerOfDoom,
		Fling,
		Kicking,
		Crouching,
		PooingStarted,
		PooingFinished,
		Jumping
	}

	private FighterObject parent;

	public float scale = 1f;

	public Vector2 startingPosition = Vector2.One;

	public HumanProfileObject HumanProfile = new HumanProfileObject();

	public string GamerTag = "";

	public int gamerTagX;

	public DateTime AIattackAfter = DateTime.MaxValue;

	public AnimationPlayer sprite;

	public AnimationPlayer spriteBlood;

	public int bloodX;

	public int bloodY;

	public bool isBleeding;

	public string name = "he who has no name";

	public string uniqueName = "";

	public string directoryName = "";

	public float health;

	public float healthMax;

	public int moveSpeed;

	public bool areWeHuman;

	public bool isLocal;

	public bool isAlive;

	public bool isNetworkPlayer;

	public bool isDying;

	public bool isBlocking;

	public bool isFlying;

	public bool isImmuneToDPS;

	public bool isPickupable = true;

	public bool isWalking;

	public bool isCrouching;

	public bool isKicking;

	public DateTime kickExpires = DateTime.MinValue;

	public DateTime crouchExpires = DateTime.MinValue;

	public string CustomAnimationName = "";

	public bool isCountering;

	public Buttons counterButton = Buttons.X;

	public DateTime counteringExpires = DateTime.MinValue;

	public FighterObject attackingFighter;

	public FighterObject targerFighter;

	public Vector2 CpuJumpDestination = Vector2.One;

	public Vector2 CpuMoveDestination = Vector2.One;

	public DateTime CpuAttackCooldown = DateTime.MinValue;

	public DateTime CpuBlockDuration = DateTime.MinValue;

	public DateTime Attack1After = DateTime.MinValue;

	public DateTime Attack2After = DateTime.MinValue;

	public DateTime ChangeAnimationAfter = DateTime.MinValue;

	public float DamageFromAttack;

	public float DamageFromQuickAttack;

	public ObstacleObject holdingObstacleObject;

	public ObstacleObject pushingObstacleObject;

	public FighterObject carryingFighter;

	public FighterObject carriedByFighter;

	public Vector2 circlePivotPoint = Vector2.Zero;

	public Vector2 circleVelocity = Vector2.Zero;

	public float circleRadius;

	public bool isStunned;

	public DateTime stunExpires = DateTime.MinValue;

	public Definitions.FighterSpecialMoves currentAttack;

	public bool isFinishedPunching = true;

	public bool isInTheMiddleOfAnAnimation;

	public BunnyOfWar.AI.AI.modes AImode;

	public int AIAmountSpeed;

	public int AIAmountDistance;

	public string AIMemory = "";

	public string AIMemory2 = "";

	public int AIMemoryInt;

	public int AIMemoryInt2;

	public string AITrigger = "";

	public Vector2 momentum = Vector2.Zero;

	public Vector2 velocity = Vector2.Zero;

	public PlayerIndex? PlayerIndexControllerNumber;

	public int score;

	public GamePadState? previousGamePadState = null;

	public KeyboardState? previousKeyboardState;

	public Buttons[] recentButtonSequence = new Buttons[100];

	public long recentButtonSequencePosition;

	public DateTime recentButtonPressTime = DateTime.MinValue;

	public DateTime recentBlockPressTime = DateTime.MinValue;

	public Definitions.facing isFacing = Definitions.facing.right;

	public AnimationName AnimationStateCurrent;

	public AnimationName AnimationStatePrevious;

	public DateTime healthBoostTimeLastGiven = DateTime.MinValue;

	public float healthPercentage => health / healthMax;

	public FighterObjectProperties(FighterObject fo)
	{
		parent = fo;
		startingPosition = parent.getXYVector2();
	}

	public Vector2 getCenter()
	{
		return new Vector2(parent.X + parent.width / 2, parent.Y + parent.height / 2);
	}

	public void CountAttack(Definitions.FighterSpecialMoves attack, int hits)
	{
		if (hits <= 0)
		{
			return;
		}
		if (attack == Definitions.FighterSpecialMoves.nulll)
		{
			int num = 0;
			num++;
			return;
		}
		if (!HumanProfile.AttacksMade.ContainsKey(attack))
		{
			HumanProfile.AttacksMade.Add(attack, 0);
			HumanProfile.AttackLevels.Add(attack, 1);
		}
		HumanProfile.AttacksMade[attack] += hits;
		float num2 = 0.05f;
		if (parent.PROPERTIES.areWeHuman)
		{
			if (HumanProfile.AttacksMade[attack] > 1000 && HumanProfile.AttackLevels[attack] < 10)
			{
				HumanProfile.AttackLevels[attack] = 10;
				speedupAnimation(attack, num2 * 3f, 10);
			}
			else if (HumanProfile.AttacksMade[attack] > 750 && HumanProfile.AttackLevels[attack] < 9)
			{
				HumanProfile.AttackLevels[attack] = 9;
				speedupAnimation(attack, num2, 9);
			}
			else if (HumanProfile.AttacksMade[attack] > 500 && HumanProfile.AttackLevels[attack] < 8)
			{
				HumanProfile.AttackLevels[attack] = 8;
				speedupAnimation(attack, num2, 8);
			}
			else if (HumanProfile.AttacksMade[attack] > 250 && HumanProfile.AttackLevels[attack] < 7)
			{
				HumanProfile.AttackLevels[attack] = 7;
				speedupAnimation(attack, num2, 7);
			}
			else if (HumanProfile.AttacksMade[attack] > 175 && HumanProfile.AttackLevels[attack] < 6)
			{
				HumanProfile.AttackLevels[attack] = 6;
				speedupAnimation(attack, num2, 6);
			}
			else if (HumanProfile.AttacksMade[attack] > 100 && HumanProfile.AttackLevels[attack] < 5)
			{
				HumanProfile.AttackLevels[attack] = 5;
				speedupAnimation(attack, num2, 5);
			}
			else if (HumanProfile.AttacksMade[attack] > 75 && HumanProfile.AttackLevels[attack] < 4)
			{
				HumanProfile.AttackLevels[attack] = 4;
				speedupAnimation(attack, num2, 4);
			}
			else if (HumanProfile.AttacksMade[attack] > 50 && HumanProfile.AttackLevels[attack] < 3)
			{
				HumanProfile.AttackLevels[attack] = 3;
				speedupAnimation(attack, num2, 3);
			}
			else if (HumanProfile.AttacksMade[attack] > 25 && HumanProfile.AttackLevels[attack] < 2 && HumanProfile.AttacksMade[attack] >= 75)
			{
				HumanProfile.AttackLevels[attack] = 2;
				speedupAnimation(attack, num2, 2);
			}
		}
	}

	public void speedupAnimation(Definitions.FighterSpecialMoves attack, float increase, int level)
	{
		GraphicsManager.Message("Level up!\r\n\r\n" + attack.ToString() + " level " + level, 6, 0);
		switch (attack)
		{
		case Definitions.FighterSpecialMoves.chop:
			parent.animationPunching.FrameTime = parent.animationPunching.FrameTime * (1f - increase);
			break;
		case Definitions.FighterSpecialMoves.swing:
			parent.animationQuickPunching.FrameTime = parent.animationQuickPunching.FrameTime * (1f - increase);
			break;
		case Definitions.FighterSpecialMoves.rangedArrow:
		case Definitions.FighterSpecialMoves.Hadouken:
			break;
		}
	}

	public int GetLevelOf(Definitions.FighterSpecialMoves attack)
	{
		if (!HumanProfile.AttacksMade.ContainsKey(attack))
		{
			HumanProfile.AttacksMade.Add(attack, 0);
			HumanProfile.AttackLevels.Add(attack, 1);
		}
		return HumanProfile.AttackLevels[attack];
	}
}
