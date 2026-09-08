using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public class ArmorSprite
{
	public static ArmorSprite Rags;

	public static ArmorSprite Clothes;

	public static ArmorSprite Cloak;

	public static ArmorSprite Harness;

	public static ArmorSprite Leather;

	public static ArmorSprite Studded;

	public static ArmorSprite ChainMail;

	public static ArmorSprite SplintMail;

	public static ArmorSprite PlateMail;

	public static ArmorSprite FullPlate;

	public Sprite N;

	public Sprite S;

	public Sprite E;

	public Sprite W;

	private ArmorSprite(Texture2D texture)
	{
		N = new StillSprite(texture, new Rectangle(64, 0, 64, 64), new Vector2(32f, 32f));
		S = new StillSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
		E = new StillSprite(texture, new Rectangle(128, 0, 64, 64), new Vector2(32f, 32f));
		W = new StillSprite(texture, new Rectangle(192, 0, 64, 64), new Vector2(32f, 32f));
	}

	public static void LoadContent(ContentManager content)
	{
		Rags = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\Rags"));
		Clothes = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\Clothes"));
		Cloak = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\Cloak"));
		Harness = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\Harness"));
		Leather = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\Leather"));
		Studded = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\Studded"));
		ChainMail = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\ChainMail"));
		SplintMail = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\SplintMail"));
		PlateMail = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\PlateMail"));
		FullPlate = new ArmorSprite(content.Load<Texture2D>("Sprites\\Items\\Armor\\FullPlate"));
	}
}
