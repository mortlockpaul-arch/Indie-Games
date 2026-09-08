using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Slots;

public static class SuitSprite
{
	public static StillSprite SuitSkull;

	public static StillSprite SuitAcorns;

	public static StillSprite SuitClubs;

	public static StillSprite SuitHearts;

	public static StillSprite SuitBells;

	public static StillSprite SuitDiamonds;

	public static void LoadContent(ContentManager content)
	{
		Texture2D texture = content.Load<Texture2D>("Sprites\\Slots\\Suits");
		SuitSkull = new StillSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
		SuitAcorns = new StillSprite(texture, new Rectangle(64, 0, 64, 64), new Vector2(32f, 32f));
		SuitClubs = new StillSprite(texture, new Rectangle(128, 0, 64, 64), new Vector2(32f, 32f));
		SuitHearts = new StillSprite(texture, new Rectangle(192, 0, 64, 64), new Vector2(32f, 32f));
		SuitBells = new StillSprite(texture, new Rectangle(256, 0, 64, 64), new Vector2(32f, 32f));
		SuitDiamonds = new StillSprite(texture, new Rectangle(320, 0, 64, 64), new Vector2(32f, 32f));
	}

	public static Sprite GetSprite(SlotSuit suit)
	{
		return suit switch
		{
			SlotSuit.Skull => SuitSkull, 
			SlotSuit.Acorns => SuitAcorns, 
			SlotSuit.Clubs => SuitClubs, 
			SlotSuit.Hearts => SuitHearts, 
			SlotSuit.Bells => SuitBells, 
			SlotSuit.Diamonds => SuitDiamonds, 
			_ => throw new Exception("Unknown Suit: " + suit), 
		};
	}
}
