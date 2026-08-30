using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

public class Award
{
	public List<AwardData> Data;

	public ushort totalPoints = 0;

	private int ID = -1;

	private float delay = 0f;

	private float transp = 0f;

	private float transp2 = 0f;

	private Texture2D locked;

	public int selected = 0;

	public Award(string project, List<Texture2D> aT)
	{
		Data = new List<AwardData>(50);
		if (project != null && project == "DOTLC")
		{
			DOTLC_Awards(aT);
		}
	}

	public void DOTLC_Awards(List<Texture2D> aT)
	{
		int num = 0;
		locked = aT[0];
		num++;
		Add("100 down", "Eliminate 100 Starians", aT[num], aT[aT.Count - 1], 2u);
		num++;
		Add("1000 down", "Eliminate 1000 Starians", aT[num], aT[aT.Count - 1], 3u);
		num++;
		Add("10000 down", "Eliminate 10000 Starians", aT[num], aT[aT.Count - 1], 5u);
		num++;
		Add("Protector", "Finish a level\nwith the Colony unharmed", aT[num], aT[aT.Count - 1], 20u);
		num++;
		Add("100% Complete", "Finish the campaign mode\nwith both,\nEngineer and Fighter", aT[num], aT[aT.Count - 1], 30u);
		num++;
		Add("Cooperative", "Finish the campaign\nmode in cooperative mode", aT[num], aT[aT.Count - 1], 10u);
		num++;
		Add("Engineer", "Finish the campaign mode\nwith the Engineer", aT[num], aT[aT.Count - 1], 20u);
		num++;
		Add("Explorer", "Find every relic\nin the galaxy", aT[num], aT[aT.Count - 1], 10u);
		Add("Fighter", "Finish the campaign mode\nwith the Engineer", aT[num], aT[aT.Count - 1], 20u);
		num++;
		Add("Boss Killer", "Finish the final Boss", aT[num], aT[aT.Count - 1], 25u);
		num++;
		Add("Chubby Rain", "Unlock Chubby Rain mode", aT[num], aT[aT.Count - 1], 5u);
		num++;
		Add("insert", "insert", aT[num], aT[aT.Count - 1], 5u);
		num++;
		Add("Collector", "Get at least 4 relics\nwith any character", aT[num], aT[aT.Count - 1], 20u);
		num++;
		Add("Sidescroller", "Unlock Sidescroller mode", aT[num], aT[aT.Count - 1], 5u);
		num++;
		Add("Survivor", "Finish a level without receiving\nany damage", aT[num], aT[aT.Count - 1], 20u);
		for (int i = 0; i < Data.Count; i++)
		{
			totalPoints += (ushort)Data[i].points;
		}
	}

	public float getPercentage()
	{
		float num = 0f;
		for (int i = 0; i < Data.Count(); i++)
		{
			if (Data[i].unlocked)
			{
				num++;
			}
		}
		return num * (100f / (float)Data.Count());
	}

	public uint getPoints()
	{
		uint num = 0u;
		for (int i = 0; i < Data.Count(); i++)
		{
			if (Data[i].unlocked)
			{
				num += Data[i].points;
			}
		}
		return num;
	}

	public void Add(string name, string desc, Texture2D image, Texture2D locked, uint points)
	{
		Data.Add(new AwardData(name, desc, image, locked, points));
	}

	public bool Unlock(string name)
	{
		bool result = false;
		for (int i = 0; i < Data.Count(); i++)
		{
			if (Data[i].name == name && !Data[i].unlocked)
			{
				Data[i].unlocked = true;
				ID = i;
				delay = 40f;
				result = true;
			}
		}
		return result;
	}

	public bool isUnlock(string name)
	{
		bool result = false;
		for (int i = 0; i < Data.Count(); i++)
		{
			if (Data[i].name == name)
			{
				result = Data[i].unlocked;
			}
		}
		return result;
	}

	public bool isMouseOver(int i, Vector2 mouse)
	{
		int width = Data[i].image.Width;
		Rectangle rectangle = new Rectangle((int)(Data[i].pos.X - (float)(width / 2)), (int)(Data[i].pos.Y - (float)(width / 2)), width, width);
		Rectangle value = new Rectangle((int)(mouse.X - 1f), (int)(mouse.Y - 1f), 2, 2);
		return rectangle.Intersects(value);
	}

	public void Update()
	{
		if (delay > 0f)
		{
			delay -= 0.1f;
		}
		if (delay <= 0f)
		{
			transp = MathHelper.Lerp(transp, 0f, 0.1f);
		}
		else
		{
			transp = MathHelper.Lerp(transp, 1f, 0.1f);
		}
		if (transp < 0.9f)
		{
			transp2 = MathHelper.Lerp(transp, 0f, 0.1f);
		}
		else
		{
			transp2 = MathHelper.Lerp(transp, 1f, 0.1f);
		}
		if (delay <= 0f && transp <= 0f)
		{
			ID = -1;
		}
	}

	public void Draw(SpriteBatch sb, SpriteFont font, GraphicsDevice gd)
	{
		Vector2 vector = new Vector2(gd.Viewport.Width / 2, gd.Viewport.TitleSafeArea.Bottom);
		if (vector.Y > (float)(gd.Viewport.Height - 100))
		{
			vector.Y = gd.Viewport.Height - 100;
		}
		if (ID >= 0 && ID < Data.Count && transp > 0f)
		{
			sb.Begin();
			sb.Draw(locked, vector, null, Color.White * transp, 0f, new Vector2(locked.Width, locked.Height) / 2f, new Vector2(transp, 1f), SpriteEffects.None, (float)ID / 100f);
			sb.DrawString(font, "Award unlocked!", vector - new Vector2(0f, 12f), Color.White * transp * transp, 0f, font.MeasureString("Achievement Unlocked!") / 2f, new Vector2(transp, 1f) * 0.7f, SpriteEffects.None, 0.5f + (float)ID / 100f);
			sb.DrawString(font, Data[ID].name, vector + new Vector2(0f, 14f), Color.White * transp2 * transp, 0f, font.MeasureString(Data[ID].name) / 2f, new Vector2(transp, 1f) * 0.8f, SpriteEffects.None, 0.5f + (float)ID / 100f);
			sb.Draw(Data[ID].image, vector - new Vector2(172f, 0f), null, Color.White * transp2 * transp2 * transp, 0f, new Vector2(Data[ID].image.Width, Data[ID].image.Height) / 2f, 0.8f, SpriteEffects.None, 0.6f + (float)ID / 100f);
			sb.End();
		}
	}

	public void DrawDebug(SpriteBatch sb, SpriteFont font, SpriteFont MenuFont, GraphicsDevice gd, float amm)
	{
		sb.Begin();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < Data.Count; i++)
		{
			num++;
			if (num > 5)
			{
				num = 1;
				num2++;
			}
			Data[i].pos = new Vector2((float)gd.Viewport.Width * 0.98f - (float)(num * gd.Viewport.Width) * 0.115f, (float)gd.Viewport.Height * 0.355f + (float)(num2 * gd.Viewport.Height) * 0.156f);
			Texture2D image = Data[i].locked;
			if (Data[i].unlocked)
			{
				image = Data[i].image;
			}
			sb.Draw(image, Data[i].pos + Vector2.UnitX * (1f - amm) * 100f * i, null, Color.White * amm, 0f, new Vector2(image.Width, image.Height) / 2f, 1f, SpriteEffects.None, (float)i / 100f);
			sb.Draw(Data[i].image, Data[i].pos + Vector2.UnitX * (1f - amm) * 100f * i, null, Color.White * 0.25f * amm, 0f, new Vector2(image.Width, image.Height) / 2f, 1f, SpriteEffects.None, 0.1f + (float)i / 100f);
		}
		if (selected > Data.Count - 1)
		{
			selected -= Data.Count;
		}
		if (selected < 0)
		{
			selected += Data.Count;
		}
		sb.DrawString(MenuFont, Data[selected].name, new Vector2((float)gd.Viewport.Width * 0.062f, (float)gd.Viewport.Height * 0.24f), Color.LightCyan * amm);
		sb.DrawString(font, Data[selected].desc, new Vector2((float)gd.Viewport.Width * 0.06f, (float)gd.Viewport.Height * 0.35f), Color.LightCyan * amm, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
		sb.DrawString(font, "Points: " + Data[selected].points, new Vector2((float)gd.Viewport.Width * 0.305f, (float)gd.Viewport.Height * 0.755f), Color.LightCyan * amm, 0f, font.MeasureString("Points: " + Data[selected].points), 0.75f, SpriteEffects.None, 0f);
		sb.End();
	}
}
