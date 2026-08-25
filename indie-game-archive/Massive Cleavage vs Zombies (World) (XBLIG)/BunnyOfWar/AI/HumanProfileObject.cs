using System.Collections.Generic;
using System.Diagnostics;

namespace BunnyOfWar.AI;

public class HumanProfileObject
{
	public List<FighterManager.CauseOfDeath> VictimsCausesOfDeath = new List<FighterManager.CauseOfDeath>(100);

	public Dictionary<Definitions.FighterSpecialMoves, int> AttacksMade = new Dictionary<Definitions.FighterSpecialMoves, int>(10);

	public Dictionary<Definitions.FighterSpecialMoves, int> AttackLevels = new Dictionary<Definitions.FighterSpecialMoves, int>();

	public int damageDealt;

	public int damageTaken;

	public int healthRegenerated;

	public int kills;

	public int deaths;

	public int revivalsOfTeammate;

	public int shotsFired;

	public int shotsMade;

	public int shotsBlocked;

	public int blocks;

	public int parries;

	public int counters;

	public double timeSpentBlocking;

	public double timeSpentPlaying;

	public Stopwatch stopwatchTimeSpentPlaying = new Stopwatch();

	public Stopwatch stopwatchTimeSpentBlocking = new Stopwatch();

	public int pushes;

	public int hammerAttacks;

	private static int kMaxAttacksHistory = 100;

	public List<Definitions.FighterSpecialMoves> previousMoves = new List<Definitions.FighterSpecialMoves>(kMaxAttacksHistory);

	public void logAttack(Definitions.FighterSpecialMoves attack)
	{
		while (previousMoves.Count >= kMaxAttacksHistory)
		{
			previousMoves.Remove(previousMoves[0]);
		}
		previousMoves.Add(attack);
	}

	public bool isAttackBeingSpammed(Definitions.FighterSpecialMoves attack, int amount)
	{
		if (previousMoves.Count <= amount)
		{
			return false;
		}
		int num = previousMoves.Count - 1;
		while (num > 0 && num > previousMoves.Count - amount)
		{
			if (previousMoves[num] != attack)
			{
				return false;
			}
			num--;
		}
		return true;
	}
}
