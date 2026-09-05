using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class clock
{
	private Texture2D img;

	private SpriteFont font;

	private Color fnt_col;

	private float zoom_fact;

	private int center_x;

	private int center_y;

	private bool countdown;

	private int time;

	public int seconds => time / 60;

	public int minutes => seconds / 60;

	public float zoom
	{
		get
		{
			return zoom_fact;
		}
		set
		{
			zoom_fact = ((value > 0f) ? value : zoom_fact);
		}
	}

	public clock(Texture2D image, SpriteFont fnt, int x_center, int y_center, Color font_col, int initial_time = 0, bool count_down = true)
	{
		img = image;
		font = fnt;
		fnt_col = font_col;
		zoom_fact = 1f;
		center_x = x_center;
		center_y = y_center;
		time = initial_time * 60;
		countdown = count_down;
	}

	public void update()
	{
		time += ((!countdown) ? 1 : (-1));
	}

	public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
	{
		spr_batch.Draw(img, new Rectangle((int)((float)dest_x - (float)center_x * zoom_fact), (int)((float)dest_y - (float)center_y * zoom_fact), (int)(zoom_fact * (float)img.Width), (int)(zoom_fact * (float)img.Height)), Color.White);
		string text = Convert.ToString(seconds);
		Vector2 vector = font.MeasureString(text);
		spr_batch.DrawString(font, text, new Vector2(dest_x, dest_y), fnt_col, 0f, new Vector2(vector.X / 2f, vector.Y / 2f), zoom_fact, SpriteEffects.None, 0f);
	}
}
