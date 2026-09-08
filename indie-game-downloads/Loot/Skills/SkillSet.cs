using System.IO;

namespace Loot.Skills;

public class SkillSet
{
	public FreezeSkill Freeze;

	public FrenzySkill Frenzy;

	public RegenSkill Regen;

	public OrbSkill Orb;

	public PoisonSkill Poison;

	public StealthSkill Stealth;

	public StatBoostSkill StatBoost;

	public PerceptionSkill Perception;

	public ThornsSkill Thorns;

	public SkillSet()
	{
		Freeze = new FreezeSkill();
		Frenzy = new FrenzySkill();
		Regen = new RegenSkill();
		Orb = new OrbSkill();
		Poison = new PoisonSkill();
		Stealth = new StealthSkill();
		StatBoost = new StatBoostSkill();
		Perception = new PerceptionSkill();
		Thorns = new ThornsSkill();
	}

	public SkillSet(BinaryReader reader)
	{
		Read(reader);
	}

	private void Read(BinaryReader reader)
	{
		Freeze = new FreezeSkill(reader);
		Frenzy = new FrenzySkill(reader);
		Regen = new RegenSkill(reader);
		Orb = new OrbSkill(reader);
		Poison = new PoisonSkill(reader);
		Stealth = new StealthSkill(reader);
		StatBoost = new StatBoostSkill(reader);
		Perception = new PerceptionSkill(reader);
		Thorns = new ThornsSkill(reader);
	}

	public void Write(BinaryWriter writer)
	{
		Freeze.Write(writer);
		Frenzy.Write(writer);
		Regen.Write(writer);
		Orb.Write(writer);
		Poison.Write(writer);
		Stealth.Write(writer);
		StatBoost.Write(writer);
		Perception.Write(writer);
		Thorns.Write(writer);
	}
}
