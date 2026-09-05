using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class fade_effect
{
	private enum TYPE
	{
		NORMAL,
		STRIPE,
		CROSS,
		WEIRD
	}

	private TYPE selected_fade;

	private Texture2D pixel;

	private Color col;

	private uint number_of_stripes;

	private bool vertical;

	private uint counter;

	private uint max_counter;

	private bool fade_in;

	public bool done
	{
		get
		{
			return counter > max_counter;
		}
		set
		{
			if (value)
			{
				counter = max_counter + 1;
			}
			else
			{
				counter = 0u;
			}
		}
	}

	public bool almost_done => counter == max_counter;

	public fade_effect(GraphicsDevice gfx_dev)
	{
		pixel = new Texture2D(gfx_dev, 1, 1);
		pixel.SetData(new Color[1] { Color.White });
		selected_fade = TYPE.NORMAL;
		fade_in = false;
	}

	~fade_effect()
	{
		pixel.Dispose();
	}

	public void to_color(uint frame_num, Color color, bool fadein = false)
	{
		selected_fade = TYPE.NORMAL;
		fade_in = fadein;
		counter = 0u;
		max_counter = frame_num;
		col = color;
	}

	public void stripe(uint frame_num, uint stripe_num, Color color, bool vert = false, bool fadein = false)
	{
		selected_fade = TYPE.STRIPE;
		fade_in = fadein;
		vertical = vert;
		number_of_stripes = stripe_num;
		counter = 0u;
		max_counter = frame_num;
		col = color;
	}

	public void cross(uint frame_num, uint stripe_num, Color color, bool fadein = false)
	{
		selected_fade = TYPE.CROSS;
		fade_in = fadein;
		number_of_stripes = stripe_num;
		counter = 0u;
		max_counter = frame_num;
		col = color;
	}

	public void weird(uint frame_num, Color color, bool fadein = false)
	{
		selected_fade = TYPE.WEIRD;
		fade_in = fadein;
		counter = 0u;
		max_counter = frame_num;
		col = color;
	}

	public void random(uint frame_num, Color col, bool fadein = false)
	{
		switch (game_state.random_gen.Next() % 3)
		{
		case 0:
			to_color(frame_num, col, fadein);
			break;
		case 1:
			stripe(frame_num, (uint)game_state.random_gen.Next() % 10u + 1, col, game_state.random_gen.Next() % 2 == 0, fadein);
			break;
		case 2:
			cross(frame_num, (uint)game_state.random_gen.Next() % 10u + 1, col, fadein);
			break;
		}
	}

	public void update()
	{
		if (counter <= max_counter)
		{
			counter++;
		}
	}

	private void draw_normal(SpriteBatch spr_batch)
	{
		float num = (float)counter / (float)max_counter;
		num = (fade_in ? (1f - num) : num);
		spr_batch.Draw(pixel, new Rectangle(0, 0, spr_batch.GraphicsDevice.Viewport.Width, spr_batch.GraphicsDevice.Viewport.Height), col * num);
	}

	private void draw_stripes(SpriteBatch spr_batch)
	{
		float num = (float)counter / (float)max_counter;
		num = (fade_in ? (1f - num) : num);
		int num2 = (int)(vertical ? (spr_batch.GraphicsDevice.Viewport.Width / number_of_stripes) : (spr_batch.GraphicsDevice.Viewport.Height / number_of_stripes));
		for (int i = 0; i < number_of_stripes; i++)
		{
			spr_batch.Draw(pixel, new Rectangle((int)(vertical ? ((float)(i * num2) + (float)(num2 / 2) * (1f - num)) : 0f), (int)(vertical ? 0f : ((float)(i * num2) + (float)(num2 / 2) * (1f - num))), (int)(vertical ? ((float)num2 * num) : ((float)spr_batch.GraphicsDevice.Viewport.Width)), (int)(vertical ? ((float)spr_batch.GraphicsDevice.Viewport.Height) : ((float)num2 * num))), col);
		}
	}

	private void draw_cross(SpriteBatch spr_batch)
	{
		vertical = false;
		draw_stripes(spr_batch);
		vertical = true;
		draw_stripes(spr_batch);
	}

	private void draw_weird(SpriteBatch spr_batch)
	{
		float num = (float)counter / (float)max_counter;
		num = (fade_in ? (1f - num) : num);
		int num2 = spr_batch.GraphicsDevice.Viewport.Width / 2;
		int num3 = spr_batch.GraphicsDevice.Viewport.Height / 2;
		spr_batch.Draw(pixel, new Rectangle(0, 0, (int)((float)num2 * num), 2 * num3), col);
		spr_batch.Draw(pixel, new Rectangle((int)((float)num2 * (2f - num)), 0, (int)((float)num2 * num), 2 * num3), col);
		spr_batch.Draw(pixel, new Rectangle(0, 0, num2 * 2, (int)(num * (float)num3)), col);
		spr_batch.Draw(pixel, new Rectangle(0, (int)((float)num3 * (2f - num)), num2 * 2, (int)(num * (float)num3)), col);
	}

	public void draw(SpriteBatch spr_batch)
	{
		if (!done)
		{
			switch (selected_fade)
			{
			case TYPE.NORMAL:
				draw_normal(spr_batch);
				break;
			case TYPE.STRIPE:
				draw_stripes(spr_batch);
				break;
			case TYPE.CROSS:
				draw_cross(spr_batch);
				break;
			case TYPE.WEIRD:
				draw_weird(spr_batch);
				break;
			}
		}
	}
}
