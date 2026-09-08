using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Skills;

public class SkillSprite
{
	public static Sprite Regen;

	public static Sprite Frenzy;

	public static Sprite Freeze;

	public static Sprite Poison;

	public static Sprite Orb;

	public static Sprite Stealth;

	public static Sprite StatBoost;

	public static Sprite Perception;

	public static Sprite Thorns;

	public static Sprite None;

	public static void LoadContent(ContentManager content)
	{
		Texture2D texture = content.Load<Texture2D>("Sprites\\StatSkill\\SkillIcons");
		Regen = new StillSprite(texture, new Rectangle(0, 0, 64, 64), null);
		Frenzy = new StillSprite(texture, new Rectangle(64, 0, 64, 64), null);
		Freeze = new StillSprite(texture, new Rectangle(128, 0, 64, 64), null);
		Poison = new StillSprite(texture, new Rectangle(192, 0, 64, 64), null);
		Orb = new StillSprite(texture, new Rectangle(256, 0, 64, 64), null);
		Stealth = new StillSprite(texture, new Rectangle(0, 64, 64, 64), null);
		StatBoost = new StillSprite(texture, new Rectangle(64, 64, 64, 64), null);
		Perception = new StillSprite(texture, new Rectangle(128, 64, 64, 64), null);
		Thorns = new StillSprite(texture, new Rectangle(192, 64, 64, 64), null);
		None = new StillSprite(texture, new Rectangle(256, 64, 64, 64), null);
	}
}
