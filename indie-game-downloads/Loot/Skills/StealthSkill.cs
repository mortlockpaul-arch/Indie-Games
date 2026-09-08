using System.IO;
using Eyehook.Framework;

namespace Loot.Skills;

public class StealthSkill : Skill
{
	public override string Name => "Stealth";

	public override Sprite Sprite => SkillSprite.Stealth;

	public StealthSkill()
	{
	}

	public StealthSkill(BinaryReader reader)
		: base(reader)
	{
	}
}
