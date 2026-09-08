using System.IO;
using Eyehook.Framework;

namespace Loot.Skills;

public class PerceptionSkill : Skill
{
	public override string Name => "Perception";

	public override Sprite Sprite => SkillSprite.Perception;

	public PerceptionSkill()
	{
	}

	public PerceptionSkill(BinaryReader reader)
		: base(reader)
	{
	}
}
