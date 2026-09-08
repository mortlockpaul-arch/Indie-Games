using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Slots;

public class SlotReel
{
	private SlotSuit[] Suits = new SlotSuit[4];

	private float spinTime;

	private float spinSpeed;

	private float offset;

	private bool spin;

	private static Vector2 rowOffset = new Vector2(0f, 82f);

	public bool IsSpinning => spin;

	public SlotSuit Value => Suits[2];

	public SlotReel()
	{
		for (int i = 0; i < 4; i++)
		{
			Suits[i] = RandomSuit();
		}
	}

	public void Update(GameTime gameTime)
	{
		if (!spin)
		{
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		spinTime -= num;
		offset += spinSpeed * num;
		if (!((double)offset < 82.0))
		{
			offset -= 82f;
			for (int num2 = 3; num2 > 0; num2--)
			{
				Suits[num2] = Suits[num2 - 1];
			}
			Suits[0] = RandomSuit();
			if (!((double)spinTime > 0.0))
			{
				offset = 0f;
				spin = false;
				SlotSounds.ReelStop();
			}
		}
	}

	public void Spin(int column)
	{
		offset = 0f;
		spinTime = (float)(1.0 + 0.5 * (double)column);
		spinSpeed = 500 + DM.Random.Next(100);
		spin = true;
	}

	public SlotSuit RandomSuit()
	{
		return (DM.Random.Next(100 + 3 * DM.Player.Stats.LCK) >= 25) ? ((SlotSuit)(DM.Random.Next(5) + 1)) : SlotSuit.Skull;
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 pos)
	{
		for (int i = 0; i < 4; i++)
		{
			Sprite sprite = SuitSprite.GetSprite(Suits[i]);
			Vector2 position = pos + i * rowOffset;
			position.Y += offset;
			sprite.Draw(spriteBatch, position);
		}
	}
}
