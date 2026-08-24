using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WorldAmbiance;

public class DialogScreen
{
	private string Title;

	private string Description;

	private Texture2D BgImage;

	private SpriteFont Font;

	private SpriteFont TitleFont;

	public Vector2 TitlePadding = new Vector2(175f, 15f);

	public Vector2 DescriptionPadding = new Vector2(175f, 30f);

	public Color TitleColor = Color.Black;

	public Color DescriptionColor = Color.Black;

	private List<string> descriptionLinesToPrint = new List<string>();

	private bool _descriptionLinesCalculated;

	public int rightPadding = 20;

	public DialogScreen(string title, string description, Texture2D bgImage, SpriteFont font, SpriteFont titleFont)
	{
		Title = title;
		Description = description;
		BgImage = bgImage;
		Font = font;
		TitleFont = titleFont;
	}

	private void GetDescriptionLines(float w)
	{
		if (_descriptionLinesCalculated)
		{
			return;
		}
		_descriptionLinesCalculated = true;
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < Description.Length; i++)
		{
			if (rightPadding != 0)
			{
				if (descriptionLinesToPrint.Count == 1)
				{
					rightPadding = 100;
				}
				else if (descriptionLinesToPrint.Count == 3)
				{
					rightPadding = 110;
				}
			}
			if (Description[i] == '\n')
			{
				int num4 = rightPadding;
				if (rightPadding == 0)
				{
					rightPadding = (int)DescriptionPadding.X;
				}
				if (Font.MeasureString(Description.Substring(num3, i - num3 + 1)).X > w - (DescriptionPadding.X + (float)rightPadding))
				{
					descriptionLinesToPrint.Add(Description.Substring(num3, num2 - num3));
					descriptionLinesToPrint.Add(Description.Substring(num2 + 1, i - num2));
				}
				else
				{
					descriptionLinesToPrint.Add(Description.Substring(num3, i - num3));
				}
				rightPadding = num4;
				num3 = i + 1;
				num = 0f;
				num2 = i;
			}
			else if (Description[i] == ' ')
			{
				float x = Font.MeasureString(Description.Substring(num2, i - num2)).X;
				num += x;
				int num5 = rightPadding;
				if (rightPadding == 0)
				{
					rightPadding = (int)DescriptionPadding.X;
				}
				if (num > w - (DescriptionPadding.X + (float)rightPadding))
				{
					descriptionLinesToPrint.Add(Description.Substring(num3, num2 - num3 + 1));
					num3 = num2 + 1;
					num = x;
					num2 = i + 1;
				}
				else
				{
					num2 = i;
				}
				rightPadding = num5;
			}
			if (i == Description.Length - 1)
			{
				int num6 = rightPadding;
				if (rightPadding == 0)
				{
					rightPadding = (int)DescriptionPadding.X;
				}
				if (Font.MeasureString(Description.Substring(num3, i - num3 + 1)).X > w - (DescriptionPadding.X + (float)rightPadding))
				{
					descriptionLinesToPrint.Add(Description.Substring(num3, num2 - num3 + 1));
					descriptionLinesToPrint.Add(Description.Substring(num2 + 1, i - num2));
				}
				else
				{
					descriptionLinesToPrint.Add(Description.Substring(num3, i - num3 + 1));
				}
				rightPadding = num6;
			}
		}
	}

	public void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
	{
		Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
		float num = 1f;
		float num2 = (float)BgImage.Width * num;
		float num3 = (float)BgImage.Height * num;
		Rectangle destinationRectangle = new Rectangle((int)((float)titleSafeArea.Center.X - num2 / 2f), (int)((float)titleSafeArea.Center.Y - num3 / 2f), (int)num2, (int)num3);
		GetDescriptionLines(num2);
		spriteBatch.Begin();
		spriteBatch.Draw(BgImage, destinationRectangle, Color.White);
		spriteBatch.DrawString(TitleFont, Title, new Vector2((float)destinationRectangle.X + TitlePadding.X, (float)destinationRectangle.Y + TitlePadding.Y), TitleColor);
		float num4 = (float)destinationRectangle.Y + DescriptionPadding.Y;
		for (int i = 0; i < descriptionLinesToPrint.Count; i++)
		{
			num4 += Font.MeasureString(Title).Y;
			if (rightPadding != 0)
			{
				switch (i)
				{
				case 2:
					destinationRectangle.X += 80;
					break;
				case 4:
					destinationRectangle.X += 10;
					break;
				}
			}
			spriteBatch.DrawString(Font, descriptionLinesToPrint[i], new Vector2((float)destinationRectangle.X + DescriptionPadding.X, num4), DescriptionColor);
		}
		spriteBatch.End();
	}
}
