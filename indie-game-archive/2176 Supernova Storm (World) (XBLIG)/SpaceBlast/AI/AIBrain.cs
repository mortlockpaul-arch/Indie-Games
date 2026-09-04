using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceBlast.AI;

internal class AIBrain
{
	private Stack<AITask> m_AITasks = new Stack<AITask>();

	public AIPlayer Player;

	public readonly AIPersonality Personality;

	private bool m_Terminating;

	private AISkill m_Skill;

	public int TimeSlice => (TimeManager.FrameNumber + Player.PlayerID) % 10;

	public AIBrain(AIPlayer theplayer, AISkill skill)
	{
		Player = theplayer;
		m_Skill = skill;
		switch (skill)
		{
		case AISkill.VeryEasy:
			Personality = new AIPersonalityVeryEasy();
			break;
		case AISkill.Easy:
			Personality = new AIPersonalityEasy();
			break;
		case AISkill.Medium:
			Personality = new AIPersonalityMedium();
			break;
		case AISkill.Hard:
			Personality = new AIPersonalityHard();
			break;
		case AISkill.VeryHard:
			Personality = new AIPersonalityVeryHard();
			break;
		}
		Reset();
	}

	public void Terminate()
	{
		m_Terminating = true;
		while (m_AITasks.Count > 0)
		{
			Think();
		}
	}

	public void Think()
	{
		AITask newTask = null;
		if (m_AITasks.Peek().UpdateTask(out newTask, m_Terminating))
		{
			m_AITasks.Pop();
		}
		if (newTask != null)
		{
			m_AITasks.Push(newTask);
		}
	}

	public void Reset()
	{
		m_AITasks.Clear();
		m_AITasks.Push(new AITaskPlanSearch(this));
		m_Terminating = false;
	}

	public void DebugDumpStack()
	{
		string text = "Brain Stack:\n";
		foreach (AITask aITask in m_AITasks)
		{
			text = text + aITask.GetType().ToString() + "\n";
		}
		MainGame.DebugMsg = text;
	}

	public EAttackOrRetreat AttackOrRetreat(Player enemy)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Ship theShip = Player.TheShip;
		if (!enemy.IsActive || enemy.IsCloakActive)
		{
			return EAttackOrRetreat.NotApplicable;
		}
		Vector3 val = theShip.Position - enemy.TheShip.Position;
		if (((Vector3)(ref val)).Length() > (float)Personality.VisualRange * 1.25f)
		{
			return EAttackOrRetreat.NotApplicable;
		}
		if (Player.IsMegaDamageActive || Player.IsInvincibile || Player.IsCloakActive)
		{
			return EAttackOrRetreat.Attack;
		}
		if (Player.TheShip.Weapons.ActiveFrontWeapon.Ammo <= 0)
		{
			return EAttackOrRetreat.Retreat;
		}
		if (theShip.Strength < (float)Personality.StrengthRetreatThreshold || theShip.Shields < (float)Personality.ShieldRetreatThreshold)
		{
			return EAttackOrRetreat.Retreat;
		}
		if (enemy.IsPowerCut)
		{
			return EAttackOrRetreat.Attack;
		}
		return EAttackOrRetreat.Attack;
	}
}
