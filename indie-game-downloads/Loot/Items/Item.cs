using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public abstract class Item : BinaryRW
{
	private bool cursed;

	private bool identified;

	private Color itemNameColor = new Color(255, 255, 255);

	private Color itemNameShadowColor = new Color(0, 0, 0);

	private static readonly Color shadowColor = new Color(0, 0, 0) * 0.5f;

	private static readonly Vector2 shadowOffset = new Vector2(4f, 4f);

	private static Type[] itemSortOrder = new Type[6]
	{
		typeof(Potion),
		typeof(Scroll),
		typeof(Armor),
		typeof(Weapon),
		typeof(Amulet),
		typeof(Ring)
	};

	public abstract string Name { get; }

	public abstract int Value { get; }

	public abstract Sprite Sprite { get; }

	public bool Cursed => cursed;

	public virtual bool Identified => identified;

	public Item()
	{
	}

	public Item(BinaryReader reader)
	{
		Read(reader);
	}

	public virtual void Curse()
	{
		cursed = true;
		identified = false;
	}

	public virtual void Identify()
	{
		identified = true;
	}

	public virtual bool onStep()
	{
		if (!DM.Player.AddItem(this))
		{
			return false;
		}
		DM.AddEffect(FXText.GetFX(DM.Player.Location, Name, itemNameColor, itemNameShadowColor));
		PlaySound.PickUp();
		return true;
	}

	public abstract void onActivate(int id);

	public virtual void Update(GameTime gameTime)
	{
		Sprite.Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 pos, float scale)
	{
		Sprite.Draw(spriteBatch, pos, Color.White, scale);
	}

	public void DrawWithShadow(SpriteBatch spriteBatch, Vector2 pos, Color color, float scale)
	{
		Sprite.Draw(spriteBatch, pos + shadowOffset * scale, shadowColor, scale);
		Sprite.Draw(spriteBatch, pos, color, scale);
	}

	public static int Comparator(Item a, Item b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return 1;
		}
		if (b == null)
		{
			return -1;
		}
		int num = int.MaxValue;
		int num2 = int.MaxValue;
		for (int i = 0; i < itemSortOrder.Length; i++)
		{
			if (a.GetType().IsSubclassOf(itemSortOrder[i]))
			{
				num = i;
			}
			if (b.GetType().IsSubclassOf(itemSortOrder[i]))
			{
				num2 = i;
			}
		}
		if (num != num2)
		{
			return num - num2;
		}
		if (a.Identified && !b.Identified)
		{
			return -1;
		}
		return (!a.Identified && b.Identified) ? 1 : a.Name.CompareTo(b.Name);
	}

	public virtual void Read(BinaryReader reader)
	{
		cursed = reader.ReadBoolean();
		identified = reader.ReadBoolean();
	}

	public virtual void Write(BinaryWriter writer)
	{
		writer.Write(Cursed);
		writer.Write(Identified);
	}
}
