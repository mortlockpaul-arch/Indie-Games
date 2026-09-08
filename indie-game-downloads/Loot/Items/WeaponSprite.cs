using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public class WeaponSprite
{
	public static WeaponSprite Club;

	public static WeaponSprite Dagger;

	public static WeaponSprite Quarterstaff;

	public static WeaponSprite Shortsword;

	public static WeaponSprite Mace;

	public static WeaponSprite Longsword;

	public static WeaponSprite GreatAxe;

	public static WeaponSprite WarHammer;

	public static WeaponSprite Claymore;

	public static WeaponSprite Crook;

	public static WeaponSprite SpikedClub;

	public static WeaponSprite Kris;

	public static WeaponSprite Pike;

	public static WeaponSprite Dirk;

	public static WeaponSprite Scepter;

	public static WeaponSprite Sai;

	public static WeaponSprite SpikedHammer;

	public static WeaponSprite JeweledClaymore;

	public Sprite Sprite;

	public Sprite AttackSprite;

	private WeaponSprite(Texture2D texture)
	{
		Sprite = new StillSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
		AttackSprite = new StillSprite(texture, new Rectangle(0, 64, 64, 128), new Vector2(32f, 96f));
	}

	public static void LoadContent(ContentManager content)
	{
		Club = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Club"));
		Dagger = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Dagger"));
		Quarterstaff = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Quarterstaff"));
		Shortsword = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Shortsword"));
		Mace = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Mace"));
		Longsword = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Longsword"));
		GreatAxe = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\GreatAxe"));
		WarHammer = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\WarHammer"));
		Claymore = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Claymore"));
		Crook = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Crook"));
		SpikedClub = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\SpikedClub"));
		Kris = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Kris"));
		Pike = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Pike"));
		Dirk = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Dirk"));
		Scepter = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Scepter"));
		Sai = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\Sai"));
		SpikedHammer = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\SpikedHammer"));
		JeweledClaymore = new WeaponSprite(content.Load<Texture2D>("Sprites\\Items\\Weapon\\JeweledClaymore"));
	}
}
