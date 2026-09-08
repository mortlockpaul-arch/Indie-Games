using System.IO;
using Eyehook.Framework;

namespace Loot.Skills;

public class ThornsSkill : Skill
{
	public override string Name => "Thorns";

	public override Sprite Sprite => SkillSprite.Thorns;

	public ThornsSkill()
	{
	}

	public ThornsSkill(BinaryReader reader)
		: base(reader)
	{
	}
}
